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
using Hercules.MA.ServicioExterno.Models.Graficas.DataGraficaAreasTags;
using Microsoft.AspNetCore.Cors;
using System.Net.Http;
using Hercules.MA.ServicioExterno.Models.Similarity;

namespace Hercules.MA.ServicioExterno.Controllers.Acciones
{
    [EnableCors("_myAllowSpecificOrigins")]
    public class AccionesSimilarity
    {
        #region --- Constantes   
        private static string RUTA_OAUTH = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config/ConfigOAuth/OAuthV3.config";
        private static ResourceApi mResourceApi = new ResourceApi(RUTA_OAUTH);
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pId">ID del elemento al que buscar similares</param>
        /// <param name="pConfig">Config</param>
        /// <param name="pType">Tipo: 'research_paper' o 'code_project'</param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, float>> GetSimilarities(string pId, ConfigService pConfig, string pType)
        {
            Dictionary<string, Dictionary<string, float>> dicSimilars = new Dictionary<string, Dictionary<string, float>>();

            string rdfType = "";
            string graph = "";
            switch (pType)
            {
                case "research_paper":
                    rdfType = "http://purl.org/ontology/bibo/Document";
                    graph = "";
                    break;
                case "code_project":
                    rdfType = "http://w3id.org/roh/ResearchObject";
                    graph = "researchobject";
                    break;
                default:
                    throw new Exception("El tipo " + pType + " no está permitido para buscar similitudes, sólo está permitido 'research_paper' o 'code_project'");
            }

            // Cliente.
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromDays(1);

            EnrichmentSimilarityGet enrichmentSimilarityGet = new EnrichmentSimilarityGet();
            enrichmentSimilarityGet.ro_id = pId;
            enrichmentSimilarityGet.ro_type_target = pType;

            // Conversión de los datos.
            string informacion = JsonConvert.SerializeObject(enrichmentSimilarityGet);
            StringContent contentData = new StringContent(informacion, System.Text.Encoding.UTF8, "application/json");

            var responsePost = client.PostAsync(pConfig.GetUrlSimilarity(), contentData).Result;
            if (responsePost.IsSuccessStatusCode)
            {
                EnrichmentSimilarityGetResponse responseGetObject = responsePost.Content.ReadAsAsync<EnrichmentSimilarityGetResponse>().Result;
                dicSimilars = responseGetObject.similar_ros_calculado;
                dicSimilars.Remove(pId);
            }
            if (dicSimilars.Count > 0)
            {
                //Hacemos una verificación para que sólo se devuelvan validados o publicios en el cv
                string select = "select distinct ?id from <http://gnoss.com/curriculumvitae.owl> ";
                string where = $@"
where{{
    FILTER(?id in (<{string.Join(">,<", dicSimilars.Keys)}>))
    ?id a <{rdfType}>.
    {{ ?id <http://w3id.org/roh/isValidated> 'true'.}}
    UNION
    {{  
        ?cv a <http://w3id.org/roh/CV>.
        ?cv ?p1 ?o1.
        ?o1 ?p2 ?item.
        ?item <http://vivoweb.org/ontology/core#relatedBy> ?id.
        ?item <http://w3id.org/roh/isPublic> 'true'.
    }}

}}";
                List<string> listID = mResourceApi.VirtuosoQuery(select, where, graph).results.bindings.Select(x => x["doc"].value).ToList();
                dicSimilars = dicSimilars.Where(x => listID.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
            }
            return dicSimilars;
        }
    }
}
