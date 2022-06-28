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

        public List<string> GetSimilaritiesDocument(string pIdDocument, ConfigService pConfig)
        {
            List<string> listID = new List<string>();

            // Cliente.
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromDays(1);

            EnrichmentSimilarityGet enrichmentSimilarityGet = new EnrichmentSimilarityGet();
            enrichmentSimilarityGet.ro_id = pIdDocument;
            enrichmentSimilarityGet.ro_type_target = "research_paper";

            // Conversión de los datos.
            string informacion = JsonConvert.SerializeObject(enrichmentSimilarityGet);
            StringContent contentData = new StringContent(informacion, System.Text.Encoding.UTF8, "application/json");

            var responsePost = client.PostAsync(pConfig.GetUrlSimilarity(), contentData).Result;
            if (responsePost.IsSuccessStatusCode)
            {
                EnrichmentSimilarityGetResponse responseGetObject = responsePost.Content.ReadAsAsync<EnrichmentSimilarityGetResponse>().Result;
                listID = responseGetObject.similar_ros_calculado.Keys.ToList();
                listID.Remove(pIdDocument);
            }
            if (listID.Count > 0)
            {
                string select = "select distinct ?doc from <http://gnoss.com/curriculumvitae.owl> ";
                string where = $@"
where{{
    FILTER(?doc in (<{string.Join(">,<", listID)}>))
    ?doc a <http://purl.org/ontology/bibo/Document>.
    {{ ?doc <http://w3id.org/roh/isValidated> 'true'.}}
    UNION
    {{  
        ?cv a <http://w3id.org/roh/CV>.
        ?cv ?p1 ?o1.
        ?o1 ?p2 ?item.
        ?item <http://vivoweb.org/ontology/core#relatedBy> ?doc.
        ?item <http://w3id.org/roh/isPublic> 'true'.
    }}

}}";
                listID = listID.Intersect(mResourceApi.VirtuosoQuery(select, where, "document").results.bindings.Select(x => x["doc"].value)).ToList();
                listID = listID.Select(x => mResourceApi.GetShortGuid(x)).ToList();
            }
            return listID;
        }

    }
}
