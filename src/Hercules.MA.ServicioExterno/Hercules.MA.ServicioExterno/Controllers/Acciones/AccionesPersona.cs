﻿using Gnoss.ApiWrapper;
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
        private static string COLOR_GRAFICAS_HORIZONTAL = "#1177ff";
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
            where.Append("?documento <http://purl.org/ontology/bibo/authorList> ?listaAutores. ");
            where.Append("?listaAutores <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?persona. ");
            where.Append("?documento <http://w3id.org/roh/hasKnowledgeArea> ?area. ");
            where.Append("?area <http://w3id.org/roh/categoryNode> ?categoria. ");
            where.Append("?categoria <http://www.w3.org/2008/05/skos#prefLabel> ?nombreCategoria. ");
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
        /// <param name="pParametros">En este caso, el nombre completo de la persona.</param>
        /// <returns>Listado de objetos de la gráfica.</returns>
        public List<DataGraficaColaboradores> GetDatosGraficaRedColaboradoresPersonas(string pIdPersona, string pParametros)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdPersona);
            Dictionary<string, string> dicNodos = new Dictionary<string, string>();
            Dictionary<string, DataQueryRelaciones> dicRelaciones = new Dictionary<string, DataQueryRelaciones>();
            Dictionary<string, int> dicPersonasColabo = new();
            SparqlObject resultadoQuery = null;

            string personas = $@"<{idGrafoBusqueda}>";

            if (!string.IsNullOrEmpty(pParametros))
            {
                dicNodos.Add(idGrafoBusqueda, pParametros.ToLower().Trim());
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
                        FILTER(?personaDoc = <{idGrafoBusqueda}>)
                        ?documento bibo:authorList ?listaAutores2.
                        ?listaAutores2 rdf:member ?id.
                        ?id foaf:name ?nombre.
                        FILTER(?id != <{idGrafoBusqueda}>)
                    }}
                    }} UNION {{
                        SELECT *
                        WHERE {{
                        ?proyecto vivo:relates ?relacion.
                        ?relacion roh:roleOf ?persona.
                        FILTER(?persona = <{idGrafoBusqueda}>)
                        ?proyecto vivo:relates ?relacion2.
                        ?relacion2 roh:roleOf ?id.
                        ?id foaf:name ?nombre.
                        FILTER(?id != <{idGrafoBusqueda}>)
                    }} }} }} ORDER BY DESC (COUNT(*)) LIMIT 10
                ";

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
            List<DataGraficaColaboradores> colaboradores = new List<DataGraficaColaboradores>();
            int maximasRelaciones = 1;
            if(dicPersonasColabo.Values.Max() > numColaboraciones.Max())
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
                        Models.DataGraficaColaboradores.Data data = new Models.DataGraficaColaboradores.Data(id, null, sujeto.Key, relaciones.idRelacionado, CalcularGrosor(maximasRelaciones, relaciones.numVeces), "edges");
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
            where.Append("?documento <http://purl.org/ontology/bibo/authorList> ?listaAutores.");
            where.Append("?listaAutores <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?persona.");
            where.Append("?documento <http://w3id.org/roh/hasKnowledgeArea> ?area. ");
            where.Append("?area <http://w3id.org/roh/categoryNode> ?categoria. ");
            where.Append("?categoria <http://www.w3.org/2008/05/skos#prefLabel> ?nombreCategoria. ");
            where.Append("FILTER( ?persona = <http://gnoss/7C1768AE-BA7C-4CBF-9087-63606E74B085>)");
            where.Append("} ");
            where.Append("Group by(?nombreCategoria)");

            if (!string.IsNullOrEmpty(pParametros))
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = ObtenerParametros(pParametros);
                string filtros = CrearFiltros(dicParametros, "?documento", ref aux);
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
            List<string> listaColores = CrearListaColores(dicionarioOrdenado.Count(), COLOR_GRAFICAS_HORIZONTAL);
            Models.DataGraficaPublicacionesHorizontal.Datasets datasets = new Models.DataGraficaPublicacionesHorizontal.Datasets(dicResultadosPorcentaje.Values.ToList(), listaColores);
            Models.DataGraficaPublicacionesHorizontal.Data data = new Models.DataGraficaPublicacionesHorizontal.Data(dicResultadosPorcentaje.Keys.ToList(), new List<Models.DataGraficaPublicacionesHorizontal.Datasets> { datasets });

            // Máximo.
            x xAxes = new x(new Ticks(0, 100), new ScaleLabel(true, "Percentage"));

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
