using Hercules.MA.ServicioExterno.Controllers.Acciones;
using Hercules.MA.ServicioExterno.Models;
using Hercules.MA.ServicioExterno.Models.DataGraficaColaboradores;
using Hercules.MA.ServicioExterno.Models.DataGraficaPublicaciones;
using Hercules.MA.ServicioExterno.Models.DataGraficaPublicacionesHorizontal;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.MA.ServicioExterno.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class HerculesController : ControllerBase
    {
        /// <summary>
        /// Controlador para obtener los datos del proyecto en la cabecera de la ficha.
        /// </summary>
        /// <param name="pIdProyecto">ID del proyecto en cuestión.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpPost("DatosFichaProyecto")]
        public IActionResult DatosFichaProyecto(string pIdProyecto)
        {
            Dictionary<string, int> datosCabeceraFichas = null;

            try
            {
                AccionesProyecto accionProyecto = new AccionesProyecto();
                datosCabeceraFichas = accionProyecto.GetDatosCabeceraProyecto(pIdProyecto);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosCabeceraFichas);
        }

        /// <summary>
        /// Controlador para obtener los datos de la gráfica de red de colaboradores.
        /// </summary>
        /// <param name="pIdProyecto">ID del proyecto en cuestión.</param>
        /// <param name="pParametros">Filtros de las facetas.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpPost("DatosGraficaRedColaboradores")]
        public IActionResult DatosGraficaRedColaboradores(string pIdProyecto, string pParametros)
        {
            List<DataGraficaColaboradores> datosRedColaboradores = null;

            try
            {
                AccionesProyecto accionProyecto = new AccionesProyecto();
                datosRedColaboradores = accionProyecto.GetDatosGraficaRedColaboradores(pIdProyecto, pParametros);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosRedColaboradores);
        }

        /// <summary>
        /// Controlador para obtener los datos de la gráfica de publicaciones.
        /// </summary>
        /// <param name="pIdProyecto">ID del proyecto en cuestión.</param>
        /// <param name="pParametros">Filtros de las facetas.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpPost("DatosGraficaPublicaciones")]
        public IActionResult DatosGraficaPublicaciones(string pIdProyecto, string pParametros)
        {
            DataGraficaPublicaciones datosPublicaciones = null;

            try
            {
                AccionesProyecto accionProyecto = new AccionesProyecto();
                datosPublicaciones = accionProyecto.GetDatosGraficaPublicaciones(pIdProyecto, pParametros);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosPublicaciones);
        }

        /// <summary>
        /// Controlador para obtener los datos de la gráfica de publicaciones (horizontal).
        /// </summary>
        /// <param name="pIdProyecto">ID del proyecto en cuestión.</param>
        /// <param name="pParametros">Filtros de las facetas.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpPost("DatosGraficaPublicacionesHorizontal")]
        public IActionResult DatosGraficaPublicacionesHorizontal(string pIdProyecto, string pParametros)
        {
            DataGraficaPublicacionesHorizontal datosPublicaciones = null;

            try
            {
                AccionesProyecto accionProyecto = new AccionesProyecto();
                datosPublicaciones = accionProyecto.GetDatosGraficaPublicacionesHorizontal(pIdProyecto, pParametros);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosPublicaciones);
        }
    }
}