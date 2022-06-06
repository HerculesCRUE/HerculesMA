using Hercules.MA.ServicioExterno.Controllers.Acciones;
using Hercules.MA.ServicioExterno.Models.Offer;
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
    public class OfertasController : ControllerBase
    {


        /// <summary>
        /// Controlador para guardar los datos del cluster.
        /// </summary>
        /// <param name="pIdClusterId">Usuario de gnoss.</param>
        /// <returns>Id del cluster creado o modificado.</returns>
        [HttpGet("LoadCluster")]
        public IActionResult LoadOffer([Required] string pIdClusterId)
        {
            Offer idClusterRes;
            try
            {
                AccionesCluster accionCluster = new AccionesCluster();
                idClusterRes = null;
                //idClusterRes = accionCluster.LoadCluster(pIdClusterId);
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(idClusterRes);
        }

        /// <summary>
        /// Controlador para Obtener los usuarios del/los grupos de un investigador
        /// </summary>
        /// <param name="pIdUserId">Usuario investigador.</param>
        /// <returns>Id del cluster creado o modificado.</returns>
        [HttpGet("LoadUsersGroup")]
        public IActionResult LoadUsers([Required] string pIdUserId)
        {
            try
            {
                AccionesOferta accionOferta = new AccionesOferta();
                return Ok(accionOferta.LoadUsers(pIdUserId));
            }
            catch (Exception)
            {
                throw;
            }
        }



        /// <summary>
        /// Controlador para Obtener las líneas de invetigación de los grupos de los usuarios investigadores dados
        /// </summary>
        /// <param name="pIdUsersId">Usuarios investigadores.</param>
        /// <returns>Listado de las líneas de investigación.</returns>
        [HttpPost("LoadLineResearchs")]
        [Produces("application/json")]
        public IActionResult LoadLineResearchs([FromForm] string[] pIdUsersId)
        {
            try
            {
                AccionesOferta accionOferta = new AccionesOferta();
                return Ok(accionOferta.LoadLineResearchs(pIdUsersId));
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Controlador para Obtener los sectores de encuadre
        /// </summary>
        /// <param name="lang">Idioma a cargar.</param>
        /// <returns>Listado de las líneas de investigación.</returns>
        [HttpGet("LoadFramingSectors")]
        public IActionResult LoadFramingSectors(string lang)
        {
            try
            {
                AccionesOferta accionOferta = new AccionesOferta();
                return Ok(accionOferta.LoadFramingSectors(lang));
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Controlador para Obtener los estados de madurez
        /// </summary>
        /// <param name="lang">Idioma a cargar.</param>
        /// <returns>Listado de las líneas de investigación.</returns>
        [HttpGet("LoadMatureStates")]
        public IActionResult LoadMatureStates(string lang)
        {
            try
            {
                AccionesOferta accionOferta = new AccionesOferta();
                return Ok(accionOferta.LoadMatureStates(lang));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
