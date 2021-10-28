using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Hercules.MA.ServicioExterno.Controllers.Utilidades;
using Hercules.MA.ServicioExterno.Models.DataGraficaColaboradores;
using Hercules.MA.ServicioExterno.Models.DataGraficaPublicaciones;
using Hercules.MA.ServicioExterno.Models.DataGraficaPublicacionesHorizontal;
using Hercules.MA.ServicioExterno.Models.DataQueryRelaciones;
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
    public class AccionesProyecto
    {
        #region --- Constantes     
        private static string RUTA_OAUTH = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config\configOAuth\OAuthV3.config";
        private static string RUTA_PREFIJOS = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Models\JSON\prefijos.json";
        private static string COLOR_GRAFICAS = "#6cafe3";
        private static string COLOR_GRAFICAS_HORIZONTAL = "#1177ff";
        #endregion

        private readonly ResourceApi mResourceApi;
        private readonly CommunityApi mCommunityApi;
        private readonly Guid mIdComunidad;
        private readonly string mPrefijos;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccionesProyecto()
        {
            mResourceApi = new ResourceApi(RUTA_OAUTH);
            mCommunityApi = new CommunityApi(RUTA_OAUTH);
            mIdComunidad = mCommunityApi.GetCommunityId();
            mPrefijos = string.Join(" ", JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(RUTA_PREFIJOS)));
        }

        /// <summary>
        /// Obtienes los datos de las pestañas de cada sección de la ficha.
        /// </summary>
        /// <param name="pProyecto">ID del recurso del proyecto.</param>
        /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        public Dictionary<string, int> GetDatosCabeceraProyecto(string pProyecto)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pProyecto);
            Dictionary<string, int> dicResultados = new Dictionary<string, int>();
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT COUNT(DISTINCT ?persona) AS ?NumParticipantes COUNT(DISTINCT ?documento) AS ?NumPublicaciones COUNT(DISTINCT ?nombreCategoria) AS ?NumCategorias ");
            where.Append("WHERE {{ "); // Total de Participantes.
            where.Append($@"<{idGrafoBusqueda}> vivo:relates ?relacion. ");
            where.Append("?relacion roh:roleOf ?persona. ");
            where.Append("} UNION { "); // Total Publicaciones.
            where.Append($@"?documento roh:project <{idGrafoBusqueda}>. ");
            where.Append("} UNION { "); // Total Categorías.
            where.Append($@"?documento roh:project <{idGrafoBusqueda}>. ");
            where.Append("?documento roh:hasKnowledgeArea ?area. ");
            where.Append("?area roh:categoryNode ?categoria. ");
            where.Append("?categoria skos:prefLabel ?nombreCategoria. ");
            where.Append("}} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    int numParticipantes = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "NumParticipantes"));
                    int numPublicaciones = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "NumPublicaciones"));
                    int numCategorias = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "NumCategorias"));
                    dicResultados.Add("Participantes", numParticipantes);
                    dicResultados.Add("Publicaciones", numPublicaciones);
                    dicResultados.Add("Categorias", numCategorias);
                }
            }

            return dicResultados;
        }

        /// <summary>
        /// Obtiene los datos para crear el grafo de relaciones con otros investigadores.
        /// </summary>
        /// <param name="pIdProyecto">ID del recurso del proyecto.</param>
        /// <param name="pParametros">Filtros aplicados en las facetas.</param>
        /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        public List<DataGraficaColaboradores> GetDatosGraficaRedColaboradores(string pIdProyecto, string pParametros)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdProyecto);
            Dictionary<string, string> dicNodos = new Dictionary<string, string>();
            Dictionary<string, DataQueryRelaciones> dicRelaciones = new Dictionary<string, DataQueryRelaciones>();
            SparqlObject resultadoQuery = null;
            StringBuilder select = null, where = null;

            #region --- Proyecto de la ficha
            // Consulta sparql.
            select = new StringBuilder(mPrefijos);
            select.Append("SELECT ?titulo ?proyecto AS ?id ");
            where = new StringBuilder("WHERE { ");
            where.Append("?proyecto roh:title ?titulo. ");
            where.Append($@"FILTER(?proyecto = <{idGrafoBusqueda}>) ");
            where.Append("} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    string id = UtilidadesAPI.GetValorFilaSparqlObject(fila, "id");
                    string nombreProyecto = UtilidadesAPI.GetValorFilaSparqlObject(fila, "titulo");
                    if (!dicNodos.ContainsKey(id))
                    {
                        dicNodos.Add(id, nombreProyecto.ToLower().Trim());
                    }
                }
            }
            #endregion

            #region --- Obtener todas las personas que hayan colaborado en el proyecto.            
            string personas = $@"<{idGrafoBusqueda}>";

            // Consulta sparql.
            select = new StringBuilder(mPrefijos);
            select.Append("SELECT ?nombre ?persona AS ?id ");
            where = new StringBuilder("WHERE { ");
            where.Append($@"<{idGrafoBusqueda}> vivo:relates ?relacion. ");
            where.Append("?relacion roh:roleOf ?persona. ");
            where.Append("?persona foaf:name ?nombre. ");
            where.Append("} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    string id = UtilidadesAPI.GetValorFilaSparqlObject(fila, "id");
                    string nombreColaborador = UtilidadesAPI.GetValorFilaSparqlObject(fila, "nombre");
                    if (!dicNodos.ContainsKey(id))
                    {
                        dicNodos.Add(id, nombreColaborador.ToLower().Trim());
                        personas += ",<" + UtilidadesAPI.GetValorFilaSparqlObject(fila, "id") + ">";
                    }
                }
            }
            #endregion

            #region --- Creación de las relaciones.
            KeyValuePair<string, string> proyecto = dicNodos.First();
            foreach (KeyValuePair<string, string> item in dicNodos)
            {
                if (item.Key != proyecto.Key)
                {
                    dicRelaciones.Add(item.Key, new DataQueryRelaciones(new List<Datos> { new Datos(proyecto.Key, 1) }));
                }
            }
            #endregion

            // Construcción del objeto de la gráfica.            
            List<DataGraficaColaboradores> colaboradores = new List<DataGraficaColaboradores>();
            int maximasRelaciones = 1;

            // Nodos. 
            if (dicNodos != null && dicNodos.Count > 0)
            {
                foreach (KeyValuePair<string, string> nodo in dicNodos)
                {
                    string clave = nodo.Key;
                    string valor = UtilsCadenas.ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(nodo.Value);
                    Models.DataGraficaColaboradores.Data data = new Models.DataGraficaColaboradores.Data(clave, valor, null, null, null, "nodes");
                    DataGraficaColaboradores dataColabo = new DataGraficaColaboradores(data, true, true);
                    colaboradores.Add(dataColabo);
                }
            }

            // Relaciones.
            if (dicRelaciones != null && dicRelaciones.Count > 0)
            {
                foreach (KeyValuePair<string, DataQueryRelaciones> sujeto in dicRelaciones)
                {
                    foreach (Datos relaciones in sujeto.Value.idRelacionados)
                    {
                        relaciones.numVeces = 1;
                        string id = $@"{sujeto.Key}~{relaciones.idRelacionado}~{relaciones.numVeces}";
                        Models.DataGraficaColaboradores.Data data = new Models.DataGraficaColaboradores.Data(id, null, sujeto.Key, relaciones.idRelacionado, CalcularGrosor(maximasRelaciones, relaciones.numVeces), "edges");
                        DataGraficaColaboradores dataColabo = new DataGraficaColaboradores(data, null, null);
                        colaboradores.Add(dataColabo);
                    }
                }
            }

            return colaboradores;

        }

        /// <summary>
        /// Obtiene los datos para crear la gráfica de las publicaciones.
        /// </summary>
        /// <param name="pIdProyecto">ID del recurso del proyecto.</param>
        /// <param name="pParametros">Filtros aplicados en las facetas.</param>
        /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        public DataGraficaPublicaciones GetDatosGraficaPublicaciones(string pIdProyecto, string pParametros)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdProyecto);
            Dictionary<string, int> dicResultados = new Dictionary<string, int>(); // Dictionary<año, numDocumentos>
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT ?fecha COUNT(DISTINCT(?documento)) AS ?NumPublicaciones ");
            where.Append("WHERE { ");
            where.Append($@"?documento roh:project <{idGrafoBusqueda}>. ");
            where.Append("?documento dct:issued ?fecha. ");
            if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = ObtenerParametros(pParametros);
                string filtros = CrearFiltros(dicParametros, "?documento", ref aux);
                where.Append(filtros);
            }
            where.Append("} ");
            where.Append("ORDER BY ?fecha ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

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
                RellenarAnys(dicResultados, dicResultados.First().Key, dicResultados.Last().Key);
                dicResultados = AgruparAnys(dicResultados);
                dicResultados = dicResultados.OrderBy(item => item.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
            }

            // Contruir el objeto de la gráfica.
            List<string> listaColores = CrearListaColores(dicResultados.Count, COLOR_GRAFICAS);
            Models.DataGraficaPublicaciones.Datasets datasets = new Models.DataGraficaPublicaciones.Datasets("Publicaciones", GetValuesList(dicResultados), listaColores, listaColores, 1);
            Models.DataGraficaPublicaciones.Data data = new Models.DataGraficaPublicaciones.Data(GetKeysList(dicResultados), new List<Models.DataGraficaPublicaciones.Datasets> { datasets });
            Models.DataGraficaPublicaciones.Options options = new Models.DataGraficaPublicaciones.Options(new Models.DataGraficaPublicaciones.Scales(new Y(true)), new Models.DataGraficaPublicaciones.Plugins(new Models.DataGraficaPublicaciones.Title(true, "Evolución temporal publicaciones"), new Models.DataGraficaPublicaciones.Legend(new Labels(true), "top", "end")));
            DataGraficaPublicaciones dataGrafica = new DataGraficaPublicaciones("bar", data, options);

            return dataGrafica;
        }

        /// <summary>
        /// Obtiene los datos para crear la gráfica de las publicaciones (horizontal).
        /// </summary>
        /// <param name="pIdProyecto">ID del recurso del proyecto.</param>
        /// <param name="pParametros">Filtros aplicados en las facetas.</param>
        /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        public DataGraficaPublicacionesHorizontal GetDatosGraficaPublicacionesHorizontal(string pIdProyecto, string pParametros)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdProyecto);
            Dictionary<string, int> dicResultados = new Dictionary<string, int>();
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT ?nombreCategoria COUNT(?nombreCategoria) AS ?numCategorias ");
            where.Append("WHERE { ");
            where.Append($@"?documento roh:project <{idGrafoBusqueda}>. ");
            where.Append("?documento dct:issued ?fecha. ");
            where.Append("?documento roh:hasKnowledgeArea ?area. ");
            where.Append("?area roh:categoryNode ?categoria. ");
            where.Append("?categoria skos:prefLabel ?nombreCategoria. ");
            if (!string.IsNullOrEmpty(pParametros))
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = ObtenerParametros(pParametros);
                string filtros = CrearFiltros(dicParametros, "?documento", ref aux);
                where.Append(filtros);
            }
            where.Append("} ");
            where.Append("GROUP BY (?nombreCategoria) ORDER BY DESC (?numCategorias) ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    string nombreCategoria = UtilidadesAPI.GetValorFilaSparqlObject(fila, "nombreCategoria");
                    int numCategoria = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numCategorias"));
                    dicResultados.Add(nombreCategoria, numCategoria);
                }
            }

            // Calculo del porcentaje.
            int numTotalCategorias = 0;
            foreach (KeyValuePair<string, int> item in dicResultados)
            {
                numTotalCategorias += item.Value;
            }
            Dictionary<string, double> dicResultadosPorcentaje = new Dictionary<string, double>();
            foreach (KeyValuePair<string, int> item in dicResultados)
            {
                double porcentaje = Math.Round((double)(100 * item.Value) / numTotalCategorias, 2);
                dicResultadosPorcentaje.Add(item.Key, porcentaje);
            }

            // Contruir el objeto de la gráfica.
            List<string> listaColores = CrearListaColores(dicResultados.Count, COLOR_GRAFICAS_HORIZONTAL);
            Models.DataGraficaPublicacionesHorizontal.Datasets datasets = new Models.DataGraficaPublicacionesHorizontal.Datasets(dicResultadosPorcentaje.Values.ToList(), listaColores);
            Models.DataGraficaPublicacionesHorizontal.Data data = new Models.DataGraficaPublicacionesHorizontal.Data(dicResultadosPorcentaje.Keys.ToList(), new List<Models.DataGraficaPublicacionesHorizontal.Datasets> { datasets });

            // Máximo.
            x xAxes = new x(new Ticks(0, 100), new ScaleLabel(true, "Percentage"));

            Models.DataGraficaPublicacionesHorizontal.Options options = new Models.DataGraficaPublicacionesHorizontal.Options("y", new Models.DataGraficaPublicacionesHorizontal.Plugins(new Models.DataGraficaPublicacionesHorizontal.Title(true, "Resultados de la investigación por fuente de datos"), new Models.DataGraficaPublicacionesHorizontal.Legend(false)), new Models.DataGraficaPublicacionesHorizontal.Scales(xAxes));
            DataGraficaPublicacionesHorizontal dataGrafica = new DataGraficaPublicacionesHorizontal("bar", data, options);

            return dataGrafica;
        }

        #region --- Utilidades
        /// <summary>
        /// Permite calcular el valor del ancho de la línea según el número de colaboraciones que tenga el nodo.
        /// </summary>
        /// <param name="pMax">Valor máximo.</param>
        /// <param name="pColabo">Número de colaboraciones.</param>
        /// <returns>Ancho de la línea en formate double.</returns>
        private double CalcularGrosor(int pMax, int pColabo)
        {
            return Math.Round(((double)pColabo / (double)pMax) * 10, 2);
        }

        /// <summary>
        /// Obtiene los filtros por los parámetros de la URL.
        /// </summary>
        /// <param name="pParametros">String de filtros.</param>
        /// <returns>Diccionario de filtros.</returns>
        private Dictionary<string, List<string>> ObtenerParametros(string pParametros)
        {
            if (!string.IsNullOrEmpty(pParametros))
            {
                Dictionary<string, List<string>> dicFiltros = new Dictionary<string, List<string>>();

                // Agregamos al diccionario los filtros.
                foreach (string filtro in pParametros.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string keyFiltro = filtro.Split('=')[0];
                    string valorFiltro = filtro.Split('=')[1];
                    if (dicFiltros.ContainsKey(keyFiltro))
                    {
                        dicFiltros[keyFiltro].Add(valorFiltro);
                    }
                    else
                    {
                        dicFiltros.Add(keyFiltro, new List<string> { valorFiltro });
                    }
                }

                return dicFiltros;
            }

            return null;
        }

        /// <summary>
        /// Crea los filtros en formato sparql.
        /// </summary>
        /// <param name="pDicFiltros">Diccionario con los filtros.</param>
        /// <param name="pVarAnterior">Variable anterior para no repetir nombre.</param>
        /// <param name="pAux">Variable auxiliar para que no se repitan los nombres.</param>
        /// <returns>String con los filtros creados.</returns>
        private string CrearFiltros(Dictionary<string, List<string>> pDicFiltros, string pVarAnterior, ref int pAux)
        {
            string varInicial = pVarAnterior;

            if (pDicFiltros != null && pDicFiltros.Count > 0)
            {
                StringBuilder filtro = new StringBuilder();

                foreach (KeyValuePair<string, List<string>> item in pDicFiltros)
                {
                    // Filtro de fechas.
                    if (item.Key == "dct:issued")
                    {
                        foreach (string fecha in item.Value)
                        {
                            filtro.Append($@"FILTER(?fecha >= {fecha.Split('-')[0]}000000) ");
                            filtro.Append($@"FILTER(?fecha <= {fecha.Split('-')[1]}000000) ");
                        }
                    }
                    else
                    {
                        // Filtros normales.
                        foreach (string parteFiltro in item.Key.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string varActual = $@"?{parteFiltro.Substring(parteFiltro.IndexOf(":") + 1)}{pAux}";
                            filtro.Append($@"{pVarAnterior} ");
                            filtro.Append($@"{parteFiltro} ");
                            filtro.Append($@"{varActual}. ");
                            pVarAnterior = varActual;
                            pAux++;
                        }

                        string valorFiltro = string.Empty;
                        foreach (string valor in item.Value)
                        {
                            Uri uriAux = null;
                            bool esUri = Uri.TryCreate(valor, UriKind.Absolute, out uriAux);
                            if (esUri)
                            {
                                valorFiltro += $@",<{valor}>";
                            }
                            else
                            {
                                valorFiltro += $@",'{valor}'";
                            }
                        }

                        if (valorFiltro.Length > 0)
                        {
                            valorFiltro = valorFiltro.Substring(1);
                        }

                        filtro.Append($@"FILTER({pVarAnterior} IN ({HttpUtility.UrlDecode(valorFiltro)})) ");
                        pVarAnterior = varInicial;
                    }
                }
                return filtro.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Mediante el ID del grafo de la ontología, obtiene el ID del grafo de búsqueda.
        /// </summary>
        /// <param name="pIdOntologia">ID del grafo de la ontología.</param>
        /// <returns>ID del grafo de búsqueda.</returns>
        private string ObtenerIdBusqueda(string pIdOntologia)
        {
            Guid idCorto = mResourceApi.GetShortGuid(pIdOntologia);
            return $@"http://gnoss/{idCorto.ToString().ToUpper()}";
        }

        /// <summary>
        /// Rellenar los años faltantes del diccionario.
        /// </summary>
        /// <param name="pDicResultados">Diccionario a rellenar.</param>
        /// <param name="pFechaInicial">Primer año.</param>
        /// <param name="pFechaFinal">Último año.</param>
        private void RellenarAnys(Dictionary<string, int> pDicResultados, string pFechaInicial, string pFechaFinal)
        {
            // Fecha inicial.
            int dia1 = int.Parse(pFechaInicial.Substring(6, 2));
            int mes1 = int.Parse(pFechaInicial.Substring(4, 2));
            int anio1 = int.Parse(pFechaInicial.Substring(0, 4));
            DateTime fecha1 = new DateTime(anio1, mes1, dia1);

            // Fecha final.
            int dia2 = int.Parse(pFechaFinal.Substring(6, 2));
            int mes2 = int.Parse(pFechaFinal.Substring(4, 2));
            int anio2 = int.Parse(pFechaFinal.Substring(0, 4));
            DateTime fecha2 = new DateTime(anio2, mes2, dia2);

            while (fecha1 <= fecha2)
            {
                // Hay que rellenar con los años intermedios.
                string fechaString = $@"{fecha1.ToString("yyyyMMdd")}010000";
                if (!pDicResultados.ContainsKey(fechaString))
                {
                    pDicResultados.Add(fechaString, 0);
                }
                fecha1 = fecha1.AddYears(1);
            }
        }

        /// <summary>
        /// Agrupa el número de publicaciones por año.
        /// </summary>
        /// <param name="pDicResultados">Diccionario a agrupar.</param>
        /// <returns>Diccionario con los datos agrupados por año.</returns>
        private Dictionary<string, int> AgruparAnys(Dictionary<string, int> pDicResultados)
        {
            Dictionary<string, int> pDicAgrupado = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> item in pDicResultados)
            {
                string anio = ExtraerAny(item.Key);
                int numPublicaciones = item.Value;

                if (!pDicAgrupado.ContainsKey(anio))
                {
                    pDicAgrupado.Add(anio, numPublicaciones);
                }
                else
                {
                    pDicAgrupado[anio] += numPublicaciones;
                }
            }
            return pDicAgrupado;
        }

        /// <summary>
        /// Obtiene el año de un string.
        /// </summary>
        /// <param name="pFecha">String del año.</param>
        /// <returns>Año en string.</returns>
        private string ExtraerAny(string pFecha)
        {
            return pFecha.Substring(0, 4);
        }

        /// <summary>
        /// Obtiene las llaves de un diccionario.
        /// </summary>
        /// <param name="pDic"></param>
        /// <returns>Lista de llaves.</returns>
        private List<string> GetKeysList(Dictionary<string, int> pDic)
        {
            return pDic.Keys.ToList();
        }

        /// <summary>
        /// Obtiene los valores de un diccionario.
        /// </summary>
        /// <param name="pDic"></param>
        /// <returns>Lista de valores.</returns>
        private List<int> GetValuesList(Dictionary<string, int> pDic)
        {
            return pDic.Values.ToList();
        }

        /// <summary>
        /// Permite crear la lista con los colores.
        /// </summary>
        /// <param name="pSize">Tamaño de la lista.</param>
        /// <param name="pColorHex">Colores asignados.</param>
        /// <returns>Lista con los colores.</returns>
        private List<string> CrearListaColores(int pSize, string pColorHex)
        {
            List<string> listaColores = new List<string>();
            for (int i = 0; i < pSize; i++)
            {
                listaColores.Add(pColorHex);
            }
            return listaColores;
        }
        #endregion
    }
}
