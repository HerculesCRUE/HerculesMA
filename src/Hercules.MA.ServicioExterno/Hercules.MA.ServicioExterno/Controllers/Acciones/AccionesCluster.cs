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
using static Hercules.MA.ServicioExterno.Models.Cluster.Cluster;

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
        public Dictionary<string, List<ThesaurusItem>> GetListThesaurus(string listadoCluster)
        {

            List<string> thesaurusTypes = new List<string>() { "researcharea" };

            try
            {
                if (listadoCluster != "")
                {
                    thesaurusTypes = JsonConvert.DeserializeObject<List<string>>(listadoCluster);
                }
            }
            catch (Exception e) { throw new Exception("El texto que ha introducido no corresponde a un json válido"); }

            var thesaurus = GetTesauros(thesaurusTypes);

            return thesaurus;
        }


        /// <summary>
        /// Método público para guardar / editar un cluster
        /// </summary>
        /// <param name="pIdGnossUser">Identificador del usuario</param>
        /// <param name="cluster">Objecto cluster</param>
        /// <returns>Diccionario con las listas de thesaurus.</returns>
        public string SaveCluster(string pIdGnossUser, Models.Cluster.Cluster cluster)
        {
            string idRecurso = cluster.entityID;
            int MAX_INTENTOS = 10;
            bool uploadedR = false;

            // Obtener el id del usuario usando el id de la cuenta
            string select = "select ?s ";
            string where = @$"where {{
                    ?s a <http://xmlns.com/foaf/0.1/Person>.
                    ?s <http://w3id.org/roh/gnossUser> ?idGnoss.
                    FILTER(?idGnoss = <http://gnoss/{pIdGnossUser.ToUpper()}>)
                }}";
            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "person");
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
                        IdsRdf_member = e.users.Select(x=>x.userID).ToList(),
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
                    ComplexOntologyResource resource = cRsource.ToGnossApiResource(mResourceApi, null, new Guid(recursoSplit[recursoSplit.Length - 2]), new Guid(recursoSplit[recursoSplit.Length - 1]));
                    int numIntentos = 0;
                    while (!resource.Modified)
                    {
                        numIntentos++;
                        if (numIntentos > MAX_INTENTOS)
                        {
                            break;
                        }

                        mResourceApi.ModifyComplexOntologyResource(resource, false, false);
                        uploadedR = resource.Modified;
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
            }
            else
            {
                throw new Exception("Recurso no creado");
            }
        }

        /// <summary>
        /// Método público para obtener los datos de un cluster
        /// </summary>
        /// <param name="pIdClusterId">Identificador del cluster</param>
        /// <returns>Diccionario con las listas de thesaurus.</returns>
        internal Models.Cluster.Cluster LoadCluster(string pIdClusterId)
        {

            // Obtener el id del usuario usando el id de la cuenta
            string select = "select ?p ?o ";
            string where = @$"where {{
                    ?s a 'cluster'.
                    ?s ?p ?o.
                    FILTER(?s = <http://gnoss/{pIdClusterId.ToUpper()}>)
                }}";
            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

            // Inicizalizamos el modelo del Cluster para devolver
            Models.Cluster.Cluster pDataCluster = new();
            pDataCluster.terms = new();
            pDataCluster.profiles = new();

            // Lista de los ids de los perfiles devuelto por la consulta
            List<string> perfiles = new();

            sparqlObject.results.bindings.ForEach(e =>
            {
                pDataCluster.entityID = @$"http://gnoss/{pIdClusterId.ToUpper()}";

                switch (e["p"].value) {
                    case "http://w3id.org/roh/title":
                        pDataCluster.name = e["o"].value;
                        break;
                    case "http://vivoweb.org/ontology/core#description":
                        pDataCluster.description = e["o"].value;
                        break;
                    case "http://w3id.org/roh/hasKnowledgeArea":
                        pDataCluster.terms.Add(e["o"].value);
                        break;
                    case "http://w3id.org/roh/clusterPerfil":
                        perfiles.Add(e["o"].value);
                        break;
                }
            });



            // Obtenemos todos los datos de las areas temáticas
            if (pDataCluster.terms.Count > 0)
            {
                pDataCluster.terms = LoadCurrentTerms(pDataCluster.terms);

            }


            // Obtenemos todos los datos de los perfiles
            foreach (string p in perfiles)
            {
                select = "select ?p ?o ";
                where = @$"where {{
                    ?s a <http://w3id.org/roh/ClusterPerfil>.
                    ?s ?p ?o.
                    FILTER(?s = <{p}>)
                }}";
                sparqlObject = mResourceApi.VirtuosoQuery(select, where, "cluster");

                PerfilCluster perfilCluster = new();
                perfilCluster.tags = new();
                perfilCluster.terms = new();
                perfilCluster.users = new();

                // Carga los datos en el objeto
                sparqlObject.results.bindings.ForEach(e =>
                {

                    switch (e["p"].value)
                    {
                        case "http://w3id.org/roh/title":
                            perfilCluster.name = e["o"].value;
                            break;
                        case "http://vivoweb.org/ontology/core#freeTextKeyword":
                            perfilCluster.tags.Add(e["o"].value);
                            break;
                        case "http://w3id.org/roh/hasKnowledgeArea":
                            perfilCluster.terms.Add(e["o"].value);
                            break;
                        case "http://www.w3.org/1999/02/22-rdf-syntax-ns#member":
                            perfilCluster.users.Add(new PerfilCluster.UserCluster()
                            {
                                userID = e["o"].value
                            });
                            break;
                    }
                });

                perfilCluster.terms = LoadCurrentTerms(perfilCluster.terms);

                // Añade el perfil creado a los datos del cluster
                pDataCluster.profiles.Add(perfilCluster);
            }

            return pDataCluster;
        }

        public Dictionary<string, Dictionary<string, ScoreCluster>> LoadProfiles(Models.Cluster.Cluster pDataCluster, List<string> pPersons)
        {
            //ID persona/ID perfil/score
            Dictionary<string, Dictionary<string, ScoreCluster>> respuesta = new Dictionary<string, Dictionary<string, ScoreCluster>>();

            List<string> filtrosPerfiles = new List<string>();
            foreach (PerfilCluster perfilCluster in pDataCluster.profiles)
            {
                string filtroCategorias = "";
                if (perfilCluster.terms != null && perfilCluster.terms.Count > 0)
                {
                    filtroCategorias = $@"  {{
					                            ?doc <http://w3id.org/roh/hasKnowledgeArea> ?area.
					                            ?area <http://w3id.org/roh/categoryNode> ?node.
					                            FILTER(?node in(<{string.Join(">,<", perfilCluster.terms)}>))
				                            }}";
                }
                string filtroTags = "";
                if (perfilCluster.tags != null && perfilCluster.tags.Count > 0)
                {
                    filtroTags = $@"   {{
				                            ?doc <http://vivoweb.org/ontology/core#freeTextKeyword>  ?keywordO.
                                            ?keywordO <http://w3id.org/roh/title> ?tag.
				                            FILTER(?tag in('{string.Join("','", perfilCluster.tags)}'))
				                        }}";
                }
                string union = "";
                if(!string.IsNullOrEmpty(filtroCategorias) && !string.IsNullOrEmpty(filtroTags))
                {
                    union = "UNION";
                }
                string filtroPerfil = $@"   {{
                                                BIND('{perfilCluster.entityID}' as ?perfil)
                                                {filtroCategorias}
                                                {union}
                                                {filtroTags}
                                            }}";
                filtrosPerfiles.Add(filtroPerfil);

                foreach(string person in pPersons)
                {
                    if(!respuesta.ContainsKey(person))
                    {
                        respuesta.Add(person,new Dictionary<string, ScoreCluster>());
                    }
                    if (!respuesta[person].ContainsKey(perfilCluster.entityID))
                    {
                        respuesta[person].Add(perfilCluster.entityID, new ScoreCluster());
                    }
                }
            }

            string select = "select ?person ?perfil (count(distinct ?node) + count(distinct ?tag)) as ?scoreAux ";
            string where = @$"where {{
                    ?doc a 'document'.
                    ?doc <http://w3id.org/roh/isValidated> 'true'.
				    ?doc <http://purl.org/ontology/bibo/authorList> ?authorList.
				    ?authorList <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
				    ?person a 'person'.
                    ?person <http://w3id.org/roh/isActive> 'true'.
                    FILTER(?person in (<http://gnoss/{string.Join(">,<http://gnoss/", pPersons.Select(x => x.ToUpper()))}>))
                    {string.Join("UNION", filtrosPerfiles)}
                }}";
            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

            foreach (Dictionary<string, SparqlObject.Data> fila in sparqlObject.results.bindings)
            {
                string person = fila["person"].value.Replace("http://gnoss/","").ToLower();
                string perfil = fila["perfil"].value;                
                PerfilCluster perfilCluster = pDataCluster.profiles.FirstOrDefault(x => x.entityID == perfil);
                float scoreAux = float.Parse(fila["scoreAux"].value);
                float scoreMax = 0;
                if(perfilCluster.tags!=null)
                {
                    scoreMax += perfilCluster.tags.Count;
                }
                if (perfilCluster.terms != null)
                {
                    scoreMax += perfilCluster.terms.Count;
                }
                float scoreAjuste = scoreAux / scoreMax;
                respuesta[person][perfil].ajuste = scoreAjuste;
            }

            select = "select ?person ?perfil (count(distinct ?doc)) as ?numDoc ";
            where = @$"where {{
                    ?doc a 'document'.
                    ?doc <http://w3id.org/roh/isValidated> 'true'.
				    ?doc <http://purl.org/ontology/bibo/authorList> ?authorList.
				    ?authorList <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
				    ?person a 'person'.
                    ?person <http://w3id.org/roh/isActive> 'true'.
                    FILTER(?person in (<http://gnoss/{string.Join(">,<http://gnoss/", pPersons.Select(x => x.ToUpper()))}>))
                    {string.Join("UNION", filtrosPerfiles)}
                }}";
            sparqlObject = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

            foreach (Dictionary<string, SparqlObject.Data> fila in sparqlObject.results.bindings)
            {
                string person = fila["person"].value.Replace("http://gnoss/", "").ToLower();
                string perfil = fila["perfil"].value;
                int numDoc = int.Parse(fila["numDoc"].value);
                respuesta[person][perfil].numPublicaciones = numDoc;
            }

            select = "select ?person (count(distinct ?doc)) as ?numDoc ";
            where = @$"where {{
                    ?doc a 'document'.
                    ?doc <http://w3id.org/roh/isValidated> 'true'.
				    ?doc <http://purl.org/ontology/bibo/authorList> ?authorList.
				    ?authorList <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
				    ?person a 'person'.
                    ?person <http://w3id.org/roh/isActive> 'true'.
                    FILTER(?person in (<http://gnoss/{string.Join(">,<http://gnoss/", pPersons.Select(x => x.ToUpper()))}>))
                }}";
            sparqlObject = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

            foreach (Dictionary<string, SparqlObject.Data> fila in sparqlObject.results.bindings)
            {
                string person = fila["person"].value.Replace("http://gnoss/", "").ToLower();
                int numDoc = int.Parse(fila["numDoc"].value);
                foreach(string perfil in respuesta[person].Keys)
                {
                    respuesta[person][perfil].numPublicacionesTotal = numDoc;
                }
            }
            foreach(string idperson in respuesta.Keys.ToList())
            {
                respuesta[idperson] = respuesta[idperson].OrderByDescending(x => x.Value.ajuste).ToDictionary(x => x.Key, x => x.Value);
            }
            return respuesta;
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
            string where = $"WHERE {{ ?s a <http://purl.org/ontology/bibo/Document>. ?s <http://vivoweb.org/ontology/core#freeTextKeyword> ?freeTextKeyword. ?freeTextKeyword <http://w3id.org/roh/title> ?o. FILTER( {filter} )    }} ORDER BY ?o";
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


        private List<string> LoadCurrentTerms(List<string> terms)
        {

            string termsTxt = String.Join(',', terms.Select(e => "<" + e + ">"));

            string select = "select ?o";
            string where = @$"where {{
                ?s a <http://w3id.org/roh/CategoryPath>.
                ?s <http://w3id.org/roh/categoryNode> ?o.
                FILTER(?s IN ({termsTxt}))
            }}";
            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "cluster");

            List<string> termsRes = new();

            sparqlObject.results.bindings.ForEach(e =>
            {
                termsRes.Add(e["o"].value);
            });




            return termsRes;
        }


        /// <summary>
        /// Obtiene la categoría padre.
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
