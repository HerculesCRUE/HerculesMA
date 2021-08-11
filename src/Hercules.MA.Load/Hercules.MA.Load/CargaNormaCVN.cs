using ContributiongradeprojectOntology;
using FeatureOntology;
using DedicationregimeOntology;
using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using Hercules.MA.Load.Models.CVN;
using ModalityOntology;
using ParticipationtypeOntology;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using static GnossBase.GnossOCBase;
using MotivationOntology;
using ContributiongradedocumentOntology;
using ReferencesourceOntology;
using ImpactindexcategoryOntology;
using LanguageOntology;
using PublicationtypeOntology;
using EventtypeOntology;
using GeographicregionOntology;
using OrganizationtypeOntology;
using SupporttypeOntology;

namespace Hercules.MA.Load
{
    /// <summary>
    /// Clase encargada de cargar los datos de las entidades secundarias de Hércules-MA.
    /// </summary>
    public class CargaNormaCVN
    {
        //Ruta con el XML de datos a leer.
        private static string RUTA_XML = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Dataset\CVN\ReferenceTables.xml";

        //Resource API.
        public static ResourceApi mResourceApi { get; set; }

        //Identificadores de las tablas.
        private static readonly string idPaises = "ISO_3166";
        private static readonly string idRegiones = "CVN_REGION";
        private static readonly string idProvincias = "CVN_PROVINCE";
        private static readonly string idParticipationType = "CVN_PARTICIPATION_A";
        private static readonly string idContributionGradeProject = "CVN_PARTICIPATION_B";
        private static readonly string idContributionGradeDocument = "CVN_PARTICIPATION_G";
        private static readonly string idModalidad = "CVN_PROJECT_C";
        private static readonly string idDedicationRegime = "CVN_DEDICATION_A";
        private static readonly string idMotivation = "CVN_SUPERVISION_A";
        private static readonly string idReferenceSource = "CVN_AGENCY_B";
        private static readonly string idImpactIndexCategory = "CVN_CATEGORY_A";
        private static readonly string idLanguage = "ISO_639";
        private static readonly string idPublicationType = "CVN_PUBLICATION_A";
        private static readonly string idEventType = "CVN_EVENT_B";
        private static readonly string idGeographicRegion = "CVN_SCOPE_A";
        private static readonly string idOrganizationType = "CVN_ENTITY_TYPE";
        private static readonly string idSupportType = "CVN_SUPPORT_B";

        /// <summary>
        /// Método para cargar las entidades secundarias.
        /// </summary>
        public static void CargarEntidadesSecundarias()
        {
            //Lectura del XML con los datos.
            XmlDocument documento = new XmlDocument();
            documento.Load(RUTA_XML);
            XmlSerializer serializer = new XmlSerializer(typeof(ReferenceTables));
            ReferenceTables tablas = (ReferenceTables)serializer.Deserialize(new StringReader(documento.InnerXml));

            //Carga de entidades secundarias.
            CargarFeatures(tablas, "feature");
            CargarModality(tablas, "modality");
            CargarContributionGradeProject(tablas, "contributiongradeproject");
            CargarParticipationType(tablas, "participationtype");
            CargarDedicationRegime(tablas, "dedicationregime");
            CargarMotivation(tablas, "motivation");
            CargarContributionGradeDocument(tablas, "contributiongradedocument");
            CargarReferenceSource(tablas, "referencesource");
            CargarImpactIndexCategory(tablas, "impactindexcategory");
            CargarLanguage(tablas, "language");
            CargarPublicationType(tablas, "publicationtype");
            CargarEventType(tablas, "eventtype");
            CargarGeographicRegion(tablas, "geographicregion");
            CargarOrganizationType(tablas, "organizationtype");
            CargarSupportType(tablas, "supporttype");
        }

        /// <summary>
        /// Carga la entidad secundaria Feature.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarFeatures(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://www.geonames.org/ontology#Feature", pOntology);

            //Obtención de los objetos a cargar.
            List<Feature> features = new List<Feature>();
            features = ObtenerDatosFeature(pTablas, idPaises, "PCLD", features, pOntology);
            features = ObtenerDatosFeature(pTablas, idRegiones, "ADM1", features, pOntology);
            features = ObtenerDatosFeature(pTablas, idProvincias, "ADM2", features, pOntology);

