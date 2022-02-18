using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Hercules.MA.ServicioExterno.Controllers.Utilidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hercules.MA.ServicioExterno.Controllers.Acciones
{
    public class AccionesRedesUsuario
    {
        #region --- Constantes   
        private static string RUTA_OAUTH = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config/ConfigOAuth/OAuthV3.config";
        private static ResourceApi mResourceApi = new ResourceApi(RUTA_OAUTH);
        private static CommunityApi mCommunityApi = new CommunityApi(RUTA_OAUTH);
        private static Guid mIdComunidad = mCommunityApi.GetCommunityId();
        private static string RUTA_PREFIJOS = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Models/JSON/prefijos.json";
        private static string mPrefijos = string.Join(" ", JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(RUTA_PREFIJOS)));
        #endregion

        /// <summary>
        /// Obtiene los datos de las fuentes de RO de una persona.
        /// </summary>
        /// <param name="pIdGnossUser">ID del usuario gnoss.</param>
        /// <returns>Diccionario con los datos resultantes.</returns>
        public Dictionary<string, string> GetDataRedesUsuario(string pIdGnossUser)
        {
            Dictionary<string, string> dicResultados = null;
            string idGnossUser = $@"http://gnoss/{pIdGnossUser.ToUpper()}";
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT DISTINCT ?s ?usuarioFigShare ?tokenFigShare ?usuarioGitHub ?tokenGitHub ");
            where.Append("WHERE { ");
            where.Append($@"?s roh:gnossUser <{idGnossUser}>. ");
            where.Append($@"OPTIONAL{{?s roh:usuarioFigShare ?usuarioFigShare. }} ");
            where.Append($@"OPTIONAL{{?s roh:tokenFigShare ?tokenFigShare. }} ");
            where.Append($@"OPTIONAL{{?s roh:usuarioGitHub ?usuarioGitHub. }} ");
            where.Append($@"OPTIONAL{{?s roh:tokenGitHub ?tokenGitHub. }} ");
            where.Append("} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                dicResultados = new Dictionary<string, string>();

                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    if (fila.ContainsKey("usuarioFigShare"))
                    {
                        dicResultados.Add("usuarioFigShare", fila["usuarioFigShare"].value);
                    }
                    else
                    {
                        dicResultados.Add("usuarioFigShare", string.Empty);
                    }

                    if (fila.ContainsKey("tokenFigShare"))
                    {
                        dicResultados.Add("tokenFigShare", fila["tokenFigShare"].value);
                    }
                    else
                    {
                        dicResultados.Add("tokenFigShare", string.Empty);
                    }

                    if (fila.ContainsKey("usuarioGitHub"))
                    {
                        dicResultados.Add("usuarioGitHub", fila["usuarioGitHub"].value);
                    }
                    else
                    {
                        dicResultados.Add("usuarioGitHub", string.Empty);
                    }

                    if (fila.ContainsKey("tokenGitHub"))
                    {
                        dicResultados.Add("tokenGitHub", fila["tokenGitHub"].value);
                    }
                    else
                    {
                        dicResultados.Add("tokenGitHub", string.Empty);
                    }
                }
            }

            return dicResultados;
        }
    }
}
