using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Hercules.MA.ServicioExterno.Controllers.Utilidades;
using Hercules.MA.ServicioExterno.Models;
using Hercules.MA.ServicioExterno.Models.Graficas.DataGraficaPublicaciones;
using Hercules.MA.ServicioExterno.Models.Graficas.DataItemRelacion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Hercules.MA.ServicioExterno.Controllers.Acciones
{
    public class AccionesPublicaciones
    {
        #region --- Constantes     
        private static string RUTA_OAUTH = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config/ConfigOAuth/OAuthV3.config";
        private static ResourceApi mResourceApi = new ResourceApi(RUTA_OAUTH);
        private static CommunityApi mCommunityApi = new CommunityApi(RUTA_OAUTH);
        private static Guid mIdComunidad = mCommunityApi.GetCommunityId();
        private static string RUTA_PREFIJOS = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Models/JSON/prefijos.json";
        private static string mPrefijos = string.Join(" ", JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(RUTA_PREFIJOS)));
        private static string COLOR_GRAFICAS = "#6cafe3";
        private static string COLOR_GRAFICAS_HORIZONTAL = "#6cafe3"; //#1177ff
        #endregion


        /// <summary>
        /// Obtiene los datos para crear la gráfica de las publicaciones.
        /// </summary>
        /// <param name="pParametros">Filtros aplicados</param>
        /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        public DataGraficaPublicaciones GetDatosGraficaPublicaciones(string pParametros)
        {
            Dictionary<string, int> dicResultados = new Dictionary<string, int>();
            SparqlObject resultadoQuery = null;
            string select = $@"  {mPrefijos}
                                SELECT ?fecha COUNT(DISTINCT(?documento)) AS ?NumPublicaciones ";
            int aux = 0;
            string where = $@"  WHERE {{
                                    ?documento dct:issued ?fechaAux.
                                    {UtilidadesAPI.CrearFiltros(UtilidadesAPI.ObtenerParametros(pParametros), "?documento", ref aux)}
                                    BIND((?fechaAux/10000000000) as ?fecha)
                                }}ORDER BY ?fecha";

            resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    string fechaPublicacion = UtilidadesAPI.GetValorFilaSparqlObject(fila, "fecha");
                    int numPublicaciones = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "NumPublicaciones"));
                    dicResultados.Add(fechaPublicacion, numPublicaciones);
                }
            }

            // Rellenar, agrupar y ordenar los años.
            if (dicResultados != null && dicResultados.Count > 0)
            {
                int anioIni = int.Parse(dicResultados.First().Key);
                int anioFin = int.Parse(dicResultados.Last().Key);
                for (int i = anioFin; i < anioFin; i++)
                {
                    if (!dicResultados.ContainsKey(i.ToString()))
                    {
                        dicResultados.Add(i.ToString(), 0);
                    }
                }
                dicResultados = dicResultados.OrderBy(item => item.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
            }

            // Contruir el objeto de la gráfica.
            List<string> listaColores = UtilidadesAPI.CrearListaColores(dicResultados.Count, COLOR_GRAFICAS);
            Datasets datasets = new Datasets("Publicaciones", UtilidadesAPI.GetValuesList(dicResultados), listaColores, listaColores, 1);
            Models.Graficas.DataGraficaPublicaciones.Data data = new Models.Graficas.DataGraficaPublicaciones.Data(UtilidadesAPI.GetKeysList(dicResultados), new List<Datasets> { datasets });
            Options options = new Options(new Scales(new Y(true)), new Plugins(new Title(true, "Evolución temporal publicaciones"), new Legend(new Labels(true), "top", "end")));
            DataGraficaPublicaciones dataGrafica = new DataGraficaPublicaciones("bar", data, options);

            return dataGrafica;
        }


        /// <summary>
        /// Obtienes los datos de las pestañas de cada sección de la ficha.
        /// </summary>
        /// <param name="pPersona">Nombre de la persona.</param>
        /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        public Dictionary<string, int> GetDatosCabeceraDocumento(string pDocumento)
        {
            string idGrafoBusqueda = UtilidadesAPI.ObtenerIdBusqueda(mResourceApi, pDocumento);
            Dictionary<string, int> dicResultados = new Dictionary<string, int>();
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder();
            String where = "";

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT count(DISTINCT ?s ) as ?numRelacionados");
            where = $@"
               WHERE
               {{
                  FILTER(?item = <{idGrafoBusqueda}>)
                  ?item  <http://w3id.org/roh/hasKnowledgeArea> ?areaConocimiento.
                  ?areaConocimiento <http://w3id.org/roh/categoryNode> ?id_areaConocimiento.
                  ?s <http://w3id.org/roh/hasKnowledgeArea> ?areaConocimiento2.  
                  ?areaConocimiento2 <http://w3id.org/roh/categoryNode> ?id_areaConocimiento
                  OPTIONAL {{
                        ?s <http://vivoweb.org/ontology/core#freeTextKeyword> ?etiquetasRelacionadosDocumento.
                       ?item <http://vivoweb.org/ontology/core#freeTextKeyword> ?etiquetasRelacionadosDocumento.
                  }}
                  FILTER(?s != ?item)
               }} ";

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where, mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    int numRelacionados = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numRelacionados"));
                    if(numRelacionados > 20)
                    {
                        numRelacionados = 20;
                    }
                    dicResultados.Add("numRelacionados", numRelacionados);
                }
            }
            return dicResultados;
        }
    }
}
