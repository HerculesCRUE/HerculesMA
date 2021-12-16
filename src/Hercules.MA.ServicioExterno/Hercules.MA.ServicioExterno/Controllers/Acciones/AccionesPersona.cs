using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Hercules.MA.ServicioExterno.Controllers.Utilidades;
using Hercules.MA.ServicioExterno.Models;
using Hercules.MA.ServicioExterno.Models.Graficas.DataGraficaAreasTags;
using Hercules.MA.ServicioExterno.Models.Graficas.DataItemRelacion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            string idGrafoBusqueda = UtilidadesAPI.ObtenerIdBusqueda(mResourceApi, pPersona);
            Dictionary<string, int> dicResultados = new Dictionary<string, int>();
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT COUNT(DISTINCT ?proyecto) AS ?NumProyectos COUNT(DISTINCT ?documento) AS ?NumPublicaciones COUNT(DISTINCT ?categoria) AS ?NumCategorias ");
            where.Append("WHERE {{ "); // Total Proyectos.
            where.Append("?proyecto vivo:relates ?relacion. ");
            where.Append("?proyecto gnoss:hasprivacidadCom 'publico'. ");
            where.Append("?relacion <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?persona. ");
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
            where.Append("?relacion <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?persona. ");
            where.Append($@"FILTER(?persona = <{idGrafoBusqueda}>) ");
            where.Append("?proyecto vivo:relates ?relacion2. ");
            where.Append("?relacion2 <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?id. ");
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


        ///// <summary>
        ///// Obtiene los datos para crear la gráfico de los proyectos por año.
        ///// </summary>
        ///// <param name="pIdPersona">ID del recurso de la persona.</param>
        ///// <param name="pParametros">Filtros aplicados en las facetas.</param>
        //public ObjGrafica GetDatosGraficaProyectos(string pIdPersona, string pParametros)
        //{
        //    string idGrafoBusqueda = UtilidadesAPI.ObtenerIdBusqueda(mResourceApi, pIdPersona);
        //    Dictionary<string, DataFechas> dicResultados = new();
        //    SparqlObject resultadoQuery = null;
        //    StringBuilder select1 = new(), where1 = new();
        //    StringBuilder select2 = new(), where2 = new();

        //    // Consultas sparql.

        //    #region --- Obtención de datos del año de inicio de los proyectos
        //    select1.Append(mPrefijos);
        //    select1.Append("SELECT COUNT(DISTINCT(?proyecto)) AS ?numPublicaciones ?anyoInicio ");
        //    where1.Append("WHERE { ");
        //    where1.Append("?proyecto vivo:relates ?relacion. ");
        //    where1.Append("?proyecto gnoss:hasprivacidadCom 'publico'. ");
        //    where1.Append("?proyecto vivo:start ?fecha. ");
        //    where1.Append("?proyecto vivo:end ?fechaFin. ");
        //    where1.Append("?relacion <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?persona. ");
        //    where1.Append("BIND( SUBSTR( STR(?fecha), 0, 4) AS ?anyoInicio) ");
        //    where1.Append($@"FILTER(?persona = <{idGrafoBusqueda}>) ");
        //    if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
        //    {
        //        // Creación de los filtros obtenidos por parámetros.
        //        int aux = 0;
        //        Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
        //        string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?proyecto", ref aux);
        //        where1.Append(filtros);
        //    }
        //    where1.Append("} ");
        //    where1.Append("ORDER BY ?anyoInicio ");

        //    resultadoQuery = mResourceApi.VirtuosoQuery(select1.ToString(), where1.ToString(), mIdComunidad);

        //    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
        //    {
        //        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
        //        {
        //            string anyo = UtilidadesAPI.GetValorFilaSparqlObject(fila, "anyoInicio");
        //            int numProyectos = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numPublicaciones"));

        //            if (!dicResultados.ContainsKey(anyo))
        //            {
        //                // Si no contiene el año, creo el objeto.
        //                DataFechas data = new();
        //                data.numProyectosInicio = numProyectos;
        //                data.numProyectosFin = 0;
        //                dicResultados.Add(anyo, data);
        //            }
        //            else
        //            {
        //                // Si lo contiene, se lo agrego.
        //                dicResultados[anyo].numProyectosInicio += numProyectos;
        //            }
        //        }
        //    }
        //    #endregion

        //    #region --- Obtención de datos del año de fin de los proyectos
        //    select2.Append(mPrefijos);
        //    select2.Append("SELECT COUNT(DISTINCT(?proyecto)) AS ?numPublicaciones ?anyoFin ");
        //    where2.Append("WHERE { ");
        //    where2.Append("?proyecto vivo:relates ?relacion. ");
        //    where2.Append("?proyecto gnoss:hasprivacidadCom 'publico'. ");
        //    where2.Append("?proyecto vivo:start ?fecha. ");
        //    where2.Append("?proyecto vivo:end ?fechaFin. ");
        //    where2.Append("?relacion <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?persona. ");
        //    where2.Append("BIND( SUBSTR( STR(?fechaFin), 0, 4) AS ?anyoFin) ");
        //    where2.Append($@"FILTER(?persona = <{idGrafoBusqueda}>) ");
        //    if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
        //    {
        //        // Creación de los filtros obtenidos por parámetros.
        //        int aux = 0;
        //        Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
        //        string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?proyecto", ref aux);
        //        where2.Append(filtros);
        //    }
        //    where2.Append("} ");
        //    where2.Append("ORDER BY ?anyoFin ");

        //    resultadoQuery = mResourceApi.VirtuosoQuery(select2.ToString(), where2.ToString(), mIdComunidad);

        //    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
        //    {
        //        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
        //        {
        //            string anyo = UtilidadesAPI.GetValorFilaSparqlObject(fila, "anyoFin");
        //            int numProyectos = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numPublicaciones"));

        //            if (!dicResultados.ContainsKey(anyo))
        //            {
        //                // Si no contiene el año, creo el objeto.
        //                DataFechas data = new();
        //                data.numProyectosInicio = 0;
        //                data.numProyectosFin = numProyectos;
        //                dicResultados.Add(anyo, data);
        //            }
        //            else
        //            {
        //                // Si lo contiene, se lo agrego.
        //                dicResultados[anyo].numProyectosFin += numProyectos;
        //            }
        //        }
        //    }
        //    #endregion

        //    // Rellenar años intermedios y ordenarlos.
        //    string max = "2100";
        //    string min = "1900";
        //    if (dicResultados != null && dicResultados.Count > 0)
        //    {
        //        max = dicResultados.Keys.First();
        //        min = dicResultados.Keys.Last();
        //    }
        //    UtilidadesAPI.RellenarAnys(dicResultados, max, min);
        //    dicResultados = dicResultados.OrderBy(item => item.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

        //    // Se coge los datos del número de proyectos.
        //    List<int> listaInicios = new();
        //    List<int> listaFines = new();
        //    foreach (KeyValuePair<string, DataFechas> item in dicResultados)
        //    {
        //        listaInicios.Add(item.Value.numProyectosInicio);
        //        listaFines.Add(item.Value.numProyectosFin);
        //    }

        //    // Se construye el objeto con los datos.
        //    List<DatosAnyo> listaDatos = new List<DatosAnyo>();
        //    listaDatos.Add(new DatosAnyo("Inicio", "#6cafe3", listaInicios));
        //    listaDatos.Add(new DatosAnyo("Fin", "#BF4858", listaFines));

        //    // Se crea el objeto de la gráfica.
        //    DataGraficaProyectosGroupBars dataObj = new DataGraficaProyectosGroupBars(dicResultados.Keys.ToList(), listaDatos);

        //    return new ObjGrafica("bar", dataObj, new Models.DataGraficaProyectosGroupBars.Options(20, new Models.DataGraficaProyectosGroupBars.Scales(new List<YAxes>() { new YAxes(new Models.DataGraficaProyectosGroupBars.Ticks(0)) })));
        //}

        /// <summary>
        /// Obtiene el dato del grupo de investigación el cual pertenece la persona.
        /// </summary>
        /// <param name="pIdPersona">ID del recurso de la persona.</param>
        /// <returns>Lista con los grupos de investigación pertenecientes.</returns>
        public List<string> GetGrupoInvestigacion(string pIdPersona)
        {
            string idGrafoBusqueda = UtilidadesAPI.ObtenerIdBusqueda(mResourceApi, pIdPersona);
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
        public List<Dictionary<string, string>> GetTopicsPersona(string pIdPersona)
        {
            string idGrafoBusqueda = UtilidadesAPI.ObtenerIdBusqueda(mResourceApi, pIdPersona);
            List<Dictionary<string, string>> categorias = new List<Dictionary<string, string>>();
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT DISTINCT ?nombreCategoria ?source ?identifier ");
            where.Append("WHERE { ");
            where.Append("?s ?p ?documento. ");
            where.Append("?documento bibo:authorList ?listaAutores. ");
            where.Append("?listaAutores rdf:member ?persona. ");
            where.Append("?documento roh:hasKnowledgeArea ?area. ");
            where.Append("?area roh:categoryNode ?categoria. ");
            where.Append("?categoria skos:prefLabel ?nombreCategoria. ");
            where.Append("?categoria <http://purl.org/dc/elements/1.1/source> ?source. ");
            where.Append("?categoria <http://purl.org/dc/elements/1.1/identifier> ?identifier. ");


            where.Append($@"FILTER(?persona = <{idGrafoBusqueda}>)");
            where.Append("} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    var dictRes = new Dictionary<string, string>();
                    dictRes.Add("nombreCategoria", UtilidadesAPI.GetValorFilaSparqlObject(fila, "nombreCategoria"));
                    dictRes.Add("source", UtilidadesAPI.GetValorFilaSparqlObject(fila, "source"));
                    dictRes.Add("identifier", UtilidadesAPI.GetValorFilaSparqlObject(fila, "identifier"));
                    categorias.Add(dictRes);
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
        public List<DataItemRelacion> GetDatosGraficaRedColaboradoresPersonas(string pIdPersona, string pParametros, string pNombrePersona)
        {
            //string idGrafoBusqueda = UtilidadesAPI.ObtenerIdBusqueda(mResourceApi, pIdPersona);
            //Dictionary<string, string> dicNodos = new Dictionary<string, string>();
            //Dictionary<string, DataQueryRelaciones> dicRelaciones = new Dictionary<string, DataQueryRelaciones>();
            //Dictionary<string, int> dicPersonasColabo = new();
            //SparqlObject resultadoQuery = null;

            //string personas = $@"<{idGrafoBusqueda}>";

            //if (!string.IsNullOrEmpty(pNombrePersona))
            //{
            //    dicNodos.Add(idGrafoBusqueda, pNombrePersona.ToLower().Trim());
            //}

            //// Consulta sparql.
            //string select = mPrefijos;
            //select += "SELECT ?id ?nombre COUNT(*) AS ?numRelaciones ";
            //string where = $@"
            //    WHERE {{ {{
            //            SELECT *
            //            WHERE {{
            //            ?documento bibo:authorList ?listaAutores.
            //            ?listaAutores rdf:member ?personaDoc.
            //            FILTER(?personaDoc = <{idGrafoBusqueda}>)";
            //where += $@"?documento bibo:authorList ?listaAutores2.
            //            ?listaAutores2 rdf:member ?id.
            //            ?id foaf:name ?nombre.
            //            FILTER(?id != <{idGrafoBusqueda}>)";
            //if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            //{
            //    // Creación de los filtros obtenidos por parámetros.
            //    int aux = 0;
            //    Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
            //    string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?id", ref aux);
            //    where += filtros;
            //}
            //where += $@"}}
            //        }} UNION {{
            //            SELECT *
            //            WHERE {{
            //            ?proyecto vivo:relates ?relacion.
            //            ?relacion <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?persona.
            //            FILTER(?persona = <{idGrafoBusqueda}>)";
            //where += $@"?proyecto vivo:relates ?relacion2.
            //            ?relacion2 <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?id.
            //            ?id foaf:name ?nombre.
            //            FILTER(?id != <{idGrafoBusqueda}>)";
            //if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            //{
            //    // Creación de los filtros obtenidos por parámetros.
            //    int aux = 0;
            //    Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
            //    string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?id", ref aux);
            //    where += filtros;
            //}
            //where += $@"}} }} }} ORDER BY DESC (COUNT(*)) LIMIT 10";

            //resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

            //if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            //{
            //    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
            //    {
            //        string id = UtilidadesAPI.GetValorFilaSparqlObject(fila, "id");
            //        int proyectosComun = Int32.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numRelaciones"));
            //        string nombreColaborador = UtilidadesAPI.GetValorFilaSparqlObject(fila, "nombre");

            //        dicPersonasColabo.Add(id, proyectosComun);
            //        dicNodos.Add(id, nombreColaborador.ToLower().Trim());
            //        personas += ",<" + UtilidadesAPI.GetValorFilaSparqlObject(fila, "id") + ">";
            //    }
            //}

            //KeyValuePair<string, string> proyecto = dicNodos.First();
            //foreach (KeyValuePair<string, string> item in dicNodos)
            //{
            //    if (item.Key != proyecto.Key)
            //    {
            //        dicRelaciones.Add(item.Key, new DataQueryRelaciones(new List<Datos> { new Datos(proyecto.Key, dicPersonasColabo[item.Key]) }));
            //    }
            //}

            //#region --- Crear las relaciones entre dichas personas
            //List<int> numColaboraciones = new();

            //// Consulta Sparql
            //StringBuilder select2 = new StringBuilder(mPrefijos);
            //select2.Append("SELECT ?persona1 ?persona2 (COUNT(DISTINCT ?proyectos) + COUNT(DISTINCT ?publicaciones)) AS ?numVeces ");
            //StringBuilder where2 = new StringBuilder("WHERE {{ ");
            //where2.Append("?proyectos vivo:relates ?personas. ");
            //where2.Append("?personas <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?persona1. ");
            //where2.Append("BIND(?proyectos AS ?proyectos2) ");
            //where2.Append("?proyectos2 vivo:relates ?personas2. ");
            //where2.Append("?personas2 <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?persona2. ");
            //where2.Append("?persona2 foaf:name ?nombre. ");
            //where2.Append("FILTER(?persona1 != ?persona2) ");
            //where2.Append($@"FILTER(?persona1 IN ({personas})) ");
            //where2.Append($@"FILTER(?persona2 IN ({personas})) ");
            //where2.Append("} UNION { ");
            //where2.Append("?publicaciones bibo:authorList ?lista. ");
            //where2.Append("?lista roh:item ?persona1. ");
            //where2.Append("BIND(?publicaciones AS ?publicaciones2) ");
            //where2.Append("?publicaciones2 bibo:authorList ?lista2. ");
            //where2.Append("?lista2 roh:item ?persona2. ");
            //where2.Append("?persona2 foaf:name ?nombre. ");
            //where2.Append("FILTER(?persona1 != ?persona2) ");
            //where2.Append($@"FILTER(?persona1 IN ({personas})) ");
            //where2.Append($@"FILTER(?persona2 IN ({personas})) ");
            //where2.Append("}} ");

            //resultadoQuery = mResourceApi.VirtuosoQuery(select2.ToString(), where2.ToString(), mIdComunidad);

            //if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            //{
            //    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
            //    {
            //        string persona1 = UtilidadesAPI.GetValorFilaSparqlObject(fila, "persona1");
            //        string persona2 = UtilidadesAPI.GetValorFilaSparqlObject(fila, "persona2");
            //        int veces = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numVeces"));

            //        // Guarda en una lista para sacar el máximo de relaciones que hay entre los nodos.
            //        numColaboraciones.Add(veces);

            //        // TODO: REVISAR QUE FUNCIONA BIEN DEL TODO
            //        // Crea las relaciones entre personas.
            //        if (!dicRelaciones.ContainsKey(persona1))
            //        {
            //            //dicRelaciones.Add(persona1, new DataQueryRelaciones(new List<Datos> { new Datos(persona2, veces) }));
            //            if (!dicRelaciones.ContainsKey(persona1))
            //            {
            //                bool encontrado = false;
            //                foreach (KeyValuePair<string, DataQueryRelaciones> relaciones in dicRelaciones)
            //                {
            //                    foreach (Datos datos in relaciones.Value.idRelacionados)
            //                    {
            //                        if (datos.idRelacionado == persona1)
            //                        {
            //                            encontrado = true;
            //                            break;
            //                        }
            //                    }

            //                    if (encontrado) break;
            //                }

            //                if (encontrado == false)
            //                {
            //                    dicRelaciones.Add(persona1, new DataQueryRelaciones(new List<Datos> { new Datos(persona2, veces) }));
            //                }
            //            }
            //            else
            //            {
            //                bool encontrado = false;
            //                foreach (Datos relaciones in dicRelaciones[persona2].idRelacionados)
            //                {
            //                    if (relaciones.idRelacionado == persona1)
            //                    {
            //                        encontrado = true;
            //                        break;
            //                    }
            //                }
            //                foreach (Datos relaciones in dicRelaciones[persona1].idRelacionados)
            //                {
            //                    if (relaciones.idRelacionado == persona2)
            //                    {
            //                        encontrado = true;
            //                        break;
            //                    }
            //                }

            //                if (!encontrado)
            //                {
            //                    dicRelaciones[persona1].idRelacionados.Add(new Datos(persona2, veces));
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (!dicRelaciones.ContainsKey(persona2))
            //            {
            //                bool encontrado = false;
            //                foreach (KeyValuePair<string, DataQueryRelaciones> relaciones in dicRelaciones)
            //                {
            //                    foreach (Datos datos in relaciones.Value.idRelacionados)
            //                    {
            //                        if (datos.idRelacionado == persona2)
            //                        {
            //                            encontrado = true;
            //                            break;
            //                        }
            //                    }

            //                    if (encontrado) break;
            //                }

            //                if (encontrado == false)
            //                {
            //                    dicRelaciones.Add(persona2, new DataQueryRelaciones(new List<Datos> { new Datos(persona1, veces) }));
            //                }
            //            }
            //            else
            //            {
            //                bool encontrado = false;
            //                foreach (Datos relaciones in dicRelaciones[persona1].idRelacionados)
            //                {
            //                    if (relaciones.idRelacionado == persona2)
            //                    {
            //                        encontrado = true;
            //                        break;
            //                    }
            //                }
            //                foreach (Datos relaciones in dicRelaciones[persona2].idRelacionados)
            //                {
            //                    if (relaciones.idRelacionado == persona1)
            //                    {
            //                        encontrado = true;
            //                        break;
            //                    }
            //                }

            //                if (!encontrado)
            //                {
            //                    dicRelaciones[persona2].idRelacionados.Add(new Datos(persona1, veces));
            //                }
            //            }
            //        }
            //    }
            //}
            //#endregion

            //// Construcción del objeto de la gráfica.            
            //List<DataGraficaColaboradores> colaboradores = new List<DataGraficaColaboradores>();
            //int maximasRelaciones = 1;
            //if (dicPersonasColabo.Values.Max() > numColaboraciones.Max())
            //{
            //    maximasRelaciones = dicPersonasColabo.Values.Max();
            //}
            //else
            //{
            //    maximasRelaciones = numColaboraciones.Max();
            //}

            //// Nodos. 
            //if (dicNodos != null && dicNodos.Count > 0)
            //{
            //    foreach (KeyValuePair<string, string> nodo in dicNodos)
            //    {
            //        string clave = nodo.Key;
            //        string valor = UtilsCadenas.ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(nodo.Value);
            //        Models.DataGraficaColaboradores.Data data = new Models.DataGraficaColaboradores.Data(clave, valor, null, null, null, "nodes");
            //        DataGraficaColaboradores dataColabo = new DataGraficaColaboradores(data, true, true);
            //        colaboradores.Add(dataColabo);
            //    }
            //}

            //// Relaciones.
            //if (dicRelaciones != null && dicRelaciones.Count > 0)
            //{
            //    foreach (KeyValuePair<string, DataQueryRelaciones> sujeto in dicRelaciones)
            //    {
            //        foreach (Datos relaciones in sujeto.Value.idRelacionados)
            //        {
            //            string id = $@"{sujeto.Key}~{relaciones.idRelacionado}~{relaciones.numVeces}";
            //            Models.DataGraficaColaboradores.Data data = new Models.DataGraficaColaboradores.Data(id, null, sujeto.Key, relaciones.idRelacionado, UtilidadesAPI.CalcularGrosor(maximasRelaciones, relaciones.numVeces), "edges");
            //            DataGraficaColaboradores dataColabo = new DataGraficaColaboradores(data, null, null);
            //            colaboradores.Add(dataColabo);
            //        }
            //    }
            //}

            //return colaboradores;
            return null;
        }

        /// <summary>
        /// Obtiene el objeto de la gráfica horizontal de topics.
        /// </summary>
        /// <param name="pIdPersona">ID del recurso de la persona.</param>
        /// <param name="pParametros">Filtros de las facetas.</param>
        /// <returns>Objeto que se trata en JS para contruir la gráfica.</returns>
        public DataGraficaAreasTags GetDatosGraficaProyectosPersonaHorizontal(string pIdPersona, string pParametros)
        {
            string idGrafoBusqueda = UtilidadesAPI.ObtenerIdBusqueda(mResourceApi, pIdPersona);
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
