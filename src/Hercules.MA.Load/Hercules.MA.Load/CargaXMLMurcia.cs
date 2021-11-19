using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using CurriculumvitaeOntology;
using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using Hercules.MA.Load.Models.TaxonomyOntology;
using Hercules.MA.Load.Models.UMU;
using TaxonomyOntology;

namespace Hercules.MA.Load
{
    /// <summary>
    /// Clase encargada de cargar los datos de las entidades principales de Hércules-MA.
    /// </summary>
    public class CargaXMLMurcia
    {
        //Directorio de lectura.
        private static string inputFolder = "Dataset/UMU";

        //Resource API.
        public static ResourceApi mResourceApi { get; set; }

        //Comunidad.
        public static Guid mIdComunidad { get; set; }

        //Diccionario secundarias.
        public static Dictionary<string, string> dicAreasCategoria { get; set; }

        /// <summary>
        /// Método para cargar las entidades principales.
        /// </summary>
        public static void CargarEntidadesPrincipales()
        {
            //Lectura de datos XML.   
            List<AreasUnescoProyectos> areasUnescoProyectos = LeerAreasUnescoProyectos(inputFolder + "/Areas UNESCO Proyectos.xml");
            List<Articulo> articulos = LeerArticulos(inputFolder + "/Articulos.xml");
            List<AutorArticulo> autoresArticulos = LeerAutoresArticulos(inputFolder + "/Autores articulos.xml");
            List<AutorCongreso> autoresCongresos = LeerAutoresCongresos(inputFolder + "/Autores congresos.xml");
            List<AutorExposicion> autoresExposiciones = LeerAutoresExposiciones(inputFolder + "/Autores exposiciones.xml");
            List<Centro> centros = LeerCentros(inputFolder + "/Centros.xml");
            List<CodigosUnesco> codigosUnesco = LeerCodigosUnesco(inputFolder + "/Codigos UNESCO.xml");
            List<Congreso> congresos = LeerCongresos(inputFolder + "/Congresos.xml");
            List<DatoEquipoInvestigacion> datoEquiposInvestigacion = LeerDatoEquiposInvestigacion(inputFolder + "/Datos equipo investigacion.xml");
            List<Departamento> departamentos = LeerDepartamentos(inputFolder + "/Departamentos.xml");
            List<DirectoresTesis> directoresTesis = LeerDirectoresTesis(inputFolder + "/Directores tesis.xml");
            List<EquipoProyecto> equiposProyectos = LeerEquiposProyectos(inputFolder + "/Equipos proyectos.xml");
            List<Exposicion> exposiciones = LeerExposiciones(inputFolder + "/Exposiciones.xml");
            List<Feature> features = LeerFeatures();
            List<FechaEquipoProyecto> fechasEquiposProyectos = LeerFechasEquiposProyectos(inputFolder + "/Fechas equipos proyectos.xml");
            List<FechaProyecto> fechasProyectos = LeerFechasProyectos(inputFolder + "/Fechas proyectos.xml");
            List<FuentesFinanciacionProyectos> fuentesFinanciacionProyectos = LeerFuentesFinanciacionProyectos(inputFolder + "/Fuentes financiacion proyectos.xml");
            List<GrupoInvestigacion> gruposInvestigacion = LeerGruposInvestigacion(inputFolder + "/Grupos de investigacion.xml");
            List<InventoresPatentes> inventoresPatentes = LeerInventoresPatentes(inputFolder + "/Inventores patentes.xml");
            List<LineasDeInvestigacion> lineasDeInvestigacion = LeerLineasDeInvestigacion(inputFolder + "/Lineas de investigacion.xml");
            List<LineasInvestigador> lineasInvestigador = LeerLineasInvestigador(inputFolder + "/Lineas investigador.xml");
            List<LineasUnesco> lineasUnesco = LeerLineasUnesco(inputFolder + "/Lineas UNESCO.xml");
            List<OrganizacionesExternas> organizacionesExternas = LeerOrganizacionesExternas(inputFolder + "/Organizaciones externas.xml");
            List<PalabrasClaveArticulos> palabrasClave = LeerPalabrasClaveArticulos(inputFolder + "/Palabras clave articulos.xml");
            List<Patentes> patentes = LeerPatentes(inputFolder + "/Patentes.xml");
            List<Persona> personas = LeerPersonas(inputFolder + "/Personas.xml");
            List<Proyecto> proyectos = LeerProyectos(inputFolder + "/Proyectos.xml");
            List<Tesis> tesis = LeerTesis(inputFolder + "/Tesis.xml");
            List<TipoParticipacionGrupo> tipoParticipacionGrupos = LeerTipoParticipacionGrupos(inputFolder + "/Tipo participacion grupo.xml");
            List<TiposEventos> tiposEventos = LeerTiposEventos(inputFolder + "/Tipos eventos.xml");

            // Obtención del ImpactIndexCategory (Secundaria).
            dicAreasCategoria = CargaListadosAreasRevistas();

            //Persona en específico a cargar.
            HashSet<string> personasACargar = new HashSet<string>() { "79", "1276" }; // Otro investigador: 1276
            //HashSet<string> personasACargar = new HashSet<string>();

            //Recursos para NO borrarlos.
            HashSet<string> listaNoBorrar = new HashSet<string>();

            //Lista de recursos a cargar.
            List<ComplexOntologyResource> listaRecursosCargar = new List<ComplexOntologyResource>();

            //Cargar organizaciones.
            CambiarOntologia("organization");
            //EliminarDatosCargados("http://xmlns.com/foaf/0.1/Organization", "organization", listaNoBorrar);
            Dictionary<string, string> organizacionesCargar = ObtenerOrganizaciones(personasACargar, ref listaRecursosCargar, equiposProyectos, organizacionesExternas, fuentesFinanciacionProyectos);
            //CargarDatos(listaRecursosCargar);
            listaRecursosCargar.Clear();

            //Cargar personas.
            CambiarOntologia("person");
            //EliminarDatosCargados("http://xmlns.com/foaf/0.1/Person", "person", listaNoBorrar);
            Dictionary<string, string> personasCargar = ObtenerPersonas(personasACargar, ref listaRecursosCargar, personas, autoresArticulos, autoresCongresos, autoresExposiciones, directoresTesis, equiposProyectos, inventoresPatentes, organizacionesCargar, datoEquiposInvestigacion);
            //CargarDatos(listaRecursosCargar);
            listaRecursosCargar.Clear();

            //Cargar grupos de investigación.
            CambiarOntologia("group");
            //EliminarDatosCargados("http://xmlns.com/foaf/0.1/Group", "group", listaNoBorrar);
            Dictionary<string, string> gruposCargar = ObtenerGrupos(personasACargar, personasCargar, ref listaRecursosCargar, personas, gruposInvestigacion, datoEquiposInvestigacion, organizacionesCargar, lineasDeInvestigacion, lineasInvestigador);
            //CargarDatos(listaRecursosCargar);
            listaRecursosCargar.Clear();

            //Cargar proyectos.
            CambiarOntologia("project");
            //EliminarDatosCargados("http://vivoweb.org/ontology/core#Project", "project", listaNoBorrar);
            Dictionary<string, string> proyectosCargar = ObtenerProyectos(personasACargar, personasCargar, organizacionesCargar, ref listaRecursosCargar, equiposProyectos, proyectos, organizacionesExternas, fechasProyectos, fechasEquiposProyectos, areasUnescoProyectos, codigosUnesco, fuentesFinanciacionProyectos);
            //CargarDatos(listaRecursosCargar);
            listaRecursosCargar.Clear();

            //Cargar revistas.
            CambiarOntologia("maindocument");
            //EliminarDatosCargados("http://w3id.org/roh/MainDocument", "maindocument", listaNoBorrar);
            Dictionary<string, string> revistarCargar = ObtenerMainDocuments(personasACargar, ref listaRecursosCargar, articulos, autoresArticulos);
            //CargarDatos(listaRecursosCargar);
            listaRecursosCargar.Clear();

            //Cargar documentos. (Publicaciones)
            CambiarOntologia("document");
            //EliminarDatosCargados("http://purl.org/ontology/bibo/Document", "document", listaNoBorrar);
            Dictionary<string, string> documentosCargar = ObtenerDocumentos(proyectosCargar, personasACargar, personasCargar, revistarCargar, ref listaRecursosCargar, articulos, autoresArticulos, personas, palabrasClave, proyectos, equiposProyectos);
            //CargarDatos(listaRecursosCargar);
            listaRecursosCargar.Clear();

            //Cargar documentos. (Congresos)
            CambiarOntologia("document");
            //EliminarDatosCargados("http://purl.org/ontology/bibo/Document", "document", listaNoBorrar);
            Dictionary<string, string> congresosCargar = ObtenerCongresos(proyectosCargar, personasACargar, personasCargar, revistarCargar, ref listaRecursosCargar, congresos, autoresCongresos, personas, equiposProyectos);
            //CargarDatos(listaRecursosCargar);
            listaRecursosCargar.Clear();

            //Cargar curriculums
            CambiarOntologia("curriculumvitae");
            //EliminarDatosCargados("http://w3id.org/roh/CV", "curriculumvitae", listaNoBorrar);
            Dictionary<string, string> cvCargar = ObtenerCVs(personasACargar, personasCargar, documentosCargar, ref listaRecursosCargar, articulos, autoresArticulos, personas);
            //CargarDatos(listaRecursosCargar);
            listaRecursosCargar.Clear();

            //Secundarios
            List<SecondaryResource> listaRecursosSecundariosCargar = new List<SecondaryResource>();

            //Categorización UM
            CambiarOntologia("taxonomy");
            CargaNormaCVN.EliminarDatosCargados("http://www.w3.org/2008/05/skos#Collection", "taxonomy", "um");
            CargaNormaCVN.EliminarDatosCargados("http://www.w3.org/2008/05/skos#Concept", "taxonomy", "um");
            ObtenerTesauroUMDocumentos(articulos, ref listaRecursosSecundariosCargar, "um");
            CargarDatosSecundarios(listaRecursosSecundariosCargar);
            listaRecursosSecundariosCargar.Clear();

            //TODO
            //Categorización UNESCO
            //CambiarOntologia("taxonomy");
            //CargaNormaCVN.EliminarDatosCargados("http://www.w3.org/2008/05/skos#Collection", "taxonomy", "unesco");
            //CargaNormaCVN.EliminarDatosCargados("http://www.w3.org/2008/05/skos#Concept", "taxonomy", "unesco");
            //ObtenerTesauroUMUnesco(articulos, ref listaRecursosSecundariosCargar, "unesco");
            //CargarDatosSecundarios(listaRecursosSecundariosCargar);
            //listaRecursosSecundariosCargar.Clear();
        }