            //Carga.
            foreach (Feature feature in features)
            {
                mResourceApi.LoadSecondaryResource(feature.ToGnossApiResource(mResourceApi, pOntology + "_" + feature.Dc_identifier));
            }
        }

        /// <summary>
        /// Obtiene los objetos Feature a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pId">Número que se le agregará al ID creado.</param>
        /// <param name="pListaFeatures">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<Feature> ObtenerDatosFeature(ReferenceTables pTablas, string pCodigoTabla, string pId, List<Feature> pListaFeatures, string pOntology)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {

                    Feature feature = new Feature();
                    Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                    string identificador = $@"{pId}_{item.Code}";
                    foreach (TableItemNameDetail pais in item.Name)
                    {
                        LanguageEnum idioma = dicIdiomasMapeados[pais.lang];
                        string nombre = pais.Name;
                        dicIdioma.Add(idioma, nombre);
                    }

                    //Se agrega las propiedades.
                    feature.Dc_identifier = identificador;
                    feature.Gn_name = dicIdioma;
                    feature.Gn_featureCode = pCodigoTabla;

                    //Se comprueba que no haya ningún código padre que sea null y que no sea de país.
                    string codigoPadre = item.AntecesorCode;
                    if (string.IsNullOrEmpty(codigoPadre))
                    {
                        codigoPadre = null;
                        if (pId != "PCLD")
                        {
                            continue;
                        }
                    }

                    switch (pId)
                    {
                        case "ADM1":
                            //Contruyo el ID del padre, debido al error que hay en los datos.
                            if (!string.IsNullOrEmpty(codigoPadre) && Int32.Parse(codigoPadre) < 10)
                            {
                                codigoPadre = "00" + codigoPadre;
                            }
                            else if (!string.IsNullOrEmpty(codigoPadre) && Int32.Parse(codigoPadre) < 100)
                            {
                                codigoPadre = "0" + codigoPadre;
                            }

                            feature.IdGn_parentFeature = $@"{pOntology}_PCLD_{codigoPadre}";
                            feature.Gn_parentFeature = pListaFeatures.First(x => x.Dc_identifier == $@"PCLD_{codigoPadre}");
                            break;
                        case "ADM2":
                            feature.IdGn_parentFeature = $@"{pOntology}_ADM1_{codigoPadre}";
                            feature.Gn_parentFeature = pListaFeatures.First(x => x.Dc_identifier == $@"ADM1_{codigoPadre}");
                            break;
                    }

