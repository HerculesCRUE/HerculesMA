using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Hercules.MA.ServicioExterno.Controllers.Utilidades;
using Hercules.MA.ServicioExterno.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hercules.MA.ServicioExterno.Controllers.Acciones
{
    public class AccionesMetaBusqueda
    {
    
        #region --- Constantes     
        private static string RUTA_OAUTH = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config/ConfigOAuth/OAuthV3.config";
        private static ResourceApi mResourceApi = new ResourceApi(RUTA_OAUTH);
        private static CommunityApi mCommunityApi = new CommunityApi(RUTA_OAUTH);
        private static Guid mIdComunidad = mCommunityApi.GetCommunityId();
        private static string RUTA_PREFIJOS = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Models/JSON/prefijos.json";
        private static string mPrefijos = string.Join(" ", JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(RUTA_PREFIJOS)));
        public static ObjectSearch objSearch;
        public static List<string> lastSearchs;
        #endregion

        /// <summary>
        /// Busca los elementos necesarios y los guarda en una variable estática para realizar posteriormente la búsqueda en el metabuscador
        /// </summary>
        public void GenertateMetaShearch()
        {
            objSearch = new ObjectSearch();

            #region CargarGrupos
            List<string> grupos = new List<string>();
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            select.Append(mPrefijos);
            select.Append("SELECT DISTINCT(?tituloGrupo) ");
            where.Append("WHERE { ");
            where.Append("?s a 'project'.");
            where.Append("?s roh:title ?tituloGrupo. ");
            where.Append("} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    grupos.Add(UtilidadesAPI.GetValorFilaSparqlObject(fila, "tituloGrupo"));
                }
            }

            objSearch.grupos = grupos;
            #endregion


            #region CargarInvestigadores
            List<string> investigadores = new List<string>();
            resultadoQuery = null;
            select = new StringBuilder();
            where = new StringBuilder();

            select.Append(mPrefijos);
            select.Append("SELECT DISTINCT(?name) ");
            where.Append("WHERE { ");
            where.Append("?s a 'person'.");
            where.Append("?s foaf:name ?name. ");
            where.Append("} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    investigadores.Add(UtilidadesAPI.GetValorFilaSparqlObject(fila, "name"));
                }
            }

            objSearch.investigadores = investigadores;
            #endregion


            #region CargarProyectos
            List<string> proyectos = new List<string>();
            resultadoQuery = null;
            select = new StringBuilder();
            where = new StringBuilder();

            select.Append(mPrefijos);
            select.Append("SELECT DISTINCT(?name) ");
            where.Append("WHERE { ");
            where.Append("?s a 'project'.");
            where.Append("?s roh:title ?name. ");
            where.Append("} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    proyectos.Add(UtilidadesAPI.GetValorFilaSparqlObject(fila, "name"));
                }
            }

            objSearch.proyectos = proyectos;
            #endregion


            #region CargarDocumentos
            List<string> documents = new List<string>();
            resultadoQuery = null;
            select = new StringBuilder();
            where = new StringBuilder();

            select.Append(mPrefijos);
            select.Append("SELECT DISTINCT(?name) ");
            where.Append("WHERE { ");
            where.Append("?s a 'document'.");
            where.Append("?s roh:title ?name. ");
            where.Append("} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    documents.Add(UtilidadesAPI.GetValorFilaSparqlObject(fila, "name"));
                }
            }

            objSearch.documents = documents;
            #endregion


            #region CargarROs
            List<string> ros = new List<string>();
            resultadoQuery = null;
            select = new StringBuilder();
            where = new StringBuilder();

            select.Append(mPrefijos);
            select.Append("SELECT DISTINCT(?name) ");
            where.Append("WHERE { ");
            where.Append("?s a 'researchobject'.");
            where.Append("?s roh:title ?name. ");
            where.Append("} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    ros.Add(UtilidadesAPI.GetValorFilaSparqlObject(fila, "name"));
                }
            }

            objSearch.ros = ros;
            #endregion


        }

        /// <summary>
        /// Busca los elementos necesarios y devuelve los resultados
        /// </summary>
        /// <param name="stringBusqueda">string de búsqueda.</param>
        /// <returns>Objeto con el resultado de la búsqueda.</returns>
        public ObjectSearch Busqueda(string stringBusqueda)
        {
            // Añadir nuevo elemento a la lista de últimas búsquedas
            if (lastSearchs == null)
            {
                lastSearchs = new List<string>();
            }
            try
            {
                stringBusqueda = stringBusqueda.Trim();
                if (!lastSearchs.Contains(stringBusqueda))
                {
                    lastSearchs.Insert(0, stringBusqueda);
                    lastSearchs = lastSearchs.GetRange(0, (lastSearchs.Count < 10 ? lastSearchs.Count : 10));
                }
            } catch (Exception e) { throw new Exception(e.Message); }


            // Inicializar las variables
            ObjectSearch result = new ObjectSearch();
            StringComparison comp = StringComparison.OrdinalIgnoreCase;


            // Grupos
            result.grupos = localSearch(stringBusqueda, objSearch.grupos);

            // Investigadores
            result.investigadores = localSearch(stringBusqueda, objSearch.investigadores);


            // Proyectos
            result.proyectos = localSearch(stringBusqueda, objSearch.proyectos);


            // Documentos
            result.documents = localSearch(stringBusqueda, objSearch.documents);


            // Research Objects
            result.ros = localSearch(stringBusqueda, objSearch.ros);

            List<string> localSearch(string searchT, List<string> searchList)
            {
                var res = new List<string>();
                if (searchT.Trim().Contains(' '))
                {
                    string[] texts = searchT.Split(' ');
                    foreach (string texto in searchList)
                    {
                        var found = true;
                        int ind = 0;
                        while (found && ind < texts.Length)
                        {
                            if (!texto.Contains(texts[ind], comp))
                            {
                                found = false;
                            }
                            ind++;
                        }
                        if (found)
                        {
                            res.Add(texto);
                        }
                        
                    }
                } else
                {
                    foreach (string texto in searchList)
                    {
                        if (texto.Contains(searchT, comp))
                        {
                            res.Add(texto);
                        }
                    }
                }
                
                return res;
            }


            return result;
        }

        /// <summary>
        /// Obtiene las últimas búsquedas
        /// </summary>
        public List<string> GetLastSearchs()
        {
            return lastSearchs;
        }
    }
}
