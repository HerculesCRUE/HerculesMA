﻿using Gnoss.ApiWrapper;
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
        private static string[] listTagsNotForvidden = new string[] { "<ol>", "<li>", "<b>", "<i>", "<u>", "<ul>", "<strike>", "<blockquote>", "<div>", "<hr>", "</ol>", "</li>", "</b>", "</i>", "</u>", "</ul>", "</strike>", "</blockquote>", "</div>", "<br/>" };
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
                    select distinct ?s ?identifier ?title";

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
                        string s = fila["s"].value;
                        string identifier = fila["identifier"].value;
                        string title = fila["title"].value;

                        if (!respuesta.ContainsKey(identifier))
                        {

                            respuesta.Add(s, title);
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
                    select distinct ?s ?identifier ?title ?lang";

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
                        string s = fila["s"].value;
                        string identifier = fila["identifier"].value;
                        string title = fila["title"].value;

                        if (!respuesta.ContainsKey(identifier))
                        {

                            respuesta.Add(s, title);
                        }

                    }
                    catch (Exception e) { }

                }
            }

            return respuesta;
        }

        /// <summary>
        /// Método público para cargar los estados de la oferta (activado, en borrador,...)
        /// </summary>
        /// <param name="lang">Idioma a cargar</param>
        /// <returns>Diccionario con los datos.</returns>

        public List<Tuple<string, string, string>> LoadOfferStates(string lang)
        {
            List<Tuple<string, string, string>> respuesta = new();

            if (lang.Length < 3)
            {

                string select = $@"{ mPrefijos }
                    select distinct ?s ?identifier ?title";

                string where = @$"where {{
                    ?s a <http://w3id.org/roh/OfferState>.
                    ?s dc:title ?title.
                    ?s dc:identifier ?identifier.
                    FILTER( lang(?title) = '{lang}' OR lang(?title) = '')
                }} ORDER BY ASC(?identifier)";
                SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "offerstate");

                foreach (Dictionary<string, SparqlObject.Data> fila in sparqlObject.results.bindings)
                {

                    try
                    {
                        string s = fila["s"].value;
                        string identifier = fila["identifier"].value;
                        string title = fila["title"].value;

                        respuesta.Add(new Tuple<string, string, string>(s, identifier, title));

                    }
                    catch (Exception e) { }

                }
            }

            return respuesta;
        }



        /// <summary>
        /// Controlador para guardar los datos de la oferta 
        /// </summary>
        /// <param name="pIdGnossUser">Usuario de gnoss.</param>
        /// <param name="oferta">Objeto con la oferta tecnológica a añadir / modificar.</param>
        /// <returns>Id de la oferta creada o modificada.</returns>
        public string SaveOffer(string pIdGnossUser, Offer oferta)
        {
            string idRecurso = oferta.entityID;
            int MAX_INTENTOS = 10;
            bool uploadedR = false;

            // Obtener el id del usuario usando el id de la cuenta
            string select = "select ?s ";
            string where = @$"where {{
                    ?s a <http://xmlns.com/foaf/0.1/Person>.
                    ?s <http://w3id.org/roh/gnossUser> ?idGnoss.
                    FILTER(?idGnoss = <http://gnoss/{pIdGnossUser.ToUpper()}>)
                }}";
            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "person");
            var userGnossId = string.Empty;
            sparqlObject.results.bindings.ForEach(e =>
            {
                userGnossId = e["s"].value;
            });

            if (!string.IsNullOrEmpty(userGnossId))
            {
                // Creando el objeto la oferta
                // Creando las categorías
                List<CategoryPath> categorias = new List<CategoryPath>();
                categorias.Add(new CategoryPath() { IdsRoh_categoryNode = oferta.tags });

                List<ClusterPerfil> listClusterPerfil = new();
                // Creando los perfiles del cluster

                // Obtiene el ID largo de los investigadores
                List<string> numMember = new();
                Dictionary<string, string> relationIDs = new();
                if (oferta.researchers != null)
                {
                    oferta.researchers.Values.ToList().ForEach(user =>
                    {
                        if (user != null)
                        {
                            if (user.shortId != Guid.Empty)
                            {
                                numMember.Add("<http://gnoss.com/" + user.shortId + ">");
                            }
                        }
                    });
                    numMember = numMember.Distinct().ToList();

                    // Query to get the full ID
                    if (numMember.Count > 0)
                    {

                        select = "select distinct ?s ?entidad FROM <http://gnoss.com/person.owl>";
                        where = @$"where {{
                            ?s <http://gnoss/hasEntidad> ?entidad.
                            ?entidad a <http://xmlns.com/foaf/0.1/Person>.
                            FILTER(?s in ({string.Join(',', numMember)}))
                        }}
                        ";

                        try
                        {
                            sparqlObject = mResourceApi.VirtuosoQuery(select, where, "person");
                            sparqlObject.results.bindings.ForEach(e =>
                            {
                                relationIDs.Add(e["s"].value, e["entidad"].value);
                            });
                        } catch (Exception e) { }
                        
                    }
                }


                // Obtiene el ID largo de los proyectos
                List<string> numProj = new();
                Dictionary<string, string> relationProjIDs = new();
                if (oferta.projects != null)
                {
                    oferta.projects.Values.ToList().ForEach(item =>
                    {
                        if (item != null)
                        {
                            if (item.shortId != Guid.Empty)
                            {
                                numProj.Add("<http://gnoss.com/" + item.shortId + ">");
                            }
                        }
                    });

                    // Query to get the full ID
                    if (numProj.Count > 0)
                    {

                        select = "select distinct ?s ?entidad FROM <http://gnoss.com/project.owl>";
                        where = @$"where {{
                            ?s <http://gnoss/hasEntidad> ?entidad.
                            ?entidad a <http://vivoweb.org/ontology/core#Project>.
                            FILTER(?s in ({string.Join(',', numProj)}))
                        }}
                        ";
                        try
                        {
                            sparqlObject = mResourceApi.VirtuosoQuery(select, where, "project");
                            sparqlObject.results.bindings.ForEach(e =>
                            {
                                relationProjIDs.Add(e["s"].value, e["entidad"].value);
                            });
                        }
                        catch (Exception e) { }

                    }
                }



                // Obtiene el ID largo de los Documentos
                List<string> numDocs = new();
                Dictionary<string, string> relationDocsIDs = new();
                if (oferta.documents != null)
                {
                    oferta.documents.Values.ToList().ForEach(item =>
                    {
                        if (item != null)
                        {
                            if (item.shortId != Guid.Empty)
                            {
                                numDocs.Add("<http://gnoss.com/" + item.shortId + ">");
                            }
                        }
                    });

                    // Query to get the full ID
                    if (numDocs.Count > 0)
                    {

                        select = "select distinct ?s ?entidad FROM <http://gnoss.com/document.owl>";
                        where = @$"where {{
                            ?s <http://gnoss/hasEntidad> ?entidad.
                            ?entidad a <http://purl.org/ontology/bibo/Document>.
                            FILTER(?s in ({string.Join(',', numDocs)}))
                        }}
                        ";
                        try
                        {
                            sparqlObject = mResourceApi.VirtuosoQuery(select, where, "document");
                            sparqlObject.results.bindings.ForEach(e =>
                            {
                                relationDocsIDs.Add(e["s"].value, e["entidad"].value);
                            });
                        }
                        catch (Exception e) { }

                    }
                }




                // Obtiene el ID largo de los Documentos
                List<string> numPII = new();
                Dictionary<string, string> relationPiiIDs = new();
                if (oferta.pii != null)
                {
                    oferta.documents.Values.ToList().ForEach(item =>
                    {
                        if (item != null)
                        {
                            if (item.shortId != Guid.Empty)
                            {
                                numPII.Add("<http://gnoss.com/" + item.shortId + ">");
                            }
                        }
                    });

                    // Query to get the full ID
                    if (numPII.Count > 0)
                    {

                        select = "select distinct ?s ?entidad FROM <http://gnoss.com/patent.owl>";
                        where = @$"where {{
                            ?s <http://gnoss/hasEntidad> ?entidad.
                            ?entidad a <http://purl.org/ontology/bibo/Patent>.
                            FILTER(?s in ({string.Join(',', numPII)}))
                        }}
                        ";
                        try
                        {
                            sparqlObject = mResourceApi.VirtuosoQuery(select, where, "patent");
                            sparqlObject.results.bindings.ForEach(e =>
                            {
                                relationPiiIDs.Add(e["s"].value, e["entidad"].value);
                            });
                        }
                        catch (Exception e) { }

                    }
                }


                // Obtengo todos los estados
                var todosLosEstados = LoadOfferStates("es");
                var estado = todosLosEstados.Find(e => e.Item2 == "001");


                // Registrar cambio en la disponibilidad
                OfferOntology.AvailabilityChangeEvent availabilityChangeEvent = new();
                availabilityChangeEvent.IdRoh_roleOf = userGnossId;
                availabilityChangeEvent.IdSchema_availability = estado.Item1;
                availabilityChangeEvent.Schema_validFrom = DateTime.UtcNow;

                // creando los cluster
                OfferOntology.Offer cRsource = new();
                // Usuario creador
                cRsource.IdSchema_offeredBy = userGnossId;
                // Otros campos
                cRsource.Schema_name = oferta.name;
                cRsource.Dct_issued = DateTime.UtcNow;
                // Estado inicial (En borrador)
                cRsource.IdSchema_availability = estado.Item1;
                // Sección de las descripciones, limpiamos los strings de tags que no queramos
                cRsource.Schema_description = oferta.objectFieldsHtml.descripcion != null ? CleanHTML.StripTagsCharArray(oferta.objectFieldsHtml.descripcion, listTagsNotForvidden) : "";
                cRsource.Roh_innovation = oferta.objectFieldsHtml.innovacion != null ? CleanHTML.StripTagsCharArray(oferta.objectFieldsHtml.innovacion, listTagsNotForvidden) : "";
                cRsource.Drm_origin = oferta.objectFieldsHtml.origen != null ? CleanHTML.StripTagsCharArray(oferta.objectFieldsHtml.origen, listTagsNotForvidden) : "";
                cRsource.Roh_partnerType = oferta.objectFieldsHtml.socios != null ? CleanHTML.StripTagsCharArray(oferta.objectFieldsHtml.socios, listTagsNotForvidden) : "";
                cRsource.Roh_collaborationSought = oferta.objectFieldsHtml.colaboracion != null ? CleanHTML.StripTagsCharArray(oferta.objectFieldsHtml.colaboracion, listTagsNotForvidden) : "";
                cRsource.Qb_observation = oferta.objectFieldsHtml.observaciones != null ? CleanHTML.StripTagsCharArray(oferta.objectFieldsHtml.observaciones, listTagsNotForvidden) : "";
                cRsource.Roh_application = oferta.objectFieldsHtml.aplicaciones != null ? CleanHTML.StripTagsCharArray(oferta.objectFieldsHtml.aplicaciones, listTagsNotForvidden) : "";
                cRsource.Bibo_recipient = oferta.objectFieldsHtml.destinatarios != null ? CleanHTML.StripTagsCharArray(oferta.objectFieldsHtml.destinatarios, listTagsNotForvidden) : "";
                // Selectores de los estados de madurez y el sector
                cRsource.IdRoh_framingSector = oferta.framingSector != null ? oferta.framingSector : null;
                cRsource.IdBibo_status = oferta.matureState != null ? oferta.matureState : null;
                // Añadir evento de creación
                cRsource.Roh_availabilityChangeEvent = new();
                try
                {
                    cRsource.Roh_availabilityChangeEvent.Add(availabilityChangeEvent);
                }
                catch (Exception e) { }

                try
                {
                    cRsource.Vivo_freetextKeyword = oferta.tags;
                    cRsource.Roh_lineResearch = oferta.lineResearchs.Values.ToList();
                }
                catch (Exception e) {}

                // Añadir los investigadores de la oferta
                try
                {
                    cRsource.IdsRoh_researchers =  relationIDs.Values.ToList();
                }
                catch (Exception e) { }


                // Proyectos, Documentos y PII
                try
                {
                    if (oferta.projects != null)
                    {
                        cRsource.IdsRoh_project = relationProjIDs.Values.ToList();
                    }
                    else
                    {
                        cRsource.IdsRoh_project = null;
                    }
                } catch (Exception e)
                {
                    cRsource.IdsRoh_project = null;
                }
                try
                {
                    if (oferta.documents != null)
                    {
                        cRsource.IdsRoh_document = relationDocsIDs.Values.ToList();
                    } else
                    {
                        cRsource.IdsRoh_document = null;
                    }
                }
                catch (Exception e)
                {
                    cRsource.IdsRoh_document = null;
                }
                try
                {
                    if (oferta.pii != null)
                    {
                        cRsource.IdsRoh_patents = relationPiiIDs.Values.ToList();
                    } else
                    {
                        cRsource.IdsRoh_patents = null;
                    }
                }
                catch (Exception e)
                {
                    cRsource.IdsRoh_patents = null;
                }


                // Guardando o actualizando el recurso
                mResourceApi.ChangeOntoly("offer");
                // Comprueba si es una actualización o no
                if (idRecurso != null && idRecurso != "")
                {
                    string[] recursoSplit = idRecurso.Split('_');

                    // Modificación.
                    ComplexOntologyResource resource = cRsource.ToGnossApiResource(mResourceApi, null, new Guid(recursoSplit[recursoSplit.Length - 2]), new Guid(recursoSplit[recursoSplit.Length - 1]));
                    int numIntentos = 0;
                    while (!resource.Modified)
                    {
                        numIntentos++;
                        if (numIntentos > MAX_INTENTOS)
                        {
                            break;
                        }

                        mResourceApi.ModifyComplexOntologyResource(resource, false, false);
                        uploadedR = resource.Modified;
                    }

                }
                else
                {
                    // Inserción.
                    ComplexOntologyResource resource = cRsource.ToGnossApiResource(mResourceApi, null);
                    int numIntentos = 0;
                    while (!resource.Uploaded)
                    {
                        numIntentos++;
                        if (numIntentos > MAX_INTENTOS)
                        {
                            break;
                        }
                        idRecurso = mResourceApi.LoadComplexSemanticResource(resource, true, true);
                        uploadedR = resource.Uploaded;
                    }
                }
            }

            if (uploadedR)
            {
                return idRecurso;
            }
            else
            {
                throw new Exception("Recurso no creado");
            }
        }

    }
}
