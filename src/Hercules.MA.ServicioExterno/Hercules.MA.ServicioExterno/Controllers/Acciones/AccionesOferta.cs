using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using Hercules.MA.ServicioExterno.Models.Offer;
using ClusterOntology;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Hercules.MA.ServicioExterno.Models.Offer.Offer;
using Hercules.MA.ServicioExterno.Models;
using Hercules.MA.ServicioExterno.Models.Graficas.DataItemRelacion;
using Hercules.MA.ServicioExterno.Controllers.Utilidades;
using Hercules.MA.ServicioExterno.Models.Graficas.DataGraficaAreasTags;
using Microsoft.AspNetCore.Cors;

namespace Hercules.MA.ServicioExterno.Controllers.Acciones
{
    [EnableCors("_myAllowSpecificOrigins")]
    public class AccionesOferta
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
        /// Método público para cargar los investigadores del grupo al que pertenece el usuario
        /// </summary>
        /// <param name="researcherId">Datos del cluster para obtener los perfiles</param>
        /// <returns>Diccionario con los datos necesarios para cada persona.</returns>
        
        public Dictionary<string, UsersOffer> LoadUsers(string researcherId)
        {
            //ID persona/ID perfil/score
            Dictionary<string, UsersOffer> respuesta = new();

            // Comprueba que el id dado es un guid válido
            Guid userGUID = Guid.Empty;
            try
            {
                userGUID = new Guid(researcherId);
            }
            catch (Exception e)
            {
                throw new Exception("The id is't a correct guid");
            }


            string select = $@"{ mPrefijos }
                select distinct ?person ?name group_concat(distinct ?group;separator=',') as ?groups ?tituloOrg ?hasPosition ?departamento (count(distinct ?doc)) as ?numDoc
                FROM <http://gnoss.com/organization.owl>  FROM <http://gnoss.com/group.owl> FROM <http://gnoss.com/department.owl> FROM <http://gnoss.com/document.owl>";

            string where = @$"where {{
                ?main a <http://xmlns.com/foaf/0.1/Person>.
                ?main <http://w3id.org/roh/gnossUser> ?idGnoss.
                    
                ?group a <http://xmlns.com/foaf/0.1/Group>.
                ?group roh:membersGroup ?main.
                    
                ?group roh:membersGroup ?person.
                    
                ?person foaf:name ?name.
                OPTIONAL{{
                    ?person roh:hasRole ?organization.
                    ?organization <http://w3id.org/roh/title> ?tituloOrg.
                }}
                OPTIONAL{{
                    ?person <http://w3id.org/roh/hasPosition> ?hasPosition.
                }}
                OPTIONAL{{
                    ?person <http://vivoweb.org/ontology/core#departmentOrSchool> ?dept.
                    ?dept <http://purl.org/dc/elements/1.1/title> ?departamento
                }}
                ?person <http://w3id.org/roh/isActive> 'true'.
                    
                OPTIONAL{{
                    ?doc a <http://purl.org/ontology/bibo/Document>.
                    ?doc <http://w3id.org/roh/isValidated> 'true'.
				    ?doc <http://purl.org/ontology/bibo/authorList> ?authorList.
				    ?authorList <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
                }}

                FILTER(?idGnoss = <http://gnoss/{userGUID.ToString().ToUpper()}>)
            }}";
            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "person");

            foreach (Dictionary<string, SparqlObject.Data> fila in sparqlObject.results.bindings)
            {

                try
                {
                    string person = fila["person"].value.Replace("http://gnoss/", "").ToLower();

                    Guid guid = new Guid(person.Split('_')[1]);
                    string name = fila["name"].value;
                    string groups = fila.ContainsKey("groups") ? fila["groups"].value : null;
                    string organization = fila.ContainsKey("tituloOrg") ? fila["tituloOrg"].value : null;
                    string hasPosition = fila.ContainsKey("hasPosition") ? fila["hasPosition"].value : null;
                    string departamento = fila.ContainsKey("departamento") ? fila["departamento"].value : null;
                    // Número de publicaciones totales, intenta convertirlo en entero
                    string numDoc = fila.ContainsKey("numDoc") ? fila["numDoc"].value : null;
                    int numPublicacionesTotal = 0;
                    int.TryParse(fila["numDoc"].value, out numPublicacionesTotal);

                    respuesta.Add(guid.ToString(), new UsersOffer()
                    {
                        name = name,
                        shortId = guid,
                        id = person,
                        groups = (groups != "" || groups != null) ? groups.Split(',').ToList() : new List<string>(),
                        organization = organization,
                        hasPosition = hasPosition,
                        departamento = departamento,
                        numPublicacionesTotal = numPublicacionesTotal
                    });
                }
                catch (Exception e) { }

            }
            

            return respuesta;
        }




        /// <summary>
        /// Método público para cargar los investigadores del grupo al que pertenece el usuario
        /// </summary>
        /// <param name="ids">Ids de los investigadores</param>
        /// <returns>Diccionario con los datos necesarios para cada persona.</returns>

        public List<string> LoadLineResearchs(string[] ids)
        {
            //ID persona/ID perfil/score
            List<string> respuesta = new();

            List<Guid> usersGUIDs = new();
            foreach (var id in ids)
            {
                try
                {
                    // Comprueba que el id dado es un guid válido
                    usersGUIDs.Add(new Guid(id));

                }
                catch (Exception e)
                {
                    throw new Exception("The id is't a correct guid");
                }

            }

            string select = $@"{ mPrefijos }
                        select distinct ?lineResearch";

            string where = @$"where {{

                        ?group a 'group'.
                        ?group roh:membersGroup ?person.
                        ?group roh:lineResearch ?lineResearch.

                        FILTER(?person in(<http://gnoss/{string.Join(">,<http://gnoss/", usersGUIDs.Select(x => x.ToString().ToUpper()))}>))
                    }}";

            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

            foreach (Dictionary<string, SparqlObject.Data> fila in sparqlObject.results.bindings)
            {

                try
                {
                    string lineResearch = fila["lineResearch"].value;

                    respuesta.Add(lineResearch);
                }
                catch (Exception e) { }

            }

            return respuesta;
        }




        /// <summary>
        /// Método público para cargar los matureStates
        /// </summary>
        /// <param name="lang">Idioma a cargar</param>
        /// <returns>Diccionario con los datos.</returns>

        public Dictionary<string, string> LoadMatureStates(string lang)
        {
            Dictionary<string, string> respuesta = new();

            if (lang.Length < 3)
            {

                string select = $@"{ mPrefijos }
                    select distinct ?identifier ?title";

                string where = @$"where {{
                    ?s a <http://w3id.org/roh/MatureState>.
                    ?s dc:title ?title.
                    ?s dc:identifier ?identifier.
                    FILTER( lang(?title) = '{lang}' OR lang(?title) = '')
                }} ORDER BY ASC(?identifier)";
                SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "maturestate");

                foreach (Dictionary<string, SparqlObject.Data> fila in sparqlObject.results.bindings)
                {

                    try
                    {
                        string identifier = fila["identifier"].value;
                        string title = fila["title"].value;

                        if (!respuesta.ContainsKey(identifier))
                        {

                            respuesta.Add(identifier, title);
                        }

                    }
                    catch (Exception e) { }

                }
            }

            return respuesta;
        }




        /// <summary>
        /// Método público para cargar los FramingSectors
        /// </summary>
        /// <param name="lang">Idioma a cargar</param>
        /// <returns>Diccionario con los datos.</returns>

        public Dictionary<string, string> LoadFramingSectors(string lang)
        {

            Dictionary<string, string> respuesta = new();

            if (lang.Length < 3)
            {

                string select = $@"{ mPrefijos }
                    select distinct ?identifier ?title ?lang";

                string where = @$"where {{
                    ?s a <http://w3id.org/roh/FramingSector>.
                    ?s dc:title ?title.
                    ?s dc:identifier ?identifier.
                    FILTER( lang(?title) = '{lang}' OR lang(?title) = '')
                }} ORDER BY ASC(?title)";
                SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "framingsector");

                foreach (Dictionary<string, SparqlObject.Data> fila in sparqlObject.results.bindings)
                {

                    try
                    {
                        string identifier = fila["identifier"].value;
                        string title = fila["title"].value;

                        if (!respuesta.ContainsKey(identifier))
                        {

                            respuesta.Add(identifier, title);
                        }

                    }
                    catch (Exception e) { }

                }
            }

            return respuesta;
        }

    }
}