        /// <summary>
        /// Permite cambiar de ontología sin que de error.
        /// </summary>
        /// <param name="pOntologia">Ontología a cambiar.</param>
        private static void CambiarOntologia(string pOntologia)
        {
            mResourceApi.ChangeOntoly(pOntologia);
            Thread.Sleep(1000);
            while (mResourceApi.OntologyNameWithoutExtension != pOntologia)
            {
                mResourceApi.ChangeOntoly(pOntologia);
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Tesauro.
        /// </summary>
        /// <param name="articulos"></param>
        /// <param name="pListaRecursosCargar"></param>
        /// <param name="pSource"></param>
        private static void ObtenerTesauroUMDocumentos(List<Articulo> articulos, ref List<SecondaryResource> pListaRecursosCargar, string pSource)
        {
            Dictionary<string, string> listaCategorias = new Dictionary<string, string>();
            foreach (Articulo articulo in articulos)
            {
                if (!listaCategorias.ContainsKey(articulo.AREA))
                {
                    listaCategorias.Add(articulo.AREA, articulo.DESCRI_AREA);
                }
            }

            List<Concept> listConcepts = new List<Concept>();
            foreach (string id in listaCategorias.Keys)
            {
                ConceptEDMA concept = new ConceptEDMA();
                concept.Dc_identifier = id;
                concept.Dc_source = pSource;
                concept.Skos_prefLabel = listaCategorias[id];
                concept.Skos_symbol = "1";
                listConcepts.Add(concept);
            }

            CollectionEDMA collection = new CollectionEDMA();
            collection.Dc_source = pSource;
            collection.Skos_member = listConcepts;
            collection.Skos_scopeNote = "Tesauro UM";
            pListaRecursosCargar.Add(collection.ToGnossApiResource(mResourceApi, "0"));

            foreach (ConceptEDMA concept in listConcepts)
            {
                pListaRecursosCargar.Add(concept.ToGnossApiResource(mResourceApi, concept.Dc_identifier));
            }
        }

        /// <summary>
        /// TODO: Tesauro UNESCO.
        /// </summary>
        /// <param name="articulos"></param>
        /// <param name="pListaRecursosCargar"></param>
        /// <param name="pSource"></param>
        private static void ObtenerTesauroUMUnesco(List<CodigosUnesco> codigosUnesco, ref List<SecondaryResource> pListaRecursosCargar, string pSource)
        {
            Dictionary<string, string> listaCategorias = new Dictionary<string, string>();
            foreach (CodigosUnesco codigo in codigosUnesco)
            {
                string codigoUnesco = codigo.UNES_UNAR_CODIGO;
                if (!string.IsNullOrEmpty(codigo.UNES_UNCA_CODIGO) && codigo.UNES_UNCA_CODIGO != "00")
                {
                    codigoUnesco += $@"_{codigo.UNES_UNCA_CODIGO}";
                }
                if (!string.IsNullOrEmpty(codigo.UNES_CODIGO) && codigo.UNES_CODIGO != "00")
                {
                    codigoUnesco += $@"_{codigo.UNES_CODIGO}";
                }

                if (!listaCategorias.ContainsKey(codigo.UNES_NOMBRE))
                {
                    listaCategorias.Add(codigo.UNES_NOMBRE, codigoUnesco);
                }
            }

            List<Concept> listConcepts = new List<Concept>();
            foreach (string id in listaCategorias.Keys)
            {
                ConceptEDMA concept = new ConceptEDMA();
                concept.Dc_identifier = id;
                concept.Dc_source = pSource;
                concept.Skos_prefLabel = listaCategorias[id];
                concept.Skos_symbol = "1";
                listConcepts.Add(concept);
            }

            CollectionEDMA collection = new CollectionEDMA();
            collection.Dc_source = pSource;
            collection.Skos_member = listConcepts;
            collection.Skos_scopeNote = "Tesauro UM";
            pListaRecursosCargar.Add(collection.ToGnossApiResource(mResourceApi, "0"));

            foreach (ConceptEDMA concept in listConcepts)
            {
                pListaRecursosCargar.Add(concept.ToGnossApiResource(mResourceApi, concept.Dc_identifier));
            }
        }


        /// <summary>
        /// Proceso de obtención de datos de las Personas.
        /// </summary>
        /// <param name="pPersonasACargar">IDs de las personas que se quieran cargar. Si viene vacío, se cargan todas.</param>
        /// <param name="pListaRecursosCargar">Lista de recursos a cargar.</param>
        /// <param name="pPersonas">Datos de las personas.</param>
        /// <param name="pAutoresArticulos">Datos de los artículos.</param>
        /// <returns>Diccionario con el ID persona / ID recurso.</returns>
        private static Dictionary<string, string> ObtenerPersonas(HashSet<string> pPersonasACargar, ref List<ComplexOntologyResource> pListaRecursosCargar, List<Persona> pPersonas, List<AutorArticulo> pAutoresArticulos, List<AutorCongreso> pAutoresCongreso, List<AutorExposicion> pAutoresExposicion, List<DirectoresTesis> pDirectoresTesis, List<EquipoProyecto> pEquiposProyectos, List<InventoresPatentes> pInventoresPatentes, Dictionary<string, string> pOrganizacionesCargar, List<DatoEquipoInvestigacion> pListaDatoEquipoInvestigacions)
        {
            Dictionary<string, string> dicIDs = new Dictionary<string, string>();
            HashSet<string> listaPersonasCargarDefinitiva = new HashSet<string>();

            if (pPersonasACargar == null || pPersonasACargar.Count == 0)
            {
                //Si viene vacía la lista de personas, cargamos todas.
                listaPersonasCargarDefinitiva = new HashSet<string>(pPersonas.Select(x => x.IDPERSONA));
            }
            else
            {
                //Cargamos las personas de la lista.
                listaPersonasCargarDefinitiva.UnionWith(pPersonasACargar);

                #region --- Personas que han participado en los mismos artículos.
                HashSet<string> idsArticulos = new HashSet<string>();
                foreach (string personaID in pPersonasACargar)
                {
                    idsArticulos.UnionWith(pAutoresArticulos.Where(x => x.IDPERSONA == personaID).Select(x => x.ARTI_CODIGO));
                }
                listaPersonasCargarDefinitiva.UnionWith(pAutoresArticulos.Where(x => idsArticulos.Contains(x.ARTI_CODIGO)).Select(x => x.IDPERSONA));
                #endregion

                #region --- Personas que han participado en los mismos congresos.
                HashSet<string> idsCongreso = new HashSet<string>();
                foreach (string personaID in pPersonasACargar)
                {
                    idsCongreso.UnionWith(pAutoresCongreso.Where(x => x.IDPERSONA == personaID).Select(x => x.CONG_NUMERO));
                }
                listaPersonasCargarDefinitiva.UnionWith(pAutoresCongreso.Where(x => idsCongreso.Contains(x.CONG_NUMERO)).Select(x => x.IDPERSONA));
                #endregion

                #region --- Personas que han participado en las mismas exposiciones.
                HashSet<string> idsExposiciones = new HashSet<string>();
                foreach (string personaID in pPersonasACargar)
                {
                    idsExposiciones.UnionWith(pAutoresExposicion.Where(x => x.IDPERSONA == personaID).Select(x => x.EXPO_CODIGO));
                }
                listaPersonasCargarDefinitiva.UnionWith(pAutoresExposicion.Where(x => idsExposiciones.Contains(x.EXPO_CODIGO)).Select(x => x.IDPERSONA));
                #endregion

                #region --- Personas que han participado en las mismas tesis.
                HashSet<string> idsDirectoresTesis = new HashSet<string>();
                foreach (string personaID in pPersonasACargar)
                {
                    idsDirectoresTesis.UnionWith(pDirectoresTesis.Where(x => x.IDPERSONADIRECTOR == personaID).Select(x => x.CODIGO_TESIS));
                }
                listaPersonasCargarDefinitiva.UnionWith(pDirectoresTesis.Where(x => idsDirectoresTesis.Contains(x.CODIGO_TESIS)).Select(x => x.IDPERSONADIRECTOR));
                #endregion

                #region --- Personas que han participado en los mismos proyectos.
                HashSet<string> idsProyectos = new HashSet<string>();
                foreach (string personaID in pPersonasACargar)
                {
                    idsProyectos.UnionWith(pEquiposProyectos.Where(x => x.IDPERSONA == personaID).Select(x => x.IDPROYECTO));
                }
                listaPersonasCargarDefinitiva.UnionWith(pEquiposProyectos.Where(x => idsProyectos.Contains(x.IDPROYECTO)).Select(x => x.IDPERSONA));
                #endregion

                #region --- Personas que han participado en los mismas patentes.
                HashSet<string> idsPatentes = new HashSet<string>();
                foreach (string personaID in pPersonasACargar)
                {
                    idsPatentes.UnionWith(pInventoresPatentes.Where(x => x.IDPERSONAINVENTOR == personaID).Select(x => x.IDPATENTE));
                }
                listaPersonasCargarDefinitiva.UnionWith(pInventoresPatentes.Where(x => idsPatentes.Contains(x.IDPATENTE)).Select(x => x.IDPERSONAINVENTOR));
                #endregion

                #region --- Personas que han participado en los mismos grupos.
                HashSet<string> idsGrupos = new HashSet<string>();
                foreach (string personaID in pPersonasACargar)
                {
                    idsGrupos.UnionWith(pListaDatoEquipoInvestigacions.Where(x => x.IDPERSONA == personaID).Select(x => x.IDGRUPOINVESTIGACION));
                }
                idsGrupos.Remove("INVES/POSTGRADO");
                listaPersonasCargarDefinitiva.UnionWith(pListaDatoEquipoInvestigacions.Where(x => idsGrupos.Contains(x.IDGRUPOINVESTIGACION)).Select(x => x.IDPERSONA));
                #endregion
            }

            foreach (string idPersona in listaPersonasCargarDefinitiva)
            {
                Persona persona = pPersonas.FirstOrDefault(x => x.IDPERSONA == idPersona);
                if (persona != null)
                {
                    //Agregamos las propiedades con los datos pertinentes.
                    PersonOntology.Person personaCarga = new PersonOntology.Person();

                    string[] partesNombre = persona.NOMBRE.Split(' ').ToArray();
                    string apellidos = string.Empty;
                    foreach (string palabra in partesNombre.Skip(1))
                    {
                        apellidos += palabra + " ";
                    }

                    personaCarga.Foaf_name = ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(persona.NOMBRE);
                    personaCarga.Foaf_firstName = ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(partesNombre[0]);
                    personaCarga.Foaf_lastName = ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(apellidos.Trim());
                    personaCarga.Roh_crisIdentifier = persona.NUMERODOCUMENTO;
                    if (persona.PERSONAL_UMU == "S")
                    {
                        personaCarga.IdRoh_hasRole = pOrganizacionesCargar["UMU"];
                    }
                    personaCarga.Roh_isSynchronized = false;
                    personaCarga.IdVivo_departmentOrSchool = $@"http://gnoss.com/items/department_{persona.PERS_DEPT_CODIGO}";

                    // ---------- ÑAPA
                    if (idPersona == "79")
                    {
                        personaCarga.Roh_hasPosition = "Catedrático de universidad";
                        personaCarga.IdVivo_departmentOrSchool = "Ingeniería de la información y las comunicaciones";
                        personaCarga.Roh_h_index = 55;
                        personaCarga.Roh_ORCID = "0000-0002-5525-1259";
                        personaCarga.Foaf_homepage = "https://curie.um.es/curie/servlet/um.curie.ginvest.ControlGrinvest?accion=fichainvestigador&dept_codigo=E096&grin_codigo=02&grin_nombre=SISTEMAS%20INTELIGENTES%20Y%20TELEM%C3%uFFFDTICA&d=EA641347CE5593612D1D3BB52DFCCBAD";
                        personaCarga.Vcard_email = "skarmeta@um.es";
                        personaCarga.Vcard_hasTelephone = "+34868884607";
                        personaCarga.Vcard_address = "Despacho 1.09, Facultad de Informática, Campus de Espinardo, 30100 Murcia";
                        personaCarga.Vivo_description = $@"Nacido en Santiago de Chile en Abril de 1965, obtuvo la licenciatura en Informática por la Universidad de Granada en el año 1991
y el Doctorado en Informática por la Universidad de Murcia en el año 1995. Se convirtió en profesor titular de la Universidad de Murcia en al año 1997
y es Catedrático en Ingeniería Telemática desde el año 2009. Vicedecano de Infraestructuras de la Facultad de Informática entre los años 1991 a 1993 y,
posteriormente, vicedecano de Relaciones Externas del 1997 a 1999. Ha sido director del departamento de Ingeniería de la Información y las
Comunicaciones desde 2001 hasta 2007. Coordinador del programa de doctorado en Tecnologías de la Información y Telemáticas Avanzadas desde el 2002
hasta el 2010, con mención de calidad del Ministerio. Desde el año 2008 es coordinador de la Oficina de Proyectos Europeos de la Universidad de Murcia,
adscrita al vicerrectorado de Investigación. Desde la misma ha sido responsable del proyecto de Eurociencia concedido a la Universidad de Murcia,
así como del proyecto COFUND concedido asociado al proyecto U-IMPACT-COFUND que ha coordinado como investigador principal.

Actualmente es, además, representante nacional del MINECO en el programa H2020 para el pilar de Ciencia Excelente en el área de MARIE SKŁODOWSKA-CURIE,
así como representante en el grupo de Recursos Humanos y movilidad de la Comisión Europea.

Antonio Skarmeta es autor de más de 100 publicaciones en revistas internacionales, 200 artículos en congresos y ha sido investigador principal de más
de 15 proyectos europeos, habiendo dirigido más de 20 tesis de doctorado. Además, ha participado en numerosos comités de programas de conferencias en informática,
seguridad, redes móviles e internet de las cosas como Adhoc NoW, IEEE SMC, ACM Group, IEEE MSN, TrustBus, etc., siendo co-chair del Second International
Conference on Dependability DEPEND 2009, y chair del Workshop on Research and Deployment Possibilities based on MIPv6 in the 16th IST Mobile & Wireless
Communications Summit, 2007. Además es editor asociado de la revista IEEE Trans SMC.Part B y ha participado como editor en diversos special issue como los
de IEE Proc. Communication, IJIPT journal, Computer Networks y International Journal of Web and Grid Services.";
                    }
                    else if (idPersona == "1276")
                    {
                        personaCarga.Roh_crisIdentifier = "27443184";
                        personaCarga.Roh_hasPosition = "Catedrático de universidad";
                        personaCarga.IdVivo_departmentOrSchool = "Matemáticas";
                        personaCarga.Roh_h_index = 51;
                        personaCarga.Roh_ORCID = "0000-0003-4550-0183";
                        //personaCarga.Foaf_homepage = "https://curie.um.es/curie/servlet/um.curie.ginvest.ControlGrinvest?accion=fichainvestigador&dept_codigo=E096&grin_codigo=02&grin_nombre=SISTEMAS%20INTELIGENTES%20Y%20TELEM%C3%uFFFDTICA&d=EA641347CE5593612D1D3BB52DFCCBAD";
                        personaCarga.Vcard_email = "fem@um.es";
                        //personaCarga.Vcard_hasTelephone = "+34868884607";
                        //personaCarga.Vcard_address = "Despacho 1.09, Facultad de Informática, Campus de Espinardo, 30100 Murcia";
                        personaCarga.Vivo_description = $@"Catedrático de Análisis Matemático en el Departamento de Matemáticas, desempeña sus labores docentes en la Facultad de Matemáticas, entre las que cabe destacar la asignatura de Laboratorio de Modelización y la coordinación de las prácticas externas del grado en Matemáticas.

Su investigación se centra en la Modelización Matemática y la Simulación, así como en la Enseñanza Asistida por Ordenador. Ha participado en siete proyectos regionales, nueve nacionales y diez internacionales. Es miembro de las organizaciones internacionales Multimedia in Physics Teaching and Learning, que co-preside, GIREP y del proyecto Open Source Physics (EE. UU.).

Su trabajo, en colaboración con investigadores de otros países, ha recibido varios premios internacionales: SPORE Prize de la revista Science en 2011, MPTL Excellence Award en 2015 y UNESCO King Hamad Bin Isa Al-Khalifa Prize en 2016.

Ha sido dos años asesor de la Consejería de Cultura y Educación, dos años Director General de Universidades de la Comunidad Autónoma de la Región de Murcia y Patrono Apoderado de la Fundación Séneca, Agencia Regional de Investigación, cuatro años director de la OTRI de la Universidad de Murcia, cuatro años miembro de la Comisión de Investigación y ocho años decano de la Facultad de Matemáticas.";
                    }

                    //Creamos el recurso.
                    ComplexOntologyResource resource = personaCarga.ToGnossApiResource(mResourceApi, new List<string>());
                    pListaRecursosCargar.Add(resource);

                    //Guardamos los IDs en el diccionario.
                    dicIDs.Add(persona.IDPERSONA, resource.GnossId);
                }
            }

            return dicIDs;
        }

        /// <summary>
        /// Proceso de obtención de datos de las Organizaciones.
        /// </summary>
        /// <param name="pPersonasACargar"></param>
        /// <param name="pListaRecursosCargar"></param>
        /// <param name="pEquiposProyectos"></param>
        /// <param name="pOrganizaciones"></param>
        /// <param name="pFuentesFinanciacionProy"></param>
        /// <returns>Diccionario con el ID organización / ID recurso.</returns>
        private static Dictionary<string, string> ObtenerOrganizaciones(HashSet<string> pPersonasACargar, ref List<ComplexOntologyResource> pListaRecursosCargar, List<EquipoProyecto> pEquiposProyectos, List<OrganizacionesExternas> pOrganizaciones, List<FuentesFinanciacionProyectos> pFuentesFinanciacionProy)
        {
            Dictionary<string, string> dicIDs = new Dictionary<string, string>();
            HashSet<string> idsProyectosCargar = new HashSet<string>();

            if (pPersonasACargar == null || pPersonasACargar.Count == 0)
            {
                //Si viene vacía la lista de personas, cargamos todas.
                idsProyectosCargar = new HashSet<string>(pEquiposProyectos.Select(x => x.IDPROYECTO));
            }
            else
            {
                //Obtengo los IDs de los proyectos.
                HashSet<string> idsProyectos = new HashSet<string>();
                foreach (string personaID in pPersonasACargar)
                {
                    idsProyectosCargar.UnionWith(pEquiposProyectos.Where(x => x.IDPERSONA == personaID).Select(x => x.IDPROYECTO));
                }
            }

            //Obtengo las organizaciones de dichos proyectos.
            foreach (string proyectoID in idsProyectosCargar)
            {
                OrganizacionesExternas organizacion = pOrganizaciones.FirstOrDefault(x => x.IDPROYECTO == proyectoID);
                if (organizacion != null)
                {
                    //Agregamos las propiedades con los datos pertinentes.
                    OrganizationOntology.Organization organizacionCargar = new OrganizationOntology.Organization();
                    organizacionCargar.Roh_title = organizacion.ENTIDAD;

                    //Guardamos los IDs en la lista.
                    if (!dicIDs.ContainsKey(organizacion.ENTIDAD))
                    {
                        //Creamos el recurso.
                        ComplexOntologyResource resource = organizacionCargar.ToGnossApiResource(mResourceApi, new List<string>());
                        pListaRecursosCargar.Add(resource);

                        dicIDs.Add(organizacion.ENTIDAD, resource.GnossId);
                    }
                }
            }

            //Agregamos la Organización de la Universidad de Murcia
            {
                OrganizationOntology.Organization organizacionCargar = new OrganizationOntology.Organization();
                organizacionCargar.Roh_title = "Universidad de Murcia";
                organizacionCargar.Roh_crisIdentifier = "UMU";
                organizacionCargar.IdDc_type = "http://gnoss.com/items/organizationtype_000";
                ComplexOntologyResource resource = organizacionCargar.ToGnossApiResource(mResourceApi, new List<string>());
                pListaRecursosCargar.Add(resource);
                dicIDs.Add("UMU", resource.GnossId);
            }

            //Agregamos las Organizaciones de las Fuentes de Financiación
            Dictionary<string, string> dicOrganizacionFuente = new Dictionary<string, string>();
            foreach (FuentesFinanciacionProyectos fuente in pFuentesFinanciacionProy)
            {
                if (!dicOrganizacionFuente.ContainsKey(fuente.AYFI_FUEN_CODIGO))
                {
                    dicOrganizacionFuente.Add(fuente.AYFI_FUEN_CODIGO, fuente.FUEN_NOMBRE);
                }
            }

            foreach (KeyValuePair<string, string> organizacion in dicOrganizacionFuente)
            {
                //Guardamos los IDs en la lista.
                if (!dicIDs.ContainsKey(organizacion.Value))
                {
                    OrganizationOntology.Organization organizacionCargar = new OrganizationOntology.Organization();
                    organizacionCargar.Roh_title = organizacion.Value;
                    organizacionCargar.Roh_crisIdentifier = organizacion.Key;
                    ComplexOntologyResource resource = organizacionCargar.ToGnossApiResource(mResourceApi, new List<string>());
                    pListaRecursosCargar.Add(resource);
                    dicIDs.Add(organizacion.Value, resource.GnossId);
                }
            }

            return dicIDs;
        }

        /// <summary>
        /// Proceso de obtención de datos de los Proyectos.
        /// </summary>
        /// <param name="pPersonasACargar"></param>
        /// <param name="pDicPersonasCargadas"></param>
        /// <param name="pDicOrganizacionesCargadas"></param>
        /// <param name="pListaRecursosCargar"></param>
        /// <param name="pEquiposProyectos"></param>
        /// <param name="pProyectos"></param>
        /// <param name="pOrganizacionesExternas"></param>
        /// <param name="pFechaProyectos"></param>
        /// <returns>Diccionario con el ID proyecto / ID recurso.</returns>
        private static Dictionary<string, string> ObtenerProyectos(HashSet<string> pPersonasACargar, Dictionary<string, string> pDicPersonasCargadas, Dictionary<string, string> pDicOrganizacionesCargadas, ref List<ComplexOntologyResource> pListaRecursosCargar, List<EquipoProyecto> pEquiposProyectos, List<Proyecto> pProyectos, List<OrganizacionesExternas> pOrganizacionesExternas, List<FechaProyecto> pFechaProyectos, List<FechaEquipoProyecto> pFechaEquipoProyectos, List<AreasUnescoProyectos> pAreasUnesco, List<CodigosUnesco> pCodigosUnesco, List<FuentesFinanciacionProyectos> pFuentesFinanciacionProy)
        {
            Dictionary<string, string> dicIDs = new Dictionary<string, string>();
            HashSet<string> idsProyectosCargar = new HashSet<string>();

            if (pPersonasACargar == null || pPersonasACargar.Count == 0)
            {
                //Si viene vacía la lista de personas, cargamos todas.
                idsProyectosCargar = new HashSet<string>(pEquiposProyectos.Select(x => x.IDPROYECTO));
            }
            else
            {
                //Obtengo los IDs de los proyectos.
                HashSet<string> idsProyectos = new HashSet<string>();
                foreach (string personaID in pPersonasACargar)
                {
                    idsProyectosCargar.UnionWith(pEquiposProyectos.Where(x => x.IDPERSONA == personaID).Select(x => x.IDPROYECTO));
                }
            }

            //Obtengo las organizaciones de dichos proyectos.
            foreach (string proyectoID in idsProyectosCargar)
            {
                Proyecto proyecto = pProyectos.FirstOrDefault(x => x.IDPROYECTO == proyectoID);
                if (proyecto != null)
                {
                    //Agregamos las propiedades con los datos pertinentes.
                    ProjectOntology.Project proyectoCargar = new ProjectOntology.Project();
                    proyectoCargar.Roh_crisIdentifier = proyecto.IDPROYECTO;
                    proyectoCargar.Roh_title = proyecto.NOMBRE;
                    proyectoCargar.Roh_researchersNumber = pEquiposProyectos.Where(x => x.IDPROYECTO == proyecto.IDPROYECTO).Count();
                    proyectoCargar.Vivo_relates = new List<ProjectOntology.BFO_0000023>();

                    //Fechas.
                    foreach (FechaProyecto fechaProyecto in pFechaProyectos.Where(x => x.IDPROYECTO == proyecto.IDPROYECTO))
                    {
                        DateTime fechaInicio = new DateTime();
                        DateTime fechaFin = new DateTime();

                        if (!string.IsNullOrEmpty(fechaProyecto.FECHAINICIOPROYECTO))
                        {
                            int anio = Int32.Parse(fechaProyecto.FECHAINICIOPROYECTO.Substring(0, 4));
                            int mes = Int32.Parse(fechaProyecto.FECHAINICIOPROYECTO.Substring(5, 2));
                            int dia = Int32.Parse(fechaProyecto.FECHAINICIOPROYECTO.Substring(8, 2));
                            int horas = Int32.Parse(fechaProyecto.FECHAINICIOPROYECTO.Substring(11, 2));
                            int minutos = Int32.Parse(fechaProyecto.FECHAINICIOPROYECTO.Substring(14, 2));
                            int segundos = Int32.Parse(fechaProyecto.FECHAINICIOPROYECTO.Substring(17, 2));
                            fechaInicio = new DateTime(anio, mes, dia, horas, minutos, segundos);
                            proyectoCargar.Vivo_start = fechaInicio;
                        }

                        if (!string.IsNullOrEmpty(fechaProyecto.FECHAFINPROYECTO))
                        {
                            int anio = Int32.Parse(fechaProyecto.FECHAFINPROYECTO.Substring(0, 4));
                            int mes = Int32.Parse(fechaProyecto.FECHAFINPROYECTO.Substring(5, 2));
                            int dia = Int32.Parse(fechaProyecto.FECHAFINPROYECTO.Substring(8, 2));
                            int horas = Int32.Parse(fechaProyecto.FECHAFINPROYECTO.Substring(11, 2));
                            int minutos = Int32.Parse(fechaProyecto.FECHAFINPROYECTO.Substring(14, 2));
                            int segundos = Int32.Parse(fechaProyecto.FECHAFINPROYECTO.Substring(17, 2));
                            fechaFin = new DateTime(anio, mes, dia, horas, minutos, segundos);
                            proyectoCargar.Vivo_end = fechaFin;
                        }

                        if (fechaInicio != DateTime.MinValue && fechaFin != DateTime.MinValue)
                        {
                            DateTime zeroTime = new DateTime(1, 1, 1);
                            TimeSpan diferencia = fechaFin - fechaInicio;
                            proyectoCargar.Roh_durationYears = ((zeroTime + diferencia).Year - 1).ToString();
                            proyectoCargar.Roh_durationMonths = ((zeroTime + diferencia).Month - 1).ToString();
                            proyectoCargar.Roh_durationDays = ((zeroTime + diferencia).Day - 1).ToString();
                        }
                    }

                    //Auxiliar BFO_0000023.
                    foreach (EquipoProyecto equipo in pEquiposProyectos.Where(x => x.IDPROYECTO == proyecto.IDPROYECTO))
                    {
                        ProjectOntology.BFO_0000023 persona = new ProjectOntology.BFO_0000023();
                        if (pDicPersonasCargadas.ContainsKey(equipo.IDPERSONA))
                        {
                            persona.IdRoh_roleOf = pDicPersonasCargadas[equipo.IDPERSONA];
                        }
                        persona.Vivo_preferredDisplayOrder = Int32.Parse(equipo.NUMEROCOLABORADOR);

                        //TODO Tipo de participación.
                        //string tipoParticipacion = pFechaEquipoProyectos.FirstOrDefault(x => x.IDPROYECTO == proyectoID && x.NUMEROCOLABORADOR == equipo.NUMEROCOLABORADOR).CODTIPOPARTICIPACION;
                        //switch (tipoParticipacion)
                        //{
                        //    case "IP":
                        //        persona.IdRoh_participationType = "http://gnoss.com/items/participationtype_050";
                        //        break;
                        //    case "IPRE":
                        //        persona.IdRoh_participationType = "http://gnoss.com/items/participationtype_OTHERS";
                        //        persona.Roh_participationTypeOther = "Investigador principal responsable económico";
                        //        break;
                        //    case "INV":
                        //        persona.IdRoh_participationType = "http://gnoss.com/items/participationtype_060";
                        //        break;
                        //    case "RE":
                        //        persona.IdRoh_participationType = "http://gnoss.com/items/participationtype_OTHERS";
                        //        persona.Roh_participationTypeOther = "Responsable económico";
                        //        break;
                        //}

                        proyectoCargar.Vivo_relates.Add(persona);
                    }

                    //Principal Organization.
                    proyectoCargar.IdsRoh_participates = new List<string>();
                    foreach (OrganizacionesExternas organizacion in pOrganizacionesExternas.Where(x => x.IDPROYECTO == proyecto.IDPROYECTO))
                    {
                        if (pDicOrganizacionesCargadas.ContainsKey(organizacion.ENTIDAD))
                        {
                            proyectoCargar.IdsRoh_participates.Add(pDicOrganizacionesCargadas[organizacion.ENTIDAD]);
                        }
                    }

                    //Organización financiadora.
                    proyectoCargar.IdRoh_conductedBy = pDicOrganizacionesCargadas["UMU"];

                    //Temas de Investigación
                    proyectoCargar.tagList = new List<string>();
                    foreach (AreasUnescoProyectos areas in pAreasUnesco.Where(x => x.IDPROYECTO == proyecto.IDPROYECTO))
                    {
                        foreach (CodigosUnesco codigos in pCodigosUnesco.Where(x => x.UNES_UNAR_CODIGO == areas.UNAR_CODIGO && x.UNES_UNCA_CODIGO == areas.UNCA_CODIGO && x.UNES_CODIGO == x.UNES_CODIGO))
                        {
                            if (!string.IsNullOrEmpty(codigos.UNES_NOMBRE))
                            {
                                proyectoCargar.tagList.Add(codigos.UNES_NOMBRE);
                            }
                        }
                    }

                    //Ámbito geográfico
                    if (!string.IsNullOrEmpty(proyecto.AMBITO_GEOGRAFICO))
                    {
                        switch (proyecto.AMBITO_GEOGRAFICO)
                        {
                            case "REGIONAL":
                                proyectoCargar.IdVivo_geographicFocus = "http://gnoss.com/items/geographicregion_000";
                                break;
                            case "NACIONAL":
                                proyectoCargar.IdVivo_geographicFocus = "http://gnoss.com/items/geographicregion_010";
                                break;
                            case "EUROPEO":
                                proyectoCargar.IdVivo_geographicFocus = "http://gnoss.com/items/geographicregion_020";
                                break;
                            case "INTERNACIONAL":
                                proyectoCargar.IdVivo_geographicFocus = "http://gnoss.com/items/geographicregion_030";
                                break;
                            case "PROPIO":
                                proyectoCargar.IdVivo_geographicFocus = "http://gnoss.com/items/geographicregion_OTHERS";
                                proyectoCargar.Roh_geographicFocusOther = "Propio";
                                break;
                        }
                    }

                    //Organización financiadora
                    proyectoCargar.IdsRoh_grantedBy = new List<string>();
                    HashSet<string> fuentesFinanciacion = new HashSet<string>();
                    foreach (FuentesFinanciacionProyectos fuente in pFuentesFinanciacionProy.Where(x => x.IDPROYECTO == proyecto.IDPROYECTO))
                    {
                        fuentesFinanciacion.Add(fuente.FUEN_NOMBRE);
                    }
                    foreach (string nomFuente in fuentesFinanciacion)
                    {
                        proyectoCargar.IdsRoh_grantedBy.Add(pDicOrganizacionesCargadas[nomFuente]);
                    }

                    //isSynchronized
                    proyectoCargar.Roh_isSynchronized = false;

                    // ---------- ÑAPA
                    if(proyectoID == "solayudas|13334")
                    {
                        proyectoCargar.Vivo_description = $@"El objetivo general del proyecto Hidroleaf es desarrollar y validar un sistema integral para la producción sostenible de plantas hortícolas de hoja en invernadero y en cultivo de interior mediante luz artificial, aplicando las nuevas tecnologías TICs para optimizar las condiciones de cultivo de las plantas.



En concreto fruto de este proyecto se desarrollarán fábricas de cultivos de hortalizas en contenedores inteligentes. La idea es reconvertir contenedores de mercancías para ser usados como medios de cultivo. Dentro de dichos contenedores se establecerán las condiciones óptimas para que la producción agrícola se pueda llevar a cabo, controlando los parámetros de humedad, temperatura, luminosidad, etc empleando para ello sistemas de sensorización y automatización, lo que permite encuadrar este proyecto dentro del ámbito de la Industria 4.0.";
                        proyectoCargar.Roh_isSupportedBy = new ProjectOntology.Funding();
                        proyectoCargar.Roh_isSupportedBy.Roh_fundedBy = new List<ProjectOntology.FundingProgram>();
                        proyectoCargar.Roh_isSupportedBy.Roh_fundedBy.Add(new ProjectOntology.FundingProgram { Roh_title = "Programa Estatal de I+D+i Orientada a los Retos de la Sociedad" });
                        proyectoCargar.IdRoh_scientificExperienceProject = "http://gnoss.com/items/scientificexperienceproject_SEP1";
                        proyectoCargar.Vivo_relates = new List<ProjectOntology.BFO_0000023>();
                        ProjectOntology.BFO_0000023 persona = new ProjectOntology.BFO_0000023();
                        persona.IdRoh_roleOf = pDicPersonasCargadas["79"];
                        persona.Vivo_preferredDisplayOrder = 1;
                        proyectoCargar.Vivo_relates.Add(persona);
                        proyectoCargar.Roh_hasResultsProjectClassification = new List<ProjectOntology.ProjectClassification>();
                        //TODO: ProjectClassification
                    }

                    ComplexOntologyResource resource = proyectoCargar.ToGnossApiResource(mResourceApi, new List<string>());
                    pListaRecursosCargar.Add(resource);

                    //Guardamos los IDs en la lista.
                    dicIDs.Add(proyecto.IDPROYECTO, resource.GnossId);
                }
            }

            return dicIDs;
        }

        /// <summary>
        /// Proceso de obtención de los datos de documentos. (Publicaciones)
        /// </summary>
        /// <param name="pPersonasACargar"></param>
        /// <param name="pDicPersonasCargadas"></param>
        /// <param name="pListaRecursosCargar"></param>
        /// <param name="pListaArticulos"></param>
        /// <param name="pListaAutoresArticulos"></param>
        /// <param name="pListaPersonas"></param>
        /// <param name="pListaPalabrasClave"></param>
        /// <returns>Diccionario con el ID documento / ID recurso.</returns>
        private static Dictionary<string, string> ObtenerDocumentos(Dictionary<string, string> pDicProyectosACargar, HashSet<string> pPersonasACargar, Dictionary<string, string> pDicPersonasCargadas, Dictionary<string, string> pDicRevistasCargadas, ref List<ComplexOntologyResource> pListaRecursosCargar, List<Articulo> pListaArticulos, List<AutorArticulo> pListaAutoresArticulos, List<Persona> pListaPersonas, List<PalabrasClaveArticulos> pListaPalabrasClave, List<Proyecto> pListaProyectos, List<EquipoProyecto> pEquipoProyectos)
        {
            Dictionary<string, string> dicIDs = new Dictionary<string, string>();
            HashSet<string> idsArticulosACargar = new HashSet<string>();

            if (pPersonasACargar == null || pPersonasACargar.Count == 0)
            {
                //Si viene vacía la lista de personas, cargamos todas.
                idsArticulosACargar = new HashSet<string>(pListaArticulos.Select(x => x.CODIGO));
            }
            else
            {
                //Obtengo los IDs de los artículos.
                HashSet<string> idsArticulos = new HashSet<string>();
                foreach (string personaID in pPersonasACargar)
                {
                    idsArticulosACargar.UnionWith(pListaAutoresArticulos.Where(x => x.IDPERSONA == personaID).Select(x => x.ARTI_CODIGO));
                }
            }

            //Obtengo los datos de los documentos.
            foreach (string documentoID in idsArticulosACargar)
            {
                Articulo articulo = pListaArticulos.FirstOrDefault(x => x.CODIGO == documentoID);

                if (articulo != null)
                {
                    DocumentOntology.Document documentoACargar = new DocumentOntology.Document();
                    documentoACargar.IdRoh_scientificActivityDocument = "http://gnoss.com/items/scientificactivitydocument_SAD1";
                    documentoACargar.IdDc_type = "http://gnoss.com/items/publicationtype_020";
                    documentoACargar.IdVivo_hasPublicationVenue = pDicRevistasCargadas[articulo.NOMBRE_REVISTA];
                    documentoACargar.Roh_title = articulo.TITULO;
                    documentoACargar.Roh_crisIdentifier = articulo.CODIGO;

                    if (!string.IsNullOrEmpty(articulo.ANO))
                    {
                        documentoACargar.Dct_issued = new DateTime(Int32.Parse(articulo.ANO), 01, 01, 00, 00, 00);
                    }
                    if (!string.IsNullOrEmpty(articulo.PAGDESDE))
                    {
                        documentoACargar.Bibo_pageStart = Int32.Parse(articulo.PAGDESDE);
                    }
                    if (!string.IsNullOrEmpty(articulo.PAGHASTA))
                    {
                        documentoACargar.Bibo_pageEnd = Int32.Parse(articulo.PAGHASTA);
                    }
                    documentoACargar.Bibo_authorList = new List<DocumentOntology.BFO_0000023>();
                    List<AutorArticulo> listaAutores = pListaAutoresArticulos.Where(x => x.ARTI_CODIGO == articulo.CODIGO).ToList();
                    int numAutores = 0;
                    foreach (AutorArticulo autor in listaAutores)
                    {
                        if (pListaPersonas.Exists(x => x.IDPERSONA == autor.IDPERSONA))
                        {
                            //BFO_0000023
                            DocumentOntology.BFO_0000023 persona = new DocumentOntology.BFO_0000023();
                            persona.IdRdf_member = pDicPersonasCargadas[autor.IDPERSONA];
                            persona.Rdf_comment = Int32.Parse(autor.ORDEN);
                            persona.Foaf_nick = ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(pListaPersonas.FirstOrDefault(x => x.IDPERSONA == autor.IDPERSONA).NOMBRE);
                            documentoACargar.Bibo_authorList.Add(persona);

                            numAutores++;

                            //Proyectos
                            //TODO cambiar
                            List<EquipoProyecto> listaEquipos = pEquipoProyectos.Where(x => x.IDPERSONA == autor.IDPERSONA).ToList();
                            foreach (EquipoProyecto equipo in listaEquipos)
                            {
                                int anyIncio = 0;
                                int anyFin = DateTime.Now.Year;
                                if (!string.IsNullOrEmpty(equipo.FECHAINICIO))
                                {
                                    anyIncio = Int32.Parse(equipo.FECHAINICIO.Split('/')[0]);
                                }
                                if (!string.IsNullOrEmpty(equipo.FECHAFIN))
                                {
                                    anyFin = Int32.Parse(equipo.FECHAFIN.Split('/')[0]);
                                }
                                if (!string.IsNullOrEmpty(articulo.ANO) && Int32.Parse(articulo.ANO) >= anyIncio && Int32.Parse(articulo.ANO) <= anyFin)
                                {
                                    if (pDicProyectosACargar.ContainsKey(equipo.IDPROYECTO))
                                    {
                                        documentoACargar.IdRoh_project = pDicProyectosACargar[equipo.IDPROYECTO];
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    documentoACargar.Roh_authorsNumber = numAutores;

                    if (!string.IsNullOrEmpty(articulo.AREA))
                    {
                        documentoACargar.Roh_hasKnowledgeArea = new List<DocumentOntology.CategoryPath>();
                        DocumentOntology.CategoryPath categoryPath = new DocumentOntology.CategoryPath();
                        categoryPath.IdsRoh_categoryNode = new List<string>() { "http://gnoss.com/items/um_" + articulo.AREA };
                        documentoACargar.Roh_hasKnowledgeArea.Add(categoryPath);
                    }

                    //Localidad
                    documentoACargar.Vcard_locality = "Murcia";
                    documentoACargar.IdVcard_hasRegion = "http://gnoss.com/items/feature_ADM1_ES62";
                    documentoACargar.IdVcard_hasCountryName = "http://gnoss.com/items/feature_PCLD_724";

                    //FreeTextKeyword
                    documentoACargar.Vivo_freeTextKeyword = pListaPalabrasClave.Where(x => x.PC_ARTI_CODIGO == articulo.CODIGO).Select(x => x.PC_PALABRA).ToList();

                    //DOI
                    if (!string.IsNullOrEmpty(articulo.ARTI_DOI))
                    {
                        documentoACargar.Bibo_identifier = new List<DocumentOntology.fDocument>();
                        DocumentOntology.fDocument docu = new DocumentOntology.fDocument();
                        docu.IdFoaf_primaryTopic = "http://gnoss.com/items/publicationidentifiertype_040";
                        docu.Dc_title = LimpiarDoi(articulo.ARTI_DOI);
                        documentoACargar.Bibo_identifier.Add(docu);
                    }

                    //Validación.
                    documentoACargar.Roh_isValidated = true;

                    //Creamos el recurso.
                    ComplexOntologyResource resource = documentoACargar.ToGnossApiResource(mResourceApi, new List<string>());
                    pListaRecursosCargar.Add(resource);

                    //Guardamos los IDs en la lista.
                    dicIDs.Add(articulo.CODIGO, resource.GnossId);
                }
            }

            return dicIDs;
        }

        private static Dictionary<string, string> ObtenerCongresos(Dictionary<string, string> pDicProyectosACargar, HashSet<string> pPersonasACargar, Dictionary<string, string> pDicPersonasCargadas, Dictionary<string, string> pDicRevistasCargadas, ref List<ComplexOntologyResource> pListaRecursosCargar, List<Congreso> pListaCongresos, List<AutorCongreso> pListaAutoresCongreso, List<Persona> pListaPersonas, List<EquipoProyecto> pListaEquipoProyectos)
        {
            Dictionary<string, string> dicIDs = new Dictionary<string, string>();
            HashSet<string> idsCongresosACargar = new HashSet<string>();

            if (pPersonasACargar == null || pPersonasACargar.Count == 0)
            {
                //Si viene vacía la lista de personas, cargamos todas.
                idsCongresosACargar = new HashSet<string>(pListaCongresos.Select(x => x.CONG_NUMERO));
            }
            else
            {
                //Obtengo los IDs de los artículos.
                HashSet<string> idsArticulos = new HashSet<string>();
                foreach (string personaID in pPersonasACargar)
                {
                    idsCongresosACargar.UnionWith(pListaAutoresCongreso.Where(x => x.IDPERSONA == personaID).Select(x => x.CONG_NUMERO));
                }
            }

            //Obtengo los datos de los documentos.
            foreach (string documentoID in idsCongresosACargar)
            {
                Congreso congreso = pListaCongresos.FirstOrDefault(x => x.CONG_NUMERO == documentoID);

                if (congreso != null)
                {
                    DocumentOntology.Document documentoACargar = new DocumentOntology.Document();
                    documentoACargar.IdRoh_scientificActivityDocument = "http://gnoss.com/items/scientificactivitydocument_SAD2";
                    documentoACargar.Roh_title = congreso.TITULO_CONTRIBUCION;
                    documentoACargar.Vcard_locality = congreso.LUGAR_CELEBRACION;

                    //Fecha
                    if (!string.IsNullOrEmpty(congreso.FECHA_CELEBRACION))
                    {
                        DateTime fecha = new DateTime();
                        int anio = Int32.Parse(congreso.FECHA_CELEBRACION.Substring(0, 4));
                        int mes = Int32.Parse(congreso.FECHA_CELEBRACION.Substring(5, 2));
                        int dia = Int32.Parse(congreso.FECHA_CELEBRACION.Substring(8, 2));
                        int horas = Int32.Parse(congreso.FECHA_CELEBRACION.Substring(11, 2));
                        int minutos = Int32.Parse(congreso.FECHA_CELEBRACION.Substring(14, 2));
                        int segundos = Int32.Parse(congreso.FECHA_CELEBRACION.Substring(17, 2));
                        fecha = new DateTime(anio, mes, dia, horas, minutos, segundos);
                        documentoACargar.Dct_issued = fecha;
                    }

                    documentoACargar.Bibo_authorList = new List<DocumentOntology.BFO_0000023>();
                    List<AutorCongreso> listaAutores = pListaAutoresCongreso.Where(x => x.CONG_NUMERO == congreso.CONG_NUMERO).ToList();
                    int numAutores = 0;
                    foreach (AutorCongreso autor in listaAutores)
                    {
                        if (pListaPersonas.Exists(x => x.IDPERSONA == autor.IDPERSONA))
                        {
                            //BFO_0000023
                            DocumentOntology.BFO_0000023 persona = new DocumentOntology.BFO_0000023();
                            persona.IdRdf_member = pDicPersonasCargadas[autor.IDPERSONA];
                            persona.Rdf_comment = Int32.Parse(autor.ORDEN);
                            persona.Foaf_nick = ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(pListaPersonas.FirstOrDefault(x => x.IDPERSONA == autor.IDPERSONA).NOMBRE);
                            documentoACargar.Bibo_authorList.Add(persona);
                            numAutores++;

                            //TODO: Proyectos
                            List<EquipoProyecto> listaEquipos = pListaEquipoProyectos.Where(x => x.IDPERSONA == autor.IDPERSONA).ToList();
                            foreach (EquipoProyecto equipo in listaEquipos)
                            {
                                int anyIncio = 0;
                                int anyFin = DateTime.Now.Year;
                                if (!string.IsNullOrEmpty(equipo.FECHAINICIO))
                                {
                                    anyIncio = Int32.Parse(equipo.FECHAINICIO.Split('/')[0]);
                                }
                                if (!string.IsNullOrEmpty(equipo.FECHAFIN))
                                {
                                    anyFin = Int32.Parse(equipo.FECHAFIN.Split('/')[0]);
                                }
                                if (!string.IsNullOrEmpty(congreso.FECHA_CELEBRACION) && Int32.Parse(congreso.FECHA_CELEBRACION.Substring(0, 4)) >= anyIncio && Int32.Parse(congreso.FECHA_CELEBRACION.Substring(0, 4)) <= anyFin)
                                {
                                    if (pDicProyectosACargar.ContainsKey(equipo.IDPROYECTO))
                                    {
                                        documentoACargar.IdRoh_project = pDicProyectosACargar[equipo.IDPROYECTO];
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    documentoACargar.Roh_authorsNumber = numAutores;

                    //Evento
                    documentoACargar.Bibo_presentedAt = new DocumentOntology.Event();
                    documentoACargar.Bibo_presentedAt.Dc_title = congreso.TITULO_CONGRESO;

                    //Creamos el recurso.
                    ComplexOntologyResource resource = documentoACargar.ToGnossApiResource(mResourceApi, new List<string>());
                    pListaRecursosCargar.Add(resource);

                    //Guardamos los IDs en la lista.
                    dicIDs.Add(congreso.CONG_NUMERO, resource.GnossId);
                }                
            }

            return dicIDs;
        }

        private static Dictionary<string, string> ObtenerGrupos(HashSet<string> pPersonasACargar, Dictionary<string, string> pDicPersonasCargadas, ref List<ComplexOntologyResource> pListaRecursosCargar, List<Persona> pListaPersonas, List<GrupoInvestigacion> pListaGrupoInvestigacion, List<DatoEquipoInvestigacion> pListaDatoEquipoInvestigacion, Dictionary<string, string> pOrganizacionesCargar, List<LineasDeInvestigacion> pListaLineasDeInvestigacion, List<LineasInvestigador> pListaLineasInvestigador)
        {
            Dictionary<string, string> dicIDs = new Dictionary<string, string>();
            HashSet<string> idsGruposACargar = new HashSet<string>();

            if (pPersonasACargar == null || pPersonasACargar.Count == 0)
            {
                //Si viene vacía la lista de personas, cargamos todas.
                idsGruposACargar = new HashSet<string>(pListaGrupoInvestigacion.Select(x => x.IDGRUPOINVESTIGACION));
            }
            else
            {
                //Cargamos las personas de la lista.
                foreach (string personaID in pPersonasACargar)
                {
                    idsGruposACargar.UnionWith(pListaDatoEquipoInvestigacion.Where(x => x.IDPERSONA == personaID).Select(x => x.IDGRUPOINVESTIGACION));
                }
            }
            idsGruposACargar.Remove("INVES/POSTGRADO");

            foreach (string grupoID in idsGruposACargar)
            {
                GrupoInvestigacion grupo = pListaGrupoInvestigacion.FirstOrDefault(x => x.IDGRUPOINVESTIGACION == grupoID);

                if (grupo != null)
                {
                    //Agregamos las propiedades con los datos pertinentes.
                    GroupOntology.Group grupoCargar = new GroupOntology.Group();

                    //CrisIdentifier
                    grupoCargar.Roh_crisIdentifier = grupo.IDGRUPOINVESTIGACION;

                    //Title
                    grupoCargar.Roh_title = grupo.DESCRIPCION;

                    //AffiliatedOrganization
                    grupoCargar.IdVivo_affiliatedOrganization = pOrganizacionesCargar["UMU"];

                    //FoundationDate
                    int anio = Int32.Parse(grupo.FECHACREACION.Substring(0, 4));
                    int mes = Int32.Parse(grupo.FECHACREACION.Substring(5, 2));
                    int dia = Int32.Parse(grupo.FECHACREACION.Substring(8, 2));
                    int horas = Int32.Parse(grupo.FECHACREACION.Substring(11, 2));
                    int minutos = Int32.Parse(grupo.FECHACREACION.Substring(14, 2));
                    int segundos = Int32.Parse(grupo.FECHACREACION.Substring(17, 2));
                    DateTime fechaCreacion = new DateTime(anio, mes, dia, horas, minutos, segundos);
                    grupoCargar.Roh_foundationDate = fechaCreacion;

                    //MainResearcher y Member
                    grupoCargar.Roh_mainResearcher = new List<GroupOntology.BFO_0000023>();
                    grupoCargar.Foaf_member = new List<GroupOntology.BFO_0000023>();

                    List<DatoEquipoInvestigacion> equipoInvestigacion = pListaDatoEquipoInvestigacion.Where(x => x.IDGRUPOINVESTIGACION == grupo.IDGRUPOINVESTIGACION).ToList();
                    foreach (DatoEquipoInvestigacion equipo in equipoInvestigacion)
                    {
                        if (pDicPersonasCargadas.ContainsKey(equipo.IDPERSONA))
                        {
                            //BFO_0000023
                            GroupOntology.BFO_0000023 persona = new GroupOntology.BFO_0000023();
                            persona.IdRoh_roleOf = pDicPersonasCargadas[equipo.IDPERSONA];
                            if (!string.IsNullOrEmpty(equipo.FECHAINICIOPERIODO))
                            {
                                anio = Int32.Parse(equipo.FECHAINICIOPERIODO.Substring(0, 4));
                                mes = Int32.Parse(equipo.FECHAINICIOPERIODO.Substring(5, 2));
                                dia = Int32.Parse(equipo.FECHAINICIOPERIODO.Substring(8, 2));
                                horas = Int32.Parse(equipo.FECHAINICIOPERIODO.Substring(11, 2));
                                minutos = Int32.Parse(equipo.FECHAINICIOPERIODO.Substring(14, 2));
                                segundos = Int32.Parse(equipo.FECHAINICIOPERIODO.Substring(17, 2));
                                DateTime fechaInicio = new DateTime(anio, mes, dia, horas, minutos, segundos);
                                persona.Vivo_start = fechaInicio;
                            }
                            if (!string.IsNullOrEmpty(equipo.FECHAFINPERIODO))
                            {
                                anio = Int32.Parse(equipo.FECHAFINPERIODO.Substring(0, 4));
                                mes = Int32.Parse(equipo.FECHAFINPERIODO.Substring(5, 2));
                                dia = Int32.Parse(equipo.FECHAFINPERIODO.Substring(8, 2));
                                horas = Int32.Parse(equipo.FECHAFINPERIODO.Substring(11, 2));
                                minutos = Int32.Parse(equipo.FECHAFINPERIODO.Substring(14, 2));
                                segundos = Int32.Parse(equipo.FECHAFINPERIODO.Substring(17, 2));
                                DateTime fechaFin = new DateTime(anio, mes, dia, horas, minutos, segundos);
                                persona.Vivo_end = fechaFin;
                            }

                            // Líneas de investigación
                            persona.Vivo_hasResearchArea = new List<string>();
                            List<LineasInvestigador> lineasIngestigador = pListaLineasInvestigador.Where(x => x.IDPERSONAINVESTIGADOR == equipo.IDPERSONA).ToList();
                            foreach(LineasInvestigador linea in lineasIngestigador)
                            {
                                LineasDeInvestigacion item = pListaLineasDeInvestigacion.FirstOrDefault(x => x.LINE_CODIGO == linea.LINE_CODIGO);
                                persona.Vivo_hasResearchArea.Add(item.LINE_DESCRIPCION);
                            }

                            if (equipo.CODTIPOPARTICIPACION == "IP")
                            {
                                grupoCargar.Roh_mainResearcher.Add(persona);
                            }
                            else
                            {
                                grupoCargar.Foaf_member.Add(persona);
                            }
                        }
                    }

                    //ResearchersNumber
                    grupoCargar.Roh_researchersNumber = grupoCargar.Roh_mainResearcher.Select(x => x.IdRoh_roleOf).Union(grupoCargar.Foaf_member.Select(x => x.IdRoh_roleOf)).Distinct().Count();

                    // --- ÑAPA
                    if (grupo.IDGRUPOINVESTIGACION == "E096-02")
                    {                        
                        grupoCargar.Vivo_description = $@"El grupo de investigación de Sistemas Inteligentes de la Universidad de Murcia desarrolla su investigación dentro de diversas líneas de investigación de vanguardia de Soft Computing y Sistemas Inteligentes desde 1997.
Actualmente el grupo desarrolla su labor en líneas de investigación como los sistemas embebidos, el telecontrol y la telemática aplicada al transporte, el procesamiento sensorial y la fusión de datos o los sistemas cooperativos inteligentes.
Las técnicas utilizadas por este grupo son: los sistemas automáticos, las redes causales, el soft computing o la minería de datos, entre otros.



Actualmente 78 investigadores forman el grupo, todos ellos miembros del Departamento de Ingeniería de la Información y las Comunicaciones de la Universidad de Murcia.";
                    }

                    //Creamos el recurso.
                    ComplexOntologyResource resource = grupoCargar.ToGnossApiResource(mResourceApi, new List<string>());
                    pListaRecursosCargar.Add(resource);

                    //Guardamos los IDs en la lista.
                    dicIDs.Add(grupo.IDGRUPOINVESTIGACION, resource.GnossId);
                }
            }

            return dicIDs;
        }

        private static Dictionary<string, string> ObtenerCVs(HashSet<string> pPersonasACargar, Dictionary<string, string> pDicPersonasCargadas, Dictionary<string, string> pDicDocumentosCargados, ref List<ComplexOntologyResource> pListaRecursosCargar, List<Articulo> pListaArticulos, List<AutorArticulo> pListaAutoresArticulos, List<Persona> pListaPersonas)
        {
            Dictionary<string, string> dicIDs = new Dictionary<string, string>();
            HashSet<string> idsCVsACargar = new HashSet<string>();

            if (pPersonasACargar == null || pPersonasACargar.Count == 0)
            {
                //Si viene vacía la lista de personas, cargamos todas las de Murcia.
                idsCVsACargar = new HashSet<string>(pListaPersonas.Where(x => x.PERSONAL_UMU == "S").Select(x => x.IDPERSONA));
            }
            else
            {
                //Obtengo los IDs de loas personas de Murcia a Cargar
                idsCVsACargar = new HashSet<string>(pListaPersonas.Where(x => x.PERSONAL_UMU == "S").Select(x => x.IDPERSONA).Intersect(pDicPersonasCargadas.Keys));
            }

            //Obtengo los datos de los documentos.
            foreach (string id in idsCVsACargar)
            {
                Persona persona = pListaPersonas.FirstOrDefault(x => x.IDPERSONA == id);

                if (persona != null && persona.PERSONAL_UMU == "S" && pDicPersonasCargadas.ContainsKey(id))
                {
                    CV cvACargar = new CV();
                    cvACargar.Foaf_name = persona.NOMBRE;
                    cvACargar.IdRoh_cvOf = pDicPersonasCargadas[id];
                    cvACargar.Roh_personalData = new PersonalData();
                    cvACargar.Roh_scientificExperience = new ScientificExperience();
                    cvACargar.Roh_scientificExperience.Roh_title = "-";
                    cvACargar.Roh_scientificActivity = new ScientificActivity();
                    cvACargar.Roh_scientificActivity.Roh_title = "-";

                    //PersonalData
                    if (persona.SEXO == "M")
                    {
                        cvACargar.Roh_personalData.IdFoaf_gender = "http://gnoss.com/items/gender_000";
                    }
                    else if (persona.SEXO == "F")
                    {
                        cvACargar.Roh_personalData.IdFoaf_gender = "http://gnoss.com/items/gender_010";
                    }
                    cvACargar.Roh_personalData.Vcard_email = persona.EMAIL;

                    //ScientificActivity
                    cvACargar.Roh_scientificActivity.Roh_scientificPublications = new List<RelatedScientificPublication>();

                    foreach (string idArticulo in pListaAutoresArticulos.Where(x => x.IDPERSONA == id && pDicDocumentosCargados.ContainsKey(x.ARTI_CODIGO)).Select(x => x.ARTI_CODIGO))
                    {
                        RelatedScientificPublication relatedDocuments = new RelatedScientificPublication();
                        relatedDocuments.IdVivo_relatedBy = pDicDocumentosCargados[idArticulo];
                        relatedDocuments.Roh_isPublic = true;
                        cvACargar.Roh_scientificActivity.Roh_scientificPublications.Add(relatedDocuments);
                    }

                    //Creamos el recurso.
                    ComplexOntologyResource resource = cvACargar.ToGnossApiResource(mResourceApi, new List<string>());
                    pListaRecursosCargar.Add(resource);

                    //Guardamos los IDs en la lista.
                    dicIDs.Add(id, resource.GnossId);
                }
            }

            return dicIDs;
        }

        private static Dictionary<string, string> ObtenerMainDocuments(HashSet<string> pPersonasACargar, ref List<ComplexOntologyResource> pListaRecursosCargar, List<Articulo> pListaArticulos, List<AutorArticulo> pListaAutorArticulos)
        {
            Dictionary<string, string> dicIDs = new Dictionary<string, string>();
            HashSet<string> idsRevistasACargar = new HashSet<string>();
            Dictionary<string, HashSet<string>> tituloRevistas = new Dictionary<string, HashSet<string>>();

            if (pPersonasACargar == null || pPersonasACargar.Count == 0)
            {
                //Si viene vacía la lista de personas, cargamos todas.
                idsRevistasACargar = new HashSet<string>(pListaArticulos.Select(x => x.CODIGO));
            }
            else
            {
                //Cargamos las personas de la lista.
                foreach (string personaID in pPersonasACargar)
                {
                    idsRevistasACargar.UnionWith(pListaAutorArticulos.Where(x => x.IDPERSONA == personaID).Select(x => x.ARTI_CODIGO));
                }
            }

            List<MaindocumentOntology.MainDocument> listaRevistas = new List<MaindocumentOntology.MainDocument>();

            foreach (string id in idsRevistasACargar)
            {
                Articulo articulo = pListaArticulos.FirstOrDefault(x => x.CODIGO == id);
                if (articulo != null)
                {
                    if (!tituloRevistas.ContainsKey(articulo.NOMBRE_REVISTA))
                    {
                        MaindocumentOntology.MainDocument revista = new MaindocumentOntology.MainDocument();
                        revista.Roh_crisIdentifier = articulo.NOMBRE_REVISTA;
                        revista.Roh_title = articulo.NOMBRE_REVISTA;
                        revista.Bibo_issn = articulo.REIS_ISSN;
                        revista.IdRoh_format = "http://gnoss.com/items/documentformat_057"; // De momento, todos son revistas.

                        // Índice de impacto.
                        revista.Roh_impactIndex = new List<MaindocumentOntology.ImpactIndex>();

                        MaindocumentOntology.ImpactIndex indice = new MaindocumentOntology.ImpactIndex();
                        indice.IdRoh_impactSource = "http://gnoss.com/items/referencesource_OTHERS";
                        indice.Roh_impactSourceOther = "Murcia";
                        foreach (KeyValuePair<string, string> item in dicAreasCategoria)
                        {
                            if (item.Value.Contains(articulo.DESCRI_AREA))
                            {
                                indice.IdRoh_impactCategory = item.Key;
                                break;
                            }
                        }

                        indice.Roh_impactIndexInYear = float.Parse(articulo.IMPACTO_REVISTA);

                        if (articulo.CUARTIL_REVISTA == "1")
                        {
                            indice.Roh_journalTop25 = true;
                        }
                        else
                        {
                            indice.Roh_journalTop25 = false;
                        }

                        DateTime fecha = new DateTime();
                        if (!string.IsNullOrEmpty(articulo.ANO))
                        {
                            int anio = Int32.Parse(articulo.ANO);
                            int mes = Int32.Parse("01");
                            int dia = Int32.Parse("01");
                            int horas = Int32.Parse("00");
                            int minutos = Int32.Parse("00");
                            int segundos = Int32.Parse("00");
                            fecha = new DateTime(anio, mes, dia, horas, minutos, segundos);
                            indice.Roh_year = fecha;
                        }


                        revista.Roh_impactIndex.Add(indice);

                        // Guardo el título.
                        tituloRevistas.Add(articulo.NOMBRE_REVISTA, new HashSet<string>() { articulo.ANO });

                        // Guardo la revista.
                        listaRevistas.Add(revista);
                    }
                    else if (!tituloRevistas[articulo.NOMBRE_REVISTA].Contains(articulo.ANO))
                    {
                        MaindocumentOntology.MainDocument revista = listaRevistas.FirstOrDefault(x => x.Roh_title == articulo.NOMBRE_REVISTA);

                        if (revista != null)
                        {
                            // Índice de impacto.
                            MaindocumentOntology.ImpactIndex indice = new MaindocumentOntology.ImpactIndex();
                            indice.IdRoh_impactSource = "http://gnoss.com/items/referencesource_OTHERS";
                            indice.Roh_impactSourceOther = "Murcia";
                            foreach (KeyValuePair<string, string> item in dicAreasCategoria)
                            {
                                if (item.Value.Contains(articulo.DESCRI_AREA))
                                {
                                    indice.IdRoh_impactCategory = item.Key;
                                    break;
                                }
                            }

                            indice.Roh_impactIndexInYear = float.Parse(articulo.IMPACTO_REVISTA);

                            if (articulo.CUARTIL_REVISTA == "1")
                            {
                                indice.Roh_journalTop25 = true;
                            }
                            else
                            {
                                indice.Roh_journalTop25 = false;
                            }

                            DateTime fecha = new DateTime();
                            if (!string.IsNullOrEmpty(articulo.ANO))
                            {
                                int anio = Int32.Parse(articulo.ANO);
                                int mes = Int32.Parse("01");
                                int dia = Int32.Parse("01");
                                int horas = Int32.Parse("01");
                                int minutos = Int32.Parse("00");
                                int segundos = Int32.Parse("00");
                                fecha = new DateTime(anio, mes, dia, horas, minutos, segundos);
                                indice.Roh_year = fecha;
                            }


                            revista.Roh_impactIndex.Add(indice);

                            // Guardo el título.
                            tituloRevistas[articulo.NOMBRE_REVISTA].Add(articulo.ANO);
                        }
                    }
                }
            }

            foreach (MaindocumentOntology.MainDocument revista in listaRevistas)
            {
                ComplexOntologyResource resource = revista.ToGnossApiResource(mResourceApi, new List<string>());
                pListaRecursosCargar.Add(resource);

                //Guardamos los IDs en la lista.
                dicIDs.Add(revista.Roh_crisIdentifier, resource.GnossId);
            }

            return dicIDs;
        }

        /// <summary>
        /// Permite cargar los recursos.
        /// </summary>
        /// <param name="pListaRecursosCargar">Lista de recursos a cargar.</param>
        /// <param name="pOntology">Ontología.</param>
        private static void CargarDatos(List<ComplexOntologyResource> pListaRecursosCargar)
        {
            //Carga.
            foreach (ComplexOntologyResource recursoCargar in pListaRecursosCargar)
            {
                if (pListaRecursosCargar.Last() == recursoCargar)
                {
                    mResourceApi.LoadComplexSemanticResource(recursoCargar, false, true);
                }
                else
                {
                    mResourceApi.LoadComplexSemanticResource(recursoCargar);
                }
            }
        }

        /// <summary>
        /// Permite cargar los recursos secundarios.
        /// </summary>
        /// <param name="pListaRecursosSecundariosCargar">Lista de recursos secundarios a cargar.</param>
        private static void CargarDatosSecundarios(List<SecondaryResource> pListaRecursosCargar)
        {
            //Carga.
            foreach (SecondaryResource recursoCargar in pListaRecursosCargar)
            {
                mResourceApi.LoadSecondaryResource(recursoCargar);
            }
        }

        /// <summary>
        /// Elimina los datos del grafo.
        /// </summary>
        /// <param name="pRdfType">RdfType del recurso a borrar.</param>
        /// <param name="pOntology">Ontología a consultar.</param>
        private static void EliminarDatosCargados(string pRdfType, string pOntology, HashSet<string> pListaRecursosNoBorrar)
        {
            int max = 10000;

            while (max == 10000)
            {
                max = 10000;
                //Consulta.
                string select = string.Empty, where = string.Empty;
                select += $@"SELECT ?s ";
                where += $@"WHERE {{ ";
                where += $@"?s a <{pRdfType}> ";
                where += $@"}}limit 10000 ";

                //Obtiene las URLs de los recursos a borrar.
                List<string> listaUrl = new List<string>();
                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, pOntology);

                max = resultadoQuery.results.bindings.Count;
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    string recurso = GetValorFilaSparqlObject(fila, "s");
                    if (!pListaRecursosNoBorrar.Contains(recurso))
                    {
                        listaUrl.Add(recurso);
                    }
                }

                //Borra los recursos.
                foreach (string idLargo in listaUrl)
                {
                    try
                    {
                        if (listaUrl.Last() == idLargo)
                        {
                            mResourceApi.PersistentDelete(mResourceApi.GetShortGuid(idLargo), true, true);
                        }
                        else
                        {
                            mResourceApi.PersistentDelete(mResourceApi.GetShortGuid(idLargo));
                        }

                    }
                    catch (Exception) { }
                }
            }
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

        #region Lectura de XMLs
        private static List<AreasUnescoProyectos> LeerAreasUnescoProyectos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<AreasUnescoProyectos> elementos = new List<AreasUnescoProyectos>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                AreasUnescoProyectos elemento = new AreasUnescoProyectos();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDPROYECTO":
                            elemento.IDPROYECTO = node.SelectSingleNode("IDPROYECTO").InnerText;
                            break;
                        case "NUMERO":
                            elemento.NUMERO = node.SelectSingleNode("NUMERO").InnerText;
                            break;
                        case "UNAR_CODIGO":
                            elemento.UNAR_CODIGO = node.SelectSingleNode("UNAR_CODIGO").InnerText;
                            break;
                        case "UNCA_CODIGO":
                            elemento.UNCA_CODIGO = node.SelectSingleNode("UNCA_CODIGO").InnerText;
                            break;
                        case "UNES_CODIGO":
                            elemento.UNES_CODIGO = node.SelectSingleNode("UNES_CODIGO").InnerText;
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

        private static List<Articulo> LeerArticulos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<Articulo> elementos = new List<Articulo>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                Articulo elemento = new Articulo();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "CODIGO":
                            elemento.CODIGO = node.SelectSingleNode("CODIGO").InnerText;
                            break;
                        case "TITULO":
                            elemento.TITULO = node.SelectSingleNode("TITULO").InnerText;
                            break;
                        case "ANO":
                            elemento.ANO = node.SelectSingleNode("ANO").InnerText;
                            break;
                        case "PAIS_CODIGO":
                            elemento.PAIS_CODIGO = node.SelectSingleNode("PAIS_CODIGO").InnerText;
                            break;
                        case "REIS_ISSN":
                            elemento.REIS_ISSN = node.SelectSingleNode("REIS_ISSN").InnerText;
                            break;
                        case "CATALOGO":
                            elemento.CATALOGO = node.SelectSingleNode("CATALOGO").InnerText;
                            break;
                        case "DESCRI_CATALOGO":
                            elemento.DESCRI_CATALOGO = node.SelectSingleNode("DESCRI_CATALOGO").InnerText;
                            break;
                        case "AREA":
                            elemento.AREA = node.SelectSingleNode("AREA").InnerText;
                            break;
                        case "DESCRI_AREA":
                            elemento.DESCRI_AREA = node.SelectSingleNode("DESCRI_AREA").InnerText;
                            break;
                        case "NOMBRE_REVISTA":
                            elemento.NOMBRE_REVISTA = node.SelectSingleNode("NOMBRE_REVISTA").InnerText;
                            break;
                        case "IMPACTO_REVISTA":
                            elemento.IMPACTO_REVISTA = node.SelectSingleNode("IMPACTO_REVISTA").InnerText;
                            break;
                        case "CUARTIL_REVISTA":
                            elemento.CUARTIL_REVISTA = node.SelectSingleNode("CUARTIL_REVISTA").InnerText;
                            break;
                        case "URL_REVISTA":
                            elemento.URL_REVISTA = node.SelectSingleNode("URL_REVISTA").InnerText;
                            break;
                        case "VOLUMEN":
                            elemento.VOLUMEN = node.SelectSingleNode("VOLUMEN").InnerText;
                            break;
                        case "NUMERO":
                            elemento.NUMERO = node.SelectSingleNode("NUMERO").InnerText;
                            break;
                        case "PAGDESDE":
                            elemento.PAGDESDE = node.SelectSingleNode("PAGDESDE").InnerText;
                            break;
                        case "PAGHASTA":
                            elemento.PAGHASTA = node.SelectSingleNode("PAGHASTA").InnerText;
                            break;
                        case "NUMPAG":
                            elemento.NUMPAG = node.SelectSingleNode("NUMPAG").InnerText;
                            break;
                        case "COAUT_EXT":
                            elemento.COAUT_EXT = node.SelectSingleNode("COAUT_EXT").InnerText;
                            break;
                        case "ARTI_DOI":
                            elemento.ARTI_DOI = node.SelectSingleNode("ARTI_DOI").InnerText;
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

        private static List<AutorArticulo> LeerAutoresArticulos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<AutorArticulo> elementos = new List<AutorArticulo>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                AutorArticulo elemento = new AutorArticulo();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "ARTI_CODIGO":
                            elemento.ARTI_CODIGO = node.SelectSingleNode("ARTI_CODIGO").InnerText;
                            break;
                        case "IDPERSONA":
                            elemento.IDPERSONA = node.SelectSingleNode("IDPERSONA").InnerText;
                            break;
                        case "ORDEN":
                            elemento.ORDEN = node.SelectSingleNode("ORDEN").InnerText;
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

        private static List<AutorCongreso> LeerAutoresCongresos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<AutorCongreso> elementos = new List<AutorCongreso>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                AutorCongreso elemento = new AutorCongreso();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "CONG_NUMERO":
                            elemento.CONG_NUMERO = node.SelectSingleNode("CONG_NUMERO").InnerText;
                            break;
                        case "IDPERSONA":
                            elemento.IDPERSONA = node.SelectSingleNode("IDPERSONA").InnerText;
                            break;
                        case "ORDEN":
                            elemento.ORDEN = node.SelectSingleNode("ORDEN").InnerText;
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

        private static List<AutorExposicion> LeerAutoresExposiciones(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<AutorExposicion> elementos = new List<AutorExposicion>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                AutorExposicion elemento = new AutorExposicion();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "EXPO_CODIGO":
                            elemento.EXPO_CODIGO = node.SelectSingleNode("EXPO_CODIGO").InnerText;
                            break;
                        case "IDPERSONA":
                            elemento.IDPERSONA = node.SelectSingleNode("IDPERSONA").InnerText;
                            break;
                        case "ORDEN":
                            elemento.ORDEN = node.SelectSingleNode("ORDEN").InnerText;
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

        private static List<Centro> LeerCentros(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<Centro> elementos = new List<Centro>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                Centro elemento = new Centro();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "CED_CODIGO":
                            elemento.CED_CODIGO = node.SelectSingleNode("CED_CODIGO").InnerText;
                            break;
                        case "CED_NOMBRE":
                            elemento.CED_NOMBRE = node.SelectSingleNode("CED_NOMBRE").InnerText;
                            break;
                        case "COD_ORGANIZACION":
                            elemento.COD_ORGANIZACION = node.SelectSingleNode("COD_ORGANIZACION").InnerText;
                            break;
                        case "DESCRI_ORGANIZACION":
                            elemento.DESCRI_ORGANIZACION = node.SelectSingleNode("DESCRI_ORGANIZACION").InnerText;
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

        private static List<CodigosUnesco> LeerCodigosUnesco(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<CodigosUnesco> elementos = new List<CodigosUnesco>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                CodigosUnesco elemento = new CodigosUnesco();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "UNES_UNAR_CODIGO":
                            elemento.UNES_UNAR_CODIGO = node.SelectSingleNode("UNES_UNAR_CODIGO").InnerText;
                            break;
                        case "UNES_UNCA_CODIGO":
                            elemento.UNES_UNCA_CODIGO = node.SelectSingleNode("UNES_UNCA_CODIGO").InnerText;
                            break;
                        case "UNES_CODIGO":
                            elemento.UNES_CODIGO = node.SelectSingleNode("UNES_CODIGO").InnerText;
                            break;
                        case "UNES_NOMBRE":
                            elemento.UNES_NOMBRE = node.SelectSingleNode("UNES_NOMBRE").InnerText;
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

        private static List<Congreso> LeerCongresos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<Congreso> elementos = new List<Congreso>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                Congreso elemento = new Congreso();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "CONG_NUMERO":
                            elemento.CONG_NUMERO = node.SelectSingleNode("CONG_NUMERO").InnerText;
                            break;
                        case "TIPO_PARTICIPACION":
                            elemento.TIPO_PARTICIPACION = node.SelectSingleNode("TIPO_PARTICIPACION").InnerText;
                            break;
                        case "TITULO_CONTRIBUCION":
                            elemento.TITULO_CONTRIBUCION = node.SelectSingleNode("TITULO_CONTRIBUCION").InnerText;
                            break;
                        case "TITULO_CONGRESO":
                            elemento.TITULO_CONGRESO = node.SelectSingleNode("TITULO_CONGRESO").InnerText;
                            break;
                        case "FECHA_CELEBRACION":
                            elemento.FECHA_CELEBRACION = node.SelectSingleNode("FECHA_CELEBRACION").InnerText;
                            break;
                        case "LUGAR_CELEBRACION":
                            elemento.LUGAR_CELEBRACION = node.SelectSingleNode("LUGAR_CELEBRACION").InnerText;
                            break;
                        case "CONGRESO_INTERNACIONAL":
                            elemento.CONGRESO_INTERNACIONAL = node.SelectSingleNode("CONGRESO_INTERNACIONAL").InnerText;
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

        private static List<DatoEquipoInvestigacion> LeerDatoEquiposInvestigacion(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<DatoEquipoInvestigacion> elementos = new List<DatoEquipoInvestigacion>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                DatoEquipoInvestigacion elemento = new DatoEquipoInvestigacion();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDGRUPOINVESTIGACION":
                            elemento.IDGRUPOINVESTIGACION = node.SelectSingleNode("IDGRUPOINVESTIGACION").InnerText;
                            break;
                        case "NUMERO":
                            elemento.NUMERO = node.SelectSingleNode("NUMERO").InnerText;
                            break;
                        case "IDPERSONA":
                            elemento.IDPERSONA = node.SelectSingleNode("IDPERSONA").InnerText;
                            break;
                        case "CODTIPOPARTICIPACION":
                            elemento.CODTIPOPARTICIPACION = node.SelectSingleNode("CODTIPOPARTICIPACION").InnerText;
                            break;
                        case "RESPONSABLE":
                            elemento.RESPONSABLE = node.SelectSingleNode("RESPONSABLE").InnerText;
                            break;
                        case "EDP":
                            elemento.EDP = node.SelectSingleNode("EDP").InnerText;
                            break;
                        case "FECHAINICIOPERIODO":
                            elemento.FECHAINICIOPERIODO = node.SelectSingleNode("FECHAINICIOPERIODO").InnerText;
                            break;
                        case "FECHAFINPERIODO":
                            elemento.FECHAFINPERIODO = node.SelectSingleNode("FECHAFINPERIODO").InnerText;
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

        private static List<DirectoresTesis> LeerDirectoresTesis(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<DirectoresTesis> elementos = new List<DirectoresTesis>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                DirectoresTesis elemento = new DirectoresTesis();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "CODIGO_TESIS":
                            elemento.CODIGO_TESIS = node.SelectSingleNode("CODIGO_TESIS").InnerText;
                            break;
                        case "IDPERSONADIRECTOR":
                            elemento.IDPERSONADIRECTOR = node.SelectSingleNode("IDPERSONADIRECTOR").InnerText;
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

        private static List<EquipoProyecto> LeerEquiposProyectos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<EquipoProyecto> elementos = new List<EquipoProyecto>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                EquipoProyecto elemento = new EquipoProyecto();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDPROYECTO":
                            elemento.IDPROYECTO = node.SelectSingleNode("IDPROYECTO").InnerText;
                            break;
                        case "NUMEROCOLABORADOR":
                            elemento.NUMEROCOLABORADOR = node.SelectSingleNode("NUMEROCOLABORADOR").InnerText;
                            break;
                        case "IDPERSONA":
                            elemento.IDPERSONA = node.SelectSingleNode("IDPERSONA").InnerText;
                            break;
                        case "CODTITULACION":
                            elemento.CODTITULACION = node.SelectSingleNode("CODTITULACION").InnerText;
                            break;
                        case "FECHAINICIO":
                            elemento.FECHAINICIO = node.SelectSingleNode("FECHAINICIO").InnerText;
                            break;
                        case "FECHAFIN":
                            elemento.FECHAFIN = node.SelectSingleNode("FECHAFIN").InnerText;
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

        private static List<Exposicion> LeerExposiciones(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<Exposicion> elementos = new List<Exposicion>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                Exposicion elemento = new Exposicion();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "CODIGO":
                            elemento.CODIGO = node.SelectSingleNode("CODIGO").InnerText;
                            break;
                        case "NOMBRE":
                            elemento.NOMBRE = node.SelectSingleNode("NOMBRE").InnerText;
                            break;
                        case "FECHA":
                            elemento.FECHA = node.SelectSingleNode("FECHA").InnerText;
                            break;
                        case "LUGAR":
                            elemento.LUGAR = node.SelectSingleNode("LUGAR").InnerText;
                            break;
                        case "TIPO":
                            elemento.TIPO = node.SelectSingleNode("TIPO").InnerText;
                            break;
                        case "CALIDADES":
                            elemento.CALIDADES = node.SelectSingleNode("CALIDADES").InnerText;
                            break;
                        case "INCORPORAR_A_CVN":
                            elemento.INCORPORAR_A_CVN = node.SelectSingleNode("INCORPORAR_A_CVN").InnerText;
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

        private static List<Feature> LeerFeatures()
        {
            Console.Write($"Leyendo features...");
            List<Feature> elementos = new List<Feature>();
            Feature nacional = new Feature() { ID = "NACIONAL", Name = "España", Uri = "https://www.geonames.org/2510769" };
            elementos.Add(nacional);
            Feature regional = new Feature() { ID = "REGIONAL", Name = "Región de Murcia", Uri = "https://www.geonames.org/2513413" };
            elementos.Add(regional);
            Feature propio = new Feature() { ID = "PROPIO", Name = "Universidad de Murcia", Uri = "https://www.geonames.org/6255004" };
            elementos.Add(propio);
            Feature europeo = new Feature() { ID = "EUROPEO", Name = "Europa", Uri = "https://www.geonames.org/6255148" };
            elementos.Add(europeo);
            Feature internacional = new Feature() { ID = "INTERNACIONAL", Name = "Mundo", Uri = "https://www.geonames.org/6295630" };
            elementos.Add(internacional);
            Console.WriteLine($"\rLeídos {elementos.Count} elementos de features");
            return elementos;
        }

        private static List<FechaEquipoProyecto> LeerFechasEquiposProyectos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<FechaEquipoProyecto> elementos = new List<FechaEquipoProyecto>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                FechaEquipoProyecto elemento = new FechaEquipoProyecto();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDPROYECTO":
                            elemento.IDPROYECTO = node.SelectSingleNode("IDPROYECTO").InnerText;
                            break;
                        case "NUMEROCOLABORADOR":
                            elemento.NUMEROCOLABORADOR = node.SelectSingleNode("NUMEROCOLABORADOR").InnerText;
                            break;
                        case "NUMERO":
                            elemento.NUMERO = node.SelectSingleNode("NUMERO").InnerText;
                            break;
                        case "CODTIPOPARTICIPACION":
                            elemento.CODTIPOPARTICIPACION = node.SelectSingleNode("CODTIPOPARTICIPACION").InnerText;
                            break;
                        case "HORASDEDICADAS":
                            elemento.HORASDEDICADAS = node.SelectSingleNode("HORASDEDICADAS").InnerText;
                            break;
                        case "CODTIPOMOTIVOCAMBIOFECHA":
                            //elemento.CODTIPOMOTIVOCAMBIOFECHA = node.SelectSingleNode("CODTIPOMOTIVOCAMBIOFECHA").InnerText;
                            break;
                        case "MOTIVOCAMBIOFECHA":
                            elemento.MOTIVOCAMBIOFECHA = node.SelectSingleNode("MOTIVOCAMBIOFECHA").InnerText;
                            break;
                        case "FECHAINICIOPERIODO":
                            elemento.FECHAINICIOPERIODO = node.SelectSingleNode("FECHAINICIOPERIODO").InnerText;
                            break;
                        case "FECHAFINPERIODO":
                            elemento.FECHAFINPERIODO = node.SelectSingleNode("FECHAFINPERIODO").InnerText;
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

        private static List<FechaProyecto> LeerFechasProyectos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<FechaProyecto> elementos = new List<FechaProyecto>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                FechaProyecto elemento = new FechaProyecto();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDPROYECTO":
                            elemento.IDPROYECTO = node.SelectSingleNode("IDPROYECTO").InnerText;
                            break;
                        case "NUMERO":
                            elemento.NUMERO = node.SelectSingleNode("NUMERO").InnerText;
                            break;
                        case "FECHAINICIOEXPEDIENTE":
                            elemento.FECHAINICIOEXPEDIENTE = node.SelectSingleNode("FECHAINICIOEXPEDIENTE").InnerText;
                            break;
                        case "FECHAINICIOPROYECTO":
                            elemento.FECHAINICIOPROYECTO = node.SelectSingleNode("FECHAINICIOPROYECTO").InnerText;
                            break;
                        case "FECHAFINPROYECTO":
                            elemento.FECHAFINPROYECTO = node.SelectSingleNode("FECHAFINPROYECTO").InnerText;
                            break;
                        case "ESTADO":
                            elemento.ESTADO = node.SelectSingleNode("ESTADO").InnerText;
                            break;
                        case "CODTIPOMOIVOCAMBIOFECHA":
                            elemento.CODTIPOMOIVOCAMBIOFECHA = node.SelectSingleNode("CODTIPOMOIVOCAMBIOFECHA").InnerText;
                            break;
                        case "MOTIVOCAMBIOFECHA":
                            elemento.MOTIVOCAMBIOFECHA = node.SelectSingleNode("MOTIVOCAMBIOFECHA").InnerText;
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

        private static List<FuentesFinanciacionProyectos> LeerFuentesFinanciacionProyectos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<FuentesFinanciacionProyectos> elementos = new List<FuentesFinanciacionProyectos>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                FuentesFinanciacionProyectos elemento = new FuentesFinanciacionProyectos();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDPROYECTO":
                            elemento.IDPROYECTO = node.SelectSingleNode("IDPROYECTO").InnerText;
                            break;
                        case "AYFI_FUEN_CODIGO":
                            elemento.AYFI_FUEN_CODIGO = node.SelectSingleNode("AYFI_FUEN_CODIGO").InnerText;
                            break;
                        case "FUEN_NOMBRE":
                            elemento.FUEN_NOMBRE = node.SelectSingleNode("FUEN_NOMBRE").InnerText;
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

        private static List<GrupoInvestigacion> LeerGruposInvestigacion(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<GrupoInvestigacion> elementos = new List<GrupoInvestigacion>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                GrupoInvestigacion elemento = new GrupoInvestigacion();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDGRUPOINVESTIGACION":
                            elemento.IDGRUPOINVESTIGACION = node.SelectSingleNode("IDGRUPOINVESTIGACION").InnerText;
                            break;
                        case "DESCRIPCION":
                            elemento.DESCRIPCION = node.SelectSingleNode("DESCRIPCION").InnerText;
                            break;
                        case "CODUNIDADADM":
                            elemento.CODUNIDADADM = node.SelectSingleNode("CODUNIDADADM").InnerText;
                            break;
                        case "EXCELENCIA":
                            elemento.EXCELENCIA = node.SelectSingleNode("EXCELENCIA").InnerText;
                            break;
                        case "FECHACREACION":
                            elemento.FECHACREACION = node.SelectSingleNode("FECHACREACION").InnerText;
                            break;
                        case "FECHADESAPARICION":
                            elemento.FECHADESAPARICION = node.SelectSingleNode("FECHADESAPARICION").InnerText;
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

        private static List<InventoresPatentes> LeerInventoresPatentes(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<InventoresPatentes> elementos = new List<InventoresPatentes>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                InventoresPatentes elemento = new InventoresPatentes();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDPATENTE":
                            elemento.IDPATENTE = node.SelectSingleNode("IDPATENTE").InnerText;
                            break;
                        case "IDPERSONAINVENTOR":
                            elemento.IDPERSONAINVENTOR = node.SelectSingleNode("IDPERSONAINVENTOR").InnerText;
                            break;
                        case "INVENTORPRINCIPAL":
                            elemento.INVENTORPRINCIPAL = node.SelectSingleNode("INVENTORPRINCIPAL").InnerText;
                            break;
                        case "PERSONALPROPIO":
                            elemento.PERSONALPROPIO = node.SelectSingleNode("PERSONALPROPIO").InnerText;
                            break;
                        case "NUMEROORDEN":
                            elemento.NUMEROORDEN = node.SelectSingleNode("NUMEROORDEN").InnerText;
                            break;
                        case "PARTICIPACION":
                            elemento.PARTICIPACION = node.SelectSingleNode("PARTICIPACION").InnerText;
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

        private static List<LineasDeInvestigacion> LeerLineasDeInvestigacion(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<LineasDeInvestigacion> elementos = new List<LineasDeInvestigacion>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                LineasDeInvestigacion elemento = new LineasDeInvestigacion();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "LINE_CODIGO":
                            elemento.LINE_CODIGO = node.SelectSingleNode("LINE_CODIGO").InnerText;
                            break;
                        case "LINE_DESCRIPCION":
                            elemento.LINE_DESCRIPCION = node.SelectSingleNode("LINE_DESCRIPCION").InnerText;
                            break;
                        case "LINE_INICIO":
                            elemento.LINE_INICIO = node.SelectSingleNode("LINE_INICIO").InnerText;
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

        private static List<LineasInvestigador> LeerLineasInvestigador(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<LineasInvestigador> elementos = new List<LineasInvestigador>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                LineasInvestigador elemento = new LineasInvestigador();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDGRUPOINVESTIGACION":
                            elemento.IDGRUPOINVESTIGACION = node.SelectSingleNode("IDGRUPOINVESTIGACION").InnerText;
                            break;
                        case "IDPERSONAINVESTIGADOR":
                            elemento.IDPERSONAINVESTIGADOR = node.SelectSingleNode("IDPERSONAINVESTIGADOR").InnerText;
                            break;
                        case "FECHAINCORPORACIONGRUPO":
                            elemento.FECHAINCORPORACIONGRUPO = node.SelectSingleNode("FECHAINCORPORACIONGRUPO").InnerText;
                            break;
                        case "LINE_CODIGO":
                            elemento.LINE_CODIGO = node.SelectSingleNode("LINE_CODIGO").InnerText;
                            break;
                        case "FECHAINICIOTRABAJOLINEA":
                            elemento.FECHAINICIOTRABAJOLINEA = node.SelectSingleNode("FECHAINICIOTRABAJOLINEA").InnerText;
                            break;
                        case "FECHAFINTRABAJOLINEA":
                            elemento.FECHAFINTRABAJOLINEA = node.SelectSingleNode("FECHAFINTRABAJOLINEA").InnerText;
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

        private static List<LineasUnesco> LeerLineasUnesco(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<LineasUnesco> elementos = new List<LineasUnesco>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                LineasUnesco elemento = new LineasUnesco();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "LIUN_LINE_CODIGO":
                            elemento.LIUN_LINE_CODIGO = node.SelectSingleNode("LIUN_LINE_CODIGO").InnerText;
                            break;
                        case "LIUN_UNAR_CODIGO":
                            elemento.LIUN_UNAR_CODIGO = node.SelectSingleNode("LIUN_UNAR_CODIGO").InnerText;
                            break;
                        case "LIUN_UNCA_CODIGO":
                            elemento.LIUN_UNCA_CODIGO = node.SelectSingleNode("LIUN_UNCA_CODIGO").InnerText;
                            break;
                        case "LIUN_UNES_CODIGO":
                            elemento.LIUN_UNES_CODIGO = node.SelectSingleNode("LIUN_UNES_CODIGO").InnerText;
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

        private static List<OrganizacionesExternas> LeerOrganizacionesExternas(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<OrganizacionesExternas> elementos = new List<OrganizacionesExternas>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                OrganizacionesExternas elemento = new OrganizacionesExternas();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDPROYECTO":
                            elemento.IDPROYECTO = node.SelectSingleNode("IDPROYECTO").InnerText;
                            break;
                        case "TIPO_COLABORACION":
                            elemento.TIPO_COLABORACION = node.SelectSingleNode("TIPO_COLABORACION").InnerText;
                            break;
                        case "ENTIDAD":
                            elemento.ENTIDAD = node.SelectSingleNode("ENTIDAD").InnerText;
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

        private static List<PalabrasClaveArticulos> LeerPalabrasClaveArticulos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<PalabrasClaveArticulos> elementos = new List<PalabrasClaveArticulos>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                PalabrasClaveArticulos elemento = new PalabrasClaveArticulos();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "PC_ARTI_CODIGO":
                            elemento.PC_ARTI_CODIGO = node.SelectSingleNode("PC_ARTI_CODIGO").InnerText;
                            break;
                        case "PC_PALABRA":
                            elemento.PC_PALABRA = node.SelectSingleNode("PC_PALABRA").InnerText;
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

        private static List<Patentes> LeerPatentes(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<Patentes> elementos = new List<Patentes>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                Patentes elemento = new Patentes();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDPATENTE":
                            elemento.IDPATENTE = node.SelectSingleNode("IDPATENTE").InnerText;
                            break;
                        case "TIPO":
                            elemento.TIPO = node.SelectSingleNode("TIPO").InnerText;
                            break;
                        case "REFERENCIA":
                            elemento.REFERENCIA = node.SelectSingleNode("REFERENCIA").InnerText;
                            break;
                        case "TITULO":
                            elemento.TITULO = node.SelectSingleNode("TITULO").InnerText;
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

        private static List<Persona> LeerPersonas(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<Persona> elementos = new List<Persona>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                Persona elemento = new Persona();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDPERSONA":
                            elemento.IDPERSONA = node.SelectSingleNode("IDPERSONA").InnerText;
                            break;
                        case "NUMERODOCUMENTO":
                            elemento.NUMERODOCUMENTO = node.SelectSingleNode("NUMERODOCUMENTO").InnerText;
                            break;
                        case "LETRADOCUMENTO":
                            elemento.LETRADOCUMENTO = node.SelectSingleNode("LETRADOCUMENTO").InnerText;
                            break;
                        case "NOMBRE":
                            elemento.NOMBRE = node.SelectSingleNode("NOMBRE").InnerText;
                            break;
                        case "PERS_CENT_CODIGO":
                            elemento.PERS_CENT_CODIGO = node.SelectSingleNode("PERS_CENT_CODIGO").InnerText;
                            break;
                        case "CED_NOMBRE":
                            elemento.CED_NOMBRE = node.SelectSingleNode("CED_NOMBRE").InnerText;
                            break;
                        case "PERS_DEPT_CODIGO":
                            elemento.PERS_DEPT_CODIGO = node.SelectSingleNode("PERS_DEPT_CODIGO").InnerText;
                            break;
                        case "DEP_NOMBRE":
                            elemento.DEP_NOMBRE = node.SelectSingleNode("DEP_NOMBRE").InnerText;
                            break;
                        case "SEXO":
                            elemento.SEXO = node.SelectSingleNode("SEXO").InnerText;
                            break;
                        case "PERSONAL_ACTIVO":
                            elemento.PERSONAL_ACTIVO = node.SelectSingleNode("PERSONAL_ACTIVO").InnerText;
                            break;
                        case "PERSONAL_UMU":
                            elemento.PERSONAL_UMU = node.SelectSingleNode("PERSONAL_UMU").InnerText;
                            break;
                        case "EMAIL":
                            elemento.EMAIL = node.SelectSingleNode("EMAIL").InnerText;
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

        private static List<Proyecto> LeerProyectos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<Proyecto> elementos = new List<Proyecto>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                Proyecto elemento = new Proyecto();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "IDPROYECTO":
                            elemento.IDPROYECTO = node.SelectSingleNode("IDPROYECTO").InnerText;
                            break;
                        case "NOMBRE":
                            elemento.NOMBRE = node.SelectSingleNode("NOMBRE").InnerText;
                            break;
                        case "PROYECTOFINALISTA":
                            elemento.PROYECTOFINALISTA = node.SelectSingleNode("PROYECTOFINALISTA").InnerText;
                            break;
                        case "LIMITATIVO":
                            elemento.LIMITATIVO = node.SelectSingleNode("LIMITATIVO").InnerText;
                            break;
                        case "TIPOFINANCIACION":
                            elemento.TIPOFINANCIACION = node.SelectSingleNode("TIPOFINANCIACION").InnerText;
                            break;
                        case "AMBITO_GEOGRAFICO":
                            elemento.AMBITO_GEOGRAFICO = node.SelectSingleNode("AMBITO_GEOGRAFICO").InnerText;
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

        private static List<Tesis> LeerTesis(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<Tesis> elementos = new List<Tesis>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                Tesis elemento = new Tesis();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "CODIGO_TESIS":
                            elemento.CODIGO_TESIS = node.SelectSingleNode("CODIGO_TESIS").InnerText;
                            break;
                        case "TITULO_TESIS":
                            elemento.TITULO_TESIS = node.SelectSingleNode("TITULO_TESIS").InnerText;
                            break;
                        case "FECHA_LECTURA":
                            elemento.FECHA_LECTURA = node.SelectSingleNode("FECHA_LECTURA").InnerText;
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

        private static List<TipoParticipacionGrupo> LeerTipoParticipacionGrupos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<TipoParticipacionGrupo> elementos = new List<TipoParticipacionGrupo>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                TipoParticipacionGrupo elemento = new TipoParticipacionGrupo();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "CODTIPOPARTICIPACIONGRUPO":
                            elemento.CODTIPOPARTICIPACIONGRUPO = node.SelectSingleNode("CODTIPOPARTICIPACIONGRUPO").InnerText;
                            break;
                        case "DESCRIPCION":
                            elemento.DESCRIPCION = node.SelectSingleNode("DESCRIPCION").InnerText;
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

        private static List<TiposEventos> LeerTiposEventos(string pFile)
        {
            Console.Write($"Leyendo {pFile}...");
            List<TiposEventos> elementos = new List<TiposEventos>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(pFile));

            foreach (XmlNode node in doc.SelectNodes("main/DATA_RECORD"))
            {
                TiposEventos elemento = new TiposEventos();
                foreach (string propiedad in Propiedades(elemento))
                {
                    switch (propiedad)
                    {
                        case "TIEV_CODIGO":
                            elemento.TIEV_CODIGO = node.SelectSingleNode("TIEV_CODIGO").InnerText;
                            break;
                        case "TIEV_NOMBRE":
                            elemento.TIEV_NOMBRE = node.SelectSingleNode("TIEV_NOMBRE").InnerText;
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

        private static List<string> Propiedades(Object objeto)
        {
            List<string> prpos = new List<string>();
            Type type = objeto.GetType();
            System.Reflection.PropertyInfo[] listaPropiedades = type.GetProperties();
            return listaPropiedades.Select(x => x.Name).ToList();
        }
        #endregion

        /// <summary>
        /// Convierte la 1º letra de cada palabra a mayúsculas.
        /// </summary>
        /// <param name="pTexto">Texto a convertir</param>
        /// <returns>Texto con la 1º letra de cada palabra a mayúsculas</returns>
        public static string ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(string pTexto)
        {
            pTexto = pTexto.ToLower();
            string[] SEPARADORES = { ","/*, "."*/, "...", ":", ";", "(", ")", "<", ">", "/", "|", " y ", " o ", " u ", " e ", "·", " .", ". ", " -", "- ", "[", "]", "{", "}" };
            Regex RegExSiglos = new Regex(@"\bx{0,3}(i{1,3}|i[vx]|vi{0,3})\b", RegexOptions.IgnoreCase);

            string[] separadores = new string[SEPARADORES.Length + 3];
            SEPARADORES.CopyTo(separadores, 0);
            separadores[SEPARADORES.Length] = " ";
            separadores[SEPARADORES.Length + 1] = ".";
            separadores[SEPARADORES.Length + 2] = "-";

            string[] palabras = pTexto.Split(separadores, StringSplitOptions.RemoveEmptyEntries);

            string textoFinal = "";

            string palabra2;

            int contador = 0;

            foreach (string palabra in palabras)
            {
                palabra2 = palabra;
                if (palabra.Contains("+") && palabra.Length >= palabra.IndexOf("+") + 2)
                {
                    palabra2 = palabra.Substring(0, palabra.IndexOf("+") + 1) + palabra.Substring(palabra.IndexOf("+") + 1, 1).ToUpper() + palabra.Substring(palabra.IndexOf("+") + 2) + " ";
                }

                //Pongo los símbolos intermedios que hay entre palabra y palabra (espacios, comas...)
                while (contador < pTexto.Length && !pTexto[contador].Equals(palabra[0]))
                {
                    textoFinal += pTexto[contador];
                    contador++;
                }

                if (RegExSiglos.IsMatch(palabra2))
                {
                    textoFinal += palabra2.ToUpper();
                }
                else if (!EsArticuloOConjuncionOPreposicionesComunes(palabra2))
                {
                    if (palabra2.Length > 1)
                    {
                        textoFinal += palabra2.Substring(0, 1).ToUpper() + palabra2.Substring(1);
                    }
                    else if (palabra2.Length == 1)
                    {
                        textoFinal += palabra2.ToUpper();
                    }
                }
                else
                {
                    textoFinal += palabra2;
                }

                contador += palabra.Length;
            }

            //Pongo los símbolos del final de la frase (puntos, cierre de paréntesis...)
            while (contador < pTexto.Length)
            {
                textoFinal += pTexto[contador];
                contador++;
            }

            return textoFinal;
        }

        /// <summary>
        /// Comprueba si la palabra es un artículo o una conjunción.
        /// </summary>
        /// <param name="pPalabra">Palabra a comprobar</param>
        /// <returns>TRUE si la palabra es un artículo o conjunción, FALSE en caso contrario</returns>
        public static bool EsArticuloOConjuncionOPreposicionesComunes(string pPalabra)
        {
            string[] ARTICULOS = { "el", "la", "los", "las", "un", "una", "lo", "unos", "unas" };
            string[] CONJUNCIONES = { "y", "o", "u", "e", "ni" };
            string[] PREPOSICIONESMUYCOMUNES = { "a", "ante", "bajo", "con", "contra", "de", "del", "desde", "en", "entre", "hacia", "hasta", "para", "por", "segun", "sin", "so", "sobre", "tras", "durante", "mediante", "al", "excepto", "salvo" };
            return (ARTICULOS.Contains(pPalabra) || CONJUNCIONES.Contains(pPalabra) || PREPOSICIONESMUYCOMUNES.Contains(pPalabra));
        }

        /// <summary>
        /// Permite normalziar el código DOI de los datos.
        /// </summary>
        /// <param name="pDoi">DOI a normalizar.</param>
        /// <returns>DOI normalizado.</returns>
        public static string LimpiarDoi(string pDoi)
        {
            try
            {
                Uri uri = new Uri(pDoi);
                return uri.AbsolutePath.Substring(1);
            }
            catch (Exception)
            {
                return pDoi;
            }
        }

        /// <summary>
        /// Consulta la BBDD para obtener un diccionario con el id de la secundaria y su nombre.
        /// </summary>
        /// <returns>Diccionario con ID y nombre de la secundaria.</returns>
        private static Dictionary<string, string> CargaListadosAreasRevistas()
        {
            Dictionary<string, string> dicResultados = new Dictionary<string, string>();
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            // Consulta sparql.
            select.Append("SELECT ?ID ?Nombre ");
            where.Append("WHERE { ");
            where.Append("?ID ?p ?o. ");
            where.Append("?o <http://purl.org/dc/elements/1.1/title> ?Nombre. FILTER(lang(?Nombre)='es')");
            where.Append("} ");

            resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), "impactindexcategory");

            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    string id = fila["ID"].value;
                    string nombre = fila["Nombre"].value;
                    dicResultados.Add(id, nombre);
                }
            }

            return dicResultados;
        }
    }
}
