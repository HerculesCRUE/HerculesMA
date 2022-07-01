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
        public List<KeyValuePair<Guid, Dictionary<string, float>>> GetSimilarities(string pId, ConfigService pConfig, string pType)
        {
            if (!string.IsNullOrEmpty(pConfig.GetUrlSimilarity()))
            {
                UtilsSimilarity utilsSimilarity = new UtilsSimilarity(pConfig.GetUrlSimilarity(), mResourceApi, pType);
                return utilsSimilarity.GetSimilars(pId);
            }
            else
            {
                return new List<KeyValuePair<Guid, Dictionary<string, float>>>();
            }
        }
    }
}
