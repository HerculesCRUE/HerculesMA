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
        /// Borra una oferta
        /// </summary>
        /// <param name="pIdOfferId">Id de la oferta a borrar.</param>
        /// <returns>Un booleano si ha sido borrado.</returns>
        [HttpGet("BorrarOferta")]
        public IActionResult BorrarOferta([Required] string pIdOfferId)
        {

            bool borrado = false;

            try
            {
                AccionesOferta accionCluster = new AccionesOferta();
                borrado = accionCluster.BorrarOferta(pIdOfferId);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(borrado);
        }


        /// <summary>
        /// Cambiar el estado de una oferta
        /// </summary>
        /// <param name="pIdOfferId">Id de la oferta a modificar.</param>
        /// <param name="estado">Id del estado al que se quiere establecer.</param>
        /// <param name="estadoActual">Id del estado que tiene actualmente (Necesario para la modificación del mismo).</param>
        /// <param name="pIdGnossUser">Id del usuario que modifica el estado, necesario para actualizar el historial.</param>
        /// <returns>String con el id del nuevo estado.</returns>
        [HttpPost("CambiarEstado")]
        public IActionResult CambiarEstado([FromForm] string pIdOfferId, [FromForm] string estado, [FromForm] string estadoActual, [FromForm] Guid pIdGnossUser)
        {

            string cambiado = "";

            try
            {
                AccionesOferta accionCluster = new AccionesOferta();
                cambiado = accionCluster.CambiarEstado(pIdOfferId, estado, estadoActual, pIdGnossUser);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(cambiado);
        }

        /// <summary>
        /// Controlador para guardar los datos de la oferta.
        /// </summary>
        /// <param name="pIdOfertaId">Id de la oferta.</param>
        /// <returns>Objeto "leible" de la oferta.</returns>
        [HttpGet("LoadOffer")]
        public IActionResult LoadOffer([Required] string pIdOfertaId)
        {
            Offer Oferta;
            try
            {
                AccionesOferta accionCluster = new AccionesOferta();
                Oferta = accionCluster.LoadOffer(pIdOfertaId);
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(Oferta);
        }

        /// <summary>
        /// Controlador para Obtener los usuarios del/los grupos de un investigador
        /// </summary>
        /// <param name="pIdUserId">Usuario investigador.</param>
        /// <returns>Diccionario con los datos necesarios para cada persona.</returns>
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


        /// <summary>
        /// Controlador para guardar los datos de la oferta 
        /// </summary>
        /// <param name="pIdGnossUser">Usuario de gnoss.</param>
        /// <param name="oferta">Objeto con la oferta tecnológica a añadir / modificar.</param>
        /// <returns>Id de la oferta creada o modificada.</returns>
        [HttpPost("SaveOffer")]
        [Produces("application/json")]
        public IActionResult SaveOffer([FromForm] string pIdGnossUser, [FromForm] Offer oferta)
        {
            try
            {
                AccionesOferta accionOferta = new AccionesOferta();
                return Ok(accionOferta.SaveOffer(pIdGnossUser, oferta));
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
