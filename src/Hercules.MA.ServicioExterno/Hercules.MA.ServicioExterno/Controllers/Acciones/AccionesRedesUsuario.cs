using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using Hercules.MA.ServicioExterno.Controllers.Utilidades;
using Hercules.MA.ServicioExterno.Models.RedesUsuario;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using User = Hercules.MA.ServicioExterno.Models.RedesUsuario.User;

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
        public List<DataUser> GetDataRedesUsuario(string pIdGnossUser)
        {
            List<DataUser> listaData = new List<DataUser>();
            listaData.Add(new DataUser() { nombre = "Identificador de FigShare", id = "usuarioFigShare", valor = string.Empty });
            listaData.Add(new DataUser() { nombre = "Token de FigShare", id = "tokenFigShare", valor = string.Empty });
            listaData.Add(new DataUser() { nombre = "Usuario de GitHub", id = "usuarioGitHub", valor = string.Empty });
            listaData.Add(new DataUser() { nombre = "Token de GitHub", id = "tokenGitHub", valor = string.Empty });
            listaData.Add(new DataUser() { nombre = "Matching", id = "matching", valor = string.Empty });

            string idGnossUser = $@"http://gnoss/{pIdGnossUser.ToUpper()}";
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append(mPrefijos);
            select.Append("SELECT DISTINCT ?s ?usuarioFigShare ?tokenFigShare ?usuarioGitHub ?tokenGitHub ?useMatching ");
            where.Append("WHERE { ");
            where.Append($@"?s roh:gnossUser <{idGnossUser}>. ");
            where.Append($@"OPTIONAL{{?s roh:usuarioFigShare ?usuarioFigShare. }} ");
            where.Append($@"OPTIONAL{{?s roh:tokenFigShare ?tokenFigShare. }} ");
            where.Append($@"OPTIONAL{{?s roh:usuarioGitHub ?usuarioGitHub. }} ");
            where.Append($@"OPTIONAL{{?s roh:tokenGitHub ?tokenGitHub. }} ");
            where.Append($@"OPTIONAL{{?s roh:useMatching ?useMatching. }} ");
            where.Append("} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), "person");

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    // Usuario FigShare
                    if (fila.ContainsKey("usuarioFigShare"))
                    {
                        foreach (DataUser userData in listaData)
                        {
                            if (userData.id == "usuarioFigShare")
                            {
                                userData.valor = fila["usuarioFigShare"].value;
                                break;
                            }
                        }
                    }

                    // Token FigShare
                    if (fila.ContainsKey("tokenFigShare"))
                    {
                        foreach (DataUser userData in listaData)
                        {
                            if (userData.id == "tokenFigShare")
                            {
                                userData.valor = fila["tokenFigShare"].value;
                                break;
                            }
                        }
                    }

                    // Usuario GitHub
                    if (fila.ContainsKey("usuarioGitHub"))
                    {
                        foreach (DataUser userData in listaData)
                        {
                            if (userData.id == "usuarioGitHub")
                            {
                                userData.valor = fila["usuarioGitHub"].value;
                                break;
                            }
                        }
                    }

                    // Token GitHub
                    if (fila.ContainsKey("tokenGitHub"))
                    {
                        foreach (DataUser userData in listaData)
                        {
                            if (userData.id == "tokenGitHub")
                            {
                                userData.valor = fila["tokenGitHub"].value;
                                break;
                            }
                        }
                    }

                    // Matching
                    if (fila.ContainsKey("useMatching"))
                    {
                        foreach (DataUser userData in listaData)
                        {
                            if (userData.id == "useMatching")
                            {
                                userData.valor = fila["useMatching"].value;
                                break;
                            }
                        }
                    }
                }
            }

            return listaData;
        }

        /// <summary>
        /// Modifica los datos de la fuente de RO de una persona.
        /// </summary>
        /// <param name="pIdGnossUser">ID del usuario de gnoss.</param>
        /// <param name="pDataUser">Objeto con los datos nuevos.</param>
        public void SetDataRedesUsuario(string pIdGnossUser, User pDataUser)
        {
            // Obtención de datos antiguos.
            List<DataUser> datosAntiguos = GetDataRedesUsuario(pIdGnossUser);

            string idRecurso = string.Empty;
            string idGnossUser = $@"http://gnoss/{pIdGnossUser.ToUpper()}";
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

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
            Guid guid = mResourceApi.GetShortGuid(idRecurso);
            Dictionary<Guid, List<TriplesToInclude>> dicInsercion = new Dictionary<Guid, List<TriplesToInclude>>();
            List<TriplesToInclude> listaTriplesInsercion = new List<TriplesToInclude>();
            Dictionary<Guid, List<TriplesToModify>> dicModificacion = new Dictionary<Guid, List<TriplesToModify>>();
            List<TriplesToModify> listaTriplesModificacion = new List<TriplesToModify>();
            Dictionary<Guid, List<RemoveTriples>> dicBorrado = new Dictionary<Guid, List<RemoveTriples>>();
            List<RemoveTriples> listaTriplesBorrado = new List<RemoveTriples>();

            foreach (DataUser item in datosAntiguos)
            {
                string propiedad = item.id;
                string dataViejo = item.valor;
                string dataNuevo = pDataUser.dataUser.FirstOrDefault(x => x.nombre == item.nombre).valor;
                if (dataNuevo == null)
                {
                    dataNuevo = string.Empty;
                }
                if (dataViejo == null)
                {
                    dataViejo = string.Empty;
                }

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
                    else if (string.IsNullOrEmpty(dataNuevo))
                    {
                        // Borrado (Triple).
                        RemoveTriples triple = new RemoveTriples();
                        triple.Predicate = $@"http://w3id.org/roh/{propiedad}";
                        triple.Value = dataViejo;
                        triple.Title = false;
                        triple.Description = false;
                        listaTriplesBorrado.Add(triple);
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

            // Borrado.
            dicBorrado.Add(guid, listaTriplesBorrado);
            mResourceApi.DeletePropertiesLoadedResources(dicBorrado);

            // Modificación.
            dicModificacion.Add(guid, listaTriplesModificacion);
            mResourceApi.ModifyPropertiesLoadedResources(dicModificacion);
        }
    }
}
