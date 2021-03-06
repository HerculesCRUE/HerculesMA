using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hercules.MA.ServicioExterno.Controllers.Acciones
{
    public class AccionesFuentesExternas
    {
        #region --- Constantes   
        private static string RUTA_OAUTH = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config/ConfigOAuth/OAuthV3.config";
        private static ResourceApi mResourceApi = new ResourceApi(RUTA_OAUTH);
        private static CommunityApi mCommunityApi = new CommunityApi($@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config/ConfigOAuth/OAuthV3.config");
        private static Guid mCommunityID = mCommunityApi.GetCommunityId();
        private static string RUTA_PREFIJOS = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Models/JSON/prefijos.json";
        private static string mPrefijos = string.Join(" ", JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(RUTA_PREFIJOS)));
        #endregion

        /// <summary>
        /// Obtiene el ORCID del usuario.
        /// </summary>
        /// <param name="pUserId">ID del usuario conectado.</param>
        /// <returns></returns>
        public static string GetORCID(string pUserId)
        {
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT ?orcid ");
            where.Append("WHERE { ");
            where.Append($@"?s roh:gnossUser <http://gnoss/{pUserId.ToUpper()}>. ");
            where.Append("OPTIONAL{?s roh:ORCID ?orcid. } ");
            where.Append("} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mCommunityID);
            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    if (fila.ContainsKey("orcid"))
                    {
                        return fila["orcid"].value;
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Obtiene la fecha de la última actualización.
        /// </summary>
        /// <param name="pUserId">ID del usuario conectado.</param>
        /// <returns></returns>
        public static string GetLastUpdatedDate(string pUserId)
        {
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT ?fecha ");
            where.Append("WHERE { ");
            where.Append($@"?s roh:gnossUser <http://gnoss/{pUserId.ToUpper()}>. ");
            where.Append("OPTIONAL{?s roh:lastUpdatedDate ?fecha. } ");
            where.Append("} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mCommunityID);
            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    if (fila.ContainsKey("fecha"))
                    {
                        string fechaGnoss = fila["fecha"].value;
                        string anio = fechaGnoss.Substring(0, 4);
                        string mes = fechaGnoss.Substring(4, 2);
                        string dia = fechaGnoss.Substring(6, 2);

                        return $@"{anio}-{mes}-{dia}";
                    }
                }
            }

            return "1500-01-01";
        }

        /// <summary>
        /// Obtiene los IDs y Token del usuario para FigShare y GitHub.
        /// </summary>
        /// <param name="pUserId">ID del usuario.</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetUsersIDs(string pUserId)
        {
            Dictionary<string, string> dicResultados = new Dictionary<string, string>();
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT ?usuarioFigshare ?tokenFigshare ?usuarioGitHub ?tokenGitHub ");
            where.Append("WHERE { ");
            where.Append($@"?s roh:gnossUser <http://gnoss/{pUserId.ToUpper()}>. ");
            where.Append("OPTIONAL{?s roh:usuarioFigShare ?usuarioFigshare. } ");
            where.Append("OPTIONAL{?s roh:tokenFigShare ?tokenFigshare. } ");
            where.Append("OPTIONAL{?s roh:usuarioGitHub ?usuarioGitHub. } ");
            where.Append("OPTIONAL{?s roh:tokenGitHub ?tokenGitHub. } ");
            where.Append("} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mCommunityID);
            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    if (fila.ContainsKey("usuarioFigshare"))
                    {
                        dicResultados.Add("usuarioFigshare", fila["usuarioFigshare"].value);
                    }
                    if (fila.ContainsKey("tokenFigshare"))
                    {
                        dicResultados.Add("tokenFigshare", fila["tokenFigshare"].value);
                    }
                    if (fila.ContainsKey("usuarioGitHub"))
                    {
                        dicResultados.Add("usuarioGitHub", fila["usuarioGitHub"].value);
                    }
                    if (fila.ContainsKey("tokenGitHub"))
                    {
                        dicResultados.Add("tokenGitHub", fila["tokenGitHub"].value);
                    }
                }
            }

            return dicResultados;
        }
    }
}
