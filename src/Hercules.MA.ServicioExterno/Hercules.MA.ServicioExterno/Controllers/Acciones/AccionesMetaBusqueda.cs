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
        public ObjectSearch Busqueda(string stringBusqueda)
        {
            ObjectSearch result = new ObjectSearch();
            StringComparison comp = StringComparison.OrdinalIgnoreCase;

            // Grupos
            result.grupos = new List<string>();
            foreach (string texto in objSearch.grupos)
            {
                if (texto.Contains(stringBusqueda, comp)) {
                    result.grupos.Add(texto);
                }
            }
            return result;
        }
    }
}
