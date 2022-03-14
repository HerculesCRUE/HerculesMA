using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using Hercules.MA.ServicioExterno.Models.Cluster;
using ClusterOntology;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hercules.MA.ServicioExterno.Controllers.Acciones
{
    public class AccionesCluster
    {
        #region --- Constantes   
        private static string RUTA_OAUTH = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config/ConfigOAuth/OAuthV3.config";
        private static ResourceApi mResourceApi = new ResourceApi(RUTA_OAUTH);
        private static CommunityApi mCommunityApi = new CommunityApi(RUTA_OAUTH);
        private static Guid mIdComunidad = mCommunityApi.GetCommunityId();
        private static string RUTA_PREFIJOS = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Models/JSON/prefijos.json";
        private static string mPrefijos = string.Join(" ", JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(RUTA_PREFIJOS)));
        #endregion

        /// <summary>
        /// Método público que obtiene una lista de thesaurus.
        /// </summary>
        /// <param name="listadoCluster">Listado de thesaurus a obtener.</param>
        /// <returns>Diccionario con las listas de thesaurus.</returns>
        public Dictionary<string, List<ThesaurusItem>> GetListThesaurus (string listadoCluster)
        {

            List<string> thesaurusTypes = new List<string>() { "researcharea" };

            try
            {
                if (listadoCluster != "")
                {
                    thesaurusTypes = JsonConvert.DeserializeObject<List<string>>(listadoCluster);
                }
            } catch (Exception e) { throw new Exception("El texto que ha introducido no corresponde a un json válido"); }

            var thesaurus = GetTesauros(thesaurusTypes);

            return thesaurus;
        }


        /// <summary>
        /// Método público para guardar / editar la información básica (paso 1) de los cluster
        /// </summary>
        /// <param name="listadoCluster">Listado de thesaurus a obtener.</param>
        /// <returns>Diccionario con las listas de thesaurus.</returns>
        public string SaveStep1Cluster (string pIdGnossUser, Models.Cluster.Cluster cluster)
        {
            string idRecurso = cluster.entityID;
            int MAX_INTENTOS = 10;
            bool uploadedR = false;

            // Obtener el id del usuario usando el id de la cuenta
            string select = "select ?s ";
            string where = @$"where {{
                    ?s a 'person'.
                    ?s <http://w3id.org/roh/gnossUser> ?idGnoss.
                    FILTER(?idGnoss = <http://gnoss/{pIdGnossUser.ToUpper()}>)
                }}";
            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);
            var userGnossId = string.Empty;
            sparqlObject.results.bindings.ForEach(e =>
            {
                userGnossId = e["s"].value;
            });

            if (!string.IsNullOrEmpty(userGnossId))
            {
                // Creando el objeto del cluster
                // Creando las categorías
                List<CategoryPath> categorias = new List<CategoryPath>();
                categorias.Add(new CategoryPath() { IdsRoh_categoryNode = cluster.terms });

                List<ClusterPerfil> listClusterPerfil = new();
                // Creando los perfiles del cluster
                if (cluster.profiles != null)
                {
                    listClusterPerfil = cluster.profiles.Select(e => new ClusterPerfil()
                    {
                        Roh_title = e.name,
                        Roh_hasKnowledgeArea = new List<CategoryPath>() { new CategoryPath() { IdsRoh_categoryNode = e.terms } },
                        IdsRdf_member = e.users,
                        Vivo_freeTextKeyword = e.tags
                    }).ToList();
                }
                // creando los cluster
                ClusterOntology.Cluster cRsource = new();
                cRsource.IdRdf_member = userGnossId;
                cRsource.Roh_title = cluster.name;
                cRsource.Vivo_description = cluster.description;
                cRsource.Roh_hasKnowledgeArea = categorias;
                cRsource.Roh_clusterPerfil = listClusterPerfil.ToList();
                cRsource.Dct_issued = DateTime.Now;

                mResourceApi.ChangeOntoly("cluster");

                if (idRecurso != null && idRecurso != "")
                {
                    string[] recursoSplit = idRecurso.Split('_');

                    // Modificación.
                    ComplexOntologyResource resource = cRsource.ToGnossApiResource(mResourceApi, null,new Guid(recursoSplit[recursoSplit.Length-2]), new Guid(recursoSplit[recursoSplit.Length-1]));
                    int numIntentos = 0;
                    while (!resource.Modified)
                    {
                        numIntentos++;
                        if (numIntentos > MAX_INTENTOS)
                        {
                            break;
                        }
                        
                        mResourceApi.ModifyComplexOntologyResource(resource,false,false);
                    }

                }
                else
                {
                    // Inserción.
                    ComplexOntologyResource resource = cRsource.ToGnossApiResource(mResourceApi, null);
                    int numIntentos = 0;
                    while (!resource.Uploaded)
                    {
                        numIntentos++;
                        if (numIntentos > MAX_INTENTOS)
                        {
                            break;
                        }
                        idRecurso = mResourceApi.LoadComplexSemanticResource(resource, true, true);
                        uploadedR = resource.Uploaded;
                    }
                }
            }

            if (uploadedR)
            {
                return idRecurso;
            } else
            {
                throw new Exception("Recurso no creado");
            }
            return idRecurso;
        }

        /// <summary>
        /// Método público buscar diferentes tags
        /// </summary>
        /// <param name="pSearch">parámetro que corresponde a la cadena de búsqueda.</param>
        /// <returns>Listado de etiquetas de resultado.</returns>
        public List<string> SearchTags(string pSearch)
        {
            int numMax = 20;
            string searchText = pSearch.Trim();
            string filter = "";
            if (!pSearch.EndsWith(' '))
            {
                string[] splitSearch = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (splitSearch.Length > 1)
                {
                    searchText = searchText.Substring(0, searchText.LastIndexOf(' '));
                    if (splitSearch.Last().Length > 3)
                    {
                        searchText += " " + splitSearch.Last() + "*";
                    }
                    else
                    {
                        filter = $" AND lcase(?o) like \"% { splitSearch.Last() }%\" ";
                    }
                }
                else if (searchText.Length > 3)
                {
                    searchText += "*";
                }
                else // Si tiene menos de 4 caracteres y no termina en espacio, buscamos por like
                {
                    filter = $"  lcase(?o) like \"{ searchText }%\" OR lcase(?o) like \"% { searchText }%\" ";
                    searchText = "";
                }
            }
            if (searchText != "")
            {
                filter = $"bif:contains(?o, \"'{ searchText }'\"){filter}";
            }
            string select = "SELECT DISTINCT ?s ?o ";
            string where = $"WHERE {{ ?s a <http://purl.org/ontology/bibo/Document>. ?s <http://vivoweb.org/ontology/core#freeTextKeyword> ?o . FILTER( {filter} )    }} ORDER BY ?o";
            SparqlObject sparqlObjectAux = mResourceApi.VirtuosoQuery(select, where, "document");
            List<string> resultados = sparqlObjectAux.results.bindings.Select(x => x["o"].value).Distinct().ToList();
            if (resultados.Count() > numMax)
            {
                resultados = resultados.ToList().GetRange(0, numMax);
            }
            return resultados;
        }


        #region Métodos de recolección de datos

        /// <summary>
        /// Método privado para obtener los tesauros.
        /// </summary>
        /// <param name="pListaTesauros">Listado de thesaurus a obtener.</param>
        /// <returns>Diccionario con las listas de thesaurus.</returns>
        private Dictionary<string, List<ThesaurusItem>> GetTesauros(List<string> pListaTesauros)
        {
            Dictionary<string, List<ThesaurusItem>> elementosTesauros = new Dictionary<string, List<ThesaurusItem>>();

            foreach (string tesauro in pListaTesauros)
            {
                string select = "select * ";
                string where = @$"where {{
                    ?s a <http://www.w3.org/2008/05/skos#Concept>.
                    ?s <http://www.w3.org/2008/05/skos#prefLabel> ?nombre.
                    ?s <http://purl.org/dc/elements/1.1/source> '{tesauro}'
                    OPTIONAL {{ ?s <http://www.w3.org/2008/05/skos#broader> ?padre }}
                }} ORDER BY ?padre ?s ";
                SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "taxonomy");

                List<ThesaurusItem> items = sparqlObject.results.bindings.Select(x => new ThesaurusItem()
                {
                    id = x["s"].value,
                    name = x["nombre"].value,
                    parentId = x.ContainsKey("padre") ? x["padre"].value : ""
                }).ToList();

                elementosTesauros.Add(tesauro, items);
            }

            return elementosTesauros;
        }





        /// <summary>
        /// Obtiene la ctaegoría padre.
        /// </summary>
        /// <param name="pIdTesauro">Categoría a consultar.</param>
        /// <returns>Categoría padre.</returns>
        private string ObtenerIdTesauro(string pIdTesauro)
        {
            string idTesauro = pIdTesauro.Split(new[] { "researcharea_" }, StringSplitOptions.None)[1];
            int num1 = Int32.Parse(idTesauro.Split('.')[0]);
            int num2 = Int32.Parse(idTesauro.Split('.')[1]);
            int num3 = Int32.Parse(idTesauro.Split('.')[2]);
            int num4 = Int32.Parse(idTesauro.Split('.')[3]);

            if (num4 != 0)
            {
                idTesauro = $@"{mResourceApi.GraphsUrl}items/researcharea_{num1}.{num2}.{num3}.0";
            }
            else if (num3 != 0 && num4 == 0)
            {
                idTesauro = $@"{mResourceApi.GraphsUrl}items/researcharea_{num1}.{num2}.0.0";
            }
            else if (num2 != 0 && num3 == 0 && num4 == 0)
            {
                idTesauro = $@"{mResourceApi.GraphsUrl}items/researcharea_{num1}.0.0.0";
            }

            return idTesauro;
        }

        /// <summary>
        /// Obtiene las categorías del tesáuro.
        /// </summary>
        /// <returns>Tupla con (clave) diccionario de las idCategorias-idPadre y (valor) diccionario de nombreCategoria-idCategoria.</returns>
        private static Tuple<Dictionary<string, string>, Dictionary<string, string>> ObtenerDatosTesauro()
        {
            Dictionary<string, string> dicAreasBroader = new Dictionary<string, string>();
            Dictionary<string, string> dicAreasNombre = new Dictionary<string, string>();

            string select = @"SELECT DISTINCT * ";
            string where = @$"WHERE {{
                ?concept a <http://www.w3.org/2008/05/skos#Concept>.
                ?concept <http://www.w3.org/2008/05/skos#prefLabel> ?nombre.
                ?concept <http://purl.org/dc/elements/1.1/source> 'researcharea'
                OPTIONAL{{?concept <http://www.w3.org/2008/05/skos#broader> ?broader}}
                }}";
            SparqlObject resultado = mResourceApi.VirtuosoQuery(select, where, "taxonomy");

            foreach (Dictionary<string, SparqlObject.Data> fila in resultado.results.bindings)
            {
                string concept = fila["concept"].value;
                string nombre = fila["nombre"].value;
                string broader = "";
                if (fila.ContainsKey("broader"))
                {
                    broader = fila["broader"].value;
                }
                dicAreasBroader.Add(concept, broader);
                if (!dicAreasNombre.ContainsKey(nombre.ToLower()))
                {
                    dicAreasNombre.Add(nombre.ToLower(), concept);
                }
            }

            Dictionary<string, string> dicAreasUltimoNivel = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> item in dicAreasNombre)
            {
                bool tieneHijos = false;
                string id = item.Value.Split(new[] { "researcharea_" }, StringSplitOptions.None)[1];
                int num1 = Int32.Parse(id.Split('.')[0]);
                int num2 = Int32.Parse(id.Split('.')[1]);
                int num3 = Int32.Parse(id.Split('.')[2]);
                int num4 = Int32.Parse(id.Split('.')[3]);

                if (num2 == 0 && num3 == 0 && num4 == 0)
                {
                    tieneHijos = dicAreasNombre.ContainsValue($@"{mResourceApi.GraphsUrl}items/researcharea_{num1}.1.0.0");
                }
                else if (num3 == 0 && num4 == 0)
                {
                    tieneHijos = dicAreasNombre.ContainsValue($@"{mResourceApi.GraphsUrl}items/researcharea_{num1}.{num2}.1.0");
                }
                else if (num4 == 0)
                {
                    tieneHijos = dicAreasNombre.ContainsValue($@"{mResourceApi.GraphsUrl}items/researcharea_{num1}.{num2}.{num3}.1");
                }

                if (!tieneHijos)
                {
                    dicAreasUltimoNivel.Add(item.Key, item.Value);
                }
            }

            return new Tuple<Dictionary<string, string>, Dictionary<string, string>>(dicAreasBroader, dicAreasUltimoNivel);
        }

        #endregion


    }
}
