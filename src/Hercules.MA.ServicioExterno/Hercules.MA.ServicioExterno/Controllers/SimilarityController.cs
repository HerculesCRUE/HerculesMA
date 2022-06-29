using Hercules.MA.ServicioExterno.Controllers.Acciones;
using Hercules.MA.ServicioExterno.Models.Cluster;
using Hercules.MA.ServicioExterno.Models.Graficas.DataGraficaAreasTags;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hercules.MA.ServicioExterno.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class SimilarityController : ControllerBase
    {
        readonly ConfigService _Configuracion;

        public SimilarityController(ConfigService pConfig)
        {
            _Configuracion = pConfig;
        }

        /// <summary>
        /// Obtiene los documentos similares
        /// </summary>
        /// <param name="pId">ID del documento</param>
        /// <returns></returns>
        [HttpGet("GetSimilaritiesDocument")]
        public IActionResult GetSimilaritiesDocument(string pId)
        {
            Dictionary<string, Dictionary<string, float>> listID = new Dictionary<string, Dictionary<string, float>>();

            try
            {
                AccionesSimilarity accionesSimilarity = new AccionesSimilarity();
                listID = accionesSimilarity.GetSimilarities(pId, _Configuracion, "research_paper");
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(listID);
        }

        /// <summary>
        /// Obtiene los research object similares
        /// </summary>
        /// <param name="pId">ID del researchobject</param>
        /// <returns></returns>
        [HttpGet("GetSimilaritiesResearchObject")]
        public IActionResult GetSimilaritiesResearchObject(string pId)
        {
            Dictionary<string, Dictionary<string, float>> listID = new Dictionary<string, Dictionary<string, float>>();

            try
            {
                AccionesSimilarity accionesSimilarity = new AccionesSimilarity();
                listID = accionesSimilarity.GetSimilarities(pId, _Configuracion, "code_project");
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(listID);
        }
    }
}
