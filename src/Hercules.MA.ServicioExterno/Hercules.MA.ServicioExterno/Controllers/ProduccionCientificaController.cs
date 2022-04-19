using Hercules.MA.ServicioExterno.Controllers.Acciones;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Hercules.MA.ServicioExterno.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class ProduccionCientificaController : ControllerBase
    {
        [HttpGet("EnvioPRC")]
        public IActionResult EnvioPRC(string pIdRecurso)
        {
            try
            {
                AccionesProduccionCientifica acionesPRC = new AccionesProduccionCientifica();
                acionesPRC.EnvioPRC(pIdRecurso);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();
        }
    }
}
