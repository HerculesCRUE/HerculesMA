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

        #region Gráficas generales
        /// <summary>
        /// Obtiene la gráfica y sus datos
        /// </summary>
        /// <param name="pIdPagina"></param>
        /// <param name="pIdGrafica"></param>
        /// <param name="pFiltroFacetas"></param>
        /// <param name="pLang"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public GraficaBase GetGrafica(string pIdPagina, string pIdGrafica, string pFiltroFacetas, string pLang)
        {
            return Models.GraphicEngine.GetGrafica(pIdPagina, pIdGrafica, pFiltroFacetas, pLang);
        }

        /// <summary>
        /// Obtiene las páginas de indicadores generales
        /// </summary>
        /// <param name="pIdPagina"></param>
        /// <param name="pLang"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Pagina GetPaginaGrafica(string pIdPagina, string pLang, string userId = "")
        {
            return Models.GraphicEngine.GetPage(pIdPagina, pLang, userId);
        }

        /// <summary>
        /// Crea una nueva página de indicadores personales.
        /// </summary>
        /// <param name="pUserId"></param>
        /// <param name="pTitulo"></param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void CrearPagina(string pUserId, string pTitulo)
        {
            Models.GraphicEngine.CrearPaginaUsuario(pUserId, pTitulo);
        }

        /// <summary>
        /// Obtención de las facetas o filtros de la página
        /// </summary>
        /// <param name="pIdPagina"></param>
        /// <param name="pIdFaceta"></param>
        /// <param name="pFiltroFacetas"></param>
        /// <param name="pLang"></param>
        /// <param name="pGetAll"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Faceta GetFaceta(string pIdPagina, string pIdFaceta, string pFiltroFacetas, string pLang, bool pGetAll = false)
        {
            return Models.GraphicEngine.GetFaceta(pIdPagina, pIdFaceta, pFiltroFacetas, pLang, pGetAll);
        }

        /// <summary>
        /// Edita la configuración de una gráfica de indicadores generales
        /// </summary>
        /// <param name="pLang"></param>
        /// <param name="pGraphicId"></param>
        /// <param name="pPageId"></param>
        /// <param name="pUserId"></param>
        /// <param name="pGraphicName"></param>
        /// <param name="pGraphicOrder"></param>
        /// <param name="pGraphicWidth"></param>
        /// <param name="pBlockId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public bool EditarConfig(string pLang, string pGraphicId, string pPageId, string pUserId = "", string pGraphicName = "", int pGraphicOrder = 0, int pGraphicWidth = 0, string pBlockId = "")
        {
            return Models.GraphicEngine.EditarConfig(pLang, pUserId, pGraphicId, pPageId, pGraphicName, pGraphicOrder, pGraphicWidth, pBlockId);
        }

        /// <summary>
        /// Obtiene la configuración de una gráfica de indicadores generales
        /// </summary>
        /// <param name="pLang"></param>
        /// <param name="pPageId"></param>
        /// <param name="pGraphicId"></param>
        /// <param name="pUserId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Grafica ObtenerGraficaConfig(string pLang, string pPageId, string pGraphicId, string pUserId = "")
        {
            return Models.GraphicEngine.ObtenerGraficaConfig(pLang, pUserId, pPageId, pGraphicId);
        }
        /// <summary>
        /// Obtención de una lista de gráficas y sus datos específicos del usuario
        /// </summary>
        /// <param name="pLang"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public List<Pagina> GetPaginasGraficas(string pLang, string userId = "")
        {
            return Models.GraphicEngine.GetPages(pLang, userId);
        }
        /// <summary>
        /// Guarda una gráfica de indicadores generales en el panel de indicadores personales del usuario
        /// </summary>
        /// <param name="pTitulo"></param>
        /// <param name="pAnchura"></param>
        /// <param name="pIdPaginaGrafica"></param>
        /// <param name="pIdGrafica"></param>
        /// <param name="pFiltros"></param>
        /// <param name="pUserId"></param>
        /// <param name="pIdRecursoPagina"></param>
        /// <param name="pTituloPagina"></param>
        /// <param name="pEscalas"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public bool GuardarGrafica(string pTitulo, string pAnchura, string pIdPaginaGrafica, string pIdGrafica, string pFiltros, string pUserId, string pIdRecursoPagina = null, string pTituloPagina = null, string pEscalas = null)
        {
            return Models.GraphicEngine.GuardarGrafica(pTitulo, pAnchura, pIdPaginaGrafica, pIdGrafica, pFiltros, pUserId, pIdRecursoPagina, pTituloPagina, pEscalas);
        }

        #endregion
        #region Gráficas personales
        /// <summary>
        /// Obtención de una lista de gráficas y sus datos específicos del usuario.
        /// </summary>
        /// <param name="pPageId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public List<DataGraphicUser> GetGraficasUser(string pPageId)
        {
            return Models.GraphicEngine.GetGraficasUserByPageId(pPageId);
        }
        /// <summary>
        /// Obtiene las páginas de indicadores personales de un usuario específico
        /// </summary>
        /// <param name="pUserId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public List<DataPageUser> GetPaginasUsuario(string pUserId)
        {
            return Models.GraphicEngine.GetPagesUser(pUserId);
        }
        /// <summary>
        /// Borra la gráfica de indicadores personales del usuario
        /// </summary>
        /// <param name="pUserId"></param>
        /// <param name="pPageID"></param>
        /// <param name="pGraphicID"></param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void BorrarGrafica(string pUserId, string pPageID, string pGraphicID)
        {
            Models.GraphicEngine.BorrarGrafica(pUserId, pPageID, pGraphicID);
        }
        /// <summary>
        /// Borra la página de indicadores personales del usuario
        /// </summary>
        /// <param name="pUserId"></param>
        /// <param name="pPageID"></param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void BorrarPagina(string pUserId, string pPageID)
        {
            Models.GraphicEngine.BorrarPagina(pUserId, pPageID);
        }

        /// <summary>
        /// Edita el nombre de la página de indicadores personales
        /// </summary>
        /// <param name="pUserId"></param>
        /// <param name="pPageID"></param>
        /// <param name="pNewTitle"></param>
        /// <param name="pOldTitle"></param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void EditarNombrePagina(string pUserId, string pPageID, string pNewTitle, string pOldTitle)
        {
            Models.GraphicEngine.EditarNombrePagina(pUserId, pPageID, pNewTitle, pOldTitle);
        }
        /// <summary>
        /// Edita el orden de la página de indicadores personales
        /// </summary>
        /// <param name="pUserId"></param>
        /// <param name="pPageID"></param>
        /// <param name="pNewOrder"></param>
        /// <param name="pOldOrder"></param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void EditarOrdenPagina(string pUserId, string pPageID, int pNewOrder, int pOldOrder)
        {
            Models.GraphicEngine.EditarOrdenPagina(pUserId, pPageID, pNewOrder, pOldOrder);
        }

        /// <summary>
        /// Edita el nombre de la gráfica de indicadores personales
        /// </summary>
        /// <param name="pUserId"></param>
        /// <param name="pPageID"></param>
        /// <param name="pGraphicID"></param>
        /// <param name="pNewTitle"></param>
        /// <param name="pOldTitle"></param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void EditarNombreGrafica(string pUserId, string pPageID, string pGraphicID, string pNewTitle, string pOldTitle)
        {
            Models.GraphicEngine.EditarNombreGrafica(pUserId, pPageID, pGraphicID, pNewTitle, pOldTitle);
        }

        /// <summary>
        /// Edita el orden de la gráfica de indicadores personales
        /// </summary>
        /// <param name="pUserId"></param>
        /// <param name="pPageID"></param>
        /// <param name="pGraphicID"></param>
        /// <param name="pNewOrder"></param>
        /// <param name="pOldOrder"></param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void EditarOrdenGrafica(string pUserId, string pPageID, string pGraphicID, int pNewOrder, int pOldOrder)
        {
            Models.GraphicEngine.EditarOrdenGrafica(pUserId, pPageID, pGraphicID, pNewOrder, pOldOrder);
        }

        /// <summary>
        /// Edita la anchura de la gráfica de indicadores personales
        /// </summary>
        /// <param name="pUserId"></param>
        /// <param name="pPageID"></param>
        /// <param name="pGraphicID"></param>
        /// <param name="pNewWidth"></param>
        /// <param name="pOldWidth"></param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void EditarAnchuraGrafica(string pUserId, string pPageID, string pGraphicID, int pNewWidth, int pOldWidth)
        {
            Models.GraphicEngine.EditarAnchuraGrafica(pUserId, pPageID, pGraphicID, pNewWidth, pOldWidth);
        }

        /// <summary>
        /// Edita las escalas/ejes de la gráfica de indicadores personales en función de si tiene 1 o 2 escalas
        /// </summary>
        /// <param name="pUserId"></param>
        /// <param name="pPageID"></param>
        /// <param name="pGraphicID"></param>
        /// <param name="pNewScales"></param>
        /// <param name="pOldScales"></param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public void EditarEscalasGrafica(string pUserId, string pPageID, string pGraphicID, string pNewScales, string pOldScales)
        {
            Models.GraphicEngine.EditarEscalasGrafica(pUserId, pPageID, pGraphicID, pNewScales, pOldScales);
        }
        #endregion
        #region Utils
        /// <summary>
        /// Crea un documento CSV de una gráfica
        /// </summary>
        /// <param name="pIdPagina"></param>
        /// <param name="pIdGrafica"></param>
        /// <param name="pFiltroFacetas"></param>
        /// <param name="pLang"></param>
        /// <param name="pTitulo"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetCSVGrafica(string pIdPagina, string pIdGrafica, string pFiltroFacetas, string pLang, string pTitulo = "Gráfica")
        {
            return File(Models.GraphicEngine.GetGrafica(pIdPagina, pIdGrafica, pFiltroFacetas, pLang).GenerateCSV(), "application/CSV", pTitulo + ".csv");
        }
        /// <summary>
        /// Comprueba si el usuario es administrador de gráficas o no
        /// </summary>
        /// <param name="pLang"></param>
        /// <param name="pUserId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public bool IsAdmin(string pLang, string pUserId = "")
        {
            return Models.GraphicEngine.IsAdmin(pUserId);
        }
        /// <summary>
        /// Obtiene las configuraciones de las páginas
        /// </summary>
        /// <param name="pLang"></param>
        /// <param name="pUserId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public List<string> ObtenerConfigs(string pLang, string pUserId = "")
        {
            return Models.GraphicEngine.ObtenerConfigs(pLang, pUserId);
        }
        /// <summary>
        /// Devuelve un archivo de configuración concreto para su descarga
        /// </summary>
        /// <param name="pLang"></param>
        /// <param name="pConfig"></param>
        /// <param name="pUserId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DescargarConfig(string pLang, string pConfig, string pUserId = "")
        {
            return File(Models.GraphicEngine.DescargarConfig(pLang, pConfig, pUserId), "application/json", pConfig);
        }
        /// <summary>
        /// Actualiza un archivo de configuración concreto
        /// </summary>
        /// <param name="pLang"></param>
        /// <param name="pConfigName"></param>
        /// <param name="pConfigFile"></param>
        /// <param name="pUserId"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public bool SubirConfig([FromForm] string pLang, [FromForm] string pConfigName, IFormFile pConfigFile, [FromForm] string pUserId = "")
        {
            return Models.GraphicEngine.SubirConfig(pLang, pConfigName, pConfigFile, pUserId);
        }
        #endregion
    }
}
