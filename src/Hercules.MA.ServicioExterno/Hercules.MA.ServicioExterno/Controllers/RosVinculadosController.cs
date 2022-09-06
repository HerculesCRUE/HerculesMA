﻿using Hercules.MA.ServicioExterno.Controllers.Acciones;
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
    public class RosVinculadosController : ControllerBase
    {


        /// <summary>
        /// Borra un vínculo
        /// </summary>
        /// <param name="resourceRO">Id (Guid) del RO relacionado.</param>
        /// <param name="pIdROId">Id (Guid) del RO a eliminar de vinculados.</param>
        /// <param name="pIdGnossUser">Id del usuario que realiza la acción.</param>
        /// <returns>Un booleano si ha sido borrado.</returns>
        [HttpPost("DeleteLinked")]
        public IActionResult DeleteLinked([FromForm] string resourceRO, [FromForm] string pIdROId, [FromForm] Guid pIdGnossUser)
        {

            bool borrado;
            try
            {
                AccionesRosVinculados accionesRosVinculados = new();
                borrado = accionesRosVinculados.DeleteLinked(resourceRO, pIdROId, pIdGnossUser);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(borrado);
        }


        /// <summary>
        /// Controlador para Obtener los ROs vinculados de un RO en concreto
        /// </summary>
        /// <param name="pIdROId">ID del RO a obtener las relaciones.</param>
        /// <param name="lang">Idioma de los literales para la consulta</param>
        /// <returns>Listado de los RO vinculados.</returns>
        [HttpGet("LoadRosLinked")]
        public IActionResult LoadRosLinked(string pIdROId, string lang = "es")
        {
            try
            {
                AccionesRosVinculados accionesRosVinculados = new();
                return Ok(accionesRosVinculados.LoadRosLinked(pIdROId, lang));
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Controlador para crear una vinculación 
        /// </summary>
        /// <param name="resourceRO">Id (Guid) del RO relacionado.</param>
        /// <param name="pIdROId">Id (Guid) del RO a añadir a vinculados.</param>
        /// <param name="pIdGnossUser">Id del usuario que realiza la acción.</param>
        /// <returns>Id de la oferta creada o modificada.</returns>
        [HttpPost("AddLink")]
        [Produces("application/json")]
        public IActionResult AddLink([FromForm] string resourceRO, [FromForm] string pIdROId, [FromForm] Guid pIdGnossUser)
        {
            try
            {
                AccionesRosVinculados accionesRosVinculados = new AccionesRosVinculados();
                return Ok(accionesRosVinculados.AddLink(resourceRO, pIdROId, pIdGnossUser));
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}