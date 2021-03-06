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
            Dictionary<string, Dictionary<string, int>> dicResultados = new Dictionary<string, Dictionary<string, int>>();
            HashSet<string> listacuartiles = new HashSet<string>();
            {
                string select = $@"  {mPrefijos}
                                SELECT ?fecha ?cuartil COUNT(DISTINCT(?documento)) AS ?NumPublicaciones ";
                int aux = 0;
                string where = $@"  WHERE {{
                                    ?documento a 'document'.                                    
                                    {UtilidadesAPI.CrearFiltros(UtilidadesAPI.ObtenerParametros(pParametros), "?documento", ref aux)}
                                    {{?documento dct:issued ?fechaAux.
                                    BIND(xsd:int(?fechaAux/10000000000) as ?fecha)}}
                                    OPTIONAL{{?documento <http://w3id.org/roh/quartile> ?cuartil}}
                                }}ORDER BY ?fecha";

                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                {
                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                    {
                        string fechaPublicacion = UtilidadesAPI.GetValorFilaSparqlObject(fila, "fecha");
                        string cuartil = UtilidadesAPI.GetValorFilaSparqlObject(fila, "cuartil");
                        if (string.IsNullOrEmpty(cuartil))
                        {
                            cuartil = "";
                        }
                        if(cuartil != "1" && cuartil != "2" && cuartil != "3" && cuartil != "4")
                        {
                            cuartil = "";
                        }
                        listacuartiles.Add(cuartil);
                        int numPublicaciones = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "NumPublicaciones"));
                        if (!dicResultados.ContainsKey(fechaPublicacion))
                        {
                            dicResultados.Add(fechaPublicacion, new Dictionary<string, int>());
                        }
                        if (dicResultados[fechaPublicacion].ContainsKey(cuartil))
                        {
                            dicResultados[fechaPublicacion][cuartil] += numPublicaciones;
                        }
                        else
                        {
                            dicResultados[fechaPublicacion].Add(cuartil, numPublicaciones);
                        }
                    }
                }
            }

            Dictionary<string, int> dicResultadosCitasAnio = new Dictionary<string, int>();
            {
                string select = $@"  {mPrefijos}
                                SELECT ?fecha SUM(?numCitas) AS ?numCitas ";
                int aux = 0;
                string where = $@"  WHERE {{
                                    ?documento a 'document'.
                                    {UtilidadesAPI.CrearFiltros(UtilidadesAPI.ObtenerParametros(pParametros), "?documento", ref aux)}
                                    {{?documento dct:issued ?fechaAux.
                                    BIND(xsd:int(?fechaAux/10000000000) as ?fecha)}}
                                    ?documento <http://w3id.org/roh/citationCount> ?numCitas
                                }}ORDER BY ?fecha";

                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                {
                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                    {
                        string fechaPublicacion = UtilidadesAPI.GetValorFilaSparqlObject(fila, "fecha");
                        int numCitas = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numCitas"));
                        dicResultadosCitasAnio[fechaPublicacion] = numCitas;
                    }
                }
            }

            // Rellenar, agrupar y ordenar los años cuartiles y cita.
            if (dicResultados != null && dicResultados.Count > 0)
            {
                int anioIni = int.Parse(dicResultados.First().Key);
                int anioFin = int.Parse(dicResultados.Last().Key);
                if(int.Parse(dicResultadosCitasAnio.First().Key)< anioIni)
                {
                    anioIni = int.Parse(dicResultadosCitasAnio.First().Key);
                }
                if (int.Parse(dicResultadosCitasAnio.Last().Key) > anioFin)
                {
                    anioFin = int.Parse(dicResultadosCitasAnio.Last().Key);
                }
                for (int i = anioFin; i < anioFin; i++)
                {
                    if (!dicResultados.ContainsKey(i.ToString()))
                    {
                        dicResultados.Add(i.ToString(), new Dictionary<string, int>());
                    }
                    if (!dicResultadosCitasAnio.ContainsKey(i.ToString()))
                    {
                        dicResultadosCitasAnio.Add(i.ToString(), 0);
                    }
                }
                dicResultados = dicResultados.OrderBy(item => item.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
                dicResultadosCitasAnio = dicResultadosCitasAnio.OrderBy(item => item.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
            }

            // Rellenar, agrupar y ordenar los años citas.
            if (dicResultados != null && dicResultados.Count > 0)
            {
                int anioIni = int.Parse(dicResultados.First().Key);
                int anioFin = int.Parse(dicResultados.Last().Key);
                for (int i = anioFin; i < anioFin; i++)
                {
                    if (!dicResultados.ContainsKey(i.ToString()))
                    {
                        dicResultados.Add(i.ToString(), new Dictionary<string, int>());
                    }
                }
                dicResultados = dicResultados.OrderBy(item => item.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
            }

            // Contruir el objeto de la gráfica.
            Models.Graficas.DataGraficaPublicaciones.Data data = new Models.Graficas.DataGraficaPublicaciones.Data(dicResultados.Keys.ToList(), new List<Datasets>());
            //Construimos un dataset para las citas
            Datasets datasetCitas = new Datasets("Citas", dicResultadosCitasAnio.Values.ToList(), UtilidadesAPI.CrearListaColores(dicResultados.Count, "#333333"), UtilidadesAPI.CrearListaColores(dicResultados.Count, "#000000"), 1, null, "line", "y2");
            data.datasets.Add(datasetCitas);
            //Construimos un dataset por cada cuartil
            int num = 0;
            Dictionary<string, string> colores = new Dictionary<string, string>();
            colores.Add("1", "#45DCB4");
            colores.Add("2", "#EAF112");
            colores.Add("3", "#DE921E");
            colores.Add("4", "#DC4545");
            foreach (string cuartil in listacuartiles.OrderBy(CompareCuartil))
            {
                string nombre = "Cuartil " + cuartil;
                if (string.IsNullOrEmpty(cuartil))
                {
                    nombre = "Sin cuartil";
                }
                string color = "";
                if (colores.ContainsKey(cuartil))
                {
                    color = colores[cuartil];
                }
                else
                {
                    // color = "#e4f5fd";
                    color = "#6cafd3";
                }
                List<string> listaColores = UtilidadesAPI.CrearListaColores(dicResultados.Count, color);
                List<int> valores = new List<int>();
                foreach (string anio in dicResultados.Keys)
                {
                    if (dicResultados[anio].ContainsKey(cuartil))
                    {
                        valores.Add(dicResultados[anio][cuartil]);
                    }
                    else
                    {
                        valores.Add(0);
                    }
                }
                Datasets dataset = new Datasets(nombre, valores, listaColores, listaColores, 1, "cuartil", "bar", "y1");
                data.datasets.Add(dataset);
                num++;
            }
            

            Options options = new Options(new Scales(new Y(true)), new Plugins(new Title(true, "Evolución temporal publicaciones"), new Legend(new Labels(true), "top", "end")));
            DataGraficaPublicaciones dataGrafica = new DataGraficaPublicaciones("bar", data, options);

            return dataGrafica;
        }

        public int CompareCuartil(string pCuartil)
        {
            int cuartil = 0;
            if (pCuartil == "")
            {
                cuartil = 9999;
            }
            else
            {
                int.TryParse(pCuartil, out cuartil);
            }
            return cuartil;
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
                    if (numRelacionados > 20)
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
