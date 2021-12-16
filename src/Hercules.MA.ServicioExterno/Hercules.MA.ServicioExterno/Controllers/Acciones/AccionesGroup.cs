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
        /// Obtiene el objeto de la gráfica horizontal de topics (Áreas temáticas).
        /// </summary>
        /// <param name="pIdGrupo">ID del recurso del grupo.</param>
        /// <returns>Objeto que se trata en JS para contruir la gráfica.</returns>
        public DataGraficaAreasTags DatosGraficaAreasTematicasGrupo(string pIdGrupo)
        {
            string idGrafoBusqueda = UtilidadesAPI.ObtenerIdBusqueda(mResourceApi, pIdGrupo);
            Dictionary<string, int> dicResultados = new Dictionary<string, int>();
            int numDocumentos = 0;
            {
                //Nº de documentos por categoría
                SparqlObject resultadoQuery = null;
                string select = $"{mPrefijos} Select ?nombreCategoria count(distinct ?documento) as ?numCategorias";
                string where = $@"  where
                                {{
                                    ?documento a 'document'. 
                                    ?documento roh:isProducedBy <{idGrafoBusqueda}>.
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
                                    ?documento roh:isProducedBy <{idGrafoBusqueda}>.
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
            List<string> listaColores = UtilidadesAPI.CrearListaColores(dicionarioOrdenado.Count(), COLOR_GRAFICAS_HORIZONTAL);
            Datasets datasets = new Datasets(dicResultadosPorcentaje.Values.ToList(), listaColores);
            Models.Graficas.DataGraficaAreasTags.Data data = new Models.Graficas.DataGraficaAreasTags.Data(dicResultadosPorcentaje.Keys.ToList(), new List<Datasets> { datasets });

            // Máximo.
            x xAxes = new x(new Ticks(0, 100), new ScaleLabel(true, "Percentage"));

            Options options = new Options("y", new Plugins(new Title(true, "Resultados de la investigación por fuente de datos"), new Legend(false)), new Scales(xAxes));
            DataGraficaAreasTags dataGrafica = new DataGraficaAreasTags("bar", data, options);

            return dataGrafica;
        }



        //    /// <summary>
        //    /// Obtiene los datos para crear la gráfica de proyectos.
        //    /// </summary>
        //    /// <param name="pIdGroup">ID del recurso del grupo.</param>
        //    /// <param name="pParametros">Filtros aplicados en las facetas.</param>
        //    /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        //    public ObjGrafica GetDatosGraficaProyectos(string pIdGroup, string pParametros)
        //    {

        //        string idGrafoBusqueda = UtilidadesAPI.ObtenerIdBusqueda(mResourceApi, pIdGroup);
        //        Dictionary<string, DataFechas> dicResultados = new();
        //        SparqlObject resultadoQuery = null;
        //        StringBuilder select1 = new(), where1 = new();
        //        StringBuilder select2 = new(), where2 = new();

        //        // Consultas sparql.

        //        #region --- Obtención de datos del año de inicio de los proyectos
        //        select1.Append(mPrefijos);
        //        select1.Append("SELECT COUNT(DISTINCT(?proyecto)) AS ?numPublicaciones ?anyoInicio ");
        //        where1.Append("WHERE { ");
        //        where1.Append("?proyecto vivo:relates ?relacion. ");
        //        where1.Append("?proyecto gnoss:hasprivacidadCom 'publico'. ");
        //        where1.Append("?relacion <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?persona. ");
        //        where1.Append("OPTIONAL{?proyecto <http://vivoweb.org/ontology/core#start> ?fechaProjInit.}");
        //        where1.Append("OPTIONAL{?proyecto <http://vivoweb.org/ontology/core#end> ?fechaProjEnd.}");
        //        where1.Append("BIND(IF(bound(?fechaProjInit), ?fechaProjInit, 10000000000000) as ?fechaProjInitAux)");
        //        where1.Append("BIND(IF(bound(?fechaProjEnd), ?fechaProjEnd, 30000000000000) as ?fechaProjEndAux)");
        //        where1.Append("BIND( SUBSTR( STR(?fechaProjInit), 0, 4) AS ?anyoInicio) ");

        //        where1.Append($@"<{idGrafoBusqueda}> ?p2 ?members2.");
        //        where1.Append("FILTER (?p2 IN (<http://xmlns.com/foaf/0.1/member>, <http://w3id.org/roh/mainResearcher> ))");
        //        where1.Append("?members2 <http://w3id.org/roh/roleOf> ?persona. ");
        //        where1.Append("OPTIONAL{{?members2 <http://vivoweb.org/ontology/core#start> ?fechaGroupInit.}}");
        //        where1.Append("OPTIONAL{{?members2 <http://vivoweb.org/ontology/core#end> ?fechaGroupEnd.}}");
        //        where1.Append("BIND(IF(bound(?fechaGroupEnd), ?fechaGroupEnd, 30000000000000) as ?fechaGroupEndAux)");
        //        where1.Append("BIND(IF(bound(?fechaGroupInit), ?fechaGroupInit, 10000000000000) as ?fechaGroupInitAux)");

        //        where1.Append("FILTER(?fechaGroupEndAux >= ?fechaProjInitAux AND ?fechaGroupInitAux <= ?fechaProjEndAux)");

        //        // where1.Append($@"FILTER(?persona = <{idGrafoBusqueda}>) ");
        //        if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
        //        {
        //            // Creación de los filtros obtenidos por parámetros.
        //            int aux = 0;
        //            Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
        //            string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?proyecto", ref aux);
        //            where1.Append(filtros);
        //        }
        //        where1.Append("} ");
        //        where1.Append("ORDER BY ?anyoInicio ");

        //        resultadoQuery = mResourceApi.VirtuosoQuery(select1.ToString(), where1.ToString(), mIdComunidad);

        //        if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
        //        {
        //            foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
        //            {
        //                string anyo = UtilidadesAPI.GetValorFilaSparqlObject(fila, "anyoInicio");
        //                int numProyectos = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numPublicaciones"));

        //                try
        //                {
        //                    if (!dicResultados.ContainsKey(anyo))
        //                    {
        //                        // Si no contiene el año, creo el objeto.
        //                        DataFechas data = new();
        //                        data.numProyectosInicio = numProyectos;
        //                        data.numProyectosFin = 0;
        //                        dicResultados.Add(anyo, data);
        //                    }
        //                    else
        //                    {
        //                        // Si lo contiene, se lo agrego.
        //                        dicResultados[anyo].numProyectosInicio += numProyectos;
        //                    }
        //                }
        //                catch (Exception e) { }
        //            }
        //        }
        //        #endregion

        //        #region --- Obtención de datos del año de fin de los proyectos
        //        select2.Append(mPrefijos);
        //        select2.Append("SELECT COUNT(DISTINCT(?proyecto)) AS ?numPublicaciones ?anyoFin ");
        //        where2.Append("WHERE { ");
        //        where2.Append("?proyecto vivo:relates ?relacion. ");
        //        where2.Append("?proyecto gnoss:hasprivacidadCom 'publico'. ");
        //        where2.Append("?relacion <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?persona. ");
        //        where2.Append("OPTIONAL{?proyecto <http://vivoweb.org/ontology/core#start> ?fechaProjInit.}");
        //        where2.Append("OPTIONAL{?proyecto <http://vivoweb.org/ontology/core#end> ?fechaProjEnd.}");
        //        where2.Append("BIND(IF(bound(?fechaProjEnd), ?fechaProjEnd, 30000000000000) as ?fechaProjEndAux)");
        //        where2.Append("BIND(IF(bound(?fechaProjInit), ?fechaProjInit, 10000000000000) as ?fechaProjInitAux)");
        //        where2.Append("BIND( SUBSTR( STR(?fechaProjEnd), 0, 4) AS ?anyoFin) ");

        //        where2.Append($@"<{idGrafoBusqueda}> ?p2 ?members2.");
        //        where2.Append("FILTER (?p2 IN (<http://xmlns.com/foaf/0.1/member>, <http://w3id.org/roh/mainResearcher> ))");
        //        where2.Append("?members2 <http://w3id.org/roh/roleOf> ?persona. ");
        //        where2.Append("OPTIONAL{{?members2 <http://vivoweb.org/ontology/core#start> ?fechaGroupInit.}}");
        //        where2.Append("OPTIONAL{{?members2 <http://vivoweb.org/ontology/core#end> ?fechaGroupEnd.}}");
        //        where2.Append("BIND(IF(bound(?fechaGroupEnd), ?fechaGroupEnd, 30000000000000) as ?fechaGroupEndAux)");
        //        where2.Append("BIND(IF(bound(?fechaGroupInit), ?fechaGroupInit, 10000000000000) as ?fechaGroupInitAux)");

        //        where2.Append("FILTER(?fechaGroupEndAux >= ?fechaProjInitAux AND ?fechaGroupInitAux <= ?fechaProjEndAux)");

        //        // where2.Append($@"FILTER(?persona = <{idGrafoBusqueda}>) ");
        //        if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
        //        {
        //            // Creación de los filtros obtenidos por parámetros.
        //            int aux = 0;
        //            Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
        //            string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?proyecto", ref aux);
        //where2.Append(filtros);
        //        }
        //        where2.Append("} ");
        //        where2.Append("ORDER BY ?anyoFin ");

        //        resultadoQuery = mResourceApi.VirtuosoQuery(select2.ToString(), where2.ToString(), mIdComunidad);

        //        if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
        //        {
        //            foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
        //            {
        //                string anyo = UtilidadesAPI.GetValorFilaSparqlObject(fila, "anyoFin");
        //                int numProyectos = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numPublicaciones"));

        //                try
        //                {
        //                    if (anyo != null && !dicResultados.ContainsKey(anyo))
        //                    {
        //                        // Si no contiene el año, creo el objeto.
        //                        DataFechas data = new();
        //                        data.numProyectosInicio = 0;
        //                        data.numProyectosFin = numProyectos;
        //                        dicResultados.Add(anyo, data);
        //                    }
        //                    else if(anyo != null)
        //                    {
        //                        // Si lo contiene, se lo agrego.
        //                        dicResultados[anyo].numProyectosFin += numProyectos;
        //                    }
        //                }
        //                catch (Exception e) {}
        //            }
        //        }
        //        #endregion

        //        try
        //        {
        //            // Rellenar años intermedios y ordenarlos.
        //            string max = "2100";
        //            string min = "1900";
        //            if (dicResultados != null && dicResultados.Count > 0)
        //            {
        //                max = dicResultados.Keys.First();
        //                min = dicResultados.Keys.Last();
        //            }
        //            UtilidadesAPI.RellenarAnys(dicResultados, max, min);
        //            dicResultados = dicResultados.OrderBy(item => item.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

        //            // Se coge los datos del número de proyectos.
        //            List<int> listaInicios = new();
        //            List<int> listaFines = new();
        //            foreach (KeyValuePair<string, DataFechas> item in dicResultados)
        //            {
        //                listaInicios.Add(item.Value.numProyectosInicio);
        //                listaFines.Add(item.Value.numProyectosFin);
        //            }

        //            // Se construye el objeto con los datos.
        //            List<DatosAnyo> listaDatos = new List<DatosAnyo>();
        //            listaDatos.Add(new DatosAnyo("Inicio", "#6cafe3", listaInicios));
        //            listaDatos.Add(new DatosAnyo("Fin", "#BF4858", listaFines));

        //            // Se crea el objeto de la gráfica.
        //            DataGraficaProyectosGroupBars dataObj = new DataGraficaProyectosGroupBars(dicResultados.Keys.ToList(), listaDatos);

        //            return new ObjGrafica("bar", dataObj, new Models.DataGraficaProyectosGroupBars.Options(20, new Models.DataGraficaProyectosGroupBars.Scales(new List<YAxes>() { new YAxes(new Models.DataGraficaProyectosGroupBars.Ticks(0)) })));

        //        }
        //        catch (Exception e) { return null; }
        //    }


        public List<DataItemRelacion> DatosGraficaMiembrosGrupo(string pIdGroup, string pParametros)
        {
            HashSet<string> miembros = new HashSet<string>();
            HashSet<string> ip = new HashSet<string>();

            //Nodos            
            Dictionary<string, string> dicNodos = new Dictionary<string, string>();
            //Relaciones
            Dictionary<string, List<DataQueryRelaciones>> dicRelaciones = new Dictionary<string, List<DataQueryRelaciones>>();
            //Respuesta
            List<DataItemRelacion> items = new List<DataItemRelacion>();

            int aux = 0;
            Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
            string filtrosPersonas = UtilidadesAPI.CrearFiltros(dicParametros, "?person", ref aux);


            #region Cargamos nodos
            {
                //Miembros
                string select = $@"{mPrefijos}
                                select distinct ?person ?nombre ?ip";
                string where = $@"
                WHERE {{ 
                        {filtrosPersonas}
                        ?person a 'person'.
                        ?person foaf:name ?nombre.
                        {{
                            <http://gnoss/{pIdGroup}> <http://w3id.org/roh/mainResearchers> ?main.
                            ?main <http://w3id.org/roh/roleOf> ?person.
                            OPTIONAL{{?main <http://vivoweb.org/ontology/core#start> ?fechaPersonaInit.}}
                            OPTIONAL{{?main <http://vivoweb.org/ontology/core#end> ?fechaPersonaEnd.}}
                            BIND(IF(bound(?fechaPersonaEnd), xsd:integer(?fechaPersonaEnd), 30000000000000) as ?fechaPersonaEndAux)
                            BIND(IF(bound(?fechaPersonaInit), xsd:integer(?fechaPersonaInit), 10000000000000) as ?fechaPersonaInitAux)
                            BIND(true as ?ip)
                        }}UNION
                        {{
                            <http://gnoss/{pIdGroup}> <http://xmlns.com/foaf/0.1/member> ?member.
                            ?member <http://w3id.org/roh/roleOf> ?person.
                            OPTIONAL{{?person <http://vivoweb.org/ontology/core#start> ?fechaPersonaInit.}}
                            OPTIONAL{{?person <http://vivoweb.org/ontology/core#end> ?fechaPersonaEnd.}}
                            BIND(IF(bound(?fechaPersonaEnd), xsd:integer(?fechaPersonaEnd), 30000000000000) as ?fechaPersonaEndAux)
                            BIND(IF(bound(?fechaPersonaInit), xsd:integer(?fechaPersonaInit), 10000000000000) as ?fechaPersonaInitAux)
                            BIND(false as ?ip)
                        }}
                        FILTER(?fechaPersonaInitAux<={DateTime.Now.ToString("yyyyMMddHHmmss")} AND ?fechaPersonaEndAux>={DateTime.Now.ToString("yyyyMMddHHmmss")} )
                        
                }}";

                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);
                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                {
                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                    {
                        if (!dicNodos.ContainsKey(fila["person"].value))
                        {
                            dicNodos.Add(fila["person"].value, fila["nombre"].value);
                        }
                        if (fila.ContainsKey("ip") && (fila["ip"].value == "1" || fila["ip"].value == "true"))
                        {
                            ip.Add(fila["person"].value);
                        }
                        else
                        {
                            miembros.Add(fila["person"].value);
                        }
                    }
                }
                miembros.ExceptWith(ip);
            }
            {
                //Grupo
                string select = $@"{mPrefijos}
                                select distinct ?nombre";
                string where = $@"
                WHERE {{ 
                        <http://gnoss/{pIdGroup}> roh:title ?nombre.                        
                }}";

                string nombreGrupo = mResourceApi.VirtuosoQuery(select, where, mIdComunidad).results.bindings.First()["nombre"].value;
                dicNodos.Add("http://gnoss/" + pIdGroup, nombreGrupo);
            }
            #endregion

            if (miembros.Union(ip).Count() > 0)
            {
                #region Relaciones con el grupo
                {
                    //Proyectos
                    {
                        string select = "SELECT ?person COUNT(distinct ?project) AS ?numRelacionesProyectos";
                        string where = $@"
                    WHERE {{ 
                            ?project a 'project'.
                            ?project <http://w3id.org/roh/publicGroupList> <http://gnoss/{pIdGroup}>.
                            ?project ?propRol ?rolProy.
                            FILTER(?propRol in (<http://vivoweb.org/ontology/core#relates>,<http://vivoweb.org/ontology/core#mainRersearchers>))
                            ?rolProy <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
                            FILTER(?person in (<{string.Join(">,<", miembros.Union(ip))}>))
                        }}order by desc(?numRelacionesProyectos)";
                        SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            string person = fila["person"].value;
                            string group = "http://gnoss/" + pIdGroup.ToUpper();
                            int numRelaciones = int.Parse(fila["numRelacionesProyectos"].value);
                            string nombreRelacion = "Proyectos";
                            if (!dicRelaciones.ContainsKey(group))
                            {
                                dicRelaciones.Add(group, new List<DataQueryRelaciones>());
                            }

                            DataQueryRelaciones dataQueryRelaciones = (dicRelaciones[group].FirstOrDefault(x => x.nombreRelacion == nombreRelacion));
                            if (dataQueryRelaciones == null)
                            {
                                dataQueryRelaciones = new DataQueryRelaciones()
                                {
                                    nombreRelacion = nombreRelacion,
                                    idRelacionados = new List<Datos>()
                                };
                                dicRelaciones[group].Add(dataQueryRelaciones);
                            }
                            dataQueryRelaciones.idRelacionados.Add(new Datos()
                            {
                                idRelacionado = person,
                                numVeces = numRelaciones
                            });
                        }
                    }
                    //DOCUMENTOS
                    {
                        string select = "SELECT ?person COUNT(distinct ?document) AS ?numRelacionesDocumentos";
                        string where = $@"
                    WHERE {{ 
                            ?document a 'document'.
                            ?document <http://w3id.org/roh/isProducedBy> <http://gnoss/{pIdGroup}>.
                            ?document <http://w3id.org/roh/publicAuthorList> ?person. 
                            FILTER(?person in (<{string.Join(">,<", miembros.Union(ip))}>))
                        }}order by desc(?numRelacionesDocumentos)";
                        SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            string person = fila["person"].value;
                            string group = "http://gnoss/" + pIdGroup.ToUpper();
                            int numRelaciones = int.Parse(fila["numRelacionesDocumentos"].value);
                            string nombreRelacion = "Documentos";
                            if (!dicRelaciones.ContainsKey(group))
                            {
                                dicRelaciones.Add(group, new List<DataQueryRelaciones>());
                            }

                            DataQueryRelaciones dataQueryRelaciones = (dicRelaciones[group].FirstOrDefault(x => x.nombreRelacion == nombreRelacion));
                            if (dataQueryRelaciones == null)
                            {
                                dataQueryRelaciones = new DataQueryRelaciones()
                                {
                                    nombreRelacion = nombreRelacion,
                                    idRelacionados = new List<Datos>()
                                };
                                dicRelaciones[group].Add(dataQueryRelaciones);
                            }
                            dataQueryRelaciones.idRelacionados.Add(new Datos()
                            {
                                idRelacionado = person,
                                numVeces = numRelaciones
                            });
                        }
                    }
                }
                #endregion

                #region Relaciones entre miembros
                {
                    //Proyectos
                    {
                        string select = "SELECT ?person group_concat(?project;separator=\",\") as ?projects";
                        string where = $@"
                    WHERE {{ 
                            ?project a 'project'.
                            ?project ?propRol ?rol
                            FILTER(?propRol in (<http://vivoweb.org/ontology/core#relates>,<http://vivoweb.org/ontology/core#mainRersearchers>))
                            ?rol <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
                            FILTER(?person in (<{string.Join(">,<", miembros.Union(ip))}>))
                        }}";
                        SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);
                        Dictionary<string, HashSet<string>> personaProy = new Dictionary<string, HashSet<string>>();
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            string projects = fila["projects"].value;
                            string person = fila["person"].value;
                            personaProy.Add(person, new HashSet<string>(projects.Split(',')));
                        }
                        UtilidadesAPI.ProcessRelations("Proyectos", personaProy, ref dicRelaciones);
                    }
                    //DOCUMENTOS
                    {
                        string select = "SELECT ?person group_concat(?document;separator=\",\") as ?documents";
                        string where = $@"
                    WHERE {{ 
                            ?document a 'document'.
                            ?document <http://purl.org/ontology/bibo/authorList> ?authorList.
                            ?authorList <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
                            FILTER(?person in (<{string.Join(">,<", miembros.Union(ip))}>))
                        }}";
                        SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);
                        Dictionary<string, HashSet<string>> personaDoc = new Dictionary<string, HashSet<string>>();
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            string documents = fila["documents"].value;
                            string person = fila["person"].value;
                            personaDoc.Add(person, new HashSet<string>(documents.Split(',')));
                        }
                        UtilidadesAPI.ProcessRelations("Documentos", personaDoc, ref dicRelaciones);
                    }
                }
                #endregion

                int maximasRelaciones = 0;
                foreach (KeyValuePair<string, List<DataQueryRelaciones>> sujeto in dicRelaciones)
                {
                    foreach (DataQueryRelaciones relaciones in sujeto.Value)
                    {
                        foreach (Datos relaciones2 in relaciones.idRelacionados)
                        {
                            maximasRelaciones = Math.Max(maximasRelaciones, relaciones2.numVeces);
                        }
                    }
                }

                // Nodos. 
                if (dicNodos != null && dicNodos.Count > 0)
                {
                    foreach (KeyValuePair<string, string> nodo in dicNodos)
                    {
                        string clave = nodo.Key;
                        string valor = UtilsCadenas.ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(nodo.Value);
                        Models.Graficas.DataItemRelacion.Data.Type type = Models.Graficas.DataItemRelacion.Data.Type.none;
                        if (ip.Contains(nodo.Key))
                        {
                            type = Models.Graficas.DataItemRelacion.Data.Type.icon_ip;
                        }
                        else if (miembros.Contains(nodo.Key))
                        {
                            type = Models.Graficas.DataItemRelacion.Data.Type.icon_member;
                        }
                        Models.Graficas.DataItemRelacion.Data data = new Models.Graficas.DataItemRelacion.Data(clave, valor, null, null, null, "nodes", type);
                        DataItemRelacion dataColabo = new DataItemRelacion(data, true, true);
                        items.Add(dataColabo);
                    }
                }

                // Relaciones.
                if (dicRelaciones != null && dicRelaciones.Count > 0)
                {
                    foreach (KeyValuePair<string, List<DataQueryRelaciones>> sujeto in dicRelaciones)
                    {
                        foreach (DataQueryRelaciones relaciones in sujeto.Value)
                        {
                            foreach (Datos relaciones2 in relaciones.idRelacionados)
                            {
                                string id = $@"{sujeto.Key}~{relaciones.nombreRelacion}~{relaciones2.idRelacionado}~{relaciones2.numVeces}";
                                Models.Graficas.DataItemRelacion.Data.Type type = Models.Graficas.DataItemRelacion.Data.Type.none;
                                if (relaciones.nombreRelacion == "Proyectos")
                                {
                                    type = Models.Graficas.DataItemRelacion.Data.Type.relation_project;
                                }
                                else if (relaciones.nombreRelacion == "Documentos")
                                {
                                    type = Models.Graficas.DataItemRelacion.Data.Type.relation_document;
                                }
                                Models.Graficas.DataItemRelacion.Data data = new Models.Graficas.DataItemRelacion.Data(id, relaciones.nombreRelacion, sujeto.Key, relaciones2.idRelacionado, UtilidadesAPI.CalcularGrosor(maximasRelaciones, relaciones2.numVeces), "edges", type);
                                DataItemRelacion dataColabo = new DataItemRelacion(data, null, null);
                                items.Add(dataColabo);
                            }
                        }
                    }
                }
            }
            return items;
        }



        public List<DataItemRelacion> DatosGraficaColaboradoresGrupo(string pIdGroup, string pParametros, int pMax)
        {
            HashSet<string> colaboradores = new HashSet<string>();
            Dictionary<string, int> numRelacionesColaboradorGrupo = new Dictionary<string, int>();
            Dictionary<string, int> numRelacionesColaboradorDocumentoGrupo = new Dictionary<string, int>();
            Dictionary<string, int> numRelacionesColaboradorProyectoGrupo = new Dictionary<string, int>();

            //Nodos            
            Dictionary<string, string> dicNodos = new Dictionary<string, string>();
            //Relaciones
            Dictionary<string, List<DataQueryRelaciones>> dicRelaciones = new Dictionary<string, List<DataQueryRelaciones>>();
            //Respuesta
            List<DataItemRelacion> items = new List<DataItemRelacion>();

            int aux = 0;
            Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
            string filtrosPersonas = UtilidadesAPI.CrearFiltros(dicParametros, "?person", ref aux);


            #region Cargamos nodos
            {
                //Miembros
                string select = $@"{mPrefijos}
                                select distinct ?person ?nombre";
                string where = $@"
                WHERE {{ 
                        {filtrosPersonas}
                        ?person a 'person'.
                        ?person foaf:name ?nombre.
                }}";

                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);
                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                {
                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                    {
                        if (!dicNodos.ContainsKey(fila["person"].value))
                        {
                            dicNodos.Add(fila["person"].value, fila["nombre"].value);
                        }
                        colaboradores.Add(fila["person"].value);
                    }
                }
            }
            {
                //Grupo
                string select = $@"{mPrefijos}
                                select distinct ?nombre";
                string where = $@"
                WHERE {{ 
                        <http://gnoss/{pIdGroup}> roh:title ?nombre.                        
                }}";

                string nombreGrupo = mResourceApi.VirtuosoQuery(select, where, mIdComunidad).results.bindings.First()["nombre"].value;
                dicNodos.Add("http://gnoss/" + pIdGroup, nombreGrupo);
            }
            #endregion
            if (colaboradores.Count > 0)
            {
                #region Relaciones con el grupo
                {
                    //Proyectos
                    {
                        string select = "SELECT ?person COUNT(distinct ?project) AS ?numRelacionesProyectos";
                        string where = $@"
                    WHERE {{ 
                            ?project a 'project'.
                            ?project <http://w3id.org/roh/publicGroupList> <http://gnoss/{pIdGroup}>.
                            ?project ?propRol ?rolProy.
                            FILTER(?propRol in (<http://vivoweb.org/ontology/core#relates>,<http://vivoweb.org/ontology/core#mainRersearchers>))
                            ?rolProy <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
                            FILTER(?person in (<{string.Join(">,<", colaboradores)}>))
                        }}order by desc(?numRelacionesProyectos)";
                        SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            string person = fila["person"].value;
                            int numRelaciones = int.Parse(fila["numRelacionesProyectos"].value);
                            if (!numRelacionesColaboradorGrupo.ContainsKey(person))
                            {
                                numRelacionesColaboradorGrupo[person] = 0;
                            }
                            if (!numRelacionesColaboradorProyectoGrupo.ContainsKey(person))
                            {
                                numRelacionesColaboradorProyectoGrupo[person] = 0;
                            }
                            numRelacionesColaboradorGrupo[person] += numRelaciones;
                            numRelacionesColaboradorProyectoGrupo[person] += numRelaciones;
                        }
                    }
                    //DOCUMENTOS
                    {
                        string select = "SELECT ?person COUNT(distinct ?document) AS ?numRelacionesDocumentos";
                        string where = $@"
                    WHERE {{ 
                            ?document a 'document'.
                            ?document <http://w3id.org/roh/isProducedBy> <http://gnoss/{pIdGroup}>.
                            ?document <http://w3id.org/roh/publicAuthorList> ?person. 
                            FILTER(?person in (<{string.Join(">,<", colaboradores)}>))
                        }}order by desc(?numRelacionesDocumentos)";
                        SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            string person = fila["person"].value;
                            int numRelaciones = int.Parse(fila["numRelacionesDocumentos"].value);
                            if (!numRelacionesColaboradorGrupo.ContainsKey(person))
                            {
                                numRelacionesColaboradorGrupo[person] = 0;
                            }
                            if (!numRelacionesColaboradorDocumentoGrupo.ContainsKey(person))
                            {
                                numRelacionesColaboradorDocumentoGrupo[person] = 0;
                            }
                            numRelacionesColaboradorGrupo[person] += numRelaciones;
                            numRelacionesColaboradorDocumentoGrupo[person] += numRelaciones;
                        }
                    }
                }
                #endregion

                //Seleccionamos los pMax colaboradores mas relacionados con el grupo
                numRelacionesColaboradorGrupo = numRelacionesColaboradorGrupo.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                if (numRelacionesColaboradorGrupo.Count > pMax)
                {
                    colaboradores = new HashSet<string>(numRelacionesColaboradorGrupo.Keys.ToList().GetRange(0, pMax));
                    //Eliminamos los nodos que no son necesarios
                    foreach (string idNodo in dicNodos.Keys.ToList())
                    {
                        if (!colaboradores.Contains(idNodo) && idNodo != ("http://gnoss/" + pIdGroup))
                        {
                            dicNodos.Remove(idNodo);
                        }
                    }
                }
                //Creamos las relaciones entre el grupo y los colaboradores
                foreach (string colaborador in numRelacionesColaboradorProyectoGrupo.Keys)
                {
                    if (colaboradores.Contains(colaborador))
                    {
                        string group = "http://gnoss/" + pIdGroup.ToUpper();
                        string nombreRelacion = "Proyectos";
                        if (!dicRelaciones.ContainsKey(group))
                        {
                            dicRelaciones.Add(group, new List<DataQueryRelaciones>());
                        }

                        DataQueryRelaciones dataQueryRelaciones = (dicRelaciones[group].FirstOrDefault(x => x.nombreRelacion == nombreRelacion));
                        if (dataQueryRelaciones == null)
                        {
                            dataQueryRelaciones = new DataQueryRelaciones()
                            {
                                nombreRelacion = nombreRelacion,
                                idRelacionados = new List<Datos>()
                            };
                            dicRelaciones[group].Add(dataQueryRelaciones);
                        }
                        dataQueryRelaciones.idRelacionados.Add(new Datos()
                        {
                            idRelacionado = colaborador,
                            numVeces = numRelacionesColaboradorProyectoGrupo[colaborador]
                        });
                    }
                }
                foreach (string colaborador in numRelacionesColaboradorDocumentoGrupo.Keys)
                {
                    if (colaboradores.Contains(colaborador))
                    {
                        string group = "http://gnoss/" + pIdGroup.ToUpper();
                        string nombreRelacion = "Documentos";
                        if (!dicRelaciones.ContainsKey(group))
                        {
                            dicRelaciones.Add(group, new List<DataQueryRelaciones>());
                        }

                        DataQueryRelaciones dataQueryRelaciones = (dicRelaciones[group].FirstOrDefault(x => x.nombreRelacion == nombreRelacion));
                        if (dataQueryRelaciones == null)
                        {
                            dataQueryRelaciones = new DataQueryRelaciones()
                            {
                                nombreRelacion = nombreRelacion,
                                idRelacionados = new List<Datos>()
                            };
                            dicRelaciones[group].Add(dataQueryRelaciones);
                        }
                        dataQueryRelaciones.idRelacionados.Add(new Datos()
                        {
                            idRelacionado = colaborador,
                            numVeces = numRelacionesColaboradorDocumentoGrupo[colaborador]
                        });
                    }
                }

                #region Relaciones entre miembros DENTRO DEl GRUPO
                {
                    //Proyectos
                    {
                        string select = "SELECT ?person group_concat(?project;separator=\",\") as ?projects";
                        string where = $@"
                    WHERE {{ 
                            ?project a 'project'.
                            ?project ?propRol ?rol.
                            FILTER(?propRol in (<http://vivoweb.org/ontology/core#relates>,<http://vivoweb.org/ontology/core#mainRersearchers>))
                            ?rol <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
                            ?project <http://w3id.org/roh/publicGroupList>  <http://gnoss/{pIdGroup}>.
                            FILTER(?person in (<{string.Join(">,<", colaboradores)}>))
                        }}";
                        SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);
                        Dictionary<string, HashSet<string>> personaProy = new Dictionary<string, HashSet<string>>();
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            string projects = fila["projects"].value;
                            string person = fila["person"].value;
                            personaProy.Add(person, new HashSet<string>(projects.Split(',')));
                        }
                        UtilidadesAPI.ProcessRelations("Proyectos", personaProy, ref dicRelaciones);
                    }
                    //DOCUMENTOS
                    {
                        string select = "SELECT ?person group_concat(?document;separator=\",\") as ?documents";
                        string where = $@"
                    WHERE {{ 
                            ?document a 'document'.
                            ?document <http://purl.org/ontology/bibo/authorList> ?authorList.
                            ?authorList <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
                            ?document <http://w3id.org/roh/isProducedBy> <http://gnoss/{pIdGroup}>.
                            FILTER(?person in (<{string.Join(">,<", colaboradores)}>))
                        }}";
                        SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);
                        Dictionary<string, HashSet<string>> personaDoc = new Dictionary<string, HashSet<string>>();
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            string documents = fila["documents"].value;
                            string person = fila["person"].value;
                            personaDoc.Add(person, new HashSet<string>(documents.Split(',')));
                        }
                        UtilidadesAPI.ProcessRelations("Documentos", personaDoc, ref dicRelaciones);
                    }
                }
                #endregion



                int maximasRelaciones = 0;
                foreach (KeyValuePair<string, List<DataQueryRelaciones>> sujeto in dicRelaciones)
                {
                    foreach (DataQueryRelaciones relaciones in sujeto.Value)
                    {
                        foreach (Datos relaciones2 in relaciones.idRelacionados)
                        {
                            maximasRelaciones = Math.Max(maximasRelaciones, relaciones2.numVeces);
                        }
                    }
                }

                // Nodos. 
                if (dicNodos != null && dicNodos.Count > 0)
                {
                    foreach (KeyValuePair<string, string> nodo in dicNodos)
                    {
                        string clave = nodo.Key;
                        string valor = UtilsCadenas.ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(nodo.Value);
                        Models.Graficas.DataItemRelacion.Data.Type type = Models.Graficas.DataItemRelacion.Data.Type.none;
                        if (colaboradores.Contains(nodo.Key))
                        {
                            type = Models.Graficas.DataItemRelacion.Data.Type.icon_member;
                        }
                        Models.Graficas.DataItemRelacion.Data data = new Models.Graficas.DataItemRelacion.Data(clave, valor, null, null, null, "nodes", type);
                        DataItemRelacion dataColabo = new DataItemRelacion(data, true, true);
                        items.Add(dataColabo);
                    }
                }

                // Relaciones.
                if (dicRelaciones != null && dicRelaciones.Count > 0)
                {
                    foreach (KeyValuePair<string, List<DataQueryRelaciones>> sujeto in dicRelaciones)
                    {
                        foreach (DataQueryRelaciones relaciones in sujeto.Value)
                        {
                            foreach (Datos relaciones2 in relaciones.idRelacionados)
                            {
                                string id = $@"{sujeto.Key}~{relaciones.nombreRelacion}~{relaciones2.idRelacionado}~{relaciones2.numVeces}";
                                Models.Graficas.DataItemRelacion.Data.Type type = Models.Graficas.DataItemRelacion.Data.Type.none;
                                if (relaciones.nombreRelacion == "Proyectos")
                                {
                                    type = Models.Graficas.DataItemRelacion.Data.Type.relation_project;
                                }
                                else if (relaciones.nombreRelacion == "Documentos")
                                {
                                    type = Models.Graficas.DataItemRelacion.Data.Type.relation_document;
                                }
                                Models.Graficas.DataItemRelacion.Data data = new Models.Graficas.DataItemRelacion.Data(id, relaciones.nombreRelacion, sujeto.Key, relaciones2.idRelacionado, UtilidadesAPI.CalcularGrosor(maximasRelaciones, relaciones2.numVeces), "edges", type);
                                DataItemRelacion dataColabo = new DataItemRelacion(data, null, null);
                                items.Add(dataColabo);
                            }
                        }
                    }
                }
            }
            return items;
        }

    }
}
