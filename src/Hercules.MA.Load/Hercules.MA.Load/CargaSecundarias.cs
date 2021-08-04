using ContributionGradeOntology;
using FeatureOntology;
using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using Hercules.MA.Load.Models.CVN;
using ModalityOntology;
using ParticipationTypeOntology;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using static GnossBase.GnossOCBase;

namespace Hercules.MA.Load
{
    /// <summary>
    /// Clase encargada de cargar los datos de las entidades secundarias de Hércules-MA.
    /// </summary>
    public class CargaSecundarias
    {
        //Ruta con el XML de datos a leer.
        private static string RUTA_XML = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Dataset\CVN\ReferenceTables.xml";

        // Resource API
        private static ResourceApi mResourceApi = new ResourceApi($@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config\configOAuth\OAuthV3.config");

        //Identificadores de las tablas.
        private static string idPaises = "ISO_3166";
        private static string idRegiones = "CVN_REGION";
        private static string idProvincias = "CVN_PROVINCE";       
        private static string idParticipationType = "CVN_PARTICIPATION_A";
        private static string idContributionGrade = "CVN_PARTICIPATION_B";
        private static string idModalidad = "CVN_PROJECT_C";

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

            //Elimina los datos anteriormente cargados.
            EliminarDatosCargados("http://www.geonames.org/ontology#Feature", "Feature.owl");
            EliminarDatosCargados("http://w3id.org/roh/Modality", "Modality.owl");
            EliminarDatosCargados("http://w3id.org/roh/ContributionGrade", "ContributionGrade.owl");
            EliminarDatosCargados("http://w3id.org/roh/ParticipationType", "ParticipationType.owl");

            //Carga de entidades secundarias.
            CargarFeatures(tablas);
            CargarModality(tablas);
            CargarContributionGrade(tablas);
            CargarParticipationType(tablas);
        }

        /// <summary>
        /// Carga la entidad secundaria Feature.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        private static void CargarFeatures(ReferenceTables pTablas)
        {
            //Obtención de los objetos a cargar.
            List<Feature> features = new List<Feature>();
            features = ObtenerDatosFeature(pTablas, idPaises, "PCLD", features);
            features = ObtenerDatosFeature(pTablas, idRegiones, "ADM1", features);
            features = ObtenerDatosFeature(pTablas, idProvincias, "ADM2", features);

            //Carga.
            foreach (Feature feature in features)
            {
                mResourceApi.LoadSecondaryResource(feature.ToGnossApiResource(mResourceApi, feature.Dc_identifier));
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
        private static List<Feature> ObtenerDatosFeature(ReferenceTables pTablas, string pCodigoTabla, string pId, List<Feature> pListaFeatures)
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
                            feature.IdGn_parentFeature = $@"PCLD_{codigoPadre}";
                            feature.Gn_parentFeature = pListaFeatures.First(x => x.Dc_identifier == $@"PCLD_{codigoPadre}");
                            break;
                        case "ADM2":
                            feature.IdGn_parentFeature = $@"ADM1_{codigoPadre}";
                            feature.Gn_parentFeature = pListaFeatures.First(x => x.Dc_identifier == $@"ADM1_{codigoPadre}");
                            break;
                    }

                    //Se guarda el objeto a la lista.
                    pListaFeatures.Add(feature);
                }
            }

            return pListaFeatures;
        }

        /// <summary>
        /// Carga la entidad secundaria Modality.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        private static void CargarModality(ReferenceTables pTablas)
        {
            //Obtención de los objetos a cargar.
            List<Modality> modalities = new List<Modality>();
            modalities = ObtenerDatosModality(pTablas, idModalidad, modalities);

            //Carga.
            foreach (Modality modality in modalities)
            {
                mResourceApi.LoadSecondaryResource(modality.ToGnossApiResource(mResourceApi, modality.Dc_identifier));
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

            return pListaModality;
        }

        /// <summary>
        /// Carga la entidad secundaria ContributionGrade.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        private static void CargarContributionGrade(ReferenceTables pTablas)
        {
            //Obtención de los objetos a cargar.
            List<ContributionGrade> contributions = new List<ContributionGrade>();
            contributions = ObtenerDatosContributionGrade(pTablas, idContributionGrade, contributions);

            //Carga.
            foreach (ContributionGrade contribution in contributions)
            {
                mResourceApi.LoadSecondaryResource(contribution.ToGnossApiResource(mResourceApi, contribution.Dc_identifier));
            }
        }

        /// <summary>
        /// Obtiene los objetos ContributionGrade a cargar.
        /// </summary>
        /// <param name="pTablas">Objetos con los datos a obtener.</param>
        /// <param name="pCodigoTabla">ID de la tabla a consultar.</param>
        /// <param name="pListaContributionGrade">Lista dónde guardar los objetos.</param>
        /// <returns>Lista con los objetos creados.</returns>
        private static List<ContributionGrade> ObtenerDatosContributionGrade(ReferenceTables pTablas, string pCodigoTabla, List<ContributionGrade> pListaContributionGrade)
        {
            //Mapea los idiomas.
            Dictionary<string, LanguageEnum> dicIdiomasMapeados = MapearLenguajes();

            foreach (Table tabla in pTablas.Table.Where(x => x.name == pCodigoTabla))
            {
                foreach (TableItem item in tabla.Item)
                {
                    ContributionGrade contribution = new ContributionGrade();
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

            return pListaContributionGrade;
        }

        /// <summary>
        /// Carga la entidad secundaria ParticipationType.
        /// </summary>
        /// <param name="pTablas">Tablas con los datos a obtener.</param>
        private static void CargarParticipationType(ReferenceTables pTablas)
        {
            //Obtención de los objetos a cargar.
            List<ParticipationType> participations = new List<ParticipationType>();
            participations = ObtenerDatosParticipationType(pTablas, idParticipationType, participations);

            //Carga.
            foreach (ParticipationType participation in participations)
            {
                mResourceApi.LoadSecondaryResource(participation.ToGnossApiResource(mResourceApi, participation.Dc_identifier));
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

            return pListaParticipationType;
        }

        /// <summary>
        /// Elimina los datos del grafo.
        /// </summary>
        /// <param name="pRdfType">RdfType del recurso a borrar.</param>
        /// <param name="pOntology">Ontología a consultar.</param>
        private static void EliminarDatosCargados(string pRdfType, string pOntology)
        {
            //Consulta.
            string select = string.Empty, where = string.Empty;
            select += $@"SELECT ?s ";
            where += $@"WHERE {{ ";
            where += $@"?s a <{pRdfType}>";
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
