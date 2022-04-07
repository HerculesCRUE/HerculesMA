using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Hercules.MA.ServicioExterno.Models.Buscador;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Hercules.MA.ServicioExterno.Controllers.Acciones
{
    public class AccionesMetaBusqueda
    {

        #region --- Constantes     
        private static string RUTA_OAUTH = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config/ConfigOAuth/OAuthV3.config";
        private static ResourceApi mResourceApi = new ResourceApi(RUTA_OAUTH);
        private static CommunityApi mCommunityApi = new CommunityApi(RUTA_OAUTH);
        private static Guid mIdComunidad = mCommunityApi.GetCommunityId();
        private static string RUTA_PREFIJOS = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Models/JSON/prefijos.json";
        private static string mPrefijos = string.Join(" ", JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(RUTA_PREFIJOS)));

        public static Dictionary<Guid, Person> personsAux = null;
        public static List<Publication> publications = null;
        public static List<ResearchObject> researchObjects = null;
        public static List<Group> groups = null;
        public static List<Project> projects = null;
        public static List<Person> persons = null;

        public static ObjectSearch objSearch;
        public static List<string> lastSearchs;
        #endregion

        /// <summary>
        /// Busca los elementos necesarios y los guarda en una variable estática para realizar posteriormente la búsqueda en el metabuscador
        /// </summary>
        public void GenerateMetaShearch()
        {
            new Thread(delegate ()
            {
                while (true)
                {
                    try
                    {
                        //Aquí se almacenan todas las personas (aunque no sean buscables) para utilizarlas como auxiliar en alguna de las otras búsquedas
                        Dictionary<Guid, Person> personsAuxTemp = new Dictionary<Guid, Person>();

                        //Aquí se almacenan los objetos buscables
                        List<Publication> publicationsTemp = new List<Publication>();
                        List<ResearchObject> researchObjectsTemp = new List<ResearchObject>();
                        List<Group> groupsTemp = new List<Group>();
                        List<Project> projectsTemp = new List<Project>();
                        List<Person> personsTemp = new List<Person>();


                        #region CargarInvestigadores
                        {
                            int limit = 10000;
                            int offset = 0;
                            while (true)
                            {

                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?name ?isActive ";
                                string where = $@"  where
                                            {{
                                                ?id a 'person'.
                                                ?id foaf:name ?name.
                                                OPTIONAL{{?id roh:isActive ?isActive.}}
                                            }}ORDER BY asc(?name) asc(?id) }} LIMIT {limit} OFFSET {offset}";

                                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                                {
                                    offset += limit;
                                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                                    {
                                        Guid id = new Guid(fila["id"].value.Replace("http://gnoss/", ""));
                                        string nombre = fila["name"].value;
                                        bool isActive = false;
                                        if (fila.ContainsKey("isActive"))
                                        {
                                            isActive = fila["isActive"].value == "true";
                                        }

                                        Person person = new Person()
                                        {
                                            id = id,
                                            title = nombre,
                                            titleAuxSearch = new HashSet<string>(ObtenerTextoNormalizado(nombre).Split(' ', StringSplitOptions.RemoveEmptyEntries)),
                                            searchable = isActive
                                        };
                                        personsAuxTemp[id] = person;

                                        if (person.searchable)
                                        {
                                            personsTemp.Add(person);
                                        }
                                    }
                                    if (resultadoQuery.results.bindings.Count < limit)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            persons = personsTemp;
                            personsAux = personsAuxTemp;
                        }
                        #endregion

                        #region CargarDocumentos
                        {
                            int limit = 10000;
                            int offset = 0;
                            while (true)
                            {
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?title ?author ?description ";
                                string where = $@"  where
                                            {{
                                                ?id a 'document'.
                                                ?id roh:title ?title.
                                                ?id roh:isValidated 'true'.
                                                OPTIONAL{{ ?id bibo:abstract ?description}}
                                                OPTIONAL{{ ?id dct:issued ?fecha}}
                                                OPTIONAL{{ ?id bibo:authorList ?lista. ?lista rdf:member ?author.}}
                                            }}ORDER BY DESC(?fecha) DESC(?id) }} LIMIT {limit} OFFSET {offset}";

                                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                                {
                                    offset += limit;
                                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                                    {
                                        Guid id = new Guid(fila["id"].value.Replace("http://gnoss/", ""));
                                        Guid author = new Guid();
                                        if (fila.ContainsKey("author"))
                                        {
                                            author = new Guid(fila["author"].value.Replace("http://gnoss/", ""));
                                        }

                                        Publication publication = publicationsTemp.FirstOrDefault(x => x.id == id);
                                        if (publication == null)
                                        {
                                            string title = fila["title"].value;
                                            string description = "";
                                            if (fila.ContainsKey("description"))
                                            {
                                                description = fila["description"].value;
                                            }
                                            publication = new Publication()
                                            {
                                                id = id,
                                                title = title,
                                                titleAuxSearch = new HashSet<string>(ObtenerTextoNormalizado(title).Split(' ', StringSplitOptions.RemoveEmptyEntries)),
                                                descriptionAuxSearch = new HashSet<string>(ObtenerTextoNormalizado(description).Split(' ', StringSplitOptions.RemoveEmptyEntries)),
                                                tagsAuxSearch = new List<HashSet<string>>(),
                                                persons = new HashSet<Person>()
                                            };
                                            publicationsTemp.Add(publication);
                                        }
                                        if (author != Guid.Empty)
                                        {
                                            if (personsAuxTemp.ContainsKey(author))
                                            {
                                                publication.persons.Add(personsAuxTemp[author]);
                                            }
                                        }
                                    }
                                    if (resultadoQuery.results.bindings.Count < limit)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }


                            limit = 10000;
                            offset = 0;
                            while (true)
                            {
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?tag ";
                                string where = $@"  where
                                            {{
                                                ?id a 'document'.
                                                ?id roh:isValidated 'true'.
                                                ?id vivo:freeTextKeyword ?tagAux. ?tagAux roh:title ?tag
                                            }}ORDER BY DESC(?tag) DESC(?id) }} LIMIT {limit} OFFSET {offset}";

                                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                                {
                                    offset += limit;
                                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                                    {
                                        Guid id = new Guid(fila["id"].value.Replace("http://gnoss/", ""));

                                        Publication publication = publicationsTemp.FirstOrDefault(x => x.id == id);
                                        string tag = fila["tag"].value;
                                        if (publication != null)
                                        {
                                            publication.tagsAuxSearch.Add(new HashSet<string>(ObtenerTextoNormalizado(tag).Split(' ', StringSplitOptions.RemoveEmptyEntries)));
                                        }
                                    }
                                    if (resultadoQuery.results.bindings.Count < limit)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }

                            publications = publicationsTemp;
                        }
                        #endregion

                        #region CargarResearchObjects
                        {
                            int limit = 10000;
                            int offset = 0;
                            while (true)
                            {
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?title ?author ?description group_concat(?tag;separator=\"|\") as ?tags ";
                                string where = $@"  where
                                            {{
                                                ?id a 'researchobject'.
                                                ?id roh:title ?title.
                                                ?id roh:isValidated 'true'.
                                                OPTIONAL{{ ?id bibo:abstract ?description}}
                                                OPTIONAL{{ ?id dct:issued ?fecha}}
                                                OPTIONAL{{ ?id vivo:freeTextKeyword ?tag}}
                                                OPTIONAL{{ ?id bibo:authorList ?lista. ?lista rdf:member ?author.}}
                                            }}ORDER BY DESC(?fecha) DESC(?id) }} LIMIT {limit} OFFSET {offset}";

                                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                                {
                                    offset += limit;
                                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                                    {
                                        Guid id = new Guid(fila["id"].value.Replace("http://gnoss/", ""));
                                        Guid author = new Guid();
                                        if (fila.ContainsKey("author"))
                                        {
                                            author = new Guid(fila["author"].value.Replace("http://gnoss/", ""));
                                        }

                                        ResearchObject researchObject = researchObjectsTemp.FirstOrDefault(x => x.id == id);
                                        if (researchObject == null)
                                        {
                                            string title = fila["title"].value;
                                            string description = "";
                                            if (fila.ContainsKey("description"))
                                            {
                                                description = fila["description"].value;
                                            }
                                            string tags = fila["tags"].value;
                                            researchObject = new ResearchObject()
                                            {
                                                id = id,
                                                title = title,
                                                titleAuxSearch = new HashSet<string>(ObtenerTextoNormalizado(title).Split(' ', StringSplitOptions.RemoveEmptyEntries)),
                                                descriptionAuxSearch = new HashSet<string>(ObtenerTextoNormalizado(description).Split(' ', StringSplitOptions.RemoveEmptyEntries)),
                                                tagsAuxSearch = tags.Split('|').Select(x => new HashSet<string>(ObtenerTextoNormalizado(x).Split(' ', StringSplitOptions.RemoveEmptyEntries))).ToList(),
                                                persons = new HashSet<Person>()
                                            };
                                            researchObjectsTemp.Add(researchObject);
                                        }
                                        if (author != Guid.Empty)
                                        {
                                            if (personsAuxTemp.ContainsKey(author))
                                            {
                                                researchObject.persons.Add(personsAuxTemp[author]);
                                            }
                                        }
                                    }
                                    if (resultadoQuery.results.bindings.Count < limit)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            researchObjects = researchObjectsTemp;
                        }
                        #endregion

                        #region CargarGrupo
                        {
                            int limit = 10000;
                            int offset = 0;
                            while (true)
                            {
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?title ?author ?description ";
                                string where = $@"  where
                                            {{
                                                ?id a 'group'.
                                                ?id roh:title ?title.
                                                ?id roh:isValidated 'true'.
                                                OPTIONAL{{ ?id vivo:description ?description}}                                                
                                                OPTIONAL
                                                {{
                                                    ?author a 'person'.
                                                    {{
                                                        ?id roh:mainResearchers ?main.
                                                        ?main rdf:member ?author.
                                                    }}UNION
                                                    {{
                                                        ?id roh:researchers ?main.
                                                        ?main rdf:member ?author.
                                                    }}
                                                }}
                                            }}ORDER BY DESC(?author) DESC(?id) }} LIMIT {limit} OFFSET {offset}";

                                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                                {
                                    offset += limit;
                                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                                    {
                                        Guid id = new Guid(fila["id"].value.Replace("http://gnoss/", ""));

                                        string autorId = "";
                                        Guid author = new Guid();
                                        if (fila.ContainsKey("author"))
                                        {
                                            autorId = fila["author"].value;
                                            if (autorId.Length > 0)
                                            {
                                                author = new Guid(autorId.Replace("http://gnoss/", ""));
                                            }
                                        }

                                        Group group = groupsTemp.FirstOrDefault(e => e.id == id);

                                        if (group == null)
                                        {
                                            string title = fila["title"].value;
                                            string description = "";
                                            if (fila.ContainsKey("description"))
                                            {
                                                description = fila["description"].value;
                                            }

                                            group = new Group()
                                            {
                                                id = id,
                                                title = title,
                                                titleAuxSearch = new HashSet<string>(ObtenerTextoNormalizado(title).Split(' ', StringSplitOptions.RemoveEmptyEntries)),
                                                descriptionAuxSearch = new HashSet<string>(ObtenerTextoNormalizado(description).Split(' ', StringSplitOptions.RemoveEmptyEntries)),
                                                persons = new HashSet<Person>()
                                            };
                                            groupsTemp.Add(group);
                                        }
                                        if (author != Guid.Empty)
                                        {
                                            if (personsAuxTemp.ContainsKey(author))
                                            {
                                                group.persons.Add(personsAuxTemp[author]);
                                            }
                                        }
                                    }
                                    if (resultadoQuery.results.bindings.Count < limit)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            groups = groupsTemp;
                        }
                        #endregion

                        #region CargarProyectos
                        {
                            int limit = 10000;
                            int offset = 0;
                            while (true)
                            {
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?title ?author ?description ";
                                string where = $@"  where
                                            {{
                                                ?id a 'project'.
                                                ?id roh:title ?title.
                                                ?id roh:isValidated 'true'.
                                                OPTIONAL{{ ?id vivo:description ?description}}
                                                OPTIONAL
                                                {{
                                                    ?author a 'person'.
                                                    {{
                                                        ?id roh:mainResearchers ?main.
                                                        ?main rdf:member ?author.
                                                    }}UNION
                                                    {{
                                                        ?id roh:researchers ?main.
                                                        ?main rdf:member ?author.
                                                    }}
                                                }}
                                            }}ORDER BY DESC(?id) DESC(?author) }} LIMIT {limit} OFFSET {offset}";

                                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                                {
                                    offset += limit;
                                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                                    {
                                        Guid id = new Guid(fila["id"].value.Replace("http://gnoss/", ""));

                                        string autorId = "";
                                        Guid author = new Guid();
                                        if (fila.ContainsKey("author"))
                                        {
                                            autorId = fila["author"].value;
                                            if (autorId.Length > 0)
                                            {
                                                author = new Guid(autorId.Replace("http://gnoss/", ""));
                                            }
                                        }

                                        var currentPersons = projectsTemp.Where(e => e.id == id).ToArray();

                                        if (currentPersons.Length == 0)
                                        {
                                            string title = fila["title"].value;
                                            string description = "";
                                            if (fila.ContainsKey("description"))
                                            {
                                                description = fila["description"].value;
                                            }

                                            Project project = new Project()
                                            {
                                                id = id,
                                                title = title,
                                                titleAuxSearch = new HashSet<string>(ObtenerTextoNormalizado(title).Split(' ', StringSplitOptions.RemoveEmptyEntries)),
                                                descriptionAuxSearch = new HashSet<string>(ObtenerTextoNormalizado(description).Split(' ', StringSplitOptions.RemoveEmptyEntries)),
                                                persons = new HashSet<Person>()
                                            };

                                            if (author != Guid.Empty)
                                            {
                                                if (personsAuxTemp.ContainsKey(author))
                                                {
                                                    project.persons.Add(personsAuxTemp[author]);
                                                }
                                            }
                                            projectsTemp.Add(project);
                                        }
                                        else
                                        {
                                            if (author != Guid.Empty)
                                            {
                                                if (personsAuxTemp.ContainsKey(author))
                                                {
                                                    currentPersons.First().persons.Add(personsAuxTemp[author]);
                                                }
                                            }
                                        }
                                    }
                                    if (resultadoQuery.results.bindings.Count < limit)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            projects = projectsTemp;
                        }
                        #endregion

                        //TODO COnfigurable
                        Thread.Sleep(300000);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }).Start();
        }

        /// <summary>
        /// Busca los elementos necesarios y devuelve los resultados
        /// </summary>
        /// <param name="pStringBusqueda">string de búsqueda</param>
        /// <param name="pLang">Idioma</param>
        public Dictionary<string,KeyValuePair<bool, List<ObjectSearch>>> Busqueda(string pStringBusqueda, string pLang)
        {
            //TODO configurable
            int maxItems = 3;
            Dictionary<string, KeyValuePair<bool, List<ObjectSearch>>> respuesta = new Dictionary<string, KeyValuePair<bool, List<ObjectSearch>>>();

            pStringBusqueda = ObtenerTextoNormalizado(pStringBusqueda);
            string lastInput = "";
            HashSet<string> inputs = new HashSet<string>();
            if (pStringBusqueda.EndsWith(" "))
            {
                inputs = new HashSet<string>(pStringBusqueda.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                if (pStringBusqueda.Contains(" "))
                {
                    inputs = new HashSet<string>(pStringBusqueda.Substring(0, pStringBusqueda.LastIndexOf(" ")).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                    lastInput = pStringBusqueda.Substring(pStringBusqueda.LastIndexOf(" ")).Trim();
                }
                else
                {
                    lastInput = pStringBusqueda;
                }

            }



            //Personas
            if (persons != null)
            {
                List<Person> personasFilter = persons.Select(person => new KeyValuePair<long, Person>(person.SearchAutocompletar(inputs, lastInput), person)).Where(x => x.Key > 0).OrderByDescending(x => x.Key).ToList().Select(x => x.Value).ToList();
                bool searh = personasFilter.Exists(x => x.SearchBuscador(inputs, lastInput));                
                int min = Math.Min(personasFilter.Count, maxItems);
                List<ObjectSearch> lista = new List<ObjectSearch>();
                foreach (Person person in personasFilter.GetRange(0, min))
                {
                    lista.Add(person);
                }
                respuesta["persona"] = new KeyValuePair<bool, List<ObjectSearch>>(searh, lista);
            }

            //Publicaciones
            if (publications != null)
            {
                List<Publication> publicacionesFilter = publications.Select(publication => new KeyValuePair<long, Publication>(publication.SearchAutocompletar(inputs, lastInput), publication)).Where(x => x.Key > 0).OrderByDescending(x => x.Key).ToList().Select(x => x.Value).ToList();
                bool searh = publicacionesFilter.Exists(x => x.SearchBuscador(inputs, lastInput));
                int min = Math.Min(publicacionesFilter.Count, maxItems);
                List<ObjectSearch> lista = new List<ObjectSearch>();
                foreach (Publication publicacion in publicacionesFilter.GetRange(0, min))
                {
                    lista.Add(publicacion);
                }
                respuesta["publicacion"] = new KeyValuePair<bool, List<ObjectSearch>>(searh, lista);
            }

            //ResearchObjects
            if (researchObjects != null)
            {
                List<ResearchObject> researchObjectsFilter = researchObjects.Select(researchObject => new KeyValuePair<long, ResearchObject>(researchObject.SearchAutocompletar(inputs, lastInput), researchObject)).Where(x => x.Key > 0).OrderByDescending(x => x.Key).ToList().Select(x => x.Value).ToList();
                bool searh = researchObjectsFilter.Exists(x => x.SearchBuscador(inputs, lastInput));
                int min = Math.Min(researchObjectsFilter.Count, maxItems);
                List<ObjectSearch> lista = new List<ObjectSearch>();
                foreach (ResearchObject researchObj in researchObjectsFilter.GetRange(0, min))
                {
                    lista.Add(researchObj);
                }
                respuesta["researchObject"] = new KeyValuePair<bool, List<ObjectSearch>>(searh, lista);
            }

            //Grupos
            if (groups != null)
            {
                List<Group> groupsFilter = groups.Select(group => new KeyValuePair<long, Group>(group.SearchAutocompletar(inputs, lastInput), group)).Where(x => x.Key > 0).OrderByDescending(x => x.Key).ToList().Select(x => x.Value).ToList();
                bool searh = groupsFilter.Exists(x => x.SearchBuscador(inputs, lastInput));
                int min = Math.Min(groupsFilter.Count, maxItems);
                List<ObjectSearch> lista = new List<ObjectSearch>();
                foreach (Group group in groupsFilter.GetRange(0, min))
                {
                    lista.Add(group);
                }
                respuesta["group"] = new KeyValuePair<bool, List<ObjectSearch>>(searh, lista);
            }

            //Proyectos
            if (projects != null)
            {
                List<Project> projectFilter = projects.Select(project => new KeyValuePair<long, Project>(project.SearchAutocompletar(inputs, lastInput), project)).Where(x => x.Key > 0).OrderByDescending(x => x.Key).ToList().Select(x => x.Value).ToList();
                bool searh = projectFilter.Exists(x => x.SearchBuscador(inputs, lastInput));
                int min = Math.Min(projectFilter.Count, maxItems);
                List<ObjectSearch> lista = new List<ObjectSearch>();
                foreach (Project project in projectFilter.GetRange(0, min))
                {
                    lista.Add(project);
                }
                respuesta["project"] = new KeyValuePair<bool, List<ObjectSearch>>(searh, lista);
            }


            List<Guid> ids = new List<Guid>();
            foreach (string key in respuesta.Keys)
            {
                ids = ids.Union(respuesta[key].Value.Select(x => x.id).ToList()).ToList();
            }
            List<ResponseGetUrl> enlaces = new List<ResponseGetUrl>();
            if (ids.Count > 0)
            {
                enlaces = mResourceApi.GetUrl(ids, pLang);
            }

            foreach (string key in respuesta.Keys)
            {
                foreach (ObjectSearch item in respuesta[key].Value)
                {
                    item.url = enlaces.FirstOrDefault(x => x.resource_id == item.id)?.url;
                }
            }

            return respuesta;
        }

        private static string ObtenerTextoNormalizado(string pText)
        {
            string normalizedString = pText.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            foreach (char charin in normalizedString)
            {
                if (char.IsLetterOrDigit(charin) || charin == ' ')
                {
                    sb.Append(charin);
                }
            }
            return sb.ToString().ToLower();
        }
    }
}
