using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using Hercules.MA.ServicioExterno.Models.Cluster;
using ClusterOntology;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Hercules.MA.ServicioExterno.Models.Cluster.Cluster;
using Hercules.MA.ServicioExterno.Models;
using Hercules.MA.ServicioExterno.Models.Graficas.DataItemRelacion;
using Hercules.MA.ServicioExterno.Controllers.Utilidades;

namespace Hercules.MA.ServicioExterno.Controllers.Acciones
{
    public class AccionesCluster
    {
        #region --- Constantes   
        private static string RUTA_OAUTH = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config/ConfigOAuth/OAuthV3.config";
        private static ResourceApi mResourceApi = new ResourceApi(RUTA_OAUTH);
        private static CommunityApi mCommunityApi = new CommunityApi(RUTA_OAUTH);
        private static Guid mIdComunidad = mCommunityApi.GetCommunityId();
        private static string RUTA_PREFIJOS = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Models/JSON/prefijos.json";
        private static string mPrefijos = string.Join(" ", JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(RUTA_PREFIJOS)));
        #endregion

        /// <summary>
        /// Método público que obtiene una lista de thesaurus.
        /// </summary>
        /// <param name="listadoCluster">Listado de thesaurus a obtener.</param>
        /// <returns>Diccionario con las listas de thesaurus.</returns>
        public Dictionary<string, List<ThesaurusItem>> GetListThesaurus(string listadoCluster)
        {

            List<string> thesaurusTypes = new List<string>() { "researcharea" };

            try
            {
                if (listadoCluster != "")
                {
                    thesaurusTypes = JsonConvert.DeserializeObject<List<string>>(listadoCluster);
                }
            }
            catch (Exception e) { throw new Exception("El texto que ha introducido no corresponde a un json válido"); }

            var thesaurus = GetTesauros(thesaurusTypes);

            return thesaurus;
        }


