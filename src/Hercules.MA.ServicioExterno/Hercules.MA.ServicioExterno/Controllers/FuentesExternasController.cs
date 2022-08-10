using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

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

                    DateTime currentDate = DateTime.Now;
                    DateTime lastUpdate = new DateTime(Int32.Parse(ultimaFechaMod.Split('-')[0]), Int32.Parse(ultimaFechaMod.Split('-')[1]), Int32.Parse(ultimaFechaMod.Split('-')[2]));

                    if (lastUpdate.AddDays(1) > currentDate)
                    {
                        return BadRequest(ultimaFechaMod);
                    }

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

        [HttpGet("InsertDoiToQueue")]
        public bool InsertDoiToQueueFuentesExternas(string pDoi, string pNombreCompletoAutor, string pOrcid)
        {
            try
            {
                ReadRabbitService rabbitMQService = new ReadRabbitService(_Configuracion);

                // De momento, el DOI, ORCID y el nombre del autor son obligatorios. TODO: ¿ORCID en un futuro no va a ser obligatorio?
                if (!string.IsNullOrEmpty(pDoi) && !string.IsNullOrEmpty(pNombreCompletoAutor) && !string.IsNullOrEmpty(pOrcid))
                {
                    // Comprobación si el documento es válido o no. Para ello, hay que procesar dicho fichero para ver si corresponde algún autor al pasado por parámetro.
                    // Aunque se haga la petición, no tarda. TODO: ¿Posible manera de mejorar esto?
                    Uri url = new Uri(string.Format(_Configuracion.GetUrlPublicacion() + "GetRoPublication?pDoi={0}&pNombreCompletoAutor={1}", pDoi, pNombreCompletoAutor));
                    string info_publication = httpCall(url.ToString(), "GET", new Dictionary<string, string>()).Result;
                    if (string.IsNullOrEmpty(info_publication) || info_publication.Trim() == "[]")
                    {
                        return false;
                    }

                    // Inserción a la cola.
                    List<string> listaDatos = new List<string>() { pDoi, pNombreCompletoAutor, pOrcid };
                    rabbitMQService.PublishMessage(listaDatos, _Configuracion.GetDoiQueueRabbit());

                    return true;
                }
            }
            catch (Exception e)
            {
                string s = e.Message;
                throw;
            }

            return false;
        }

        /// <summary>
        /// A Http calls function.
        /// </summary>
        /// <param name="url">The http call URL.</param>
        /// <param name="method">Crud method for the call.</param>
        /// <returns></returns>
        protected async Task<string> httpCall(string url, string method = "GET", Dictionary<string, string> headers = null)
        {
            HttpResponseMessage response;
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromHours(24);
                using (var request = new HttpRequestMessage(new HttpMethod(method), url))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");

                    if (headers != null && headers.Count > 0)
                    {
                        foreach (var item in headers)
                        {
                            request.Headers.TryAddWithoutValidation(item.Key, item.Value);
                        }
                    }
                    response = await httpClient.SendAsync(request);
                }
            }

            if (response.Content != null)
            {
                return await response.Content.ReadAsStringAsync();
            }

            else
            {
                return string.Empty;
            }
        }
    }
}
