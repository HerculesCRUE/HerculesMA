using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using Hercules.MA.GraphicEngine.Models.Facetas;
using Hercules.MA.GraphicEngine.Models.Graficas;
using Hercules.MA.GraphicEngine.Models.Paginas;
using Hercules.MA.GraphicEngine.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hercules.MA.GraphicEngine.Models
{
    public static class GraphicEngine
    {
        // Prefijos.
        private static string mPrefijos = string.Join(" ", JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Config", "configJson", "prefijos.json"))));
        private static ResourceApi mResourceApi = new ResourceApi(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Config", "ConfigOAuth", "OAuthV3.config"));
        private static CommunityApi mCommunityApi = new CommunityApi(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Config", "ConfigOAuth", "OAuthV3.config"));
        private static Guid mCommunityID = mCommunityApi.GetCommunityId();
        private static List<ConfigModel> mTabTemplates;
        private const int NUM_HILOS = 5;

        #region --- Páginas

        /// <summary>
        /// Obtiene los datos de la página.
        /// </summary>
        /// <param name="pIdPagina">Identificador de la página.</param>
        /// <param name="pLang">Idioma</param>
        /// <param name="userId">Identificador del usuario</param>
        /// <returns></returns>
        public static Pagina GetPage(string pIdPagina, string pLang, string userId = "")
        {
            // Lectura del JSON de configuración.
            ConfigModel configModel = TabTemplates.FirstOrDefault(x => x.Identificador == pIdPagina);
            return CrearPagina(configModel, pLang, userId);
        }

        /// <summary>
        /// Obtiene si el usuario es admin o no
        /// </summary>
        /// <param name="pUserId">Identificador del usuario</param>
        /// <returns></returns>
        public static bool IsAdmin(string pUserId = "")
        {
            bool isAdmin = false;
            if (pUserId == "")
            {
                return false;
            }

            // Consulta sparql.
            string selectIsAdmin = mPrefijos;
            selectIsAdmin += "SELECT ?permisos ";
            string whereIsAdmin = $@"WHERE {{
                                ?s roh:gnossUser <http://gnoss/{pUserId.ToUpper()}> . 
                                ?s roh:isOtriManager ?permisos . 
                            }}";


            SparqlObject resultadoQueryIsAdmin = mResourceApi.VirtuosoQuery(selectIsAdmin, whereIsAdmin, mCommunityID);
            if (resultadoQueryIsAdmin != null && resultadoQueryIsAdmin.results != null &&
                resultadoQueryIsAdmin.results.bindings != null && resultadoQueryIsAdmin.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQueryIsAdmin.results.bindings)
                {
                    isAdmin = bool.Parse(fila["permisos"].value);
                }
            }
            return isAdmin;
        }

        /// <summary>
        /// Obtiene los nombres de los json de configuración.
        /// </summary>
        /// <param name="pLang">Idioma.</param>
        /// <param name="pUserId">Identificador del usuario.</param>
        /// <returns></returns>
        public static List<string> ObtenerConfigs(string pLang, string pUserId = "")
        {
            // Compruebo si es administrador
            bool isAdmin = IsAdmin(pUserId);
            if (!isAdmin)
            {
                return null;
            }
            string path = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Config", "configGraficas");
            List<string> nombres = Directory.EnumerateFiles(path).Select(x => x.Split("configGraficas").LastOrDefault()).OrderBy(x => x).ToList();

            return nombres;
        }

        /// <summary>
        /// Sobreescribe la configuración de la página.
        /// </summary>
        /// <param name="pLang">Idioma.</param>
        /// <param name="pConfigFile">Fichero a subir.</param>
        /// <param name="pUserId">Identificador del usuario.</param>
        /// <returns></returns>
        public static bool SubirConfig(string pLang, string pConfigName, IFormFile pConfigFile, string pUserId = "")
        {
            // Compruebo si es administrador
            bool isAdmin = IsAdmin(pUserId);
            if (!isAdmin || pConfigFile == null)
            {
                return false;
            }
            try
            {
                // Compruebo si es un JSON.
                string json = new StreamReader(pConfigFile.OpenReadStream()).ReadToEnd();
                ConfigModel configModel = JsonConvert.DeserializeObject<ConfigModel>(json);
                if (configModel == null)
                {
                    return false;
                }
                string pathConfig = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Config", "configGraficas");
                string path = "";
                string fileName = pConfigName;
                foreach (string file in Directory.EnumerateFiles(pathConfig))
                {
                    if (Path.GetFileName(file).Contains(fileName))
                    {
                        path = file;
                        break;
                    }
                }
                if (path == "")
                {
                    return false;
                }
                using (Stream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    pConfigFile.CopyTo(fileStream);
                }

                mTabTemplates = new List<ConfigModel>();
                foreach (string file in Directory.EnumerateFiles(pathConfig))
                {
                    ConfigModel tab = JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText(file));
                    mTabTemplates.Add(tab);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Obtiene la configuración de una gráfica
        /// </summary>
        /// <param name="pLang">Idioma.</param>
        /// <param name="pUserId">ID del usuario.</param>
        /// <param name="pPageId">ID de la página</param>
        /// <param name="pGraphicId">ID de la gráfica</param>
        public static Grafica ObtenerGraficaConfig(string pLang, string pUserId, string pPageId, string pGraphicId)
        {
            // Compruebo si es administrador
            bool isAdmin = IsAdmin(pUserId);
            ConfigModel configModel = mTabTemplates.FirstOrDefault(x => x.Identificador == pPageId);
            if (!isAdmin || configModel == null)
            {
                return null;
            }
            return configModel.Graficas.FirstOrDefault(x => x.Identificador == pGraphicId);
        }

        /// <summary>
        /// Edita la configuración de la gráfica.
        /// </summary>
        /// <param name="pLang">Idioma.</param>
        /// <param name="pUserId">ID del usuario.</param>
        /// <param name="pGraphicId">ID de la gráfica a editar</param>
        /// <param name="pPageId">ID de la página de la gráfica.</param>
        /// <param name="pGraphicName">Nuevo nombre de la gráfica.</param>
        /// <param name="pGraphicOrder">Nuevo orden de la gráfica.</param>
        /// <param name="pGraphicWidth">Nuevo ancho de la gráfica.</param>
        /// <param name="pBlockId">ID de grupo de la gráfica si tiene grupo</param>
        public static bool EditarConfig(string pLang, string pUserId, string pGraphicId, string pPageId, string pGraphicName = "", int pGraphicOrder = 0, int pGraphicWidth = 0, string pBlockId = "")
        {
            // Compruebo si es administrador
            bool isAdmin = IsAdmin(pUserId);
            ConfigModel configModel = mTabTemplates.FirstOrDefault(x => x.Identificador == pPageId);
            if (!isAdmin || configModel == null)
            {
                return false;
            }
            // Obtengo la gráfica
            Grafica grafica = configModel.Graficas.FirstOrDefault(x => x.Identificador == pGraphicId);
            if (grafica == null)
            {
                return false;
            }

            // Edito el nombre de la gráfica.
            if (pGraphicName != "")
            {
                grafica.Nombre[pLang] = pGraphicName;
            }

            // Si la gráfica pertenece a un grupo edito su anchura y orden también.
            if (!string.IsNullOrEmpty(pBlockId))
            {
                grafica = configModel.Graficas.FirstOrDefault(x => x.Identificador == pBlockId);
            }

            // Edito la anchura de la gráfica.
            if (pGraphicWidth != 0 && grafica != null)
            {
                grafica.Anchura = pGraphicWidth;
            }

            // Edito el orden de la gráfica.
            if (pGraphicOrder != 0)
            {
                pGraphicOrder--;
                Dictionary<string, List<Grafica>> dicGraficasGrupos = new Dictionary<string, List<Grafica>>();
                List<Grafica> listaRemove = new List<Grafica>();

                foreach (Grafica item in configModel.Graficas)
                {
                    if (!string.IsNullOrEmpty(item.IdGrupo))
                    {
                        if (dicGraficasGrupos.ContainsKey(item.IdGrupo))
                        {
                            dicGraficasGrupos[item.IdGrupo].Add(item);
                            listaRemove.Add(item);
                        }
                        else
                        {
                            dicGraficasGrupos.Add(item.IdGrupo, new List<Grafica>() { });
                        }
                    }
                }

                foreach (Grafica item in listaRemove)
                {
                    configModel.Graficas.Remove(item);
                }

                configModel.Graficas.Remove(grafica);

                if (pGraphicOrder > configModel.Graficas.Count)
                {
                    configModel.Graficas.Add(grafica);
                }
                else
                {
                    configModel.Graficas.Insert(pGraphicOrder, grafica);
                }

                if (dicGraficasGrupos.Count > 0)
                {
                    foreach (KeyValuePair<string, List<Grafica>> item in dicGraficasGrupos)
                    {
                        int index = configModel.Graficas.IndexOf(configModel.Graficas.FirstOrDefault(x => x.IdGrupo == item.Key));
                        if (index + 1 > configModel.Graficas.Count)
                        {
                            configModel.Graficas.AddRange(item.Value);
                        }
                        else
                        {
                            configModel.Graficas.InsertRange(index + 1, item.Value);
                        }
                    }
                }
            }

            // Guardo la configuración en el JSON.
            string pathConfig = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Config", "configGraficas");
            string jsonName = "";
            foreach (string file in Directory.EnumerateFiles(pathConfig))
            {
                ConfigModel tab = JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText(file));
                if (tab.Identificador == configModel.Identificador)
                {
                    jsonName = file.Split("\\").LastOrDefault();
                    break;
                }
            }

            string path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Config", "configGraficas", jsonName);
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            configModel.Graficas.ForEach(x => x.Identificador = x.Identificador.Contains('-') ? x.Identificador.Split('-').LastOrDefault() : x.Identificador);
            string json = JsonConvert.SerializeObject(configModel, settings);
            File.WriteAllText(path, json);

            mTabTemplates = new List<ConfigModel>();
            foreach (string file in Directory.EnumerateFiles(pathConfig))
            {
                ConfigModel tab = JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText(file));
                mTabTemplates.Add(tab);
            }

            // Si la gráfica pertenece a un grupo ahora edito la anchura y orden de las demás gráficas del grupo.
            if (!string.IsNullOrEmpty(pBlockId))
            {
                EditarConfig(pLang, pUserId, pGraphicId, pPageId, pGraphicName);
            }
            return true;
        }

        /// <summary>
        /// Descarga el fichero json correspondiente.
        /// </summary>
        /// <param name="pLang">Idioma.</param>
        /// <param name="pConfigName">Nombre del fichero.</param>
        /// <param name="pUserId">Identificador del usuario.</param>
        /// <returns></returns>
        public static byte[] DescargarConfig(string pLang, string pConfigName, string pUserId = "")
        {
            // Compruebo si es administrador
            bool isAdmin = IsAdmin(pUserId);
            if (!isAdmin)
            {
                return null;
            }
            string pathConfig = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Config", "configGraficas");
            string path = "";
            string fileName = pConfigName;
            foreach (string file in Directory.EnumerateFiles(pathConfig))
            {
                if (Path.GetFileName(file).Contains(fileName))
                {
                    path = file;
                    break;
                }
            }
            string config = File.ReadAllText(path, Encoding.UTF8);

            return Encoding.UTF8.GetBytes(config);
        }

        /// <summary>
        /// Obtiene los datos de las páginas.
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <param name="pLang">Idioma.</param>
        /// <returns></returns>
        public static List<Pagina> GetPages(string pLang, string userId = "")
        {
            List<Pagina> listaPaginas = new List<Pagina>();

            // Lectura de los JSON de configuración.
            List<ConfigModel> listaConfigModels = TabTemplates;
            foreach (ConfigModel configModel in listaConfigModels)
            {
                listaPaginas.Add(CrearPagina(configModel, pLang, userId));
            }
            return listaPaginas;
        }

        /// <summary>
        /// Crea el objeto página.
        /// </summary>
        /// <param name="pConfigModel">Configuración.</param>
        /// <param name="pLang">Idioma.</param>
        /// <returns></returns>
        public static Pagina CrearPagina(ConfigModel pConfigModel, string pLang, string userId = "")
        {
            Pagina pagina = new Pagina();
            pagina.id = pConfigModel.Identificador;
            pagina.nombre = GetTextLang(pLang, pConfigModel.Nombre);
            
            pagina.listaConfigGraficas = new List<ConfigPagina>();
            foreach (Grafica itemGrafica in pConfigModel.Graficas)
            {
                ConfigPagina configPagina = new ConfigPagina()
                {
                    id = itemGrafica.Identificador,
                    anchura = itemGrafica.Anchura,
                    idGrupo = itemGrafica.IdGrupo,
                    isPrivate = itemGrafica.IsPrivate

                };

                if (itemGrafica.IsPrivate && !IsGraphicManager(userId))
                {
                    continue;
                }

                string prefijoNodos = "nodes";
                string prefijoBarraHorizonal = "isHorizontal";
                string prefijoCircular = "circular";
                string prefijoAbreviar = "abr";
                string prefijoPorcentaje = "prc";

                configPagina.isCircular = itemGrafica.Tipo == EnumGraficas.Circular;
                configPagina.isAbr = itemGrafica.Config.Abreviar;
                configPagina.hideLegend = itemGrafica.Config.OcultarLeyenda;
                configPagina.isNodes = itemGrafica.Tipo == EnumGraficas.Nodos;
                configPagina.isHorizontal = !(itemGrafica.Tipo == EnumGraficas.Circular || itemGrafica.Config.OrientacionVertical);
                configPagina.isCircular = itemGrafica.Tipo == EnumGraficas.Circular;
                configPagina.isPercentage = itemGrafica.Config.Porcentual;

                if (itemGrafica.Config.Abreviar && !itemGrafica.Identificador.Contains(prefijoAbreviar))
                {
                    itemGrafica.Identificador = prefijoAbreviar + "-" + itemGrafica.Identificador;
                    configPagina.id = prefijoAbreviar + "-" + configPagina.id;
                }

                if (itemGrafica.Tipo == EnumGraficas.Nodos && !itemGrafica.Identificador.Contains(prefijoNodos))
                {
                    itemGrafica.Identificador = prefijoNodos + "-" + itemGrafica.Identificador;
                    configPagina.id = prefijoNodos + "-" + configPagina.id;
                }
                else if (!(itemGrafica.Tipo == EnumGraficas.Circular || itemGrafica.Config.OrientacionVertical) && !itemGrafica.Identificador.Contains(prefijoBarraHorizonal) && !itemGrafica.Identificador.Contains(prefijoNodos))
                {
                    itemGrafica.Identificador = prefijoBarraHorizonal + "-" + itemGrafica.Identificador;
                    configPagina.id = prefijoBarraHorizonal + "-" + configPagina.id;

                }
                else if (itemGrafica.Tipo == EnumGraficas.Circular && !itemGrafica.Identificador.Contains(prefijoCircular))
                {
                    itemGrafica.Identificador = prefijoCircular + "-" + itemGrafica.Identificador;
                    configPagina.id = prefijoCircular + "-" + configPagina.id;

                }
                if (itemGrafica.Config.Porcentual && !itemGrafica.Identificador.Contains(prefijoPorcentaje))
                {
                    itemGrafica.Identificador = prefijoPorcentaje + "-" + itemGrafica.Identificador;
                    configPagina.id = prefijoPorcentaje + "-" + configPagina.id;

                }

                // Si la anchura no contiene un valor aceptado, se le asigna 1/2 por defecto.
                List<int> valoresAceptados = new List<int>() { 11, 12, 13, 14, 16, 23, 34, 38, 58 };
                if (!valoresAceptados.Contains(configPagina.anchura))
                {
                    configPagina.anchura = 12;
                }

                pagina.listaConfigGraficas.Add(configPagina);
            }
            
            pagina.listaIdsFacetas = new List<string>();
            foreach (FacetaConf itemFaceta in pConfigModel.Facetas)
            {
                string reciproca = string.IsNullOrEmpty(itemFaceta.Reciproca) ? "" : "(((" + itemFaceta.Reciproca;
                pagina.listaIdsFacetas.Add(itemFaceta.Filtro + reciproca);
            }
            return pagina;
        }
        #endregion

        #region --- Gráficas
        /// <summary>
        /// Lee la configuración y obtiene los datos necesarios para el servicio de gráficas.
        /// </summary>
        /// <param name="pIdPagina">Identificador de la página.</param>
        /// <param name="pIdGrafica">Identificador de la gráfica.</param>
        /// <param name="pFiltroFacetas">Filtros de la URL.</param>
        /// <param name="pLang">Idioma.</param>
        /// <returns></returns>
        public static GraficaBase GetGrafica(string pIdPagina, string pIdGrafica, string pFiltroFacetas, string pLang)
        {
            // Lectura del JSON de configuración.
            ConfigModel configModel = TabTemplates.FirstOrDefault(x => x.Identificador == pIdPagina);
            if (configModel == null)
            {
                return null;
            }
            // Obtiene los filtros relacionados con las fechas.
            List<string> listaFacetasAnios = configModel.Facetas.Where(x => x.RangoAnio).Select(x => x.Filtro).ToList();
            Grafica grafica = configModel.Graficas.FirstOrDefault(x => x.Identificador.Split('-').LastOrDefault() == pIdGrafica.Split('-').LastOrDefault());
            return CrearGrafica(grafica, configModel.Filtro, pFiltroFacetas, pLang, listaFacetasAnios);
        }

        /// <summary>
        /// Crea el objeto gráfica.
        /// </summary>
        /// <param name="pGrafica">Configuración.</param>
        /// <param name="pFiltroBase">Filtros base.</param>
        /// <param name="pFiltroFacetas">Filtros de las facetas.</param>
        /// <param name="pLang">Idioma.</param>
        /// <param name="pListaDates">Lista de valores que corresponden a fechas</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static GraficaBase CrearGrafica(Grafica pGrafica, string pFiltroBase, string pFiltroFacetas, string pLang, List<string> pListaDates)
        {
            // Cambio los '+' para decodificar correctamente
            if (!string.IsNullOrEmpty(pFiltroFacetas))
            {
                pFiltroFacetas = HttpUtility.UrlDecode(pFiltroFacetas.Replace("+", "simbolomasdecodificar"));
                pFiltroFacetas = pFiltroFacetas.Replace("simbolomasdecodificar", "+");
            }

            switch (pGrafica.Tipo)
            {
                case EnumGraficas.Barras:
                    if (pGrafica.Config.OrientacionVertical)
                    {
                        ControlarExcepcionesBarrasX(pGrafica);
                        return CrearGraficaBarrasVertical(pGrafica, pFiltroBase, pFiltroFacetas, pLang, pListaDates, pGrafica.Config.DatosNodos);
                    }
                    else
                    {
                        ControlarExcepcionesBarrasY(pGrafica);
                        return CrearGraficaBarrasHorizontal(pGrafica, pFiltroBase, pFiltroFacetas, pLang, pListaDates, pGrafica.Config.DatosNodos);
                    }
                case EnumGraficas.Circular:
                    ControlarExcepcionesCircular(pGrafica);
                    return CrearGraficaCircular(pGrafica, pFiltroBase, pFiltroFacetas, pLang, pListaDates);
                case EnumGraficas.Nodos:
                    return CrearGraficaNodos(pGrafica, pFiltroBase, pFiltroFacetas, pLang, pListaDates);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Crea el objeto de la gráfica (Gráfica de Barras vertical).
        /// </summary>
        /// <param name="pGrafica">Configuración.</param>
        /// <param name="pFiltroBase">Filtros base.</param>
        /// <param name="pFiltroFacetas">Filtros de las facetas.</param>
        /// <param name="pLang">Idioma.</param>
        /// <param name="pListaDates">Lista de valores que corresponden a fechas</param>
        /// <param name="pNodos">Indica si la gráfica obtiene los datos como una de nodos</param>
        /// <returns></returns>
        public static GraficaBarras CrearGraficaBarrasVertical(Grafica pGrafica, string pFiltroBase, string pFiltroFacetas, string pLang, List<string> pListaDates, bool pNodos)
        {
            // Objeto a devolver.
            GraficaBarras grafica = new GraficaBarras();
            grafica.Type = "bar"; // Por defecto, de tipo bar.

            // Tipo.
            grafica.isHorizontal = true;

            // Abreviación.            
            grafica.isAbr = pGrafica.Config.Abreviar;

            // Porcentaje.
            grafica.isPercentage = pGrafica.Config.Porcentual;

            //Ocultar leyenda
            grafica.hideLeyend = pGrafica.Config.OcultarLeyenda;

            // ID Grupo.
            if (!string.IsNullOrEmpty(pGrafica.IdGrupo))
            {
                grafica.groupId = pGrafica.IdGrupo;
            }

            // Es fecha
            grafica.isDate = (pListaDates != null && pListaDates.Any() && pListaDates.Contains(pGrafica.Config.EjeX));

            // Asignación de Data.
            DataBarras data = new();
            data.Datasets = new ConcurrentBag<DatasetBarras>();
            grafica.Data = data;

            // Asignación de Options.
            Options options = new();

            // Orientación
            options.indexAxis = "x";
            options.scales = new Dictionary<string, Eje>();

            // Ejes Y
            foreach (EjeYConf item in pGrafica.Config.YAxisPrint)
            {
                Eje eje = new();
                eje.position = item.Posicion;
                eje.title = new();
                eje.title.display = true;

                if (item.NombreEje != null)
                {
                    eje.title.text = GetTextLang(pLang, item.NombreEje);
                }
                else
                {
                    eje.title.text = string.Empty;
                }

                options.scales.Add(item.YAxisID, eje);
            }

            // Animación
            options.animation = new();
            options.animation.duration = 2000;

            // Título
            options.plugins = new();
            options.plugins.title = new();
            options.plugins.title.display = true;
            options.plugins.title.text = GetTextLang(pLang, pGrafica.Nombre);

            grafica.Options = options;

            ConcurrentDictionary<Dimension, List<Tuple<string, string, float>>> resultadosDimension = new ConcurrentDictionary<Dimension, List<Tuple<string, string, float>>>();
            Dictionary<Dimension, DatasetBarras> dimensionesDataset = new Dictionary<Dimension, DatasetBarras>();

            bool ejeFechas = false;
            Parallel.ForEach(pGrafica.Config.Dimensiones, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, itemGrafica =>
            {
                List<Tuple<string, string, float>> listaTuplas = new List<Tuple<string, string, float>>();
                SparqlObject resultadoQuery = null;
                StringBuilder select = new StringBuilder(), where = new StringBuilder();
                // Orden.               
                string orden = "DESC";
                if (pGrafica.Config.OrderDesc)
                {
                    orden = "ASC";
                }
                foreach (string item in pListaDates)
                {
                    if (item.Contains(pGrafica.Config.EjeX))
                    {
                        orden = "ASC";
                        ejeFechas = true;
                        break;
                    }
                }
                bool filtroEspecial = UtilsGraficas.IsFiltroEspecial(itemGrafica);
                List<string> filtros = UtilsGraficas.GetFiltrosBarras(pGrafica, itemGrafica, pFiltroFacetas, pFiltroBase, pListaDates);
                if (pNodos)
                {
                    //Nodos
                    Dictionary<string, string> dicNodos = new ();

                    //Relaciones
                    Dictionary<string, List<DataQueryRelaciones>> dicRelaciones = new ();

                    //Respuesta
                    List<DataItemRelacion> itemsRelacion = new ();

                    Dictionary<string, List<string>> dicResultadosAreaRelacionAreas = new ();
                    Dictionary<string, int> scoreNodes = new ();
                    // Consulta sparql.
                    select.Append(mPrefijos);
                    select.Append("SELECT ?s group_concat(?categoria;separator=\",\") AS ?idCategorias ");
                    where.Append("WHERE { ");
                    foreach (string item in filtros)
                    {
                        where.Append(item);
                    }
                    where.Append($@"?s {pGrafica.PropCategoryPath} ?area. ");
                    where.Append("?area roh:categoryNode ?categoria. ");
                    where.Append("MINUS { ?categoria skos:narrower ?hijos } ");
                    where.Append("} ");

                    resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mCommunityID);
                    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                    {
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            string idCategorias = fila["idCategorias"].value;
                            HashSet<string> categorias = new HashSet<string>(idCategorias.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));

                            foreach (string categoria in categorias)
                            {
                                if (!scoreNodes.ContainsKey(categoria))
                                {
                                    scoreNodes.Add(categoria, 0);
                                }

                                scoreNodes[categoria]++;

                                if (!dicResultadosAreaRelacionAreas.ContainsKey(categoria))
                                {
                                    dicResultadosAreaRelacionAreas.Add(categoria, new List<string>());
                                }

                                dicResultadosAreaRelacionAreas[categoria].AddRange(categorias.Except(new List<string>() { categoria }));
                            }
                        }
                    }

                    ProcesarRelaciones("Category", dicResultadosAreaRelacionAreas, ref dicRelaciones);

                    int maximasRelaciones = 0;
                    foreach (KeyValuePair<string, List<DataQueryRelaciones>> sujeto in dicRelaciones)
                    {
                        foreach (DataQueryRelaciones relaciones in sujeto.Value)
                        {
                            foreach (Datos relaciones2 in relaciones.idRelacionados)
                            {
                                maximasRelaciones = Math.Max(maximasRelaciones, relaciones2.numVeces);
                            }
                        }
                    }

                    // Creamos los nodos y las relaciones en función de pNumAreas.
                    int pNumAreas = pGrafica.Config.NumMaxNodos;

                    Dictionary<string, int> numRelaciones = new Dictionary<string, int>();
                    foreach (KeyValuePair<string, List<DataQueryRelaciones>> sujeto in dicRelaciones)
                    {
                        if (!numRelaciones.ContainsKey(sujeto.Key))
                        {
                            numRelaciones.Add(sujeto.Key, 0);
                        }
                        foreach (DataQueryRelaciones relaciones in sujeto.Value)
                        {
                            foreach (Datos relaciones2 in relaciones.idRelacionados)
                            {
                                if (!numRelaciones.ContainsKey(relaciones2.idRelacionado))
                                {
                                    numRelaciones.Add(relaciones2.idRelacionado, 0);
                                }
                                numRelaciones[sujeto.Key] += relaciones2.numVeces;
                                numRelaciones[relaciones2.idRelacionado] += relaciones2.numVeces;
                            }
                        }
                    }

                    List<string> itemsSeleccionados = numRelaciones.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value).Keys.Distinct().ToList();
                    if (itemsSeleccionados.Count > pNumAreas && pNumAreas != 0)
                    {
                        itemsSeleccionados = itemsSeleccionados.GetRange(0, pNumAreas);
                    }

                    if (itemsSeleccionados.Count > 0)
                    {
                        // Recuperamos los nombres de categorías y creamos los nodos.
                        select = new StringBuilder();
                        where = new StringBuilder();

                        select.Append(mPrefijos);
                        select.Append("SELECT ?categoria ?nombreCategoria ");
                        where.Append("WHERE { ");
                        where.Append("?categoria skos:prefLabel ?nombreCategoria. ");
                        where.Append($@"FILTER(?categoria IN (<{string.Join(">,<", itemsSeleccionados)}>)) ");
                        where.Append("} ");

                        resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mCommunityID);
                        if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                        {
                            foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                            {
                                if (!dicNodos.ContainsKey(fila["categoria"].value))
                                {
                                    dicNodos.Add(fila["categoria"].value, fila["nombreCategoria"].value);
                                }
                            }
                        }

                        // Nodos. 
                        if (dicNodos != null && dicNodos.Count > 0)
                        {
                            foreach (KeyValuePair<string, string> nodo in dicNodos)
                            {
                                string clave = nodo.Key;
                                Data data = new Data(clave, nodo.Value, null, null, null, "nodes", Data.Type.icon_area);
                                if (scoreNodes.ContainsKey(clave))
                                {
                                    data.score = scoreNodes[clave];
                                    data.name = $"{data.name} ({data.score})";
                                }
                                DataItemRelacion dataColabo = new DataItemRelacion(data, true, true);
                                itemsRelacion.Add(dataColabo);
                            }
                        }
                    }

                    resultadosDimension[itemGrafica] = listaTuplas;
                }
                else if (string.IsNullOrEmpty(itemGrafica.Calculo))
                {
                    // Consulta sparql.                    
                    select = new StringBuilder();
                    where = new StringBuilder();

                    select.Append(mPrefijos);
                    if (filtroEspecial)
                    {
                        select.Append($@"SELECT ?ejeX ?aux COUNT(DISTINCT ?s) AS ?numero ");
                    }
                    else
                    {
                        select.Append($@"SELECT ?ejeX COUNT(DISTINCT ?s) AS ?numero ");
                    }
                    where.Append("WHERE { ");
                    foreach (string item in filtros)
                    {
                        where.Append(item);
                    }
                    if (!string.IsNullOrEmpty(itemGrafica.Minus))
                    {
                        where.Append($@"MINUS {{ ?s {itemGrafica.Minus} ?menos }}");
                    }
                    if (filtroEspecial)
                    {
                        where.Append($@"FILTER(LANG(?aux) = 'es' OR LANG(?aux) = '' OR !isLiteral(?aux))");
                    }
                    else
                    {
                        where.Append($@"FILTER(LANG(?ejeX) = 'es' OR LANG(?ejeX) = '' OR !isLiteral(?ejeX))");
                    }
                    if (ejeFechas)
                    {
                        where.Append($@"}} ORDER BY {orden}(?ejeX) ");
                    }
                    else
                    {
                        where.Append($@"}} ORDER BY {orden}(?numero) ");
                    }
                }
                else
                {
                    // Cálculo (SUM|AVG|MIN|MAX)
                    string calculo = itemGrafica.Calculo;

                    // Consulta sparql.
                    select = new StringBuilder();
                    where = new StringBuilder();

                    select.Append(mPrefijos);
                    select.Append($@"SELECT ?ejeX {calculo}(?aux) AS ?numero ");
                    where.Append("WHERE { ");
                    foreach (string item in filtros)
                    {
                        where.Append(item);
                    }
                    where.Append($@"FILTER(LANG(?ejeX) = 'es' OR LANG(?ejeX) = '' OR !isLiteral(?ejeX))");
                    if (ejeFechas)
                    {
                        where.Append($@"}} ORDER BY {orden}(?ejeX) ");
                    }
                    else
                    {
                        where.Append($@"}} ORDER BY {orden}(?numero) ");
                    }
                }

                resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mCommunityID);
                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                {
                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                    {
                        if (filtroEspecial && string.IsNullOrEmpty(itemGrafica.Calculo))
                        {
                            listaTuplas.Add(new Tuple<string, string, float>(fila["ejeX"].value, fila["aux"].value, float.Parse(fila["numero"].value.Replace(",", "."), CultureInfo.InvariantCulture)));
                        }
                        else
                        {
                            if (itemGrafica.Filtro == "" && itemGrafica.Color == "#817E80")
                            {
                                // --- ÑAPA
                                listaTuplas.Add(new Tuple<string, string, float>(fila["ejeX"].value, string.Empty, float.Parse(fila["numero"].value.Replace(",", "."), CultureInfo.InvariantCulture) + 5.0f));
                            }
                            else
                            {
                                listaTuplas.Add(new Tuple<string, string, float>(fila["ejeX"].value, string.Empty, float.Parse(fila["numero"].value.Replace(",", "."), CultureInfo.InvariantCulture)));
                            }
                        }
                    }
                }
                if (itemGrafica.DividirDatos)
                {
                    HashSet<string> listaAux = new HashSet<string>();
                    foreach (Tuple<string, string, float> tupla in listaTuplas)
                    {
                        listaAux.Add(tupla.Item2);
                    }
                    int ordenAux = itemGrafica.Orden;
                    foreach (string aux in listaAux.OrderBy(x => x))
                    {
                        Dimension itemAux = itemGrafica.DeepCopy();
                        foreach (KeyValuePair<string, string> item in itemAux.Nombre)
                        {
                            itemAux.Nombre[item.Key] = item.Value + " " + aux;
                            switch (aux)
                            {
                                case "1":
                                    itemAux.Color = "#45DCB4";
                                    break;
                                case "2":
                                    itemAux.Color = "#EAF112";
                                    break;
                                case "3":
                                    itemAux.Color = "#DE921E";
                                    break;
                                case "4":
                                    itemAux.Color = "#DC4545";
                                    break;
                                default:
                                    break;
                            }
                        }
                        itemAux.Orden = ordenAux;
                        ordenAux++;
                        resultadosDimension[itemAux] = listaTuplas.Where(x => x.Item2 == aux).ToList();
                    }
                }
                else
                {
                    resultadosDimension[itemGrafica] = listaTuplas;
                }
            });

            #region --- Cálculo de los valores del Eje X
            HashSet<string> valuesEje = new HashSet<string>();
            HashSet<string> tipos = new HashSet<string>();

            List<Tuple<string, float>> rangoValor = new List<Tuple<string, float>>();

            foreach (KeyValuePair<Dimension, List<Tuple<string, string, float>>> item in resultadosDimension)
            {
                if (item.Value != null && item.Value.Any())
                {
                    if (pGrafica.Config.Rango)
                    {
                        int indice;
                        float[] sumaDatos = new float[pGrafica.Config.Rangos.Length];
                        string[] rangos = pGrafica.Config.Rangos;
                        foreach (Tuple<string, string, float> item2 in item.Value)
                        {
                            string valor = item2.Item1;
                            if (item2.Item1.Contains(','))
                            {
                                valor = item2.Item1.Replace(',', '.');
                            }
                            indice = Convert.ToInt32(Math.Floor(double.Parse(valor)));
                            for (int i = 0; i < rangos.Length; i++)
                            {
                                int numeroComparar1, numeroComparar2;
                                if (rangos[i].Contains('-'))
                                {
                                    numeroComparar1 = int.Parse(rangos[i].Split('-')[0]);
                                    numeroComparar2 = int.Parse(rangos[i].Split('-')[1]);
                                }
                                else
                                {
                                    numeroComparar1 = int.Parse(rangos[i].Split('+')[0]);
                                    numeroComparar2 = int.MaxValue;
                                }
                                if (indice >= numeroComparar1 && indice <= numeroComparar2)
                                {
                                    sumaDatos[i] += item2.Item3;
                                }
                            }
                        }
                        for (int i = 0; i < sumaDatos.Length; i++)
                        {
                            if (sumaDatos[i] > 0)
                            {
                                rangoValor.Add(new Tuple<string, float>(rangos[i], sumaDatos[i]));
                            }
                        }
                    }
                    else
                    {
                        foreach (Tuple<string, string, float> item2 in item.Value)
                        {
                            valuesEje.Add(item2.Item1);
                            tipos.Add(item2.Item2);
                        }
                    }
                }
            }

            bool isInt = !valuesEje.Any(x => !int.TryParse(x, out int aux));

            if (pGrafica.Config.RellenarEjeX && isInt && valuesEje.Count > 0)
            {
                int numMin = valuesEje.Min(x => int.Parse(x));
                int numMax = valuesEje.Max(x => int.Parse(x));
                for (int i = numMin; i <= numMax; i++)
                {
                    valuesEje.Add(i.ToString());
                }
            }

            if (ejeFechas || pGrafica.Config.Rango)
            {
                foreach (KeyValuePair<Dimension, List<Tuple<string, string, float>>> item in resultadosDimension)
                {
                    if (item.Value != null && item.Value.Any())
                    {
                        foreach (string valor in valuesEje)
                        {
                            Tuple<string, string, float> tuplaAux = item.Value.FirstOrDefault(x => x.Item1.Equals(valor));
                            if (tuplaAux == null)
                            {
                                item.Value.Add(new Tuple<string, string, float>(valor, "", 0));
                            }
                        }
                    }

                    if (isInt)
                    {
                        resultadosDimension[item.Key] = item.Value.OrderBy(x => Convert.ToInt32(Math.Floor(double.Parse(x.Item1)))).ToList();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(pGrafica.Config.EjeX))
                        {
                            resultadosDimension[item.Key] = item.Value.OrderBy(x => x.Item1).ToList();
                        }
                    }
                }

                if (isInt)
                {
                    valuesEje = new HashSet<string>(valuesEje.OrderBy(item => int.Parse(item)));
                }
                else
                {
                    if (!string.IsNullOrEmpty(pGrafica.Config.EjeX))
                    {
                        valuesEje = new HashSet<string>(valuesEje.OrderBy(item => item));
                    }
                }
            }
            #endregion

            // Obtención del objeto de la gráfica.
            List<string> listaLabels = new List<string>();
            if (pGrafica.Config.Rango)
            {
                foreach (Tuple<string, float> itemAux in rangoValor)
                {
                    listaLabels.Add(itemAux.Item1);
                }
            }
            else
            {
                listaLabels = valuesEje.ToList();
            }

            foreach (KeyValuePair<Dimension, List<Tuple<string, string, float>>> item in resultadosDimension)
            {
                DatasetBarras dataset = new DatasetBarras();
                List<float> listaData = new List<float>();
                if (pGrafica.Config.Rango)
                {
                    foreach (Tuple<string, float> itemAux in rangoValor)
                    {
                        listaData.Add(itemAux.Item2);
                    }
                }
                else
                {
                    foreach (Tuple<string, string, float> itemAux in item.Value)
                    {
                        listaData.Add(itemAux.Item3);
                    }
                }
                dataset.Data = listaData;

                // Nombre del dato en leyenda.
                dataset.Label = GetTextLang(pLang, item.Key.Nombre);

                // Color.
                dataset.BackgroundColor = ObtenerColores(dataset.Data.Count, item.Key.Color);
                dataset.Type = item.Key.TipoDimension;

                // Anchura.
                dataset.BarPercentage = 1;
                if (item.Key.Anchura != 0)
                {
                    dataset.BarPercentage = item.Key.Anchura;
                }
                // Anchura máxima.
                dataset.MaxBarThickness = 300;

                // Stack.
                if (!string.IsNullOrEmpty(item.Key.Stack))
                {
                    dataset.Stack = item.Key.Stack;
                }
                else
                {
                    dataset.Stack = Guid.NewGuid().ToString();
                }

                // Eje Y.
                dataset.YAxisID = item.Key.YAxisID;

                // Orden
                dataset.Order = item.Key.Orden;

                data.Labels = listaLabels;
                data.Type = item.Key.TipoDimension;
                dimensionesDataset[item.Key] = dataset;
            }

            if (pGrafica.Config.Dimensiones.Any(x => x.DividirDatos))
            {
                foreach (Dimension dim in resultadosDimension.Keys.OrderBy(x => x.Orden))
                {
                    grafica.Data.Datasets.Add(dimensionesDataset[dim]);
                }
            }
            else
            {
                foreach (Dimension dim in pGrafica.Config.Dimensiones)
                {
                    grafica.Data.Datasets.Add(dimensionesDataset[dim]);
                }
            }
            return grafica;
        }

        /// <summary>
        /// Crea el objeto de la gráfica (Gráfica de Barras horizontal).
        /// </summary>
        /// <param name="pGrafica">Configuración.</param>
        /// <param name="pFiltroBase">Filtros base.</param>
        /// <param name="pFiltroFacetas">Filtros de las facetas.</param>
        /// <param name="pLang">Idioma.</param>
        /// <param name="pListaDates">Lista de valores que corresponden a fechas</param>
        /// <param name="pNodos">Indica si la gráfica obtiene los datos como una de nodos</param>
        /// <returns></returns>
        public static GraficaBarrasY CrearGraficaBarrasHorizontal(Grafica pGrafica, string pFiltroBase, string pFiltroFacetas, string pLang, List<string> pListaDates, bool pNodos)
        {
            // Objeto a devolver.
            GraficaBarrasY grafica = new GraficaBarrasY();
            grafica.Type = "bar"; // Por defecto, de tipo bar.

            // Tipo.
            grafica.isVertical = true;

            // Abreviación.
            if (pGrafica.Config.Abreviar)
            {
                grafica.isAbr = pGrafica.Config.Abreviar;
            }
            if (pGrafica.Config.OcultarLeyenda)
            {
                grafica.hideLeyend = true;
            }
            // Porcentaje.
            if (pGrafica.Config.Porcentual)
            {
                grafica.isPercentage = pGrafica.Config.Porcentual;
            }

            // ID Grupo.
            if (!string.IsNullOrEmpty(pGrafica.IdGrupo))
            {
                grafica.groupId = pGrafica.IdGrupo;
            }

            // Es fecha.
            if (pListaDates != null && pListaDates.Any() && pListaDates.Contains(pGrafica.Config.EjeX))
            {
                grafica.isDate = true;
            }

            // Asignación de Data.
            DataBarrasY data = new DataBarrasY();
            data.Datasets = new ConcurrentBag<DatasetBarrasY>();
            grafica.Data = data;

            // Asignación de Options.
            Options options = new Options();

            // Orientación
            options.indexAxis = "y";

            options.scales = new Dictionary<string, Eje>();

            // Ejes X
            foreach (EjeXConf item in pGrafica.Config.XAxisPrint)
            {
                Eje eje = new Eje();
                eje.position = item.Posicion;
                eje.title = new Title();
                eje.title.display = true;

                if (item.NombreEje != null)
                {
                    eje.title.text = GetTextLang(pLang, item.NombreEje);
                }
                else
                {
                    eje.title.text = string.Empty;
                }

                options.scales.Add(item.XAxisID, eje);
            }

            // Animación
            options.animation = new Animation();
            options.animation.duration = 2000;

            // Título
            options.plugins = new Plugin();
            options.plugins.title = new Title();
            options.plugins.title.display = true;
            options.plugins.title.text = GetTextLang(pLang, pGrafica.Nombre);

            grafica.Options = options;

            ConcurrentDictionary<Dimension, List<Tuple<string, string, float>>> resultadosDimension = new ConcurrentDictionary<Dimension, List<Tuple<string, string, float>>>();
            Dictionary<Dimension, DatasetBarrasY> dimensionesDataset = new Dictionary<Dimension, DatasetBarrasY>();

            bool ejeFechas = false;
            Parallel.ForEach(pGrafica.Config.Dimensiones, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, itemGrafica =>
            {
                List<Tuple<string, string, float>> listaTuplas = new List<Tuple<string, string, float>>();
                SparqlObject resultadoQuery = null;
                StringBuilder select = new StringBuilder(), where = new StringBuilder();
                // Orden.                
                string orden = "DESC";
                if (pGrafica.Config.OrderDesc)
                {
                    orden = "ASC";
                }
                foreach (string item in pListaDates)
                {
                    if (item.Contains(pGrafica.Config.EjeX))
                    {
                        orden = "ASC";
                        ejeFechas = true;
                        break;
                    }
                }
                // Filtro de página.
                bool filtroEspecial = UtilsGraficas.IsFiltroEspecial(itemGrafica);
                List<string> filtros = UtilsGraficas.GetFiltrosBarras(pGrafica, itemGrafica, pFiltroFacetas, pFiltroBase, pListaDates);

                if (pNodos)
                {
                    //Nodos            
                    Dictionary<string, string> dicNodos = new Dictionary<string, string>();
                    Dictionary<string, int> scoreNodes = new Dictionary<string, int>();

                    // Consulta sparql.
                    select.Append(mPrefijos);
                    select.Append("SELECT ?s group_concat(?categoria;separator=\",\") AS ?idCategorias ");
                    where.Append("WHERE { ");
                    foreach (string item in filtros)
                    {
                        where.Append(item);
                    }
                    where.Append($@"?s {pGrafica.PropCategoryPath} ?area. ");
                    where.Append("?area roh:categoryNode ?categoria. ");
                    where.Append("MINUS { ?categoria skos:narrower ?hijos } ");
                    where.Append("} ");

                    resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mCommunityID);
                    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                    {
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            string idCategorias = fila["idCategorias"].value;
                            HashSet<string> categorias = new HashSet<string>(idCategorias.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));

                            foreach (string categoria in categorias)
                            {
                                if (!scoreNodes.ContainsKey(categoria))
                                {
                                    scoreNodes.Add(categoria, 0);
                                }

                                scoreNodes[categoria]++;
                            }
                        }
                    }
                    List<string> itemsSeleccionados = scoreNodes.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value).Keys.Distinct().ToList();
                    int pNumAreas = pGrafica.Config.NumMaxNodos;
                    if (itemsSeleccionados.Count > pNumAreas && pNumAreas != 0)
                    {
                        itemsSeleccionados = itemsSeleccionados.GetRange(0, pNumAreas);
                    }
                    // Recuperamos los nombres de categorías y creamos los nodos.
                    select = new StringBuilder();
                    where = new StringBuilder();

                    select.Append(mPrefijos);
                    select.Append("SELECT ?categoria ?nombreCategoria ");
                    where.Append("WHERE { ");
                    where.Append("?categoria skos:prefLabel ?nombreCategoria. ");
                    where.Append($@"FILTER(?categoria IN (<{string.Join(">,<", itemsSeleccionados)}>)) ");
                    where.Append("} ");

                    resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mCommunityID);
                    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                    {
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            if (!dicNodos.ContainsKey(fila["categoria"].value))
                            {
                                dicNodos.Add(fila["categoria"].value, fila["nombreCategoria"].value);
                            }
                        }
                    }

                    // Añado a la lista de tuplas el nodo con su valor.
                    if (dicNodos != null && dicNodos.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> nodo in dicNodos)
                        {
                            if (scoreNodes.ContainsKey(nodo.Key))
                            {
                                listaTuplas.Add(new Tuple<string, string, float>(nodo.Value, string.Empty, float.Parse(scoreNodes[nodo.Key].ToString().Replace(",", "."), CultureInfo.InvariantCulture)));
                            }
                        }
                    }
                    resultadosDimension[itemGrafica] = listaTuplas.OrderByDescending(x => x.Item3).ToList();
                }
                else if (string.IsNullOrEmpty(itemGrafica.Calculo))
                {
                    // Consulta sparql.                    
                    select = new StringBuilder();
                    where = new StringBuilder();

                    select.Append(mPrefijos);
                    if (filtroEspecial)
                    {
                        select.Append($@"SELECT ?ejeX ?aux COUNT(DISTINCT ?s) AS ?numero ");
                    }
                    else
                    {
                        select.Append($@"SELECT ?ejeX COUNT(DISTINCT ?s) AS ?numero ");
                    }
                    where.Append("WHERE { ");
                    foreach (string item in filtros)
                    {
                        where.Append(item);
                    }
                    if (filtroEspecial)
                    {
                        where.Append($@"FILTER(LANG(?aux) = 'es' OR LANG(?aux) = '' OR !isLiteral(?aux))");
                    }
                    else
                    {
                        where.Append($@"FILTER(LANG(?ejeX) = 'es' OR LANG(?ejeX) = '' OR !isLiteral(?ejeX))");
                    }
                    if (ejeFechas)
                    {
                        where.Append($@"}} ORDER BY {orden}(?ejeX) ");
                    }
                    else
                    {
                        where.Append($@"}} ORDER BY {orden}(?numero) ");
                    }
                }
                else
                {
                    // Cálculo (SUM|AVG|MIN|MAX)
                    string calculo = itemGrafica.Calculo;

                    // Consulta sparql.
                    select = new StringBuilder();
                    where = new StringBuilder();

                    select.Append(mPrefijos);
                    select.Append($@"SELECT ?ejeX {calculo}(?aux) AS ?numero ");
                    where.Append("WHERE { ");
                    foreach (string item in filtros)
                    {
                        where.Append(item);
                    }
                    where.Append($@"FILTER(LANG(?ejeX) = 'es' OR LANG(?ejeX) = '' OR !isLiteral(?ejeX))");
                    if (ejeFechas)
                    {
                        where.Append($@"}} ORDER BY {orden}(?ejeX) ");
                    }
                    else
                    {
                        where.Append($@"}} ORDER BY {orden}(?numero) ");
                    }
                }
                if (!pNodos)
                {
                    resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mCommunityID);
                    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                    {
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            if (filtroEspecial && string.IsNullOrEmpty(itemGrafica.Calculo))
                            {
                                listaTuplas.Add(new Tuple<string, string, float>(fila["ejeX"].value, fila["aux"].value, float.Parse(fila["numero"].value.Replace(",", "."), CultureInfo.InvariantCulture)));
                            }
                            else
                            {
                                listaTuplas.Add(new Tuple<string, string, float>(fila["ejeX"].value, string.Empty, float.Parse(fila["numero"].value.Replace(",", "."), CultureInfo.InvariantCulture)));
                            }
                        }
                    }
                    if (itemGrafica.DividirDatos)
                    {
                        HashSet<string> listaAux = new HashSet<string>();
                        foreach (Tuple<string, string, float> tupla in listaTuplas)
                        {
                            listaAux.Add(tupla.Item2);
                        }
                        int ordenAux = itemGrafica.Orden;
                        foreach (string aux in listaAux.OrderBy(x => x))
                        {
                            Dimension itemAux = itemGrafica.DeepCopy();
                            foreach (KeyValuePair<string, string> item in itemAux.Nombre)
                            {
                                itemAux.Nombre[item.Key] = item.Value + " " + aux;
                                switch (aux)
                                {
                                    case "1":
                                        itemAux.Color = "#45DCB4";
                                        break;
                                    case "2":
                                        itemAux.Color = "#EAF112";
                                        break;
                                    case "3":
                                        itemAux.Color = "#DE921E";
                                        break;
                                    case "4":
                                        itemAux.Color = "#DC4545";
                                        break;
                                    default:
                                        break;
                                }
                            }
                            itemAux.Orden = ordenAux;
                            ordenAux++;
                            resultadosDimension[itemAux] = listaTuplas.Where(x => x.Item2 == aux).ToList();
                        }
                    }
                    else
                    {
                        resultadosDimension[itemGrafica] = listaTuplas;
                    }
                }
            });

            #region --- Cálculo de los valores del Eje X
            HashSet<string> valuesEje = new HashSet<string>();
            HashSet<string> tipos = new HashSet<string>();

            List<Tuple<string, float>> rangoValor = new List<Tuple<string, float>>();

            foreach (KeyValuePair<Dimension, List<Tuple<string, string, float>>> item in resultadosDimension)
            {
                if (item.Value != null && item.Value.Any())
                {
                    if (pGrafica.Config.Rango)
                    {
                        int indice;
                        float[] sumaDatos = new float[pGrafica.Config.Rangos.Length];
                        string[] rangos = pGrafica.Config.Rangos;
                        foreach (Tuple<string, string, float> item2 in item.Value)
                        {
                            indice = int.Parse(item2.Item1);
                            for (int i = 0; i < rangos.Length; i++)
                            {
                                int numeroComparar1, numeroComparar2;
                                if (rangos[i].Contains('-'))
                                {
                                    numeroComparar1 = int.Parse(rangos[i].Split('-')[0]);
                                    numeroComparar2 = int.Parse(rangos[i].Split('-')[1]);
                                }
                                else
                                {
                                    numeroComparar1 = int.Parse(rangos[i].Split('+')[0]);
                                    numeroComparar2 = int.MaxValue;
                                }
                                if (indice >= numeroComparar1 && indice < numeroComparar2)
                                {
                                    sumaDatos[i] += item2.Item3;
                                }
                            }
                        }
                        for (int i = 0; i < sumaDatos.Length; i++)
                        {
                            if (sumaDatos[i] > 0)
                            {
                                rangoValor.Add(new Tuple<string, float>(rangos[i], sumaDatos[i]));
                            }
                        }
                    }
                    else
                    {
                        foreach (Tuple<string, string, float> item2 in item.Value)
                        {
                            valuesEje.Add(item2.Item1);
                            tipos.Add(item2.Item2);
                        }
                    }
                }
            }

            bool isInt = !valuesEje.Any(x => !int.TryParse(x, out int aux));

            if (pGrafica.Config.RellenarEjeX && isInt && valuesEje.Count > 0)
            {
                int numMin = valuesEje.Min(x => int.Parse(x));
                int numMax = valuesEje.Max(x => int.Parse(x));
                for (int i = numMin; i <= numMax; i++)
                {
                    valuesEje.Add(i.ToString());
                }
            }

            if (ejeFechas || pGrafica.Config.Rango)
            {
                foreach (KeyValuePair<Dimension, List<Tuple<string, string, float>>> item in resultadosDimension)
                {
                    if (item.Value != null && item.Value.Any())
                    {
                        foreach (string valor in valuesEje)
                        {
                            Tuple<string, string, float> tuplaAux = item.Value.FirstOrDefault(x => x.Item1.Equals(valor));
                            if (tuplaAux == null)
                            {
                                item.Value.Add(new Tuple<string, string, float>(valor, "", 0));
                            }
                        }
                    }

                    if (isInt)
                    {
                        resultadosDimension[item.Key] = item.Value.OrderBy(x => int.Parse(x.Item1)).ToList();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(pGrafica.Config.EjeX))
                        {
                            resultadosDimension[item.Key] = item.Value.OrderBy(x => x.Item1).ToList();
                        }
                    }
                }

                if (isInt)
                {
                    valuesEje = new HashSet<string>(valuesEje.OrderBy(item => int.Parse(item)));
                }
                else
                {
                    if (!string.IsNullOrEmpty(pGrafica.Config.EjeX))
                    {
                        valuesEje = new HashSet<string>(valuesEje.OrderBy(item => item));
                    }
                }
            }
            #endregion

            // Obtención del objeto de la gráfica.
            List<string> listaLabels = new List<string>();
            if (pGrafica.Config.Rango)
            {
                foreach (Tuple<string, float> itemAux in rangoValor)
                {
                    listaLabels.Add(itemAux.Item1);
                }
            }
            else
            {
                listaLabels = valuesEje.ToList();
            }

            foreach (KeyValuePair<Dimension, List<Tuple<string, string, float>>> item in resultadosDimension)
            {
                DatasetBarrasY dataset = new DatasetBarrasY();
                List<float> listaData = new List<float>();
                if (pGrafica.Config.Rango)
                {
                    foreach (Tuple<string, float> itemAux in rangoValor)
                    {
                        listaData.Add(itemAux.Item2);
                    }
                }
                else
                {
                    foreach (Tuple<string, string, float> itemAux in item.Value)
                    {
                        listaData.Add(itemAux.Item3);
                    }
                }
                dataset.Data = listaData;

                // Nombre del dato en leyenda.
                dataset.Label = GetTextLang(pLang, item.Key.Nombre);

                // Color.
                dataset.BackgroundColor = ObtenerColores(dataset.Data.Count, item.Key.Color);
                dataset.Type = item.Key.TipoDimension;

                // Anchura.
                dataset.BarPercentage = 1;
                if (item.Key.Anchura != 0)
                {
                    dataset.BarPercentage = item.Key.Anchura;
                }
                // Anchura máxima.
                dataset.MaxBarThickness = 300;

                // Stack.
                if (!string.IsNullOrEmpty(item.Key.Stack))
                {
                    dataset.Stack = item.Key.Stack;
                }
                else
                {
                    dataset.Stack = Guid.NewGuid().ToString();
                }

                // Eje X.
                dataset.XAxisID = item.Key.XAxisID;

                // Orden
                dataset.Order = item.Key.Orden;

                data.Labels = listaLabels;
                data.Type = item.Key.TipoDimension;
                dimensionesDataset[item.Key] = dataset;
            }

            if (pGrafica.Config.Dimensiones.Any(x => x.DividirDatos))
            {
                foreach (Dimension dim in resultadosDimension.Keys.OrderBy(x => x.Orden))
                {
                    grafica.Data.Datasets.Add(dimensionesDataset[dim]);
                }
            }
            else
            {
                foreach (Dimension dim in pGrafica.Config.Dimensiones)
                {
                    grafica.Data.Datasets.Add(dimensionesDataset[dim]);
                }
            }
            return grafica;
        }

        /// <summary>
        /// Crea el objeto de la gráfica (Gráfica circular).
        /// </summary>
        /// <param name="pGrafica">Configuración.</param>
        /// <param name="pFiltroBase">Filtros base.</param>
        /// <param name="pFiltroFacetas">Filtros de las facetas.</param>
        /// <param name="pLang">Idioma.</param>
        /// <param name="pListaDates">Lista de valores que corresponden a fechas</param>
        /// <returns></returns>
        public static GraficaCircular CrearGraficaCircular(Grafica pGrafica, string pFiltroBase, string pFiltroFacetas, string pLang, List<string> pListaDates)
        {
            // Objeto a devolver.
            GraficaCircular grafica = new GraficaCircular();
            grafica.type = "pie"; // Por defecto, de tipo pie.

            // Abreviación.
            if (pGrafica.Config.Abreviar)
            {
                grafica.isAbr = pGrafica.Config.Abreviar;
            }

            // Porcentaje.
            if (pGrafica.Config.Porcentual)
            {
                grafica.isPercentage = pGrafica.Config.Porcentual;
            }

            // ID Grupo.
            if (!string.IsNullOrEmpty(pGrafica.IdGrupo))
            {
                grafica.groupId = pGrafica.IdGrupo;
            }

            // Asignación de Data.
            DataCircular data = new DataCircular();
            data.datasets = new ConcurrentBag<DatasetCircular>();
            grafica.data = data;

            // Asignación de Options.
            Options options = new Options();

            // Animación
            options.animation = new Animation();
            options.animation.duration = 2000;

            // Título
            options.plugins = new Plugin();
            options.plugins.title = new Title();
            options.plugins.title.display = true;
            options.plugins.title.text = GetTextLang(pLang, pGrafica.Nombre);

            grafica.options = options;

            ConcurrentDictionary<Dimension, ConcurrentDictionary<string, float>> resultadosDimension = new ConcurrentDictionary<Dimension, ConcurrentDictionary<string, float>>();
            ConcurrentDictionary<string, float> dicNombreData = new ConcurrentDictionary<string, float>();
            List<Dimension> listaDimensiones = pGrafica.Config.Dimensiones.Where(x => !x.Exterior).ToList();

            // Compruebo si es una gráfica circular de dos dimensiones.
            bool anidado = pGrafica.Config.Dimensiones.Any(x => x.Exterior);
            if (!anidado) // Gráficas con 1 dimensión
            {
                Parallel.ForEach(listaDimensiones, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, itemGrafica =>
                {
                    SparqlObject resultadoQuery = null;
                    StringBuilder select = new StringBuilder(), where = new StringBuilder();

                    // Consulta sparql.
                    List<string> filtros = new List<string>();
                    filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFiltroBase }));
                    if (!string.IsNullOrEmpty(pFiltroFacetas))
                    {
                        if (pFiltroFacetas.Contains("((("))
                        {
                            if (pFiltroFacetas.Contains('&'))
                            {
                                foreach (string filtro in pFiltroFacetas.Split('&'))
                                {
                                    if (filtro.Contains("((("))
                                    {
                                        filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { filtro.Split("(((")[0] }, pListaDates: pListaDates, pReciproco: filtro.Split("(((")[1]));
                                    }
                                    else
                                    {
                                        filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { filtro }, pListaDates: pListaDates));
                                    }
                                }
                            }
                            else
                            {
                                filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFiltroFacetas.Split("(((")[0] }, pListaDates: pListaDates, pReciproco: pFiltroFacetas.Split("(((")[1]));
                            }
                        }
                        else
                        {
                            filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFiltroFacetas }, pListaDates: pListaDates));
                        }
                    }
                    if (!string.IsNullOrEmpty(itemGrafica.Filtro))
                    {
                        filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { itemGrafica.Filtro }));
                        filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { itemGrafica.Filtro }, "tipo"));
                    }

                    select.Append(mPrefijos);
                    select.Append($@"SELECT ?tipo COUNT(DISTINCT ?s) AS ?numero ");
                    where.Append("WHERE { ");
                    foreach (string item in filtros)
                    {
                        where.Append(item);
                    }
                    string limite = itemGrafica.Limite == 0 ? "" : "LIMIT " + itemGrafica.Limite;
                    where.Append($@"FILTER(LANG(?tipo) = '{pLang}' OR LANG(?tipo) = '' OR !isLiteral(?tipo)) ");
                    where.Append($@"}} ORDER BY DESC (?numero) {limite}");

                    resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mCommunityID);
                    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                    {
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            try
                            {
                                if (!dicNombreData.TryAdd(fila["tipo"].value, Int32.Parse(fila["numero"].value)))
                                {
                                    dicNombreData.TryAdd(fila["tipo"].value + "|@" + Guid.NewGuid(), Int32.Parse(fila["numero"].value));
                                }
                            }
                            catch (Exception)
                            {
                                throw new ArgumentException("No se ha configurado el apartado de dimensiones.");
                            }
                        }
                        resultadosDimension[itemGrafica] = dicNombreData;
                    }
                });
                // Lista de los ordenes de las revistas.
                List<string> listaNombres = new List<string>();
                List<string> listaLabels = new List<string>();

                // Ordeno los datos
                Dictionary<string, float> ordered = dicNombreData.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                List<float> listaData = new List<float>();
                foreach (KeyValuePair<string, float> nombreData in ordered)
                {
                    listaNombres.Add(nombreData.Key);
                    listaData.Add(nombreData.Value);
                }

                DatasetCircular dataset = new DatasetCircular();
                dataset.data = listaData;

                List<string> listaColores = new List<string>();

                foreach (string orden in listaNombres)
                {
                    foreach (KeyValuePair<Dimension, ConcurrentDictionary<string, float>> item in resultadosDimension)
                    {
                        if (item.Key.ColorMaximo != null)
                        {
                            listaColores = ObtenerDegradadoColores(item.Key.ColorMaximo, item.Key.Color, item.Value.Count);
                        }
                        else
                        {
                            string nombreRevista = item.Key.Filtro.Contains('=') ? item.Key.Filtro.Split("=")[1].Split("@")[0].Substring(1, item.Key.Filtro.Split("=")[1].Split("@")[0].Length - 2) : "";
                            if (nombreRevista == orden)
                            {
                                // Nombre del dato en leyenda.
                                dataset.label = GetTextLang(pLang, item.Key.Nombre);
                                listaLabels.Add(GetTextLang(pLang, item.Key.Nombre));
                                // Color. 
                                listaColores.Add(item.Key.Color);
                            }
                        }
                    }
                }
                if (listaLabels.Any())
                {
                    data.labels = listaLabels;
                }
                else
                {
                    data.labels = listaNombres;
                }
                dataset.backgroundColor = listaColores;

                // HoverOffset por defecto.

                dataset.hoverOffset = 4;
                grafica.data.datasets.Add(dataset);
            }
            else // Gráficas con 2 dimensiones
            {
                List<string> filtroInterior = new List<string>();
                foreach (Dimension item in listaDimensiones)
                {
                    filtroInterior.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { item.Filtro }, "aux"));
                }
                ConcurrentDictionary<Dimension, ConcurrentDictionary<string, float>> resultadosDimensionExt = new ConcurrentDictionary<Dimension, ConcurrentDictionary<string, float>>();
                ConcurrentDictionary<string, float> dicNombreDataExt = new ConcurrentDictionary<string, float>();

                Parallel.ForEach(pGrafica.Config.Dimensiones, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, itemGrafica =>
                {
                    if (itemGrafica.Exterior)
                    {
                        SparqlObject resultadoQuery = null;
                        StringBuilder select = new StringBuilder(), where = new StringBuilder();

                        // Consulta sparql.
                        List<string> filtros = new List<string>();
                        filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFiltroBase }));
                        if (!string.IsNullOrEmpty(pFiltroFacetas))
                        {
                            if (pFiltroFacetas.Contains("((("))
                            {
                                if (pFiltroFacetas.Contains('&'))
                                {
                                    foreach (string filtro in pFiltroFacetas.Split('&'))
                                    {
                                        if (filtro.Contains("((("))
                                        {
                                            filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { filtro.Split("(((")[0] }, pListaDates: pListaDates, pReciproco: filtro.Split("(((")[1]));
                                        }
                                        else
                                        {
                                            filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { filtro }, pListaDates: pListaDates));
                                        }
                                    }
                                }
                                else
                                {
                                    filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFiltroFacetas.Split("(((")[0] }, pListaDates: pListaDates, pReciproco: pFiltroFacetas.Split("(((")[1]));
                                }
                            }
                            else
                            {
                                filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFiltroFacetas }, pListaDates: pListaDates));
                            }
                        }
                        if (!string.IsNullOrEmpty(itemGrafica.Filtro))
                        {
                            filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { itemGrafica.Filtro }));
                            filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { itemGrafica.Filtro }, "tipo"));
                            filtros.AddRange(filtroInterior);
                        }

                        select.Append(mPrefijos);
                        select.Append($@"SELECT ?tipo ?aux COUNT(DISTINCT ?s) AS ?numero ");
                        where.Append("WHERE { ");
                        foreach (string item in filtros)
                        {
                            where.Append(item);
                        }
                        string limite = itemGrafica.Limite == 0 ? "" : "LIMIT " + itemGrafica.Limite;
                        where.Append($@"FILTER(LANG(?tipo) = '{pLang}' OR LANG(?tipo) = '' OR !isLiteral(?tipo)) ");
                        where.Append($@"}} ORDER BY DESC (?numero) {limite}");
                        resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mCommunityID);
                        if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                        {
                            foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                            {
                                try
                                {
                                    dicNombreDataExt.TryAdd(fila["aux"].value + "---" + fila["tipo"].value, Int32.Parse(fila["numero"].value));
                                }
                                catch (Exception)
                                {
                                    throw new ArgumentException("No se ha configurado el apartado de dimensiones.");
                                }
                            }
                            resultadosDimensionExt[itemGrafica] = dicNombreDataExt;
                        }
                    }
                    else
                    {
                        SparqlObject resultadoQuery = null;
                        StringBuilder select = new StringBuilder(), where = new StringBuilder();

                        // Consulta sparql.
                        List<string> filtros = new List<string>();
                        filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFiltroBase }));
                        if (!string.IsNullOrEmpty(pFiltroFacetas))
                        {
                            if (pFiltroFacetas.Contains("((("))
                            {
                                if (pFiltroFacetas.Contains('&'))
                                {
                                    foreach (string filtro in pFiltroFacetas.Split('&'))
                                    {
                                        if (filtro.Contains("((("))
                                        {
                                            filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { filtro.Split("(((")[0] }, pListaDates: pListaDates, pReciproco: filtro.Split("(((")[1]));
                                        }
                                        else
                                        {
                                            filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { filtro }, pListaDates: pListaDates));
                                        }
                                    }
                                }
                                else
                                {
                                    filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFiltroFacetas.Split("(((")[0] }, pListaDates: pListaDates, pReciproco: pFiltroFacetas.Split("(((")[1]));
                                }
                            }
                            else
                            {
                                filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFiltroFacetas }, pListaDates: pListaDates));
                            }
                        }
                        if (!string.IsNullOrEmpty(itemGrafica.Filtro))
                        {
                            filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { itemGrafica.Filtro }));
                            filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { itemGrafica.Filtro }, "tipo"));
                        }

                        select.Append(mPrefijos);
                        select.Append($@"SELECT ?tipo COUNT(DISTINCT ?s) AS ?numero ");
                        where.Append("WHERE { ");
                        foreach (string item in filtros)
                        {
                            where.Append(item);
                        }
                        string limite = itemGrafica.Limite == 0 ? "" : "LIMIT " + itemGrafica.Limite;
                        where.Append($@"FILTER(LANG(?tipo) = '{pLang}' OR LANG(?tipo) = '' OR !isLiteral(?tipo)) ");
                        where.Append($@"}} ORDER BY DESC (?numero) {limite}");

                        resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mCommunityID);
                        if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                        {
                            foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                            {
                                try
                                {
                                    dicNombreData.TryAdd(fila["tipo"].value, Int32.Parse(fila["numero"].value));
                                }
                                catch (Exception)
                                {
                                    throw new ArgumentException("No se ha configurado el apartado de dimensiones.");
                                }
                            }
                            resultadosDimension[itemGrafica] = dicNombreData;
                        }
                    }
                });
                // Lista de los ordenes de las revistas.
                List<string> listaNombres = new List<string>();
                List<string> listaLabels = new List<string>();
                // Ordeno los datos
                Dictionary<string, float> ordered = dicNombreData.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
                List<float> listaData = new List<float>();
                foreach (KeyValuePair<string, float> nombreData in ordered)
                {
                    listaNombres.Add(nombreData.Key);
                    listaData.Add(nombreData.Value);
                }

                DatasetCircular dataset = new DatasetCircular();
                dataset.data = listaData;

                List<string> listaColores = new List<string>();

                foreach (string orden in listaNombres)
                {
                    foreach (KeyValuePair<Dimension, ConcurrentDictionary<string, float>> item in resultadosDimension)
                    {
                        if (item.Key.ColorMaximo != null)
                        {
                            listaColores = ObtenerDegradadoColores(item.Key.ColorMaximo, item.Key.Color, item.Value.Count);
                        }
                        else
                        {
                            string nombreRevista = item.Key.Filtro.Contains('=') ? item.Key.Filtro.Split("=")[1].Split("@")[0].Substring(1, item.Key.Filtro.Split("=")[1].Split("@")[0].Length - 2) : "";
                            if (nombreRevista == orden)
                            {
                                // Nombre del dato en leyenda.
                                dataset.label = GetTextLang(pLang, item.Key.Nombre);
                                listaLabels.Add(GetTextLang(pLang, item.Key.Nombre));
                                // Color. 
                                listaColores.Add(item.Key.Color);
                            }
                        }
                    }
                }
                if (listaLabels.Any())
                {
                    data.labels = listaLabels;
                }
                else
                {
                    data.labels = listaNombres;
                }
                dataset.backgroundColor = listaColores;

                // HoverOffset por defecto.
                dataset.hoverOffset = 4;
                dataset.label = string.Join('|', data.labels);

                // Lista de los ordenes de las revistas.
                List<string> listaNombresExt = new List<string>();
                List<string> listaLabelsExt = new List<string>();
                // Ordeno los datos
                Dictionary<string, float> parteIzq = new Dictionary<string, float>();
                Dictionary<string, float> parteDcha = new Dictionary<string, float>();

                int cont = 0;
                foreach (KeyValuePair<string, float> nombreData in dicNombreDataExt.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value))
                {
                    if (cont >= dicNombreDataExt.Count / 2)
                    {
                        if (parteIzq.Keys.Any(x => x.StartsWith(nombreData.Key.Split("---")[0])))
                        {
                            parteIzq.Add(nombreData.Key, nombreData.Value);
                        }
                        else
                        {
                            parteDcha.Add(nombreData.Key, nombreData.Value);
                        }
                    }
                    else
                    {
                        parteIzq.Add(nombreData.Key, nombreData.Value);
                    }
                    cont++;
                }

                Dictionary<string, float> orderedExt = parteIzq.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
                foreach (KeyValuePair<string, float> item in parteDcha.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value))
                {
                    orderedExt.TryAdd(item.Key, item.Value);
                }
                List<float> listaDataExt = new List<float>();
                foreach (KeyValuePair<string, float> nombreData in orderedExt)
                {
                    listaNombresExt.Add(nombreData.Key.Split("---").LastOrDefault());
                    listaDataExt.Add(nombreData.Value);
                }

                DatasetCircular datasetExt = new DatasetCircular();
                datasetExt.data = listaDataExt;

                List<string> listaColoresExt = new List<string>();
                List<int> listaGrupos = new List<int>();
                string auxLeyenda = "~~~";
                foreach (string orden in listaNombresExt)
                {
                    foreach (KeyValuePair<Dimension, ConcurrentDictionary<string, float>> item in resultadosDimensionExt)
                    {
                        if (item.Key.ColorMaximo != null)
                        {
                            listaColoresExt = ObtenerDegradadoColores(item.Key.ColorMaximo, item.Key.Color, item.Value.Count);
                        }
                        else
                        {
                            string nombreRevista = item.Key.Filtro.Contains('=') ? item.Key.Filtro.Split("=")[1].Split("@")[0].Substring(1, item.Key.Filtro.Split("=")[1].Split("@")[0].Length - 2) : "";
                            if (nombreRevista == orden)
                            {
                                // Nombre del dato en leyenda.
                                listaLabelsExt.Add(GetTextLang(pLang, item.Key.Nombre));
                                // Obtengo los colores
                                listaColoresExt.Add(item.Key.Color);
                                if (!auxLeyenda.Contains(GetTextLang(pLang, item.Key.Nombre)))
                                {
                                    auxLeyenda += $"{GetTextLang(pLang, item.Key.Nombre)}|{item.Key.Color}---";
                                }
                            }
                        }
                    }
                }
                cont = 0;
                for (int i = 0; i < listaLabelsExt.Count; i++)
                {
                    if (i > parteIzq.Count / 2 && cont < listaLabels.Count - 1)
                    {
                        cont++;
                    }
                    listaLabelsExt[i] += $" {listaLabels[cont].ToLower()}";
                    // Mezclo los colores
                    int rInterior = Convert.ToInt32(listaColores[cont].Substring(1, 2), 16);
                    int gInterior = Convert.ToInt32(listaColores[cont].Substring(3, 2), 16);
                    int bInterior = Convert.ToInt32(listaColores[cont].Substring(5, 2), 16);
                    int rExterior = Convert.ToInt32(listaColoresExt[i].Substring(1, 2), 16);
                    int gExterior = Convert.ToInt32(listaColoresExt[i].Substring(3, 2), 16);
                    int bExterior = Convert.ToInt32(listaColoresExt[i].Substring(5, 2), 16);
                    int rMezclado = rExterior + (int)((rInterior - rExterior) * 0.25);
                    int gMezclado = gExterior + (int)((gInterior - gExterior) * 0.25);
                    int bMezclado = bExterior + (int)((bInterior - bExterior) * 0.25);
                    string colorHex = '#' + rMezclado.ToString("X2") + gMezclado.ToString("X2") + bMezclado.ToString("X2");
                    listaColoresExt[i] = colorHex;
                    listaGrupos.Add(cont);
                }
                datasetExt.grupos = listaGrupos;
                datasetExt.backgroundColor = listaColoresExt;
                datasetExt.label = string.Join('|', listaLabelsExt);
                // HoverOffset por defecto.
                datasetExt.hoverOffset = 4;

                dataset.label += auxLeyenda.Remove(auxLeyenda.Length - 3);
                grafica.data.datasets.Add(dataset);
                grafica.data.datasets.Add(datasetExt);
            }
            return grafica;
        }

        /// <summary>
        /// Crea el objeto de la gráfica (Gráfica nodos).
        /// </summary>
        /// <param name="pGrafica">Configuración.</param>
        /// <param name="pFiltroBase">Filtros base.</param>
        /// <param name="pFiltroFacetas">Filtros de las facetas.</param>
        /// <param name="pLang">Idioma.</param>
        /// <param name="pListaDates">Lista de valores que corresponden a fechas</param>
        /// <returns></returns>
        public static GraficaNodos CrearGraficaNodos(Grafica pGrafica, string pFiltroBase, string pFiltroFacetas, string pLang, List<string> pListaDates)
        {
            GraficaNodos grafica = new GraficaNodos();

            // Abreviación.
            if (pGrafica.Config.Abreviar)
            {
                grafica.isAbr = pGrafica.Config.Abreviar;
            }

            // Porcentaje.
            if (pGrafica.Config.Porcentual)
            {
                grafica.isPercentage = pGrafica.Config.Porcentual;
            }

            // ID Grupo.
            if (!string.IsNullOrEmpty(pGrafica.IdGrupo))
            {
                grafica.groupId = pGrafica.IdGrupo;
            }

            // Tipo.
            grafica.isNodes = true;

            #region --- Configuración
            // Opciones interactivas
            grafica.userZoomingEnabled = false;
            grafica.zoomingEnabled = true;
            grafica.minZoom = 0.5f;
            grafica.maxZoom = 4.0f;

            // Layout Base
            grafica.layout = new Layout();
            grafica.layout.name = "cose";
            grafica.layout.idealEdgeLength = 100;
            grafica.layout.nodeOverlap = 20;
            grafica.layout.refresh = 20;
            grafica.layout.fit = true;
            grafica.layout.padding = 30;
            grafica.layout.randomize = false;
            grafica.layout.componentSpacing = 50;
            grafica.layout.nodeRepulsion = 400000;
            grafica.layout.edgeElasticity = 100;
            grafica.layout.nestingFactor = 5;
            grafica.layout.gravity = 80;
            grafica.layout.numIter = 1000;
            grafica.layout.initialTemp = 200;
            grafica.layout.coolingFactor = 0.95f;
            grafica.layout.minTemp = 1.0f;

            // Titulo
            Dimension dimension = pGrafica.Config.Dimensiones.FirstOrDefault();
            if (dimension == null)
            {
                dimension = new Dimension();
            }
            grafica.title = dimension.Nombre.Values.FirstOrDefault();
            // Layout Nodos/Lineas
            grafica.style = new List<Style>();
            Style estiloNodo = new Style();

            // Nodos
            LayoutStyle layoutNodos = new LayoutStyle();
            layoutNodos.Width = "mapData(score, 0, 80, 10, 90)";
            layoutNodos.Height = "mapData(score, 0, 80, 10, 90)";
            layoutNodos.Content = "data(name)";
            layoutNodos.FontSize = "12px";
            layoutNodos.FontFamily = "Roboto";
            layoutNodos.BackgroundColor = dimension.ColorNodo;
            layoutNodos.TextOutlineWidth = "0px";
            layoutNodos.OverlayPadding = "6px";
            layoutNodos.LineColor = "";
            layoutNodos.ZIndex = "10";

            estiloNodo.Selector = "node";
            estiloNodo.style = layoutNodos;
            grafica.style.Add(estiloNodo);

            // Líneas
            LayoutStyle layoutLineas = new LayoutStyle();
            Style estiloLinea = new Style();
            estiloLinea.Selector = "edge";
            layoutLineas.CurveStyle = "haystack";
            layoutLineas.Content = "data(name)";
            layoutLineas.FontSize = "24px";
            layoutLineas.FontFamily = "Roboto";
            layoutLineas.BackgroundColor = "#6cafd3";
            layoutLineas.HaystackRadius = "0.5";
            layoutLineas.Opacity = "0.5";
            layoutLineas.Width = "mapData(weight, 0, 10, 0, 10)";
            layoutLineas.OverlayPadding = "1px";
            layoutLineas.ZIndex = "11";
            layoutLineas.LineColor = dimension.ColorLinea;
            estiloLinea.style = layoutLineas;
            grafica.style.Add(estiloLinea);
            #endregion

            //Nodos            
            Dictionary<string, string> dicNodos = new Dictionary<string, string>();

            //Relaciones
            Dictionary<string, List<DataQueryRelaciones>> dicRelaciones = new Dictionary<string, List<DataQueryRelaciones>>();

            //Respuesta
            List<DataItemRelacion> itemsRelacion = new List<DataItemRelacion>();

            Dictionary<string, List<string>> dicResultadosAreaRelacionAreas = new Dictionary<string, List<string>>();
            Dictionary<string, int> scoreNodes = new Dictionary<string, int>();

            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            Parallel.ForEach(pGrafica.Config.Dimensiones, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, itemGrafica =>
            {
                // Consulta sparql.
                List<string> filtros = new List<string>();
                filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFiltroBase }));
                if (!string.IsNullOrEmpty(pFiltroFacetas))
                {
                    if (pFiltroFacetas.Contains("((("))
                    {
                        if (pFiltroFacetas.Contains('&'))
                        {
                            foreach (string filtro in pFiltroFacetas.Split('&'))
                            {
                                if (filtro.Contains("((("))
                                {
                                    filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { filtro.Split("(((")[0] }, pListaDates: pListaDates, pReciproco: filtro.Split("(((")[1]));
                                }
                                else
                                {
                                    filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { filtro }, pListaDates: pListaDates));
                                }
                            }
                        }
                        else
                        {
                            filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFiltroFacetas.Split("(((")[0] }, pListaDates: pListaDates, pReciproco: pFiltroFacetas.Split("(((")[1]));
                        }
                    }
                    else
                    {
                        filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFiltroFacetas }, pListaDates: pListaDates));
                    }
                }
                if (!string.IsNullOrEmpty(itemGrafica.Filtro))
                {
                    filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { itemGrafica.Filtro }));
                }

                // Consulta sparql.
                select.Append(mPrefijos);
                select.Append("SELECT ?s group_concat(?categoria;separator=\",\") AS ?idCategorias ");
                where.Append("WHERE { ");
                foreach (string item in filtros)
                {
                    where.Append(item);
                }
                where.Append($@"?s {pGrafica.PropCategoryPath} ?area. ");
                where.Append("?area roh:categoryNode ?categoria. ");
                where.Append("MINUS { ?categoria skos:narrower ?hijos } ");
                where.Append("} ");

                resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mCommunityID);
                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                {
                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                    {
                        string idCategorias = fila["idCategorias"].value;
                        HashSet<string> categorias = new HashSet<string>(idCategorias.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));

                        foreach (string categoria in categorias)
                        {
                            if (!scoreNodes.ContainsKey(categoria))
                            {
                                scoreNodes.Add(categoria, 0);
                            }

                            scoreNodes[categoria]++;

                            if (!dicResultadosAreaRelacionAreas.ContainsKey(categoria))
                            {
                                dicResultadosAreaRelacionAreas.Add(categoria, new List<string>());
                            }

                            dicResultadosAreaRelacionAreas[categoria].AddRange(categorias.Except(new List<string>() { categoria }));
                        }
                    }
                }

                ProcesarRelaciones("Category", dicResultadosAreaRelacionAreas, ref dicRelaciones);

                int maximasRelaciones = 0;
                foreach (KeyValuePair<string, List<DataQueryRelaciones>> sujeto in dicRelaciones)
                {
                    foreach (DataQueryRelaciones relaciones in sujeto.Value)
                    {
                        foreach (Datos relaciones2 in relaciones.idRelacionados)
                        {
                            maximasRelaciones = Math.Max(maximasRelaciones, relaciones2.numVeces);
                        }
                    }
                }

                // Creamos los nodos y las relaciones en función de pNumAreas.
                int pNumAreas = dimension.NumMaxNodos;

                Dictionary<string, int> numRelaciones = new Dictionary<string, int>();
                foreach (KeyValuePair<string, List<DataQueryRelaciones>> sujeto in dicRelaciones)
                {
                    if (!numRelaciones.ContainsKey(sujeto.Key))
                    {
                        numRelaciones.Add(sujeto.Key, 0);
                    }
                    foreach (DataQueryRelaciones relaciones in sujeto.Value)
                    {
                        foreach (Datos relaciones2 in relaciones.idRelacionados)
                        {
                            if (!numRelaciones.ContainsKey(relaciones2.idRelacionado))
                            {
                                numRelaciones.Add(relaciones2.idRelacionado, 0);
                            }
                            numRelaciones[sujeto.Key] += relaciones2.numVeces;
                            numRelaciones[relaciones2.idRelacionado] += relaciones2.numVeces;
                        }
                    }
                }

                List<string> itemsSeleccionados = numRelaciones.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value).Keys.Distinct().ToList();
                if (itemsSeleccionados.Count > pNumAreas)
                {
                    itemsSeleccionados = itemsSeleccionados.GetRange(0, pNumAreas);
                }

                if (itemsSeleccionados.Count > 0)
                {
                    // Recuperamos los nombres de categorías y creamos los nodos.
                    select = new StringBuilder();
                    where = new StringBuilder();

                    select.Append(mPrefijos);
                    select.Append("SELECT ?categoria ?nombreCategoria ");
                    where.Append("WHERE { ");
                    where.Append("?categoria skos:prefLabel ?nombreCategoria. ");
                    where.Append($@"FILTER(?categoria IN (<{string.Join(">,<", itemsSeleccionados)}>)) ");
                    where.Append("} ");

                    resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mCommunityID);
                    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                    {
                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            if (!dicNodos.ContainsKey(fila["categoria"].value))
                            {
                                dicNodos.Add(fila["categoria"].value, fila["nombreCategoria"].value);
                            }
                        }
                    }

                    // Nodos. 
                    if (dicNodos != null && dicNodos.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> nodo in dicNodos)
                        {
                            string clave = nodo.Key;
                            Data data = new Data(clave, nodo.Value, null, null, null, "nodes", Data.Type.icon_area);
                            if (scoreNodes.ContainsKey(clave))
                            {
                                data.score = scoreNodes[clave];
                                data.name = $"{data.name} ({data.score})";
                            }
                            DataItemRelacion dataColabo = new DataItemRelacion(data, true, true);
                            itemsRelacion.Add(dataColabo);
                        }
                    }

                    // Relaciones.
                    if (dicRelaciones != null && dicRelaciones.Count > 0)
                    {
                        foreach (KeyValuePair<string, List<DataQueryRelaciones>> sujeto in dicRelaciones)
                        {
                            if (itemsSeleccionados.Contains(sujeto.Key))
                            {
                                foreach (DataQueryRelaciones relaciones in sujeto.Value)
                                {
                                    foreach (Datos relaciones2 in relaciones.idRelacionados)
                                    {
                                        if (itemsSeleccionados.Contains(relaciones2.idRelacionado))
                                        {
                                            string id = $@"{sujeto.Key}~{relaciones.nombreRelacion}~{relaciones2.idRelacionado}~{relaciones2.numVeces}";
                                            Data data = new Data(id, relaciones.nombreRelacion, sujeto.Key, relaciones2.idRelacionado, CalcularGrosor(maximasRelaciones, relaciones2.numVeces), "edges", Data.Type.relation_document);
                                            DataItemRelacion dataColabo = new DataItemRelacion(data, null, null);
                                            itemsRelacion.Add(dataColabo);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });

            grafica.elements = itemsRelacion;
            return grafica;
        }
        #endregion

        #region --- Facetas
        /// <summary>
        /// Lee la configuración y obtiene los datos necesarios para el servicio de facetas.
        /// </summary>
        /// <param name="pIdPagina">Identificador de la página.</param>
        /// <param name="pIdFaceta">Identificador de la faceta.</param>
        /// <param name="pFiltroFacetas">Filtros de la URL.</param>
        /// <param name="pLang">Idioma.</param>
        /// <param name="pGetAll">Indica si obtener todas las facetas</param>
        /// <returns></returns>
        public static Faceta GetFaceta(string pIdPagina, string pIdFaceta, string pFiltroFacetas, string pLang, bool pGetAll = false)
        {
            // Decode de los filtros. Cambio los '+' para decodificar correctamente
            if (!string.IsNullOrEmpty(pFiltroFacetas))
            {
                pFiltroFacetas = HttpUtility.UrlDecode(pFiltroFacetas.Replace("+", "simbolomasdecodificar"));
                pFiltroFacetas = pFiltroFacetas.Replace("simbolomasdecodificar", "+");
            }

            // Lectura del JSON de configuración.
            ConfigModel configModel = TabTemplates.FirstOrDefault(x => x.Identificador == pIdPagina);
            if (configModel == null)
            {
                return null;
            }
            // Obtiene los filtros relacionados con las fechas.
            List<string> listaFacetasAnios = configModel.Facetas.Where(x => x.RangoAnio).Select(x => x.Filtro).ToList();
            FacetaConf faceta = configModel.Facetas.FirstOrDefault(x => x.Filtro == pIdFaceta);
            return CrearFaceta(faceta, configModel.Filtro, pFiltroFacetas, pLang, listaFacetasAnios, pGetAll);
        }

        /// <summary>
        /// Crea el objeto faceta.
        /// </summary>
        /// <param name="pFacetaConf">Configuración.</param>
        /// <param name="pFiltroBase">Filtros base.</param>
        /// <param name="pFiltroFacetas">Filtros de las facetas.</param>
        /// <param name="pLang">Idioma.</param>
        /// <param name="pGetAll">Indica si obtener todas las facetas</param>
        /// <param name="pListaDates">Lista de valores que corresponden a fechas</param>
        /// <returns></returns>
        public static Faceta CrearFaceta(FacetaConf pFacetaConf, string pFiltroBase, string pFiltroFacetas, string pLang, List<string> pListaDates, bool pGetAll = false)
        {
            Faceta faceta = new (pFacetaConf.Filtro, pFacetaConf.RangoAnio, GetTextLang(pLang, pFacetaConf.Nombre), pFacetaConf.OrdenAlfaNum, pFacetaConf.VerTodos, pFacetaConf.Tesauro,pFacetaConf.Reciproca);

            if (pFacetaConf.NumeroItemsFaceta != 0 && !pGetAll)
            {
                faceta.numeroItemsFaceta = pFacetaConf.NumeroItemsFaceta;
            }

            // Filtro de página.
            List<string> filtros = new List<string>();
            filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFiltroBase }));
            if (!string.IsNullOrEmpty(pFacetaConf.Reciproca))
            {
                filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFacetaConf.Filtro }, "nombreFaceta", null, pFacetaConf.Reciproca));
            }
            else if (!faceta.tesauro)
            {
                filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFacetaConf.Filtro }, "nombreFaceta"));
            }
            else
            {
                filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFacetaConf.Filtro }, "categoria"));
            }
            if (!string.IsNullOrEmpty(pFiltroFacetas))
            {
                if (pFiltroFacetas.Contains("((("))
                {
                    if (pFiltroFacetas.Contains('&'))
                    {
                        foreach (string filtro in pFiltroFacetas.Split('&'))
                        {
                            if (filtro.Contains("((("))
                            {
                                filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { filtro.Split("(((")[0] }, pListaDates: pListaDates, pReciproco: filtro.Split("(((")[1]));
                            }
                            else
                            {
                                filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { filtro }, pListaDates: pListaDates));
                            }
                        }
                    }
                    else
                    {
                        filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFiltroFacetas.Split("(((")[0] }, pListaDates: pListaDates, pReciproco: pFiltroFacetas.Split("(((")[1]));
                    }
                }
                else
                {
                    filtros.AddRange(UtilsGraficas.ObtenerFiltros(new List<string>() { pFiltroFacetas }, pListaDates: pListaDates));
                }
            }
            SparqlObject resultadoQuery = null;
            StringBuilder select = new StringBuilder(), where = new StringBuilder();

            if (!faceta.tesauro)
            {
                select.Append(mPrefijos);
                select.Append($@"SELECT DISTINCT ?nombreFaceta LANG(?nombreFaceta) AS ?lang COUNT(DISTINCT ?s) AS ?numero ");
                where.Append("WHERE { ");
                foreach (string item in filtros)
                {
                    where.Append(item);
                }
                where.Append($@"FILTER(LANG(?nombreFaceta) = '{pLang}' OR LANG(?nombreFaceta) = '' OR !isLiteral(?nombreFaceta)) ");
                if (faceta.ordenAlfaNum)
                {
                    where.Append($@"}} ORDER BY ASC (?nombreFaceta) ");
                }
                else
                {
                    where.Append($@"}} ORDER BY DESC (?numero) ");
                }

                where.Append($@"LIMIT {faceta.numeroItemsFaceta} ");

                resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mCommunityID);
                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                {
                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                    {
                        ItemFaceta itemFaceta = new ItemFaceta();
                        itemFaceta.nombre = fila["nombreFaceta"].value;
                        itemFaceta.numero = int.Parse(fila["numero"].value);

                        // Comprobación si tiene idioma asignado.
                        string lang = "";
                        if (!string.IsNullOrEmpty(fila["lang"].value))
                        {
                            lang = $@"@{fila["lang"].value}";
                        }

                        // Comprobación si es literal o numerico.
                        string filtro = itemFaceta.nombre;
                        if (fila["nombreFaceta"].type == "literal")
                        {
                            filtro = $@"'{filtro}'";
                        }

                        itemFaceta.filtro = $@"{pFacetaConf.Filtro}={filtro}{lang}";
                        faceta.items.Add(itemFaceta);
                    }
                }
            }
            else
            {
                select.Append(mPrefijos);
                select.Append($@"SELECT ?categoria ?nombre COUNT(DISTINCT (?s)) AS ?numero ");
                where.Append("WHERE { ");
                foreach (string item in filtros)
                {
                    where.Append(item);
                }
                where.Append("?categoria skos:prefLabel ?nombre. ");
                where.Append($@"}} ORDER BY ASC (?categoria) ");

                resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mCommunityID);
                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                {
                    Dictionary<ItemFaceta, int> itemsFaceta = new Dictionary<ItemFaceta, int>();
                    int maxNivel = 0;
                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                    {
                        ItemFaceta itemFaceta = new ItemFaceta();
                        itemFaceta.idTesauro = fila["categoria"].value.Substring(fila["categoria"].value.LastIndexOf("_") + 1);
                        itemFaceta.nombre = fila["nombre"].value;
                        itemFaceta.numero = Int32.Parse(fila["numero"].value);
                        itemFaceta.filtro = $@"{pFacetaConf.Filtro}={fila["categoria"].value}";
                        itemFaceta.childsTesauro = new List<ItemFaceta>();
                        // Asigno el nivel del item.
                        int nivel = 0;
                        for (int i = 0; i < itemFaceta.idTesauro.Length; i++)
                        {
                            if (itemFaceta.idTesauro[i] == '0' && (i == 0 || itemFaceta.idTesauro[i - 1] == '.'))
                            {
                                nivel++;
                            }
                        }
                        maxNivel = Math.Max(nivel, maxNivel);
                        itemsFaceta.Add(itemFaceta, nivel);
                    }
                    foreach (KeyValuePair<ItemFaceta, int> item in itemsFaceta)
                    {
                        if (item.Value != 0)
                        {
                            getHijosTesauro(item.Key, item.Value, itemsFaceta);
                        }
                    }
                    faceta.items.AddRange(itemsFaceta.Where(x => x.Value == maxNivel).Select(x => x.Key));
                }
            }

            return faceta;
        }

        private static void getHijosTesauro(ItemFaceta item, int nivel, Dictionary<ItemFaceta, int> itemsFaceta)
        {
            string prefijo = item.idTesauro.Substring(0, item.idTesauro.Length - 2 * nivel + 1);
            item.childsTesauro.AddRange(itemsFaceta.Where(x => x.Value == nivel - 1 && x.Key.idTesauro.StartsWith(prefijo)).Select(x => x.Key));
        }

        #endregion

        #region --- Utils
        /// <summary>
        /// Permite agregar un triple a un recurso.
        /// </summary>
        /// <param name="pResourceApi">API.</param>
        /// <param name="pRecursoID">Id del recurso al que se quiere agregar el triple.</param>
        /// <param name="pTriples">Triple a agregar.</param>
        /// <returns></returns>
        public static bool IncluirTriplesRecurso(ResourceApi pResourceApi, Guid pRecursoID, List<TriplesToInclude> pTriples)
        {
            List<TriplesToInclude> triplesIncluir = new List<TriplesToInclude>();

            foreach (TriplesToInclude triple in pTriples)
            {
                if (triple.NewValue == string.Empty)
                {
                    triple.NewValue = null;
                }
                triplesIncluir.Add(triple);
            }

            Dictionary<Guid, List<TriplesToInclude>> dicTriplesInsertar = new Dictionary<Guid, List<TriplesToInclude>>();
            dicTriplesInsertar.Add(pRecursoID, triplesIncluir);
            Dictionary<Guid, bool> dicInsertado = pResourceApi.InsertPropertiesLoadedResources(dicTriplesInsertar);
            return (dicInsertado != null && dicInsertado.ContainsKey(pRecursoID) && dicInsertado[pRecursoID]);
        }

        /// <summary>
        /// Permite obtener el ID del recurso de la persona mediante el ID del usuario.
        /// </summary>
        /// <param name="pUserId">ID del usuario.</param>
        /// <returns></returns>
        public static string GetIdPersonByGnossUser(string pUserId)
        {
            // ID de la persona.
            string idRecurso = string.Empty;

            // Filtro de página.
            string select = mPrefijos;
            select += "SELECT ?s ";
            string where = $@"WHERE {{ 
                                ?s roh:gnossUser <http://gnoss/{pUserId.ToUpper()}> . 
                            }} ";

            SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mCommunityID);
            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    idRecurso = fila["s"].value;
                }
            }

            return idRecurso;
        }

        /// <summary>
        /// devuelve la propiedad IsGraphicManager del usuario con id pUserId.
        /// </summary>
        /// <param name="pUserId"></param>
        /// <returns></returns>
        public static bool IsGraphicManager(string pUserId)
        {
            // ID de la persona.
            bool idRecurso = false;

            string select = mPrefijos;
            select += "SELECT ?permisos ";
            string where = $@"WHERE {{ 
                                ?s roh:gnossUser <http://gnoss/{pUserId.ToUpper()}> . 
                                ?s roh:isGraphicManager ?permisos .
                            }} ";

            SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mCommunityID);
            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    idRecurso = bool.Parse(fila["permisos"].value);
                }
            }

            return idRecurso;
        }

        /// <summary>
        /// Obtiene los datos de las gráficas guardadas del usuario.
        /// </summary>
        /// <param name="pIdPage">ID de la página.</param>
        /// <returns>Lista de datos de las gráficas.</returns>
        public static List<DataGraphicUser> GetGraficasUserByPageId(string pIdPage)
        {
            // Lista de datos de las gráficas.
            List<DataGraphicUser> listaGraficas = new List<DataGraphicUser>();

            string select = mPrefijos;
            select += "SELECT distinct ?datosGraficas ?titulo ?orden ?idPagina ?idGrafica ?filtro ?anchura ?escalas";
            string where = $@"WHERE {{ 
                                <{pIdPage}> roh:metricGraphic ?datosGraficas. 
                                ?datosGraficas roh:title ?titulo. 
                                ?datosGraficas roh:order ?orden. 
                                ?datosGraficas roh:pageId ?idPagina. 
                                ?datosGraficas roh:graphicId ?idGrafica.
                                OPTIONAL{{?datosGraficas roh:filters ?filtro. }} 
                                ?datosGraficas roh:width ?anchura. 
                                OPTIONAL{{?datosGraficas roh:scales ?escalas. }} 
                            }} ";

            SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, mCommunityID);
            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    DataGraphicUser data = new();
                    data.idRecurso = fila["datosGraficas"].value;
                    data.titulo = fila["titulo"].value;
                    data.orden = int.Parse(fila["orden"].value);
                    data.idPagina = fila["idPagina"].value;
                    data.idGrafica = fila["idGrafica"].value;
                    if (fila.ContainsKey("filtro") && !string.IsNullOrEmpty(fila["filtro"].value))
                    {
                        data.filtro = fila["filtro"].value;
                    }
                    data.anchura = fila["anchura"].value;
                    data.escalas = fila.ContainsKey("escalas") ? fila["escalas"].value : string.Empty;
                    listaGraficas.Add(data);
                }
            }

            return listaGraficas.OrderBy(x => x.orden).ToList();
        }

        /// <summary>
        /// Obtiene el listado de páginas asociadas a un usuario.
        /// </summary>
        /// <param name="pUserId">ID del usuario.</param>
        /// <returns></returns>
        public static List<DataPageUser> GetPagesUser(string pUserId)
        {
            // ID del recurso del usuario.
            string idRecurso = GetIdPersonByGnossUser(pUserId);

            // Lista de datos de las páginas.
            List<DataPageUser> listaPaginas = new List<DataPageUser>();

            string select = mPrefijos;
            select += "SELECT ?datosPagina ?titulo ?orden ";
            string where = $@"WHERE {{ 
                                <{idRecurso}> roh:metricPage ?datosPagina. 
                                ?datosPagina roh:title ?titulo. 
                                ?datosPagina roh:order ?orden. 
                            }} ";

            SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select.ToString(), where.ToString(), mCommunityID);
            if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
            {
                foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                {
                    DataPageUser data = new DataPageUser();
                    data.idRecurso = fila["datosPagina"].value;
                    data.titulo = fila["titulo"].value;
                    data.orden = Int32.Parse(fila["orden"].value);
                    listaPaginas.Add(data);
                }
            }

            return listaPaginas.OrderBy(x => x.orden).ToList();
        }

        /// <summary>
        /// Guarda a la persona la gráfica que quiere quedarse en su administración.
        /// </summary>
        /// <param name="pTitulo">Título de la gráfica a guardar.</param>
        /// <param name="pAnchura">Anchura de la gráfica a guardar.</param>
        /// <param name="pIdRecursoPagina">ID del recurso de la página.</param>
        /// <param name="pIdPaginaGrafica">ID de la página de la gráfica.<</param>
        /// <param name="pIdGrafica">ID de la gráfica.</param>
        /// <param name="pFiltros">Filtros a aplicar en la gráfica.</param>
        /// <param name="pUserId">ID del usuario conectado.</param>
        /// <param name="pEscalas">Escalas de la gráfica</param>
        /// <param name="pTituloPagina">Título de la página nueva</param>
        public static bool GuardarGrafica(string pTitulo, string pAnchura, string pIdPaginaGrafica, string pIdGrafica, string pFiltros, string pUserId, string pIdRecursoPagina = null, string pTituloPagina = null, string pEscalas = null)
        {
            string idRecursoPagina = pIdRecursoPagina;
            if (string.IsNullOrEmpty(idRecursoPagina) && !string.IsNullOrEmpty(pTituloPagina))
            {
                idRecursoPagina = CrearPaginaUsuario(pUserId, pTituloPagina);
            }

            // ID del recurso del usuario.
            string idRecurso = GetIdPersonByGnossUser(pUserId);

            mResourceApi.ChangeOntoly("person");

            Guid shortId = mResourceApi.GetShortGuid(idRecurso);
            Guid entidadGuid = Guid.NewGuid();
            List<TriplesToInclude> triplesInclude = new List<TriplesToInclude>();
            string predicadoBase = "http://w3id.org/roh/metricPage|http://w3id.org/roh/metricGraphic|";
            string idRecursoGrafica = $@"{mResourceApi.GraphsUrl}items/MetricGraphic_{shortId}_{entidadGuid}";
            string valorEntidadAuxiliar = $@"{idRecursoPagina}|{idRecursoGrafica}";
            string valorBase = $@"{valorEntidadAuxiliar}|";

            // Título de la página
            triplesInclude.Add(new TriplesToInclude
            {
                Description = false,
                Title = false,
                Predicate = predicadoBase + "http://w3id.org/roh/title",
                NewValue = valorBase + pTitulo
            });

            // Orden de la gráfica
            int orden = 0;
            List<DataGraphicUser> listaGraficas = GetGraficasUserByPageId(idRecursoPagina);
            foreach (DataGraphicUser item in listaGraficas)
            {
                if (item.orden > orden)
                {
                    orden = item.orden;
                }
            }
            orden++;

            triplesInclude.Add(new TriplesToInclude
            {
                Description = false,
                Title = false,
                Predicate = predicadoBase + "http://w3id.org/roh/order",
                NewValue = valorBase + orden
            });

            // ID de la página de la gráfica
            triplesInclude.Add(new TriplesToInclude
            {
                Description = false,
                Title = false,
                Predicate = predicadoBase + "http://w3id.org/roh/pageId",
                NewValue = valorBase + pIdPaginaGrafica
            });

            // ID de la gráfica
            triplesInclude.Add(new TriplesToInclude
            {
                Description = false,
                Title = false,
                Predicate = predicadoBase + "http://w3id.org/roh/graphicId",
                NewValue = valorBase + pIdGrafica
            });

            // Filtros
            if (!string.IsNullOrEmpty(pFiltros))
            {
                // --- ÑAPA
                if (System.Text.RegularExpressions.Regex.Match(pFiltros, "@[\\w]{2}$").Success)
                {
                    pFiltros = pFiltros + "_filter";
                }

                triplesInclude.Add(new TriplesToInclude
                {
                    Description = false,
                    Title = false,
                    Predicate = predicadoBase + "http://w3id.org/roh/filters",
                    NewValue = valorBase + pFiltros
                });
            }

            // Anchura
            triplesInclude.Add(new TriplesToInclude
            {
                Description = false,
                Title = false,
                Predicate = predicadoBase + "http://w3id.org/roh/width",
                NewValue = valorBase + pAnchura
            });

            // Escalas
            if (pEscalas != null)
            {
                triplesInclude.Add(new TriplesToInclude
                {
                    Description = false,
                    Title = false,
                    Predicate = predicadoBase + "http://w3id.org/roh/scales",
                    NewValue = valorBase + pEscalas
                });
            }
            bool insertado = IncluirTriplesRecurso(mResourceApi, shortId, triplesInclude);
            return insertado;
        }

        /// <summary>
        /// Crea una página de indicadores personales.
        /// </summary>
        /// <param name="pUserId">ID del usuario.</param>
        /// <param name="pTitulo">Título de la página</param>
        public static string CrearPaginaUsuario(string pUserId, string pTitulo)
        {
            mResourceApi.ChangeOntoly("person");

            // ID del recurso del usuario.
            string idRecurso = GetIdPersonByGnossUser(pUserId);

            Guid shortId = mResourceApi.GetShortGuid(idRecurso);
            Guid entidadGuid = Guid.NewGuid();
            List<TriplesToInclude> triplesInclude = new List<TriplesToInclude>();
            string predicadoBase = "http://w3id.org/roh/metricPage|";
            string valorEntidadAuxiliar = $@"{mResourceApi.GraphsUrl}items/MetricPage_{shortId}_{entidadGuid}";
            string valorBase = $@"{valorEntidadAuxiliar}|";

            // Título de la página
            triplesInclude.Add(new TriplesToInclude
            {
                Description = false,
                Title = false,
                Predicate = predicadoBase + "http://w3id.org/roh/title",
                NewValue = valorBase + pTitulo
            });

            // Orden de la página
            int orden = 0;
            List<DataPageUser> listaPaginas = GetPagesUser(pUserId);
            foreach (DataPageUser item in listaPaginas)
            {
                if (item.orden > orden)
                {
                    orden = item.orden;
                }
            }
            orden++;

            triplesInclude.Add(new TriplesToInclude
            {
                Description = false,
                Title = false,
                Predicate = predicadoBase + "http://w3id.org/roh/order",
                NewValue = valorBase + orden
            });

            bool insertado = IncluirTriplesRecurso(mResourceApi, shortId, triplesInclude);
            if (insertado)
            {
                return valorEntidadAuxiliar;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Borra la gráfica.
        /// </summary>
        /// <param name="pUserId">ID del usuario.</param>
        /// <param name="pPageID">ID de la página</param>
        /// <param name="pGraphicID">ID de la gráfica</param>
        public static void BorrarGrafica(string pUserId, string pPageID, string pGraphicID)
        {
            mResourceApi.ChangeOntoly("person");

            // ID del recurso del usuario.
            string idRecurso = GetIdPersonByGnossUser(pUserId);

            Guid guid = mResourceApi.GetShortGuid(idRecurso);
            Dictionary<Guid, List<RemoveTriples>> dicBorrado = new Dictionary<Guid, List<RemoveTriples>>();
            List<RemoveTriples> listaTriplesBorrado = new List<RemoveTriples>();

            RemoveTriples triple = new RemoveTriples();
            triple.Title = false;
            triple.Description = false;
            triple.Predicate = $@"http://w3id.org/roh/metricPage|http://w3id.org/roh/metricGraphic";
            triple.Value = pPageID + "|" + pGraphicID;
            listaTriplesBorrado.Add(triple);

            dicBorrado.Add(guid, listaTriplesBorrado);
            mResourceApi.DeletePropertiesLoadedResources(dicBorrado);
            ReordenarGráficas(pUserId, pPageID);
        }

        /// <summary>
        /// Borra la página de indicadores personal
        /// </summary>
        /// <param name="pUserId">ID del usuario.</param>
        /// <param name="pPageID">ID del recurso a borrar el triple.</param>
        public static void BorrarPagina(string pUserId, string pPageID)
        {
            mResourceApi.ChangeOntoly("person");

            // ID del recurso del usuario.
            string idRecurso = GetIdPersonByGnossUser(pUserId);

            Guid guid = mResourceApi.GetShortGuid(idRecurso);
            Dictionary<Guid, List<RemoveTriples>> dicBorrado = new Dictionary<Guid, List<RemoveTriples>>();
            List<RemoveTriples> listaTriplesBorrado = new List<RemoveTriples>();

            RemoveTriples triple = new RemoveTriples();
            triple.Title = false;
            triple.Description = false;
            triple.Predicate = $@"http://w3id.org/roh/metricPage";
            triple.Value = pPageID;
            listaTriplesBorrado.Add(triple);

            dicBorrado.Add(guid, listaTriplesBorrado);
            mResourceApi.DeletePropertiesLoadedResources(dicBorrado);
            ReordenarPaginas(pUserId);
        }

        /// <summary>
        /// Reordena las páginas después de un cambio
        /// </summary>
        /// <param name="pUserId">ID del usuario.</param>
        public static void ReordenarPaginas(string pUserId)
        {
            string idRecurso = GetIdPersonByGnossUser(pUserId);

            Guid guid = mResourceApi.GetShortGuid(idRecurso);
            Dictionary<Guid, List<TriplesToModify>> dicModificacion = new Dictionary<Guid, List<TriplesToModify>>();
            List<TriplesToModify> listaTriplesModificacion = new List<TriplesToModify>();
            List<DataPageUser> listaPaginas = GetPagesUser(pUserId).OrderBy(x => x.orden).ToList();

            int index = 1;
            foreach (DataPageUser pagina in listaPaginas)
            {
                if (pagina.orden != index)
                {
                    TriplesToModify triple = new TriplesToModify();
                    triple.Predicate = "http://w3id.org/roh/metricPage|http://w3id.org/roh/order";
                    triple.NewValue = pagina.idRecurso + "|" + index;
                    triple.OldValue = pagina.idRecurso + "|" + pagina.orden;
                    listaTriplesModificacion.Add(triple);
                }
                index++;
            }

            dicModificacion.Add(guid, listaTriplesModificacion);
            mResourceApi.ModifyPropertiesLoadedResources(dicModificacion);
        }

        /// <summary>
        /// Edita el nombre de la página de usuario.
        /// </summary>
        /// <param name="pUserId">ID del usuario.</param>
        /// <param name="pPageID">ID del recurso a editar el triple.</param>
        /// <param name="pNewTitle">Nuevo título de la página.</param>
        /// <param name="pOldTitle">Anterior título de la página.</param>
        public static void EditarNombrePagina(string pUserId, string pPageID, string pNewTitle, string pOldTitle)
        {
            mResourceApi.ChangeOntoly("person");

            // ID del recurso del usuario.
            string idRecurso = GetIdPersonByGnossUser(pUserId);

            Guid guid = mResourceApi.GetShortGuid(idRecurso);

            Dictionary<Guid, List<TriplesToModify>> dicModificacion = new Dictionary<Guid, List<TriplesToModify>>();
            List<TriplesToModify> listaTriplesModificacion = new List<TriplesToModify>();

            TriplesToModify triple = new TriplesToModify();
            triple.Predicate = "http://w3id.org/roh/metricPage|http://w3id.org/roh/title";
            triple.NewValue = pPageID + "|" + pNewTitle;
            triple.OldValue = pPageID + "|" + pOldTitle;
            listaTriplesModificacion.Add(triple);

            dicModificacion.Add(guid, listaTriplesModificacion);
            mResourceApi.ModifyPropertiesLoadedResources(dicModificacion);
        }

        /// <summary>
        /// Edita el orden de la página
        /// </summary>
        /// <param name="pUserId">ID del usuario.</param>
        /// <param name="pPageID">ID de la página.</param>
        /// <param name="pOldOrder">Orden anterior de la página.</param>
        /// <param name="pNewOrder">Orden nuevo de la página</param>
        public static void EditarOrdenPagina(string pUserId, string pPageID, int pNewOrder, int pOldOrder)
        {
            mResourceApi.ChangeOntoly("person");

            // ID del recurso del usuario.
            string idRecurso = GetIdPersonByGnossUser(pUserId);

            Guid guid = mResourceApi.GetShortGuid(idRecurso);
            Dictionary<Guid, List<TriplesToModify>> dicModificacion = new Dictionary<Guid, List<TriplesToModify>>();
            List<TriplesToModify> listaTriplesModificacion = new List<TriplesToModify>();
            List<DataPageUser> listaPaginas = GetPagesUser(pUserId).OrderBy(x => x.orden).ToList();
            foreach (DataPageUser pagina in listaPaginas)
            {
                if (pagina.orden >= pNewOrder && pagina.orden < pOldOrder)
                {
                    pagina.orden++;
                }
                else if (pagina.orden <= pNewOrder && pagina.orden > pOldOrder)
                {
                    pagina.orden--;
                }
            }
            listaPaginas[pOldOrder - 1].orden = pNewOrder;
            int index = 1;
            foreach (DataPageUser pagina in listaPaginas)
            {
                if (pagina.orden != index)
                {
                    TriplesToModify triple = new TriplesToModify();
                    triple.Predicate = "http://w3id.org/roh/metricPage|http://w3id.org/roh/order";
                    triple.NewValue = pagina.idRecurso + "|" + pagina.orden;
                    triple.OldValue = pagina.idRecurso + "|" + index;
                    listaTriplesModificacion.Add(triple);
                }
                index++;
            }

            dicModificacion.Add(guid, listaTriplesModificacion);
            mResourceApi.ModifyPropertiesLoadedResources(dicModificacion);
        }

        /// <summary>
        /// Edita el nombre de la gráfica
        /// </summary>
        /// <param name="pUserId">ID del usuario.</param>
        /// <param name="pGraphicID">ID de la gráfica</param>
        /// <param name="pPageID">ID de la página</param>
        /// <param name="pNewTitle">Nuevo título</param>
        /// <param name="pOldTitle">Antiguo título</param>
        public static void EditarNombreGrafica(string pUserId, string pPageID, string pGraphicID, string pNewTitle, string pOldTitle)
        {
            mResourceApi.ChangeOntoly("person");

            // ID del recurso del usuario.
            string idRecurso = GetIdPersonByGnossUser(pUserId);

            Guid guid = mResourceApi.GetShortGuid(idRecurso);
            Dictionary<Guid, List<TriplesToModify>> dicModificacion = new Dictionary<Guid, List<TriplesToModify>>();
            List<TriplesToModify> listaTriplesModificacion = new List<TriplesToModify>();
            TriplesToModify triple = new TriplesToModify();
            triple.Predicate = $@"http://w3id.org/roh/metricPage|http://w3id.org/roh/metricGraphic|http://w3id.org/roh/title";
            triple.NewValue = pPageID + "|" + pGraphicID + "|" + pNewTitle;
            triple.OldValue = pPageID + "|" + pGraphicID + "|" + pOldTitle;
            listaTriplesModificacion.Add(triple);

            dicModificacion.Add(guid, listaTriplesModificacion);
            mResourceApi.ModifyPropertiesLoadedResources(dicModificacion);
        }

        /// <summary>
        /// Edita el orden de la gráfica
        /// </summary>
        /// <param name="pUserId">ID del usuario.</param>
        /// <param name="pPageID">ID de la página</param>
        /// <param name="pGraphicID">ID de la gráfica</param>
        /// <param name="pNewOrder">El orden nuevo</param>
        /// <param name="pOldOrder">El orden antiguo</param>
        public static void EditarOrdenGrafica(string pUserId, string pPageID, string pGraphicID, int pNewOrder, int pOldOrder)
        {
            mResourceApi.ChangeOntoly("person");

            // ID del recurso del usuario.
            string idRecurso = GetIdPersonByGnossUser(pUserId);

            Guid guid = mResourceApi.GetShortGuid(idRecurso);
            Dictionary<Guid, List<TriplesToModify>> dicModificacion = new Dictionary<Guid, List<TriplesToModify>>();
            List<TriplesToModify> listaTriplesModificacion = new List<TriplesToModify>();
            List<DataGraphicUser> listaGraficas = GetGraficasUserByPageId(pPageID).OrderBy(x => x.orden).ToList();
            foreach (DataGraphicUser grafica in listaGraficas)
            {
                if (grafica.orden >= pNewOrder && grafica.orden < pOldOrder)
                {
                    grafica.orden++;
                }
                else if (grafica.orden <= pNewOrder && grafica.orden > pOldOrder)
                {
                    grafica.orden--;
                }
            }
            listaGraficas[pOldOrder - 1].orden = pNewOrder;
            int index = 1;
            foreach (DataGraphicUser grafica in listaGraficas)
            {
                if (grafica.orden != index)
                {
                    TriplesToModify triple = new TriplesToModify();
                    triple.Predicate = "http://w3id.org/roh/metricPage|http://w3id.org/roh/metricGraphic|http://w3id.org/roh/order";
                    triple.NewValue = pPageID + "|" + grafica.idRecurso + "|" + grafica.orden;
                    triple.OldValue = pPageID + "|" + grafica.idRecurso + "|" + index;
                    listaTriplesModificacion.Add(triple);
                }
                index++;
            }

            dicModificacion.Add(guid, listaTriplesModificacion);
            mResourceApi.ModifyPropertiesLoadedResources(dicModificacion);
        }
        /// <summary>
        /// Reordena las gráficas después de un cambio
        /// </summary>
        /// <param name="pUserId">ID del usuario.</param>
        /// <param name="pPageID">ID de la página.</param>
        public static void ReordenarGráficas(string pUserId, string pPageID)
        {
            string idRecurso = GetIdPersonByGnossUser(pUserId);

            Guid guid = mResourceApi.GetShortGuid(idRecurso);
            Dictionary<Guid, List<TriplesToModify>> dicModificacion = new Dictionary<Guid, List<TriplesToModify>>();
            List<TriplesToModify> listaTriplesModificacion = new List<TriplesToModify>();
            List<DataGraphicUser> listaGraficas = GetGraficasUserByPageId(pPageID).OrderBy(x => x.orden).ToList();

            int index = 1;
            foreach (DataGraphicUser grafica in listaGraficas)
            {
                if (grafica.orden != index)
                {
                    TriplesToModify triple = new TriplesToModify();
                    triple.Predicate = "http://w3id.org/roh/metricPage|http://w3id.org/roh/metricGraphic|http://w3id.org/roh/order";
                    triple.NewValue = pPageID + "|" + grafica.idRecurso + "|" + index;
                    triple.OldValue = pPageID + "|" + grafica.idRecurso + "|" + grafica.orden;
                    listaTriplesModificacion.Add(triple);
                }
                index++;
            }

            dicModificacion.Add(guid, listaTriplesModificacion);
            mResourceApi.ModifyPropertiesLoadedResources(dicModificacion);
        }

        /// <summary>
        /// Borra la relación de la página.
        /// </summary>
        /// <param name="pUserId">ID del usuario.</param>
        /// <param name="pPageID">ID de la página</param>
        /// <param name="pGraphicID">ID de la gráfica</param>
        /// <param name="pNewWidth">Nueva anchura</param>
        /// <param name="pOldWidth">Antigua anchura</param>
        public static void EditarAnchuraGrafica(string pUserId, string pPageID, string pGraphicID, int pNewWidth, int pOldWidth)
        {
            mResourceApi.ChangeOntoly("person");

            // ID del recurso del usuario.
            string idRecurso = GetIdPersonByGnossUser(pUserId);

            Guid guid = mResourceApi.GetShortGuid(idRecurso);
            Dictionary<Guid, List<TriplesToModify>> dicModificacion = new Dictionary<Guid, List<TriplesToModify>>();
            List<TriplesToModify> listaTriplesModificacion = new List<TriplesToModify>();
            TriplesToModify triple = new TriplesToModify();
            triple.Predicate = $@"http://w3id.org/roh/metricPage|http://w3id.org/roh/metricGraphic|http://w3id.org/roh/width";
            triple.NewValue = pPageID + "|" + pGraphicID + "|" + pNewWidth;
            triple.OldValue = pPageID + "|" + pGraphicID + "|" + pOldWidth;
            listaTriplesModificacion.Add(triple);

            dicModificacion.Add(guid, listaTriplesModificacion);
            mResourceApi.ModifyPropertiesLoadedResources(dicModificacion);
        }

        /// <summary>
        /// Borra la relación de la página.
        /// </summary>
        /// <param name="pUserId">ID del usuario.</param>
        /// <param name="pGraphicID">ID de la gráfica</param>
        /// <param name="pNewScales">Valor de la escala nueva</param>
        /// <param name="pOldScales">Valor de la escala antigua</param>
        public static void EditarEscalasGrafica(string pUserId, string pPageID, string pGraphicID, string pNewScales, string pOldScales)
        {
            mResourceApi.ChangeOntoly("person");
            // ID del recurso del usuario.
            string idRecurso = GetIdPersonByGnossUser(pUserId);
            Guid guid = mResourceApi.GetShortGuid(idRecurso);
            Dictionary<Guid, List<TriplesToModify>> dicModificacion = new Dictionary<Guid, List<TriplesToModify>>();
            List<TriplesToModify> listaTriplesModificacion = new List<TriplesToModify>();
            TriplesToModify triple = new TriplesToModify();
            triple.Predicate = $@"http://w3id.org/roh/metricPage|http://w3id.org/roh/metricGraphic|http://w3id.org/roh/scales";
            triple.NewValue = pPageID + "|" + pGraphicID + "|" + pNewScales;
            triple.OldValue = pPageID + "|" + pGraphicID + "|" + pOldScales;
            listaTriplesModificacion.Add(triple);

            dicModificacion.Add(guid, listaTriplesModificacion);
            mResourceApi.ModifyPropertiesLoadedResources(dicModificacion);
        }

        /// <summary>
        /// Obtiene la lista de configuraciones.
        /// </summary>
        public static List<ConfigModel> TabTemplates
        {
            get
            {
                if (mTabTemplates == null || mTabTemplates.Count != Directory.EnumerateFiles($@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config{Path.DirectorySeparatorChar}configGraficas").Count())
                {
                    mTabTemplates = new List<ConfigModel>();
                    foreach (string file in Directory.EnumerateFiles($@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config{Path.DirectorySeparatorChar}configGraficas"))
                    {
                        ConfigModel tab = JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText(file));
                        mTabTemplates.Add(tab);
                    }
                }
                return mTabTemplates.OrderBy(x => x.Orden).ToList();
            }
        }

        /// <summary>
        /// Crea la lista de colores para rellenar las gráficas.
        /// </summary>
        /// <param name="pNumVeces">Número de la lista.</param>
        /// <param name="pColorHex">Color a rellenar.</param>
        /// <returns></returns>
        public static List<string> ObtenerColores(int pNumVeces, string pColorHex)
        {
            List<string> colores = new List<string>();
            for (int i = 0; i < pNumVeces; i++)
            {
                colores.Add(pColorHex);
            }
            return colores;
        }

        /// <summary>
        /// Obtiene el idioma de del diccionario de idiomas.
        /// </summary>
        /// <param name="pLang">Idioma.</param>
        /// <param name="pValores">Diccionario.</param>
        /// <returns></returns>
        public static string GetTextLang(string pLang, Dictionary<string, string> pValores)
        {
            if (pValores == null)
            {
                return "";
            }
            else if (pValores.ContainsKey(pLang))
            {
                return pValores[pLang];
            }
            else if (pValores.ContainsKey("es"))
            {
                return pValores["es"];
            }
            else if (pValores.Count > 0)
            {
                return pValores.Values.First();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Obtiene la lista de colores degradados.
        /// </summary>
        /// <param name="pColorMax">Color máximo como límite.</param>
        /// <param name="pColorMin">Color mínimo como límite.</param>
        /// <param name="pNumColores">Número de colores a devolver.</param>
        /// <returns></returns>
        public static List<string> ObtenerDegradadoColores(string pColorMax, string pColorMin, int pNumColores)
        {
            List<string> listaColores = new List<string>();
            if (pColorMax.Length < 7 || pColorMin.Length < 7)
            {
                pColorMax = "#FFFFFF";
                pColorMin = "#000000";
            }
            int rMax = Convert.ToInt32(pColorMax.Substring(1, 2), 16);
            int gMax = Convert.ToInt32(pColorMax.Substring(3, 2), 16);
            int bMax = Convert.ToInt32(pColorMax.Substring(5, 2), 16);
            int rMin = Convert.ToInt32(pColorMin.Substring(1, 2), 16);
            int gMin = Convert.ToInt32(pColorMin.Substring(3, 2), 16);
            int bMin = Convert.ToInt32(pColorMin.Substring(5, 2), 16);

            for (int i = 0; i < pNumColores; i++)
            {
                int rAverage = rMin + ((rMax - rMin) * i / pNumColores);
                int gAverage = gMin + ((gMax - gMin) * i / pNumColores);
                int bAverage = bMin + ((bMax - bMin) * i / pNumColores);
                string colorHex = '#' + rAverage.ToString("X2") + gAverage.ToString("X2") + bAverage.ToString("X2");
                listaColores.Add(colorHex);
            }

            return listaColores;
        }

        /// <summary>
        /// Procesa las relaciones de las gráficas de nodos.
        /// </summary>
        /// <param name="pNombreRelacion">Nombre de la relación.</param>
        /// <param name="pItems">Número de ítems.</param>
        /// <param name="pDicRelaciones">Diccionario con las relaciones.</param>
        public static void ProcesarRelaciones(string pNombreRelacion, Dictionary<string, List<string>> pItems, ref Dictionary<string, List<DataQueryRelaciones>> pDicRelaciones)
        {
            foreach (string itemA in pItems.Keys)
            {
                if (!pDicRelaciones.ContainsKey(itemA))
                {
                    pDicRelaciones.Add(itemA, new List<DataQueryRelaciones>());
                }
                DataQueryRelaciones dataQueryRelaciones = (pDicRelaciones[itemA].FirstOrDefault(x => x.nombreRelacion == pNombreRelacion));
                if (dataQueryRelaciones == null)
                {
                    dataQueryRelaciones = new DataQueryRelaciones()
                    {
                        nombreRelacion = pNombreRelacion,
                        idRelacionados = new List<Datos>()
                    };
                    pDicRelaciones[itemA].Add(dataQueryRelaciones);
                }
                foreach (string itemB in pItems.Keys)
                {
                    if (itemA != itemB && string.Compare(itemA, itemB, StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        int num = pItems[itemA].Intersect(pItems[itemB]).Count();
                        if (num > 0)
                        {
                            dataQueryRelaciones.idRelacionados.Add(new Datos()
                            {
                                idRelacionado = itemB,
                                numVeces = num
                            });
                        }
                    }
                }
                if (dataQueryRelaciones.idRelacionados.Count == 0)
                {
                    pDicRelaciones[itemA].Remove(dataQueryRelaciones);
                }
            }
        }
        public static string GetUserFromMetricPage(string pMetricID)
        {

            string select = "SELECT DISTINCT ?user ";
            string where = $@"WHERE{{
               
                {{
                  ?user ?s <{pMetricID}>.
                  ?user a <http://xmlns.com/foaf/0.1/Person>.
                }}
                
            }}

            ";
            SparqlObject sparqlObjectAux = mResourceApi.VirtuosoQuery(select, where, "person");
            List<string> resultados = sparqlObjectAux.results.bindings.Select(x => x["user"].value).Distinct().ToList();
            string user = resultados.ToList().FirstOrDefault();
            return user;


        }

        /// <summary>
        /// Permite calcular el valor del ancho de la línea según el número de colaboraciones que tenga el nodo.
        /// </summary>
        /// <param name="pMax">Valor máximo.</param>
        /// <param name="pColabo">Número de colaboraciones.</param>
        /// <returns>Ancho de la línea en formate double.</returns>
        public static double CalcularGrosor(int pMax, int pColabo)
        {
            return Math.Round(((double)pColabo / (double)pMax) * 10, 2);
        }
        #endregion

        #region --- Excepciones
        public static void ControlarExcepcionesBarrasX(Grafica pGrafica)
        {
            if (pGrafica.Config == null)
            {
                throw new ArgumentException("La gráfica no tiene configuración");
            }
            if (pGrafica.Config.YAxisPrint == null)
            {
                throw new ArgumentException("No está configurada la propiedad yAxisPrint");
            }
            if (pGrafica.Config.Dimensiones == null || !pGrafica.Config.Dimensiones.Any())
            {
                throw new ArgumentException("No se ha configurado dimensiones.");
            }
        }
        public static void ControlarExcepcionesBarrasY(Grafica pGrafica)
        {
            if (pGrafica.Config == null)
            {
                throw new ArgumentException("La gráfica no tiene configuración");
            }
            if (pGrafica.Config.XAxisPrint == null)
            {
                throw new ArgumentException("No está configurada la propiedad xAxisPrint");
            }
            if (pGrafica.Config.Dimensiones == null || !pGrafica.Config.Dimensiones.Any())
            {
                throw new ArgumentException("No se ha configurado dimensiones.");
            }
        }
        public static void ControlarExcepcionesCircular(Grafica pGrafica)
        {
            if (pGrafica.Config == null)
            {
                throw new ArgumentException("La gráfica no tiene configuración");
            }
            if (pGrafica.Config.Dimensiones == null || !pGrafica.Config.Dimensiones.Any())
            {
                throw new ArgumentException("No se ha configurado dimensiones.");
            }
        }
        #endregion
    }
}
