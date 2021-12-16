using Hercules.MA.ServicioExterno.Controllers.Acciones;
using Hercules.MA.ServicioExterno.Models;
using Hercules.MA.ServicioExterno.Models.Graficas.DataGraficaAreasTags;
using Hercules.MA.ServicioExterno.Models.Graficas.DataGraficaProyectos;
using Hercules.MA.ServicioExterno.Models.Graficas.DataGraficaPublicaciones;
using Hercules.MA.ServicioExterno.Models.Graficas.DataItemRelacion;
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
        #region Comunes
        /// <summary>
        /// Obtiene los datos para crear la gráfica de las publicaciones.
        /// </summary>
        /// <param name="pParametros">Filtros aplicados</param>
        /// <returns>Objeto con todos los datos necesarios para crear la gráfica en el JS.</returns>
        [HttpGet("DatosGraficaPublicaciones")]
        public IActionResult DatosGraficaPublicaciones(string pParametros)
        {
            DataGraficaPublicaciones datosPublicaciones = null;

            try
            {
                AccionesPublicaciones accionesPublicaciones = new AccionesPublicaciones();
                datosPublicaciones = accionesPublicaciones.GetDatosGraficaPublicaciones(pParametros);
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(datosPublicaciones);
        }

        /// <summary>
        /// Obtiene los datos para crear la gráfica de los proyectos.
        /// </summary>
        /// <param name="pParametros">Filtros aplicados</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosGraficaProyectos")]
        public IActionResult DatosGraficaProyectos(string pParametros)
        {
            ObjGrafica datosProyectos = null;

            try
            {
                AccionesProyecto accionesProyecto = new AccionesProyecto();
                datosProyectos = accionesProyecto.GetDatosGraficaProyectos(pParametros);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosProyectos);
        }
        #endregion

        #region Grupo
        /// <summary>
        /// Obtiene los datos para crear la gráfica de miembros de un grupo
        /// </summary>
        /// <param name="pIdGrupo">ID del grupo en cuestión.</param>
        /// <param name="pParametros">Filtros de las facetas.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosGraficaMiembrosGrupo")]
        public IActionResult DatosGraficaMiembrosGrupo(string pIdGrupo, string pParametros)
        {
            List<DataItemRelacion> datosGrupo = null;

            try
            {
                AccionesGroup accionGrupo = new AccionesGroup();
                datosGrupo = accionGrupo.DatosGraficaMiembrosGrupo(pIdGrupo, pParametros);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosGrupo);
        }


        /// <summary>
        /// Controlador para obtener las áreas temáticas de las publicaciones de un grupo.
        /// </summary>
        /// <param name="pIdGrupo">ID del grupo en cuestión.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosGraficaAreasTematicasGrupo")]
        public IActionResult DatosGraficaAreasTematicasGrupo(string pIdGrupo)
        {
            DataGraficaAreasTags datos = null;

            try
            {
                AccionesGroup accionGrupo = new AccionesGroup();
                datos = accionGrupo.DatosGraficaAreasTematicasGrupo(pIdGrupo);
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
        /// <param name="pParametros">Filtros de la búsqueda.</param>
        /// <param name="pMax">Nº máximo para pintar</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosGraficaColaboradoresGrupo")]
        public IActionResult DatosGraficaColaboradoresGrupo(string pIdGrupo, string pParametros, int pMax)
        {
            List<DataItemRelacion> datosGrupo = null;

            try
            {
                AccionesGroup accionGrupo = new AccionesGroup();
                datosGrupo = accionGrupo.DatosGraficaColaboradoresGrupo(pIdGrupo, pParametros, pMax);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosGrupo);
        }

        #endregion












        #region --- Grupo        






        ///// <summary>
        ///// Controlador para obtener los datos del grupo en la cabecera de la ficha.
        ///// </summary>
        ///// <param name="pIdGrupo">ID del grupo en cuestión.</param>
        ///// <param name="pParametros">Filtros de las facetas.</param>
        ///// <returns>JSON con los datos necesarios para el JS.</returns>
        //[HttpGet("DatosGraficaProyectosGrupo")]
        //public IActionResult DatosGraficaProyectosGrupo(string pIdGrupo, string pParametros)
        //{
        //    ObjGrafica datos = null;

        //    try
        //    {
        //        AccionesGroup accionGrupo = new AccionesGroup();
        //        datos = accionGrupo.GetDatosGraficaProyectos(pIdGrupo, pParametros);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //    return Ok(datos);
        //}





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
            List<DataItemRelacion> datosRedColaboradores = null;

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

        ///// <summary>
        ///// Controlador para obtener los datos de la gráfica de publicaciones.
        ///// </summary>
        ///// <param name="pIdProyecto">ID del proyecto en cuestión.</param>
        ///// <param name="pParametros">Filtros de las facetas.</param>
        ///// <returns>JSON con los datos necesarios para el JS.</returns>
        //[HttpGet("DatosGraficaPublicaciones")]
        //public IActionResult DatosGraficaPublicaciones(string pIdProyecto, string pParametros)
        //{
        //    DataGraficaPublicaciones datosPublicaciones = null;

        //    try
        //    {
        //        AccionesProyecto accionProyecto = new AccionesProyecto();
        //        datosPublicaciones = accionProyecto.GetDatosGraficaPublicaciones(pIdProyecto, pParametros);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //    return Ok(datosPublicaciones);
        //}

        /// <summary>
        /// Controlador para obtener los datos de la gráfica de publicaciones (horizontal).
        /// </summary>
        /// <param name="pIdProyecto">ID del proyecto en cuestión.</param>
        /// <param name="pParametros">Filtros de las facetas.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosGraficaPublicacionesHorizontal")]
        public IActionResult DatosGraficaPublicacionesHorizontal(string pIdProyecto, string pParametros)
        {
            DataGraficaAreasTags datosPublicaciones = null;

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

        ///// <summary>
        ///// Controlador para obtener los datos de la gráfica de publicaciones en persona.
        ///// </summary>
        ///// <param name="pIdPersona">ID de la persona en cuestión.</param>
        ///// <returns>JSON con los datos necesarios para el JS.</returns>
        //[HttpGet("DatosGraficaPublicacionesPersona")]
        //public IActionResult DatosGraficaPublicacionesPersona(string pIdPersona, string pParametros)
        //{
        //    DataGraficaPublicaciones datosPublicacionesPersona = null;

        //    try
        //    {
        //        AccionesPersona accionPersona = new AccionesPersona();
        //        datosPublicacionesPersona = accionPersona.GetDatosGraficaPublicaciones(pIdPersona, pParametros);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //    return Ok(datosPublicacionesPersona);
        //}

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
            List<Dictionary<string, string>> datosPublicacionesPersona = null;

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
            DataGraficaAreasTags datosPublicacionesPersona = null;

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
            List<DataItemRelacion> datosPublicacionesPersona = null;

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

        ///// <summary>
        ///// Controlador para obtener los datos de los proyectos por año.
        ///// </summary>
        ///// <param name="pIdPersona">ID de la persona en cuestión.</param>
        ///// <returns>JSON con los datos necesarios para el JS.</returns>
        //[HttpGet("DatosGraficaProyectosPersona")]
        //public IActionResult DatosGraficaProyectosPersona(string pIdPersona, string pParametros)
        //{
        //    ObjGrafica datosPublicacionesPersona = null;

        //    try
        //    {
        //        AccionesPersona accionPersona = new AccionesPersona();
        //        datosPublicacionesPersona = accionPersona.GetDatosGraficaProyectos(pIdPersona, pParametros);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //    return Ok(datosPublicacionesPersona);
        //}


        /// <summary>
        /// Controlador para obtener las citas a un documento.
        /// </summary>
        /// <param name="pIdDocumento">ID del documento en cuestión.</param>
        /// <param name="pParametros">Filtros de las facetas.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosGraficaCitas")]
        public IActionResult DatosGraficaCitas(string pIdDocumento, string pParametros)
        {
            List<DataItemRelacion> datosGrupo = null;

            try
            {
                AccionesPublicaciones accionGrupo = new AccionesPublicaciones();
                datosGrupo = accionGrupo.GetDatosGraficaCitas(pIdDocumento, pParametros);
                // GetDatosGraficaReferencias
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosGrupo);
        }


        /// <summary>
        /// Controlador para obtener las referencias de un documento.
        /// </summary>
        /// <param name="pIdDocumento">ID del documento en cuestión.</param>
        /// <param name="pParametros">Filtros de las facetas.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DatosGraficaReferencias")]
        public IActionResult DatosGraficaReferencias(string pIdDocumento, string pParametros)
        {
            List<DataItemRelacion> datosGrupo = null;

            try
            {
                AccionesPublicaciones accionGrupo = new AccionesPublicaciones();
                datosGrupo = accionGrupo.GetDatosGraficaReferencias(pIdDocumento, pParametros);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosGrupo);
        }



        /// <summary>
        /// Controlador para obtener los datos del documento en la cabecera de la ficha.
        /// </summary>
        /// <param name="pIdDocumento">ID del documento en cuestión.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("GetDatosCabeceraDocumento")]
        public IActionResult GetDatosCabeceraDocumento(string pIdDocumento)
        {
            Dictionary<string, int> datosCabeceraFichas = null;

            try
            {
                AccionesPublicaciones accionDocumento = new AccionesPublicaciones();
                datosCabeceraFichas = accionDocumento.GetDatosCabeceraDocumento(pIdDocumento);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(datosCabeceraFichas);
        }

    }
}