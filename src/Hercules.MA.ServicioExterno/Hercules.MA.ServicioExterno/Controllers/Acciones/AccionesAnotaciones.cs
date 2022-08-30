using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hercules.MA.ServicioExterno.Controllers.Utilidades;
using AnnotationOntology;

namespace Hercules.MA.ServicioExterno.Controllers.Acciones
{
    public class AccionesAnotaciones
    {

        #region --- Constantes     
        private static string RUTA_OAUTH = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config/ConfigOAuth/OAuthV3.config";
        private static ResourceApi mResourceApi = new ResourceApi(RUTA_OAUTH);
        private static CommunityApi mCommunityApi = new CommunityApi(RUTA_OAUTH);
        private static Guid mIdComunidad = mCommunityApi.GetCommunityId();
        private static string RUTA_PREFIJOS = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Models/JSON/prefijos.json";
        private static string mPrefijos = string.Join(" ", JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(RUTA_PREFIJOS)));
        private static string COLOR_GRAFICAS = "#6cafe3";
        private static string COLOR_GRAFICAS_HORIZONTAL = "#6cafe3"; //#1177ff
        #endregion

        public List<Dictionary<string, string>> GetOwnAnnotationsInRO(string idRO, string idUser, string rdfType, string ontology)
        {

            List<Dictionary<string, string>> typesRO = new();

            // "http://purl.org/ontology/bibo/Document", "document"
            // "http://w3id.org/roh/ResearchObject", "researchobject"
            //typesRO.Add("document", "http://purl.org/ontology/bibo/Document");
            //typesRO.Add("researchobject", "http://w3id.org/roh/ResearchObject");


            // Obtengo el id del RO si es Guid
            Guid guidRO = Guid.Empty;
            Dictionary<Guid, string> longsIdRO = new();
            if (Guid.TryParse(idRO, out guidRO))
            {
                longsIdRO = UtilidadesAPI.GetLongIds(new List<Guid>() { guidRO }, mResourceApi, rdfType, ontology);
                idRO = longsIdRO[guidRO];
            }
            else
            {
                guidRO = mResourceApi.GetShortGuid(idRO);
            }


            // Obtengo el id del usuario si es Guid
            Guid guidUser = Guid.Empty;
            Dictionary<Guid, string> longsIdUs = new();
            if (!Guid.TryParse(idUser, out guidUser))
            {
                guidUser = mResourceApi.GetShortGuid(idUser);
            }


            // Obtener el id del usuario usando el id de la cuenta
            string userGnossId = UtilidadesAPI.GetResearcherIdByGnossUser(mResourceApi, guidUser);




            // Obtenemos todos los datos de los perfiles y Añadimos el perfil creado a los datos de la oferta
            string select = "select distinct ?s ?date ?texto FROM <http://gnoss.com/annotation.owl>";

            //string filterRO = "";
            //switch (ontology)
            //{
            //    case "document":
            //        filterRO = @$"
            //            ?s <http://w3id.org/roh/document> ?document.
            //            FILTER(?document = <{idRO}>)) ";
            //        break;

            //    case "researchobject":
            //        filterRO = @$"
            //            ?s <http://w3id.org/roh/researchobject> ?ro.
            //            FILTER(?ro = <{idRO}>)) ";
            //        break;
            //}

            string where = @$"where {{

                ?s a <http://w3id.org/roh/Annotation>.
                ?s <http://w3id.org/roh/text> ?texto.
                ?s <http://w3id.org/roh/dateIssued> ?date.

                # Filtra por el RO
                ?s ?roRfType ?ro.
                FILTER (?roRfType in (<http://w3id.org/roh/document>, <http://w3id.org/roh/researchobject>))
                FILTER(?ro = <{idRO}>)

                # Filtra por el usuario actual 
                ?s <http://w3id.org/roh/owner> ?user.
                FILTER(?user = <{userGnossId}>)
            }} ORDER BY DESC(?date)";

            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "person");

