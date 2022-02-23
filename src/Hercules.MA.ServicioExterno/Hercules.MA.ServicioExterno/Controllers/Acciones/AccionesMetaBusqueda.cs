using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Hercules.MA.ServicioExterno.Controllers.Utilidades;
using Hercules.MA.ServicioExterno.Models;
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
                        Dictionary<Guid, Person> personsAuxTemp = new Dictionary<Guid, Person>();
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
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?name ?isPublic";
                                string where = $@"  where
                                            {{
                                                ?id a 'person'.
                                                ?id foaf:name ?name.
                                                OPTIONAL{{?id roh:isPublic ?isPublic.}}
                                            }}ORDER BY asc(?name) asc(?id) }} LIMIT {limit} OFFSET {offset}";

                                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                                {
                                    offset += limit;
                                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                                    {
                                        Guid id = new Guid(fila["id"].value.Replace("http://gnoss/", ""));
                                        string nombre = fila["name"].value;
                                        bool isPublic = false;
                                        if (fila.ContainsKey("isPublic"))
                                        {
                                            isPublic = fila["isPublic"].value == "true";
                                        }
                                        Person person = new Person()
                                        {
                                            id = id,
                                            title = nombre,
                                            titleAuxSearch = ObtenerTextoNormalizado(nombre),
                                            searchable = isPublic
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
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?title ?author ?description group_concat(?tag;separator=\"|\") as ?tags #group_concat(?author;separator=\"|\") as ?authors";
                                string where = $@"  where
                                            {{
                                                ?id a 'document'.
                                                ?id roh:title ?title.
                                                ?id roh:isPublic 'true'.
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


                                        // var currentPersons = publicationsTemp.Select(e => new { e.id, people = e.persons.Where(p => p.id == author) }).Where(el => el.people.ToArray().Length > 0).ToArray();
                                        var currentPersons = publicationsTemp.Where(e => e.id == id).ToArray();

                                        if (currentPersons.Length == 0)
                                        {
                                            string title = fila["title"].value;
                                            string description = "";
                                            if (fila.ContainsKey("description"))
                                            {
                                                description = fila["description"].value;
                                            }
                                            string tags = fila["tags"].value;

                                            Publication publication = new Publication()
                                            {
                                                id = id,
                                                title = title,
                                                titleAuxSearch = ObtenerTextoNormalizado(title),
                                                descriptionAuxSearch = ObtenerTextoNormalizado(description),
                                                tagsAuxSearch = new HashSet<string>(tags.Split('|')),
                                                persons = new HashSet<Person>()
                                            };

                                            if (author != Guid.Empty)
                                            {
                                                if (personsAuxTemp.ContainsKey(author))
                                                {
                                                    publication.persons.Add(personsAuxTemp[author]);
                                                }
                                            }
                                            publicationsTemp.Add(publication);
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
                            publications = publicationsTemp;
                        }
                        #endregion

                        #region CargarResearchObjects
                        {
                            int limit = 10000;
                            int offset = 0;
                            while (true)
                            {
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?title ?description group_concat(?tag;separator=\"|\") as ?tags ?author # group_concat(?author;separator=\"|\") as ?authors ";
                                string where = $@"  where
                                            {{
                                                ?id a 'researchobject'.
                                                ?id roh:title ?title.
                                                ?id roh:isPublic 'true'.
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


                                        // var currentPersons = researchObjectsTemp.Select(e => new { e.id, people = e.persons.Where(p => p.id == author) }).Where(el => el.people.ToArray().Length > 0).ToArray();
                                        var currentPersons = researchObjectsTemp.Where(e => e.id == id).ToArray();

                                        if (currentPersons.Length == 0)
                                        {
                                            string title = fila["title"].value;
                                            string description = "";
                                            if (fila.ContainsKey("description"))
                                            {
                                                description = fila["description"].value;
                                            }
                                            string tags = fila["tags"].value;

                                            ResearchObject researchObject = new ResearchObject()
                                            {
                                                id = id,
                                                title = title,
                                                titleAuxSearch = ObtenerTextoNormalizado(title),
                                                descriptionAuxSearch = ObtenerTextoNormalizado(description),
                                                tagsAuxSearch = new HashSet<string>(tags.Split('|')),
                                                persons = new HashSet<Person>()
                                            };

                                            if (author != Guid.Empty)
                                            {
                                                if (personsAuxTemp.ContainsKey(author))
                                                {
                                                    researchObject.persons.Add(personsAuxTemp[author]);
                                                }
                                            }
                                            researchObjectsTemp.Add(researchObject);
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
                                } else
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
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?title ?author ?description # group_concat(?author;separator=\"|\") as ?members";
                                string where = $@"  where
                                            {{
                                                ?id a 'group'.
                                                ?id roh:title ?title.
                                                OPTIONAL{{ ?id vivo:description ?description}}
                                                ?author a 'person'.
                                                ?author foaf:name ?nombre.
                                                {{
                                                    ?id <http://w3id.org/roh/mainResearchers> ?main.
                                                    ?main <http://w3id.org/roh/roleOf> ?author.
                                                    OPTIONAL{{?main <http://vivoweb.org/ontology/core#start> ?fechaPersonaInit.}}
                                                    OPTIONAL{{?main <http://vivoweb.org/ontology/core#end> ?fechaPersonaEnd.}}
                                                    BIND(IF(bound(?fechaPersonaEnd), xsd:integer(?fechaPersonaEnd), 30000000000000) as ?fechaPersonaEndAux)
                                                    BIND(IF(bound(?fechaPersonaInit), xsd:integer(?fechaPersonaInit), 10000000000000) as ?fechaPersonaInitAux)
                                                    BIND(true as ?ip)
                                                }}UNION
                                                {{
                                                    ?id <http://xmlns.com/foaf/0.1/member> ?member.
                                                    ?member <http://w3id.org/roh/roleOf> ?author.
                                                    OPTIONAL{{?author <http://vivoweb.org/ontology/core#start> ?fechaPersonaInit.}}
                                                    OPTIONAL{{?author <http://vivoweb.org/ontology/core#end> ?fechaPersonaEnd.}}
                                                    BIND(IF(bound(?fechaPersonaEnd), xsd:integer(?fechaPersonaEnd), 30000000000000) as ?fechaPersonaEndAux)
                                                    BIND(IF(bound(?fechaPersonaInit), xsd:integer(?fechaPersonaInit), 10000000000000) as ?fechaPersonaInitAux)
                                                    BIND(false as ?ip)
                                                }}
                                                FILTER(?fechaPersonaInitAux<={DateTime.Now.ToString("yyyyMMddHHmmss")} AND ?fechaPersonaEndAux>={DateTime.Now.ToString("yyyyMMddHHmmss")} )


                                            }}ORDER BY DESC(?fechaPersonaInitAux) DESC(?id) }} LIMIT {limit} OFFSET {offset}";

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

                                        var currentPersons = groupsTemp.Where(e => e.id == id).ToArray();

                                        if (currentPersons.Length == 0)
                                        {
                                            string title = fila["title"].value;
                                            string description = "";
                                            if (fila.ContainsKey("description"))
                                            {
                                                description = fila["description"].value;
                                            }

                                            Group group = new Group()
                                            {
                                                id = id,
                                                title = title,
                                                titleAuxSearch = ObtenerTextoNormalizado(title),
                                                descriptionAuxSearch = ObtenerTextoNormalizado(description),
                                                persons = new HashSet<Person>()
                                            };

                                            if (author != Guid.Empty)
                                            {
                                                if (personsAuxTemp.ContainsKey(author))
                                                {
                                                    group.persons.Add(personsAuxTemp[author]);
                                                }
                                            }
                                            groupsTemp.Add(group);
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
                            groups = groupsTemp;
                        }
                        #endregion

                        #region CargarProyectos
                        {
                            int limit = 10000;
                            int offset = 0;
                            while (true)
                            {
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?title ?author ?description # group_concat(?author;separator=\"|\") as ?authors";
                                string where = $@"  where
                                            {{
                                                ?id a 'project'.
                                                ?id roh:title ?title.
                                                ?id roh:isPublic 'true'.
                                                OPTIONAL{{ ?id vivo:description ?description}}
                                                OPTIONAL{{ ?id vivo:start ?fecha}}
                                                OPTIONAL{{ ?id vivo:relates ?lista. ?lista rdf:member ?author.}}
                                            }}ORDER BY DESC(?fecha) DESC(?id) }} LIMIT {limit} OFFSET {offset}";

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
                                        

                                        // var currentPersons = projectsTemp.Select(e => new { e.id, people = e.persons.Where(p => p.id == author) }).Where(el => el.people.ToArray().Length > 0).ToArray();
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
                                                titleAuxSearch = ObtenerTextoNormalizado(title),
                                                descriptionAuxSearch = ObtenerTextoNormalizado(description),
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
                                        } else
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
                        Thread.Sleep(3600000);
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
        public Dictionary<string, List<ObjectSearch>> Busqueda(string pStringBusqueda, string pLang)
        {
            //TODO configurable
            int maxItems = 3;
            Dictionary<string, List<ObjectSearch>> respuesta = new Dictionary<string, List<ObjectSearch>>();

            pStringBusqueda = ObtenerTextoNormalizado(pStringBusqueda);
            string[] inputs = pStringBusqueda.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            //Personas
            if (persons != null)
            {
                respuesta["persona"] = new List<ObjectSearch>();
                List<Person> personas = persons.Select(person => new KeyValuePair<long, Person>(person.Search(inputs), person)).Where(x => x.Key > 0).OrderByDescending(x => x.Key).ToList().Select(x => x.Value).ToList();
                int min = Math.Min(personas.Count, maxItems);
                foreach (Person person in personas.GetRange(0, min))
                {
                    respuesta["persona"].Add(person);
                }
            }

            //Publicaciones
            if (publications != null)
            {
                respuesta["publicacion"] = new List<ObjectSearch>();
                List<Publication> publicaciones = publications.Select(publication => new KeyValuePair<long, Publication>(publication.Search(inputs), publication)).Where(x => x.Key > 0).OrderByDescending(x => x.Key).ToList().Select(x => x.Value).ToList();
                int min = Math.Min(publicaciones.Count, maxItems);
                foreach (Publication publicacion in publicaciones.GetRange(0, min))
                {
                    respuesta["publicacion"].Add(publicacion);
                }
            }

            //ResearchObjects
            if (researchObjects != null)
            {
                respuesta["researchObject"] = new List<ObjectSearch>();
                List<ResearchObject> researchObjectsFilter = researchObjects.Select(researchObject => new KeyValuePair<long, ResearchObject>(researchObject.Search(inputs), researchObject)).Where(x => x.Key > 0).OrderByDescending(x => x.Key).ToList().Select(x => x.Value).ToList();
                int min = Math.Min(researchObjectsFilter.Count, maxItems);
                foreach (ResearchObject researchObj in researchObjectsFilter.GetRange(0, min))
                {
                    respuesta["researchObject"].Add(researchObj);
                }
            }

            //Grupos
            if (groups != null)
            {
                respuesta["group"] = new List<ObjectSearch>();
                List<Group> groupsFilter = groups.Select(group => new KeyValuePair<long, Group>(group.Search(inputs), group)).Where(x => x.Key > 0).OrderByDescending(x => x.Key).ToList().Select(x => x.Value).ToList();
                int min = Math.Min(groupsFilter.Count, maxItems);
                foreach (Group grp in groupsFilter.GetRange(0, min))
                {
                    respuesta["group"].Add(grp);
                }
            }

            //Proyectos
            if (projects != null)
            {
                respuesta["project"] = new List<ObjectSearch>();
                List<Project> projectFilter = projects.Select(project => new KeyValuePair<long, Project>(project.Search(inputs), project)).Where(x => x.Key > 0).OrderByDescending(x => x.Key).ToList().Select(x => x.Value).ToList();
                int min = Math.Min(projectFilter.Count, maxItems);
                foreach (Project project in projectFilter.GetRange(0, min))
                {
                    respuesta["project"].Add(project);
                }
            }


            List<Guid> ids = new List<Guid>();
            foreach(string key in respuesta.Keys)
            {
                ids=ids.Union(respuesta[key].Select(x => x.id).ToList()).ToList();            
            }
            List<ResponseGetUrl> enlaces = new List<ResponseGetUrl>();
            if (ids.Count > 0)
            {
                enlaces = mResourceApi.GetUrl(ids, pLang);
            }

            foreach (string key in respuesta.Keys)
            {
                foreach (ObjectSearch item in respuesta[key])
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
