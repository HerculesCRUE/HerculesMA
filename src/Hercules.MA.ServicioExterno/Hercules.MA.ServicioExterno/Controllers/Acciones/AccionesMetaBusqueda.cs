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
        public void GenertateMetaShearch()
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
                                string select = mPrefijos + "SELECT * WHERE { SELECT DISTINCT ?id ?title ?description group_concat(?author;separator=\"|\") as ?authors group_concat(?tag;separator=\"|\") as ?tags";
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
                                        string title = fila["title"].value;
                                        string authors = fila["authors"].value;
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
                                        foreach (string author in authors.Split('|'))
                                        {
                                            Guid authorAux = new Guid(author.Replace("http://gnoss/", ""));
                                            if (personsAuxTemp.ContainsKey(authorAux))
                                            {
                                                publication.persons.Add(personsAuxTemp[authorAux]);
                                            }
                                        }
                                        publicationsTemp.Add(publication);
                                    }
                                    if (resultadoQuery.results.bindings.Count < limit)
                                    {
                                        break;
                                    }
                                }
                            }
                            publications = publicationsTemp;
                        }
                        #endregion



                        //objSearch = new ObjectSearch();

                        //#region CargarGrupos
                        //List<string> grupos = new List<string>();
                        //SparqlObject resultadoQuery = null;
                        //StringBuilder select = new StringBuilder(), where = new StringBuilder();

                        //select.Append(mPrefijos);
                        //select.Append("SELECT DISTINCT(?tituloGrupo) ");
                        //where.Append("WHERE { ");
                        //where.Append("?s a 'project'.");
                        //where.Append("?s roh:title ?tituloGrupo. ");
                        //where.Append("} ");

                        //resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

                        //if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                        //{
                        //    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        //    {
                        //        grupos.Add(UtilidadesAPI.GetValorFilaSparqlObject(fila, "tituloGrupo"));
                        //    }
                        //}

                        //objSearch.grupos = grupos;
                        //#endregion

                        //#region CargarProyectos
                        //List<string> proyectos = new List<string>();
                        //resultadoQuery = null;
                        //select = new StringBuilder();
                        //where = new StringBuilder();

                        //select.Append(mPrefijos);
                        //select.Append("SELECT DISTINCT(?name) ");
                        //where.Append("WHERE { ");
                        //where.Append("?s a 'project'.");
                        //where.Append("?s roh:title ?name. ");
                        //where.Append("} ");

                        //resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

                        //if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                        //{
                        //    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        //    {
                        //        proyectos.Add(UtilidadesAPI.GetValorFilaSparqlObject(fila, "name"));
                        //    }
                        //}

                        //objSearch.proyectos = proyectos;
                        //#endregion

                        //#region CargarDocumentos
                        //List<string> documents = new List<string>();
                        //resultadoQuery = null;
                        //select = new StringBuilder();
                        //where = new StringBuilder();

                        //select.Append(mPrefijos);
                        //select.Append("SELECT DISTINCT(?name) ");
                        //where.Append("WHERE { ");
                        //where.Append("?s a 'document'.");
                        //where.Append("?s roh:title ?name. ");
                        //where.Append("} ");

                        //resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

                        //if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                        //{
                        //    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        //    {
                        //        documents.Add(UtilidadesAPI.GetValorFilaSparqlObject(fila, "name"));
                        //    }
                        //}

                        //objSearch.documents = documents;
                        //#endregion

                        //#region CargarROs
                        //List<string> ros = new List<string>();
                        //resultadoQuery = null;
                        //select = new StringBuilder();
                        //where = new StringBuilder();

                        //select.Append(mPrefijos);
                        //select.Append("SELECT DISTINCT(?name) ");
                        //where.Append("WHERE { ");
                        //where.Append("?s a 'researchobject'.");
                        //where.Append("?s roh:title ?name. ");
                        //where.Append("} ");

                        //resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mIdComunidad);

                        //if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                        //{
                        //    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        //    {
                        //        ros.Add(UtilidadesAPI.GetValorFilaSparqlObject(fila, "name"));
                        //    }
                        //}

                        //objSearch.ros = ros;
                        //#endregion




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
            return sb.ToString();
        }
    }
}
