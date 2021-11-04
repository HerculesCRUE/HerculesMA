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
        [HttpGet("DatosFichaProyecto")]
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
        [HttpGet("DatosGraficaRedColaboradores")]
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
        [HttpGet("DatosGraficaPublicaciones")]
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
        [HttpGet("DatosGraficaPublicacionesHorizontal")]
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

        /// <summary>
        /// Controlador para obtener los datos de la persona en la cabecera de la ficha.
        /// </summary>
        /// <param name="pIdPersona">ID de la persona en cuestión.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosFichaPersona")]
        public IActionResult DatosFichaPersona(string pIdPersona)
        {
            Dictionary<string, int> datosCabeceraFichas = null;

            try
            {
                AccionesPersona accionPersona = new AccionesPersona();
                datosCabeceraFichas = accionPersona.GetDatosCabeceraPersona(pIdPersona);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosCabeceraFichas);
        }

        /// <summary>
        /// Controlador para obtener los datos de la gráfica de publicaciones en persona.
        /// </summary>
        /// <param name="pIdPersona">ID de la persona en cuestión.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosGraficaPublicacionesPersona")]
        public IActionResult DatosGraficaPublicacionesPersona(string pIdPersona, string pParametros)
        {
            DataGraficaPublicaciones datosPublicacionesPersona = null;

            try
            {
                AccionesPersona accionPersona = new AccionesPersona();
                datosPublicacionesPersona = accionPersona.GetDatosGraficaPublicaciones(pIdPersona, pParametros);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosPublicacionesPersona);
        }

        /// <summary>
        /// Controlador para obtener los datos de los grupos.
        /// </summary>
        /// <param name="pIdPersona">ID de la persona en cuestión.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosGruposPersona")]
        public IActionResult DatosGruposPersona(string pIdPersona)
        {
            List<string> datosPublicacionesPersona = null;

            try
            {
                AccionesPersona accionPersona = new AccionesPersona();
                datosPublicacionesPersona = accionPersona.GetGrupoInvestigacion(pIdPersona);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosPublicacionesPersona);
        }

        /// <summary>
        /// Controlador para obtener los datos de las categorias.
        /// </summary>
        /// <param name="pIdPersona">ID de la persona en cuestión.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosCategoriasPersona")]
        public IActionResult DatosCategoriasPersona(string pIdPersona)
        {
            List<string> datosPublicacionesPersona = null;

            try
            {
                AccionesPersona accionPersona = new AccionesPersona();
                datosPublicacionesPersona = accionPersona.GetTopicsPersona(pIdPersona);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosPublicacionesPersona);
        }

        /// <summary>
        /// Controlador para obtener los datos de la gráfica en horizontal de personas.
        /// </summary>
        /// <param name="pIdPersona">ID de la persona en cuestión.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosGraficaHorizontalPersonas")]
        public IActionResult DatosGraficaHorizontalPersonas(string pIdPersona, string pParametros)
        {
            DataGraficaPublicacionesHorizontal datosPublicacionesPersona = null;

            try
            {
                AccionesPersona accionPersona = new AccionesPersona();
                datosPublicacionesPersona = accionPersona.GetDatosGraficaProyectosPersonaHorizontal(pIdPersona, pParametros);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosPublicacionesPersona);
        }

        /// <summary>
        /// Controlador para obtener los datos de la gráfica de nodos de personas.
        /// </summary>
        /// <param name="pIdPersona">ID de la persona en cuestión.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosGraficaColaboradoresPersonas")]
        public IActionResult DatosGraficaColaboradoresPersonas(string pIdPersona, string pParametros)
        {
            List<DataGraficaColaboradores> datosPublicacionesPersona = null;

            try
            {
                AccionesPersona accionPersona = new AccionesPersona();
                datosPublicacionesPersona = accionPersona.GetDatosGraficaRedColaboradoresPersonas(pIdPersona, pParametros);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosPublicacionesPersona);
        }
    }
}