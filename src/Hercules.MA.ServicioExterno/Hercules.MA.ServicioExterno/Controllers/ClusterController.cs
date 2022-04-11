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
        [HttpPost("SaveCluster")]
        public IActionResult SaveCluster([Required] string pIdGnossUser, [FromForm] Cluster pDataCluster)
        {
            string idClusterRes = string.Empty;
            try
            {
                AccionesCluster accionCluster = new AccionesCluster();
                idClusterRes = accionCluster.SaveCluster(pIdGnossUser, pDataCluster);
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(idClusterRes);
        }


        /// <summary>
        /// Controlador para guardar los datos del cluster.
        /// </summary>
        /// <param name="pIdClusterId">Usuario de gnoss.</param>
        /// <returns>Id del cluster creado o modificado.</returns>
        [HttpGet("LoadCluster")]
        public IActionResult LoadCluster([Required] string pIdClusterId)
        {
            Cluster idClusterRes;
            try
            {
                AccionesCluster accionCluster = new AccionesCluster();
                idClusterRes = accionCluster.LoadCluster(pIdClusterId);
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(idClusterRes);
        }

        [HttpPost("LoadProfiles")]
        public IActionResult LoadProfiles([FromForm] Cluster pDataCluster, [FromForm] List<string> pPersons)
        {
            try
            {
                AccionesCluster accionCluster = new AccionesCluster();
                return Ok( accionCluster.LoadProfiles(pDataCluster, pPersons));
            }
            catch (Exception)
            {
                throw;
            }
        }



        /// <summary>
        /// Controlador que sugiere etiquetas con la búsqueda dada
        /// </summary>
        /// <param name="tagInput">Texto para la búsqueda de etiquetas.</param>
        /// <returns>Listado de las etiquetas de resultado.</returns>
        [HttpGet("searchTags")]
        public IActionResult SearchTags([Required] string tagInput)
        {
            List<string> idClusterRes = new();
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
