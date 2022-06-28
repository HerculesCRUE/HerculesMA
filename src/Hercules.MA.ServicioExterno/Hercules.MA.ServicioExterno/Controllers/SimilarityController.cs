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
        /// <param name="pIdDocument">ID del socumento</param>
        /// <returns></returns>
        [HttpGet("GetSimilaritiesDocument")]
        public IActionResult GetSimilaritiesDocument(string pIdDocument)
        {
            List<string> listID = new List<string>();

            try
            {
                AccionesSimilarity accionesSimilarity = new AccionesSimilarity();
                listID = accionesSimilarity.GetSimilaritiesDocument(pIdDocument, _Configuracion);
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(listID);
        }
    }
}
