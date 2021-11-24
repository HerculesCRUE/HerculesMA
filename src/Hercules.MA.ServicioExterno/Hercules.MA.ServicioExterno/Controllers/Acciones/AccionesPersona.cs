using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Hercules.MA.ServicioExterno.Controllers.Utilidades;
using Hercules.MA.ServicioExterno.Models.DataFechas;
using Hercules.MA.ServicioExterno.Models.DataGraficaColaboradores;
using Hercules.MA.ServicioExterno.Models.DataGraficaProyectosGroupBars;
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

namespace Hercules.MA.ServicioExterno.Controllers.Acciones
{
    public class AccionesPersona
    {
        #region --- Constantes   
        private static string RUTA_OAUTH = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config/ConfigOAuth/OAuthV3.config";
        private static ResourceApi mResourceApi = new ResourceApi(RUTA_OAUTH);
        private static CommunityApi mCommunityApi = new CommunityApi(RUTA_OAUTH);
        private static Guid mIdComunidad = mCommunityApi.GetCommunityId();
        private static string RUTA_PREFIJOS = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Models/JSON/prefijos.json";
        private static string mPrefijos = string.Join(" ", JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(RUTA_PREFIJOS)));
        private static string COLOR_GRAFICAS = "#6cafe3";
        private static string COLOR_GRAFICAS_HORIZONTAL = "#6cafe3";
        #endregion

