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

        public static List<Publication> publications = null;
        public static List<ResearchObject> researchObjects = null;
        public static List<Group> groups = null;
        public static List<Project> projects = null;
        public static List<Person> persons = null;
        public static List<Offer> offers = null;

        public static Dictionary<string, List<ObjectSearch.Property>> textSearch = new Dictionary<string, List<ObjectSearch.Property>>();



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
                        //Aquí se almacenan los objetos buscables
                        List<Publication> publicationsTemp = new List<Publication>();
                        List<ResearchObject> researchObjectsTemp = new List<ResearchObject>();
                        List<Group> groupsTemp = new List<Group>();
                        List<Project> projectsTemp = new List<Project>();
                        List<Person> personsTemp = new List<Person>();
                        List<Offer> offersTemp = new List<Offer>();

                        Dictionary<string, List<ObjectSearch.Property>> textSearchTemp = new Dictionary<string, List<ObjectSearch.Property>>();

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
                                            properties = new List<ObjectSearch.Property>(),
                                            searchable = isActive,
                                            publications = new List<Publication>(),
                                            researchObjects = new List<ResearchObject>(),
                                            groups = new List<Group>(),
                                            projects = new List<Project>(),
                                            offers = new List<Offer>(),
                                            order=nombre
                                        };

                                        person.properties.Add(new ObjectSearch.Property(new HashSet<string>(ObtenerTextoNormalizado(nombre).Split(' ', StringSplitOptions.RemoveEmptyEntries)), 1, person));
                                        personsTemp.Add(person);
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
                        }
                        #endregion

                        #region CargarDocumentos
                        {
                            int limit = 10000;
                            int offset = 0;
                            while (true)
                            {
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?title ?fecha ?description ";
                                string where = $@"  where
                                            {{
                                                ?id a 'document'.
                                                ?id roh:title ?title.
                                                ?id roh:isValidated 'true'.
                                                OPTIONAL{{ ?id bibo:abstract ?description}}
                                                OPTIONAL{{ ?id dct:issued ?fecha}}
                                            }}ORDER BY DESC(?fecha) DESC(?id) }} LIMIT {limit} OFFSET {offset}";

                                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                                {
                                    offset += limit;
                                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                                    {
                                        Guid id = new Guid(fila["id"].value.Replace("http://gnoss/", ""));
                                        string fecha = "";
                                        if(fila.ContainsKey("fecha"))
                                        {
                                            fecha = fila["fecha"].value;
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
                                                properties = new List<ObjectSearch.Property>(),
                                                order=fecha
                                            };

                                            publication.properties.Add(new ObjectSearch.Property(new HashSet<string>(ObtenerTextoNormalizado(title).Split(' ', StringSplitOptions.RemoveEmptyEntries)), 10000000, publication));
                                            publication.properties.Add(new ObjectSearch.Property(new HashSet<string>(ObtenerTextoNormalizado(description).Split(' ', StringSplitOptions.RemoveEmptyEntries)), 1000, publication));

                                            publicationsTemp.Add(publication);
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
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?author ";
                                string where = $@"  where
                                            {{
                                                ?id a 'document'.
                                                ?id roh:title ?title.
                                                ?id roh:isValidated 'true'.
                                                ?id bibo:authorList ?lista. 
                                                ?lista rdf:member ?author.
                                            }}ORDER BY DESC(?id) DESC(?author) }} LIMIT {limit} OFFSET {offset}";

                                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                                {
                                    offset += limit;
                                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                                    {
                                        Guid id = new Guid(fila["id"].value.Replace("http://gnoss/", ""));
                                        Guid author = new Guid(fila["author"].value.Replace("http://gnoss/", ""));

                                        Publication publication = publicationsTemp.FirstOrDefault(x => x.id == id);
                                        if (publication != null)
                                        {
                                            Person person = personsTemp.FirstOrDefault(x => x.id == author);
                                            if (person!=null)
                                            {
                                                person.publications.Add(publication);
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
                                            }}ORDER BY DESC(?id) DESC(?tag) }} LIMIT {limit} OFFSET {offset}";

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
                                            publication.properties.Add(new ObjectSearch.Property(new HashSet<string>(ObtenerTextoNormalizado(tag).Split(' ', StringSplitOptions.RemoveEmptyEntries)), 10000, publication));
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
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?title ?fecha ?description ";
                                string where = $@"  where
                                            {{
                                                ?id a 'researchobject'.
                                                ?id roh:title ?title.
                                                ?id roh:isValidated 'true'.
                                                OPTIONAL{{ ?id bibo:abstract ?description}}
                                                OPTIONAL{{ ?id dct:issued ?fecha}}
                                            }}ORDER BY DESC(?fecha) DESC(?id) }} LIMIT {limit} OFFSET {offset}";

                                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                                {
                                    offset += limit;
                                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                                    {
                                        Guid id = new Guid(fila["id"].value.Replace("http://gnoss/", ""));
                                        string fecha = "";
                                        if (fila.ContainsKey("fecha"))
                                        {
                                            fecha = fila["fecha"].value;
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
                                            researchObject = new ResearchObject()
                                            {
                                                id = id,
                                                title = title,
                                                properties = new List<ObjectSearch.Property>(),
                                                order=fecha,
                                            };
                                            researchObject.properties.Add(new ObjectSearch.Property(new HashSet<string>(ObtenerTextoNormalizado(title).Split(' ', StringSplitOptions.RemoveEmptyEntries)), 10000000, researchObject));
                                            researchObject.properties.Add(new ObjectSearch.Property(new HashSet<string>(ObtenerTextoNormalizado(description).Split(' ', StringSplitOptions.RemoveEmptyEntries)), 1000, researchObject));

                                            researchObjectsTemp.Add(researchObject);
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
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?author ";
                                string where = $@"  where
                                            {{
                                                ?id a 'researchobject'.
                                                ?id roh:title ?title.
                                                ?id roh:isValidated 'true'.
                                                ?id bibo:authorList ?lista. 
                                                ?lista rdf:member ?author.
                                            }}ORDER BY DESC(?id) DESC(?author) }} LIMIT {limit} OFFSET {offset}";

                                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                                {
                                    offset += limit;
                                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                                    {
                                        Guid id = new Guid(fila["id"].value.Replace("http://gnoss/", ""));
                                        Guid author = new Guid(fila["author"].value.Replace("http://gnoss/", ""));

                                        ResearchObject researchObject = researchObjectsTemp.FirstOrDefault(x => x.id == id);
                                        if (researchObject != null)
                                        {
                                            Person person = personsTemp.FirstOrDefault(x => x.id == author);
                                            if (person != null)
                                            {
                                                person.researchObjects.Add(researchObject);
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
                                                ?id a 'researchobject'.
                                                ?id roh:title ?title.
                                                ?id roh:isValidated 'true'.
                                                ?id vivo:freeTextKeyword ?tag
                                            }}ORDER BY DESC(?id) DESC(?tag) }} LIMIT {limit} OFFSET {offset}";

                                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                                {
                                    offset += limit;
                                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                                    {
                                        Guid id = new Guid(fila["id"].value.Replace("http://gnoss/", ""));

                                        ResearchObject researchObject = researchObjectsTemp.FirstOrDefault(x => x.id == id);
                                        string tag = fila["tag"].value;
                                        if (researchObject != null)
                                        {
                                            researchObject.properties.Add(new ObjectSearch.Property(new HashSet<string>(ObtenerTextoNormalizado(tag).Split(' ', StringSplitOptions.RemoveEmptyEntries)), 10000, researchObject));
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
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?title ?description ";
                                string where = $@"  where
                                            {{
                                                ?id a 'group'.
                                                ?id roh:title ?title.
                                                ?id roh:isValidated 'true'.
                                                OPTIONAL{{ ?id vivo:description ?description}}  
                                            }}ORDER BY DESC(?title) DESC(?id) }} LIMIT {limit} OFFSET {offset}";

                                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                                {
                                    offset += limit;
                                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                                    {
                                        Guid id = new Guid(fila["id"].value.Replace("http://gnoss/", ""));
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
                                                properties = new List<ObjectSearch.Property>(),
                                                order=title
                                            };

                                            group.properties.Add(new ObjectSearch.Property(new HashSet<string>(ObtenerTextoNormalizado(title).Split(' ', StringSplitOptions.RemoveEmptyEntries)), 10000, group));
                                            group.properties.Add(new ObjectSearch.Property(new HashSet<string>(ObtenerTextoNormalizado(description).Split(' ', StringSplitOptions.RemoveEmptyEntries)), 1000, group));

                                            groupsTemp.Add(group);
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
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?author ";
                                string where = $@"  where
                                            {{
                                                ?id a 'group'.
                                                ?id roh:title ?title.
                                                ?id roh:isValidated 'true'.   
                                                ?author a 'person'.                                                    
                                                ?id roh:membersGroup ?author.
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

                                        Group group = groupsTemp.FirstOrDefault(e => e.id == id);
                                        if (group != null)
                                        {
                                            Person person = personsTemp.FirstOrDefault(x => x.id == author);
                                            if (person != null)
                                            {
                                                person.groups.Add(group);
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
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?title ?description ";
                                string where = $@"  where
                                            {{
                                                ?id a 'project'.
                                                ?id roh:title ?title.
                                                ?id roh:isValidated 'true'.
                                                OPTIONAL{{ ?id vivo:description ?description}}
                                            }}ORDER BY DESC(?title) DESC(?id)  }} LIMIT {limit} OFFSET {offset}";

                                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                                {
                                    offset += limit;
                                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                                    {
                                        Guid id = new Guid(fila["id"].value.Replace("http://gnoss/", ""));
                                        Project project = projectsTemp.FirstOrDefault(e => e.id == id);

                                        if (project == null)
                                        {
                                            string title = fila["title"].value;
                                            string description = "";
                                            if (fila.ContainsKey("description"))
                                            {
                                                description = fila["description"].value;
                                            }

                                            project = new Project()
                                            {
                                                id = id,
                                                title = title,
                                                properties = new List<ObjectSearch.Property>(),
                                                order=title
                                            };

                                            project.properties.Add(new ObjectSearch.Property(new HashSet<string>(ObtenerTextoNormalizado(title).Split(' ', StringSplitOptions.RemoveEmptyEntries)), 10000, project));
                                            project.properties.Add(new ObjectSearch.Property(new HashSet<string>(ObtenerTextoNormalizado(description).Split(' ', StringSplitOptions.RemoveEmptyEntries)), 1000, project));

                                            projectsTemp.Add(project);
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
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?author ";
                                string where = $@"  where
                                            {{
                                                ?id a 'project'.
                                                ?id roh:title ?title.
                                                ?id roh:isValidated 'true'.
                                                ?author a 'person'.
                                                ?id roh:membersProject  ?author.
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

                                        Project project = projectsTemp.FirstOrDefault(e => e.id == id);
                                        if (project != null)
                                        {
                                            Person person = personsTemp.FirstOrDefault(x => x.id == author);
                                            if (person != null)
                                            {
                                                person.projects.Add(project);
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

                        #region CargarOfertas
                        {
                            int limit = 10000;
                            int offset = 0;
                            while (true)
                            {
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?title ?fecha ?search ";
                                string where = $@"  where
                                            {{
                                                ?id a 'offer'.
                                                ?id schema:name ?title.
                                                ?id schema:availability <http://gnoss.com/items/offerstate_003>.
                                                OPTIONAL{{ ?id roh:search ?search}}
                                                OPTIONAL{{ ?id dct:issued ?fecha}}
                                            }}ORDER BY DESC(?fecha) DESC(?id) }} LIMIT {limit} OFFSET {offset}";

                                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                                {
                                    offset += limit;
                                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                                    {
                                        Guid id = new Guid(fila["id"].value.Replace("http://gnoss/", ""));
                                        string fecha = "";
                                        if (fila.ContainsKey("fecha"))
                                        {
                                            fecha = fila["fecha"].value;
                                        }
                                        Offer offer = offersTemp.FirstOrDefault(x => x.id == id);
                                        if (offer == null)
                                        {
                                            string title = fila["title"].value;
                                            string search = "";
                                            if (fila.ContainsKey("search"))
                                            {
                                                search = fila["search"].value;
                                            }
                                            offer = new Offer()
                                            {
                                                id = id,
                                                title = title,
                                                properties = new List<ObjectSearch.Property>(),
                                                order=fecha
                                            };

                                            offer.properties.Add(new ObjectSearch.Property(new HashSet<string>(ObtenerTextoNormalizado(title).Split(' ', StringSplitOptions.RemoveEmptyEntries)), 10000, offer));
                                            offer.properties.Add(new ObjectSearch.Property(new HashSet<string>(ObtenerTextoNormalizado(search).Split(' ', StringSplitOptions.RemoveEmptyEntries)), 1000, offer));


                                            offersTemp.Add(offer);
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
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?author ";
                                string where = $@"  where
                                            {{
                                                ?id a 'offer'.
                                                ?id schema:availability <http://gnoss.com/items/offerstate_003>.
                                                ?id roh:researchers ?author.
                                            }}ORDER BY DESC(?id) DESC(?author) }} LIMIT {limit} OFFSET {offset}";

                                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

                                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                                {
                                    offset += limit;
                                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                                    {
                                        Guid id = new Guid(fila["id"].value.Replace("http://gnoss/", ""));
                                        Guid author = new Guid(fila["author"].value.Replace("http://gnoss/", ""));

                                        Offer offer = offersTemp.FirstOrDefault(x => x.id == id);
                                        if (offer != null)
                                        {
                                            Person person = personsTemp.FirstOrDefault(x => x.id == author);
                                            if (person != null)
                                            {
                                                person.offers.Add(offer);
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

                            offers = offersTemp;
                        }
                        #endregion

                        foreach (Person person in personsTemp)
                        {
                            foreach (ObjectSearch.Property prop in person.properties)
                            {
                                foreach (string text in prop.texts)
                                {
                                    if (!textSearchTemp.ContainsKey(text))
                                    {
                                        textSearchTemp[text] = new List<ObjectSearch.Property>();
                                    }
                                    textSearchTemp[text].Add(prop);
                                }
                            }
                        }

                        foreach (Publication publication in publicationsTemp)
                        {
                            foreach (ObjectSearch.Property prop in publication.properties)
                            {
                                foreach (string text in prop.texts)
                                {
                                    if (!textSearchTemp.ContainsKey(text))
                                    {
                                        textSearchTemp[text] = new List<ObjectSearch.Property>();
                                    }
                                    textSearchTemp[text].Add(prop);
                                }
                            }
                        }

                        foreach (ResearchObject researchObject in researchObjectsTemp)
                        {
                            foreach (ObjectSearch.Property prop in researchObject.properties)
                            {
                                foreach (string text in prop.texts)
                                {
                                    if (!textSearchTemp.ContainsKey(text))
                                    {
                                        textSearchTemp[text] = new List<ObjectSearch.Property>();
                                    }
                                    textSearchTemp[text].Add(prop);
                                }
                            }
                        }

                        foreach (Group group in groupsTemp)
                        {
                            foreach (ObjectSearch.Property prop in group.properties)
                            {
                                foreach (string text in prop.texts)
                                {
                                    if (!textSearchTemp.ContainsKey(text))
                                    {
                                        textSearchTemp[text] = new List<ObjectSearch.Property>();
                                    }
                                    textSearchTemp[text].Add(prop);
                                }
                            }
                        }

                        foreach (Project project in projectsTemp)
                        {
                            foreach (ObjectSearch.Property prop in project.properties)
                            {
                                foreach (string text in prop.texts)
                                {
                                    if (!textSearchTemp.ContainsKey(text))
                                    {
                                        textSearchTemp[text] = new List<ObjectSearch.Property>();
                                    }
                                    textSearchTemp[text].Add(prop);
                                }
                            }
                        }

                        foreach (Offer offer in offersTemp)
                        {
                            foreach (ObjectSearch.Property prop in offer.properties)
                            {
                                foreach (string text in prop.texts)
                                {
                                    if (!textSearchTemp.ContainsKey(text))
                                    {
                                        textSearchTemp[text] = new List<ObjectSearch.Property>();
                                    }
                                    textSearchTemp[text].Add(prop);
                                }
                            }
                        }

                        textSearch = textSearchTemp;

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
        public Dictionary<string, KeyValuePair<bool, List<ObjectSearch>>> Busqueda(string pStringBusqueda, string pLang)
        {
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

            List<ObjectSearch.Property> propertiesAutocomplete = new List<ObjectSearch.Property>();
            List<ObjectSearch.Property> propertiesSearch = new List<ObjectSearch.Property>();
            foreach (string inputIn in inputs)
            {
                if (textSearch.ContainsKey(inputIn))
                {
                    List<ObjectSearch.Property> propertiesAux = textSearch[inputIn];
                    if (propertiesAux.Count == 0)
                    {
                        propertiesAutocomplete = new List<ObjectSearch.Property>();
                        propertiesSearch = new List<ObjectSearch.Property>();
                        break;
                    }
                    else
                    {
                        if (propertiesAutocomplete.Count == 0)
                        {
                            propertiesAutocomplete = propertiesAux;
                        }
                        else
                        {
                            propertiesAutocomplete = propertiesAutocomplete.Intersect(propertiesAux).ToList();
                        }
                        if (propertiesSearch.Count == 0)
                        {
                            propertiesSearch = propertiesAux;
                        }
                        else
                        {
                            propertiesSearch = propertiesSearch.Intersect(propertiesAux).ToList();
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(lastInput))
            {
                if ((inputs.Count > 0 && propertiesAutocomplete.Count > 0) || inputs.Count == 0)
                {
                    if (propertiesAutocomplete.Count > 0)
                    {
                        propertiesAutocomplete = propertiesAutocomplete.Intersect(textSearch.Where(x => x.Key.StartsWith(lastInput)).ToList().SelectMany(x => x.Value)).ToList();
                    }
                    else
                    {
                        propertiesAutocomplete = textSearch.Where(x => x.Key.StartsWith(lastInput)).ToList().SelectMany(x => x.Value).ToList();
                    }
                }
                if ((inputs.Count > 0 && propertiesSearch.Count > 0) || inputs.Count == 0)
                {                    
                    if (textSearch.ContainsKey(lastInput))
                    {
                        if (inputs.Count == 0)
                        {
                            propertiesSearch = textSearch[lastInput].ToList();
                        }
                        else
                        {
                            propertiesSearch = propertiesSearch.Intersect(textSearch[lastInput]).ToList();
                        }
                    }
                    else
                    {
                        propertiesSearch = new List<ObjectSearch.Property>();
                    }

                }
            }


            Dictionary<Person, int> personasFilter = new Dictionary<Person, int>();
            Dictionary<Publication, int> publicacionesFilter = new Dictionary<Publication, int>();
            Dictionary<ResearchObject, int> researchObjectsFilter = new Dictionary<ResearchObject, int>();
            Dictionary<Group, int> groupsFilter = new Dictionary<Group, int>();
            Dictionary<Project, int> projectsFilter = new Dictionary<Project, int>();
            Dictionary<Offer, int> offersFilter = new Dictionary<Offer, int>();
            bool personasSearch = false;
            bool publicacionesSearch = false;
            bool researchObjectsSearch = false;
            bool groupsSearch = false;
            bool projectsSearch = false;
            bool offersSearch = false;

            foreach (ObjectSearch.Property property in propertiesAutocomplete)
            {
                //Personas
                if (property.owner is Person)
                {
                    Person item = (Person)property.owner;
                    if (item.searchable)
                    {
                        if (!personasFilter.ContainsKey(item))
                        {
                            personasFilter.Add(item, 0);
                        }
                        personasFilter[item] += property.score;
                    }
                    if(item.publications.Count>0)
                    {
                        foreach(Publication publication in item.publications)
                        {
                            if (!publicacionesFilter.ContainsKey(publication))
                            {
                                publicacionesFilter.Add(publication, 0);
                            }
                            publicacionesFilter[publication] += property.score;
                        }
                    }
                    if (item.researchObjects.Count > 0)
                    {
                        foreach (ResearchObject researchObject in item.researchObjects)
                        {
                            if (!researchObjectsFilter.ContainsKey(researchObject))
                            {
                                researchObjectsFilter.Add(researchObject, 0);
                            }
                            researchObjectsFilter[researchObject] += property.score;
                        }
                    }
                    if (item.groups.Count > 0)
                    {
                        foreach (Group group in item.groups)
                        {
                            if (!groupsFilter.ContainsKey(group))
                            {
                                groupsFilter.Add(group, 0);
                            }
                            groupsFilter[group] += property.score;
                        }
                    }
                    if (item.projects.Count > 0)
                    {
                        foreach (Project project in item.projects)
                        {
                            if (!projectsFilter.ContainsKey(project))
                            {
                                projectsFilter.Add(project, 0);
                            }
                            projectsFilter[project] += property.score;
                        }
                    }
                    if (item.offers.Count > 0)
                    {
                        foreach (Offer offer in item.offers)
                        {
                            if (!offersFilter.ContainsKey(offer))
                            {
                                offersFilter.Add(offer, 0);
                            }
                            offersFilter[offer] += property.score;
                        }
                    }
                }

                //Publicaciones
                if (property.owner is Publication)
                {
                    Publication item = (Publication)property.owner;
                    if (!publicacionesFilter.ContainsKey(item))
                    {
                        publicacionesFilter.Add(item, 0);
                    }
                    publicacionesFilter[item] += property.score;
                }

                //ResearchObjects
                if (property.owner is ResearchObject)
                {
                    ResearchObject item = (ResearchObject)property.owner;
                    if (!researchObjectsFilter.ContainsKey(item))
                    {
                        researchObjectsFilter.Add(item, 0);
                    }
                    researchObjectsFilter[item] += property.score;
                }

                //Grupos                
                if (property.owner is Group)
                {
                    Group item = (Group)property.owner;
                    if (!groupsFilter.ContainsKey(item))
                    {
                        groupsFilter.Add(item, 0);
                    }
                    groupsFilter[item] += property.score;
                }

                //Proyectos
                if (property.owner is Project)
                {
                    Project item = (Project)property.owner;
                    if (!projectsFilter.ContainsKey(item))
                    {
                        projectsFilter.Add(item, 0);
                    }
                    projectsFilter[item] += property.score;
                }

                //Ofertas
                if (property.owner is Offer)
                {
                    Offer item = (Offer)property.owner;
                    if (!offersFilter.ContainsKey(item))
                    {
                        offersFilter.Add(item, 0);
                    }
                    offersFilter[item] += property.score;
                }
            }

            foreach (ObjectSearch.Property property in propertiesSearch)
            {
                //Personas
                if (property.owner is Person)
                {
                    if (((Person)property.owner).searchable)
                    {
                        personasSearch = true;
                    }
                    if (((Person)property.owner).publications.Count > 0)
                    {
                        publicacionesSearch = true;
                    }
                    if (((Person)property.owner).researchObjects.Count > 0)
                    {
                        researchObjectsSearch = true;
                    }
                    if (((Person)property.owner).groups.Count > 0)
                    {
                        groupsSearch = true;
                    }
                    if (((Person)property.owner).projects.Count > 0)
                    {
                        projectsSearch = true;
                    }
                    if (((Person)property.owner).offers.Count > 0)
                    {
                        offersSearch = true;
                    }
                }

                //Publicaciones
                if (property.owner is Publication)
                {
                    publicacionesSearch = true;
                }

                //ResearchObjects
                if (property.owner is ResearchObject)
                {
                    researchObjectsSearch = true;
                }

                //Grupos
                if (property.owner is Group)
                {
                    groupsSearch = true;
                }

                //Proyectos
                if (property.owner is Project)
                {
                    projectsSearch = true;
                }

                //Ofertas
                if (property.owner is Offer)
                {
                    offersSearch = true;
                }
            }

            personasFilter = personasFilter.OrderByDescending(x => x.Value).ThenByDescending(x=>x.Key.order).ToDictionary(x => x.Key, x => x.Value);
            publicacionesFilter = publicacionesFilter.OrderByDescending(x => x.Value).ThenByDescending(x => x.Key.order).ToDictionary(x => x.Key, x => x.Value);
            researchObjectsFilter = researchObjectsFilter.OrderByDescending(x => x.Value).ThenByDescending(x => x.Key.order).ToDictionary(x => x.Key, x => x.Value);
            groupsFilter = groupsFilter.OrderByDescending(x => x.Value).ThenByDescending(x => x.Key.order).ToDictionary(x => x.Key, x => x.Value);
            projectsFilter = projectsFilter.OrderByDescending(x => x.Value).ThenByDescending(x => x.Key.order).ToDictionary(x => x.Key, x => x.Value);
            offersFilter = offersFilter.OrderByDescending(x => x.Value).ThenByDescending(x => x.Key.order).ToDictionary(x => x.Key, x => x.Value);


            //Personas
            {
                int min = Math.Min(personasFilter.Count, maxItems);
                List<ObjectSearch> lista = new List<ObjectSearch>();
                foreach (Person item in personasFilter.Keys.ToList().GetRange(0, min))
                {
                    lista.Add(new Person() { title = item.title,id=item.id } );
                }
                respuesta["persona"] = new KeyValuePair<bool, List<ObjectSearch>>(personasSearch, lista);
            }


            //Publicaciones
            {
                int min = Math.Min(publicacionesFilter.Count, maxItems);
                List<ObjectSearch> lista = new List<ObjectSearch>();
                foreach (Publication item in publicacionesFilter.Keys.ToList().GetRange(0, min))
                {
                    lista.Add(new Publication() { title = item.title, id = item.id });
                }
                respuesta["publicacion"] = new KeyValuePair<bool, List<ObjectSearch>>(publicacionesSearch, lista);
            }

            //ResearchObjects
            {
                int min = Math.Min(researchObjectsFilter.Count, maxItems);
                List<ObjectSearch> lista = new List<ObjectSearch>();
                foreach (ResearchObject item in researchObjectsFilter.Keys.ToList().GetRange(0, min))
                {
                    lista.Add(new ResearchObject() { title = item.title, id = item.id });
                }
                respuesta["researchObject"] = new KeyValuePair<bool, List<ObjectSearch>>(researchObjectsSearch, lista);
            }

            //Grupos
            {
                int min = Math.Min(groupsFilter.Count, maxItems);
                List<ObjectSearch> lista = new List<ObjectSearch>();
                foreach (Group item in groupsFilter.Keys.ToList().GetRange(0, min))
                {
                    lista.Add(new Group() { title = item.title, id = item.id });
                }
                respuesta["group"] = new KeyValuePair<bool, List<ObjectSearch>>(groupsSearch, lista);
            }

            //Proyectos
            {
                int min = Math.Min(projectsFilter.Count, maxItems);
                List<ObjectSearch> lista = new List<ObjectSearch>();
                foreach (Project item in projectsFilter.Keys.ToList().GetRange(0, min))
                {
                    lista.Add(new Project() { title = item.title, id = item.id });
                }
                respuesta["project"] = new KeyValuePair<bool, List<ObjectSearch>>(projectsSearch, lista);
            }

            //Ofertas
            {
                int min = Math.Min(offersFilter.Count, maxItems);
                List<ObjectSearch> lista = new List<ObjectSearch>();
                foreach (Offer item in offersFilter.Keys.ToList().GetRange(0, min))
                {
                    lista.Add(new Offer() { title = item.title, id = item.id });
                }
                respuesta["offer"] = new KeyValuePair<bool, List<ObjectSearch>>(offersSearch, lista);
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

        /// <summary>
        /// Método que obtiene el número total de elementos de cada tipo (basado en el hilo que obtiene los datos de la búsqueda)
        /// </summary>
        /// <returns>Devuelve diccionario de 'tipo de items' => 'número de items'.</returns>
        public Dictionary<string, int> GetNumItems()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            result["persons"] = 0;
            result["documents"] = 0;
            result["researchObjects"] = 0;
            result["groups"] = 0;
            result["projects"] = 0;
            result["offers"] = 0;
            if (persons != null && publications != null && researchObjects != null && groups != null && projects != null && offers != null)
            {
                result["persons"] = persons.Where(x => x.searchable).Count();
                result["documents"] = publications.Count;
                result["researchObjects"] = researchObjects.Count;
                result["groups"] = groups.Count;
                result["projects"] = projects.Count;
                result["offers"] = offers.Count;
            }

            return result;

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
