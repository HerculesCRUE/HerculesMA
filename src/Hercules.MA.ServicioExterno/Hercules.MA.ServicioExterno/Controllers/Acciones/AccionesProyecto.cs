﻿using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Hercules.MA.ServicioExterno.Controllers.Utilidades;
using Hercules.MA.ServicioExterno.Models;
using Hercules.MA.ServicioExterno.Models.Graficas.DataGraficaAreasTags;
using Hercules.MA.ServicioExterno.Models.Graficas.DataGraficaProyectos;
using Hercules.MA.ServicioExterno.Models.Graficas.DataItemRelacion;
using Hercules.MA.ServicioExterno.Models.Graficas.GraficaBarras;
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
        /// Obtiene los datos para crear la gráfico de los proyectos por año.
        /// </summary>
        /// <param name="pParametros">Filtros aplicados en las facetas.</param>
        public GraficasProyectos GetDatosGraficaProyectos(string pParametros)
        {
            GraficasProyectos graficasProyectos = new GraficasProyectos();

            #region Gráfico de barras
            {
                Dictionary<string, DataFechas> dicResultados = new();
                SparqlObject resultadoQuery = null;
                // Consultas sparql.

                #region --- Obtención de datos del año de inicio de los proyectos
                {
                    string select = $@"{mPrefijos}
                                    SELECT COUNT(DISTINCT(?proyecto)) AS ?numProyectos ?idTipo ?anyoInicio ";
                    int aux = 0;
                    string where = $@"WHERE {{ 
                                    ?proyecto vivo:start ?fecha
                                    BIND( (?fecha/10000000000) AS ?anyoInicio)
                                    OPTIONAL{{
                                        ?proyecto roh:scientificExperienceProject ?tipo.
                                        ?tipo dc:identifier ?idTipo
                                    }}
                                    {UtilidadesAPI.CrearFiltros(UtilidadesAPI.ObtenerParametros(pParametros), "?proyecto", ref aux)}
                                }}ORDER BY ?anyoInicio";
                    resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                    {
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            string anyo = UtilidadesAPI.GetValorFilaSparqlObject(fila, "anyoInicio");
                            int numProyectos = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numProyectos"));
                            string tipo = UtilidadesAPI.GetValorFilaSparqlObject(fila, "idTipo");
                            if (tipo == null)
                            {
                                tipo = "";
                            }
                            if (!dicResultados.ContainsKey(anyo))
                            {
                                // Si no contiene el año, creo el objeto.
                                DataFechas data = new();
                                data.numProyectosInicio = new Dictionary<string, int>() { { tipo, numProyectos } };
                                data.numProyectosFin = 0;
                                dicResultados.Add(anyo, data);
                            }
                            else
                            {
                                // Si lo contiene, se lo agrego.
                                dicResultados[anyo].numProyectosInicio[tipo] = numProyectos;
                            }
                        }
                    }
                }
                #endregion

                #region --- Obtención de datos del año de fin de los proyectos
                {
                    string select = $@"{mPrefijos}
                                    SELECT COUNT(DISTINCT(?proyecto)) AS ?numProyectos ?anyoFin ";
                    int aux = 0;
                    string where = $@"WHERE {{ 
                                    ?proyecto vivo:end ?fecha
                                    BIND( (?fecha/10000000000) AS ?anyoFin)
                                    {UtilidadesAPI.CrearFiltros(UtilidadesAPI.ObtenerParametros(pParametros), "?proyecto", ref aux)}
                                }}ORDER BY ?anyoFin";
                    resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                    {
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            string anyo = UtilidadesAPI.GetValorFilaSparqlObject(fila, "anyoFin");
                            int numProyectos = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numProyectos"));

                            if (!dicResultados.ContainsKey(anyo))
                            {
                                // Si no contiene el año, creo el objeto.
                                DataFechas data = new();
                                data.numProyectosInicio = new Dictionary<string, int>();
                                data.numProyectosFin = numProyectos;
                                dicResultados.Add(anyo, data);
                            }
                            else
                            {
                                // Si lo contiene, se lo agrego.
                                dicResultados[anyo].numProyectosFin = numProyectos;
                            }
                        }
                    }
                }
                #endregion

                // Rellenar años intermedios y ordenarlos.
                if (dicResultados.Count > 0)
                {
                    int inicio = dicResultados.Keys.Select(x => int.Parse(x)).Min();
                    int fin = dicResultados.Keys.Select(x => int.Parse(x)).Max();
                    for (int i = inicio; i < fin; i++)
                    {
                        if (!dicResultados.ContainsKey(i.ToString()))
                        {
                            dicResultados.Add(i.ToString(), new DataFechas() { numProyectosInicio = new Dictionary<string, int>(), numProyectosFin = 0 });
                        }
                    }
                }
                dicResultados = dicResultados.OrderBy(item => item.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

                // Se coge los datos del número de proyectos.
                List<int> listaIniciosCompetitivos = new();
                List<int> listaIniciosNoCompetitivos = new();
                List<int> listaIniciosOtros = new();

                List<int> listaFines = new();
                foreach (KeyValuePair<string, DataFechas> item in dicResultados)
                {
                    if (item.Value.numProyectosInicio.ContainsKey("SEP1"))
                    {
                        listaIniciosCompetitivos.Add(item.Value.numProyectosInicio["SEP1"]);
                    }
                    if (item.Value.numProyectosInicio.ContainsKey("SEP2"))
                    {
                        listaIniciosNoCompetitivos.Add(item.Value.numProyectosInicio["SEP2"]);
                    }
                    if (item.Value.numProyectosInicio.ContainsKey(""))
                    {
                        listaIniciosOtros.Add(item.Value.numProyectosInicio[null]);
                    }
                    listaFines.Add(item.Value.numProyectosFin);
                }

                // Se construye el objeto con los datos.
                List<DatosBarra> listaDatos = new List<DatosBarra>();
                listaDatos.Add(new DatosBarra("Inicio competitivos", "#6cafd3", listaIniciosCompetitivos, 1, "inicio"));
                listaDatos.Add(new DatosBarra("Inicio no competitivos", "#7cbfe3", listaIniciosNoCompetitivos, 1, "inicio"));
                if (listaIniciosOtros.Exists(x => x > 0))
                {
                    listaDatos.Add(new DatosBarra("Inicio otros", "#8ccff3", listaIniciosOtros, 1, "inicio"));
                }
                listaDatos.Add(new DatosBarra("Fin", "#BF4858", listaFines, 0.2f, null));
                // Se crea el objeto de la gráfica.
                graficasProyectos.graficaBarrasAnios = new GraficaBarras(new DataGraficaBarras(dicResultados.Keys.ToList(), listaDatos));
            }
            #endregion

            #region Gráfico de sectores
            {
                Dictionary<string, KeyValuePair<string, int>> dicAmbitos = new Dictionary<string, KeyValuePair<string, int>>();
                SparqlObject resultadoQuery = null;
                // Consultas sparql.

                #region --- Obtención de ámbitos
                {
                    string select = $@"{mPrefijos}
                                    SELECT COUNT(DISTINCT(?proyecto)) AS ?numProyectos ?ambitoID ?ambitoNombre";
                    int aux = 0;
                    string where = $@"WHERE {{ 
                                    ?proyecto a 'project'.
                                    ?proyecto vivo:geographicFocus ?ambito.
                                    ?ambito dc:identifier ?ambitoID.
                                    ?ambito dc:title ?ambitoNombre.
                                    FILTER(lang(?ambitoNombre)='es')
                                    {UtilidadesAPI.CrearFiltros(UtilidadesAPI.ObtenerParametros(pParametros), "?proyecto", ref aux)}
                                }}ORDER BY desc(?numProyectos)";
                    resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                    {
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            dicAmbitos[UtilidadesAPI.GetValorFilaSparqlObject(fila, "ambitoID")] = new KeyValuePair<string, int>(UtilidadesAPI.GetValorFilaSparqlObject(fila, "ambitoNombre"), int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numProyectos")));
                        }
                    }
                }
                #endregion

                Dictionary<string, string> backGroundColor = new Dictionary<string, string>();
                backGroundColor.Add("000", "rgba(255, 99, 132, 0.5)");//Autonómica
                backGroundColor.Add("010", "rgba(54, 162, 235, 0.2)");//Nacional
                backGroundColor.Add("020", "rgba(255, 206, 86, 0.2)");//Unión Europea
                backGroundColor.Add("030", "rgba(75, 192, 192, 0.2)");//Internacional no UE
                backGroundColor.Add("OTHERS", "rgba(175, 92, 92, 0.2)");//Otros

                Dictionary<string, string> borderColor = new Dictionary<string, string>();
                borderColor.Add("000", "rgba(255, 99, 132, 1)");//Autonómica
                borderColor.Add("010", "rgba(54, 162, 235, 1)");//Nacional
                borderColor.Add("020", "rgba(255, 206, 86, 1)");//Unión Europea
                borderColor.Add("030", "rgba(75, 192, 192, 1)");//Internacional no UE
                borderColor.Add("OTHERS", "rgba(175, 92, 92, 1)");//Otros

                Dictionary<string, string> backGroundColorAux = new Dictionary<string, string>();
                Dictionary<string, string> borderColorAux = new Dictionary<string, string>();
                foreach (string idAmbito in dicAmbitos.Keys)
                {
                    backGroundColorAux[idAmbito] = backGroundColor[idAmbito];
                    borderColorAux[idAmbito] = borderColor[idAmbito];
                }


                DatosSector datosSector = new DatosSector(backGroundColorAux.Values.ToList(), borderColorAux.Values.ToList(), dicAmbitos.Values.Select(x => x.Value).ToList());
                graficasProyectos.graficaSectoresAmbito = new GraficaSectores(new DataGraficaSectores(dicAmbitos.Values.Select(x => x.Key).ToList(), new List<DatosSector>() { datosSector }));
            }
            #endregion

            #region Gráfico de barras miembros
            {
                Dictionary<string, int> dicNumMiembrosProyecto = new Dictionary<string, int>();
                SparqlObject resultadoQuery = null;
                // Consultas sparql.

                #region --- Obtención nº de miembros
                {
                    string select = $@"{mPrefijos}
                                    SELECT ?proyecto COUNT(DISTINCT(?person)) AS ?numMiembros";
                    int aux = 0;
                    string where = $@"WHERE {{ 
                                    ?proyecto a 'project'.
                                    OPTIONAL{{
                                        ?proyecto ?propRol ?rol.
                                        FILTER(?propRol in (<http://vivoweb.org/ontology/core#relates>,<http://w3id.org/roh/mainResearchers>))
                                        ?rol <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
                                    }}
                                    {UtilidadesAPI.CrearFiltros(UtilidadesAPI.ObtenerParametros(pParametros), "?proyecto", ref aux)}
                                }}ORDER BY desc(?numMiembros)";
                    resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                    {
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            dicNumMiembrosProyecto[UtilidadesAPI.GetValorFilaSparqlObject(fila, "proyecto")] = int.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numMiembros"));
                        }
                    }
                }
                #endregion

                Dictionary<string, int> numMiembros = new Dictionary<string, int>();
                numMiembros.Add("1-3",0);
                numMiembros.Add("4-10", 0);
                numMiembros.Add("11-30", 0);
                numMiembros.Add("30+", 0);

                foreach (string id in dicNumMiembrosProyecto.Keys)
                {
                    string text = "";
                    if (dicNumMiembrosProyecto[id] > 0 && dicNumMiembrosProyecto[id] < 4)
                    {
                        text = "1-3";
                    }
                    else if (dicNumMiembrosProyecto[id] >= 4 && dicNumMiembrosProyecto[id] < 11)
                    {
                        text = "4-10";
                    }
                    else if (dicNumMiembrosProyecto[id] >= 11 && dicNumMiembrosProyecto[id] < 31)
                    {
                        text = "11-30";
                    }
                    else if (dicNumMiembrosProyecto[id] >= 31)
                    {
                        text = "30+";
                    }
                    numMiembros[text] ++;
                }

                // Se construye el objeto con los datos.
                List<DatosBarra> listaDatos = new List<DatosBarra>();
                listaDatos.Add(new DatosBarra("Miembros", "#BF4858", numMiembros.Values.ToList(), 1, null));
                // Se crea el objeto de la gráfica.
                graficasProyectos.graficaBarrasMiembros = new GraficaBarras(new DataGraficaBarras(numMiembros.Keys.ToList(), listaDatos));

            }
            #endregion

            return graficasProyectos;
        }


        /// <summary>
        /// Obtienes los datos de las pestañas de cada sección de la ficha.
        /// </summary>
        /// <param name="pProyecto">ID del recurso del proyecto.</param>
        /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        public Dictionary<string, int> GetDatosCabeceraProyecto(string pProyecto)
        {
            string idGrafoBusqueda = UtilidadesAPI.ObtenerIdBusqueda(mResourceApi, pProyecto);
            Dictionary<string, int> dicResultados = new Dictionary<string, int>();
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT COUNT(DISTINCT ?persona) AS ?NumParticipantes COUNT(DISTINCT ?documento) AS ?NumPublicaciones COUNT(DISTINCT ?nombreCategoria) AS ?NumCategorias ");
            where.Append("WHERE {{ "); // Total de Participantes.
            where.Append($@"<{idGrafoBusqueda}> vivo:relates ?relacion. ");
            where.Append("?relacion <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?persona. ");
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
        /// Obtiene los datos para crear el grafo de miembros.
        /// </summary>
        /// <param name="pIdProyecto">ID del recurso del proyecto.</param>
        /// <param name="pParametros">Filtros aplicados en las facetas.</param>
        /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        public List<DataItemRelacion> DatosGraficaRedMiembros(string pIdProyecto, string pParametros)
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
                            <http://gnoss/{pIdProyecto}> <http://vivoweb.org/ontology/core#relates> ?main.
                            ?main <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
                            BIND(false as ?ip)
                        }}UNION
                        {{
                            <http://gnoss/{pIdProyecto}> <http://w3id.org/roh/mainReseachers> ?member.
                            ?member <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
                            BIND(true as ?ip)
                        }}
                        
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
                //Proyecto
                string select = $@"{mPrefijos}
                                select distinct ?nombre";
                string where = $@"
                WHERE {{ 
                        <http://gnoss/{pIdProyecto}> roh:title ?nombre.                        
                }}";

                string nombreProyecto = mResourceApi.VirtuosoQuery(select, where, mIdComunidad).results.bindings.First()["nombre"].value;
                dicNodos.Add("http://gnoss/" + pIdProyecto, nombreProyecto);
            }
            #endregion

            if (miembros.Union(ip).Count() > 0)
            {
                #region Relaciones con el proyecto
                {

                    string nombreRelacion = "Proyectos";
                    foreach (var fila in dicNodos)
                    {
                        string person = fila.Key;
                        string project = "http://gnoss/" + pIdProyecto.ToUpper();
                        int numRelaciones = 1;
                        if (!dicRelaciones.ContainsKey(project))
                        {
                            dicRelaciones.Add(project, new List<DataQueryRelaciones>());
                        }

                        DataQueryRelaciones dataQueryRelaciones = (dicRelaciones[project].FirstOrDefault(x => x.nombreRelacion == nombreRelacion));
                        if (dataQueryRelaciones == null)
                        {
                            dataQueryRelaciones = new DataQueryRelaciones()
                            {
                                nombreRelacion = nombreRelacion,
                                idRelacionados = new List<Datos>()
                            };
                            dicRelaciones[project].Add(dataQueryRelaciones);
                        }
                        dataQueryRelaciones.idRelacionados.Add(new Datos()
                        {
                            idRelacionado = person,
                            numVeces = numRelaciones
                        });
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



        /// <summary>
        /// Obtiene los datos para crear el grafo de relaciones con otros investigadores.
        /// </summary>
        /// <param name="pIdProyecto">ID del recurso del proyecto.</param>
        /// <param name="pParametros">Filtros aplicados en las facetas.</param>
        /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        public List<DataItemRelacion> GetDatosGraficaRedColaboradores(string pIdProyecto, string pParametros)
        {
            //mResourceApi = new ResourceApi(RUTA_OAUTH);
            //string idGrafoBusqueda = UtilidadesAPI.ObtenerIdBusqueda(mResourceApi, pIdProyecto);
            //Dictionary<string, string> dicNodos = new Dictionary<string, string>();
            //Dictionary<string, DataQueryRelaciones> dicRelaciones = new Dictionary<string, DataQueryRelaciones>();
            //SparqlObject resultadoQuery = null;
            //StringBuilder select = null, where = null;

            //#region --- Proyecto de la ficha
            //// Consulta sparql.
            //select = new StringBuilder(mPrefijos);
            //select.Append("SELECT ?titulo ?proyecto AS ?id ");
            //where = new StringBuilder("WHERE { ");
            //where.Append("?proyecto roh:title ?titulo. ");
            //where.Append($@"FILTER(?proyecto = <{idGrafoBusqueda}>) ");
            //where.Append("} ");

            //resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            //if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            //{
            //    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
            //    {
            //        string id = UtilidadesAPI.GetValorFilaSparqlObject(fila, "id");
            //        string nombreProyecto = UtilidadesAPI.GetValorFilaSparqlObject(fila, "titulo");
            //        if (!dicNodos.ContainsKey(id))
            //        {
            //            dicNodos.Add(id, nombreProyecto.ToLower().Trim());
            //        }
            //    }
            //}
            //#endregion

            //#region --- Obtener todas las personas que hayan colaborado en el proyecto.            
            //string personas = $@"<{idGrafoBusqueda}>";

            //// Consulta sparql.
            //select = new StringBuilder(mPrefijos);
            //select.Append("SELECT ?nombre ?persona AS ?id ");
            //where = new StringBuilder("WHERE { ");
            //where.Append($@"<{idGrafoBusqueda}> vivo:relates ?relacion. ");
            //where.Append("?relacion <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?persona. ");
            //where.Append("?persona foaf:name ?nombre. ");

            //// Add the filter over the person
            //if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            //{
            //    // Creación de los filtros obtenidos por parámetros.
            //    int aux = 0;
            //    Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
            //    string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?persona", ref aux);
            //    where.Append(filtros);
            //}
            //where.Append("} ");

            //resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            //if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            //{
            //    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
            //    {
            //        string id = UtilidadesAPI.GetValorFilaSparqlObject(fila, "id");
            //        string nombreColaborador = UtilidadesAPI.GetValorFilaSparqlObject(fila, "nombre");
            //        if (!dicNodos.ContainsKey(id))
            //        {
            //            dicNodos.Add(id, nombreColaborador.ToLower().Trim());
            //            personas += ",<" + UtilidadesAPI.GetValorFilaSparqlObject(fila, "id") + ">";
            //        }
            //    }
            //}
            //#endregion

            //#region --- Creación de las relaciones.
            //KeyValuePair<string, string> proyecto = dicNodos.First();
            //foreach (KeyValuePair<string, string> item in dicNodos)
            //{
            //    if (item.Key != proyecto.Key)
            //    {
            //        dicRelaciones.Add(item.Key, new DataQueryRelaciones(new List<Datos> { new Datos(proyecto.Key, 1) }));
            //    }
            //}
            //#endregion

            //// Construcción del objeto de la gráfica.            
            //List<DataGraficaColaboradores> colaboradores = new List<DataGraficaColaboradores>();
            //int maximasRelaciones = 1;

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
            //            relaciones.numVeces = 1;
            //            string id = $@"{sujeto.Key}~{relaciones.idRelacionado}~{relaciones.numVeces}";
            //            Models.DataGraficaColaboradores.Data data = new Models.DataGraficaColaboradores.Data(id, null, sujeto.Key, relaciones.idRelacionado,UtilidadesAPI.CalcularGrosor(maximasRelaciones, relaciones.numVeces), "edges");
            //            DataGraficaColaboradores dataColabo = new DataGraficaColaboradores(data, null, null);
            //            colaboradores.Add(dataColabo);
            //        }
            //    }
            //}

            //return colaboradores;


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


                //where.Append($@"<{idGrafoBusqueda}> vivo:relates ?relacion. ");
                //where.Append("?relacion <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?persona. ");
                //where.Append("?persona foaf:name ?nombre. ");

                //Miembros
                string select = $@"{mPrefijos}
                                select distinct ?person ?nombre ?ip";
                string where = $@"
                WHERE {{ 
                        {filtrosPersonas}
                        ?person a 'person'.
                        ?person foaf:name ?nombre.
                        {{
                            <http://gnoss/{pIdProyecto}> <http://vivoweb.org/ontology/core#relates> ?main.
                            ?main <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
                            BIND(false as ?ip)
                        }}UNION
                        {{
                            <http://gnoss/{pIdProyecto}> <http://w3id.org/roh/mainReseachers> ?member.
                            ?member <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
                            BIND(true as ?ip)
                        }}
                        
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
                //Proyecto
                string select = $@"{mPrefijos}
                                select distinct ?nombre";
                string where = $@"
                WHERE {{ 
                        <http://gnoss/{pIdProyecto}> roh:title ?nombre.                        
                }}";

                string nombreProyecto = mResourceApi.VirtuosoQuery(select, where, mIdComunidad).results.bindings.First()["nombre"].value;
                dicNodos.Add("http://gnoss/" + pIdProyecto, nombreProyecto);
            }
            #endregion

            if (miembros.Union(ip).Count() > 0)
            {
                #region Relaciones con el proyecto
                {

                    string nombreRelacion = "Proyectos";
                    foreach (var fila in dicNodos)
                    {
                        string person = fila.Key;
                        string project = "http://gnoss/" + pIdProyecto.ToUpper();
                        int numRelaciones = 1;
                        if (!dicRelaciones.ContainsKey(project))
                        {
                            dicRelaciones.Add(project, new List<DataQueryRelaciones>());
                        }

                        DataQueryRelaciones dataQueryRelaciones = (dicRelaciones[project].FirstOrDefault(x => x.nombreRelacion == nombreRelacion));
                        if (dataQueryRelaciones == null)
                        {
                            dataQueryRelaciones = new DataQueryRelaciones()
                            {
                                nombreRelacion = nombreRelacion,
                                idRelacionados = new List<Datos>()
                            };
                            dicRelaciones[project].Add(dataQueryRelaciones);
                        }
                        dataQueryRelaciones.idRelacionados.Add(new Datos()
                        {
                            idRelacionado = person,
                            numVeces = numRelaciones
                        });
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


        /// <summary>
        /// Obtiene los datos para crear la gráfica de las publicaciones (horizontal).
        /// </summary>
        /// <param name="pIdProyecto">ID del recurso del proyecto.</param>
        /// <param name="pParametros">Filtros aplicados en las facetas.</param>
        /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        public DataGraficaAreasTags GetDatosGraficaPublicacionesHorizontal(string pIdProyecto, string pParametros)
        {
            string idGrafoBusqueda = UtilidadesAPI.ObtenerIdBusqueda(mResourceApi, pIdProyecto);
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
                Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
                string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?documento", ref aux);
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
            List<string> listaColores = UtilidadesAPI.CrearListaColores(dicResultados.Count, COLOR_GRAFICAS_HORIZONTAL);
            Datasets datasets = new Datasets(dicResultadosPorcentaje.Values.ToList(), listaColores);
            Models.Graficas.DataGraficaAreasTags.Data data = new Models.Graficas.DataGraficaAreasTags.Data(dicResultadosPorcentaje.Keys.ToList(), new List<Datasets> { datasets });

            // Máximo.
            x xAxes = new x(new Models.Graficas.DataGraficaAreasTags.Ticks(0, 100), new ScaleLabel(true, "Percentage"));

            Models.Graficas.DataGraficaAreasTags.Options options = new Models.Graficas.DataGraficaAreasTags.Options("y", new Plugins(new Title(true, "Resultados de la investigación por fuente de datos"), new Legend(false)), new Models.Graficas.DataGraficaAreasTags.Scales(xAxes));
            DataGraficaAreasTags dataGrafica = new DataGraficaAreasTags("bar", data, options);

            return dataGrafica;
        }
    }
}
