﻿using ContributiongradeprojectOntology;
using FeatureOntology;
using DedicationregimeOntology;
using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using Hercules.MA.Load.Models.CVN;
using ProjectmodalityOntology;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using static GnossBase.GnossOCBase;
using EventinscriptiontypeOntology;
using ContributiongradedocumentOntology;
using ReferencesourceOntology;
using ImpactindexcategoryOntology;
using LanguageOntology;
using PublicationtypeOntology;
using EventtypeOntology;
using GeographicregionOntology;
using OrganizationtypeOntology;
using DocumentformatOntology;
using GenderOntology;
using ParticipationtypeprojectOntology;
using ProjecttypeOntology;
using ParticipationtypedocumentOntology;
using IndustrialpropertytypeOntology;
using ColaborationtypegroupOntology;
using ManagementtypeactivityOntology;
using TargetgroupprofileOntology;
using AccesssystemactivityOntology;
using ParticipationtypeactivityOntology;
using StaygoalOntology;
using GrantaimOntology;
using RelationshiptypeOntology;
using ScientificactivitydocumentOntology;
using DepartmentOntology;
using Hercules.MA.Load.Models.UMU;
using ScientificexperienceprojectOntology;
using ActivitymodalityOntology;
using TaxonomyOntology;
using Hercules.MA.Load.Models.TaxonomyOntology;
using ResearchobjecttypeOntology;
using ContractmodalityOntology;
using ScopemanagementactivityOntology;

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
        private static readonly string idParticipationTypeProject = "CVN_PARTICIPATION_A";
        private static readonly string idContributionGradeProject = "CVN_PARTICIPATION_B";
        private static readonly string idContributionGradeDocument = "CVN_PARTICIPATION_G";
        private static readonly string idProjectModality = "CVN_PROJECT_C";
        private static readonly string idDedicationRegime = "CVN_DEDICATION_A";
        private static readonly string idEventInscriptionType = "CVN_SUPERVISION_A";
        private static readonly string idReferenceSource = "CVN_AGENCY_B";
        private static readonly string idImpactIndexCategory = "CVN_CATEGORY_A";
        private static readonly string idLanguage = "ISO_639";
        private static readonly string idPublicationType = "CVN_PUBLICATION_A";
        private static readonly string idEventType = "CVN_EVENT_B";
        private static readonly string idGeographicRegion = "CVN_SCOPE_A";
        private static readonly string idOrganizationType = "CVN_ENTITY_TYPE";
        private static readonly string idDocumentFormat = "CVN_SUPPORT_B";
        private static readonly string idGender = "CVN_SEX_A";
        private static readonly string idProjectType = "CVN_PARTICIPATION_F";
        private static readonly string idParticipationTypeDocument = "CVN_PARTICIPATION_E";
        private static readonly string idIndustrialPropertyType = "CVN_KNOW_A";
        private static readonly string idColaborationTypeGroup = "CVN_COOPERANTION_A";
        private static readonly string idPublicationIdentifierType = "CVN_SOURCE_B";
        private static readonly string idManagementTypeActivity = "CVN_MANAGEMENT_A";
        private static readonly string idTargetGroupProfile = "CVN_AGENCY_A";
        private static readonly string idAccessSystemActivity = "CVN_ACCESS_A";
        private static readonly string idParticipationTypeActivity = "CVN_PARTICIPATION_C";
        private static readonly string idStayGoal = "CVN_STAY_A";
        private static readonly string idGrantAim = "CVN_SUMMONS_A";
        private static readonly string idRelationshipType = "CVN_COLLABORATION_A";
        private static readonly string idActivityModality = "CVN_ACTIVITY_A";
        private static readonly string idContractModality = "CVN_SITUATION_A";
        private static readonly string idScopeManagementActivity = "CVN_MANAGEMENT_TYPE_A";
        

        //Número de hilos para el paralelismo.
        public static int NUM_HILOS = 6;

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
            CargarProjectModality(tablas, "projectmodality");
            CargarContributionGradeProject(tablas, "contributiongradeproject");
            CargarParticipationTypeProject(tablas, "participationtypeproject");
            CargarDedicationRegime(tablas, "dedicationregime");
            CargarEventInscriptionType(tablas, "eventinscriptiontype");
            CargarContributionGradeDocument(tablas, "contributiongradedocument");
            CargarReferenceSource(tablas, "referencesource");
            CargarImpactIndexCategory(tablas, "impactindexcategory");
            CargarLanguage(tablas, "language");
            CargarPublicationType(tablas, "publicationtype");
            CargarEventType(tablas, "eventtype");
            CargarGeographicRegion(tablas, "geographicregion");
            CargarOrganizationType(tablas, "organizationtype");
            CargarDocumentFormat(tablas, "documentformat");
            CargarGender(tablas, "gender");
            CargarProjectType(tablas, "projecttype");
            CargarParticipationTypeDocument(tablas, "participationtypedocument");
            CargarIndustrialPropertyType(tablas, "industrialpropertytype");
            CargarColaborationTypeGroup(tablas, "colaborationtypegroup");
            CargarManagementTypeActivity(tablas, "managementtypeactivity");
            CargarTargetGroupProfile(tablas, "targetgroupprofile");
            CargarAccessSystemActivity(tablas, "accesssystemactivity");
            CargarParticipationTypeActivity(tablas, "participationtypeactivity");
            CargarStayGoal(tablas, "staygoal");
            CargarGrantAim(tablas, "grantaim");
            CargarRelationshipType(tablas, "relationshiptype");
            CargarScientificActivityDocument("scientificactivitydocument");
            CargarScientificExperienceProject("scientificexperienceproject");
            CargarDepartment("department");
            CargarActivityModality(tablas, "activitymodality");
            CargarContractModality(tablas, "contractmodality");
            CargarScopeManagementActivity(tablas, "scopemanagementactivity");
            CargarTesauroUnesco(tablas, "taxonomy");

            //Cargamos los subtipos de los RO
            CargarResearhObjectType();
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
            List<FeatureOntology.Feature> features = new List<FeatureOntology.Feature>();
            features = ObtenerDatosFeature(pTablas, idPaises, "PCLD", features, pOntology);
            features = ObtenerDatosFeature(pTablas, idRegiones, "ADM1", features, pOntology);
            features = ObtenerDatosFeature(pTablas, idProvincias, "ADM2", features, pOntology);

            //Carga.
            Parallel.ForEach(features, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, feature =>
            {
                mResourceApi.LoadSecondaryResource(feature.ToGnossApiResource(mResourceApi, pOntology + "_" + feature.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos Feature a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pId">Número que se le agregará al ID creado.</param>
        /// <param name="pListaFeatures">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<FeatureOntology.Feature> ObtenerDatosFeature(ReferenceTables pTablas, string pCodigoTabla, string pId, List<FeatureOntology.Feature> pListaFeatures, string pOntology)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {

                    FeatureOntology.Feature feature = new FeatureOntology.Feature();
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
                    feature.Gn_featureCode = pId;

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
        private static void CargarProjectModality(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/ProjectModality", pOntology);

            //Obtención de los objetos a cargar.
            List<ProjectModality> modalities = new List<ProjectModality>();
            modalities = ObtenerDatosProjectModality(pTablas, idProjectModality, modalities);

            //Carga.
            Parallel.ForEach(modalities, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, modality =>
            {
                mResourceApi.LoadSecondaryResource(modality.ToGnossApiResource(mResourceApi, pOntology + "_" + modality.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos Modality a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaModality">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ProjectModality> ObtenerDatosProjectModality(ReferenceTables pTablas, string pCodigoTabla, List<ProjectModality> pListaModality)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        ProjectModality modality = new ProjectModality();
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
            Parallel.ForEach(contributions, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, contribution =>
            {
                mResourceApi.LoadSecondaryResource(contribution.ToGnossApiResource(mResourceApi, pOntology + "_" + contribution.Dc_identifier));
            });
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
        private static void CargarParticipationTypeProject(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/ParticipationTypeProject", pOntology);

            //Obtención de los objetos a cargar.
            List<ParticipationTypeProject> participations = new List<ParticipationTypeProject>();
            participations = ObtenerDatosParticipationTypeProject(pTablas, idParticipationTypeProject, participations);

            //Carga.
            Parallel.ForEach(participations, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, participation =>
            {
                mResourceApi.LoadSecondaryResource(participation.ToGnossApiResource(mResourceApi, pOntology + "_" + participation.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos ParticipationType a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaParticipationType">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ParticipationTypeProject> ObtenerDatosParticipationTypeProject(ReferenceTables pTablas, string pCodigoTabla, List<ParticipationTypeProject> pListaParticipationType)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        ParticipationTypeProject participation = new ParticipationTypeProject();
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
            Parallel.ForEach(dedications, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, dedication =>
            {
                mResourceApi.LoadSecondaryResource(dedication.ToGnossApiResource(mResourceApi, pOntology + "_" + dedication.Dc_identifier));
            });
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
        private static void CargarEventInscriptionType(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/EventInscriptionType", pOntology);

            //Obtención de los objetos a cargar.
            List<EventInscriptionType> motivations = new List<EventInscriptionType>();
            motivations = ObtenerDatosEventInscriptionType(pTablas, idEventInscriptionType, motivations);

            //Carga.
            Parallel.ForEach(motivations, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, motivation =>
            {
                mResourceApi.LoadSecondaryResource(motivation.ToGnossApiResource(mResourceApi, pOntology + "_" + motivation.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos Motivation a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaMotivation">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<EventInscriptionType> ObtenerDatosEventInscriptionType(ReferenceTables pTablas, string pCodigoTabla, List<EventInscriptionType> pListaMotivation)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        EventInscriptionType motivation = new EventInscriptionType();
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
            Parallel.ForEach(contributions, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, contribution =>
            {
                mResourceApi.LoadSecondaryResource(contribution.ToGnossApiResource(mResourceApi, pOntology + "_" + contribution.Dc_identifier));
            });
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
            Parallel.ForEach(references, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, reference =>
            {
                mResourceApi.LoadSecondaryResource(reference.ToGnossApiResource(mResourceApi, pOntology + "_" + reference.Dc_identifier));
            });
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
            Parallel.ForEach(categorias, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, category =>
            {
                mResourceApi.LoadSecondaryResource(category.ToGnossApiResource(mResourceApi, pOntology + "_" + category.Dc_identifier));
            });
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
            Parallel.ForEach(lenguajes, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, language =>
            {
                mResourceApi.LoadSecondaryResource(language.ToGnossApiResource(mResourceApi, pOntology + "_" + language.Dc_identifier));
            });
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
            Parallel.ForEach(publicaciones, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, publication =>
            {
                mResourceApi.LoadSecondaryResource(publication.ToGnossApiResource(mResourceApi, pOntology + "_" + publication.Dc_identifier));
            });
        }


        /// <summary>
        /// Carga la entidad secundaria ResearchObjcetType.
        /// </summary>
        private static void CargarResearhObjectType()
        {
            string ontology = "researchobjecttype";
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(ontology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/ResearchObjectType", ontology);

            //Obtención de los objetos a cargar.
            List<ResearchObjectType> researchObjects = new List<ResearchObjectType>();
            researchObjects.Add(
                new ResearchObjectType()
                {
                    Dc_identifier = "1",
                    Dc_title = new Dictionary<LanguageEnum, string>() { { LanguageEnum.es, "Dataset" } },
                }
            );
            researchObjects.Add(
                new ResearchObjectType()
                {
                    Dc_identifier = "2",
                    Dc_title = new Dictionary<LanguageEnum, string>() { { LanguageEnum.es, "Presentación" } },
                }
            );
            researchObjects.Add(
                new ResearchObjectType()
                {
                    Dc_identifier = "3",
                    Dc_title = new Dictionary<LanguageEnum, string>() { { LanguageEnum.es, "Gráfico" } },
                }
            );
            researchObjects.Add(
                new ResearchObjectType()
                {
                    Dc_identifier = "4",
                    Dc_title = new Dictionary<LanguageEnum, string>() { { LanguageEnum.es, "Documento" } },
                }
            );
            researchObjects.Add(
                new ResearchObjectType()
                {
                    Dc_identifier = "5",
                    Dc_title = new Dictionary<LanguageEnum, string>() { { LanguageEnum.es, "Enlace" } },
                }
            );
            researchObjects.Add(
                new ResearchObjectType()
                {
                    Dc_identifier = "6",
                    Dc_title = new Dictionary<LanguageEnum, string>() { { LanguageEnum.es, "Video" } },
                }
            );
            researchObjects.Add(
                new ResearchObjectType()
                {
                    Dc_identifier = "7",
                    Dc_title = new Dictionary<LanguageEnum, string>() { { LanguageEnum.es, "Poster" } },
                }
            );
            researchObjects.Add(
                new ResearchObjectType()
                {
                    Dc_identifier = "8",
                    Dc_title = new Dictionary<LanguageEnum, string>() { { LanguageEnum.es, "Lección" } },
                }
            );
            researchObjects.Add(
                new ResearchObjectType()
                {
                    Dc_identifier = "9",
                    Dc_title = new Dictionary<LanguageEnum, string>() { { LanguageEnum.es, "Código" } },
                }
            );

            //Carga.
            Parallel.ForEach(researchObjects, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, ro =>
            {
                mResourceApi.LoadSecondaryResource(ro.ToGnossApiResource(mResourceApi, ontology + "_" + ro.Dc_identifier));
            });
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
            Parallel.ForEach(eventos, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, eventType =>
            {
                mResourceApi.LoadSecondaryResource(eventType.ToGnossApiResource(mResourceApi, pOntology + "_" + eventType.Dc_identifier));
            });
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
            EliminarDatosCargados("http://vivoweb.org/ontology/core#GeographicRegion", pOntology);

            //Obtención de los objetos a cargar.
            List<GeographicRegion> regiones = new List<GeographicRegion>();
            regiones = ObtenerDatosGeographicRegion(pTablas, idGeographicRegion, regiones);

            //Carga.
            Parallel.ForEach(regiones, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, region =>
            {
                mResourceApi.LoadSecondaryResource(region.ToGnossApiResource(mResourceApi, pOntology + "_" + region.Dc_identifier));
            });
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
            Parallel.ForEach(organizaciones, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, organization =>
            {
                mResourceApi.LoadSecondaryResource(organization.ToGnossApiResource(mResourceApi, pOntology + "_" + organization.Dc_identifier));
            });
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
        private static void CargarDocumentFormat(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/DocumentFormat", pOntology);

            //Obtención de los objetos a cargar.
            List<DocumentFormat> publicaciones = new List<DocumentFormat>();
            publicaciones = ObtenerDatosDocumentFormat(pTablas, idDocumentFormat, publicaciones);

            //Carga.
            Parallel.ForEach(publicaciones, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, publication =>
            {
                mResourceApi.LoadSecondaryResource(publication.ToGnossApiResource(mResourceApi, pOntology + "_" + publication.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos Language a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaSupportType">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<DocumentFormat> ObtenerDatosDocumentFormat(ReferenceTables pTablas, string pCodigoTabla, List<DocumentFormat> pListaSupportType)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        DocumentFormat publication = new DocumentFormat();
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
        /// Carga la entidad secundaria Gender.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarGender(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/Gender", pOntology);

            //Obtención de los objetos a cargar.
            List<Gender> generos = new List<Gender>();
            generos = ObtenerDatosGender(pTablas, idGender, generos);

            //Carga.
            Parallel.ForEach(generos, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, genre =>
            {
                mResourceApi.LoadSecondaryResource(genre.ToGnossApiResource(mResourceApi, pOntology + "_" + genre.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos Gender a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaGender">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<Gender> ObtenerDatosGender(ReferenceTables pTablas, string pCodigoTabla, List<Gender> pListaGender)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        Gender genre = new Gender();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail genero in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[genero.lang];
                            string nombre = genero.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        genre.Dc_identifier = identificador;
                        genre.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaGender.Add(genre);
                    }
                }
            }

            return pListaGender;
        }

        /// <summary>
        /// Carga la entidad secundaria ProjectType.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarProjectType(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/ProjectType", pOntology);

            //Obtención de los objetos a cargar.
            List<ProjectType> tipoProyectos = new List<ProjectType>();
            tipoProyectos = ObtenerDatosProjectType(pTablas, idProjectType, tipoProyectos);

            //Carga.
            Parallel.ForEach(tipoProyectos, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, project =>
            {
                mResourceApi.LoadSecondaryResource(project.ToGnossApiResource(mResourceApi, pOntology + "_" + project.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos ProjectType a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaProjectType">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ProjectType> ObtenerDatosProjectType(ReferenceTables pTablas, string pCodigoTabla, List<ProjectType> pListaProjectType)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        ProjectType projectType = new ProjectType();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail tipoProyecto in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[tipoProyecto.lang];
                            string nombre = tipoProyecto.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        projectType.Dc_identifier = identificador;
                        projectType.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaProjectType.Add(projectType);
                    }
                }
            }

            return pListaProjectType;
        }

        /// <summary>
        /// Carga la entidad secundaria ProjectType.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarParticipationTypeDocument(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/ParticipationTypeDocument", pOntology);

            //Obtención de los objetos a cargar.
            List<ParticipationTypeDocument> tipoParticipacion = new List<ParticipationTypeDocument>();
            tipoParticipacion = ObtenerDatosParticipationTypeDocument(pTablas, idParticipationTypeDocument, tipoParticipacion);

            //Carga.
            Parallel.ForEach(tipoParticipacion, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, participacion =>
            {
                mResourceApi.LoadSecondaryResource(participacion.ToGnossApiResource(mResourceApi, pOntology + "_" + participacion.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos ProjectType a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaProjectType">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ParticipationTypeDocument> ObtenerDatosParticipationTypeDocument(ReferenceTables pTablas, string pCodigoTabla, List<ParticipationTypeDocument> pListaDatosParticipationTypeDocument)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        ParticipationTypeDocument participationType = new ParticipationTypeDocument();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail tipoParticipacion in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[tipoParticipacion.lang];
                            string nombre = tipoParticipacion.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        participationType.Dc_identifier = identificador;
                        participationType.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaDatosParticipationTypeDocument.Add(participationType);
                    }
                }
            }

            return pListaDatosParticipationTypeDocument;
        }

        /// <summary>
        /// Carga la entidad secundaria IndustrialPropertyType.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarIndustrialPropertyType(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/IndustrialPropertyType", pOntology);

            //Obtención de los objetos a cargar.
            List<IndustrialPropertyType> propiedadIndustrial = new List<IndustrialPropertyType>();
            propiedadIndustrial = ObtenerDatosIndustrialPropertyType(pTablas, idIndustrialPropertyType, propiedadIndustrial);

            //Carga.
            Parallel.ForEach(propiedadIndustrial, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, propiedad =>
            {
                mResourceApi.LoadSecondaryResource(propiedad.ToGnossApiResource(mResourceApi, pOntology + "_" + propiedad.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos IndustrialPropertyType a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaDatosIndustrialPropertyType">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<IndustrialPropertyType> ObtenerDatosIndustrialPropertyType(ReferenceTables pTablas, string pCodigoTabla, List<IndustrialPropertyType> pListaDatosIndustrialPropertyType)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        IndustrialPropertyType idustrialProperty = new IndustrialPropertyType();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail propiedadIndustrial in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[propiedadIndustrial.lang];
                            string nombre = propiedadIndustrial.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        idustrialProperty.Dc_identifier = identificador;
                        idustrialProperty.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaDatosIndustrialPropertyType.Add(idustrialProperty);
                    }
                }
            }

            return pListaDatosIndustrialPropertyType;
        }

        /// <summary>
        /// Carga la entidad secundaria ColaborationTypeGroup.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarColaborationTypeGroup(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/ColaborationTypeGroup", pOntology);

            //Obtención de los objetos a cargar.
            List<ColaborationTypeGroup> grupoColaborativo = new List<ColaborationTypeGroup>();
            grupoColaborativo = ObtenerDatosColaborationTypeGroup(pTablas, idColaborationTypeGroup, grupoColaborativo);

            //Carga.
            Parallel.ForEach(grupoColaborativo, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, grupo =>
            {
                mResourceApi.LoadSecondaryResource(grupo.ToGnossApiResource(mResourceApi, pOntology + "_" + grupo.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos ColaborationTypeGroup a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaDatosColaborationTypeGroup">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ColaborationTypeGroup> ObtenerDatosColaborationTypeGroup(ReferenceTables pTablas, string pCodigoTabla, List<ColaborationTypeGroup> pListaDatosColaborationTypeGroup)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        ColaborationTypeGroup colaborationGroup = new ColaborationTypeGroup();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail grupoColaboracion in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[grupoColaboracion.lang];
                            string nombre = grupoColaboracion.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        colaborationGroup.Dc_identifier = identificador;
                        colaborationGroup.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaDatosColaborationTypeGroup.Add(colaborationGroup);
                    }
                }
            }

            return pListaDatosColaborationTypeGroup;
        }

        /// <summary>
        /// Carga la entidad secundaria ManagementTypeActivity.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarManagementTypeActivity(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/ManagementTypeActivity", pOntology);

            //Obtención de los objetos a cargar.
            List<ManagementTypeActivity> tipoActividad = new List<ManagementTypeActivity>();
            tipoActividad = ObtenerDatosManagementTypeActivity(pTablas, idManagementTypeActivity, tipoActividad);

            //Carga.
            Parallel.ForEach(tipoActividad, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, typeActivity =>
            {
                mResourceApi.LoadSecondaryResource(typeActivity.ToGnossApiResource(mResourceApi, pOntology + "_" + typeActivity.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos ManagementTypeActivity a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaDatosManagementTypeActivity">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ManagementTypeActivity> ObtenerDatosManagementTypeActivity(ReferenceTables pTablas, string pCodigoTabla, List<ManagementTypeActivity> pListaDatosManagementTypeActivity)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        ManagementTypeActivity typeActivity = new ManagementTypeActivity();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail tipoActividad in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[tipoActividad.lang];
                            string nombre = tipoActividad.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        typeActivity.Dc_identifier = identificador;
                        typeActivity.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaDatosManagementTypeActivity.Add(typeActivity);
                    }
                }
            }

            return pListaDatosManagementTypeActivity;
        }

        /// <summary>
        /// Carga la entidad secundaria TargetGroupProfile.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarTargetGroupProfile(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/TargetGroupProfile", pOntology);

            //Obtención de los objetos a cargar.
            List<TargetGroupProfile> perfilGrupo = new List<TargetGroupProfile>();
            perfilGrupo = ObtenerDatosTargetGroupProfile(pTablas, idTargetGroupProfile, perfilGrupo);

            //Carga.
            Parallel.ForEach(perfilGrupo, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, groupProfile =>
            {
                mResourceApi.LoadSecondaryResource(groupProfile.ToGnossApiResource(mResourceApi, pOntology + "_" + groupProfile.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos TargetGroupProfile a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaDatosTargetGroupProfile">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<TargetGroupProfile> ObtenerDatosTargetGroupProfile(ReferenceTables pTablas, string pCodigoTabla, List<TargetGroupProfile> pListaDatosTargetGroupProfile)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        TargetGroupProfile groupProfile = new TargetGroupProfile();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail perfilGrupo in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[perfilGrupo.lang];
                            string nombre = perfilGrupo.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        groupProfile.Dc_identifier = identificador;
                        groupProfile.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaDatosTargetGroupProfile.Add(groupProfile);
                    }
                }
            }

            return pListaDatosTargetGroupProfile;
        }

        /// <summary>
        /// Carga la entidad secundaria AccessSystemActivity.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarAccessSystemActivity(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/AccessSystemActivity", pOntology);

            //Obtención de los objetos a cargar.
            List<AccessSystemActivity> sistemaAcceso = new List<AccessSystemActivity>();
            sistemaAcceso = ObtenerDatosAccessSystemActivity(pTablas, idAccessSystemActivity, sistemaAcceso);

            //Carga.
            Parallel.ForEach(sistemaAcceso, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, accessSistem =>
            {
                mResourceApi.LoadSecondaryResource(accessSistem.ToGnossApiResource(mResourceApi, pOntology + "_" + accessSistem.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos AccessSystemActivity a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaDatosAccessSystemActivity">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<AccessSystemActivity> ObtenerDatosAccessSystemActivity(ReferenceTables pTablas, string pCodigoTabla, List<AccessSystemActivity> pListaDatosAccessSystemActivity)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        AccessSystemActivity accessSystem = new AccessSystemActivity();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail sistemaAcceso in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[sistemaAcceso.lang];
                            string nombre = sistemaAcceso.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        accessSystem.Dc_identifier = identificador;
                        accessSystem.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaDatosAccessSystemActivity.Add(accessSystem);
                    }
                }
            }

            return pListaDatosAccessSystemActivity;
        }

        /// <summary>
        /// Carga la entidad secundaria ParticipationTypeActivity.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarParticipationTypeActivity(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/ParticipationTypeActivity", pOntology);

            //Obtención de los objetos a cargar.
            List<ParticipationTypeActivity> tipoActividad = new List<ParticipationTypeActivity>();
            tipoActividad = ObtenerDatosParticipationTypeActivity(pTablas, idParticipationTypeActivity, tipoActividad);

            //Carga.
            Parallel.ForEach(tipoActividad, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, activityType =>
            {
                mResourceApi.LoadSecondaryResource(activityType.ToGnossApiResource(mResourceApi, pOntology + "_" + activityType.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos ParticipationTypeActivity a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaDatosParticipationTypeActivity">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ParticipationTypeActivity> ObtenerDatosParticipationTypeActivity(ReferenceTables pTablas, string pCodigoTabla, List<ParticipationTypeActivity> pListaDatosParticipationTypeActivity)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        ParticipationTypeActivity participationType = new ParticipationTypeActivity();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail tipoParticipacion in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[tipoParticipacion.lang];
                            string nombre = tipoParticipacion.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        participationType.Dc_identifier = identificador;
                        participationType.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaDatosParticipationTypeActivity.Add(participationType);
                    }
                }
            }

            return pListaDatosParticipationTypeActivity;
        }

        /// <summary>
        /// Carga la entidad secundaria StayGoal.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarStayGoal(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/StayGoal", pOntology);

            //Obtención de los objetos a cargar.
            List<StayGoal> meta = new List<StayGoal>();
            meta = ObtenerDatosStayGoal(pTablas, idStayGoal, meta);

            //Carga.
            Parallel.ForEach(meta, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, stayGoal =>
            {
                mResourceApi.LoadSecondaryResource(stayGoal.ToGnossApiResource(mResourceApi, pOntology + "_" + stayGoal.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos StayGoal a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaDatosStayGoal">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<StayGoal> ObtenerDatosStayGoal(ReferenceTables pTablas, string pCodigoTabla, List<StayGoal> pListaDatosStayGoal)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        StayGoal stayGoal = new StayGoal();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail meta in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[meta.lang];
                            string nombre = meta.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        stayGoal.Dc_identifier = identificador;
                        stayGoal.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaDatosStayGoal.Add(stayGoal);
                    }
                }
            }

            return pListaDatosStayGoal;
        }

        /// <summary>
        /// Carga la entidad secundaria GrantAim.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarGrantAim(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/GrantAim", pOntology);

            //Obtención de los objetos a cargar.
            List<GrantAim> objetivo = new List<GrantAim>();
            objetivo = ObtenerDatosGrantAim(pTablas, idGrantAim, objetivo);

            //Carga.
            Parallel.ForEach(objetivo, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, grantAim =>
            {
                mResourceApi.LoadSecondaryResource(grantAim.ToGnossApiResource(mResourceApi, pOntology + "_" + grantAim.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos GrantAim a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaDatosGrantAim">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<GrantAim> ObtenerDatosGrantAim(ReferenceTables pTablas, string pCodigoTabla, List<GrantAim> pListaDatosGrantAim)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        GrantAim grantAim = new GrantAim();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail objetivo in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[objetivo.lang];
                            string nombre = objetivo.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        grantAim.Dc_identifier = identificador;
                        grantAim.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaDatosGrantAim.Add(grantAim);
                    }
                }
            }

            return pListaDatosGrantAim;
        }

        /// <summary>
        /// Carga la entidad secundaria RelationshipType.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarRelationshipType(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/RelationshipType", pOntology);

            //Obtención de los objetos a cargar.
            List<RelationshipType> tipoRelacion = new List<RelationshipType>();
            tipoRelacion = ObtenerDatosRelationshipType(pTablas, idRelationshipType, tipoRelacion);

            //Carga.
            Parallel.ForEach(tipoRelacion, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, relationType =>
            {
                mResourceApi.LoadSecondaryResource(relationType.ToGnossApiResource(mResourceApi, pOntology + "_" + relationType.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos RelationshipType a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaDatosRelationshipType">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<RelationshipType> ObtenerDatosRelationshipType(ReferenceTables pTablas, string pCodigoTabla, List<RelationshipType> pListaDatosRelationshipType)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        RelationshipType relationType = new RelationshipType();
                        Dictionary<LanguageEnum, string> dicIdioma = new Dictionary<LanguageEnum, string>();
                        string identificador = item.Code;
                        foreach (TableItemNameDetail tipoRelacion in item.Name)
                        {
                            LanguageEnum idioma = dicIdiomasMapeados[tipoRelacion.lang];
                            string nombre = tipoRelacion.Name;
                            dicIdioma.Add(idioma, nombre);
                        }

                        //Se agrega las propiedades.
                        relationType.Dc_identifier = identificador;
                        relationType.Dc_title = dicIdioma;

                        //Se guarda el objeto a la lista.
                        pListaDatosRelationshipType.Add(relationType);
                    }
                }
            }

            return pListaDatosRelationshipType;
        }

        /// <summary>
        /// Carga la entidad secundaria ActivityModality.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarActivityModality(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/ActivityModality", pOntology);

            //Obtención de los objetos a cargar.
            List<ActivityModality> modalidad = new List<ActivityModality>();
            modalidad = ObtenerDatosActivityModality(pTablas, idActivityModality, modalidad);

            //Carga.
            Parallel.ForEach(modalidad, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, modality =>
            {
                mResourceApi.LoadSecondaryResource(modality.ToGnossApiResource(mResourceApi, pOntology + "_" + modality.Dc_identifier));
            });
        }

        /// <summary>
        /// Carga la entidad secundaria ContractModality.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarContractModality(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/ContractModality", pOntology);

            //Obtención de los objetos a cargar.
            List<ContractModality> modalidad = new List<ContractModality>();
            modalidad = ObtenerDatosContractModality(pTablas, idContractModality, modalidad);

            //Carga.
            Parallel.ForEach(modalidad, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, modality =>
            {
                mResourceApi.LoadSecondaryResource(modality.ToGnossApiResource(mResourceApi, pOntology + "_" + modality.Dc_identifier));
            });
        }

        /// <summary>
        /// Carga la entidad secundaria ScopeManagementActivity.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarScopeManagementActivity(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/ScopeManagementActivity", pOntology);

            //Obtención de los objetos a cargar.
            List<ScopeManagementActivity> modalidad = new List<ScopeManagementActivity>();
            modalidad = ObtenerDatosScopeManagementActivity(pTablas, idScopeManagementActivity, modalidad);

            //Carga.
            Parallel.ForEach(modalidad, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, modality =>
            {
                mResourceApi.LoadSecondaryResource(modality.ToGnossApiResource(mResourceApi, pOntology + "_" + modality.Dc_identifier));
            });
        }


        /// <summary>
        /// Obtiene los objetos RelationshipType a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaDatosActivityModality">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ActivityModality> ObtenerDatosActivityModality(ReferenceTables pTablas, string pCodigoTabla, List<ActivityModality> pListaDatosActivityModality)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        ActivityModality modality = new ActivityModality();
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
                        pListaDatosActivityModality.Add(modality);
                    }
                }
            }

            return pListaDatosActivityModality;
        }

        // <summary>
        /// Obtiene los objetos RelationshipType a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaDatosActivityModality">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ContractModality> ObtenerDatosContractModality(ReferenceTables pTablas, string pCodigoTabla, List<ContractModality> pListaDatosConstractModality)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        ContractModality modality = new ContractModality();
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
                        pListaDatosConstractModality.Add(modality);
                    }
                }
            }

            return pListaDatosConstractModality;
        }

        // <summary>
        /// Obtiene los objetos RelationshipType a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaDatosActivityModality">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ScopeManagementActivity> ObtenerDatosScopeManagementActivity(ReferenceTables pTablas, string pCodigoTabla, List<ScopeManagementActivity> pListaDatosScopeManagementActivity)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (string.IsNullOrEmpty(item.Delegate))
                    {
                        ScopeManagementActivity modality = new ScopeManagementActivity();
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
                        pListaDatosScopeManagementActivity.Add(modality);
                    }
                }
            }

            return pListaDatosScopeManagementActivity;
        }

        


        /// <summary>
        /// Carga el tesauro de unesco
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarTesauroUnesco(ReferenceTables pTablas, string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);
            EliminarDatosCargados("http://www.w3.org/2008/05/skos#Collection", "taxonomy", "unesco");
            EliminarDatosCargados("http://www.w3.org/2008/05/skos#Concept", "taxonomy", "unesco");

            //Obtención de los objetos a cargar.
            List<SecondaryResource> categorias = ObtenerDatosUnesco(pTablas, "unesco");

            //Carga.
            Parallel.ForEach(categorias, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, categoria =>
            {
                mResourceApi.LoadSecondaryResource(categoria);
            });
        }


        private static List<SecondaryResource> ObtenerDatosUnesco(ReferenceTables pTablas, string pSource)
        {
            List<SecondaryResource> secondaryResources = new List<SecondaryResource>();

            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            List<Concept> listConcepts = new List<Concept>();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == "UNESCO_CODES"))
            {
                foreach (TableItem item in tabla.Item)
                {
                    if (item.Code.Length != 6)
                    {
                        throw new Exception();
                    }
                    int level = 3;
                    if (item.Code.EndsWith("00"))
                    {
                        level = 2;
                    }
                    if (item.Code.EndsWith("0000"))
                    {
                        level = 1;
                    }

                    ConceptEDMA concept = new ConceptEDMA();
                    concept.Dc_identifier = item.Code;
                    concept.Dc_source = pSource;
                    concept.Skos_prefLabel = item.Name.First(x => x.lang == "spa").Name;
                    concept.Skos_symbol = level.ToString();
                    listConcepts.Add(concept);
                }
            }

            foreach (Concept concept in listConcepts)
            {
                concept.Skos_narrower = new List<Concept>();
                concept.Skos_broader = new List<Concept>();
                if (concept.Dc_identifier.EndsWith("0000"))
                {
                    concept.Skos_narrower = listConcepts.Where(x => x.Dc_identifier.StartsWith(concept.Dc_identifier.Substring(0, 2)) && x.Dc_identifier.EndsWith("00") && x.Dc_identifier != concept.Dc_identifier).ToList();
                }
                else if (concept.Dc_identifier.EndsWith("00"))
                {
                    concept.Skos_narrower = listConcepts.Where(x => x.Dc_identifier.StartsWith(concept.Dc_identifier.Substring(0, 4)) && x.Dc_identifier != concept.Dc_identifier).ToList();
                    concept.Skos_broader = listConcepts.Where(x => x.Dc_identifier.EndsWith("0000") && x.Dc_identifier.StartsWith(concept.Dc_identifier.Substring(0, 2))).ToList();
                    if (concept.Skos_broader.Count != 1)
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    concept.Skos_broader = listConcepts.Where(x => x.Dc_identifier.StartsWith(concept.Dc_identifier.Substring(0, 4)) && x.Dc_identifier.EndsWith("00") && x.Dc_identifier != concept.Dc_identifier).ToList();
                    if (concept.Skos_broader.Count != 1)
                    {
                        throw new Exception();
                    }
                }
                secondaryResources.Add(((ConceptEDMA)concept).ToGnossApiResource(mResourceApi, concept.Dc_identifier));
            }


            CollectionEDMA collection = new CollectionEDMA();
            collection.Dc_source = pSource;
            collection.Skos_member = listConcepts.Where(x => x.Dc_identifier.EndsWith("0000")).ToList();
            collection.Skos_scopeNote = "Tesauro UNESCO";
            secondaryResources.Add(collection.ToGnossApiResource(mResourceApi, "0"));

            return secondaryResources;
        }



        /// <summary>
        /// Carga la entidad secundaria ScientificActivityDocument.
        /// </summary>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarScientificActivityDocument(string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/ScientificActivityDocument", pOntology);

            //Obtención de los objetos a cargar.
            List<ScientificActivityDocument> documentoCientifico = new List<ScientificActivityDocument>();
            documentoCientifico = ObtenerDatosScientificActivityDocument(documentoCientifico);

            //Carga.
            Parallel.ForEach(documentoCientifico, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, scientificDocument =>
            {
                mResourceApi.LoadSecondaryResource(scientificDocument.ToGnossApiResource(mResourceApi, pOntology + "_" + scientificDocument.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos ScientificActivityDocument a cargar.
        /// </summary>
        /// <param name="pListaDatosScientificActivityDocument">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ScientificActivityDocument> ObtenerDatosScientificActivityDocument(List<ScientificActivityDocument> pListaDatosScientificActivityDocument)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            //Diccionario con datos.
            Dictionary<string, string> dicDatos = new Dictionary<string, string>();
            dicDatos.Add("SAD1~Publicaciones", "Publicaciones, documentos científicos y técnicos");
            dicDatos.Add("SAD2~En congresos", "Trabajos presentados en congresos nacionales o internacionales");
            dicDatos.Add("SAD3~En jornadas, seminarios…", "Trabajos presentados en jornadas, seminarios, talleres de trabajo y/o cursos nacionales o internacionales");
            dicDatos.Add("SAD4~De divulgación", "Otras actividades de divulgación");

            foreach (KeyValuePair<string, string> itemData in dicDatos)
            {
                ScientificActivityDocument scientificDocument = new ScientificActivityDocument();
                Dictionary<LanguageEnum, string> dicIdiomaTitulos = new Dictionary<LanguageEnum, string>();
                Dictionary<LanguageEnum, string> dicIdiomaDescripciones = new Dictionary<LanguageEnum, string>();

                //Titulo.
                foreach (KeyValuePair<string, LanguageEnum> item in dicIdiomasMapeados)
                {
                    LanguageEnum idioma = dicIdiomasMapeados[item.Key];
                    dicIdiomaTitulos.Add(idioma, itemData.Key.Split("~")[1]);
                }

                //Descripción.
                foreach (KeyValuePair<string, LanguageEnum> item in dicIdiomasMapeados)
                {
                    LanguageEnum idioma = dicIdiomasMapeados[item.Key];
                    dicIdiomaDescripciones.Add(idioma, itemData.Value);
                }

                //Se agrega las propiedades.
                scientificDocument.Dc_identifier = itemData.Key.Split("~")[0];
                scientificDocument.Dc_title = dicIdiomaTitulos;
                scientificDocument.Bibo_abstract = dicIdiomaDescripciones;

                //Se guarda el objeto a la lista.
                pListaDatosScientificActivityDocument.Add(scientificDocument);
            }

            return pListaDatosScientificActivityDocument;
        }

        /// <summary>
        /// Carga la entidad secundaria Department.
        /// </summary>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarDepartment(string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://vivoweb.org/ontology/core#Department", pOntology);

            //Obtención de los objetos a cargar.
            List<Department> departamentos = new List<Department>();
            departamentos = ObtenerDatosDepartment(departamentos);

            //Carga.
            Parallel.ForEach(departamentos, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, department =>
            {
                var x = mResourceApi.LoadSecondaryResource(department.ToGnossApiResource(mResourceApi, pOntology + "_" + department.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos Department a cargar.
        /// </summary>
        /// <param name="pListaDatosDepartment">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<Department> ObtenerDatosDepartment(List<Department> pListaDatosDepartment)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            //Lista con datos.
            List<Departamento> departamentos = LeerDepartamentos("Dataset/UMU" + "/Departamentos.xml");

            foreach (Departamento dept in departamentos)
            {
                Department department = new Department();

                //Se agrega las propiedades.
                department.Dc_identifier = dept.DEP_CODIGO;
                department.Dc_title = dept.DEP_NOMBRE;

                //Se guarda el objeto a la lista.
                pListaDatosDepartment.Add(department);
            }

            return pListaDatosDepartment;
        }

        /// <summary>
        /// Carga la entidad secundaria ScientificExperienceProject.
        /// </summary>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarScientificExperienceProject(string pOntology)
        {
            //Cambio de ontología.
            mResourceApi.ChangeOntoly(pOntology);

            //Elimina los datos cargados antes de volverlos a cargar.
            EliminarDatosCargados("http://w3id.org/roh/ScientificExperienceProject", pOntology);

            //Obtención de los objetos a cargar.
            List<ScientificExperienceProject> proyectoCientifico = new List<ScientificExperienceProject>();
            proyectoCientifico = ObtenerDatosScientificExperienceProject(proyectoCientifico);

            //Carga.
            Parallel.ForEach(proyectoCientifico, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, scientificProject =>
            {
                mResourceApi.LoadSecondaryResource(scientificProject.ToGnossApiResource(mResourceApi, pOntology + "_" + scientificProject.Dc_identifier));
            });
        }

        /// <summary>
        /// Obtiene los objetos ScientificExperienceProject a cargar.
        /// </summary>
        /// <param name="pListaDatosScientificActivityDocument">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ScientificExperienceProject> ObtenerDatosScientificExperienceProject(List<ScientificExperienceProject> pListaDatosScientificExperienceProject)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            //Diccionario con datos.
            Dictionary<string, string> dicDatos = new Dictionary<string, string>();
            dicDatos.Add("SEP1~Competitivos", "Proyectos de I+D+i financiados en convocatorias competitivas de Administraciones o entidades públicas y privadas");
            dicDatos.Add("SEP2~No competitivos", "Contratos, convenios o proyectos de I+D+i no competitivos con Administraciones o entidades públicas o privadas");

            foreach (KeyValuePair<string, string> itemData in dicDatos)
            {
                ScientificExperienceProject scientificProject = new ScientificExperienceProject();
                Dictionary<LanguageEnum, string> dicIdiomaTitulos = new Dictionary<LanguageEnum, string>();
                Dictionary<LanguageEnum, string> dicIdiomaDescripciones = new Dictionary<LanguageEnum, string>();

                //Titulo.
                foreach (KeyValuePair<string, LanguageEnum> item in dicIdiomasMapeados)
                {
                    LanguageEnum idioma = dicIdiomasMapeados[item.Key];
                    dicIdiomaTitulos.Add(idioma, itemData.Key.Split("~")[1]);
                }

                //Descripción.
                foreach (KeyValuePair<string, LanguageEnum> item in dicIdiomasMapeados)
                {
                    LanguageEnum idioma = dicIdiomasMapeados[item.Key];
                    dicIdiomaDescripciones.Add(idioma, itemData.Value);
                }

                //Se agrega las propiedades.
                scientificProject.Dc_identifier = itemData.Key.Split("~")[0];
                scientificProject.Dc_title = dicIdiomaTitulos;
                scientificProject.Bibo_abstract = dicIdiomaDescripciones;

                //Se guarda el objeto a la lista.
                pListaDatosScientificExperienceProject.Add(scientificProject);
            }

            return pListaDatosScientificExperienceProject;
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
                listaUrlSecundarias = resultadoQuery.results.bindings.Select(x => GetValorFilaSparqlObject(x, "s")).ToList();
            }
            Parallel.ForEach(listaUrlSecundarias, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, url =>
            {
                List<string> listaUrlSecundariasAux = new List<string>() { url };
                mResourceApi.DeleteSecondaryEntitiesList(ref listaUrlSecundariasAux);
            });
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

        /// <summary>
        /// Permite leer del fichero los datos de los departamentos.
        /// </summary>
        /// <param name="pFile">Fichero.</param>
        /// <returns>Lista de objetos con los datos.</returns>
        private static List<Departamento> LeerDepartamentos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<Departamento> elementos = new List<Departamento>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                Departamento elemento = new Departamento();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "DEP_CODIGO":
                            elemento.DEP_CODIGO = node.SelectSingleNode("DEP_CODIGO").InnerText;
                            break;
                        case "DEP_NOMBRE":
                            elemento.DEP_NOMBRE = node.SelectSingleNode("DEP_NOMBRE").InnerText;
                            break;
                        case "DEP_CED_CODIGO":
                            elemento.DEP_CED_CODIGO = node.SelectSingleNode("DEP_CED_CODIGO").InnerText;
                            break;
                        default:
                            throw new Exception("Propiedad no controlada");
                    }
                }
                elementos.Add(elemento);
            }
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de {pFile}");
            return elementos;
        }

        /// <summary>
        /// Obtiene las propiedades.
        /// </summary>
        /// <param name="objeto">Objeto con los datos a leer.</param>
        /// <returns>Lista de los nombres de las propiedades.</returns>
        private static List<string> Propiedades(Object objeto)
        {
            List<string> prpos = new List<string>();
            Type type = objeto.GetType();
            System.Reflection.PropertyInfo[] listaPropiedades = type.GetProperties();
            return listaPropiedades.Select(x => x.Name).ToList();
        }
    }
}
