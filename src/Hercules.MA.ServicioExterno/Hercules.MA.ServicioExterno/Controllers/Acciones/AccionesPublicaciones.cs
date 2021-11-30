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
        /// Obtienes los datos de las pestañas de cada sección de la ficha.
        /// </summary>
        /// <param name="pPersona">Nombre de la persona.</param>
        /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        public Dictionary<string, int> GetDatosCabeceraDocumento(string pDocumento)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pDocumento);
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
                    if(numRelacionados > 20)
                    {
                        numRelacionados = 20;
                    }
                    dicResultados.Add("numRelacionados", numRelacionados);
                }
            }

            return dicResultados;
        }



        /// <summary>
        /// Obtiene un listado con los documentos referenciados desde dicho documento
        /// </summary>
        /// <param name="pIdDocumento">ID del recurso del Documento.</param>
        /// <param name="pParametros">En este caso, el nombre completo de la persona.</param>
        /// <returns>Listado de objetos de la gráfica.</returns>
        public List<DataGraficaColaboradores> GetDatosGraficaReferencias(string pIdDocumento, string pParametros)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdDocumento);
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
            select += "SELECT ?cite ?nombre COUNT(*) AS ?numRelaciones";
            string where = $@"
                        WHERE {{
                        <{idGrafoBusqueda}> bibo:cites ?cite.
                        ?cite roh:title ?nombre.
                ";

            // Parameters
            if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
                string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?cite", ref aux);
                where += filtros;
            }

            where += $@"
                    }} LIMIT 25
                ";

            resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);


            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    string id = UtilidadesAPI.GetValorFilaSparqlObject(fila, "cite");
                    int proyectosComun = Int32.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numRelaciones"));
                    string nombreColaborador = UtilidadesAPI.GetValorFilaSparqlObject(fila, "nombre");

                    dicPersonasColabo.Add(id, proyectosComun);
                    dicNodos.Add(id, nombreColaborador.ToLower().Trim());
                    personas += ",<" + UtilidadesAPI.GetValorFilaSparqlObject(fila, "cite") + ">";
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
                            Models.DataGraficaColaboradores.Data data = new Models.DataGraficaColaboradores.Data(id, null, sujeto.Key, relaciones.idRelacionado, UtilidadesAPI.CalcularGrosor(4, relaciones.numVeces), "edges");
                            DataGraficaColaboradores dataColabo = new DataGraficaColaboradores(data, null, null);
                            colaboradores.Add(dataColabo);
                        }
                    }
                }
            }


            return colaboradores;
        }

        /// <summary>
        /// Obtiene un listado con los documentos citados con dicho documento
        /// </summary>
        /// <param name="pIdDocumento">ID del recurso del Documento.</param>
        /// <param name="pParametros">En este caso, el nombre completo de la persona.</param>
        /// <returns>Listado de objetos de la gráfica.</returns>
        public List<DataGraficaColaboradores> GetDatosGraficaCitas(string pIdDocumento, string pParametros)
        {
            string idGrafoBusqueda = ObtenerIdBusqueda(pIdDocumento);
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
            select += "SELECT ?references ?nombre COUNT(*) AS ?numRelaciones";
            string where = $@"
                        WHERE {{
                        ?references bibo:cites <{idGrafoBusqueda}>.
                        ?references roh:title ?nombre.
                ";

            // Parameters
            if (!string.IsNullOrEmpty(pParametros) || pParametros != "#")
            {
                // Creación de los filtros obtenidos por parámetros.
                int aux = 0;
                Dictionary<string, List<string>> dicParametros = UtilidadesAPI.ObtenerParametros(pParametros);
                string filtros = UtilidadesAPI.CrearFiltros(dicParametros, "?references", ref aux);
                where += filtros;
            }

            where += $@"
                    }} LIMIT 25
                ";

            resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);


            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    string id = UtilidadesAPI.GetValorFilaSparqlObject(fila, "references");
                    int proyectosComun = Int32.Parse(UtilidadesAPI.GetValorFilaSparqlObject(fila, "numRelaciones"));
                    string nombreColaborador = UtilidadesAPI.GetValorFilaSparqlObject(fila, "nombre");

                    if (!dicPersonasColabo.ContainsKey(id))
                    {
                        dicPersonasColabo.Add(id, proyectosComun);
                        dicNodos.Add(id, nombreColaborador.ToLower().Trim());
                        personas += ",<" + UtilidadesAPI.GetValorFilaSparqlObject(fila, "references") + ">";
                    } else
                    {
                        dicPersonasColabo[id]++;
                    }
                    
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
                            Models.DataGraficaColaboradores.Data data = new Models.DataGraficaColaboradores.Data(id, null, sujeto.Key, relaciones.idRelacionado, UtilidadesAPI.CalcularGrosor(4, relaciones.numVeces), "edges");
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