        /// <summary>
        /// Obtienes los datos de las pestañas de cada sección de la ficha.
        /// </summary>
        /// <param name="pPersona">Nombre de la persona.</param>
        /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        public Dictionary<string, int> GetDatosCabeceraPersona(string pPersona)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pPersona);
            Dictionary<string, int> dicResultados = new Dictionary<string, int>();
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT COUNT(DISTINCT ?proyecto) AS ?NumProyectos COUNT(DISTINCT ?documento) AS ?NumPublicaciones COUNT(DISTINCT ?categoria) AS ?NumCategorias ");
            where.Append("WHERE {{ "); // Total Proyectos.
            where.Append("?proyecto vivo:relates ?relacion. ");
            where.Append("?proyecto gnoss:hasprivacidadCom 'publico'. ");
            where.Append("?relacion roh:roleOf ?persona. ");
            where.Append($@"FILTER(?persona = <{idGrafoBusqueda}>) ");
            where.Append("} UNION { "); // Total Documentos.
            where.Append("?documento bibo:authorList ?listaAutores. ");
            where.Append("?listaAutores rdf:member ?persona. ");
            where.Append($@"FILTER(?persona = <{idGrafoBusqueda}>) ");
            where.Append("} UNION { "); // Total Categorías.
            where.Append("?s ?p ?documentoC. ");
            where.Append("?documentoC bibo:authorList ?listaAutoresC. ");
            where.Append("?listaAutoresC rdf:member ?persona. ");
            where.Append("?documentoC roh:hasKnowledgeArea ?area. ");
            where.Append("?area roh:categoryNode ?categoria. ");
            where.Append($@"FILTER(?persona = <{idGrafoBusqueda}>) ");
            where.Append("}} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    int numProyectos = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "NumProyectos"));
                    int numDocumentos = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "NumPublicaciones"));
                    int numCategorias = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "NumCategorias"));
                    dicResultados.Add("Proyectos", numProyectos);
                    dicResultados.Add("Publicaciones", numDocumentos);
                    dicResultados.Add("Categorias", numCategorias);
                }
            }

            // Consulta sparql.
            select = new StringBuilder();
            where = new StringBuilder();
            select.Append(mPrefijos);
            select.Append("SELECT  COUNT(DISTINCT ?id) AS ?NumColaboradores ");
            where.Append("WHERE {{ ");
            where.Append("SELECT * WHERE { ");
            where.Append("?proyecto vivo:relates ?relacion. ");
            where.Append("?relacion roh:roleOf ?persona. ");
            where.Append($@"FILTER(?persona = <{idGrafoBusqueda}>) ");
            where.Append("?proyecto vivo:relates ?relacion2. ");
            where.Append("?relacion2 roh:roleOf ?id. ");
            where.Append($@"FILTER(?id != <{idGrafoBusqueda}>)}} ");
            where.Append("} UNION { ");
            where.Append("SELECT * WHERE { ");
            where.Append("?documento bibo:authorList ?listaAutores. ");
            where.Append("?listaAutores rdf:member ?persona2. ");
            where.Append($@"FILTER(?persona2 = <{idGrafoBusqueda}>) ");
            where.Append("?documento bibo:authorList ?listaAutores2. ");
            where.Append("?listaAutores2 rdf:member ?id. ");
            where.Append($@"FILTER(?id != <{idGrafoBusqueda}>) ");
            where.Append("}}} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    int numColaboradores = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "NumColaboradores"));
                    dicResultados.Add("Colaboradores", numColaboradores);
                }
            }

            return dicResultados;
        }

        /// <summary>
        /// Obtiene los datos para crear la gráfica de las publicaciones.
        /// </summary>
        /// <param name="pIdPersona">ID del recurso de la persona.</param>
        /// <param name="pParametros">Filtros aplicados en las facetas.</param>
        /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        public DataGraficaPublicaciones GetDatosGraficaPublicaciones(string pIdPersona, string pParametros)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdPersona);
            Dictionary<string, int> dicResultados = new Dictionary<string, int>(); // Dictionary<año, numDocumentos>
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT ?fecha COUNT(DISTINCT(?documento)) AS ?NumPublicaciones ");
            where.Append("WHERE { ");
            where.Append("?documento bibo:authorList ?listaAutores. ");
            where.Append("?documento dct:issued ?fecha. ");
            where.Append("?listaAutores rdf:member ?persona. ");
            where.Append($@"FILTER(?persona = <{idGrafoBusqueda}>) ");
            if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
                string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?documento", ref aux);
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
                UtilidadesAPI.RellenarAnys(dicResultados, dicResultados.First().Key, dicResultados.Last().Key);
                dicResultados = UtilidadesAPI.AgruparAnys(dicResultados);
                dicResultados = dicResultados.OrderBy(item => item.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
            }

            // Contruir el objeto de la gráfica.
            List<string> listaColores = UtilidadesAPI.CrearListaColores(dicResultados.Count, COLOR_GRAFICAS);
            Models.DataGraficaPublicaciones.Datasets datasets = new Models.DataGraficaPublicaciones.Datasets("Publicaciones", UtilidadesAPI.GetValuesList(dicResultados), listaColores, listaColores, 1);
            Models.DataGraficaPublicaciones.Data data = new Models.DataGraficaPublicaciones.Data(UtilidadesAPI.GetKeysList(dicResultados), new List<Models.DataGraficaPublicaciones.Datasets> { datasets });
            Models.DataGraficaPublicaciones.Options options = new Models.DataGraficaPublicaciones.Options(new Models.DataGraficaPublicaciones.Scales(new Y(true)), new Models.DataGraficaPublicaciones.Plugins(new Models.DataGraficaPublicaciones.Title(true, "Evolución temporal publicaciones"), new Models.DataGraficaPublicaciones.Legend(new Labels(true), "top", "end")));
            DataGraficaPublicaciones dataGrafica = new DataGraficaPublicaciones("bar", data, options);

            return dataGrafica;
        }

        /// <summary>
        /// Obtiene los datos para crear la gráfico de los proyectos por año.
        /// </summary>
        /// <param name="pIdPersona">ID del recurso de la persona.</param>
        /// <param name="pParametros">Filtros aplicados en las facetas.</param>
        public ObjGrafica GetDatosGraficaProyectos(string pIdPersona, string pParametros)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdPersona);
            Dictionary<string, DataFechas> dicResultados = new();
            SparqlObject resultadoQuery = null;
            StringBuilder select1 = new(), where1 = new();
            StringBuilder select2 = new(), where2 = new();

            // Consultas sparql.

            #region --- Obtención de datos del año de inicio de los proyectos
            select1.Append(mPrefijos);
            select1.Append("SELECT COUNT(DISTINCT(?proyecto)) AS ?numPublicaciones ?anyoInicio ");
            where1.Append("WHERE { ");
            where1.Append("?proyecto vivo:relates ?relacion. ");
            where1.Append("?proyecto gnoss:hasprivacidadCom 'publico'. ");
            where1.Append("?proyecto vivo:start ?fecha. ");
            where1.Append("?proyecto vivo:end ?fechaFin. ");
            where1.Append("?relacion roh:roleOf ?persona. ");
            where1.Append("BIND( SUBSTR( STR(?fecha), 0, 4) AS ?anyoInicio) ");
            where1.Append($@"FILTER(?persona = <{idGrafoBusqueda}>) ");
            if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = ObtenerParametros(pParametros);
                string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?proyecto", ref aux);
                where1.Append(filtros);
            }
            where1.Append("} ");
            where1.Append("ORDER BY ?anyoInicio ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select1.ToString(), where1.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    string anyo = UtilidadesAPI.GetValorFilaSparqlObject(fila, "anyoInicio");
                    int numProyectos = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numPublicaciones"));

                    if (!dicResultados.ContainsKey(anyo))
                    {
                        // Si no contiene el año, creo el objeto.
                        DataFechas data = new();
                        data.numProyectosInicio = numProyectos;
                        data.numProyectosFin = 0;
                        dicResultados.Add(anyo, data);
                    }
                    else
                    {
                        // Si lo contiene, se lo agrego.
                        dicResultados[anyo].numProyectosInicio += numProyectos;
                    }
                }
            }
            #endregion

            #region --- Obtención de datos del año de fin de los proyectos
            select2.Append(mPrefijos);
            select2.Append("SELECT COUNT(DISTINCT(?proyecto)) AS ?numPublicaciones ?anyoFin ");
            where2.Append("WHERE { ");
            where2.Append("?proyecto vivo:relates ?relacion. ");
            where2.Append("?proyecto gnoss:hasprivacidadCom 'publico'. ");
            where2.Append("?proyecto vivo:start ?fecha. ");
            where2.Append("?proyecto vivo:end ?fechaFin. ");
            where2.Append("?relacion roh:roleOf ?persona. ");
            where2.Append("BIND( SUBSTR( STR(?fechaFin), 0, 4) AS ?anyoFin) ");
            where2.Append($@"FILTER(?persona = <{idGrafoBusqueda}>) ");
            if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = ObtenerParametros(pParametros);
                string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?proyecto", ref aux);
                where2.Append(filtros);
            }
            where2.Append("} ");
            where2.Append("ORDER BY ?anyoFin ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select2.ToString(), where2.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    string anyo = UtilidadesAPI.GetValorFilaSparqlObject(fila, "anyoFin");
                    int numProyectos = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numPublicaciones"));

                    if (!dicResultados.ContainsKey(anyo))
                    {
                        // Si no contiene el año, creo el objeto.
                        DataFechas data = new();
                        data.numProyectosInicio = 0;
                        data.numProyectosFin = numProyectos;
                        dicResultados.Add(anyo, data);
                    }
                    else
                    {
                        // Si lo contiene, se lo agrego.
                        dicResultados[anyo].numProyectosFin += numProyectos;
                    }
                }
            }
            #endregion

            // Rellenar años intermedios y ordenarlos.
            string max = "2100";
            string min = "1900";
            if (dicResultados != null && dicResultados.Count > 0)
            {
                max = dicResultados.Keys.First();
                min = dicResultados.Keys.Last();
            }
            RellenarAnys(dicResultados, max, min);
            dicResultados = dicResultados.OrderBy(item => item.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

            // Se coge los datos del número de proyectos.
            List<int> listaInicios = new();
            List<int> listaFines = new();
            foreach (KeyValuePair<string, DataFechas> item in dicResultados)
            {
                listaInicios.Add(item.Value.numProyectosInicio);
                listaFines.Add(item.Value.numProyectosFin);
            }

            // Se construye el objeto con los datos.
            List<DatosAnyo> listaDatos = new List<DatosAnyo>();
            listaDatos.Add(new DatosAnyo("Inicio", "#6cafe3", listaInicios));
            listaDatos.Add(new DatosAnyo("Fin", "#BF4858", listaFines));

            // Se crea el objeto de la gráfica.
            DataGraficaProyectosGroupBars dataObj = new DataGraficaProyectosGroupBars(dicResultados.Keys.ToList(), listaDatos);

            return new ObjGrafica("bar", dataObj, new Models.DataGraficaProyectosGroupBars.Options(20, new Models.DataGraficaProyectosGroupBars.Scales(new List<YAxes>() { new YAxes(new Models.DataGraficaProyectosGroupBars.Ticks(0)) })));
        }

        /// <summary>
        /// Obtiene el dato del grupo de investigación el cual pertenece la persona.
        /// </summary>
        /// <param name="pIdPersona">ID del recurso de la persona.</param>
        /// <returns>Lista con los grupos de investigación pertenecientes.</returns>
        public List<string> GetGrupoInvestigacion(string pIdPersona)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdPersona);
            List<string> grupos = new List<string>();
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT DISTINCT(?tituloGrupo) ");
            where.Append("WHERE { ");
            where.Append("?s ?p ?grupo. ");
            where.Append("?s foaf:member ?miembros. ");
            where.Append("?s roh:mainResearcher ?investigadorPrincipal. ");
            where.Append("?s roh:title ?tituloGrupo. ");
            where.Append("?miembros roh:roleOf ?persona1. ");
            where.Append("?investigadorPrincipal  roh:roleOf ?persona2. ");
            where.Append("FILTER(?grupo = 'group') ");
            where.Append($@"FILTER(?persona1 LIKE <{idGrafoBusqueda}> || ?persona2 LIKE <{idGrafoBusqueda}>) ");
            where.Append("} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    grupos.Add(UtilidadesAPI.GetValorFilaSparqlObject(fila, "tituloGrupo"));
                }
            }

            return grupos;
        }

        /// <summary>
        /// Obtiene los topics de las publicaciones de la persona.
        /// </summary>
        /// <param name="pIdPersona">ID del recurso de la persona.</param>
        /// <returns>Listado de topics de dicha persona.</returns>
        public List<string> GetTopicsPersona(string pIdPersona)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdPersona);
            List<string> categorias = new List<string>();
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT DISTINCT(?nombreCategoria) ");
            where.Append("WHERE { ");
            where.Append("?s ?p ?documento. ");
            where.Append("?documento bibo:authorList ?listaAutores. ");
            where.Append("?listaAutores rdf:member ?persona. ");
            where.Append("?documento roh:hasKnowledgeArea ?area. ");
            where.Append("?area roh:categoryNode ?categoria. ");
            where.Append("?categoria skos:prefLabel ?nombreCategoria. ");
            where.Append($@"FILTER(?persona = <{idGrafoBusqueda}>)");
            where.Append("} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    categorias.Add(UtilidadesAPI.GetValorFilaSparqlObject(fila, "nombreCategoria"));
                }
            }

            return categorias;
        }

        /// <summary>
        /// Obtiene un listado con los objetos de la gráfica.
        /// </summary>
        /// <param name="pIdPersona">ID del recurso de la persona.</param>
        /// <param name="pNombrePersona">En este caso, el nombre completo de la persona.</param>
        /// <returns>Listado de objetos de la gráfica.</returns>
        public List<DataGraficaColaboradores> GetDatosGraficaRedColaboradoresPersonas(string pIdPersona, string pParametros, string pNombrePersona)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdPersona);
            Dictionary<string, string> dicNodos = new Dictionary<string, string>();
            Dictionary<string, DataQueryRelaciones> dicRelaciones = new Dictionary<string, DataQueryRelaciones>();
            Dictionary<string, int> dicPersonasColabo = new();
            SparqlObject resultadoQuery = null;

            string personas = $@"<{idGrafoBusqueda}>";

            if (!string.IsNullOrEmpty(pNombrePersona))
            {
                dicNodos.Add(idGrafoBusqueda, pNombrePersona.ToLower().Trim());
            }

            // Consulta sparql.
            string select = mPrefijos;
            select += "SELECT ?id ?nombre COUNT(*) AS ?numRelaciones ";
            string where = $@"
                WHERE {{ {{
                        SELECT *
                        WHERE {{
                        ?documento bibo:authorList ?listaAutores.
                        ?listaAutores rdf:member ?personaDoc.
                        FILTER(?personaDoc = <{idGrafoBusqueda}>)";
            where += $@"?documento bibo:authorList ?listaAutores2.
                        ?listaAutores2 rdf:member ?id.
                        ?id foaf:name ?nombre.
                        FILTER(?id != <{idGrafoBusqueda}>)";
            if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = ObtenerParametros(pParametros);
                string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?id", ref aux);
                where += filtros;
            }
            where += $@"}}
                    }} UNION {{
                        SELECT *
                        WHERE {{
                        ?proyecto vivo:relates ?relacion.
                        ?relacion roh:roleOf ?persona.
                        FILTER(?persona = <{idGrafoBusqueda}>)";
            where += $@"?proyecto vivo:relates ?relacion2.
                        ?relacion2 roh:roleOf ?id.
                        ?id foaf:name ?nombre.
                        FILTER(?id != <{idGrafoBusqueda}>)";
            if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = ObtenerParametros(pParametros);
                string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?id", ref aux);
                where += filtros;
            }
            where += $@"}} }} }} ORDER BY DESC (COUNT(*)) LIMIT 10";

            resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    string id = UtilidadesAPI.GetValorFilaSparqlObject(fila, "id");
                    int proyectosComun = Int32.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numRelaciones"));
                    string nombreColaborador = UtilidadesAPI.GetValorFilaSparqlObject(fila, "nombre");

                    dicPersonasColabo.Add(id, proyectosComun);
                    dicNodos.Add(id, nombreColaborador.ToLower().Trim());
                    personas += ",<" + UtilidadesAPI.GetValorFilaSparqlObject(fila, "id") + ">";
                }
            }

            KeyValuePair<string, string> proyecto = dicNodos.First();
            foreach (KeyValuePair<string, string> item in dicNodos)
            {
                if (item.Key != proyecto.Key)
                {
                    dicRelaciones.Add(item.Key, new DataQueryRelaciones(new List<Datos> { new Datos(proyecto.Key, dicPersonasColabo[item.Key]) }));
                }
            }

            #region --- Crear las relaciones entre dichas personas
            List<int> numColaboraciones = new();

            // Consulta Sparql
            StringBuilder select2 = new StringBuilder(mPrefijos);
            select2.Append("SELECT ?persona1 ?persona2 (COUNT(DISTINCT ?proyectos) + COUNT(DISTINCT ?publicaciones)) AS ?numVeces ");
            StringBuilder where2 = new StringBuilder("WHERE {{ ");
            where2.Append("?proyectos vivo:relates ?personas. ");
            where2.Append("?personas roh:roleOf ?persona1. ");
            where2.Append("BIND(?proyectos AS ?proyectos2) ");
            where2.Append("?proyectos2 vivo:relates ?personas2. ");
            where2.Append("?personas2 roh:roleOf ?persona2. ");
            where2.Append("?persona2 foaf:name ?nombre. ");
            where2.Append("FILTER(?persona1 != ?persona2) ");
            where2.Append($@"FILTER(?persona1 IN ({personas})) ");
            where2.Append($@"FILTER(?persona2 IN ({personas})) ");
            where2.Append("} UNION { ");
            where2.Append("?publicaciones bibo:authorList ?lista. ");
            where2.Append("?lista roh:item ?persona1. ");
            where2.Append("BIND(?publicaciones AS ?publicaciones2) ");
            where2.Append("?publicaciones2 bibo:authorList ?lista2. ");
            where2.Append("?lista2 roh:item ?persona2. ");
            where2.Append("?persona2 foaf:name ?nombre. ");
            where2.Append("FILTER(?persona1 != ?persona2) ");
            where2.Append($@"FILTER(?persona1 IN ({personas})) ");
            where2.Append($@"FILTER(?persona2 IN ({personas})) ");
            where2.Append("}} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select2.ToString(), where2.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    string persona1 = UtilidadesAPI.GetValorFilaSparqlObject(fila, "persona1");
                    string persona2 = UtilidadesAPI.GetValorFilaSparqlObject(fila, "persona2");
                    int veces = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numVeces"));

                    // Guarda en una lista para sacar el máximo de relaciones que hay entre los nodos.
                    numColaboraciones.Add(veces);

                    // TODO: REVISAR QUE FUNCIONA BIEN DEL TODO
                    // Crea las relaciones entre personas.
                    if (!dicRelaciones.ContainsKey(persona1))
                    {
                        //dicRelaciones.Add(persona1, new DataQueryRelaciones(new List<Datos> { new Datos(persona2, veces) }));
                        if (!dicRelaciones.ContainsKey(persona1))
                        {
                            bool encontrado = false;
                            foreach (KeyValuePair<string, DataQueryRelaciones> relaciones in dicRelaciones)
                            {
                                foreach (Datos datos in relaciones.Value.idRelacionados)
                                {
                                    if (datos.idRelacionado == persona1)
                                    {
                                        encontrado = true;
                                        break;
                                    }
                                }

                                if (encontrado) break;
                            }

                            if (encontrado == false)
                            {
                                dicRelaciones.Add(persona1, new DataQueryRelaciones(new List<Datos> { new Datos(persona2, veces) }));
                            }
                        }
                        else
                        {
                            bool encontrado = false;
                            foreach (Datos relaciones in dicRelaciones[persona2].idRelacionados)
                            {
                                if (relaciones.idRelacionado == persona1)
                                {
                                    encontrado = true;
                                    break;
                                }
                            }
                            foreach (Datos relaciones in dicRelaciones[persona1].idRelacionados)
                            {
                                if (relaciones.idRelacionado == persona2)
                                {
                                    encontrado = true;
                                    break;
                                }
                            }

                            if (!encontrado)
                            {
                                dicRelaciones[persona1].idRelacionados.Add(new Datos(persona2, veces));
                            }
                        }
                    }
                    else
                    {
                        if (!dicRelaciones.ContainsKey(persona2))
                        {
                            bool encontrado = false;
                            foreach (KeyValuePair<string, DataQueryRelaciones> relaciones in dicRelaciones)
                            {
                                foreach (Datos datos in relaciones.Value.idRelacionados)
                                {
                                    if (datos.idRelacionado == persona2)
                                    {
                                        encontrado = true;
                                        break;
                                    }
                                }

                                if (encontrado) break;
                            }

                            if (encontrado == false)
                            {
                                dicRelaciones.Add(persona2, new DataQueryRelaciones(new List<Datos> { new Datos(persona1, veces) }));
                            }
                        }
                        else
                        {
                            bool encontrado = false;
                            foreach (Datos relaciones in dicRelaciones[persona1].idRelacionados)
                            {
                                if (relaciones.idRelacionado == persona2)
                                {
                                    encontrado = true;
                                    break;
                                }
                            }
                            foreach (Datos relaciones in dicRelaciones[persona2].idRelacionados)
                            {
                                if (relaciones.idRelacionado == persona1)
                                {
                                    encontrado = true;
                                    break;
                                }
                            }

                            if (!encontrado)
                            {
                                dicRelaciones[persona2].idRelacionados.Add(new Datos(persona1, veces));
                            }
                        }
                    }
                }
            }
            #endregion

            // Construcción del objeto de la gráfica.            
            List<DataGraficaColaboradores> colaboradores = new List<DataGraficaColaboradores>();
            int maximasRelaciones = 1;
            if (dicPersonasColabo.Values.Max() > numColaboraciones.Max())
            {
                maximasRelaciones = dicPersonasColabo.Values.Max();
            }
            else
            {
                maximasRelaciones = numColaboraciones.Max();
            }

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
                        string id = $@"{sujeto.Key}~{relaciones.idRelacionado}~{relaciones.numVeces}";
                        Models.DataGraficaColaboradores.Data data = new Models.DataGraficaColaboradores.Data(id, null, sujeto.Key, relaciones.idRelacionado, UtilidadesAPI.CalcularGrosor(maximasRelaciones, relaciones.numVeces), "edges");
                        DataGraficaColaboradores dataColabo = new DataGraficaColaboradores(data, null, null);
                        colaboradores.Add(dataColabo);
                    }
                }
            }

            return colaboradores;
        }

        /// <summary>
        /// Obtiene el objeto de la gráfica horizontal de topics.
        /// </summary>
        /// <param name="pIdPersona">ID del recurso de la persona.</param>
        /// <param name="pParametros">Filtros de las facetas.</param>
        /// <returns>Objeto que se trata en JS para contruir la gráfica.</returns>
        public DataGraficaPublicacionesHorizontal GetDatosGraficaProyectosPersonaHorizontal(string pIdPersona, string pParametros)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdPersona);
            Dictionary<string, int> dicResultados = new Dictionary<string, int>();
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("Select ?nombreCategoria count(*) as ?numCategorias ");
            where.Append("where");
            where.Append("{ ");
            where.Append("?s ?p ?documento. ");
            where.Append("?documento bibo:authorList ?listaAutores.");
            where.Append("?listaAutores rdf:member ?persona.");
            where.Append("?documento roh:hasKnowledgeArea ?area. ");
            where.Append("?area roh:categoryNode ?categoria. ");
            where.Append("?categoria skos:prefLabel ?nombreCategoria. ");
            where.Append($@"FILTER(?persona = <{idGrafoBusqueda}>)");
            where.Append("} ");
            where.Append("Group by(?nombreCategoria)");

            if (!string.IsNullOrEmpty(pParametros))
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
                string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?documento", ref aux);
                where.Append(filtros);
            }

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

            //Ordenar diccionario
            var dicionarioOrdenado = dicResultados.OrderByDescending(x => x.Value);

            // Calculo del porcentaje.
            int numTotalCategorias = 0;
            foreach (KeyValuePair<string, int> item in dicionarioOrdenado)
            {
                numTotalCategorias += item.Value;
            }
            Dictionary<string, double> dicResultadosPorcentaje = new Dictionary<string, double>();
            foreach (KeyValuePair<string, int> item in dicionarioOrdenado)
            {
                double porcentaje = Math.Round((double)(100 * item.Value) / numTotalCategorias, 2);
                dicResultadosPorcentaje.Add(item.Key, porcentaje);
            }

            // Contruir el objeto de la gráfica.
            List<string> listaColores = UtilidadesAPI.CrearListaColores(dicionarioOrdenado.Count(), COLOR_GRAFICAS_HORIZONTAL);
            Models.DataGraficaPublicacionesHorizontal.Datasets datasets = new Models.DataGraficaPublicacionesHorizontal.Datasets(dicResultadosPorcentaje.Values.ToList(), listaColores);
            Models.DataGraficaPublicacionesHorizontal.Data data = new Models.DataGraficaPublicacionesHorizontal.Data(dicResultadosPorcentaje.Keys.ToList(), new List<Models.DataGraficaPublicacionesHorizontal.Datasets> { datasets });

            // Máximo.
            x xAxes = new x(new Models.DataGraficaPublicacionesHorizontal.Ticks(0, 100), new ScaleLabel(true, "Percentage"));

            Models.DataGraficaPublicacionesHorizontal.Options options = new Models.DataGraficaPublicacionesHorizontal.Options("y", new Models.DataGraficaPublicacionesHorizontal.Plugins(new Models.DataGraficaPublicacionesHorizontal.Title(true, "Resultados de la investigación por fuente de datos"), new Models.DataGraficaPublicacionesHorizontal.Legend(false)), new Models.DataGraficaPublicacionesHorizontal.Scales(xAxes));
            DataGraficaPublicacionesHorizontal dataGrafica = new DataGraficaPublicacionesHorizontal("bar", data, options);

            return dataGrafica;
        }

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
        /// Rellenar los años faltantes del diccionario.
        /// </summary>
        /// <param name="pDicResultados">Diccionario a rellenar.</param>
        /// <param name="pFechaInicial">Primer año.</param>
        /// <param name="pFechaFinal">Último año.</param>
        private void RellenarAnys(Dictionary<string, DataFechas> pDicResultados, string pFechaInicial, string pFechaFinal)
        {
            // Fecha inicial.
            int dia1 = 01;
            int mes1 = 01;
            int anio1 = int.Parse(pFechaInicial);
            DateTime fecha1 = new DateTime(anio1, mes1, dia1);

            // Fecha final.
            int dia2 = 01;
            int mes2 = 01;
            int anio2 = int.Parse(pFechaFinal);
            DateTime fecha2 = new DateTime(anio2, mes2, dia2);

            while (fecha1 <= fecha2)
            {
                // Hay que rellenar con los años intermedios.
                string fechaString = $@"{fecha1.ToString("yyyy")}";
                if (!pDicResultados.ContainsKey(fechaString))
                {
                    DataFechas data = new();
                    data.numProyectosInicio = 0;
                    data.numProyectosFin = 0;
                    pDicResultados.Add(fechaString, data);
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
    }
}
