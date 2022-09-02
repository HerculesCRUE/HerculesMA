using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using Hercules.MA.ServicioExterno.Models.Offer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hercules.MA.ServicioExterno.Controllers.Utilidades;
using Microsoft.AspNetCore.Cors;
using Hercules.MA.ServicioExterno.Models.Cluster;
using Hercules.MA.ServicioExterno.Models.ROsLinked;

namespace Hercules.MA.ServicioExterno.Controllers.Acciones
{


    public class AccionesRosVinculados
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
        /// Método público para cargar los estados de la oferta (activado, en borrador,...)
        /// </summary>
        /// <param name="idRecurso">Id del RO sobre el que obtener los elementos linkados</param>
        /// <param name="pIdGnossUser">Id del usuario que realiza la acción</param>
        /// <param name="lang">Idioma de los literales para la consulta</param>
        /// <returns>Diccionario con los datos.</returns>
        public List<ROLinked> LoadRosLinked(string idRecurso, string lang = "es")
        {

            // Selecciono qué tipo de RO son los recursos pasados y obtengo las propiedades de la ontología
            ResTypeRo typeResource = GetTypeRo(idRecurso);


            // Establezco la propiedad que se va a usar dependiendo de si es un documento que estoy relacionando o es un RO
            string predicateLinkInRO = string.Empty;
            if (typeResource.typeRO == TypeRO.Document)
            {
                predicateLinkInRO = "http://w3id.org/roh/linkedDocument";
            }
            else
            {
                predicateLinkInRO = "http://w3id.org/roh/linkedRO";
            }


            // Obtengo el id del recurso si es un Guid
            Guid guid = Guid.Empty;
            Dictionary<Guid, string> longsId = new();
            if (Guid.TryParse(idRecurso, out guid))
            {
                longsId = UtilidadesAPI.GetLongIds(new List<Guid>() { guid }, mResourceApi, typeResource.longType, typeResource.type);
                idRecurso = longsId[guid];
            }
            else
            {
                guid = mResourceApi.GetShortGuid(idRecurso);
            }


            // Listado de clusters para rellenar.
            List<ROLinked> rosLinked = new();
            List<string> listIdslinked = new();

            // Obtener el los profiles a través del id de usuario de la cuenta
            string select = "select DISTINCT ?s ?title ?abstract ?issued ?type ?roType ?roTypeTitle ?origin group_concat(distinct ?idGnossL;separator=',') as ?idGnoss group_concat(distinct ?clKnowledgeArea;separator=',') as ?gckarea " +
                "FROM <http://gnoss.com/document.owl> FROM <http://gnoss.com/person.owl> FROM <http://gnoss.com/researchobject.owl> FROM <http://gnoss.com/taxonomy.owl>";
            string where = @$"where {{

                    {{

                        # ?personR a <http://xmlns.com/foaf/0.1/Person>.
                        # OPTIONAL
                        # {{
                        #    ?resource <http://purl.org/ontology/bibo/authorList> ?authorListR.
                        #    ?authorListR <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?personR.
                        #    ?personR <http://w3id.org/roh/gnossUser> ?idGnossR.
                        # }}

                        # Obtenemos los elementos relacionados desde el recurso dado
                        ?resource ?related ?s.
                        Filter (?related in (<http://w3id.org/roh/linkedDocument>, <http://w3id.org/roh/linkedRO>))

                        ?personL a <http://xmlns.com/foaf/0.1/Person>.
                        OPTIONAL
                        {{
                            ?s <http://purl.org/ontology/bibo/authorList> ?authorListL.
                            ?authorListL <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?personL.
                            ?personL <http://w3id.org/roh/gnossUser> ?idGnossL.
                        }}

                        
                        ?s <http://w3id.org/roh/title> ?title.
                        ?s <http://w3id.org/roh/isValidated> 'true'.

                        OPTIONAL
                        {{
                            ?s <http://purl.org/dc/terms/issued> ?issued.
                        }}

                        OPTIONAL
                        {{
                            ?s <http://purl.org/ontology/bibo/abstract> ?abstract.
                        }}

                        OPTIONAL
                        {{
                            ?s <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> ?type.
                        }}

                        OPTIONAL
                        {{
                            ?s <http://purl.org/dc/elements/1.1/type> ?roType.
                            ?roType <http://purl.org/dc/elements/1.1/title> ?roTypeTitle.
                            FILTER( lang(?roTypeTitle) = '{lang}' OR lang(?roTypeTitle) = '')
                        }}

                        OPTIONAL
                        {{
                            ?s <http://w3id.org/roh/hasKnowledgeArea> ?clHasKnowledgeArea.
                            ?clHasKnowledgeArea <http://w3id.org/roh/categoryNode> ?clKnowledgeArea.
                        }}


                        BIND ('false' as ?origin)

                    }}
                    UNION
                    {{
                        # ?personR a <http://xmlns.com/foaf/0.1/Person>.
                        # OPTIONAL
                        # {{
                        #    ?personR <http://w3id.org/roh/gnossUser> ?idGnossR.
                        #    ?resource <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?personR.
                        # }}

                        # Obtenemos los elementos relacionados desde el recurso dado
                        ?s ?related ?resource
                        Filter (?related in (<http://w3id.org/roh/linkedDocument>, <http://w3id.org/roh/linkedRO>))

                    
                        ?personL a <http://xmlns.com/foaf/0.1/Person>.
                        OPTIONAL
                        {{
                            ?s <http://purl.org/ontology/bibo/authorList> ?authorListL.
                            ?authorListL <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?personL.
                            ?personL <http://w3id.org/roh/gnossUser> ?idGnossL.
                        }}

                        ?s <http://w3id.org/roh/title> ?title.
                        ?s <http://w3id.org/roh/isValidated> 'true'.

                        OPTIONAL
                        {{
                            ?s <http://purl.org/dc/terms/issued> ?issued.
                        }}

                        OPTIONAL
                        {{
                            ?s <http://purl.org/ontology/bibo/abstract> ?abstract.
                        }}

                        OPTIONAL
                        {{
                            ?s <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> ?type.
                        }}

                        OPTIONAL
                        {{
                            ?s <http://purl.org/dc/elements/1.1/type> ?roType.
                            ?roType <http://purl.org/dc/elements/1.1/title> ?roTypeTitle.
                            FILTER( lang(?roTypeTitle) = '{lang}' OR lang(?roTypeTitle) = '')
                        }}

                        OPTIONAL
                        {{
                            ?s <http://w3id.org/roh/hasKnowledgeArea> ?clHasKnowledgeArea.
                            ?clHasKnowledgeArea <http://w3id.org/roh/categoryNode> ?clKnowledgeArea.
                        }}


                    }}

                    FILTER(?resource = <{idRecurso.ToString()}>)
                }} ORDER BY DESC(?type)";
            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, typeResource.type);

            // Rellena el los clusters
            sparqlObject.results.bindings.ForEach(e =>
            {

                // Busca si existe el cluster actualmente
                if (rosLinked.Find(cl => cl.entityID == e["s"].value) != null)
                {
                    // Añade el perfil al cluster que corresponde previamente cargado
                    //rosLinked.Find(cl => cl.entityID == e["cluster"].value).profiles.Add(profile);
                }
                else
                {
                    // Añade el ID en el listado de IDs
                    listIdslinked.Add(e["s"].value);







                    // Obtengo las areas de conocimiento del cluster
                    List<string> rOTerms = new List<string>();
                    if (e.ContainsKey("gckarea") && e["gckarea"].value != String.Empty)
                    {
                        rOTerms = e["gckarea"].value.Split(",").ToList();
                    }

                    // 1. Crea un nuevo cluter
                    // 2. Añade el perfil al cluster
                    // 3. Añade el cluster creado al listado de clusters
                    try
                    {

                        var fecha = e.ContainsKey("issued") ? e["issued"].value : String.Empty;
                        DateTime fechaDate = DateTime.Now;
                        try
                        {
                            fechaDate = DateTime.ParseExact(fecha, "yyyyMMddHHmmss", null);
                            fecha = fechaDate.ToString("dd/MM/yyyy");
                        }
                        catch (Exception ex)
                        {
                            mResourceApi.Log.Error("Excepcion: " + ex.Message);
                        }
                        ROLinked ro = new()
                        {
                            title = e.ContainsKey("title") ? e["title"].value : String.Empty,
                            entityID = e.ContainsKey("s") ? e["s"].value : String.Empty,
                            description = e.ContainsKey("abstract") ? e["abstract"].value : String.Empty,
                            roType = e.ContainsKey("roType") ? e["roType"].value : String.Empty,
                            roTypeTitle = e.ContainsKey("roTypeTitle") ? e["roTypeTitle"].value : String.Empty,
                            origin = e.ContainsKey("origin") ? false : true,
                            fecha = fechaDate,
                            terms = rOTerms
                        };

                        // Añade el cluster al listado
                        rosLinked.Add(ro);

                    }
                    catch (Exception ex)
                    {
                        mResourceApi.Log.Error("Excepcion: " + ex.Message);
                    }

                }

            });



            return rosLinked;
        }



        /// <summary>
        /// Método público para vincular un RO a otro RO (RO o publicación)
        /// Se usan las propiedades http://w3id.org/roh/linkedDocument o http://w3id.org/roh/linkedRO 
        /// dependiendo de si es un RO o una publicación el recurso que se vincula
        /// </summary>
        /// <param name="idRecurso">Id del RO en el que se va a hacer la vinculación</param>
        /// <param name="idLinkedRo">Id del RO a vincular</param>
        /// <param name="pIdGnossUser">Id del usuario que modifica el estado, necesario para actualizar el historial</param>
        /// <returns>String con el RO vinculado.</returns>
        internal bool AddLink(string idRecurso, string idLinkedRo, Guid pIdGnossUser)
        {

            Dictionary<Guid, bool> result = new();

            // Selecciono qué tipo de RO son los recursos pasados y obtengo las propiedades de la ontología
            ResTypeRo typeResource = GetTypeRo(idRecurso);
            ResTypeRo typeLinked = GetTypeRo(idLinkedRo);


            // Establezco la propiedad que se va a usar dependiendo de si es un documento que estoy relacionando o es un RO
            string predicateLinkInRO = string.Empty;
            if (typeLinked.typeRO == TypeRO.Document)
            {
                predicateLinkInRO = "http://w3id.org/roh/linkedDocument";
            }
            else
            {
                predicateLinkInRO = "http://w3id.org/roh/linkedRO";
            }


            // Obtengo el id del recurso si es un Guid
            Guid guid = Guid.Empty;
            Dictionary<Guid, string> longsId = new();
            if (Guid.TryParse(idRecurso, out guid))
            {
                longsId = UtilidadesAPI.GetLongIds(new List<Guid>() { guid }, mResourceApi, typeResource.longType, typeResource.type);
                idRecurso = longsId[guid];
            }
            else
            {
                guid = mResourceApi.GetShortGuid(idRecurso);
            }

            // Obtengo el id del recurso vinculado si es un Guid
            Guid guidLinked = Guid.Empty;
            longsId = new();
            if (Guid.TryParse(idLinkedRo, out guidLinked))
            {
                longsId = UtilidadesAPI.GetLongIds(new List<Guid>() { guidLinked }, mResourceApi, typeLinked.longType, typeLinked.type);
                idLinkedRo = longsId[guidLinked];
            }
            else
            {
                guidLinked = mResourceApi.GetShortGuid(idLinkedRo);
            }



            // Obtengo el id del recurso si es un Guid

            string LongpIdGnossUser = UtilidadesAPI.GetResearcherIdByGnossUser(mResourceApi, pIdGnossUser);
            


            // Modificar el estado y añadir un nuevo estado en el "historial"
            if (guid != Guid.Empty && guidLinked != Guid.Empty)
            {

                // Obtengo el recurso al que pretendo modificar el estado
                // Es necesario para las notificaciones y para comprobar si tengo o no permisos
                // Obtengo el recurso para conseguir el id del creador de la oferta

                //oferta = LoadOffer(idRecurso, false);

                // Compruebo si se tiene permisos para realizar la actualización de la oferta
                if (!CheckUpdateLink(LongpIdGnossUser, idRecurso, idLinkedRo))
                {
                    throw new Exception("Error al intentar modificar el estado, no tienes permiso para cambiar a este estado");
                }


                if (idRecurso != idLinkedRo)
                {
                    // Añadir cambio en el historial de la disponibilidad
                    // Comprueba si el id del recuro no está vacío
                    mResourceApi.ChangeOntoly(typeResource.type);

                    // Modifico el estado
                    try
                    {
                        Dictionary<Guid, List<TriplesToModify>> dicModificacion = new Dictionary<Guid, List<TriplesToModify>>();
                        List<TriplesToModify> listaTriplesModificacion = new List<TriplesToModify>();


                        // Modificación (Triples).
                        TriplesToModify triple = new TriplesToModify();
                        //triple.Predicate = "http://w3id.org/roh/linkedRO";
                        //triple.Predicate = "http://w3id.org/roh/linkedDocument";
                        triple.Predicate = predicateLinkInRO;
                        triple.NewValue = idLinkedRo;
                        //triple.OldValue = idLinkedRo;
                        listaTriplesModificacion.Add(triple);

                        // Modificación.
                        dicModificacion.Add(guid, listaTriplesModificacion);
                        result = mResourceApi.ModifyPropertiesLoadedResources(dicModificacion);
                    }
                    catch (Exception ex)
                    {
                        mResourceApi.Log.Error("Excepcion: " + ex.Message);
                    }
                }

            }

            return result[guid];
        }



        /// <summary>
        /// Método que comprueba si el usuario actual tiene permiso para realizar la modificación del recurso (Sea cambiar el estado, editar el recurso en si, o borrarlo) o no.
        /// </summary>
        /// <param name="longUserId">Id del usuario actual</param>
        /// <param name="idCurrentResource">Tipo de acción</param>
        /// <param name="idOtherResource">Permiso antigüo</param>
        /// <returns>Retorna un booleano indicando si puede o no ser actualizado.</returns>
        private bool CheckUpdateLink(string longUserId, string idCurrentResource, string idOtherResource)
        {

            return true;
        }

        /// <summary>
        /// Método que comprueba el tipo de recurso que es.
        /// </summary>
        /// <param name="idRecurso">Id del recurso</param>
        /// <returns>Retorna un enum TypeRO indicando el tipo de recurso.</returns>
        private ResTypeRo GetTypeRo(string idRecurso)
        {
            ResTypeRo resTypeRo = new();

            Guid guid = Guid.Empty;
            Dictionary<Guid, string> longsId = new();
            if (!Guid.TryParse(idRecurso, out guid))
            {
                guid = mResourceApi.GetShortGuid(idRecurso);
            }

            string select = $@"select distinct ?type ?longType";
            string where = $@"where {{
                ?s <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> ?type.
                ?s <http://gnoss/type> ?longType.
                Filter (?s = <http://gnoss/{guid.ToString().ToUpper()}>)
   
            }}";

            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);
            string type = string.Empty;
            string longType = string.Empty;
            sparqlObject.results.bindings.ForEach(e =>
            {
                try
                {
                    resTypeRo.type = e["type"].value;
                    resTypeRo.longType = e["longType"].value;

                    switch (resTypeRo.type)
                    {
                        case "document":
                            resTypeRo.typeRO = TypeRO.Document;
                            break;
                        case "researchobject":
                            resTypeRo.typeRO = TypeRO.RO;
                            break;
                    }

                }
                catch (Exception ex)
                {
                    mResourceApi.Log.Error("Excepcion: " + ex.Message);
                }
            });

            return resTypeRo;
        }
    }
}
