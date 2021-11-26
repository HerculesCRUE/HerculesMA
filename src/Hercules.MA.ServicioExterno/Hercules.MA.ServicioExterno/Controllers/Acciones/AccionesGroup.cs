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
    public class AccionesGroup
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
        /// <param name="pGrupo">ID del recurso del grupo.</param>
        public Dictionary<string, int> GetDatosCabeceraGrupo(string pGrupo)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pGrupo);
            Dictionary<string, int> dicResultados = new Dictionary<string, int>();
            {
                string select = "SELECT COUNT(DISTINCT ?proyecto) AS ?NumProyectos";
                string where = $@"
                    WHERE 
                    {{
                        
                        ?proyecto a 'project'.
                        ?proyecto <http://vivoweb.org/ontology/core#relates> ?members.
                        ?proyecto gnoss:hasprivacidadCom 'publico'.
                        ?members <http://w3id.org/roh/roleOf> ?people. 
                        OPTIONAL{{?proyecto <http://vivoweb.org/ontology/core#start> ?fechaProjInit.}}
                        OPTIONAL{{?proyecto <http://vivoweb.org/ontology/core#end> ?fechaProjEnd.}}
                        BIND(IF(bound(?fechaProjEnd), ?fechaProjEnd, 30000000000000) as ?fechaProjEndAux)
                        BIND(IF(bound(?fechaProjInit), ?fechaProjInit, 10000000000000) as ?fechaProjInitAux)
 
                        <{idGrafoBusqueda}> ?p2 ?members2.
                        FILTER (?p2 IN (<http://xmlns.com/foaf/0.1/member>, <http://w3id.org/roh/mainResearcher> ) )
                        ?members2 <http://w3id.org/roh/roleOf> ?people. 
                        OPTIONAL{{?members2 <http://vivoweb.org/ontology/core#start> ?fechaGroupInit.}}
                        OPTIONAL{{?members2 <http://vivoweb.org/ontology/core#end> ?fechaGroupEnd.}}
                        BIND(IF(bound(?fechaGroupEnd), ?fechaGroupEnd, 30000000000000) as ?fechaGroupEndAux)
                        BIND(IF(bound(?fechaGroupInit), ?fechaGroupInit, 10000000000000) as ?fechaGroupInitAux)
      
                        FILTER(?fechaGroupEndAux >= ?fechaProjInitAux AND ?fechaGroupInitAux <= ?fechaProjEndAux)

                    }} ";

                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                {
                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                    {
                        int numProyectos = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "NumProyectos"));
                        dicResultados.Add("Proyectos", numProyectos);
                    }
                }
            }
            {

                string select = "SELECT COUNT(DISTINCT ?publicacion) AS ?NumPublicaciones";
                string where = $@"
                    WHERE 
                    {{  
                        <{idGrafoBusqueda}> ?propmembers ?members.
                        ?members <http://w3id.org/roh/roleOf> ?person.
                        FILTER(?propmembers  in (<http://xmlns.com/foaf/0.1/member>, <http://w3id.org/roh/mainResearcher>))
	                    ?person a 'person'.
                        OPTIONAL{{ ?members2 <http://vivoweb.org/ontology/core#start> ?fechaPersonaInit.}}
	                    OPTIONAL{{ ?members2 <http://vivoweb.org/ontology/core#end> ?fechaPersonaEnd.}}
	                    BIND(IF(bound(?fechaPersonaEnd), ?fechaPersonaEnd, 30000000000000) as ?fechaPersonaEndAux)
                        BIND(IF(bound(?fechaPersonaInit), ?fechaPersonaInit, 10000000000000) as ?fechaPersonaInitAux)
                        ?publicacion a 'document'.
                        ?publicacion <http://purl.org/ontology/bibo/authorList> ?author.
                        ?author <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
                        ?publicacion <http://purl.org/dc/terms/issued> ?fechaPublicacion.     
                        FILTER(?fechaPublicacion>= ?fechaPersonaInitAux AND ?fechaPublicacion<= ?fechaPersonaEndAux)
                    }} ";
                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                {
                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                    {
                        int numPublicaciones = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "NumPublicaciones"));
                        dicResultados.Add("Publicaciones", numPublicaciones);
                    }
                }
            }
            {
                string select = "SELECT COUNT(DISTINCT ?person) AS ?NumMiembros";
                string where = $@"
                    WHERE 
                    {{
                        <{idGrafoBusqueda}> ?propmembers ?members.
                        ?members <http://w3id.org/roh/roleOf> ?person.
                        FILTER(?propmembers  in (<http://xmlns.com/foaf/0.1/member>, <http://w3id.org/roh/mainResearcher>))                               
                    }} ";
                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                {
                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                    {
                        int numMiembros = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "NumMiembros"));
                        dicResultados.Add("Miembros", numMiembros);
                    }
                }
            }
            {
                StringBuilder select = new StringBuilder(), where = new StringBuilder();

                // Consulta sparql.
                select.Append(mPrefijos);
                select.Append("SELECT COUNT(DISTINCT ?categoria) AS ?NumCategorias ");
                where.Append("WHERE { ");
                // Total Categorías.
                where.Append($@"<{idGrafoBusqueda}> ?propmembers ?members.");
                where.Append("?members <http://w3id.org/roh/roleOf> ?person.");
                where.Append("FILTER(?propmembers in (<http://xmlns.com/foaf/0.1/member>, <http://w3id.org/roh/mainResearcher>))");
                where.Append("?s ?p ?documentoC. ");
                where.Append("?documentoC bibo:authorList ?listaAutoresC. ");
                where.Append("?listaAutoresC rdf:member ?persona. ");
                where.Append("?documentoC roh:hasKnowledgeArea ?area. ");
                where.Append("?area roh:categoryNode ?categoria. ");
                where.Append("FILTER(?persona = ?person)");
                where.Append("} ");

                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                {
                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                    {
                        int numCategorias = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "NumCategorias"));
                        dicResultados.Add("Categorias", numCategorias);
                    }
                }
            }
            {

                StringBuilder select = new StringBuilder();

                // Consulta sparql.
                select.Append(mPrefijos);
                select.Append("SELECT COUNT(DISTINCT ?nombre) AS ?NumColaboradores ");

                string where = $@"
                WHERE {{ {{
                        SELECT *
                        WHERE {{
                        <{idGrafoBusqueda}> roh:mainResearcher ?mainrp.
                        ?mainrp roh:roleOf ?mainresearcher.

                        ?documento a 'document'.
                        ?documento bibo:authorList ?listaAutores.
                        ?listaAutores rdf:member ?mainresearcher.
                        ?documento bibo:authorList ?listaAutores2.
                        ?listaAutores2 rdf:member ?id.

                        FILTER(?id != ?mainresearcher)
                        ?id foaf:name ?nombre.
                    }}
                    }} UNION {{
                        SELECT *
                        WHERE {{
                        <{idGrafoBusqueda}> roh:mainResearcher ?mainrp.
                        ?mainrp roh:roleOf ?mainresearcher.

                        ?proyecto a 'project'.
                        ?proyecto gnoss:hasprivacidadCom 'publico'.
                        ?proyecto vivo:relates ?relacion.
                        ?relacion roh:roleOf ?mainresearcher.
                        ?proyecto vivo:relates ?relacion2.
                        ?relacion2 roh:roleOf ?id.

                        FILTER(?id != ?mainresearcher)
                        ?id foaf:name ?nombre.
                    }} }} }}
                ";


                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where, mIdComunidad);

                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                {
                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                    {
                        int numCategorias = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "NumColaboradores"));
                        dicResultados.Add("NumColaboradores", numCategorias);
                    }
                }

                


            }
            return dicResultados;
        }


        /// <summary>
        /// Obtiene los topics de las publicaciones del grupo.
        /// </summary>
        /// <param name="pIdGrupo">ID del recurso del grupo.</param>
        /// <returns>Listado de topics del grupo.</returns>
        public List<string> GetTopicsGrupo(string pIdGrupo)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdGrupo);
            List<string> categorias = new List<string>();
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT DISTINCT(?nombreCategoria) ");
            where.Append("WHERE { ");
            where.Append($@"<{idGrafoBusqueda}> ?propmembers ?members.");
            where.Append("FILTER(?propmembers  in (foaf:member, roh:mainResearcher))");
            where.Append("?members roh:roleOf ?people.");
            where.Append("?s ?p ?documento. ");
            where.Append("?documento bibo:authorList ?listaAutores.");
            where.Append("?listaAutores rdf:member ?persona.");
            where.Append("?documento roh:hasKnowledgeArea ?area. ");
            where.Append("?area roh:categoryNode ?categoria. ");
            where.Append("?categoria skos:prefLabel ?nombreCategoria. ");
            where.Append("FILTER(?persona = ?people)");
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
        /// Obtiene el objeto de la gráfica horizontal de topics (Áreas temáticas).
        /// </summary>
        /// <param name="pIdGrupo">ID del recurso del grupo.</param>
        /// <param name="pParametros">Filtros de las facetas.</param>
        /// <returns>Objeto que se trata en JS para contruir la gráfica.</returns>
        public DataGraficaPublicacionesHorizontal GetDatosGraficaTopicsHorizontal(string pIdGrupo, string pParametros)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdGrupo);
            Dictionary<string, int> dicResultados = new Dictionary<string, int>();
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("Select ?nombreCategoria count(*) as ?numCategorias ");
            where.Append("where");
            where.Append("{ ");
            where.Append($@"<{idGrafoBusqueda}> ?propmembers ?members.");
            where.Append("FILTER(?propmembers  in (foaf:member, roh:mainResearcher))");
            where.Append("?members roh:roleOf ?people.");
            where.Append("?s ?p ?documento. ");
            where.Append("?documento bibo:authorList ?listaAutores.");
            where.Append("?listaAutores rdf:member ?persona.");
            where.Append("?documento roh:hasKnowledgeArea ?area. ");
            where.Append("?area roh:categoryNode ?categoria. ");
            where.Append("?categoria skos:prefLabel ?nombreCategoria. ");
            where.Append("FILTER(?persona = ?people)");
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
        /// Obtiene los datos para crear la gráfica de las publicaciones.
        /// </summary>
        /// <param name="pIdGroup">ID del recurso del grupo.</param>
        /// <param name="pParametros">Filtros aplicados en las facetas.</param>
        /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        public DataGraficaPublicaciones GetDatosGraficaPublicaciones(string pIdGroup, string pParametros)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdGroup);
            Dictionary<string, int> dicResultados = new Dictionary<string, int>(); // Dictionary<año, numDocumentos>
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT ?fecha COUNT(DISTINCT(?publicacion)) AS ?NumPublicaciones ");
            where.Append("WHERE { ");
            where.Append($@"<{idGrafoBusqueda}> ?propmembers ?members.");
            where.Append("?members <http://w3id.org/roh/roleOf> ?person.");
            where.Append("?person a 'person'.");
            where.Append("FILTER(?propmembers  in (<http://xmlns.com/foaf/0.1/member>, <http://w3id.org/roh/mainResearcher>))");
            where.Append("OPTIONAL{{ ?members2 <http://vivoweb.org/ontology/core#start> ?fechaPersonaInit.}}");
            where.Append("OPTIONAL{{ ?members2 <http://vivoweb.org/ontology/core#end> ?fechaPersonaEnd.}}");
            where.Append("BIND(IF(bound(?fechaPersonaEnd), ?fechaPersonaEnd, 30000000000000) as ?fechaPersonaEndAux)");
            where.Append("BIND(IF(bound(?fechaPersonaInit), ?fechaPersonaInit, 10000000000000) as ?fechaPersonaInitAux)");
            where.Append("?publicacion a 'document'.");
            where.Append("?publicacion <http://purl.org/ontology/bibo/authorList> ?author.");
            where.Append("?author <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.");
            where.Append("?publicacion <http://purl.org/dc/terms/issued> ?fecha.");
            where.Append("FILTER(?fecha>= ?fechaPersonaInitAux AND ?fecha <= ?fechaPersonaEndAux)");

            // Parameters
            if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
                string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?publicacion", ref aux);
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
        /// Obtiene los datos para crear la gráfica de proyectos.
        /// </summary>
        /// <param name="pIdGroup">ID del recurso del grupo.</param>
        /// <param name="pParametros">Filtros aplicados en las facetas.</param>
        /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        public ObjGrafica GetDatosGraficaProyectos(string pIdGroup, string pParametros)
        {

            string idGrafoBusqueda = ObtenerIdBusqueda(pIdGroup);
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
            where1.Append("?relacion roh:roleOf ?persona. ");
            where1.Append("OPTIONAL{?proyecto <http://vivoweb.org/ontology/core#start> ?fechaProjInit.}");
            where1.Append("OPTIONAL{?proyecto <http://vivoweb.org/ontology/core#end> ?fechaProjEnd.}");
            where1.Append("BIND(IF(bound(?fechaProjInit), ?fechaProjInit, 10000000000000) as ?fechaProjInitAux)");
            where1.Append("BIND(IF(bound(?fechaProjEnd), ?fechaProjEnd, 30000000000000) as ?fechaProjEndAux)");
            where1.Append("BIND( SUBSTR( STR(?fechaProjInit), 0, 4) AS ?anyoInicio) ");

            where1.Append($@"<{idGrafoBusqueda}> ?p2 ?members2.");
            where1.Append("FILTER (?p2 IN (<http://xmlns.com/foaf/0.1/member>, <http://w3id.org/roh/mainResearcher> ))");
            where1.Append("?members2 <http://w3id.org/roh/roleOf> ?persona. ");
            where1.Append("OPTIONAL{{?members2 <http://vivoweb.org/ontology/core#start> ?fechaGroupInit.}}");
            where1.Append("OPTIONAL{{?members2 <http://vivoweb.org/ontology/core#end> ?fechaGroupEnd.}}");
            where1.Append("BIND(IF(bound(?fechaGroupEnd), ?fechaGroupEnd, 30000000000000) as ?fechaGroupEndAux)");
            where1.Append("BIND(IF(bound(?fechaGroupInit), ?fechaGroupInit, 10000000000000) as ?fechaGroupInitAux)");

            where1.Append("FILTER(?fechaGroupEndAux >= ?fechaProjInitAux AND ?fechaGroupInitAux <= ?fechaProjEndAux)");

            // where1.Append($@"FILTER(?persona = <{idGrafoBusqueda}>) ");
            if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
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

                    try
                    {
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
                    catch (Exception e) { }
                }
            }
            #endregion

            #region --- Obtención de datos del año de fin de los proyectos
            select2.Append(mPrefijos);
            select2.Append("SELECT COUNT(DISTINCT(?proyecto)) AS ?numPublicaciones ?anyoFin ");
            where2.Append("WHERE { ");
            where2.Append("?proyecto vivo:relates ?relacion. ");
            where2.Append("?proyecto gnoss:hasprivacidadCom 'publico'. ");
            where2.Append("?relacion roh:roleOf ?persona. ");
            where2.Append("OPTIONAL{?proyecto <http://vivoweb.org/ontology/core#start> ?fechaProjInit.}");
            where2.Append("OPTIONAL{?proyecto <http://vivoweb.org/ontology/core#end> ?fechaProjEnd.}");
            where2.Append("BIND(IF(bound(?fechaProjEnd), ?fechaProjEnd, 30000000000000) as ?fechaProjEndAux)");
            where2.Append("BIND(IF(bound(?fechaProjInit), ?fechaProjInit, 10000000000000) as ?fechaProjInitAux)");
            where2.Append("BIND( SUBSTR( STR(?fechaProjEnd), 0, 4) AS ?anyoFin) ");

            where2.Append($@"<{idGrafoBusqueda}> ?p2 ?members2.");
            where2.Append("FILTER (?p2 IN (<http://xmlns.com/foaf/0.1/member>, <http://w3id.org/roh/mainResearcher> ))");
            where2.Append("?members2 <http://w3id.org/roh/roleOf> ?persona. ");
            where2.Append("OPTIONAL{{?members2 <http://vivoweb.org/ontology/core#start> ?fechaGroupInit.}}");
            where2.Append("OPTIONAL{{?members2 <http://vivoweb.org/ontology/core#end> ?fechaGroupEnd.}}");
            where2.Append("BIND(IF(bound(?fechaGroupEnd), ?fechaGroupEnd, 30000000000000) as ?fechaGroupEndAux)");
            where2.Append("BIND(IF(bound(?fechaGroupInit), ?fechaGroupInit, 10000000000000) as ?fechaGroupInitAux)");

            where2.Append("FILTER(?fechaGroupEndAux >= ?fechaProjInitAux AND ?fechaGroupInitAux <= ?fechaProjEndAux)");

            // where2.Append($@"FILTER(?persona = <{idGrafoBusqueda}>) ");
            if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
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

                    try
                    {
                        if (anyo != null && !dicResultados.ContainsKey(anyo))
                        {
                            // Si no contiene el año, creo el objeto.
                            DataFechas data = new();
                            data.numProyectosInicio = 0;
                            data.numProyectosFin = numProyectos;
                            dicResultados.Add(anyo, data);
                        }
                        else if(anyo != null)
                        {
                            // Si lo contiene, se lo agrego.
                            dicResultados[anyo].numProyectosFin += numProyectos;
                        }
                    }
                    catch (Exception e) {}
                }
            }
            #endregion

            try
            {
                // Rellenar años intermedios y ordenarlos.
                string max = "2100";
                string min = "1900";
                if (dicResultados != null && dicResultados.Count > 0)
                {
                    max = dicResultados.Keys.First();
                    min = dicResultados.Keys.Last();
                }
                UtilidadesAPI.RellenarAnys(dicResultados, max, min);
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
            catch (Exception e) { return null; }
        }


        /// <summary>
        /// Obtiene un listado con los objetos de la gráfica de relaciones entre el investigador principal con otros miembros del grupo
        /// </summary>
        /// <param name="pIdGroup">ID del recurso del grupo.</param>
        /// <param name="pParametros">En este caso, el nombre completo de la persona.</param>
        /// <returns>Listado de objetos de la gráfica.</returns>
        public List<DataGraficaColaboradores> GetDatosGraficaRedColaboradoresMainResearcher(string pIdGroup, string pParametros)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdGroup);
            Dictionary<string, string> dicNodos = new Dictionary<string, string>();
            Dictionary<string, DataQueryRelaciones> dicRelaciones = new Dictionary<string, DataQueryRelaciones>();
            Dictionary<string, int> dicPersonasColabo = new();
            SparqlObject resultadoQuery = null;
            List<DataGraficaColaboradores> colaboradores = new List<DataGraficaColaboradores>();

            string personas = $@"<{idGrafoBusqueda}>";

            if (!string.IsNullOrEmpty(pParametros))
            {
                dicNodos.Add(idGrafoBusqueda, pParametros.ToLower().Trim());
            }


            // Consulta sparql.
            string select = mPrefijos;
            select += "SELECT ?theId ?nombre COUNT(*) AS ?numRelaciones";
            //string where = $@"
            //    WHERE {{ {{
            //            SELECT *
            //            WHERE {{
            //            <{idGrafoBusqueda}> <http://w3id.org/roh/mainResearcher> ?mainrp.
            //            ?mainrp <http://w3id.org/roh/roleOf> ?mainresearcher.

            //            <{idGrafoBusqueda}> <http://xmlns.com/foaf/0.1/member> ?members1.
            //            ?members1 <http://w3id.org/roh/roleOf> ?theId.

            //            ?documento a 'document'.
            //            OPTIONAL{{?documento bibo:authorList ?listaAutores.
            //            ?listaAutores rdf:member ?theId.}}
            //            FILTER(?theId != ?mainresearcher)
            //            ?theId foaf:name ?nombre.
            //        }}
            //        }} UNION {{
            //            SELECT *
            //            WHERE {{
            //            <{idGrafoBusqueda}> <http://w3id.org/roh/mainResearcher> ?mainrp.
            //            ?mainrp <http://w3id.org/roh/roleOf> ?mainresearcher.

            //            <{idGrafoBusqueda}> <http://xmlns.com/foaf/0.1/member> ?members2.
            //            ?members2 <http://w3id.org/roh/roleOf> ?membersids2.

            //            ?proyecto a 'project'.
            //            OPTIONAL{{?proyecto vivo:relates ?relacion.
            //            ?relacion roh:roleOf ?mainresearcher.
            //            ?proyecto vivo:relates ?relacion2.
            //            ?relacion2 roh:roleOf ?id.}}
            //            FILTER (?theId IN (?membersids2 ))
            //            FILTER(?theId != ?mainresearcher)
            //            ?theId foaf:name ?nombre.

            //        }} }} }} ORDER BY DESC (COUNT(*))
            //    ";

            string where = $@"
                WHERE {{ {{
                        SELECT *
                        WHERE {{
                        <{idGrafoBusqueda}> <http://w3id.org/roh/mainResearcher> ?mainrp.
                        ?mainrp <http://w3id.org/roh/roleOf> ?mainresearcher.

                        <{idGrafoBusqueda}> <http://xmlns.com/foaf/0.1/member> ?members1.
                        ?members1 <http://w3id.org/roh/roleOf> ?theId.

                        ?documento a 'document'.
                        OPTIONAL{{?documento bibo:authorList ?listaAutores.
                        ?listaAutores rdf:member ?theId.}}
                        FILTER(?theId != ?mainresearcher)
                        ?theId foaf:name ?nombre.
                ";

            if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
                string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?theId", ref aux);
                where += filtros;
            }



            where += $@"
                    }}
                    }} UNION {{
                        SELECT *
                        WHERE {{
                        <{idGrafoBusqueda}> <http://w3id.org/roh/mainResearcher> ?mainrp.
                        ?mainrp <http://w3id.org/roh/roleOf> ?mainresearcher.

                        <{idGrafoBusqueda}> <http://xmlns.com/foaf/0.1/member> ?members2.
                        ?members2 <http://w3id.org/roh/roleOf> ?membersids2.

                        ?proyecto a 'project'.
                        OPTIONAL{{?proyecto vivo:relates ?relacion.
                        ?relacion roh:roleOf ?mainresearcher.
                        ?proyecto vivo:relates ?relacion2.
                        ?relacion2 roh:roleOf ?membersids2.}}
                        FILTER (?theId IN (?membersids2 ))
                        FILTER(?theId != ?mainresearcher)
                        ?theId foaf:name ?nombre.
                ";

            if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
                string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?theId", ref aux);
                where += filtros;
            }
            where += $@"
                    }} }} }} ORDER BY DESC (COUNT(*))
                ";


            resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

            string mainresearcher = "";

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    string id = UtilidadesAPI.GetValorFilaSparqlObject(fila, "theId");
                    int proyectosComun = Int32.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numRelaciones"));
                    string nombreColaborador = UtilidadesAPI.GetValorFilaSparqlObject(fila, "nombre");
                    mainresearcher = UtilidadesAPI.GetValorFilaSparqlObject(fila, "mainresearcher");

                    dicPersonasColabo.Add(id, proyectosComun);
                    dicNodos.Add(id, nombreColaborador.ToLower().Trim());
                    personas += ",<" + UtilidadesAPI.GetValorFilaSparqlObject(fila, "theId") + ">";
                }
            }

            if (dicNodos.Count > 0)
            {
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

                        // Crea las relaciones entre personas.
                        if (!dicRelaciones.ContainsKey(persona1))
                        {
                            dicRelaciones.Add(persona1, new DataQueryRelaciones(new List<Datos> { new Datos(persona2, veces) }));
                        }
                        else
                        {
                            if (!dicRelaciones.ContainsKey(persona2))
                            {
                                dicRelaciones.Add(persona2, new DataQueryRelaciones(new List<Datos> { new Datos(persona1, veces) }));
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
                int maximasRelaciones = 1;
                if (numColaboraciones.Count != 0)
                {
                    if (dicPersonasColabo.Values.Max() > numColaboraciones.Max())
                    {
                        maximasRelaciones = dicPersonasColabo.Values.Max();
                    }
                    else
                    {
                        maximasRelaciones = numColaboraciones.Max();
                    }
                    
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

            } else
            {
                // Set the main researcher default
                idGrafoBusqueda = ObtenerIdBusqueda(pIdGroup);
                dicNodos = new Dictionary<string, string>();
                dicRelaciones = new Dictionary<string, DataQueryRelaciones>();
                dicPersonasColabo = new();
                resultadoQuery = null;
                colaboradores = new List<DataGraficaColaboradores>();

                personas = $@"<{idGrafoBusqueda}>";


                // Consulta sparql.
                select = mPrefijos;
                select += "SELECT ?mainresearcher ?nombre COUNT(*) AS ?numRelaciones";
                where = $@"
                WHERE {{ 
                        <{idGrafoBusqueda}> roh:mainResearcher ?mainrp.
                        ?mainrp roh:roleOf ?mainresearcher.
                        ?mainresearcher foaf:name ?nombre.
                    }} ORDER BY DESC (COUNT(*)) LIMIT 10
                ";

                resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                mainresearcher = "";

                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                {
                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                    {
                        try
                        {
                            string id = UtilidadesAPI.GetValorFilaSparqlObject(fila, "mainresearcher");
                            int proyectosComun = Int32.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numRelaciones"));
                            string nombreColaborador = UtilidadesAPI.GetValorFilaSparqlObject(fila, "nombre");
                            mainresearcher = UtilidadesAPI.GetValorFilaSparqlObject(fila, "mainresearcher");

                            dicPersonasColabo.Add(id, proyectosComun);
                            dicNodos.Add(id, nombreColaborador.ToLower().Trim());
                            personas += ",<" + UtilidadesAPI.GetValorFilaSparqlObject(fila, "mainresearcher") + ">";


                            colaboradores = new List<DataGraficaColaboradores>();

                            string valor = UtilsCadenas.ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(nombreColaborador);
                            Models.DataGraficaColaboradores.Data data = new Models.DataGraficaColaboradores.Data(id, valor, null, null, null, "nodes");
                            DataGraficaColaboradores dataColabo = new DataGraficaColaboradores(data, true, true);
                            colaboradores.Add(dataColabo);
                        }
                        catch (Exception e) {}
                        
                    }
                }

                // colaboradores.Add(new DataGraficaColaboradores(new SparqlObject.Data(), true, true));

               

            }

            

            return colaboradores;
        }



        /// <summary>
        /// Obtiene un listado con los objetos de la gráfica de relaciones entre el investigador principal con otros investigadores
        /// </summary>
        /// <param name="pIdGroup">ID del recurso del grupo.</param>
        /// <param name="pParametros">En este caso, el nombre completo de la persona.</param>
        /// <returns>Listado de objetos de la gráfica.</returns>
        public List<DataGraficaColaboradores> GetDatosGraficaRedColaboradoresMainResearcherFuera(string pIdGroup, string pParametros)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdGroup);
            Dictionary<string, string> dicNodos = new Dictionary<string, string>();
            Dictionary<string, DataQueryRelaciones> dicRelaciones = new Dictionary<string, DataQueryRelaciones>();
            Dictionary<string, int> dicPersonasColabo = new();
            SparqlObject resultadoQuery = null;
            List<DataGraficaColaboradores> colaboradores = new List<DataGraficaColaboradores>();

            string personas = $@"<{idGrafoBusqueda}>";

            if (!string.IsNullOrEmpty(pParametros))
            {
                dicNodos.Add(idGrafoBusqueda, pParametros.ToLower().Trim());
            }


            // Consulta sparql.
            string select = mPrefijos;
            select += "SELECT ?id ?nombre COUNT(*) AS ?numRelaciones";
            string where = $@"
                WHERE {{ {{
                        SELECT *
                        WHERE {{
                        <{idGrafoBusqueda}> roh:mainResearcher ?mainrp.
                        ?mainrp roh:roleOf ?mainresearcher.

                        <{idGrafoBusqueda}> <http://xmlns.com/foaf/0.1/member> ?members1.
                        ?members1 roh:roleOf ?membersID.
                        
                        ?documento a 'document'.
                        ?documento bibo:authorList ?listaAutores.
                        ?listaAutores rdf:member ?mainresearcher.
                        ?documento bibo:authorList ?listaAutores2.
                        ?listaAutores2 rdf:member ?id.

                        FILTER(?id != ?mainresearcher)
                        FILTER(?id != ?membersID)
                        ?id foaf:name ?nombre.
                ";

            // Parameters
            if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
                string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?id", ref aux);
                where += filtros;
            }

            where += $@"
                   }} }} UNION {{
                        SELECT *
                        WHERE {{
                        <{idGrafoBusqueda}> roh:mainResearcher ?mainrp.
                        ?mainrp roh:roleOf ?mainresearcher.

                        <{idGrafoBusqueda}> <http://xmlns.com/foaf/0.1/member> ?members1.
                        ?members1 roh:roleOf ?membersID.

                        ?proyecto a 'project'.
                        ?proyecto vivo:relates ?relacion.
                        ?relacion roh:roleOf ?mainresearcher.
                        ?proyecto vivo:relates ?relacion2.
                        ?relacion2 roh:roleOf ?id.

                        FILTER(?id != ?mainresearcher)
                        FILTER(?id != ?membersID)
                        ?id foaf:name ?nombre.
                ";

            // Parameters
            if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
                string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?id", ref aux);
                where += filtros;
            }
            where += $@"
                    }} }} }} ORDER BY DESC (COUNT(*)) LIMIT 10
                ";

            resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

            string mainresearcher = "";

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    string id = UtilidadesAPI.GetValorFilaSparqlObject(fila, "id");
                    int proyectosComun = Int32.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numRelaciones"));
                    string nombreColaborador = UtilidadesAPI.GetValorFilaSparqlObject(fila, "nombre");
                    mainresearcher = UtilidadesAPI.GetValorFilaSparqlObject(fila, "mainresearcher");

                    dicPersonasColabo.Add(id, proyectosComun);
                    dicNodos.Add(id, nombreColaborador.ToLower().Trim());
                    personas += ",<" + UtilidadesAPI.GetValorFilaSparqlObject(fila, "id") + ">";
                }
            }

            if (dicNodos.Count > 0)
            {
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

                        // Crea las relaciones entre personas.
                        if (!dicRelaciones.ContainsKey(persona1))
                        {
                            dicRelaciones.Add(persona1, new DataQueryRelaciones(new List<Datos> { new Datos(persona2, veces) }));
                        }
                        else
                        {
                            if (!dicRelaciones.ContainsKey(persona2))
                            {
                                dicRelaciones.Add(persona2, new DataQueryRelaciones(new List<Datos> { new Datos(persona1, veces) }));
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
            }


            return colaboradores;
        }




        private string ObtenerIdBusqueda(string pIdOntologia)
        {
            Guid idCorto = mResourceApi.GetShortGuid(pIdOntologia);
            return $@"http://gnoss/{idCorto.ToString().ToUpper()}";
        }
    }
}
