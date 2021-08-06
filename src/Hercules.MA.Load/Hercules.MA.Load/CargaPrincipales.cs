using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using Hercules.MA.Load.Models.UMU;

namespace Hercules.MA.Load
{
    /// <summary>
    /// Clase encargada de cargar los datos de las entidades principales de Hércules-MA.
    /// </summary>
    public class CargaPrincipales
    {
        //Directorio de lectura.
        private static string inputFolder = "Dataset/UMU";

        // Resource API.
        private static ResourceApi mResourceApi = new ResourceApi($@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config\configOAuth\OAuthV3.config");

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
            HashSet<string> personasACargar = new HashSet<string>();
            personasACargar.Add("79");

            //Lista de recursos a cargar.
            List<ComplexOntologyResource> listaRecursosCargar = new List<ComplexOntologyResource>();

            //Cargar personas.
            mResourceApi.ChangeOntoly("person");
            EliminarDatosCargados("http://xmlns.com/foaf/0.1/Person", "person");
            Dictionary<string, string> personasCargar = ObtenerPersonas(personasACargar, ref listaRecursosCargar, personas, autoresArticulos, autoresCongresos, autoresExposiciones, directoresTesis, equiposProyectos, inventoresPatentes);
            CargarDatos(listaRecursosCargar);
            listaRecursosCargar.Clear();

            //Cargar organizaciones.
            mResourceApi.ChangeOntoly("organization");
            EliminarDatosCargados("http://xmlns.com/foaf/0.1/Organization", "organization");
            Dictionary<string, string> organizacionesCargar = ObtenerOrganizaciones(personasACargar, ref listaRecursosCargar, equiposProyectos, organizacionesExternas);
            CargarDatos(listaRecursosCargar);
            listaRecursosCargar.Clear();

            //Cargar proyectos.
            mResourceApi.ChangeOntoly("project");
            EliminarDatosCargados("http://vivoweb.org/ontology/core#Project", "project");
            Dictionary<string, string> proyectosCargar = ObtenerProyectos(personasACargar, personasCargar, organizacionesCargar, ref listaRecursosCargar, equiposProyectos, proyectos, organizacionesExternas, fechasProyectos);
            CargarDatos(listaRecursosCargar);
            listaRecursosCargar.Clear();
        }

        /// <summary>
        /// Proceso de obtención de datos de las Personas.
        /// </summary>
        /// <param name="pPersonasACargar">IDs de las personas que se quieran cargar. Si viene vacío, se cargan todas.</param>
        /// <param name="pListaRecursosCargar">Lista de recursos a cargar.</param>
        /// <param name="pPersonas">Datos de las personas.</param>
        /// <param name="pAutoresArticulos">Datos de los artículos.</param>
        /// <returns>Diccionario con el ID persona / ID recurso.</returns>
        private static Dictionary<string, string> ObtenerPersonas(HashSet<string> pPersonasACargar, ref List<ComplexOntologyResource> pListaRecursosCargar, List<Persona> pPersonas, List<AutorArticulo> pAutoresArticulos, List<AutorCongreso> pAutoresCongreso, List<AutorExposicion> pAutoresExposicion, List<DirectoresTesis> pDirectoresTesis, List<EquipoProyecto> pEquiposProyectos, List<InventoresPatentes> pInventoresPatentes)
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
                    personaCarga.Foaf_familyName = persona.NOMBRE;
                    personaCarga.Foaf_name = persona.NOMBRE;
                    personaCarga.Foaf_firstName = persona.NOMBRE;
                    personaCarga.Foaf_lastName = persona.NOMBRE;

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
        private static Dictionary<string, string> ObtenerProyectos(HashSet<string> pPersonasACargar, Dictionary<string, string> pDicPersonasCargadas, Dictionary<string, string> pDicOrganizacionesCargadas, ref List<ComplexOntologyResource> pListaRecursosCargar, List<EquipoProyecto> pEquiposProyectos, List<Proyecto> pProyectos, List<OrganizacionesExternas> pOrganizacionesExternas, List<FechaProyecto> pFechaProyectos)
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
                    proyectoCargar.Roh_title = proyecto.NOMBRE;
                    proyectoCargar.Roh_researchersNumber = pEquiposProyectos.Where(x => x.IDPROYECTO == proyecto.IDPROYECTO).Count();
                    proyectoCargar.Vivo_relates = new List<ProjectOntology.BFO_0000023>();
                    
                    //Fechas.
                    foreach(FechaProyecto fechaProyecto in pFechaProyectos.Where(x => x.IDPROYECTO == proyecto.IDPROYECTO))
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

                        if(fechaInicio != DateTime.MinValue && fechaFin != DateTime.MinValue)
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
            where += $@"?s a <{pRdfType}> ";
            where += $@"}} ";

            //Obtiene las URLs de los recursos a borrar.
            List<string> listaUrl = new List<string>();
            SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, pOntology);
            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    listaUrl.Add(GetValorFilaSparqlObject(fila, "s"));
                }
            }

            //Borra los recursos.
            foreach (string idLargo in listaUrl)
            {
                mResourceApi.PersistentDelete(mResourceApi.GetShortGuid(idLargo));
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
    }
}
