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
                string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?publicacion", ref aux);
                where.Append(filtros);
            }

            where.Append("} ");
            where.Append("ORDER BY ?fechaPublicacion ");


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



        /// <summary>
        /// Obtiene los datos para crear la gráfica de proyectos.
        /// </summary>
        /// <param name="pIdGroup">ID del recurso del grupo.</param>
        /// <param name="pParametros">Filtros aplicados en las facetas.</param>
        /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        public DataGraficaPublicaciones GetDatosGraficaProyectos(string pIdGroup, string pParametros)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdGroup);
            Dictionary<string, int> dicResultados = new Dictionary<string, int>(); // Dictionary<año, numDocumentos>
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT ?fechaProjInitAux COUNT(DISTINCT ?proyecto) AS ?NumProyectos");
            where.Append("WHERE { ");
            where.Append("?proyecto a 'project'.");
            where.Append("?proyecto <http://vivoweb.org/ontology/core#relates> ?members.");
            where.Append("?members <http://w3id.org/roh/roleOf> ?people. ");
            where.Append("OPTIONAL{{ ?proyecto <http://vivoweb.org/ontology/core#start> ?fechaProjInit.}}");
            where.Append("OPTIONAL{{ ?proyecto <http://vivoweb.org/ontology/core#end> ?fechaProjEnd.}} ");
            where.Append("BIND(IF(bound(?fechaProjEnd), ?fechaProjEnd, 30000000000000) as ?fechaProjEndAux)");
            where.Append("BIND(IF(bound(?fechaProjInit), ?fechaProjInit, 10000000000000) as ?fechaProjInitAux)");
            where.Append($@"<{idGrafoBusqueda}> ?p2 ?members2.");
            where.Append("FILTER (?p2 IN (<http://xmlns.com/foaf/0.1/member>, <http://w3id.org/roh/mainResearcher> ) )");
            where.Append("?members2 <http://w3id.org/roh/roleOf> ?people. ");
            where.Append("OPTIONAL{{ ?members2 <http://vivoweb.org/ontology/core#start> ?fechaGroupInit.}}");
            where.Append("OPTIONAL{{ ?members2 <http://vivoweb.org/ontology/core#end> ?fechaGroupEnd.}}");
            where.Append("BIND(IF(bound(?fechaGroupEnd), ?fechaGroupEnd, 30000000000000) as ?fechaGroupEndAux)");
            where.Append("BIND(IF(bound(?fechaGroupInit), ?fechaGroupInit, 10000000000000) as ?fechaGroupInitAux)");
            where.Append("FILTER(?fechaGroupEndAux > ?fechaProjInitAux AND ?fechaGroupInitAux < ?fechaProjEndAux) ");




            if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
                string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?proyecto", ref aux);
                where.Append(filtros);
            }

            where.Append("} ");
            where.Append("ORDER BY ?fechaProjInitAux ");


            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    string fechaProjInitAux = UtilidadesAPI.GetValorFilaSparqlObject(fila, "fechaProjInitAux");
                    int NumProyectos = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "NumProyectos"));
                    dicResultados.Add(fechaProjInitAux, NumProyectos);
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
            Models.DataGraficaPublicaciones.Datasets datasets = new Models.DataGraficaPublicaciones.Datasets("Proyectos", UtilidadesAPI.GetValuesList(dicResultados), listaColores, listaColores, 1);
            Models.DataGraficaPublicaciones.Data data = new Models.DataGraficaPublicaciones.Data(UtilidadesAPI.GetKeysList(dicResultados), new List<Models.DataGraficaPublicaciones.Datasets> { datasets });
            Models.DataGraficaPublicaciones.Options options = new Models.DataGraficaPublicaciones.Options(new Models.DataGraficaPublicaciones.Scales(new Y(true)), new Models.DataGraficaPublicaciones.Plugins(new Models.DataGraficaPublicaciones.Title(true, "Evolución temporal proyectos"), new Models.DataGraficaPublicaciones.Legend(new Labels(true), "top", "end")));
            DataGraficaPublicaciones dataGrafica = new DataGraficaPublicaciones("bar", data, options);

            return dataGrafica;
        }


        /// <summary>
        /// Obtiene un listado con los objetos de la gráfica.
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
            select += "SELECT ?id ?nombre COUNT(*) AS ?numRelaciones";
            string where = $@"
                WHERE {{ {{
                        SELECT *
                        WHERE {{
                        <{idGrafoBusqueda}> <http://w3id.org/roh/mainResearcher> ?mainrp.
                        <{idGrafoBusqueda}> <http://xmlns.com/foaf/0.1/member> ?members1.
                        ?members1 <http://w3id.org/roh/roleOf> ?membersids.
                        ?mainrp <http://w3id.org/roh/roleOf> ?mainresearcher.
                        ?documento bibo:authorList ?listaAutores.
                        ?listaAutores rdf:member ?personaDoc.
                        FILTER(?personaDoc = ?mainresearcher)
                        ?documento bibo:authorList ?listaAutores2.
                        ?listaAutores2 rdf:member ?id.
                        FILTER(?id != ?mainresearcher)
                        FILTER(?id IN (?membersids))
                        ?id foaf:name ?nombre.
                    }}
                    }} UNION {{
                        SELECT *
                        WHERE {{
                        <{idGrafoBusqueda}> <http://w3id.org/roh/mainResearcher> ?mainrp.
                        <{idGrafoBusqueda}> <http://xmlns.com/foaf/0.1/member> ?members2.
                        ?members2 <http://w3id.org/roh/roleOf> ?membersids.
                        ?mainrp <http://w3id.org/roh/roleOf> ?mainresearcher.
                        ?proyecto vivo:relates ?relacion.
                        ?relacion roh:roleOf ?persona.
                        FILTER(?persona = ?mainresearcher)
                        ?proyecto vivo:relates ?relacion2.
                        ?relacion2 roh:roleOf ?id.
                        FILTER(?id != ?mainresearcher)
                        FILTER(?id IN (?membersids))
                        ?id foaf:name ?nombre.
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
