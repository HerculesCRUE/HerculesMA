using Hercules.MA.ServicioExterno.Controllers.Acciones;
using Hercules.MA.ServicioExterno.Models.Cluster;
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

        /// <summary>
        /// Controlador para guardar los datos del cluster.
        /// </summary>
        /// <param name="pIdGnossUser">Usuario de gnoss.</param>
        /// <param name="pDataCluster">Datos a añadir / modificar.</param>
        /// <returns>Id del cluster creado o modificado.</returns>
        [HttpPost("SetSection1")]
        public IActionResult SetSection1([Required] string pIdGnossUser, [FromForm] Cluster pDataCluster)
        {
            string idClusterRes = string.Empty;
            try
            {
                AccionesCluster accionCluster = new AccionesCluster();
                idClusterRes = accionCluster.SaveStep1Cluster(pIdGnossUser, pDataCluster);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(idClusterRes);
        }




        /// <summary>
        /// Controlador que sugiere etiquetas con la búsqueda dada
        /// </summary>
        /// <param name="tagInput">Texto para la búsqueda de etiquetas.</param>
        /// <returns>Listado de las etiquetas de resultado.</returns>
        [HttpGet("searchTags")]
        public IActionResult SearchTags([Required] string tagInput)
        {
            List<TagsItem> idClusterRes = new();
            try
            {
                AccionesCluster accionCluster = new AccionesCluster();
                idClusterRes = accionCluster.SearchTags(tagInput);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(idClusterRes);
        }

    }
}
