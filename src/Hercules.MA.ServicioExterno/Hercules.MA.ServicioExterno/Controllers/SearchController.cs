using Hercules.MA.ServicioExterno.Controllers.Acciones;
using Hercules.MA.ServicioExterno.Models;
using Hercules.MA.ServicioExterno.Models.Graficas.DataGraficaAreasTags;
using Hercules.MA.ServicioExterno.Models.Graficas.DataGraficaProyectos;
using Hercules.MA.ServicioExterno.Models.Graficas.DataGraficaPublicaciones;
using Hercules.MA.ServicioExterno.Models.Graficas.DataItemRelacion;
using Hercules.MA.ServicioExterno.Models.RedesUsuario;
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
    public class SearchController : ControllerBase
    {        

        /// <summary>
        /// Inicia una búsqueda en los elementos seleccionados
        /// </summary>
        /// <param name="stringSearch">String de bíusqueda.</param>
        /// <param name="lang">Idioma</param>
        /// <returns>JSON con los datos necesarios para el JS.</returns>
        [HttpGet("DoMetaSearch")]
        public IActionResult DoMetaSearch(string stringSearch,string lang)
        {
            Dictionary<string, List<ObjectSearch>> resultBusqueda = null;

            try
            {
                AccionesMetaBusqueda accionBusqueda = new AccionesMetaBusqueda();
                resultBusqueda = accionBusqueda.Busqueda(stringSearch,lang);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(resultBusqueda);
        }
    }
}