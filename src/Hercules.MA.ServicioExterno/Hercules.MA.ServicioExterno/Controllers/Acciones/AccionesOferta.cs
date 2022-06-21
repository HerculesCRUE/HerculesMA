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
        /// Método público para eliminar una oferta tecnológica
        /// </summary>
        /// <param name="pIdOfertaId">Identificador de la oferta</param>
        /// <returns>Diccionario con las listas de thesaurus.</returns>
        public bool BorrarOferta(string pIdOfertaId)
        {

            if (!string.IsNullOrEmpty(pIdOfertaId))
            {
                // Carga los datos del Cluster
                // Models.Offer.Offer OfferData = LoadOffer(pIdOfertaId);

                // Establezco las entidades secundarias a borrar
                List<string> urlSecondaryListEntities = new() { "http://w3id.org/roh/availabilityChangeEvent" };

                mResourceApi.ChangeOntoly("offer");

                if (pIdOfertaId != null && pIdOfertaId != "")
                {
                    Guid resourceGuid = mResourceApi.GetShortGuid(pIdOfertaId);

                    try
                    {
                        mResourceApi.CommunityShortName = mResourceApi.GetCommunityShortNameByResourceID(resourceGuid);

                        // Establece las entidades secundarias a borrar
                        mResourceApi.DeleteSecondaryEntitiesList(ref urlSecondaryListEntities);
                        // borra el recurso
                        mResourceApi.PersistentDelete(resourceGuid);
                    }
                    catch (Exception e)
                    {
                        return false;
                    }

                }
            }
            else
            {
                throw new Exception("Recurso no borrado");
            }
            return true;
        }


        /// <summary>
        /// Método público para obtener los datos de un cluster
        /// </summary>
        /// <param name="pIdOfertaId">Identificador del cluster</param>
        /// <returns>Diccionario con las listas de thesaurus.</returns>
        internal Models.Offer.Offer LoadOffer(string pIdOfertaId)
        {

            // Obtener datos del cluster
            string select = "select ?p ?o ";
            string where = @$"where {{
                    ?s a <http://www.schema.org/Offer>.
                    ?s ?p ?o.
                    FILTER(?s = <{pIdOfertaId}>)
                }}";
            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "offer");

            // Inicizalizamos el modelo del Cluster para devolver
            Models.Offer.Offer pDataOffer = new();
            pDataOffer.lineResearchs = new();
            pDataOffer.tags = new();
            pDataOffer.objectFieldsHtml = new();
            pDataOffer.researchers = new();
            pDataOffer.documents = new();
            pDataOffer.projects = new();
            pDataOffer.pii = new();


            sparqlObject.results.bindings.ForEach(e =>
            {
                pDataOffer.entityID = pIdOfertaId;

                switch (e["p"].value)
                {
                    case "http://www.schema.org/name":
                        pDataOffer.name = e["o"].value;
                    break;
                    case "http://w3id.org/roh/researchers":
                        try
                        {
                            pDataOffer.researchers.Add(UtilidadesAPI.ObtenerIdCorto(mResourceApi, e["o"].value), new UsersOffer() { id = e["o"].value, shortId = UtilidadesAPI.ObtenerIdCorto(mResourceApi, e["o"].value) });
                        } catch (Exception xcp) { }
                        break;
                    case "http://w3id.org/roh/document":
                        try
                        {
                            pDataOffer.documents.Add(UtilidadesAPI.ObtenerIdCorto(mResourceApi, e["o"].value), new DocumentsOffer() { id = e["o"].value, shortId = UtilidadesAPI.ObtenerIdCorto(mResourceApi, e["o"].value) });
                        }
                        catch (Exception xcp) { }
                        break;
                    case "http://w3id.org/roh/project":
                        try
                        {
                            pDataOffer.projects.Add(UtilidadesAPI.ObtenerIdCorto(mResourceApi, e["o"].value), new ProjectsOffer() { id = e["o"].value, shortId = UtilidadesAPI.ObtenerIdCorto(mResourceApi, e["o"].value) });
                        }
                        catch (Exception xcp) { }
                        break;
                    case "http://w3id.org/roh/patents":
                        try
                        {
                            pDataOffer.pii.Add(UtilidadesAPI.ObtenerIdCorto(mResourceApi, e["o"].value), new PIIOffer() { id = e["o"].value, shortId = UtilidadesAPI.ObtenerIdCorto(mResourceApi, e["o"].value) });
                        }
                        catch (Exception xcp) { }
                        break;
                    case "http://vocab.data.gov/def/drm#origin":
                        pDataOffer.objectFieldsHtml.origen = e["o"].value;
                        break;
                    case "http://w3id.org/roh/lineResearch":
                        pDataOffer.lineResearchs.Add(e["o"].value, e["o"].value);
                        break;
                    case "http://vivoweb.org/ontology/core#freetextKeyword":
                        pDataOffer.tags.Add(e["o"].value);
                        break;
                    case "http://w3id.org/roh/innovation":
                        pDataOffer.objectFieldsHtml.innovacion = e["o"].value;
                        break;
                    case "http://w3id.org/roh/collaborationSought":
                        pDataOffer.objectFieldsHtml.colaboracion = e["o"].value;
                        break;
                    case "http://w3id.org/roh/partnerType":
                        pDataOffer.objectFieldsHtml.socios = e["o"].value;
                        break;
                    case "http://purl.org/linked-data/cube#observation":
                        pDataOffer.objectFieldsHtml.observaciones = e["o"].value;
                        break;
                    case "http://purl.org/ontology/bibo/recipient":
                        pDataOffer.objectFieldsHtml.destinatarios = e["o"].value;
                        break;
                    case "http://purl.org/dc/terms/issued":
                        try
                        {
                            pDataOffer.date = e["o"].value;
                        }
                        catch (Exception xcp) { }
                        break;
                    case "http://www.schema.org/offeredBy":
                        pDataOffer.creatorId = e["o"].value;
                        break;
                    case "http://w3id.org/roh/application":
                        pDataOffer.objectFieldsHtml.aplicaciones = e["o"].value;
                        break;
                    case "http://www.schema.org/availability":
                        pDataOffer.state = e["o"].value;
                        break;
                    case "http://www.schema.org/description":
                        pDataOffer.objectFieldsHtml.descripcion = e["o"].value;
                        break;
                    case "http://w3id.org/roh/framingSector":
                        pDataOffer.framingSector = e["o"].value;
                        break;
                    case "http://purl.org/ontology/bibo/status":
                        pDataOffer.matureState = e["o"].value;
                        break;
                }
            });


            // Obtenemos los resúmenes de los investigadores y los añadimos al objeto de la oferta
            try
            {
                pDataOffer.researchers = GetUsersTeaser(pDataOffer.researchers.Values.Select(x => x.id).ToList());
            } catch (Exception ext) { }


            // Obtenemos los resúmenes de los documentos y los añadimos al objeto de la oferta
            try
            {
                pDataOffer.documents = GetDocumentsTeaser(pDataOffer.documents.Values.Select(x => x.id).ToList());
            }
            catch (Exception ext) { }



            // Obtenemos los resúmenes de los projectos y los añadimos al objeto de la oferta
            try
            {
                pDataOffer.projects = GetProjectsTeaser(pDataOffer.projects.Values.Select(x => x.id).ToList());
            }
            catch (Exception ext) { }



            // Obtenemos los resúmenes de las propiedades industriales intelectuales (PII) y los añadimos al objeto de la oferta
            try
            {
                pDataOffer.pii = GetPIITeaser(pDataOffer.pii.Values.Select(x => x.id).ToList());
            }
            catch (Exception ext) { }


            return pDataOffer;
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
                Dictionary<Guid, string> relationIDs = new();
                if (oferta.researchers != null)
                {
                    try
                    {
                        relationIDs = UtilidadesAPI.GetLongIds(oferta.researchers.Select(e => e.Key).ToList(), mResourceApi, "http://xmlns.com/foaf/0.1/Person", "person");
                    }
                    catch (Exception e) { }
                }


                // Obtiene el ID largo de los proyectos
                List<string> numProj = new();
                Dictionary<Guid, string> relationProjIDs = new();
                if (oferta.projects != null)
                {
                    try
                    {
                        relationProjIDs = UtilidadesAPI.GetLongIds(oferta.projects.Select(e => e.Key).ToList(), mResourceApi, "http://vivoweb.org/ontology/core#Project", "project");
                    }
                    catch (Exception e) { }
                }



                // Obtiene el ID largo de los Documentos
                List<string> numDocs = new();
                Dictionary<Guid, string> relationDocsIDs = new();
                if (oferta.documents != null)
                {
                    try
                    {
                        relationDocsIDs = UtilidadesAPI.GetLongIds(oferta.documents.Select(e => e.Key).ToList(), mResourceApi, "http://purl.org/ontology/bibo/Document", "document");
                    }
                    catch (Exception e) { }
                }




                // Obtiene el ID largo de los Documentos
                List<string> numPII = new();
                Dictionary<Guid, string> relationPiiIDs = new();
                if (oferta.pii != null)
                {
                    try
                    {
                        relationPiiIDs = UtilidadesAPI.GetLongIds(oferta.pii.Select(e => e.Key).ToList(), mResourceApi, "http://purl.org/ontology/bibo/Patent", "patent");
                    }
                    catch (Exception e) { }
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

                    // Si es una actualización, hay que recuperar los cambios anteriores del evento
                    select = "SELECT distinct ?s ?roleOf ?validFrom ?availability \n";
                    select += "FROM <http://gnoss.com/offer.owl> FROM<http://gnoss.com/person.owl> FROM<http://gnoss.com/offerstate.owl>";
                    where = @$"where {{
                            ?offer <http://w3id.org/roh/availabilityChangeEvent> ?s.
                            ?s <http://w3id.org/roh/roleOf> ?roleOf.
                            ?s <http://www.schema.org/validFrom> ?validFrom.
                            ?s <http://www.schema.org/availability> ?availability.
	                        ?s ?p ?o.
	                        FILTER(?offer = <{idRecurso}>)
                        }}";

                    try
                    {
                        sparqlObject = mResourceApi.VirtuosoQuery(select, where, "offer");
                        sparqlObject.results.bindings.ForEach(e =>
                        {

                            //Use of DateTime.ParseExact()
                            // Convert the date into DateTime object
                            DateTime DateObject = new DateTime();
                            try
                            {
                                DateObject = DateTime.ParseExact(e["validFrom"].value, "yyyyMMddhhmmss", null);
                            } catch (Exception exc) { }

                            cRsource.Roh_availabilityChangeEvent.Add(new OfferOntology.AvailabilityChangeEvent()
                            {
                                GNOSSID = e["s"].value,
                                IdRoh_roleOf = e["roleOf"].value,
                                IdSchema_availability = e["availability"].value,
                                Schema_validFrom = DateObject,
                            });
                        });
                    }
                    catch (Exception e) { }


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


        /// <summary>
        /// Función que obtiene un resumen de los investigadores enviados en una lista de IDs (cortos o largos) 
        /// </summary>
        /// <param name="ids">Listado de investigadores.</param>
        /// <param name="isLongIds">Booleano que determina si los Ids son Ids largos o cortos.</param>
        /// <returns>relación entre el guid y un objeto de un usuario (resumido).</returns>
        internal Dictionary<Guid, UsersOffer> GetUsersTeaser (List<string> ids, bool isLongIds = true)
        {

            Dictionary<Guid, UsersOffer> result = new();

            // Obtiene los ids largos si únicamente disponemos de los cortos
            List<string> longIds = ids;
            if (!isLongIds)
            {
                // 1. Convierte los ids cortos en guid
                // 2. Llama a la función GetLongIds para obtener los Ids largos
                // 3. Selecciona únicamente los Ids largos
                try
                {
                    longIds = UtilidadesAPI.GetLongIds(ids.Select(e => new Guid(e)).ToList(), mResourceApi, "http://xmlns.com/foaf/0.1/Person", "person").Select(e => e.Value).ToList();
                } catch (Exception e) { }
            }


            // Obtenemos todos los datos de los perfiles y Añadimos el perfil creado a los datos del cluster
            string select = "select distinct ?memberPerfil ?nombreUser ?hasPosition ?tituloOrg ?departamento (count(distinct ?doc)) as ?numDoc (count(distinct ?proj)) as ?ipNumber FROM <http://gnoss.com/person.owl> FROM <http://gnoss.com/document.owl> FROM <http://gnoss.com/project.owl> FROM <http://gnoss.com/organization.owl> FROM <http://gnoss.com/department.owl>";
            string where = @$"where {{
                ?memberPerfil <http://xmlns.com/foaf/0.1/name> ?nombreUser.
                OPTIONAL {{
                    ?doc a <http://purl.org/ontology/bibo/Document>.
                    ?doc <http://w3id.org/roh/isValidated> 'true'.
                    ?doc <http://purl.org/ontology/bibo/authorList> ?authorList.
                    ?authorList <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?memberPerfil.
                }}
                OPTIONAL {{
                    ?proj a <http://vivoweb.org/ontology/core#Project>.
                    ?proj <http://w3id.org/roh/isValidated> 'true'.
                    ?proj <http://vivoweb.org/ontology/core#relates> ?listprojauth.
                    ?listprojauth <http://w3id.org/roh/roleOf> ?memberPerfil.
                    ?listprojauth <http://w3id.org/roh/isIP> 'true'.
                }}
                OPTIONAL {{
                    ?memberPerfil <http://w3id.org/roh/hasPosition> ?hasPosition.
                }}
                OPTIONAL {{
                    ?memberPerfil <http://vivoweb.org/ontology/core#departmentOrSchool> ?dept.
                    ?dept <http://purl.org/dc/elements/1.1/title> ?departamento
                }}
                OPTIONAL {{
                    ?memberPerfil <http://w3id.org/roh/hasRole> ?org.
                    ?org <http://w3id.org/roh/title> ?tituloOrg
                }}
                FILTER(?memberPerfil in ({string.Join(",", longIds.Select(x => "<" + x + ">")) }))
            }}";

            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "person");

            // Carga los datos en el objeto
            sparqlObject.results.bindings.ForEach(e =>
            {
                try
                {
                    Guid currentShortId = UtilidadesAPI.ObtenerIdCorto(mResourceApi, e["memberPerfil"].value);
                    result.Add(currentShortId, new UsersOffer()
                    {
                        id = e["memberPerfil"].value,
                        shortId = currentShortId,
                        name = e["nombreUser"].value,
                        ipNumber = e.ContainsKey("ipNumber") ? int.Parse(e["ipNumber"].value) : 0,
                        numPublicaciones = e.ContainsKey("numDoc") ? int.Parse(e["numDoc"].value) : 0,
                        organization = e.ContainsKey("tituloOrg") ? e["tituloOrg"].value : "",
                        departamento = e.ContainsKey("departamento") ? e["departamento"].value : "",
                        hasPosition = e.ContainsKey("hasPosition") ? e["hasPosition"].value : "",

                    });
                }
                catch (Exception ext) { new Exception("Ha habido un error al procesar los datos de los usuarios:" + ext.Message); }

            });


            return result;

        }



        /// <summary>
        /// Función que obtiene un resumen de los documentos enviados en una lista de IDs (cortos o largos) 
        /// </summary>
        /// <param name="ids">Listado (Ids) de los documentos.</param>
        /// <param name="isLongIds">Booleano que determina si los Ids son Ids largos o cortos.</param>
        /// <returns>relación entre el guid y el objeto de los documentos correspondientes (resumido).</returns>
        internal Dictionary<Guid, DocumentsOffer> GetDocumentsTeaser(List<string> ids, bool isLongIds = true)
        {

            Dictionary<Guid, DocumentsOffer> result = new();

            // Obtiene los ids largos si únicamente disponemos de los cortos
            List<string> longIds = ids;
            if (!isLongIds)
            {
                // 1. Convierte los ids cortos en guid
                // 2. Llama a la función GetLongIds para obtener los Ids largos
                // 3. Selecciona únicamente los Ids largos
                try
                {
                    longIds = UtilidadesAPI.GetLongIds(ids.Select(e => new Guid(e)).ToList(), mResourceApi, "http://purl.org/ontology/bibo/Document", "document").Select(e => e.Value).ToList();
                }
                catch (Exception e) { }
            }


            // Obtenemos los datos de los documentos y lo guardamos en el diccionario
            string select = $@"{ mPrefijos }
                SELECT DISTINCT ?s ?title ?fecha ?description ?organizacion ?start ?end GROUP_CONCAT(?userName;separator=',') as ?autores 
                FROM <http://gnoss.com/organization.owl> FROM <http://gnoss.com/person.owl>";

            string where = @$"where {{
                ?s a <http://purl.org/ontology/bibo/Document>.
                ?s <http://w3id.org/roh/title> ?title.
                ?s <http://w3id.org/roh/isValidated> 'true'.
                OPTIONAL{{ ?s <http://purl.org/ontology/bibo/abstract> ?description}}.
                OPTIONAL{{ ?s <http://purl.org/dc/terms/issued> ?fecha}}.
                OPTIONAL{{ ?s <http://w3id.org/roh/presentedAtOrganizerTitle> ?organizacion }}.
                OPTIONAL{{ ?s <http://w3id.org/roh/presentedAtStart> ?start}}.
                OPTIONAL{{ ?s <http://w3id.org/roh/presentedAtEnd> ?end}}.
                OPTIONAL{{ 
                    ?s <http://purl.org/ontology/bibo/authorList> ?listaAutores.
				    ?listaAutores <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
                    ?person <http://xmlns.com/foaf/0.1/name> ?userName.
                }}
                FILTER(?s in ({string.Join(",", longIds.Select(x => "<" + x + ">")) }))
            }}";

            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "document");

            // Carga los datos en el objeto
            sparqlObject.results.bindings.ForEach(e =>
            {
                try
                {
                    Guid currentShortId = UtilidadesAPI.ObtenerIdCorto(mResourceApi, e["s"].value);
                    result.Add(currentShortId, new DocumentsOffer()
                    {
                        id = e["s"].value,
                        shortId = currentShortId,
                        name = e["title"].value,
                        info = e.ContainsKey("organizacion") ? e["organizacion"].value : "",
                        description = e.ContainsKey("description") ? e["description"].value : "",
                        dates = new string[] { e.ContainsKey("start") ? e["start"].value : (e.ContainsKey("fecha") ? e["fecha"].value : ""), e.ContainsKey("end") ? e["end"].value : "" },
                        researchers = e.ContainsKey("autores") ? e["autores"].value.Split(",").ToList() : new List<string>(),

                    });
                }
                catch (Exception ext) { new Exception("Ha habido un error al procesar los datos de los documentos:" + ext.Message); }

            });


            return result;

        }



        /// <summary>
        /// Función que obtiene un resumen de los proyectos enviados en una lista de IDs (cortos o largos) 
        /// </summary>
        /// <param name="ids">Listado (Ids) de los proyectos.</param>
        /// <param name="isLongIds">Booleano que determina si los Ids son Ids largos o cortos.</param>
        /// <returns>relación entre el guid y el objeto de los proyectos correspondientes (resumido).</returns>
        internal Dictionary<Guid, ProjectsOffer> GetProjectsTeaser(List<string> ids, bool isLongIds = true)
        {

            Dictionary<Guid, ProjectsOffer> result = new();

            // Obtiene los ids largos si únicamente disponemos de los cortos
            List<string> longIds = ids;
            if (!isLongIds)
            {
                // 1. Convierte los ids cortos en guid
                // 2. Llama a la función GetLongIds para obtener los Ids largos
                // 3. Selecciona únicamente los Ids largos
                try
                {
                    longIds = UtilidadesAPI.GetLongIds(ids.Select(e => new Guid(e)).ToList(), mResourceApi, "http://vivoweb.org/ontology/core#Project", "project").Select(e => e.Value).ToList();
                }
                catch (Exception e) { }
            }


            // Obtenemos los datos de los proyectos y lo guardamos en el diccionario
            string select = $@"{ mPrefijos }
                SELECT DISTINCT ?s ?title ?end ?start ?description ?geographicRegion ?organizacion GROUP_CONCAT(?userName;separator=',') as ?autores 
                FROM <http://gnoss.com/organization.owl>";

            string where = @$"where {{

                ?s a <http://vivoweb.org/ontology/core#Project>.
                ?s <http://w3id.org/roh/title> ?title.
                ?s <http://w3id.org/roh/isValidated> 'true'.
                OPTIONAL{{ ?s <http://vivoweb.org/ontology/core#end> ?end }}
                OPTIONAL{{ ?s <http://vivoweb.org/ontology/core#start> ?start }}
                OPTIONAL{{ ?s <http://vivoweb.org/ontology/core#description> ?description }}
                OPTIONAL{{
                    ?s <http://w3id.org/roh/conductedBy> ?conductedBy.
                    ?conductedBy <http://w3id.org/roh/title> ?organizacion
                }}
                OPTIONAL{{
                    ?s <http://vivoweb.org/ontology/core#geographicFocus> ?o.
                    ?o <http://purl.org/dc/elements/1.1/title> ?geographicRegion.
                    FILTER(lang(?geographicRegion)='es')
                }}
                OPTIONAL{{ 
                    ?s ?pAux ?listaAutores
                    FILTER (?pAux in (<http://w3id.org/roh/mainResearchers>, <http://w3id.org/roh/researchers>)).
                    ?listaAutores <http://xmlns.com/foaf/0.1/nick> ?userName.
                }}
                FILTER(?s in ({string.Join(",", longIds.Select(x => "<" + x + ">")) }))
            }}";

            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "project");

            // Carga los datos en el objeto
            sparqlObject.results.bindings.ForEach(e =>
            {
                try
                {
                    Guid currentShortId = UtilidadesAPI.ObtenerIdCorto(mResourceApi, e["s"].value);
                    string geographicRegion = e.ContainsKey("geographicRegion") ? e["geographicRegion"].value : "";
                    string organizacion = e.ContainsKey("organizacion") ? e["organizacion"].value : "";
                    var info = geographicRegion + ((geographicRegion != "" && organizacion != "") ? ", " : "") + organizacion;

                    result.Add(currentShortId, new ProjectsOffer()
                    {
                        id = e["s"].value,
                        shortId = currentShortId,
                        name = e["title"].value,
                        info = info,
                        description = e.ContainsKey("description") ? e["description"].value : "",
                        dates = new string[] { e.ContainsKey("start") ? e["start"].value : "", e.ContainsKey("end") ? e["end"].value : "" },
                        researchers = e.ContainsKey("autores") ? e["autores"].value.Split(",").ToList() : new List<string>(),

                    });
                }
                catch (Exception ext) { new Exception("Ha habido un error al procesar los datos de los proyectos:" + ext.Message); }

            });


            return result;

        }




        /// <summary>
        /// Función que obtiene un resumen de los proyectos enviados en una lista de IDs (cortos o largos) 
        /// </summary>
        /// <param name="ids">Listado (Ids) de los proyectos.</param>
        /// <param name="isLongIds">Booleano que determina si los Ids son Ids largos o cortos.</param>
        /// <returns>relación entre el guid y el objeto de los proyectos correspondientes (resumido).</returns>
        internal Dictionary<Guid, PIIOffer> GetPIITeaser(List<string> ids, bool isLongIds = true)
        {

            Dictionary<Guid, PIIOffer> result = new();

            // Obtiene los ids largos si únicamente disponemos de los cortos
            List<string> longIds = ids;
            if (!isLongIds)
            {
                // 1. Convierte los ids cortos en guid
                // 2. Llama a la función GetLongIds para obtener los Ids largos
                // 3. Selecciona únicamente los Ids largos
                try
                {
                    longIds = UtilidadesAPI.GetLongIds(ids.Select(e => new Guid(e)).ToList(), mResourceApi, "http://vivoweb.org/ontology/core#Project", "project").Select(e => e.Value).ToList();
                }
                catch (Exception e) { }
            }


            // Obtenemos los datos de los proyectos y lo guardamos en el diccionario
            string select = $@"{ mPrefijos }
                SELECT DISTINCT ?s ?title ?end ?start ?description ?geographicRegion ?organizacion GROUP_CONCAT(?userName;separator=',') as ?autores 
                FROM <http://gnoss.com/organization.owl>";

            string where = @$"where {{

                ?s a <http://vivoweb.org/ontology/core#Project>.
                ?s <http://w3id.org/roh/title> ?title.
                ?s <http://w3id.org/roh/isValidated> 'true'.
                OPTIONAL{{ ?s <http://vivoweb.org/ontology/core#end> ?end }}
                OPTIONAL{{ ?s <http://vivoweb.org/ontology/core#start> ?start }}
                OPTIONAL{{ ?s <http://vivoweb.org/ontology/core#description> ?description }}
                OPTIONAL{{
                    ?s <http://w3id.org/roh/conductedBy> ?conductedBy.
                    ?conductedBy <http://w3id.org/roh/title> ?organizacion
                }}
                OPTIONAL{{
                    ?s <http://vivoweb.org/ontology/core#geographicFocus> ?o.
                    ?o <http://purl.org/dc/elements/1.1/title> ?geographicRegion.
                    FILTER(lang(?geographicRegion)='es')
                }}
                OPTIONAL{{ 
                    ?s ?pAux ?listaAutores
                    FILTER (?pAux in (<http://w3id.org/roh/mainResearchers>, <http://w3id.org/roh/researchers>)).
                    ?listaAutores <http://xmlns.com/foaf/0.1/nick> ?userName.
                }}
                FILTER(?s in ({string.Join(",", longIds.Select(x => "<" + x + ">")) }))
            }}";

            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "project");

            // Carga los datos en el objeto
            sparqlObject.results.bindings.ForEach(e =>
            {
                try
                {
                    Guid currentShortId = UtilidadesAPI.ObtenerIdCorto(mResourceApi, e["s"].value);
                    string geographicRegion = e.ContainsKey("geographicRegion") ? e["geographicRegion"].value : "";
                    string organizacion = e.ContainsKey("organizacion") ? e["organizacion"].value : "";
                    var info = geographicRegion + ((geographicRegion != "" && organizacion != "") ? ", " : "") + organizacion;

                    result.Add(currentShortId, new PIIOffer()
                    {
                        id = e["s"].value,
                        shortId = currentShortId,
                        name = e["title"].value,
                        info = info,
                        description = e.ContainsKey("description") ? e["description"].value : "",
                        dates = new string[] { e.ContainsKey("start") ? e["start"].value : "", e.ContainsKey("end") ? e["end"].value : "" },
                        researchers = e.ContainsKey("autores") ? e["autores"].value.Split(",").ToList() : new List<string>(),

                    });
                }
                catch (Exception ext) { new Exception("Ha habido un error al procesar los datos de los proyectos:" + ext.Message); }

            });


            return result;

        }


    }
}