                    //Se guarda el objeto a la lista.
                    if (pCodigoTabla == idPaises && feature.Dc_identifier.Split('_')[1].Length == 3)
                    {
                        pListaFeatures.Add(feature);
                    }
                    else if (string.IsNullOrEmpty(item.Delegate) && (pCodigoTabla == idRegiones || pCodigoTabla == idProvincias))
                    {
                        pListaFeatures.Add(feature);
                    }
                }
            }

            return pListaFeatures;
        }

        /// <summary>
        /// Carga la entidad secundaria Modality.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarModality(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/Modality", pOntology);

            //Obtención de los objetos a cargar.
            List<Modality> modalities = new List<Modality>();
            modalities = ObtenerDatosModality(pTablas, idModalidad, modalities);

            //Carga.
            foreach (Modality modality in modalities)
            {
                mResourceApi.LoadSecondaryResource(modality.ToGnossApiResource(mResourceApi, pOntology + "_" + modality.Dc_identifier));
            }
        }

        /// <summary>
        /// Obtiene los objetos Modality a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaModality">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<Modality> ObtenerDatosModality(ReferenceTables pTablas, string pCodigoTabla, List<Modality> pListaModality)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        Modality modality = new Modality();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail modalidad in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[modalidad.lang];
                            string nombre = modalidad.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        modality.Dc_identifier = identificador;
                        modality.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaModality.Add(modality);
                    }
                }
            }

            return pListaModality;
        }

        /// <summary>
        /// Carga la entidad secundaria ContributionGradeProject.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarContributionGradeProject(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/ContributionGradeProject", pOntology);

            //Obtención de los objetos a cargar.
            List<ContributionGradeProject> contributions = new List<ContributionGradeProject>();
            contributions = ObtenerDatosContributionGradeProject(pTablas, idContributionGradeProject, contributions);

            //Carga.
            foreach (ContributionGradeProject contribution in contributions)
            {
                mResourceApi.LoadSecondaryResource(contribution.ToGnossApiResource(mResourceApi, pOntology + "_" + contribution.Dc_identifier));
            }
        }

        /// <summary>
        /// Obtiene los objetos ContributionGradeProject a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaContributionGrade">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ContributionGradeProject> ObtenerDatosContributionGradeProject(ReferenceTables pTablas, string pCodigoTabla, List<ContributionGradeProject> pListaContributionGrade)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        ContributionGradeProject contribution = new ContributionGradeProject();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail contribucion in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[contribucion.lang];
                            string nombre = contribucion.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        contribution.Dc_identifier = identificador;
                        contribution.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaContributionGrade.Add(contribution);
                    }
                }
            }

            return pListaContributionGrade;
        }

        /// <summary>
        /// Carga la entidad secundaria ParticipationType.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarParticipationType(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/ParticipationType", pOntology);

            //Obtención de los objetos a cargar.
            List<ParticipationType> participations = new List<ParticipationType>();
            participations = ObtenerDatosParticipationType(pTablas, idParticipationType, participations);

            //Carga.
            foreach (ParticipationType participation in participations)
            {
                mResourceApi.LoadSecondaryResource(participation.ToGnossApiResource(mResourceApi, pOntology + "_" + participation.Dc_identifier));
            }
        }

        /// <summary>
        /// Obtiene los objetos ParticipationType a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaParticipationType">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ParticipationType> ObtenerDatosParticipationType(ReferenceTables pTablas, string pCodigoTabla, List<ParticipationType> pListaParticipationType)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        ParticipationType participation = new ParticipationType();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail participacion in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[participacion.lang];
                            string nombre = participacion.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        participation.Dc_identifier = identificador;
                        participation.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaParticipationType.Add(participation);
                    }
                }
            }

            return pListaParticipationType;
        }

        /// <summary>
        /// Carga la entidad secundaria DedicationRegime.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarDedicationRegime(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/DedicationRegime", pOntology);

            //Obtención de los objetos a cargar.
            List<DedicationRegime> dedications = new List<DedicationRegime>();
            dedications = ObtenerDatosDedicationRegime(pTablas, idDedicationRegime, dedications);

            //Carga.
            foreach (DedicationRegime dedication in dedications)
            {
                mResourceApi.LoadSecondaryResource(dedication.ToGnossApiResource(mResourceApi, pOntology + "_" + dedication.Dc_identifier));
            }
        }

        /// <summary>
        /// Obtiene los objetos DedicationRegime a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaDedicationRegime">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<DedicationRegime> ObtenerDatosDedicationRegime(ReferenceTables pTablas, string pCodigoTabla, List<DedicationRegime> pListaDedicationRegime)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        DedicationRegime dedication = new DedicationRegime();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail dedicacion in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[dedicacion.lang];
                            string nombre = dedicacion.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        dedication.Dc_identifier = identificador;
                        dedication.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaDedicationRegime.Add(dedication);
                    }
                }
            }

            return pListaDedicationRegime;
        }

        /// <summary>
        /// Carga la entidad secundaria Motivation.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarMotivation(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/Motivation", pOntology);

            //Obtención de los objetos a cargar.
            List<Motivation> motivations = new List<Motivation>();
            motivations = ObtenerDatosMotivation(pTablas, idMotivation, motivations);

            //Carga.
            foreach (Motivation motivation in motivations)
            {
                mResourceApi.LoadSecondaryResource(motivation.ToGnossApiResource(mResourceApi, pOntology + "_" + motivation.Dc_identifier));
            }
        }

        /// <summary>
        /// Obtiene los objetos Motivation a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaMotivation">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<Motivation> ObtenerDatosMotivation(ReferenceTables pTablas, string pCodigoTabla, List<Motivation> pListaMotivation)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        Motivation motivation = new Motivation();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail motivacion in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[motivacion.lang];
                            string nombre = motivacion.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        motivation.Dc_identifier = identificador;
                        motivation.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaMotivation.Add(motivation);
                    }
                }
            }

            return pListaMotivation;
        }

        /// <summary>
        /// Carga la entidad secundaria ContributionGradeDocument.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarContributionGradeDocument(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/ContributionGradeDocument", pOntology);

            //Obtención de los objetos a cargar.
            List<ContributionGradeDocument> contributions = new List<ContributionGradeDocument>();
            contributions = ObtenerDatosContributionGradeDocument(pTablas, idContributionGradeDocument, contributions);

            //Carga.
            foreach (ContributionGradeDocument contribution in contributions)
            {
                mResourceApi.LoadSecondaryResource(contribution.ToGnossApiResource(mResourceApi, pOntology + "_" + contribution.Dc_identifier));
            }
        }

        /// <summary>
        /// Obtiene los objetos ContributionGradeDocument a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaContributionGrade">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ContributionGradeDocument> ObtenerDatosContributionGradeDocument(ReferenceTables pTablas, string pCodigoTabla, List<ContributionGradeDocument> pListaContributionGrade)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        ContributionGradeDocument contribution = new ContributionGradeDocument();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail contribucion in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[contribucion.lang];
                            string nombre = contribucion.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        contribution.Dc_identifier = identificador;
                        contribution.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaContributionGrade.Add(contribution);
                    }
                }
            }

            return pListaContributionGrade;
        }

        /// <summary>
        /// Carga la entidad secundaria ReferenceSource.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarReferenceSource(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://purl.org/ontology/bibo/ReferenceSource", pOntology);

            //Obtención de los objetos a cargar.
            List<ReferenceSource> references = new List<ReferenceSource>();
            references = ObtenerDatosReferenceSource(pTablas, idReferenceSource, references);

            //Carga.
            foreach (ReferenceSource reference in references)
            {
                mResourceApi.LoadSecondaryResource(reference.ToGnossApiResource(mResourceApi, pOntology + "_" + reference.Dc_identifier));
            }
        }

        /// <summary>
        /// Obtiene los objetos ReferenceSource a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaReferenceSource">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ReferenceSource> ObtenerDatosReferenceSource(ReferenceTables pTablas, string pCodigoTabla, List<ReferenceSource> pListaReferenceSource)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        ReferenceSource reference = new ReferenceSource();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail referencia in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[referencia.lang];
                            string nombre = referencia.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        reference.Dc_identifier = identificador;
                        reference.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaReferenceSource.Add(reference);
                    }
                }
            }

            return pListaReferenceSource;
        }

        /// <summary>
        /// Carga la entidad secundaria ImpactIndexCategory.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarImpactIndexCategory(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/ImpactIndexCategory", pOntology);

            //Obtención de los objetos a cargar.
            List<ImpactIndexCategory> categorias = new List<ImpactIndexCategory>();
            categorias = ObtenerDatosImpactIndexCategory(pTablas, idImpactIndexCategory, categorias);

            //Carga.
            foreach (ImpactIndexCategory category in categorias)
            {
                mResourceApi.LoadSecondaryResource(category.ToGnossApiResource(mResourceApi, pOntology + "_" + category.Dc_identifier));
            }
        }

        /// <summary>
        /// Obtiene los objetos ImpactIndexCategory a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaImpactIndexCategory">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ImpactIndexCategory> ObtenerDatosImpactIndexCategory(ReferenceTables pTablas, string pCodigoTabla, List<ImpactIndexCategory> pListaImpactIndexCategory)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        ImpactIndexCategory categories = new ImpactIndexCategory();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail categoria in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[categoria.lang];
                            string nombre = categoria.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        categories.Dc_identifier = identificador;
                        categories.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaImpactIndexCategory.Add(categories);
                    }
                }
            }

            return pListaImpactIndexCategory;
        }

        /// <summary>
        /// Carga la entidad secundaria Language.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarLanguage(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/Language", pOntology);

            //Obtención de los objetos a cargar.
            List<Language> lenguajes = new List<Language>();
            lenguajes = ObtenerDatosLanguage(pTablas, idLanguage, lenguajes);

            //Carga.
            foreach (Language language in lenguajes)
            {
                mResourceApi.LoadSecondaryResource(language.ToGnossApiResource(mResourceApi, pOntology + "_" + language.Dc_identifier));
            }
        }

        /// <summary>
        /// Obtiene los objetos Language a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaLanguage">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<Language> ObtenerDatosLanguage(ReferenceTables pTablas, string pCodigoTabla, List<Language> pListaLanguage)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        Language language = new Language();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail lenguaje in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[lenguaje.lang];
                            string nombre = lenguaje.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        language.Dc_identifier = identificador;
                        language.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaLanguage.Add(language);
                    }
                }
            }

            return pListaLanguage;
        }

        /// <summary>
        /// Carga la entidad secundaria PublicationType.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarPublicationType(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/PublicationType", pOntology);

            //Obtención de los objetos a cargar.
            List<PublicationType> publicaciones = new List<PublicationType>();
            publicaciones = ObtenerDatosPublicationType(pTablas, idPublicationType, publicaciones);

            //Carga.
            foreach (PublicationType publication in publicaciones)
            {
                mResourceApi.LoadSecondaryResource(publication.ToGnossApiResource(mResourceApi, pOntology + "_" + publication.Dc_identifier));
            }
        }

        /// <summary>
        /// Obtiene los objetos Language a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaPublicationType">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<PublicationType> ObtenerDatosPublicationType(ReferenceTables pTablas, string pCodigoTabla, List<PublicationType> pListaPublicationType)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        PublicationType publication = new PublicationType();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail publicacion in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[publicacion.lang];
                            string nombre = publicacion.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        publication.Dc_identifier = identificador;
                        publication.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaPublicationType.Add(publication);
                    }
                }
            }

            return pListaPublicationType;
        }

        /// <summary>
        /// Carga la entidad secundaria EventType.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarEventType(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/EventType", pOntology);

            //Obtención de los objetos a cargar.
            List<EventType> eventos = new List<EventType>();
            eventos = ObtenerDatosEventType(pTablas, idEventType, eventos);

            //Carga.
            foreach (EventType eventType in eventos)
            {
                mResourceApi.LoadSecondaryResource(eventType.ToGnossApiResource(mResourceApi, pOntology + "_" + eventType.Dc_identifier));
            }
        }

        /// <summary>
        /// Obtiene los objetos EventType a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaEventType">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<EventType> ObtenerDatosEventType(ReferenceTables pTablas, string pCodigoTabla, List<EventType> pListaEventType)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        EventType eventType = new EventType();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail evento in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[evento.lang];
                            string nombre = evento.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        eventType.Dc_identifier = identificador;
                        eventType.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaEventType.Add(eventType);
                    }
                }
            }

            return pListaEventType;
        }

        /// <summary>
        /// Carga la entidad secundaria GeographicRegion.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarGeographicRegion(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/GeographicRegion", pOntology);

            //Obtención de los objetos a cargar.
            List<GeographicRegion> regiones = new List<GeographicRegion>();
            regiones = ObtenerDatosGeographicRegion(pTablas, idGeographicRegion, regiones);

            //Carga.
            foreach (GeographicRegion region in regiones)
            {
                mResourceApi.LoadSecondaryResource(region.ToGnossApiResource(mResourceApi, pOntology + "_" + region.Dc_identifier));
            }
        }

        /// <summary>
        /// Obtiene los objetos GeographicRegion a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaGeographicRegion">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<GeographicRegion> ObtenerDatosGeographicRegion(ReferenceTables pTablas, string pCodigoTabla, List<GeographicRegion> pListaGeographicRegion)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        GeographicRegion geographicRegion = new GeographicRegion();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail region in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[region.lang];
                            string nombre = region.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        geographicRegion.Dc_identifier = identificador;
                        geographicRegion.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaGeographicRegion.Add(geographicRegion);
                    }
                }
            }

            return pListaGeographicRegion;
        }

        /// <summary>
        /// Carga la entidad secundaria OrganizationType.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarOrganizationType(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/OrganizationType", pOntology);

            //Obtención de los objetos a cargar.
            List<OrganizationType> organizaciones = new List<OrganizationType>();
            organizaciones = ObtenerDatosOrganizationType(pTablas, idOrganizationType, organizaciones);

            //Carga.
            foreach (OrganizationType organization in organizaciones)
            {
                mResourceApi.LoadSecondaryResource(organization.ToGnossApiResource(mResourceApi, pOntology + "_" + organization.Dc_identifier));
            }
        }


        /// <summary>
        /// Obtiene los objetos OrganizationType a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaOrganizationType">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<OrganizationType> ObtenerDatosOrganizationType(ReferenceTables pTablas, string pCodigoTabla, List<OrganizationType> pListaOrganizationType)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        OrganizationType organization = new OrganizationType();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail organizacion in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[organizacion.lang];
                            string nombre = organizacion.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        organization.Dc_identifier = identificador;
                        organization.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaOrganizationType.Add(organization);
                    }
                }
            }

            return pListaOrganizationType;
        }

        /// <summary>
        /// Carga la entidad secundaria SupportType.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarSupportType(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/SupportType", pOntology);

            //Obtención de los objetos a cargar.
            List<SupportType> publicaciones = new List<SupportType>();
            publicaciones = ObtenerDatosSupportType(pTablas, idSupportType, publicaciones);

            //Carga.
            foreach (SupportType publication in publicaciones)
            {
                mResourceApi.LoadSecondaryResource(publication.ToGnossApiResource(mResourceApi, pOntology + "_" + publication.Dc_identifier));
            }
        }

        /// <summary>
        /// Obtiene los objetos Language a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaSupportType">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<SupportType> ObtenerDatosSupportType(ReferenceTables pTablas, string pCodigoTabla, List<SupportType> pListaSupportType)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        SupportType publication = new SupportType();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail publicacion in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[publicacion.lang];
                            string nombre = publicacion.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        publication.Dc_identifier = identificador;
                        publication.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaSupportType.Add(publication);
                    }
                }
            }

            return pListaSupportType;
        }


        /// <summary>
        /// Elimina los datos del grafo.
        /// </summary>
        /// <param name="pRdfType">RdfType del recurso a borrar.</param>
        /// <param name="pOntology">Ontología a consultar.</param>
        /// <param name="pSource">Source para los tesauros semánticos</param>
        public static void EliminarDatosCargados(string pRdfType, string pOntology, string pSource = "")
        {
            //Consulta.
            string select = string.Empty, where = string.Empty;
            select += $@"SELECT ?s ";
            where += $@"WHERE {{ ";
            where += $@"?s a <{pRdfType}>. ";
            if (!string.IsNullOrEmpty(pSource))
            {
                where += "?s <http://purl.org/dc/elements/1.1/source> '" + pSource + "'";
            }
            where += $@"}} ";

            //Obtiene las URLs de los recursos a borrar.
            List<string> listaUrlSecundarias = new List<string>();
            SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, pOntology);

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    listaUrlSecundarias.Add(GetValorFilaSparqlObject(fila, "s"));
                }
            }

            //Borra los recursos.
            mResourceApi.DeleteSecondaryEntitiesList(ref listaUrlSecundarias);
        }

        /// <summary>
        /// Obtiene el valor de de las filas de la consulta.
        /// </summary>
        /// <param name="pFila">Fila con el resultado.</param>
        /// <param name="pParametro">Parametro a obtener.</param>
        /// <returns>Dato guardado.</returns>
        public static string GetValorFilaSparqlObject(Dictionary<string, SparqlObject.Data> pFila, string pParametro)
        {
            if (pFila.ContainsKey(pParametro) && !string.IsNullOrEmpty(pFila[pParametro].value))
            {
                return pFila[pParametro].value;
            }
            return null;
        }

        /// <summary>
        /// Permite mapear el idioma obtenido del XML al idioma de las clases generadas.
        /// </summary>
        /// <returns>Diccionario con la conversión de los idiomas.</returns>
        private static Dictionary<string, LanguageEnum> MapearLenguajes()
        {
            Dictionary<string, LanguageEnum> dicIdiomas = new Dictionary<string, LanguageEnum>();
            dicIdiomas.Add("spa", LanguageEnum.es);
            dicIdiomas.Add("eng", LanguageEnum.en);
            dicIdiomas.Add("cat", LanguageEnum.ca);
            dicIdiomas.Add("eus", LanguageEnum.eu);
            dicIdiomas.Add("glg", LanguageEnum.gl);
            dicIdiomas.Add("fra", LanguageEnum.fr);
            return dicIdiomas;
        }
    }
}
