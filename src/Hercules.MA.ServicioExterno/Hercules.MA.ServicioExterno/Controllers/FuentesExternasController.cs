using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Hercules.MA.ServicioExterno.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class FuentesExternasController : ControllerBase
    {
        readonly ConfigService _Configuracion;

        public FuentesExternasController(ConfigService pConfig)
        {
            _Configuracion = pConfig;
        }

        [HttpGet("InsertToQueue")]
        public IActionResult InsertToQueueFuentesExternas(string pUserId)
        {
            try
            {
                ReadRabbitService rabbitMQService = new ReadRabbitService(_Configuracion);
                string orcid = Acciones.AccionesFuentesExternas.GetORCID(pUserId);

                if (!string.IsNullOrEmpty(orcid))
                {
                    string ultimaFechaMod = Acciones.AccionesFuentesExternas.GetLastUpdatedDate(pUserId);
                    Dictionary<string, string> dicIDs = Acciones.AccionesFuentesExternas.GetUsersIDs(pUserId);

                    // Publicaciones.
                    List<string> listaDatos = new List<string>() { "investigador", orcid, ultimaFechaMod };
                    rabbitMQService.PublishMessage(listaDatos, _Configuracion.GetQueueRabbit());

                    // Zenodo
                    List<string> listaDatosZenodo = new List<string>() { "zenodo", orcid };
                    rabbitMQService.PublishMessage(listaDatosZenodo, _Configuracion.GetQueueRabbit());

                    // FigShare
                    if (dicIDs.ContainsKey("usuarioFigshare") && dicIDs.ContainsKey("tokenFigshare"))
                    {
                        List<string> listaDatosFigShare = new List<string>() { "figshare", dicIDs["tokenFigshare"] };
                        rabbitMQService.PublishMessage(listaDatosFigShare, _Configuracion.GetQueueRabbit());
                    }

                    // GitHub
                    if (dicIDs.ContainsKey("usuarioGitHub") && dicIDs.ContainsKey("tokenGitHub"))
                    {
                        List<string> listaDatosGitHub = new List<string>() { "github", dicIDs["usuarioGitHub"], dicIDs["tokenGitHub"] };
                        rabbitMQService.PublishMessage(listaDatosGitHub, _Configuracion.GetQueueRabbit());
                    }
                }                
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();
        }
    }
}
