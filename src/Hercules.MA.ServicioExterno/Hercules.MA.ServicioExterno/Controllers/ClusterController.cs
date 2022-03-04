using Hercules.MA.ServicioExterno.Controllers.Acciones;
using Hercules.MA.ServicioExterno.Models.Cluster;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Hercules.MA.ServicioExterno.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class ClusterController : ControllerBase
    {
        // GET: ClusterController
        /// <summary>
        /// Controlador para obtener los thesaurus usados en el cluster.
        /// </summary>
        /// <param name="listThesaurus">listado de thesaurus</param>
        /// <returns>Diccionario con los datos.</returns>
        [HttpGet("GetThesaurus")]
        public IActionResult GetThesaurus(string listThesaurus = "")
        {
            Dictionary<string, List<ThesaurusItem>> datosThesaurus = null;

            try
            {
                AccionesCluster cluster = new AccionesCluster();
                datosThesaurus = cluster.GetListThesaurus(listThesaurus);
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(datosThesaurus);
        }
        
    }
}
