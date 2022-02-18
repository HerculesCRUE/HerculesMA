using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
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
            Dictionary<string, string> dicResultados = new Dictionary<string, string>();
            dicResultados.Add("usuarioFigShare", string.Empty);
            dicResultados.Add("tokenFigShare", string.Empty);
            dicResultados.Add("usuarioGitHub", string.Empty);
            dicResultados.Add("tokenGitHub", string.Empty);

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
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    if (fila.ContainsKey("usuarioFigShare"))
                    {
                        dicResultados["usuarioFigShare"] = fila["usuarioFigShare"].value;
                    }

                    if (fila.ContainsKey("tokenFigShare"))
                    {
                        dicResultados["tokenFigShare"] = fila["tokenFigShare"].value;
                    }

                    if (fila.ContainsKey("usuarioGitHub"))
                    {
                        dicResultados["usuarioGitHub"] = fila["usuarioGitHub"].value;
                    }

                    if (fila.ContainsKey("tokenGitHub"))
                    {
                        dicResultados["tokenGitHub"] = fila["tokenGitHub"].value;
                    }
                }
            }

            return dicResultados;
        }

        /// <summary>
        /// Modifica los datos de la fuente de RO de una persona.
        /// </summary>
        /// <param name="pIdGnossUser">ID del usuario de gnoss.</param>
        /// <param name="pDicDatosAntiguos">Diccionario con los datos antiguos. (JSON)</param>
        /// <param name="pDicDatosNuevos">Diccionario con los datos nuevos. (JSON)</param>
        public void SetDataRedesUsuario(string pIdGnossUser, string pDicDatosAntiguos, string pDicDatosNuevos)
        {
            string idRecurso = string.Empty;
            string idGnossUser = $@"http://gnoss/{pIdGnossUser.ToUpper()}";
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Diccionarios.
            Dictionary<string, string> dicDatosAntiguos = JsonConvert.DeserializeObject<Dictionary<string, string>>(pDicDatosAntiguos);
            Dictionary<string, string> dicDatosNuevos = JsonConvert.DeserializeObject<Dictionary<string, string>>(pDicDatosNuevos);

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT DISTINCT ?s ");
            where.Append("WHERE { ");
            where.Append($@"?s roh:gnossUser <{idGnossUser}>. ");
            where.Append("} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), "person");

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    if (fila.ContainsKey("s"))
                    {
                        idRecurso = fila["s"].value;
                    }
                }
            }

            // Inserción/Modificación de triples.
            mResourceApi.ChangeOntoly("person");
            Guid guid = new Guid(idRecurso.Split("_")[1]);
            Dictionary<Guid, List<TriplesToInclude>> dicInsercion = new Dictionary<Guid, List<TriplesToInclude>>();
            List<TriplesToInclude> listaTriplesInsercion = new List<TriplesToInclude>();
            Dictionary<Guid, List<TriplesToModify>> dicModificacion = new Dictionary<Guid, List<TriplesToModify>>();
            List<TriplesToModify> listaTriplesModificacion = new List<TriplesToModify>();

            foreach (KeyValuePair<string, string> item in dicDatosAntiguos)
            {
                string propiedad = item.Key;
                string dataViejo = item.Value;
                string dataNuevo = dicDatosNuevos[propiedad];

                if (dataViejo != dataNuevo)
                {
                    if (string.IsNullOrEmpty(dataViejo))
                    {
                        // Inserción (Triples).                 
                        TriplesToInclude triple = new TriplesToInclude();
                        triple.Predicate = $@"http://w3id.org/roh/{propiedad}";
                        triple.NewValue = dataNuevo;
                        listaTriplesInsercion.Add(triple);
                    }
                    else
                    {
                        // Modificación (Triples).
                        TriplesToModify triple = new TriplesToModify();
                        triple.Predicate = $@"http://w3id.org/roh/{propiedad}";
                        triple.NewValue = dataNuevo;
                        triple.OldValue = dataViejo;
                        listaTriplesModificacion.Add(triple);
                    }
                }
            }

            // Inserción.
            dicInsercion.Add(guid, listaTriplesInsercion);
            mResourceApi.InsertPropertiesLoadedResources(dicInsercion);

            // Modificación.
            dicModificacion.Add(guid, listaTriplesModificacion);
            mResourceApi.ModifyPropertiesLoadedResources(dicModificacion);
        }
    }
}
