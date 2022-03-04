using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Hercules.MA.ServicioExterno.Models.Cluster;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        #region Métodos de recolección de datos

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