        /// <summary>
        /// Método público para guardar / editar un cluster
        /// </summary>
        /// <param name="pIdGnossUser">Identificador del usuario</param>
        /// <param name="cluster">Objecto cluster</param>
        /// <returns>Diccionario con las listas de thesaurus.</returns>
        public string SaveCluster(string pIdGnossUser, Models.Cluster.Cluster cluster)
        {
            string idRecurso = cluster.entityID;
            int MAX_INTENTOS = 10;
            bool uploadedR = false;

            // Obtener el id del usuario usando el id de la cuenta
            string select = "select ?s ";
            string where = @$"where {{
                    ?s a <http://xmlns.com/foaf/0.1/Person>.
                    ?s <http://w3id.org/roh/gnossUser> ?idGnoss.
                    FILTER(?idGnoss = <http://gnoss/{pIdGnossUser.ToUpper()}>)
                }}";
            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "person");
            var userGnossId = string.Empty;
            sparqlObject.results.bindings.ForEach(e =>
            {
                userGnossId = e["s"].value;
            });

            if (!string.IsNullOrEmpty(userGnossId))
            {
                // Creando el objeto del cluster
                // Creando las categorías
                List<CategoryPath> categorias = new List<CategoryPath>();
                categorias.Add(new CategoryPath() { IdsRoh_categoryNode = cluster.terms });

                List<ClusterPerfil> listClusterPerfil = new();
                // Creando los perfiles del cluster
                if (cluster.profiles != null)
                {

                    // Get the full ID
                    List<string> numMember = new();
                    Dictionary<string, string> relationIDs = new();
                    cluster.profiles.ForEach(e =>
                    {
                        if (e.users != null)
                        {
                            foreach (var us in e.users)
                            {
                                if (us.userID != null && us.shortUserID != null)
                                {
                                    relationIDs.Add("http://gnoss.com/" + us.shortUserID, us.userID);
                                }
                                else
                                {
                                    numMember.Add("<http://gnoss.com/" + us.shortUserID + ">");
                                }
                            }
                        }
                    });
                    numMember = numMember.Distinct().ToList();

                    // Query to get the full ID
                    if (numMember.Count > 0)
                    {

                        select = "select distinct ?s ?entidad FROM<http://gnoss.com/person.owl>";
                        where = @$"where {{
                            ?s <http://gnoss/hasEntidad> ?entidad.
                            ?entidad a<http://xmlns.com/foaf/0.1/Person>.
                            FILTER(?s in ({string.Join(',', numMember)}))
                        }}
                        ";

                        sparqlObject = mResourceApi.VirtuosoQuery(select, where, "person");
                        sparqlObject.results.bindings.ForEach(e =>
                        {
                            relationIDs.Add(e["s"].value, e["entidad"].value);
                        });
                    }
                    try
                    {

                        // Create the list of profiles
                        listClusterPerfil = cluster.profiles.Select(e =>
                        {
                            List<string> theUsersP = new List<string>();
                            if (e.users != null)
                            {
                                theUsersP = e.users.Select(x => relationIDs.ContainsKey(("http://gnoss.com/" + x.shortUserID)) ? relationIDs[("http://gnoss.com/" + x.shortUserID)] : "http://gnoss.com/" + x.shortUserID).ToList();
                            }
                            var knowledge = new List<CategoryPath>() { new CategoryPath() { IdsRoh_categoryNode = e.terms } };
                            var clsp = new ClusterPerfil()
                            {
                                Roh_title = e.name,
                                Roh_hasKnowledgeArea = knowledge,
                                IdsRdf_member = theUsersP,
                                Vivo_freeTextKeyword = e.tags
                            };
                            if (e.entityID.StartsWith("http"))
                            {
                                clsp.GNOSSID = e.entityID;
                            }
                            return clsp;
                        }).ToList();


                    } catch(Exception e) { }
                }


                // creando los cluster
                ClusterOntology.Cluster cRsource = new();
                cRsource.IdRdf_member = userGnossId;
                cRsource.Roh_title = cluster.name;
                cRsource.Vivo_description = cluster.description;
                cRsource.Roh_hasKnowledgeArea = categorias;
                cRsource.Roh_clusterPerfil = listClusterPerfil.ToList();
                cRsource.Dct_issued = DateTime.Now;

                mResourceApi.ChangeOntoly("cluster");

                if (idRecurso != null && idRecurso != "")
                {
                    string[] recursoSplit = idRecurso.Split('_');

                    // Modificación.
                    ComplexOntologyResource resource = cRsource.ToGnossApiResource(mResourceApi, null, new Guid(recursoSplit[recursoSplit.Length - 2]), new Guid(recursoSplit[recursoSplit.Length - 1]));
                    int numIntentos = 0;
                    while (!resource.Modified)
                    {
                        numIntentos++;
                        if (numIntentos > MAX_INTENTOS)
                        {
                            break;
                        }

                        mResourceApi.ModifyComplexOntologyResource(resource, false, false);
                        uploadedR = resource.Modified;
                    }

                }
                else
                {
                    // Inserción.
                    ComplexOntologyResource resource = cRsource.ToGnossApiResource(mResourceApi, null);
                    int numIntentos = 0;
                    while (!resource.Uploaded)
                    {
                        numIntentos++;
                        if (numIntentos > MAX_INTENTOS)
                        {
                            break;
                        }
                        idRecurso = mResourceApi.LoadComplexSemanticResource(resource, true, true);
                        uploadedR = resource.Uploaded;
                    }
                }
            }

            if (uploadedR)
            {
                return idRecurso;
            }
            else
            {
                throw new Exception("Recurso no creado");
            }
        }


        /// <summary>
        /// Método público para obtener los datos de un cluster
        /// </summary>
        /// <param name="pIdClusterId">Identificador del cluster</param>
        /// <returns>Diccionario con las listas de thesaurus.</returns>
        internal Models.Cluster.Cluster LoadCluster(string pIdClusterId)
        {

            // Obtener datos del cluster
            string select = "select ?p ?o ";
            string where = @$"where {{
                    ?s a <http://w3id.org/roh/Cluster>.
                    ?s ?p ?o.
                    FILTER(?s = <{pIdClusterId}>)
                }}";
            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "cluster");

            // Inicizalizamos el modelo del Cluster para devolver
            Models.Cluster.Cluster pDataCluster = new();
            pDataCluster.terms = new();
            pDataCluster.profiles = new();

            // Lista de los ids de los perfiles devuelto por la consulta
            List<string> perfiles = new();

            sparqlObject.results.bindings.ForEach(e =>
            {
                pDataCluster.entityID = pIdClusterId;

                switch (e["p"].value)
                {
                    case "http://w3id.org/roh/title":
                        pDataCluster.name = e["o"].value;
                        break;
                    case "http://vivoweb.org/ontology/core#description":
                        pDataCluster.description = e["o"].value;
                        break;
                    case "http://w3id.org/roh/hasKnowledgeArea":
                        pDataCluster.terms.Add(e["o"].value);
                        break;
                    case "http://w3id.org/roh/clusterPerfil":
                        perfiles.Add(e["o"].value);
                        break;
                }
            });



            // Obtenemos todos los datos de las areas temáticas
            if (pDataCluster.terms.Count > 0)
            {
                pDataCluster.terms = LoadCurrentTerms(pDataCluster.terms);

            }


            // Obtenemos todos los datos de los perfiles
            foreach (string p in perfiles)
            {
                //Datos del perfil (nombre, categorías y tags)
                select = "select distinct ?s ?title group_concat(distinct ?freeTextKeyword;separator=',') as ?freeTextKeywordGroup group_concat(distinct ?KnowledgeArea;separator=',') as ?knowledgeAreaGroup FROM <http://gnoss.com/person.owl>";
                where = @$"where {{
                    ?s a <http://w3id.org/roh/ClusterPerfil>.
                    ?s <http://w3id.org/roh/title> ?title.
                    OPTIONAL
                    {{
                        ?s <http://vivoweb.org/ontology/core#freeTextKeyword> ?freeTextKeyword.
                    }}
                    OPTIONAL
                    {{
                        ?s <http://w3id.org/roh/hasKnowledgeArea> ?hasKnowledgeArea.
                        ?hasKnowledgeArea <http://w3id.org/roh/categoryNode> ?KnowledgeArea.
                    }}
                    FILTER(?s = <{p}>)
                }}";
                sparqlObject = mResourceApi.VirtuosoQuery(select, where, "cluster");

                PerfilCluster perfilCluster = new();
                perfilCluster.tags = new();
                perfilCluster.terms = new();
                perfilCluster.users = new();

                // Carga los datos en el objeto
                sparqlObject.results.bindings.ForEach(e =>
                {
                    perfilCluster.entityID = e["s"].value;
                    perfilCluster.name = e["title"].value;
                    perfilCluster.tags = e["freeTextKeywordGroup"].value.Split(',',StringSplitOptions.RemoveEmptyEntries).ToList();
                    perfilCluster.terms = e["knowledgeAreaGroup"].value.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                    perfilCluster.users = new List<PerfilCluster.UserCluster>();
                });

                //Datos de los miembros
                select = "select distinct ?memberPerfil ?nombreUser FROM <http://gnoss.com/person.owl>";
                where = @$"where {{
                    ?s <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?memberPerfil.
                    ?memberPerfil <http://xmlns.com/foaf/0.1/name> ?nombreUser.
                    FILTER(?s = <{p}>)
                }}";
                sparqlObject = mResourceApi.VirtuosoQuery(select, where, "cluster");

                // Carga los datos en el objeto
                sparqlObject.results.bindings.ForEach(e =>
                {
                    perfilCluster.users.Add(new PerfilCluster.UserCluster()
                    {
                        userID = e["memberPerfil"].value,
                        name = e["nombreUser"].value,
                    });
                });

                // Añade el perfil creado a los datos del cluster
                pDataCluster.profiles.Add(perfilCluster);
            }

            return pDataCluster;
        }


        /// <summary>
        /// Método público para cargar los perfiles de cada investigador sugerido del cluster
        /// </summary>
        /// <param name="pDataCluster">Datos del cluster para obtener los perfiles</param>
        /// <param name="pPersons">Listado de personas sobre los que pedir información</param>
        /// <returns>Diccionario con los datos necesarios para cada persona por cluster.</returns>
        public Dictionary<string, Dictionary<string, ScoreCluster>> LoadProfiles(Models.Cluster.Cluster pDataCluster, List<string> pPersons)
        {
            //ID persona/ID perfil/score
            Dictionary<string, Dictionary<string, ScoreCluster>> respuesta = new Dictionary<string, Dictionary<string, ScoreCluster>>();

            List<string> filtrosPerfiles = new List<string>();
            List<string> filtrosPerfilesTerms = new List<string>();
            List<string> filtrosPerfilesTags = new List<string>();
            foreach (PerfilCluster perfilCluster in pDataCluster.profiles)
            {
                // Inicializa cada respuesta
                foreach (string person in pPersons)
                {
                    if (!respuesta.ContainsKey(person))
                    {
                        respuesta.Add(person, new Dictionary<string, ScoreCluster>());
                    }
                    if (!respuesta[person].ContainsKey(perfilCluster.entityID))
                    {
                        respuesta[person].Add(perfilCluster.entityID, new ScoreCluster());
                    }
                }


                string filtroCategorias = "";
                if (perfilCluster.terms != null && perfilCluster.terms.Count > 0)
                {
                    filtroCategorias = $@"  {{
					                            ?doc <http://w3id.org/roh/hasKnowledgeArea> ?area.
					                            ?area <http://w3id.org/roh/categoryNode> ?node.
					                            FILTER(?node in(<{string.Join(">,<", perfilCluster.terms)}>))
				                            }}";
                }
                string filtroTags = "";
                if (perfilCluster.tags != null && perfilCluster.tags.Count > 0)
                {
                    filtroTags = $@"   {{
				                            ?doc <http://vivoweb.org/ontology/core#freeTextKeyword>  ?keywordO.
                                            ?keywordO <http://w3id.org/roh/title> ?tag.
				                            FILTER(?tag in('{string.Join("','", perfilCluster.tags)}'))
				                        }}";
                }
                string union = "";
                if (!string.IsNullOrEmpty(filtroCategorias) && !string.IsNullOrEmpty(filtroTags))
                {
                    union = "UNION";
                }
                string filtroPerfil = $@"   {{
                                                BIND('{perfilCluster.entityID}' as ?perfil)
                                                {filtroCategorias}
                                                {union}
                                                {filtroTags}
                                            }}";
                filtrosPerfiles.Add(filtroPerfil);


                string filtroPerfilTerm = $@"   {{
                                                BIND('{perfilCluster.entityID}' as ?perfil)
                                                {filtroCategorias}
                                            }}";
                filtrosPerfilesTerms.Add(filtroPerfilTerm);


                string filtroPerfilTag = $@"   {{
                                                BIND('{perfilCluster.entityID}' as ?perfil)
                                                {filtroTags}
                                            }}";
                filtrosPerfilesTags.Add(filtroPerfilTag);



            }

            string select = "select ?person ?perfil (count(distinct ?node) + count(distinct ?tag)) as ?scoreAux ";
            string where = @$"where {{
                    ?doc a 'document'.
                    ?doc <http://w3id.org/roh/isValidated> 'true'.
				    ?doc <http://purl.org/ontology/bibo/authorList> ?authorList.
				    ?authorList <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
				    ?person a 'person'.
                    ?person <http://w3id.org/roh/isActive> 'true'.
                    FILTER(?person in (<http://gnoss/{string.Join(">,<http://gnoss/", pPersons.Select(x => x.ToUpper()))}>))
                    {string.Join("UNION", filtrosPerfiles)}
                }}";
            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

            foreach (Dictionary<string, SparqlObject.Data> fila in sparqlObject.results.bindings)
            {
                string person = fila["person"].value.Replace("http://gnoss/", "").ToLower();
                string perfil = fila["perfil"].value;
                PerfilCluster perfilCluster = pDataCluster.profiles.FirstOrDefault(x => x.entityID == perfil);
                float scoreAux = float.Parse(fila["scoreAux"].value);
                float scoreMax = 0;
                if (perfilCluster.tags != null)
                {
                    scoreMax += perfilCluster.tags.Count;
                }
                if (perfilCluster.terms != null)
                {
                    scoreMax += perfilCluster.terms.Count;
                }
                float scoreAjuste = scoreAux / scoreMax;
                respuesta[person][perfil].ajuste = scoreAjuste;
            }

            // Obtener los documentos por cada categoría
            select = "select ?person ?perfil ?node (count(distinct ?doc)) as ?numDoc";
            where = @$"where {{
                    ?doc a 'document'.
                    ?doc <http://w3id.org/roh/isValidated> 'true'.
				    ?doc <http://purl.org/ontology/bibo/authorList> ?authorList.
				    ?authorList <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
				    ?person a 'person'.
                    ?person <http://w3id.org/roh/isActive> 'true'.
                    FILTER(?person in (<http://gnoss/{string.Join(">,<http://gnoss/", pPersons.Select(x => x.ToUpper()))}>))
                    {string.Join("UNION", filtrosPerfilesTerms)}
                }}";
            sparqlObject = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

            foreach (Dictionary<string, SparqlObject.Data> fila in sparqlObject.results.bindings)
            {
                string person = fila["person"].value.Replace("http://gnoss/", "").ToLower();
                string perfil = fila["perfil"].value;
                string node = fila["node"].value;
                int numDoc = int.Parse(fila["numDoc"].value);
                if (respuesta[person][perfil].terms == null)
                {
                    respuesta[person][perfil].terms = new();
                }
                respuesta[person][perfil].terms.Add(node, numDoc);
                respuesta[person][perfil].numPublicaciones += numDoc;

            }

            // Obtener los documentos por cada tag
            select = "select ?person ?perfil ?tag (count(distinct ?doc)) as ?numDoc";
            where = @$"where {{
                    ?doc a 'document'.
                    ?doc <http://w3id.org/roh/isValidated> 'true'.
				    ?doc <http://purl.org/ontology/bibo/authorList> ?authorList.
				    ?authorList <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
				    ?person a 'person'.
                    ?person <http://w3id.org/roh/isActive> 'true'.
                    FILTER(?person in (<http://gnoss/{string.Join(">,<http://gnoss/", pPersons.Select(x => x.ToUpper()))}>))
                    {string.Join("UNION", filtrosPerfilesTags)}
                }}";
            sparqlObject = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

            foreach (Dictionary<string, SparqlObject.Data> fila in sparqlObject.results.bindings)
            {
                string person = fila["person"].value.Replace("http://gnoss/", "").ToLower();
                string perfil = fila["perfil"].value;                
                
                int numDoc = int.Parse(fila["numDoc"].value);
                if (respuesta[person][perfil].tags == null)
                {
                    respuesta[person][perfil].tags = new();
                }
                if (fila.ContainsKey("tag"))
                {
                    string node = fila["tag"].value;
                    respuesta[person][perfil].tags.Add(node, numDoc);
                }
                respuesta[person][perfil].numPublicaciones += numDoc;
            }


            // Obtener el número de veces que aparecen documentos con los diferentes tags y categorías
            select = "select ?person ?perfil (count(distinct ?doc)) as ?numDoc (count(distinct ?proj)) as ?ipNumber ";
            where = @$"where {{
                        ?person a 'person'.
                        ?person <http://w3id.org/roh/isActive> 'true'.
                        FILTER(?person in (<http://gnoss/{string.Join(">,<http://gnoss/", pPersons.Select(x => x.ToUpper()))}>))
                        OPTIONAL {{
                            ?doc a 'document'.
                            ?doc <http://w3id.org/roh/isValidated> 'true'.
                            ?doc <http://purl.org/ontology/bibo/authorList> ?authorList.
                            ?authorList <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
                        }}
                        OPTIONAL {{
                            ?proj a 'project'.
                            ?proj <http://w3id.org/roh/isValidated> 'true'.
                            ?proj <http://vivoweb.org/ontology/core#relates> ?listprojauth.
                            ?listprojauth <http://w3id.org/roh/roleOf> ?person.
                            ?listprojauth <http://w3id.org/roh/isIP> 'true'.
                        }}
                        {string.Join("UNION", filtrosPerfiles)}
                }}";
            sparqlObject = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

            foreach (Dictionary<string, SparqlObject.Data> fila in sparqlObject.results.bindings)
            {
                string person = fila["person"].value.Replace("http://gnoss/", "").ToLower();
                string perfil = fila["perfil"].value;
                respuesta[person][perfil].numPublicaciones = fila.ContainsKey("numDoc") && fila["numDoc"].value != null ? int.Parse(fila["numDoc"].value) : 0;
                respuesta[person][perfil].ipNumber = fila.ContainsKey("ipNumber") && fila["ipNumber"].value != null ? int.Parse(fila["ipNumber"].value) : 0;
            }


            // Obtener el número de documentos totales por autor
            select = "select ?person (count(distinct ?doc)) as ?numDoc ";
            where = @$"where {{
                    ?doc a 'document'.
                    ?doc <http://w3id.org/roh/isValidated> 'true'.
				    ?doc <http://purl.org/ontology/bibo/authorList> ?authorList.
				    ?authorList <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
				    ?person a 'person'.
                    ?person <http://w3id.org/roh/isActive> 'true'.
                    FILTER(?person in (<http://gnoss/{string.Join(">,<http://gnoss/", pPersons.Select(x => x.ToUpper()))}>))
                }}";
            sparqlObject = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);

            foreach (Dictionary<string, SparqlObject.Data> fila in sparqlObject.results.bindings)
            {
                string person = fila["person"].value.Replace("http://gnoss/", "").ToLower();
                int numDoc = int.Parse(fila["numDoc"].value);
                foreach (string perfil in respuesta[person].Keys)
                {
                    respuesta[person][perfil].numPublicacionesTotal = numDoc;
                }
            }
            foreach (string idperson in respuesta.Keys.ToList())
            {
                respuesta[idperson] = respuesta[idperson].OrderByDescending(x => x.Value.ajuste).ToDictionary(x => x.Key, x => x.Value);
            }
            return respuesta;
        }

        public List<DataItemRelacion> DatosGraficaColaboradoresCluster(Models.Cluster.Cluster pCluster, List<string> pPersons,bool seleccionados)
        {
            List<string> colaboradores = pPersons.Select(x => "http://gnoss/" + x.ToUpper()).ToList();
            if(seleccionados)
            {
                colaboradores = new List<string>();
            }
            if (pCluster.profiles != null)
            {
                foreach (PerfilCluster perfilCluster in pCluster.profiles)
                {
                    if (perfilCluster.users != null)
                    {
                        foreach (PerfilCluster.UserCluster userCluster in perfilCluster.users)
                        {
                            colaboradores.Add("http://gnoss/" + userCluster.shortUserID.ToUpper());
                        }
                    }
                }
            }
            colaboradores = colaboradores.Distinct().ToList();

            //Nodos            
            Dictionary<string, string> dicNodos = new Dictionary<string, string>();
            //Relaciones
            Dictionary<string, List<DataQueryRelaciones>> dicRelaciones = new Dictionary<string, List<DataQueryRelaciones>>();
            //Respuesta
            List<DataItemRelacion> items = new List<DataItemRelacion>();

            string select = "";
            string where = "";
            if (colaboradores.Count > 0)
            {
                #region Cargamos los nodos
                {
                    //Miembros
                    select = $@"{mPrefijos}
                                select distinct ?person ?nombre";
                    where = $@" WHERE
	                            {{
                                    ?person a 'person'.
                                    ?person foaf:name ?nombre.
                                    FILTER(?person in (<{string.Join(">,<", colaboradores)}>))
		            
	                            }}";

                    SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);
                    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                    {
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            if (!dicNodos.ContainsKey(fila["person"].value))
                            {

                                dicNodos.Add(fila["person"].value, fila["nombre"].value);
                            }
                        }
                    }
                }
                #endregion

                #region Relaciones entre miembros DENTRO DEl CLUSTER
                {
                    //Proyectos
                    {
                        select = "SELECT ?person group_concat(distinct ?project;separator=\",\") as ?projects";
                        where = $@"
                    WHERE {{ 
                            ?project a 'project'.
                            ?project ?propRol ?rol.
                            FILTER(?propRol in (<http://w3id.org/roh/researchers>,<http://w3id.org/roh/mainResearchers>))
                            ?rol <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
                            FILTER(?person in (<{string.Join(">,<", colaboradores)}>))
                        }}";
                        SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);
                        Dictionary<string, List<string>> personaProy = new Dictionary<string, List<string>>();
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            string projects = fila["projects"].value;
                            string person = fila["person"].value;
                            personaProy.Add(person, new List<string>(projects.Split(',')));
                        }
                        UtilidadesAPI.ProcessRelations("Proyectos", personaProy, ref dicRelaciones);
                    }
                    //DOCUMENTOS
                    {
                        select = "SELECT ?person group_concat(?document;separator=\",\") as ?documents";
                        where = $@"
                    WHERE {{ 
                            ?document a 'document'.
                            ?document <http://purl.org/ontology/bibo/authorList> ?authorList.
                            ?authorList <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> ?person.
                            FILTER(?person in (<{string.Join(">,<", colaboradores)}>))
                        }}";
                        SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mIdComunidad);
                        Dictionary<string, List<string>> personaDoc = new Dictionary<string, List<string>>();
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            string documents = fila["documents"].value;
                            string person = fila["person"].value;
                            personaDoc.Add(person, new List<string>(documents.Split(',')));
                        }
                        UtilidadesAPI.ProcessRelations("Documentos", personaDoc, ref dicRelaciones);
                    }
                }
                #endregion



                int maximasRelaciones = 0;
                foreach (KeyValuePair<string, List<DataQueryRelaciones>> sujeto in dicRelaciones)
                {
                    foreach (DataQueryRelaciones relaciones in sujeto.Value)
                    {
                        foreach (Datos relaciones2 in relaciones.idRelacionados)
                        {
                            maximasRelaciones = Math.Max(maximasRelaciones, relaciones2.numVeces);
                        }
                    }
                }

                // Nodos. 
                if (dicNodos != null && dicNodos.Count > 0)
                {
                    foreach (KeyValuePair<string, string> nodo in dicNodos)
                    {
                        string clave = nodo.Key;
                        Models.Graficas.DataItemRelacion.Data.Type type = Models.Graficas.DataItemRelacion.Data.Type.none;
                        type = Models.Graficas.DataItemRelacion.Data.Type.icon_member;
                        Models.Graficas.DataItemRelacion.Data data = new Models.Graficas.DataItemRelacion.Data(clave, nodo.Value, null, null, null, "nodes", type);
                        DataItemRelacion dataColabo = new DataItemRelacion(data, true, true);
                        items.Add(dataColabo);
                    }
                }

                // Relaciones.
                if (dicRelaciones != null && dicRelaciones.Count > 0)
                {
                    foreach (KeyValuePair<string, List<DataQueryRelaciones>> sujeto in dicRelaciones)
                    {
                        foreach (DataQueryRelaciones relaciones in sujeto.Value)
                        {
                            foreach (Datos relaciones2 in relaciones.idRelacionados)
                            {
                                string id = $@"{sujeto.Key}~{relaciones.nombreRelacion}~{relaciones2.idRelacionado}~{relaciones2.numVeces}";
                                Models.Graficas.DataItemRelacion.Data.Type type = Models.Graficas.DataItemRelacion.Data.Type.none;
                                if (relaciones.nombreRelacion == "Proyectos")
                                {
                                    type = Models.Graficas.DataItemRelacion.Data.Type.relation_project;
                                }
                                else if (relaciones.nombreRelacion == "Documentos")
                                {
                                    type = Models.Graficas.DataItemRelacion.Data.Type.relation_document;
                                }
                                Models.Graficas.DataItemRelacion.Data data = new Models.Graficas.DataItemRelacion.Data(id, relaciones.nombreRelacion, sujeto.Key, relaciones2.idRelacionado, UtilidadesAPI.CalcularGrosor(maximasRelaciones, relaciones2.numVeces), "edges", type);
                                DataItemRelacion dataColabo = new DataItemRelacion(data, null, null);
                                items.Add(dataColabo);
                            }
                        }
                    }
                }
            }
            return items;



        }

        /// <summary>
        /// Método público buscar diferentes tags
        /// </summary>
        /// <param name="pSearch">parámetro que corresponde a la cadena de búsqueda.</param>
        /// <returns>Listado de etiquetas de resultado.</returns>
        public List<string> SearchTags(string pSearch)
        {
            int numMax = 20;
            string searchText = pSearch.Trim();
            string filter = "";
            if (!pSearch.EndsWith(' '))
            {
                string[] splitSearch = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (splitSearch.Length > 1)
                {
                    searchText = searchText.Substring(0, searchText.LastIndexOf(' '));
                    if (splitSearch.Last().Length > 3)
                    {
                        searchText += " " + splitSearch.Last() + "*";
                    }
                    else
                    {
                        filter = $" AND lcase(?o) like \"% { splitSearch.Last() }%\" ";
                    }
                }
                else if (searchText.Length > 3)
                {
                    searchText += "*";
                }
                else // Si tiene menos de 4 caracteres y no termina en espacio, buscamos por like
                {
                    filter = $"  lcase(?o) like \"{ searchText }%\" OR lcase(?o) like \"% { searchText }%\" ";
                    searchText = "";
                }
            }
            if (searchText != "")
            {
                filter = $"bif:contains(?o, \"'{ searchText }'\"){filter}";
            }
            string select = "SELECT DISTINCT ?s ?o ";
            string where = $"WHERE {{ ?s a <http://purl.org/ontology/bibo/Document>. ?s <http://vivoweb.org/ontology/core#freeTextKeyword> ?freeTextKeyword. ?freeTextKeyword <http://w3id.org/roh/title> ?o. FILTER( {filter} )    }} ORDER BY ?o";
            SparqlObject sparqlObjectAux = mResourceApi.VirtuosoQuery(select, where, "document");
            List<string> resultados = sparqlObjectAux.results.bindings.Select(x => x["o"].value).Distinct().ToList();
            if (resultados.Count() > numMax)
            {
                resultados = resultados.ToList().GetRange(0, numMax);
            }
            return resultados;
        }


        #region Métodos de recolección de datos

        /// <summary>
        /// Método privado para obtener los tesauros.
        /// </summary>
        /// <param name="pListaTesauros">Listado de thesaurus a obtener.</param>
        /// <returns>Diccionario con las listas de thesaurus.</returns>
        private Dictionary<string, List<ThesaurusItem>> GetTesauros(List<string> pListaTesauros)
        {
            Dictionary<string, List<ThesaurusItem>> elementosTesauros = new Dictionary<string, List<ThesaurusItem>>();

            foreach (string tesauro in pListaTesauros)
            {
                string select = "select * ";
                string where = @$"where {{
                    ?s a <http://www.w3.org/2008/05/skos#Concept>.
                    ?s <http://www.w3.org/2008/05/skos#prefLabel> ?nombre.
                    ?s <http://purl.org/dc/elements/1.1/source> '{tesauro}'
                    OPTIONAL {{ ?s <http://www.w3.org/2008/05/skos#broader> ?padre }}
                }} ORDER BY ?padre ?s ";
                SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "taxonomy");

                List<ThesaurusItem> items = sparqlObject.results.bindings.Select(x => new ThesaurusItem()
                {
                    id = x["s"].value,
                    name = x["nombre"].value,
                    parentId = x.ContainsKey("padre") ? x["padre"].value : ""
                }).ToList();

                elementosTesauros.Add(tesauro, items);
            }

            return elementosTesauros;
        }


        private List<string> LoadCurrentTerms(List<string> terms)
        {

            string termsTxt = String.Join(',', terms.Select(e => "<" + e + ">"));

            string select = "select ?o";
            string where = @$"where {{
                ?s a <http://w3id.org/roh/CategoryPath>.
                ?s <http://w3id.org/roh/categoryNode> ?o.
                FILTER(?s IN ({termsTxt}))
            }}";
            SparqlObject sparqlObject = mResourceApi.VirtuosoQuery(select, where, "cluster");

            List<string> termsRes = new();

            sparqlObject.results.bindings.ForEach(e =>
            {
                termsRes.Add(e["o"].value);
            });




            return termsRes;
        }


        /// <summary>
        /// Obtiene la categoría padre.
        /// </summary>
        /// <param name="pIdTesauro">Categoría a consultar.</param>
        /// <returns>Categoría padre.</returns>
        private string ObtenerIdTesauro(string pIdTesauro)
        {
            string idTesauro = pIdTesauro.Split(new[] { "researcharea_" }, StringSplitOptions.None)[1];
            int num1 = Int32.Parse(idTesauro.Split('.')[0]);
            int num2 = Int32.Parse(idTesauro.Split('.')[1]);
            int num3 = Int32.Parse(idTesauro.Split('.')[2]);
            int num4 = Int32.Parse(idTesauro.Split('.')[3]);

            if (num4 != 0)
            {
                idTesauro = $@"{mResourceApi.GraphsUrl}items/researcharea_{num1}.{num2}.{num3}.0";
            }
            else if (num3 != 0 && num4 == 0)
            {
                idTesauro = $@"{mResourceApi.GraphsUrl}items/researcharea_{num1}.{num2}.0.0";
            }
            else if (num2 != 0 && num3 == 0 && num4 == 0)
            {
                idTesauro = $@"{mResourceApi.GraphsUrl}items/researcharea_{num1}.0.0.0";
            }

            return idTesauro;
        }

        /// <summary>
        /// Obtiene las categorías del tesáuro.
        /// </summary>
        /// <returns>Tupla con (clave) diccionario de las idCategorias-idPadre y (valor) diccionario de nombreCategoria-idCategoria.</returns>
        private static Tuple<Dictionary<string, string>, Dictionary<string, string>> ObtenerDatosTesauro()
        {
            Dictionary<string, string> dicAreasBroader = new Dictionary<string, string>();
            Dictionary<string, string> dicAreasNombre = new Dictionary<string, string>();

            string select = @"SELECT DISTINCT * ";
            string where = @$"WHERE {{
                ?concept a <http://www.w3.org/2008/05/skos#Concept>.
                ?concept <http://www.w3.org/2008/05/skos#prefLabel> ?nombre.
                ?concept <http://purl.org/dc/elements/1.1/source> 'researcharea'
                OPTIONAL{{?concept <http://www.w3.org/2008/05/skos#broader> ?broader}}
                }}";
            SparqlObject resultado = mResourceApi.VirtuosoQuery(select, where, "taxonomy");

            foreach (Dictionary<string, SparqlObject.Data> fila in resultado.results.bindings)
            {
                string concept = fila["concept"].value;
                string nombre = fila["nombre"].value;
                string broader = "";
                if (fila.ContainsKey("broader"))
                {
                    broader = fila["broader"].value;
                }
                dicAreasBroader.Add(concept, broader);
                if (!dicAreasNombre.ContainsKey(nombre.ToLower()))
                {
                    dicAreasNombre.Add(nombre.ToLower(), concept);
                }
            }

            Dictionary<string, string> dicAreasUltimoNivel = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> item in dicAreasNombre)
            {
                bool tieneHijos = false;
                string id = item.Value.Split(new[] { "researcharea_" }, StringSplitOptions.None)[1];
                int num1 = Int32.Parse(id.Split('.')[0]);
                int num2 = Int32.Parse(id.Split('.')[1]);
                int num3 = Int32.Parse(id.Split('.')[2]);
                int num4 = Int32.Parse(id.Split('.')[3]);

                if (num2 == 0 && num3 == 0 && num4 == 0)
                {
                    tieneHijos = dicAreasNombre.ContainsValue($@"{mResourceApi.GraphsUrl}items/researcharea_{num1}.1.0.0");
                }
                else if (num3 == 0 && num4 == 0)
                {
                    tieneHijos = dicAreasNombre.ContainsValue($@"{mResourceApi.GraphsUrl}items/researcharea_{num1}.{num2}.1.0");
                }
                else if (num4 == 0)
                {
                    tieneHijos = dicAreasNombre.ContainsValue($@"{mResourceApi.GraphsUrl}items/researcharea_{num1}.{num2}.{num3}.1");
                }

                if (!tieneHijos)
                {
                    dicAreasUltimoNivel.Add(item.Key, item.Value);
                }
            }

            return new Tuple<Dictionary<string, string>, Dictionary<string, string>>(dicAreasBroader, dicAreasUltimoNivel);
        }

        #endregion


    }
}
