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

            List<string> filtrosPerfiles = new List<string>();
            List<string> filtrosPerfilesTerms = new List<string>();
            List<string> filtrosPerfilesTags = new List<string>();

            string select = $@"{ mPrefijos }
                            select distinct ?person ?name group_concat(distinct ?group;separator=',') as ?groups ?tituloOrg ?hasPosition ?departamento";
            string where = @$"where {{
                    ?main a <http://xmlns.com/foaf/0.1/Person>.
                    ?main <http://w3id.org/roh/gnossUser> ?idGnoss.

                    ?main <http://vivoweb.org/ontology/core#relates> ?group.
                    ?group ?propRol ?person.
                    FILTER(?propRol in (<http://w3id.org/roh/researchers>,<http://w3id.org/roh/mainResearchers>)).
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

                    FILTER(?idGnoss = <http://gnoss/{researcherId.ToUpper()}>)
                }}";
            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

            foreach (Dictionary<string, SparqlObject.Data> fila in sparqlObject.results.bindings)
            {
                string person = fila["person"].value.Replace("http://gnoss/", "").ToLower();
                string name = fila["name"].value;
                string groups = fila["groups"].value;
                string organization = fila["tituloOrg"].value;
                string hasPosition = fila["hasPosition"].value;
                string departamento = fila["departamento"].value;

                respuesta.Add(person, new UsersOffer()
                {
                    name = name,
                    groups = groups.Split(',').ToList(),
                    organization = organization,
                    hasPosition = hasPosition,
                    departamento = departamento
                });

            }

            return respuesta;
        }

        

    }
}
