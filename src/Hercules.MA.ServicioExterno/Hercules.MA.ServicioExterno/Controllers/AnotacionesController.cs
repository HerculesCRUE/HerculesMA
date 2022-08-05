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
    public class AnotacionesController : ControllerBase
    {
        /// <summary>
        /// Controlador para obtener las anotaciones de un investigador en un RO concreto.
        /// Los valores posibles de la ontología serían actualmente:
        /// "http://purl.org/ontology/bibo/Document", "document"
        /// "http://w3id.org/roh/ResearchObject", "researchobject"
        /// </summary>
        /// <param name="idRO">Id del RO</param>
        /// <param name="idUser">Id del usuario </param>
        /// <param name="rdfType">rdfType de la ontología </param>
        /// <param name="ontology">nombre de la ontología </param>
        /// <returns>Diccionario con los datos.</returns>
        [HttpPost("GetOwnAnnotationsInRO")]
        public IActionResult GetOwnAnnotationsInRO([FromForm] string idRO, [FromForm] string idUser, [FromForm] string rdfType, [FromForm] string ontology)
        {
            Dictionary<string, string> anotaciones;

            try
            {
                AccionesAnotaciones annotations = new AccionesAnotaciones();
                anotaciones = annotations.GetOwnAnnotationsInRO(idRO, idUser, rdfType, ontology);
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(anotaciones);
        }



        /// <summary>
        /// Controlador para obtener las anotaciones de un investigador en un RO concreto.
        /// Los valores posibles de la ontología serían actualmente:
        /// "http://purl.org/ontology/bibo/Document", "document"
        /// "http://w3id.org/roh/ResearchObject", "researchobject"
        /// </summary>
        /// <param name="idRO">Id del RO</param>
        /// <param name="idUser">Id del usuario </param>
        /// <param name="rdfType">rdfType de la ontología </param>
        /// <param name="ontology">Nombre de la ontología </param>
        /// <param name="texto">Texto de la anotación</param>
        /// <param name="idAnnotation">Id de la anotación (si se guarda)</param>
        /// <returns>Diccionario con los datos.</returns>
        [HttpPost("CreateNewAnnotation")]
        public IActionResult CreateNewAnnotation([FromForm] string idRO, [FromForm] string idUser, [FromForm] string rdfType, [FromForm] string ontology, [FromForm] string texto, [FromForm] string idAnnotation = null)
        {
            string anotacionesId;

            try
            {
                AccionesAnotaciones annotations = new AccionesAnotaciones();
                anotacionesId = annotations.CreateNewAnnotation(idRO, idUser, rdfType, ontology, texto);
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(anotacionesId);
        }

    }
}
