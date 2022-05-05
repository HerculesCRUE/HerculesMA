﻿using Hercules.MA.ServicioExterno.Controllers.Acciones;
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
        [Produces("application/json")]
        public IActionResult SaveCluster([FromForm] string pIdGnossUser, [FromForm] Cluster pDataCluster)
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

        [HttpPost("DatosGraficaColaboradoresCluster")]
        public IActionResult DatosGraficaColaboradoresCluster ([FromForm] Cluster pCluster, [FromForm] List<string> pPersons, [FromForm] bool seleccionados)
        {
            try
            {
                AccionesCluster accionCluster = new AccionesCluster();
                return Ok(accionCluster.DatosGraficaColaboradoresCluster(pCluster, pPersons, seleccionados));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene los datos para crear la gráfica de áreas temáticas
        /// </summary>
        /// <param name="pPersons">Listado de personas.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpPost("DatosGraficaAreasTematicasCluster")]
        public IActionResult DatosGraficaAreasTematicasCluster([FromForm] List<string> pPersons)
        {
            DataGraficaAreasTags datos = null;

            try
            {
                AccionesCluster accionCluster = new AccionesCluster();
                datos = accionCluster.DatosGraficaAreasTematicas(pPersons);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datos);
        }


        /// <summary>
        /// Borra un cluster
        /// </summary>
        /// <param name="pIdClusterId">Id del Cluster a borrar.</param>
        /// <returns>Un booleano si ha sido borrado.</returns>
        [HttpPost("borrarCluster")]
        public IActionResult BorrarCluster([Required] string pIdClusterId)
        {

            bool borrado = false;

            try
            {
                AccionesCluster accionCluster = new AccionesCluster();
                borrado = accionCluster.BorrarCluster(pIdClusterId);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(borrado);
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
