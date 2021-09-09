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

            //Persona en específico a cargar.
            //HashSet<string> personasACargar = new HashSet<string>() { "79" };
            HashSet<string> personasACargar = new HashSet<string>();

            //Recursos para NO borrarlos.
            HashSet<string> listaNoBorrar = new HashSet<string>();
            listaNoBorrar.Add("http://gnoss.com/items/Person_21c0c51d-7a1e-1222-c23e-e8370eb10488_b51f0913-39cd-439d-980e-6cdb108d70e2");
            listaNoBorrar.Add("http://gnoss.com/items/CV_1fca886e-da0b-770e-1171-963e7ca03db8_2eb3851b-5489-47b2-b541-f99b37d83922");
            listaNoBorrar.Add("http://gnoss.com/items/Document_4656ba9a-af48-4bdd-83a4-832ccff0356f_df774a2a-5a9a-44b4-955d-8d631eed399b");
            listaNoBorrar.Add("http://gnoss.com/items/Document_0e80c969-015e-4fcb-a049-4c225e340490_0cfb6939-5067-4504-afdd-789307a48112");
            listaNoBorrar.Add("http://gnoss.com/items/Project_87ca995e-8ed0-4878-be62-6c0c16baa3f3_27c97476-1194-4107-85b4-a09e8c9304af");
            listaNoBorrar.Add("http://gnoss.com/items/Project_26be071f-5640-477e-b596-d0f429e2ac21_c1545417-1173-4d7e-a350-f8c645963ab2");
            listaNoBorrar.Add("http://gnoss.com/items/Project_ef0fc429-4dd9-4402-b55c-19c7562fbbfd_2697e5f6-465e-42ce-86fd-bdf61abb3375");
            listaNoBorrar.Add("http://gnoss.com/items/Project_dc1323a3-1c8f-4b87-8fdf-ec703bae3ffd_997cb1d7-8b90-4ee1-ad06-97d9a5231d9d");
            listaNoBorrar.Add("http://gnoss.com/items/Project_2e9e1645-d3e0-4303-a034-653e18137fdc_4aa03cfa-790e-4404-808e-c909f39e57f4");
            listaNoBorrar.Add("http://gnoss.com/items/Person_e55eaec7-e020-4839-aaa4-042a07dfd823_f92b6662-8514-422a-ab31-040b8c1908af");
            listaNoBorrar.Add("http://gnoss.com/items/Person_516e29e5-6ba5-4b2e-9ecb-53a900121386_00f01033-b255-4073-95cf-8d5c331b10f9");
            listaNoBorrar.Add("http://gnoss.com/items/Person_21f63d02-c1f9-4202-9839-4b1c069aadcf_5ee49ae7-1dc8-42b5-b62b-4c1280366a62");
            listaNoBorrar.Add("http://gnoss.com/items/Person_7964a83b-4aab-4614-a219-f3200a0d3477_6669764e-f17b-4c96-a643-8a775865ee72");
            listaNoBorrar.Add("http://gnoss.com/items/Person_8c5317f2-f967-49bf-b270-2faf97adaf7c_daa5f283-e693-48a3-8b6c-098864b9137e");
            listaNoBorrar.Add("http://gnoss.com/items/Person_e55eaec7-e020-4839-aaa4-042a07dfd823_f92b6662-8514-422a-ab31-040b8c1908af");
            listaNoBorrar.Add("http://gnoss.com/items/Person_32616992-5ae9-4c6a-b047-10bf9717332f_ef14e2e4-018a-4cf9-b15d-500e890bb1d6");
            listaNoBorrar.Add("http://gnoss.com/items/Person_516e29e5-6ba5-4b2e-9ecb-53a900121386_00f01033-b255-4073-95cf-8d5c331b10f9");

            //Lista de recursos a cargar.
            List<ComplexOntologyResource> listaRecursosCargar = new List<ComplexOntologyResource>();

            //Cargar organizaciones.
            CambiarOntologia("organization");
            EliminarDatosCargados("http://xmlns.com/foaf/0.1/Organization", "organization", listaNoBorrar);
            Dictionary<string, string> organizacionesCargar = ObtenerOrganizaciones(personasACargar, ref listaRecursosCargar, equiposProyectos, organizacionesExternas);
            CargarDatos(listaRecursosCargar);
            listaRecursosCargar.Clear();

            //Cargar personas.
            CambiarOntologia("person");
            EliminarDatosCargados("http://xmlns.com/foaf/0.1/Person", "person", listaNoBorrar);
            Dictionary<string, string> personasCargar = ObtenerPersonas(personasACargar, ref listaRecursosCargar, personas, autoresArticulos, autoresCongresos, autoresExposiciones, directoresTesis, equiposProyectos, inventoresPatentes, organizacionesCargar);
            CargarDatos(listaRecursosCargar);
            listaRecursosCargar.Clear();

            //Cargar proyectos.
            CambiarOntologia("project");
            EliminarDatosCargados("http://vivoweb.org/ontology/core#Project", "project", listaNoBorrar);
            Dictionary<string, string> proyectosCargar = ObtenerProyectos(personasACargar, personasCargar, organizacionesCargar, ref listaRecursosCargar, equiposProyectos, proyectos, organizacionesExternas, fechasProyectos, fechasEquiposProyectos, areasUnescoProyectos, codigosUnesco);
            CargarDatos(listaRecursosCargar);
            listaRecursosCargar.Clear();

            //Cargar documentos.
            CambiarOntologia("document");
            EliminarDatosCargados("http://purl.org/ontology/bibo/Document", "document", listaNoBorrar);
            Dictionary<string, string> documentosCargar = ObtenerDocumentos(proyectosCargar, personasACargar, personasCargar, ref listaRecursosCargar, articulos, autoresArticulos, personas, palabrasClave, proyectos, equiposProyectos);
            CargarDatos(listaRecursosCargar);
            listaRecursosCargar.Clear();

            //Cargar curriculums
            CambiarOntologia("curriculumvitae");
            EliminarDatosCargados("http://w3id.org/roh/CV", "curriculumvitae", listaNoBorrar);
            Dictionary<string, string> cvCargar = ObtenerCVs(personasACargar, personasCargar, documentosCargar, ref listaRecursosCargar, articulos, autoresArticulos, personas);
            CargarDatos(listaRecursosCargar);
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
                if(!string.IsNullOrEmpty(codigo.UNES_UNCA_CODIGO) && codigo.UNES_UNCA_CODIGO != "00")
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
        private static Dictionary<string, string> ObtenerPersonas(HashSet<string> pPersonasACargar, ref List<ComplexOntologyResource> pListaRecursosCargar, List<Persona> pPersonas, List<AutorArticulo> pAutoresArticulos, List<AutorCongreso> pAutoresCongreso, List<AutorExposicion> pAutoresExposicion, List<DirectoresTesis> pDirectoresTesis, List<EquipoProyecto> pEquiposProyectos, List<InventoresPatentes> pInventoresPatentes, Dictionary<string, string> pOrganizacionesCargar)
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
                    personaCarga.Roh_crisIdentifier = persona.IDPERSONA;
                    if (persona.PERSONAL_UMU == "S")
                    {
                        personaCarga.IdRoh_hasRole = pOrganizacionesCargar["UMU"];
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
        /// <returns>Diccionario con el ID organización / ID recurso.</returns>
        private static Dictionary<string, string> ObtenerOrganizaciones(HashSet<string> pPersonasACargar, ref List<ComplexOntologyResource> pListaRecursosCargar, List<EquipoProyecto> pEquiposProyectos, List<OrganizacionesExternas> pOrganizaciones)
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
        private static Dictionary<string, string> ObtenerProyectos(HashSet<string> pPersonasACargar, Dictionary<string, string> pDicPersonasCargadas, Dictionary<string, string> pDicOrganizacionesCargadas, ref List<ComplexOntologyResource> pListaRecursosCargar, List<EquipoProyecto> pEquiposProyectos, List<Proyecto> pProyectos, List<OrganizacionesExternas> pOrganizacionesExternas, List<FechaProyecto> pFechaProyectos, List<FechaEquipoProyecto> pFechaEquipoProyectos, List<AreasUnescoProyectos> pAreasUnesco, List<CodigosUnesco> pCodigosUnesco)
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
                        persona.IdRoh_roleOf = pDicPersonasCargadas[equipo.IDPERSONA];
                        persona.Roh_order = equipo.NUMEROCOLABORADOR;

                        //Tipo de participación.
                        string tipoParticipacion = pFechaEquipoProyectos.FirstOrDefault(x => x.IDPROYECTO == proyectoID && x.NUMEROCOLABORADOR == equipo.NUMEROCOLABORADOR).CODTIPOPARTICIPACION;
                        switch(tipoParticipacion)
                        {
                            case "IP":
                                persona.IdRoh_participationType = "http://gnoss.com/items/participationtype_050";
                                break;
                            case "IPRE":
                                persona.IdRoh_participationType = "http://gnoss.com/items/participationtype_OTHERS";
                                persona.Roh_participationTypeOther = "Investigador principal responsable económico";
                                break;
                            case "INV":
                                persona.IdRoh_participationType = "http://gnoss.com/items/participationtype_060";
                                break;
                            case "RE":
                                persona.IdRoh_participationType = "http://gnoss.com/items/participationtype_OTHERS";
                                persona.Roh_participationTypeOther = "Responsable económico";
                                break;
                        }

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

                    //Temas de Investigación
                    //foreach (AreasUnescoProyectos areas in pAreasUnesco.Where(x => x.IDPROYECTO == proyecto.IDPROYECTO))
                    //{
                    //    foreach (CodigosUnesco codigos in pCodigosUnesco.Where(x => x.UNES_UNAR_CODIGO == areas.UNAR_CODIGO && x.UNES_UNCA_CODIGO == areas.UNCA_CODIGO && x.UNES_CODIGO == x.UNES_CODIGO))
                    //    {
                    //        if (!string.IsNullOrEmpty(codigos.UNES_NOMBRE))
                    //        {
                    //            proyectoCargar.Roh_hasProjectClassification = new List<ProjectOntology.ProjectClassification>();
                    //            ProjectOntology.ProjectClassification categoryClassification = new ProjectOntology.ProjectClassification();
                    //            categoryClassification.IdsRoh_projectClassificationNode = new List<string>() { "http://gnoss.com/items/um_" + codigos.UNES_NOMBRE };
                    //            proyectoCargar.Roh_hasProjectClassification.Add(categoryClassification);
                    //        }
                    //    }
                    //}

                    //Creamos el recurso.
                    ComplexOntologyResource resource = proyectoCargar.ToGnossApiResource(mResourceApi, new List<string>());
                    pListaRecursosCargar.Add(resource);

                    //Guardamos los IDs en la lista.
                    dicIDs.Add(proyecto.IDPROYECTO, resource.GnossId);
                }
            }

            return dicIDs;
        }

        /// <summary>
        /// Proceso de obtención de los datos de documentos.
        /// </summary>
        /// <param name="pPersonasACargar"></param>
        /// <param name="pDicPersonasCargadas"></param>
        /// <param name="pListaRecursosCargar"></param>
        /// <param name="pListaArticulos"></param>
        /// <param name="pListaAutoresArticulos"></param>
        /// <param name="pListaPersonas"></param>
        /// <param name="pListaPalabrasClave"></param>
        /// <returns>Diccionario con el ID documento / ID recurso.</returns>
        private static Dictionary<string, string> ObtenerDocumentos(Dictionary<string, string> pDicProyectosACargar, HashSet<string> pPersonasACargar, Dictionary<string, string> pDicPersonasCargadas, ref List<ComplexOntologyResource> pListaRecursosCargar, List<Articulo> pListaArticulos, List<AutorArticulo> pListaAutoresArticulos, List<Persona> pListaPersonas, List<PalabrasClaveArticulos> pListaPalabrasClave, List<Proyecto> pListaProyectos, List<EquipoProyecto> pEquipoProyectos)
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
                    documentoACargar.IdDc_type = "http://gnoss.com/items/publicationtype_020";
                    if (!string.IsNullOrEmpty(articulo.NOMBRE_REVISTA))
                    {
                        documentoACargar.IdRoh_supportType = "http://gnoss.com/items/supporttype_057";
                        documentoACargar.Vivo_hasPublicationVenue = articulo.NOMBRE_REVISTA;
                    }
                    if (!string.IsNullOrEmpty(articulo.VOLUMEN))
                    {
                        documentoACargar.Bibo_volume = articulo.VOLUMEN;
                    }
                    if (!string.IsNullOrEmpty(articulo.NUMERO))
                    {
                        documentoACargar.Bibo_issue = articulo.NUMERO;
                    }

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
                    documentoACargar.Roh_dataAuthor = new List<DocumentOntology.DataAuthor>();
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

                            //DataAuthor
                            DocumentOntology.DataAuthor dataAuthor = new DocumentOntology.DataAuthor();
                            dataAuthor.IdRdf_member = pDicPersonasCargadas[autor.IDPERSONA];
                            documentoACargar.Roh_dataAuthor.Add(dataAuthor);

                            numAutores++;

                            //Proyectos
                            documentoACargar.IdsRoh_project = new List<string>();
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
                                        documentoACargar.IdsRoh_project.Add(pDicProyectosACargar[equipo.IDPROYECTO]);
                                    }
                                }
                            }
                        }
                    }
                    documentoACargar.Roh_authorsNumber = numAutores;                    

                    //Address
                    DocumentOntology.Address direccion = new DocumentOntology.Address();
                    direccion.Vcard_locality = "Murcia";
                    direccion.IdVcard_hasRegion = "http://gnoss.com/items/feature_ADM1_ES62";
                    direccion.IdVcard_hasCountryName = "http://gnoss.com/items/feature_PCLD_724";

                    //ImpactIndex
                    DocumentOntology.ImpactIndex impacto = new DocumentOntology.ImpactIndex();
                    impacto.IdRoh_impactSource = "http://gnoss.com/items/impactindexcategory_OTHERS";
                    impacto.Roh_impactSourceOther = "Universidad de Murcia";
                    impacto.Roh_impactIndexInYear = articulo.IMPACTO_REVISTA;
                    if (articulo.CUARTIL_REVISTA == "1")
                    {
                        impacto.Roh_journalTop25 = true;
                    }
                    else
                    {
                        impacto.Roh_journalTop25 = false;
                    }
                    documentoACargar.Roh_impactIndex = new List<DocumentOntology.ImpactIndex>();
                    documentoACargar.Roh_impactIndex.Add(impacto);

                    if (!string.IsNullOrEmpty(articulo.AREA))
                    {
                        documentoACargar.Roh_hasKnowledgeArea = new List<DocumentOntology.CategoryPath>();
                        DocumentOntology.CategoryPath categoryPath = new DocumentOntology.CategoryPath();
                        categoryPath.IdsRoh_categoryNode = new List<string>() { "http://gnoss.com/items/um_" + articulo.AREA };
                        documentoACargar.Roh_hasKnowledgeArea.Add(categoryPath);
                    }


                    //FreeTextKeyword
                    documentoACargar.Vivo_freeTextKeyword = pListaPalabrasClave.Where(x => x.PC_ARTI_CODIGO == articulo.CODIGO).Select(x => x.PC_PALABRA).ToList();

                    //Creamos el recurso.
                    ComplexOntologyResource resource = documentoACargar.ToGnossApiResource(mResourceApi, new List<string>());
                    pListaRecursosCargar.Add(resource);

                    //Guardamos los IDs en la lista.
                    dicIDs.Add(articulo.CODIGO, resource.GnossId);
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

                if (persona != null && persona.PERSONAL_UMU == "S")
                {
                    CV cvACargar = new CV();
                    cvACargar.Foaf_name = persona.NOMBRE;
                    cvACargar.IdRoh_cvOf = pDicPersonasCargadas[id];
                    cvACargar.Roh_personalData = new PersonalData();
                    cvACargar.Roh_scientificExperience = new ScientificExperience();
                    cvACargar.Roh_scientificExperience.Foaf_name = "Experiencia científica";
                    cvACargar.Roh_scientificActivity = new ScientificActivity();
                    cvACargar.Roh_scientificActivity.Foaf_name = "Actividad científica";

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
                    cvACargar.Roh_scientificActivity.Roh_scientificPublications = new List<RelatedDocuments>();

                    foreach (string idArticulo in pListaAutoresArticulos.Where(x => x.IDPERSONA == id && pDicDocumentosCargados.ContainsKey(x.ARTI_CODIGO)).Select(x => x.ARTI_CODIGO))
                    {
                        RelatedDocuments relatedDocuments = new RelatedDocuments();
                        relatedDocuments.IdRoh_relatedDocument = pDicDocumentosCargados[idArticulo];
                        relatedDocuments.Roh_isPublic = false;
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
                mResourceApi.LoadComplexSemanticResource(recursoCargar);
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
            //Consulta.
            string select = string.Empty, where = string.Empty;
            select += $@"SELECT ?s ";
            where += $@"WHERE {{ ";
            where += $@"?s a <{pRdfType}> ";
            where += $@"}} ";

            //Obtiene las URLs de los recursos a borrar.
            List<string> listaUrl = new List<string>();
            SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, pOntology);
            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    string recurso = GetValorFilaSparqlObject(fila, "s");
                    if (!pListaRecursosNoBorrar.Contains(recurso))
                    {
                        listaUrl.Add(recurso);
                    }
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

    }
}
