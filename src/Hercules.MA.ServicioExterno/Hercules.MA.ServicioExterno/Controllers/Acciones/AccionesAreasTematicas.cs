using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Hercules.MA.ServicioExterno.Controllers.Utilidades;
using Hercules.MA.ServicioExterno.Models.Graficas.DataGraficaAreasTags;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.MA.ServicioExterno.Controllers.Acciones
{
    public class AccionesAreasTematicas
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
        /// Obtiene el objeto para crear la gráfica de áreas temáticas
        /// </summary>
        /// <param name="pId">ID del elemento en cuestión.</param>
        /// <param name="pType">Tipo del elemento.</param>
        /// <returns>Objeto que se trata en JS para contruir la gráfica.</returns>
        public DataGraficaAreasTags DatosGraficaAreasTematicas(string pId, string pType)
        {
            string idGrafoBusqueda = UtilidadesAPI.ObtenerIdBusqueda(mResourceApi, pId);
            string filtroElemento = "";
            switch(pType)
            {
                case "group":
                    filtroElemento = $@"?documento roh:isProducedBy <{idGrafoBusqueda}>.";
                    break;
                case "person":
                    filtroElemento = $@"?documento roh:publicAuthorList <{idGrafoBusqueda}>.";
                    break;
                default:
                    throw new Exception("No hay configuración para el tipo '"+ pType+"'");
                    break;
            }

            Dictionary<string, int> dicResultados = new Dictionary<string, int>();
            int numDocumentos = 0;
            {
                //Nº de documentos por categoría
                SparqlObject resultadoQuery = null;
                string select = $"{mPrefijos} Select ?nombreCategoria count(distinct ?documento) as ?numCategorias";
                string where = $@"  where
                                {{
                                    ?documento a 'document'. 
                                    {filtroElemento}
                                    ?documento roh:hasKnowledgeArea ?area.
                                    ?area roh:categoryNode ?categoria.
                                    ?categoria skos:prefLabel ?nombreCategoria.
                                    MINUS
                                    {{
                                        ?categoria skos:narrower ?hijos
                                    }}
                                }}
                                Group by(?nombreCategoria)";

                resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                {
                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                    {
                        string nombreCategoria = UtilidadesAPI.GetValorFilaSparqlObject(fila, "nombreCategoria");
                        int numCategoria = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numCategorias"));
                        dicResultados.Add(nombreCategoria, numCategoria);
                    }
                }
            }
            {
                //Nº total de documentos
                SparqlObject resultadoQuery = null;
                string select = $"{mPrefijos} Select count(distinct ?documento) as ?numDocumentos";
                string where = $@"  where
                                {{
                                    ?documento a 'document'. 
                                    {filtroElemento}
                                }}";

                resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                {
                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                    {
                        numDocumentos = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numDocumentos"));
                    }
                }
            }

            //Ordenar diccionario
            var dicionarioOrdenado = dicResultados.OrderByDescending(x => x.Value);

            Dictionary<string, double> dicResultadosPorcentaje = new Dictionary<string, double>();
            foreach (KeyValuePair<string, int> item in dicionarioOrdenado)
            {
                double porcentaje = Math.Round((double)(100 * item.Value) / numDocumentos, 2);
                dicResultadosPorcentaje.Add(item.Key, porcentaje);
            }

            // Contruir el objeto de la gráfica.
            List<string> listaColores = UtilidadesAPI.CrearListaColores(dicionarioOrdenado.Count(), "#6cafe3");
            Datasets datasets = new Datasets(dicResultadosPorcentaje.Values.ToList(), listaColores);
            Models.Graficas.DataGraficaAreasTags.Data data = new Models.Graficas.DataGraficaAreasTags.Data(dicResultadosPorcentaje.Keys.ToList(), new List<Datasets> { datasets });

            // Máximo.
            x xAxes = new x(new Ticks(0, 100), new ScaleLabel(true, "Percentage"));

            Options options = new Options("y", new Plugins(new Title(true, "Resultados de la investigación por fuente de datos"), new Legend(false)), new Scales(xAxes));
            DataGraficaAreasTags dataGrafica = new DataGraficaAreasTags("bar", data, options);

            return dataGrafica;
        }
    }
}
