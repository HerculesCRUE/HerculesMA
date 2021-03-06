using Hercules.MA.GraphicEngine.Config;
using Hercules.MA.GraphicEngine.Models;
using Hercules.MA.GraphicEngine.Models.Graficas;
using Hercules.MA.GraphicEngine.Models.Paginas;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Hercules.MA.GraphicEngine.Controllers
{
    [ApiController]
    [Route("[action]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class GraphicControllers : ControllerBase
    {
        readonly ConfigService _Configuracion;

        public GraphicControllers(ConfigService pConfig)
        {
            _Configuracion = pConfig;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public GraficaBase GetGrafica(string pIdPagina, string pIdGrafica, string pFiltroFacetas, string pLang)
        {
            return Models.GraphicEngine.GetGrafica(pIdPagina, pIdGrafica, pFiltroFacetas, pLang);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public List<DataGraphicUser> GetGraficasUser(string pPageId)
        {
            return Models.GraphicEngine.GetGraficasUserByPageId(pPageId);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void CrearPagina(string pUserId, string pTitulo)
        {
            Models.GraphicEngine.CrearPaginaUsuario(pUserId, pTitulo);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetCSVGrafica(string pIdPagina, string pIdGrafica, string pFiltroFacetas, string pLang, string pTitulo = "Gráfica")
        {
            return File(Models.GraphicEngine.GetGrafica(pIdPagina, pIdGrafica, pFiltroFacetas, pLang).GenerateCSV(), "application/CSV", pTitulo + ".csv");
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Faceta GetFaceta(string pIdPagina, string pIdFaceta, string pFiltroFacetas, string pLang,bool pGetAll=false)
        {
            return Models.GraphicEngine.GetFaceta(pIdPagina, pIdFaceta, pFiltroFacetas, pLang, pGetAll);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Pagina GetPaginaGrafica(string pIdPagina, string pLang, string userId = "")
        {
            return Models.GraphicEngine.GetPage(pIdPagina, pLang, userId);
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public bool IsAdmin(string pLang, string pUserId = "")
        {
            return Models.GraphicEngine.IsAdmin(pLang, pUserId);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public List<string> ObtenerConfigs(string pLang, string pUserId = "")
        {
            return Models.GraphicEngine.ObtenerConfigs(pLang, pUserId);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DescargarConfig(string pLang, string pConfig, string pUserId = "")
        {
            return File(Models.GraphicEngine.DescargarConfig(pLang, pConfig, pUserId), "application/json", pConfig);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public bool SubirConfig([FromForm]string pLang, [FromForm] string pConfigName, IFormFile pConfigFile, [FromForm] string pUserId = "")
        {
            return Models.GraphicEngine.SubirConfig(pLang, pConfigName, pConfigFile, pUserId);
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public bool EditarConfig(string pLang, string pGraphicId, string pPageId, string pUserId = "", string pGraphicName = "", int pGraphicOrder = 0, int pGraphicWidth = 0, string pBlockId = "")
        {
            return Models.GraphicEngine.EditarConfig(pLang, pUserId, pGraphicId, pPageId, pGraphicName, pGraphicOrder, pGraphicWidth, pBlockId);
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Grafica ObtenerGraficaConfig(string pLang, string pPageId, string pGraphicId, string pUserId = "")
        {
            return Models.GraphicEngine.ObtenerGraficaConfig(pLang, pUserId, pPageId, pGraphicId);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public List<Pagina> GetPaginasGraficas(string pLang, string userId = "")
        {
            return Models.GraphicEngine.GetPages(pLang, userId);
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public List<DataPageUser> GetPaginasUsuario(string pUserId)
        {
            return Models.GraphicEngine.GetPagesUser(pUserId);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public bool GuardarGrafica(string pTitulo, string pAnchura, string pIdPaginaGrafica, string pIdGrafica, string pFiltros, string pUserId, string pIdRecursoPagina = null, string pTituloPagina = null, string pEscalas = null)
        {
            return Models.GraphicEngine.GuardarGrafica(pTitulo, pAnchura, pIdPaginaGrafica, pIdGrafica, pFiltros, pUserId, pIdRecursoPagina, pTituloPagina, pEscalas);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void BorrarGrafica(string pUserId, string pPageID, string pGraphicID)
        {
            Models.GraphicEngine.BorrarGrafica(pUserId, pPageID, pGraphicID);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void BorrarPagina(string pUserId, string pPageID)
        {
            Models.GraphicEngine.BorrarPagina(pUserId, pPageID);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void EditarNombrePagina(string pUserId, string pPageID, string pNewTitle, string pOldTitle)
        {
            Models.GraphicEngine.EditarNombrePagina(pUserId, pPageID, pNewTitle, pOldTitle);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void EditarOrdenPagina(string pUserId, string pPageID, int pNewOrder, int pOldOrder)
        {
            Models.GraphicEngine.EditarOrdenPagina(pUserId, pPageID, pNewOrder, pOldOrder);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void EditarNombreGrafica(string pUserId, string pPageID, string pGraphicID, string pNewTitle, string pOldTitle)
        {
            Models.GraphicEngine.EditarNombreGrafica(pUserId, pPageID, pGraphicID, pNewTitle, pOldTitle);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void EditarOrdenGrafica(string pUserId, string pPageID, string pGraphicID, int pNewOrder, int pOldOrder)
        {
            Models.GraphicEngine.EditarOrdenGrafica(pUserId, pPageID, pGraphicID, pNewOrder, pOldOrder);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void EditarAnchuraGrafica(string pUserId, string pPageID, string pGraphicID, int pNewWidth, int pOldWidth)
        {
            Models.GraphicEngine.EditarAnchuraGrafica(pUserId, pPageID, pGraphicID, pNewWidth, pOldWidth);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void EditarEscalasGrafica(string pUserId, string pPageID, string pGraphicID, string pNewScales, string pOldScales)
        {
            Models.GraphicEngine.EditarEscalasGrafica(pUserId, pPageID, pGraphicID, pNewScales, pOldScales);
        }        
    }
}
