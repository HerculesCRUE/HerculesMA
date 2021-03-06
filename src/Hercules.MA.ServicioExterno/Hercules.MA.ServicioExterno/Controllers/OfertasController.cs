using Hercules.MA.ServicioExterno.Controllers.Acciones;
using Hercules.MA.ServicioExterno.Models.Offer;
using Hercules.MA.ServicioExterno.Models.Graficas.DataGraficaAreasTags;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Hercules.MA.ServicioExterno.Models.Cluster;

namespace Hercules.MA.ServicioExterno.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class OfertasController : ControllerBase
    {



        /// <summary>
        /// Controlador para obtener los thesaurus usados las ofertas.
        /// </summary>
        /// <param name="listThesaurus">Elemento padre que define el thesaurus</param>
        /// <param name="lang">Idioma para los thesaurus multiidioma </param>
        /// <returns>Diccionario con los datos.</returns>
        [HttpPost("GetThesaurus")]
        public IActionResult GetThesaurus([FromForm] List<string> listThesaurus, [FromForm] string lang = "es")
        {
            Dictionary<string, List<ThesaurusItem>> datosThesaurus = null;

            try
            {
                AccionesOferta cluster = new AccionesOferta();
                datosThesaurus = cluster.GetListThesaurus(listThesaurus, lang);
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(datosThesaurus);
        }


        /// <summary>
        /// Borra una oferta
        /// </summary>
        /// <param name="pIdOfferId">Id (Guid) de la oferta a borrar.</param>
        /// <param name="pIdGnossUser">Id del usuario que realiza la acción.</param>
        /// <returns>Un booleano si ha sido borrado.</returns>
        [HttpPost("BorrarOferta")]
        public IActionResult BorrarOferta([FromForm] string pIdOfferId, [FromForm] Guid pIdGnossUser)
        {

            bool borrado = false;

            try
            {
                AccionesOferta accionCluster = new AccionesOferta();
                borrado = accionCluster.BorrarOferta(pIdOfferId, pIdGnossUser);
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
        /// <param name="pIdOfferId">Id(GUID) de la oferta a modificar.</param>
        /// <param name="estado">Id del estado al que se quiere establecer.</param>
        /// <param name="estadoActual">Id del estado que tiene actualmente (Necesario para la modificación del mismo).</param>
        /// <param name="pIdGnossUser">Id del usuario que modifica el estado, necesario para actualizar el historial.</param>
        /// <param name="texto">Texto de la notificación.</param>
        /// <returns>String con el id del nuevo estado.</returns>
        [HttpPost("CambiarEstado")]
        public IActionResult CambiarEstado([FromForm] string pIdOfferId, [FromForm] string estado, [FromForm] string estadoActual, [FromForm] Guid pIdGnossUser, [FromForm] string texto = "")
        {

            string cambiado = "";

            try
            {
                AccionesOferta accionCluster = new AccionesOferta();
                cambiado = accionCluster.CambiarEstado(pIdOfferId, estado, estadoActual, pIdGnossUser, texto);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(cambiado);
        }

        /// <summary>
        /// Cambiar el estado de una oferta
        /// </summary>
        /// <param name="pIdOfferIds">Ids (array de GUIDs) de las ofertas a modificar.</param>
        /// <param name="estado">Id del estado al que se quiere establecer.</param>
        /// <param name="estadoActual">Id del estado que tiene actualmente (Necesario para la modificación del mismo).</param>
        /// <param name="pIdGnossUser">Id del usuario que modifica el estado, necesario para actualizar el historial.</param>
        /// <param name="texto">Texto de la notificación.</param>
        /// <returns>String con el id del nuevo estado.</returns>
        [HttpPost("CambiarEstadoAll")]
        public IActionResult CambiarEstadoAll([FromForm] Guid[] pIdOfferIds, [FromForm] string estado, [FromForm] string estadoActual, [FromForm] Guid pIdGnossUser, [FromForm] string texto = "")
        {

            bool cambiado = true;

            try
            {
                AccionesOferta accionCluster = new AccionesOferta();
                foreach (var pIdOfferId in pIdOfferIds)
                {
                    cambiado = cambiado && accionCluster.CambiarEstado(pIdOfferId.ToString(), estado, estadoActual, pIdGnossUser, texto) != String.Empty;
                }
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
        /// Controlador para Obtener los estados de madurez de las ofertas tecnológicas
        /// </summary>
        /// <param name="lang">Idioma a cargar.</param>
        /// <returns>Listado de los estados de madurez.</returns>
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
        /// Controlador para crear/actualizar los datos de la oferta 
        /// </summary>
        /// <param name="pIdGnossUser">Usuario de gnoss que realiza la acción.</param>
        /// <param name="oferta">Objeto con la oferta tecnológica a crear/actualizar.</param>
        /// <returns>Id de la oferta creada o modificada.</returns>
        [HttpPost("SaveOffer")]
        [Produces("application/json")]
        public IActionResult SaveOffer([FromForm] Guid pIdGnossUser, [FromForm] Offer oferta)
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


        /// <summary>
        /// Controlador para crear/actualizar los datos de la oferta 
        /// </summary>
        /// <param name="idRecurso">.</param>
        /// <param name="nuevoEstado">.</param
        /// <param name="estadoActual">.</param>
        /// <param name="predicado">.</param>
        /// <param name="pIdGnossUser">.</param>
        /// <returns></returns>
        [HttpPost("ModificarTripleteUsuario")]
        public IActionResult ModificarTripleteUsuario([FromForm] string idRecurso, [FromForm] string nuevoEstado, [FromForm] string estadoActual, [FromForm] string predicado, [FromForm] Guid pIdGnossUser)
        {
            try
            {
                AccionesOferta accionOferta = new AccionesOferta();
                return Ok(true);
                // return Ok(accionOferta.ModificarTripleteUsuario(idRecurso, nuevoEstado, estadoActual, predicado, pIdGnossUser));
            }
            catch (Exception)
            {
                throw;
            }
        }




        /// <summary>
        /// Controlador que lista el perfil de usuarios al que pertenece el usuario actual respecto a una oferta tecnológica dada 
        /// </summary>
        /// <param name="pIdOfertaId">Id de la oferta tecnológica.</param>
        /// <param name="userId">Usuario de gnoss que realiza la acción.</param>
        /// <returns>Objeto json.</returns>
        [HttpPost("GetUserProfileInOffer")]
        public IActionResult GetUserProfileInOffer([FromForm] string pIdOfertaId, [FromForm] Guid userId)
        {
            try
            {
                AccionesOferta accionOferta = new AccionesOferta();
                // return Ok(true);
                return Ok(accionOferta.CheckUpdateActionsOffer(pIdOfertaId, userId));
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
