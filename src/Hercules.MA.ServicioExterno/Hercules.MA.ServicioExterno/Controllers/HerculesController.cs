using Hercules.MA.ServicioExterno.Controllers.Acciones;
using Hercules.MA.ServicioExterno.Models;
using Hercules.MA.ServicioExterno.Models.DataFechas;
using Hercules.MA.ServicioExterno.Models.DataGraficaColaboradores;
using Hercules.MA.ServicioExterno.Models.DataGraficaProyectosGroupBars;
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


        #region --- Grupo
        /// <summary>
        /// Controlador para obtener los datos del grupo en la cabecera de la ficha.
        /// </summary>
        /// <param name="pIdGrupo">ID del grupo en cuestión.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosFichaGrupo")]
        public IActionResult DatosFichaGrupo(string pIdGrupo)
        {
            Dictionary<string, int> datosGrupo = null;

            try
            {
                AccionesGroup accionGrupo = new AccionesGroup();
                datosGrupo = accionGrupo.GetDatosCabeceraGrupo(pIdGrupo);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosGrupo);
        }

        /// <summary>
        /// Controlador para obtener los datos del grupo en la cabecera de la ficha.
        /// </summary>
        /// <param name="pIdGrupo">ID del grupo en cuestión.</param>
        /// <param name="pParametros">Filtros de las facetas.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosGraficaPublicacionesGrupo")]
        public IActionResult DatosGraficaPublicacionesGrupo(string pIdGrupo, string pParametros)
        {
            DataGraficaPublicaciones datosGrupo = null;

            try
            {
                AccionesGroup accionGrupo = new AccionesGroup();
                datosGrupo = accionGrupo.GetDatosGraficaPublicaciones(pIdGrupo, pParametros);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosGrupo);
        }

        /// <summary>
        /// Controlador para obtener los datos del grupo en la cabecera de la ficha.
        /// </summary>
        /// <param name="pIdGrupo">ID del grupo en cuestión.</param>
        /// <param name="pParametros">Filtros de las facetas.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosGraficaColaboradoresMainResearcherGrupo")]
        public IActionResult DatosGraficaColaboradoresMainResearcherGrupo(string pIdGrupo, string pParametros)
        {
            List<DataGraficaColaboradores> datosGrupo = null;

            try
            {
                AccionesGroup accionGrupo = new AccionesGroup();
                datosGrupo = accionGrupo.GetDatosGraficaRedColaboradoresMainResearcher(pIdGrupo, pParametros);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosGrupo);
        }


        /// <summary>
        /// Controlador para obtener los datos del grupo en la cabecera de la ficha.
        /// </summary>
        /// <param name="pIdGrupo">ID del grupo en cuestión.</param>
        /// <param name="pParametros">Filtros de las facetas.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosGraficaColaboradoresMainResearcherFueraGrupo")]
        public IActionResult DatosGraficaColaboradoresMainResearcherFueraGrupo(string pIdGrupo, string pParametros)
        {
            List<DataGraficaColaboradores> datosGrupo = null;

            try
            {
                AccionesGroup accionGrupo = new AccionesGroup();
                datosGrupo = accionGrupo.GetDatosGraficaRedColaboradoresMainResearcherFuera(pIdGrupo, pParametros);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosGrupo);
        }



        /// <summary>
        /// Controlador para obtener los datos del grupo en la cabecera de la ficha.
        /// </summary>
        /// <param name="pIdGrupo">ID del grupo en cuestión.</param>
        /// <param name="pParametros">Filtros de las facetas.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosGraficaProyectosGrupo")]
        public IActionResult DatosGraficaProyectosGrupo(string pIdGrupo, string pParametros)
        {
            DataGraficaPublicaciones datos = null;

            try
            {
                AccionesGroup accionGrupo = new AccionesGroup();
                datos = accionGrupo.GetDatosGraficaProyectos(pIdGrupo, pParametros);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datos);
        }


        /// <summary>
        /// Controlador para obtener los datos del grupo en la cabecera de la ficha.
        /// </summary>
        /// <param name="pIdGrupo">ID del grupo en cuestión.</param>
        /// <param name="pParametros">Filtros de las facetas.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosGraficaTopicsGrupo")]
        public IActionResult DatosGraficaTopicsGrupo(string pIdGrupo, string pParametros)
        {
            DataGraficaPublicacionesHorizontal datos = null;

            try
            {
                AccionesGroup accionGrupo = new AccionesGroup();
                datos = accionGrupo.GetDatosGraficaTopicsHorizontal(pIdGrupo, pParametros);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datos);
        }



        /// <summary>
        /// Controlador para obtener los datos del grupo en la cabecera de la ficha.
        /// </summary>
        /// <param name="pIdGrupo">ID del grupo en cuestión.</param>
        /// <returns>JSON con los datos necesarios para pintar los topics.</returns>
        [HttpGet("DatosCategoriasGrupo")]
        public IActionResult DatosCategoriasGrupo(string pIdGrupo)
        {
            List<string> datos = null;

            try
            {
                AccionesGroup accionGrupo = new AccionesGroup();
                datos = accionGrupo.GetTopicsGrupo(pIdGrupo);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datos);
        }
        #endregion

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
        public IActionResult DatosGraficaColaboradoresPersonas(string pIdPersona, string pParametros, string pNombrePersona)
        {
            List<DataGraficaColaboradores> datosPublicacionesPersona = null;

            try
            {
                AccionesPersona accionPersona = new AccionesPersona();
                datosPublicacionesPersona = accionPersona.GetDatosGraficaRedColaboradoresPersonas(pIdPersona, pParametros, pNombrePersona);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosPublicacionesPersona);
        }

        /// <summary>
        /// Controlador para obtener los datos de los proyectos por año.
        /// </summary>
        /// <param name="pIdPersona">ID de la persona en cuestión.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosGraficaProyectosPersona")]
        public IActionResult DatosGraficaProyectosPersona(string pIdPersona, string pParametros)
        {
            ObjGrafica datosPublicacionesPersona = null;

            try
            {
                AccionesPersona accionPersona = new AccionesPersona();
                datosPublicacionesPersona = accionPersona.GetDatosGraficaProyectos(pIdPersona, pParametros);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosPublicacionesPersona);
        }
    }
}