            // Carga los datos en el objeto
            sparqlObject.results.bindings.ForEach(e =>
            {
                try
                {
                    // Cargamos las variables
                    var id = e["s"].value;
                    var texto = e["texto"].value;

                    // Cargamos la fecha
                    DateTime fechaDate = DateTime.UtcNow;
                    var fecha = DateTime.UtcNow.ToString("g");
                    try
                    {
                        fechaDate = DateTime.ParseExact(e["date"].value, "yyyyMMddHHmmss", null);
                        fecha = fechaDate.ToString("g");
                    }
                    catch (Exception ex)
                    {
                        mResourceApi.Log.Error("Excepcion: " + ex.Message);
                    }

                    // Creamos el diccionario
                    Dictionary<string, string> longs = new Dictionary<string, string>();
                    longs.Add("id", id);
                    longs.Add("fecha", fecha);
                    longs.Add("texto", texto);

                    // Añadimos el diccionario al listado
                    typesRO.Add(longs);

                }
                catch (Exception ext) { new Exception("Ha habido un error al procesar los datos de los usuarios:" + ext.Message); }

            });



            return typesRO;
        }


        public string CreateNewAnnotation( string idRO, string idUser, string rdfType, string ontology, string texto, string idAnnotation = null)
        {

            // Obtengo el id del RO si es Guid
            Guid guidRO = Guid.Empty;
            Dictionary<Guid, string> longsIdRO = new();
            if (Guid.TryParse(idRO, out guidRO))
            {
                longsIdRO = UtilidadesAPI.GetLongIds(new List<Guid>() { guidRO }, mResourceApi, rdfType, ontology);
                idRO = longsIdRO[guidRO];
            }
            else
            {
                guidRO = mResourceApi.GetShortGuid(idRO);
            }


            // Obtengo el id del usuario si es Guid
            Guid guidUser = Guid.Empty;
            Dictionary<Guid, string> longsIdUs = new();
            if (!Guid.TryParse(idUser, out guidUser))
            {
                guidUser = mResourceApi.GetShortGuid(idUser);
            }


            // Obtener el id del usuario usando el id de la cuenta
            string userGnossId = UtilidadesAPI.GetResearcherIdByGnossUser(mResourceApi, guidUser);




            int MAX_INTENTOS = 10;
            bool uploadedR = false;


            if (!string.IsNullOrEmpty(userGnossId))
            {

                // creando los cluster
                Annotation cRsource = new();

                // Usuario creador
                cRsource.IdRoh_owner = userGnossId;

                // Otros campos
                cRsource.Roh_dateIssued = DateTime.UtcNow;

                cRsource.Roh_title = "-";

                // Sección de las descripciones, limpiamos los strings de tags que no queramos
                cRsource.Roh_text = texto != null ? CleanHTML.StripTagsCharArray(texto.Replace("&", "&amp").Replace("<", "&lt").Replace(">", "&gt").Replace("\"", "&quot").Replace("\'", "&apos"), new string[] { }, new string[] { }) :"" ;// != null ? CleanHTML.StripTagsCharArray(texto, new string[] {}, new string[] {}) : "";

                // Comprobamos si es un documento u otro RO cualquiera
                cRsource.IdsRoh_researchobject = new();
                cRsource.IdsRoh_document =  new();

                switch (ontology)
                {
                    case "document":
                        cRsource.IdsRoh_document = new List<string>() { idRO };
                        break;
                    case "researchobject":
                        cRsource.IdsRoh_researchobject = new List<string>() { idRO };
                        break;
                }


                // Guardando o actualizando el recurso
                mResourceApi.ChangeOntoly("annotation");
                // Comprueba si es una actualización o no
                if (idAnnotation != null)
                {

                    string[] recursoSplit = idAnnotation.Split('_');

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
                        idAnnotation = mResourceApi.LoadComplexSemanticResource(resource, false, true);
                        uploadedR = resource.Uploaded;
                    }
                }
            }

            if (uploadedR)
            {
                return idAnnotation;
            }
            else
            {
                throw new Exception("Recurso no creado");
            }


        }

        public bool DeleteAnnotation(string idAnnotation)
        {
            // Obtengo el id del RO si es Guid
            Guid guidAnnotation;
            guidAnnotation = mResourceApi.GetShortGuid(idAnnotation);
            return mResourceApi.PersistentDelete(guidAnnotation);
        }
    }

}
