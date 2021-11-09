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
        private static string COLOR_GRAFICAS_HORIZONTAL = "#1177ff";
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
	                    ?members <http://w3id.org/roh/roleOf> ?people. 
	                    OPTIONAL{{ ?proyecto <http://vivoweb.org/ontology/core#start> ?fechaProjInit.}}
	                    OPTIONAL{{ ?proyecto <http://vivoweb.org/ontology/core#end> ?fechaProjEnd.}} 
                        BIND(IF(bound(?fechaProjEnd), ?fechaProjEnd, 30000000000000) as ?fechaProjEndAux)
                        BIND(IF(bound(?fechaProjInit), ?fechaProjInit, 10000000000000) as ?fechaProjInitAux)
			            <{idGrafoBusqueda}> ?p2 ?members2.
			            FILTER (?p2 IN (<http://xmlns.com/foaf/0.1/member>, <http://w3id.org/roh/mainResearcher> ) )
			            ?members2 <http://w3id.org/roh/roleOf> ?people. 
			            OPTIONAL{{ ?members2 <http://vivoweb.org/ontology/core#start> ?fechaGroupInit.}}
			            OPTIONAL{{ ?members2 <http://vivoweb.org/ontology/core#end> ?fechaGroupEnd.}}
			            BIND(IF(bound(?fechaGroupEnd), ?fechaGroupEnd, 30000000000000) as ?fechaGroupEndAux)
                        BIND(IF(bound(?fechaGroupInit), ?fechaGroupInit, 10000000000000) as ?fechaGroupInitAux)
                        FILTER(?fechaGroupEndAux > ?fechaProjInitAux AND ?fechaGroupInitAux < ?fechaProjEndAux)                                          
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
	                    ?person a 'person'.                                       
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
            select.Append("SELECT ?fechaPublicacion COUNT(DISTINCT(?publicacion)) AS ?NumPublicaciones ");
            where.Append("WHERE { ");
            where.Append($@"<{idGrafoBusqueda}> ?propmembers ?members.");

            where.Append("?members <http://w3id.org/roh/roleOf> ?person.");
            where.Append("FILTER(?propmembers  in (<http://xmlns.com/foaf/0.1/member>, <http://w3id.org/roh/mainResearcher>))");
            where.Append("?person a 'person'.");
            where.Append("OPTIONAL{{ ?members2 <http://vivoweb.org/ontology/core#start> ?fechaPersonaInit.}}");
            where.Append("OPTIONAL{{ ?members2 <http://vivoweb.org/ontology/core#end> ?fechaPersonaEnd.}}");
            where.Append("BIND(IF(bound(?fechaPersonaEnd), ?fechaPersonaEnd, 30000000000000) as ?fechaPersonaEndAux)");
            where.Append("BIND(IF(bound(?fechaPersonaInit), ?fechaPersonaInit, 10000000000000) as ?fechaPersonaInitAux)");
            where.Append("?publicacion a 'document'.");
            where.Append("?publicacion <http://purl.org/ontology/bibo/authorList> ?author.");
            where.Append("?author <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.");
            where.Append("?publicacion <http://purl.org/dc/terms/issued> ?fechaPublicacion.");
            where.Append("FILTER(?fechaPublicacion>= ?fechaPersonaInitAux AND ?fechaPublicacion<= ?fechaPersonaEndAux)");
            // Parameters
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
                    string fechaPublicacion = UtilidadesAPI.GetValorFilaSparqlObject(fila, "fechaPublicacion");
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


        private string ObtenerIdBusqueda(string pIdOntologia)
        {
            Guid idCorto = mResourceApi.GetShortGuid(pIdOntologia);
            return $@"http://gnoss/{idCorto.ToString().ToUpper()}";
        }
    }
}
