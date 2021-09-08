using Hercules.MA.ServicioExterno.Controllers.Acciones;
using Hercules.MA.ServicioExterno.ModelsDataGraficaColaboradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hercules.MA.ServicioExterno.Controllers
{
    public class HerculesController : Controller
    {
        /// <summary>
        /// Controlador para obtener los datos de la gráfica de red de colaboradores.
        /// </summary>
        /// <param name="pIdProyecto">ID del proyecto en cuestión.</param>
        /// <param name="pParametros">Filtros de las facetas.</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet]
        public ActionResult DatosGraficaRedColaboradores(string pIdProyecto, string pParametros)
        {
            List<DataGraficaColaboradores> datosRedColaboradores = null;

            try
            {
                AccionesHerculesProyecto accionProyecto = new AccionesHerculesProyecto();
                datosRedColaboradores = accionProyecto.GetDatosGraficaRedColaboradores(pIdProyecto, pParametros);
            }
            catch (Exception)
            {
                throw;
            }

            return Json(datosRedColaboradores, JsonRequestBehavior.AllowGet);
        }
    }
}
