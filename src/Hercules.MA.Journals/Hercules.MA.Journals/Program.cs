using ExcelDataReader;
using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using Hercules.MA.Journals.Controllers;
using Hercules.MA.Journals.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hercules.MA.Journals
{
    internal class Program
    {
        private static ResourceApi mResourceApi = new ResourceApi($@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config\configOAuth\OAuthV3.config");

        public static ConfigService configS;

        //Número de hilos para el paralelismo.
        public static int NUM_HILOS = 6;

        //Número máximo de intentos de subida
        public static int MAX_INTENTOS = 10;

        static void Main(string[] args)
        {            
            string nombreExcel = "Revistas";
            string nombreHoja = "revistas";
            configS = new ConfigService();
            // Obtención de revistas de BBDD.
            //List<string> idRecursosRevistas = ObtenerIDsRevistas();
            //Dictionary<string, Journal> dicRevistasBBDD = ObtenerRevistaPorID(idRecursosRevistas);

            // Diccionario de revistas.
            //List<Journal> listaRevistas = dicRevistasBBDD.Values.ToList();            
            List<Journal> listaRevistas = new List<Journal>();
            Console.WriteLine($@"{DateTime.Now} Leyendo EXCEL de revistas...");
            DataSet dataSet = LecturaExcel($@"{configS.GetRutaDatos()}/{nombreExcel}.xlsx");

            if (ComprobarColumnasExcel(dataSet, nombreHoja))
            {
                Console.WriteLine($@"{DateTime.Now} Procesando revistas...");
                listaRevistas = GetRevistas(dataSet, nombreHoja, listaRevistas);
            }

            // Ordenación de por Key.
            listaRevistas = listaRevistas.OrderBy(obj => obj.titulo).ToList();

            // Comprobar si hay datos duplicados.
            ComprobarErrores(listaRevistas);

            // Carga/Modificación/Borrado de datos de BBDD. 
            //ModificarRevistas(listaRevistas);

            // TODO: Versión antigua. No borrar de momento.
            // Lectura del excel. 
            //Console.WriteLine($@"{DateTime.Now} Leyendo EXCEL de Scopus...");
            //DataSet dataSetScopus = LecturaExcel($@"{configuracion.GetRutaDatos()}/scopus/CiteScore.xlsx");

            //// Obtiebe los años de las revistas.
            //List<int> listaAnyosRevistas = ObtenerAnyos(dataSetScopus);

            //// Obtención de las revistas.
            //foreach (int anyo in listaAnyosRevistas)
            //{
            //    // Datos de WoS (SCIE). Lectura del excel. 
            //    Console.WriteLine($@"{DateTime.Now} Leyendo EXCEL de 'WoS (SCIE) {anyo}'...");
            //    DataSet dataSetSCIE = LecturaExcel($@"{configuracion.GetRutaDatos()}/wos/{anyo}/JCR_SCIE_{anyo}.xlsx");
            //    if (ComprobarExcelWOS(dataSetSCIE, anyo, "SCIE"))
            //    {
            //        Console.WriteLine($@"{DateTime.Now} [{anyo}] Procesando {dataSetSCIE.Tables[dataSetSCIE.Tables[$@"JCR_SCIE_{anyo}"].TableName].Rows.Count} revistas de WoS (SCIE)...");
            //        listaRevistas = GetRevistasWoS(dataSetSCIE, anyo, listaRevistas, "SCIE");
            //    }

            //    // Datos de WoS (SSCI). Lectura del excel. 
            //    Console.WriteLine($@"{DateTime.Now} Leyendo EXCEL de 'WoS (SSCI) {anyo}'...");
            //    DataSet dataSetSSCI = LecturaExcel($@"{configuracion.GetRutaDatos()}/wos/{anyo}/JCR_SSCI_{anyo}.xlsx");
            //    if (ComprobarExcelWOS(dataSetSSCI, anyo, "SSCI"))
            //    {
            //        Console.WriteLine($@"{DateTime.Now} [{anyo}] Procesando {dataSetSSCI.Tables[dataSetSSCI.Tables[$@"JCR_SSCI_{anyo}"].TableName].Rows.Count} revistas de WoS (SSCI)...");
            //        listaRevistas = GetRevistasWoS(dataSetSSCI, anyo, listaRevistas, "SSCI");
            //    }

            //    // Datos de Scopus.
            //    if (ComprobarExcelScopus(dataSetScopus, anyo))
            //    {
            //        Console.WriteLine($@"{DateTime.Now} [{anyo}] Procesando {dataSetScopus.Tables[dataSetScopus.Tables[$@"CiteScore {anyo}"].TableName].Rows.Count} revistas de Scopus...");
            //        listaRevistas = GetRevistasScopus(dataSetScopus, anyo, listaRevistas);
            //    }
            //}

            //// Ordenación de por Key.
            //listaRevistas = listaRevistas.OrderBy(obj => obj.titulo).ToList();

            //// Comprobar si hay datos duplicados.
            //ComprobarErrores(listaRevistas);

            //// Carga/Modificación/Borrado de datos de BBDD. 
            //ModificarRevistas(listaRevistas);
        }


        private static void ModificarRevistas(List<Journal> pRevistas)
        {
            // Obtenemos las revistas de BBDD
            List<string> idRecursosRevistas = ObtenerIDsRevistas();
            List<Journal> dicRevistasBBDD = ObtenerRevistaPorID(idRecursosRevistas).Values.ToList();

            mResourceApi.ChangeOntoly("maindocument");

            // Creación 
            List<Journal> revistasCargar = pRevistas.Where(x => string.IsNullOrEmpty(x.idJournal)).ToList();
            List<ComplexOntologyResource> listaRecursosCargar = new List<ComplexOntologyResource>();
            ObtenerRevistas(revistasCargar, listaRecursosCargar);
            CargarDatos(listaRecursosCargar);

            // Borrado
            foreach (Journal journal in dicRevistasBBDD)
            {
                if (!pRevistas.Exists(x => x.idJournal == journal.idJournal))
                {
                    try
                    {
                        mResourceApi.PersistentDelete(mResourceApi.GetShortGuid(journal.idJournal));
                    }
                    catch
                    {
                        // Si entra por aquí, es error de CORE.
                    }
                }
            }

            // Modificación            
            foreach (Journal journalBBDD in dicRevistasBBDD)
            {
                Journal journalCargar = pRevistas.FirstOrDefault(x => x.idJournal == journalBBDD.idJournal);
                if (journalCargar != null)
                {
                    if (!journalBBDD.Equals(journalCargar))
                    {
                        List<ComplexOntologyResource> listaRecursosModificar = new List<ComplexOntologyResource>();
                        ObtenerRevistas(new List<Journal>() { journalCargar }, listaRecursosModificar);
                        ModificarDatos(listaRecursosCargar);
                    }
                }
            }
        }

        /// <summary>
        /// Contruye el objeto de Revista.
        /// </summary>
        /// <param name="pDataSet"></param>
        /// <param name="pNombreHoja"></param>
        /// <param name="pListaRevistas"></param>
        /// <returns></returns>
        public static List<Journal> GetRevistas(DataSet pDataSet, string pNombreHoja, List<Journal> pListaRevistas)
        {
            // Obtención de la hoja a leer.
            DataTable tabla = pDataSet.Tables[$@"{pNombreHoja}"];

            // Contadores.
            int revistasSinTitulo = 0;
            int revistasSinIdentificadores = 0;

            // Ordenar DATASET
            var xx = pDataSet.Tables[tabla.TableName];
            xx.DefaultView.Sort = "TITLE ASC, PUBLISHER_NAME ASC, ISSN ASC, EISSN ASC, SOURCE ASC, YEAR ASC";
            xx = pDataSet.Tables[tabla.TableName].DefaultView.ToTable();

            foreach (DataRow fila in xx.Rows)
            {
                // Si la revista no tiene título, no es válida.
                if (string.IsNullOrEmpty(fila["TITLE"].ToString()))
                {
                    revistasSinTitulo++;
                    continue;
                }

                // Si la revista no tiene ISSN ni EISSN, no es válida.
                if (string.IsNullOrEmpty(fila["ISSN"].ToString()) && string.IsNullOrEmpty(fila["EISSN"].ToString()))
                {
                    revistasSinIdentificadores++;
                    continue;
                }

                // Datos 
                string titleAux = fila["TITLE"].ToString();
                string editorialAux = fila["PUBLISHER_NAME"].ToString();
                string issnAux = LimpiarIdentificador(fila["ISSN"].ToString());
                string eissnAux = LimpiarIdentificador(fila["EISSN"].ToString());

                // Si tienen el mismo ISSN e EISSN, únicamente tienen EISSN.
                if (!string.IsNullOrEmpty(issnAux) && !string.IsNullOrEmpty(eissnAux) && issnAux == eissnAux)
                {
                    issnAux = null;
                }

                Journal revista = ComprobarRevista(pListaRevistas, titleAux, editorialAux, issnAux, eissnAux);

                if (revista == null)
                {
                    revista = new Journal();
                    revista.indicesImpacto = new HashSet<IndiceImpacto>();
                    pListaRevistas.Add(revista);
                }

                // Año.
                int anyo = Int32.Parse(fila["YEAR"].ToString());

                // Título.
                revista.titulo = titleAux;

                if (!string.IsNullOrEmpty(editorialAux))
                {
                    // Publicador.
                    revista.publicador = editorialAux;
                }

                if (!string.IsNullOrEmpty(issnAux))
                {
                    // ISSN.
                    revista.issn = issnAux;
                }

                if (!string.IsNullOrEmpty(eissnAux))
                {
                    // EISSN.
                    revista.eissn = eissnAux;
                }

                // Índice de impacto.
                bool encontrado = false;
                foreach (IndiceImpacto item in revista.indicesImpacto)
                {
                    if (item.fuente == fila["SOURCE"].ToString() && item.anyo == anyo)
                    {
                        encontrado = true;
                        break;
                    }
                }

                if (!encontrado && !string.IsNullOrEmpty(fila["IMPACT_FACTOR"].ToString()))
                {
                    revista.indicesImpacto.Add(CrearIndiceImpacto(fila, anyo));
                }

                // Categorías.
                if (!string.IsNullOrEmpty(fila["CATEGORY_DESCRIPTION"].ToString()) && !string.IsNullOrEmpty(fila["IMPACT_FACTOR"].ToString()))
                {
                    HashSet<Categoria> categorias = revista.indicesImpacto.First(x => x.anyo == anyo && x.fuente == fila["SOURCE"].ToString()).categorias;
                    Categoria categoria = CrearCategoria(fila, anyo);

                    if (!categorias.Any(x => x.nomCategoria == categoria.nomCategoria))
                    {
                        revista.indicesImpacto.First(x => x.anyo == anyo && x.fuente == fila["SOURCE"].ToString()).categorias.Add(categoria);
                    }
                    else
                    {
                        Categoria categoriaCargada = categorias.First(x => x.nomCategoria == categoria.nomCategoria);
                        float rangoNuevo = (float)categoria.posicionPublicacion / (float)categoria.numCategoria;
                        float rangoCargado = (float)categoriaCargada.posicionPublicacion / (float)categoriaCargada.numCategoria;

                        if ((rangoNuevo < rangoCargado) || (rangoNuevo == rangoCargado && categoria.numCategoria > categoriaCargada.numCategoria))
                        {
                            categorias.Remove(categoriaCargada);
                            categorias.Add(categoria);
                            revista.indicesImpacto.First(x => x.anyo == anyo && x.fuente == fila["SOURCE"].ToString()).categorias = categorias;
                        }
                    }
                }
            }

            return pListaRevistas;
        }


        /// <summary>
        /// Contruye el objeto de Revista.
        /// </summary>
        /// <param name="pDataSet">DataSet del excel.</param>
        /// <param name="pAnyo">Año.</param>
        /// <param name="pListaRevistas">Lista para almacenar revistas.</param>
        /// <returns>Diccionario con revistas actualizadas.</returns>
        public static List<Journal> GetRevistasScopus(DataSet pDataSet, int pAnyo, List<Journal> pListaRevistas)
        {
            // Obtención de la hoja a leer.
            DataTable tabla = pDataSet.Tables[$@"CiteScore {pAnyo}"];

            // Contadores.
            int revistasSinTitulo = 0;
            int revistasSinIdentificadores = 0;

            foreach (DataRow fila in pDataSet.Tables[tabla.TableName].Rows)
            {
                // Si la revista no tiene título, no es válida.
                if (string.IsNullOrEmpty(fila["Title"].ToString()))
                {
                    revistasSinTitulo++;
                    continue;
                }

                // Si la revista no tiene ISSN ni EISSN, no es válida.
                if (string.IsNullOrEmpty(fila["Print ISSN"].ToString()) && string.IsNullOrEmpty(fila["E-ISSN"].ToString()))
                {
                    revistasSinIdentificadores++;
                    continue;
                }

                // Datos 
                string titleAux = fila["Title"].ToString();
                string editorialAux = fila["Publisher"].ToString();
                string issnAux = LimpiarIdentificador(fila["Print ISSN"].ToString());
                string eissnAux = LimpiarIdentificador(fila["E-ISSN"].ToString());

                // Si tienen el mismo ISSN e EISSN, únicamente tienen EISSN.
                if (!string.IsNullOrEmpty(issnAux) && !string.IsNullOrEmpty(eissnAux) && issnAux == eissnAux)
                {
                    issnAux = null;
                }

                Journal revista = ComprobarRevista(pListaRevistas, titleAux, editorialAux, issnAux);

                if (revista == null)
                {
                    revista = new Journal();
                    revista.indicesImpacto = new HashSet<IndiceImpacto>();
                    pListaRevistas.Add(revista);
                }

                // Título.
                revista.titulo = titleAux;

                if (!string.IsNullOrEmpty(editorialAux))
                {
                    // Publicador.
                    revista.publicador = editorialAux;
                }

                if (!string.IsNullOrEmpty(issnAux))
                {
                    // ISSN.
                    revista.issn = issnAux;
                }

                if (!string.IsNullOrEmpty(eissnAux))
                {
                    // EISSN.
                    revista.eissn = eissnAux;
                }

                // Índice de impacto.
                bool encontrado = false;
                foreach (IndiceImpacto item in revista.indicesImpacto)
                {
                    if (item.fuente == "scopus" && item.anyo == pAnyo)
                    {
                        encontrado = true;
                        break;
                    }
                }

                if (!encontrado && !string.IsNullOrEmpty(fila["SJR"].ToString()))
                {
                    revista.indicesImpacto.Add(CrearIndiceImpactoScopus(fila, pAnyo));
                }

                // Categorías.
                if (!string.IsNullOrEmpty(fila["Scopus Sub-Subject Area"].ToString()) && !string.IsNullOrEmpty(fila["SJR"].ToString()))
                {
                    HashSet<Categoria> categorias = revista.indicesImpacto.First(x => x.anyo == pAnyo && x.fuente == "scopus").categorias;
                    Categoria categoria = CrearCategoriaScopus(fila, pAnyo);

                    if (!categorias.Any(x => x.nomCategoria == categoria.nomCategoria))
                    {
                        revista.indicesImpacto.First(x => x.anyo == pAnyo && x.fuente == "scopus").categorias.Add(categoria);
                    }
                    else
                    {
                        Categoria categoriaCargada = categorias.First(x => x.nomCategoria == categoria.nomCategoria);
                        float rangoNuevo = (float)categoria.posicionPublicacion / (float)categoria.numCategoria;
                        float rangoCargado = (float)categoriaCargada.posicionPublicacion / (float)categoriaCargada.numCategoria;

                        if ((rangoNuevo < rangoCargado) || (rangoNuevo == rangoCargado && categoria.numCategoria > categoriaCargada.numCategoria))
                        {
                            categorias.Remove(categoriaCargada);
                            categorias.Add(categoria);
                            revista.indicesImpacto.First(x => x.anyo == pAnyo && x.fuente == "scopus").categorias = categorias;
                        }
                    }
                }
            }

            return pListaRevistas;
        }

        /// <summary>
        /// Contruye el objeto de Revista.
        /// </summary>
        /// <param name="pDataSet">DataSet del excel.</param>
        /// <param name="pAnyo">Año.</param>
        /// <param name="pListaRevistas">Lista para almacenar revistas.</param>
        /// <returns>Diccionario con revistas actualizadas.</returns>
        public static List<Journal> GetRevistasWoS(DataSet pDataSet, int pAnyo, List<Journal> pListaRevistas, string pTipo)
        {
            // Obtención de la hoja a leer.
            DataTable tabla = pDataSet.Tables[$@"JCR_{pTipo}_{pAnyo}"];

            // Contadores.
            int revistasSinTitulo = 0;
            int revistasSinIdentificadores = 0;

            foreach (DataRow fila in pDataSet.Tables[tabla.TableName].Rows)
            {
                // Si la revista no tiene título, no es válida.
                if (string.IsNullOrEmpty(fila["TITLE"].ToString()))
                {
                    revistasSinTitulo++;
                    continue;
                }

                // Si la revista no tiene ISSN ni EISSN, no es válida.
                if (string.IsNullOrEmpty(fila["ISSN"].ToString()) && string.IsNullOrEmpty(fila["EISSN"].ToString()))
                {
                    revistasSinIdentificadores++;
                    continue;
                }

                // Datos 
                string titleAux = fila["TITLE"].ToString();
                string editorialAux = fila["PUBLISHER_NAME"].ToString();
                string issnAux = LimpiarIdentificador(fila["ISSN"].ToString());
                string eissnAux = LimpiarIdentificador(fila["EISSN"].ToString());

                // Si tienen el mismo ISSN e EISSN, únicamente tienen EISSN.
                if (!string.IsNullOrEmpty(issnAux) && !string.IsNullOrEmpty(eissnAux) && issnAux == eissnAux)
                {
                    issnAux = null;
                }

                Journal revista = ComprobarRevista(pListaRevistas, titleAux, editorialAux, issnAux);

                if (revista == null)
                {
                    revista = new Journal();
                    revista.indicesImpacto = new HashSet<IndiceImpacto>();
                    pListaRevistas.Add(revista);
                }

                // Título.
                revista.titulo = titleAux;

                if (!string.IsNullOrEmpty(editorialAux))
                {
                    // Publicador.
                    revista.publicador = editorialAux;
                }

                if (!string.IsNullOrEmpty(issnAux))
                {
                    // ISSN.
                    revista.issn = issnAux;
                }

                if (!string.IsNullOrEmpty(eissnAux))
                {
                    // EISSN.
                    revista.eissn = eissnAux;
                }

                // Índice de impacto.
                bool encontrado = false;
                foreach (IndiceImpacto item in revista.indicesImpacto)
                {
                    if (item.fuente == "wos" && item.anyo == pAnyo)
                    {
                        encontrado = true;
                        break;
                    }
                }

                if (!encontrado && !string.IsNullOrEmpty(fila["IMPACT_FACTOR"].ToString()))
                {
                    revista.indicesImpacto.Add(CrearIndiceImpactoWoS(fila, pAnyo));
                }

                // Categorías.
                if (!string.IsNullOrEmpty(fila["CATEGORY_DESCRIPTION"].ToString()) && !string.IsNullOrEmpty(fila["IMPACT_FACTOR"].ToString()))
                {
                    HashSet<Categoria> categorias = revista.indicesImpacto.First(x => x.anyo == pAnyo && x.fuente == "wos").categorias;
                    Categoria categoria = CrearCategoriaWoS(fila, pAnyo);

                    if (!categorias.Any(x => x.nomCategoria == categoria.nomCategoria))
                    {
                        revista.indicesImpacto.First(x => x.anyo == pAnyo && x.fuente == "wos").categorias.Add(categoria);
                    }
                    else
                    {
                        Categoria categoriaCargada = categorias.First(x => x.nomCategoria == categoria.nomCategoria);
                        float rangoNuevo = (float)categoria.posicionPublicacion / (float)categoria.numCategoria;
                        float rangoCargado = (float)categoriaCargada.posicionPublicacion / (float)categoriaCargada.numCategoria;

                        if ((rangoNuevo < rangoCargado) || (rangoNuevo == rangoCargado && categoria.numCategoria > categoriaCargada.numCategoria))
                        {
                            categorias.Remove(categoriaCargada);
                            categorias.Add(categoria);
                            revista.indicesImpacto.First(x => x.anyo == pAnyo && x.fuente == "wos").categorias = categorias;
                        }
                    }
                }
            }

            return pListaRevistas;
        }

        /// <summary>
        /// Permite crear un índice de impacto con los datos de Scopus.
        /// </summary>
        /// <param name="pFila">Fila con los datos pertenecientes al excel.</param>
        /// <param name="pAnyo">Año.</param>
        /// <returns>Índice de impacto.</returns>
        public static IndiceImpacto CrearIndiceImpacto(DataRow pFila, int pAnyo)
        {
            IndiceImpacto indiceImpacto = new IndiceImpacto();
            indiceImpacto.categorias = new HashSet<Categoria>();

            // Fuente.
            indiceImpacto.fuente = pFila["SOURCE"].ToString();

            // Año.
            indiceImpacto.anyo = pAnyo;

            // Índice de impacto.
            if (!string.IsNullOrEmpty(pFila["IMPACT_FACTOR"].ToString()))
            {
                indiceImpacto.indiceImpacto = float.Parse(pFila["IMPACT_FACTOR"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);
            }

            return indiceImpacto;
        }

        /// <summary>
        /// Permite crear un índice de impacto con los datos de Scopus.
        /// </summary>
        /// <param name="pFila">Fila con los datos pertenecientes al excel.</param>
        /// <param name="pAnyo">Año.</param>
        /// <returns>Índice de impacto.</returns>
        public static IndiceImpacto CrearIndiceImpactoScopus(DataRow pFila, int pAnyo)
        {
            IndiceImpacto indiceImpacto = new IndiceImpacto();
            indiceImpacto.categorias = new HashSet<Categoria>();

            // Fuente.
            indiceImpacto.fuente = "scopus";

            // Año.
            indiceImpacto.anyo = pAnyo;

            // Índice de impacto.
            if (!string.IsNullOrEmpty(pFila["SJR"].ToString()))
            {
                indiceImpacto.indiceImpacto = float.Parse(pFila["SJR"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);
            }

            return indiceImpacto;
        }

        /// <summary>
        /// Permite crear un índice de impacto con los datos de WoS.
        /// </summary>
        /// <param name="pFila">Fila con los datos pertenecientes al excel.</param>
        /// <param name="pAnyo">Año.</param>
        /// <returns>Índice de impacto.</returns>
        public static IndiceImpacto CrearIndiceImpactoWoS(DataRow pFila, int pAnyo)
        {
            IndiceImpacto indiceImpacto = new IndiceImpacto();
            indiceImpacto.categorias = new HashSet<Categoria>();

            // Fuente.
            indiceImpacto.fuente = "wos";

            // Año.
            indiceImpacto.anyo = pAnyo;

            // Índice de impacto.
            if (!string.IsNullOrEmpty(pFila["IMPACT_FACTOR"].ToString()))
            {
                indiceImpacto.indiceImpacto = float.Parse(pFila["IMPACT_FACTOR"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);
            }

            return indiceImpacto;
        }

        /// <summary>
        /// Permite crear una categoría con los datos de Scopus.
        /// </summary>
        /// <param name="pFila">Fila con los datos pertenecientes al excel.</param>
        /// <param name="pAnyo">Año.</param>
        /// <returns>Categoría.</returns>
        public static Categoria CrearCategoriaScopus(DataRow pFila, int pAnyo)
        {
            Categoria categoria = new Categoria();

            // Fuente.
            categoria.fuente = "scopus";

            // Año.
            categoria.anyo = pAnyo;

            // Nombre categoría.
            if (!string.IsNullOrEmpty(pFila["Scopus Sub-Subject Area"].ToString()))
            {
                categoria.nomCategoria = pFila["Scopus Sub-Subject Area"].ToString();
            }

            // Ranking.
            if (!string.IsNullOrEmpty(pFila["RANK"].ToString()))
            {
                categoria.posicionPublicacion = Int32.Parse(pFila["RANK"].ToString());
            }

            // Posición en ranking.
            if (!string.IsNullOrEmpty(pFila["Rank Out Of"].ToString()))
            {
                categoria.numCategoria = Int32.Parse(pFila["Rank Out Of"].ToString());
            }

            // Cuartil.
            if (!string.IsNullOrEmpty(pFila["Quartile"].ToString()))
            {
                categoria.cuartil = Int32.Parse(pFila["Quartile"].ToString());
            }

            return categoria;
        }

        /// <summary>
        /// Permite crear una categoría con los datos de WoS.
        /// </summary>
        /// <param name="pFila">Fila con los datos pertenecientes al excel.</param>
        /// <param name="pAnyo">Año.</param>
        /// <returns>Categoría.</returns>
        public static Categoria CrearCategoriaWoS(DataRow pFila, int pAnyo)
        {
            Categoria categoria = new Categoria();

            // Fuente.
            categoria.fuente = "wos";

            // Año.
            categoria.anyo = pAnyo;

            // Nombre categoría.
            if (!string.IsNullOrEmpty(pFila["CATEGORY_DESCRIPTION"].ToString()))
            {
                categoria.nomCategoria = pFila["CATEGORY_DESCRIPTION"].ToString();
            }

            // Ranking y Posición en ranking.
            if (!string.IsNullOrEmpty(pFila["CATEGORY_RANKING"].ToString()) && pFila["CATEGORY_RANKING"].ToString() != "--")
            {
                categoria.posicionPublicacion = Int32.Parse(pFila["CATEGORY_RANKING"].ToString().Split('/')[0]);
                categoria.numCategoria = Int32.Parse(pFila["CATEGORY_RANKING"].ToString().Split('/')[1]);
            }

            // Cuartil.
            if (!string.IsNullOrEmpty(pFila["QUARTILE_RANK"].ToString()))
            {
                switch (pFila["QUARTILE_RANK"].ToString().ToLower())
                {
                    case "q1":
                        categoria.cuartil = 1;
                        break;
                    case "q2":
                        categoria.cuartil = 2;
                        break;
                    case "q3":
                        categoria.cuartil = 3;
                        break;
                    case "q4":
                        categoria.cuartil = 4;
                        break;
                }
            }

            return categoria;
        }

        /// <summary>
        /// Permite crear una categoría con los datos de WoS.
        /// </summary>
        /// <param name="pFila">Fila con los datos pertenecientes al excel.</param>
        /// <param name="pAnyo">Año.</param>
        /// <returns>Categoría.</returns>
        public static Categoria CrearCategoria(DataRow pFila, int pAnyo)
        {
            Categoria categoria = new Categoria();

            // Fuente.
            categoria.fuente = pFila["SOURCE"].ToString();

            // Año.
            categoria.anyo = pAnyo;

            // Nombre categoría.
            if (!string.IsNullOrEmpty(pFila["CATEGORY_DESCRIPTION"].ToString()))
            {
                categoria.nomCategoria = pFila["CATEGORY_DESCRIPTION"].ToString();
            }

            // Ranking y Posición en ranking.
            if (!string.IsNullOrEmpty(pFila["RANK"].ToString()) && !string.IsNullOrEmpty(pFila["RANK_OUT_OF"].ToString()) && pFila["RANK"].ToString() != "--" && pFila["RANK_OUT_OF"].ToString() != "--")
            {
                categoria.posicionPublicacion = Int32.Parse(pFila["RANK"].ToString());
                categoria.numCategoria = Int32.Parse(pFila["RANK_OUT_OF"].ToString());
            }

            // Cuartil.
            if (!string.IsNullOrEmpty(pFila["QUARTILE_RANK"].ToString()))
            {
                switch (pFila["QUARTILE_RANK"].ToString().ToLower())
                {
                    case "q1":
                        categoria.cuartil = 1;
                        break;
                    case "q2":
                        categoria.cuartil = 2;
                        break;
                    case "q3":
                        categoria.cuartil = 3;
                        break;
                    case "q4":
                        categoria.cuartil = 4;
                        break;
                }
            }

            return categoria;
        }

        /// <summary>
        /// Dado un ISSN o E-ISSN, lo construye en buen formato.
        /// </summary>
        /// <param name="pId">Identificador a formatear.</param>
        /// <returns>Identificador formateado.</returns>
        public static string LimpiarIdentificador(string pId)
        {
            // Comprobar si el ISSN/EISSN está bien formado.
            if (pId == "****-****")
            {
                return null;
            }

            // Comprobar si el ISSN/EISSN está bien formado.
            if (pId.Length == 9 && pId.Contains("-") && pId.Split("-")[0].Length == 4 && pId.Split("-")[1].Length == 4)
            {
                return pId;
            }

            string idFinal = null;

            if (!string.IsNullOrEmpty(pId) && pId.Length <= 8)
            {
                // Los ISSN y E-ISSN se componen por 8 caracteres.
                int numDiferencia = 8 - pId.Length;

                // Rellenamos con 0s en primera posición.
                if (numDiferencia != 0)
                {
                    for (int i = 0; i < numDiferencia; i++)
                    {
                        pId = $@"0{pId}";
                    }
                }

                // Agregamos el guión en medio.
                string parte1 = pId.Substring(0, 4);
                string parte2 = pId.Substring(4);
                idFinal = $@"{parte1}-{parte2}";
            }

            return idFinal;
        }

        /// <summary>
        /// Verifica si hay revistas duplicadas con diversas condiciones.
        /// </summary>
        /// <param name="pListaRevistas">Listado dónde se almacenan las revistas.</param>
        /// <param name="pTitulo">Título.</param>
        /// <param name="pEditorial">Editorial.</param>
        /// <param name="pIssn">ISSN</param>
        /// <returns>Revista si la encuentra o null si no la encuentra.</returns>
        /// <exception cref="Exception"></exception>
        public static Journal ComprobarRevista(List<Journal> pListaRevistas, string pTitulo = null, string pEditorial = null, string pIssn = null, string pEissn = null)
        {
            // No consideramos el EISSN como identificador porque diferentes revistas (con diferentes ISSN que sean tengan algún parentesco) pueden tener la misma versión online.
            Journal revista = null;
            List<Journal> revistasMismoTituloYEditorial = pListaRevistas.Where(x => x.titulo.ToLower() == pTitulo.ToLower() && x.publicador?.ToLower() == pEditorial?.ToLower()).ToList();
            List<Journal> revistasMismoISSN = pListaRevistas.Where(x => x.issn == pIssn && !string.IsNullOrEmpty(pIssn)).Except(revistasMismoTituloYEditorial).ToList();
            List<Journal> revistasMismoTituloYEISSN = pListaRevistas.Where(x => x.titulo.ToLower() == pTitulo.ToLower() && x.eissn == pEissn && !string.IsNullOrEmpty(pEissn)).Except(revistasMismoISSN).Except(revistasMismoTituloYEditorial).ToList();

            if (revistasMismoTituloYEditorial.Count > 1 || revistasMismoISSN.Count > 1 || revistasMismoTituloYEISSN.Count > 1)
            {
                throw new Exception("Datos duplicados.");
            }

            if (revistasMismoISSN.Count > 0 && revistasMismoTituloYEditorial.Count == 0 && revistasMismoTituloYEISSN.Count == 0)
            {
                revista = revistasMismoISSN[0];
            }
            else if (revistasMismoTituloYEditorial.Count > 0 && revistasMismoISSN.Count == 0 && revistasMismoTituloYEISSN.Count == 0)
            {
                revista = revistasMismoTituloYEditorial[0];
            }
            else if (revistasMismoTituloYEISSN.Count > 0 && revistasMismoTituloYEditorial.Count == 0 && revistasMismoISSN.Count == 0)
            {
                revista = revistasMismoTituloYEISSN[0];
            }
            else if (revistasMismoTituloYEditorial.Count > 0 || revistasMismoISSN.Count > 0 || revistasMismoTituloYEISSN.Count > 0)
            {
                // No debería entrar por aquí, porque significaría que hay una revista con mismo título/editorial y otra revista diferente con el mismo ISSN.
                // Nos quedamos con la que tenga el mismo ISSN.
                revista = revistasMismoISSN[0];
                pListaRevistas.Remove(revistasMismoTituloYEditorial[0]);
                var x = pListaRevistas.IndexOf(revistasMismoTituloYEditorial[0]);
            }

            return revista;
        }

        /// <summary>
        /// Control de errores.
        /// </summary>
        /// <param name="pListaRevistas">Lista de revistas.</param>
        /// <exception cref="Exception"></exception>
        public static void ComprobarErrores(List<Journal> pListaRevistas)
        {            
            #region --- Comprobar que no haya revistas con el mismo ISSN.
            Dictionary<string, Journal> issns = new Dictionary<string, Journal>();
            foreach (Journal revista in pListaRevistas)
            {
                if (!string.IsNullOrEmpty(revista.issn))
                {
                    try
                    {
                        issns.Add(revista.issn, revista);
                    }
                    catch
                    {
                        throw new Exception("Hay revistas con el mismo ISSN.");
                    }
                }
            }
            #endregion

            #region --- Comprobar que no haya revistas con el mismo título y editorial.
            Dictionary<string, Journal> titulos = new Dictionary<string, Journal>();
            foreach (Journal revista in pListaRevistas)
            {
                try
                {
                    titulos.Add(revista.titulo.ToLower() + "|||" + revista.publicador.ToLower(), revista);
                }
                catch
                {
                    throw new Exception("Hay revistas con el mismo título y publicador.");
                }
            }
            #endregion

            #region --- Comprobar que no haya revistas con el mismo título y editorial.
            Dictionary<string, Journal> eissns = new Dictionary<string, Journal>();
            foreach (Journal revista in pListaRevistas)
            {
                try
                {
                    eissns.Add(revista.titulo.ToLower() + "|||" + revista.eissn, revista);
                }
                catch
                {
                    throw new Exception("Hay revistas con el mismo título y EISSN.");
                }
            }
            #endregion
        }

        /// <summary>
        /// Obtiene todos los identificadores de los recursos de las revistas cargadas.
        /// </summary>
        /// <returns>Lista con los identificadores.</returns>
        private static List<string> ObtenerIDsRevistas()
        {
            List<string> idsRecursos = new List<string>();
            int limit = 10000;
            int offset = 0;
            bool salirBucle = false;

            do
            {
                // Consulta sparql.
                string select = "SELECT * WHERE { SELECT ?revista ";
                string where = $@"WHERE {{
                                ?revista a <http://w3id.org/roh/MainDocument>.
                                }} ORDER BY DESC(?revista) }} LIMIT {limit} OFFSET {offset} ";

                SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, "maindocument");
                if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                {
                    offset += limit;

                    foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                    {
                        idsRecursos.Add(fila["revista"].value);
                    }

                    if (resultadoQuery.results.bindings.Count < limit)
                    {
                        salirBucle = true;
                    }
                }
                else
                {
                    salirBucle = true;
                }

            } while (!salirBucle);

            return idsRecursos;
        }

        /// <summary>
        /// Obtiene los datos de las revistas por el id del recurso.
        /// </summary>
        /// <param name="pListaRevistasIds"></param>
        /// <returns></returns>
        private static Dictionary<string, Journal> ObtenerRevistaPorID(List<string> pListaRevistasIds)
        {
            Dictionary<string, Journal> dicResultado = new Dictionary<string, Journal>();
            List<List<string>> listaSplit = SplitList(pListaRevistasIds, 1000).ToList();

            #region --- MainDocument
            foreach (List<string> listaSpliteada in listaSplit)
            {
                int limit = 10000;
                int offset = 0;
                bool salirBucle = false;

                do
                {
                    // Consulta sparql.
                    string select = $@"SELECT * WHERE {{ SELECT ?revista ?titulo ?issn ?eissn ?editor FROM <{mResourceApi.GraphsUrl}documentformat.owl> FROM <{mResourceApi.GraphsUrl}referencesource.owl> FROM <{mResourceApi.GraphsUrl}impactindexcategory.owl> ";
                    string where = $@"WHERE {{
                                ?revista a <http://w3id.org/roh/MainDocument>. 
                                ?revista <http://w3id.org/roh/format> <{mResourceApi.GraphsUrl}items/documentformat_057>. 
                                ?revista <http://w3id.org/roh/title> ?titulo. 
                                OPTIONAL{{?revista <http://purl.org/ontology/bibo/issn> ?issn. }} 
                                OPTIONAL{{?revista <http://purl.org/ontology/bibo/eissn> ?eissn. }} 
                                OPTIONAL{{?revista <http://purl.org/ontology/bibo/editor> ?editor. }}                     
                                FILTER(?revista IN (<{string.Join(">,<", listaSpliteada)}>)) 
                                }} ORDER BY DESC(?revista) }} LIMIT {limit} OFFSET {offset} ";

                    SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, "maindocument");
                    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                    {
                        offset += limit;

                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            // Valores.
                            string revistaId = fila["revista"].value;
                            string titulo = fila["titulo"].value;
                            string issn = null;
                            string eissn = null;
                            string editor = null;
                            if (fila.ContainsKey("issn"))
                            {
                                issn = fila["issn"].value;
                            }
                            if (fila.ContainsKey("eissn"))
                            {
                                eissn = fila["eissn"].value;
                            }
                            if (fila.ContainsKey("editor"))
                            {
                                editor = fila["editor"].value;
                            }

                            // Creación del objeto.
                            Journal revista = new Journal();
                            revista.idJournal = revistaId;
                            revista.titulo = titulo;
                            revista.issn = issn;
                            revista.eissn = eissn;
                            revista.publicador = editor;
                            revista.indicesImpacto = new HashSet<IndiceImpacto>();
                            dicResultado.Add(revista.idJournal, revista);
                        }

                        if (resultadoQuery.results.bindings.Count < limit)
                        {
                            salirBucle = true;
                        }
                    }
                    else
                    {
                        salirBucle = true;
                    }

                } while (!salirBucle);
            }
            #endregion

            #region --- ImpactIndex
            foreach (List<string> listaSpliteada in listaSplit)
            {
                int limit = 10000;
                int offset = 0;
                bool salirBucle = false;

                // ImpactIndex
                do
                {
                    // Consulta sparql.
                    string select = $@"SELECT * WHERE {{ SELECT ?revista ?impactIndex ?fuente ?year ?impactIndexInYear FROM <{mResourceApi.GraphsUrl}documentformat.owl> FROM <{mResourceApi.GraphsUrl}referencesource.owl> FROM <{mResourceApi.GraphsUrl}impactindexcategory.owl> ";
                    string where = $@"WHERE {{
                                ?revista a <http://w3id.org/roh/MainDocument>.
                                ?revista <http://w3id.org/roh/format> <{mResourceApi.GraphsUrl}items/documentformat_057>. 
                                ?revista <http://w3id.org/roh/impactIndex> ?impactIndex.
                                OPTIONAL{{?impactIndex <http://w3id.org/roh/impactSource> ?fuente. }} 
                                ?impactIndex <http://w3id.org/roh/year> ?year.
                                ?impactIndex <http://w3id.org/roh/impactIndexInYear> ?impactIndexInYear.    
                                FILTER(?revista IN (<{string.Join(">,<", listaSpliteada)}>)) 
                                }} ORDER BY DESC(?revista) DESC(?impactIndex) }} LIMIT {limit} OFFSET {offset} ";

                    SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, "maindocument");
                    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                    {
                        offset += limit;

                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            string revistaId = fila["revista"].value;
                            string impactIndexId = fila["impactIndex"].value;
                            int year = Int32.Parse(fila["year"].value);
                            float impactIndexInYear = float.Parse(fila["impactIndexInYear"].value.Replace(",", "."), CultureInfo.InvariantCulture);
                            string fuente = null;

                            if (fila.ContainsKey("fuente"))
                            {
                                switch (fila["fuente"].value)
                                {
                                    case "http://gnoss.com/items/referencesource_000":
                                        fuente = "wos";
                                        break;
                                    case "http://gnoss.com/items/referencesource_010":
                                        fuente = "scopus";
                                        break;
                                    case "http://gnoss.com/items/referencesource_020":
                                        fuente = "inrecs";
                                        break;
                                }
                            }

                            dicResultado[revistaId].indicesImpacto.Add(new IndiceImpacto()
                            {
                                idImpactIndex = impactIndexId,
                                categorias = new HashSet<Categoria>(),
                                fuente = fuente,
                                indiceImpacto = impactIndexInYear,
                                anyo = year
                            });
                        }

                        if (resultadoQuery.results.bindings.Count < limit)
                        {
                            salirBucle = true;
                        }
                    }
                    else
                    {
                        salirBucle = true;
                    }

                } while (!salirBucle);
            }
            #endregion

            #region --- ImpactCategory
            foreach (List<string> listaSpliteada in listaSplit)
            {
                int limit = 10000;
                int offset = 0;
                bool salirBucle = false;

                // ImpactCategory
                do
                {
                    // Consulta sparql.
                    string select = $@"SELECT * WHERE {{ SELECT ?revista ?impactIndex ?impactCategory ?year ?nombreCategoria ?posicion ?numCategoria ?cuartil FROM <{mResourceApi.GraphsUrl}documentformat.owl> FROM <{mResourceApi.GraphsUrl}referencesource.owl> FROM <{mResourceApi.GraphsUrl}impactindexcategory.owl> ";
                    string where = $@"WHERE {{
                                ?revista a <http://w3id.org/roh/MainDocument>.
                                ?revista <http://w3id.org/roh/format> <{mResourceApi.GraphsUrl}items/documentformat_057>. 
                                ?revista <http://w3id.org/roh/impactIndex> ?impactIndex.
                                ?impactIndex <http://w3id.org/roh/year> ?year.
                                ?impactIndex <http://w3id.org/roh/impactCategory> ?impactCategory. 
                                ?impactCategory <http://w3id.org/roh/quartile> ?cuartil. 
                                ?impactCategory <http://w3id.org/roh/impactIndexCategory> ?categoria.
                                ?categoria <http://purl.org/dc/elements/1.1/title> ?nombreCategoria. 
                                FILTER (LANG(?nombreCategoria) = 'es')
                                OPTIONAL{{?impactCategory <http://w3id.org/roh/publicationPosition> ?posicion. }} 
                                OPTIONAL{{?impactCategory <http://w3id.org/roh/journalNumberInCat> ?numCategoria. }} 
                                FILTER(?revista IN (<{string.Join(">,<", listaSpliteada)}>)) 
                                }} ORDER BY DESC(?revista) DESC(?impactIndex) DESC(?impactCategory) }} LIMIT {limit} OFFSET {offset} ";

                    SparqlObject resultadoQuery = mResourceApi.VirtuosoQuery(select, where, "maindocument");
                    if (resultadoQuery != null && resultadoQuery.results != null && resultadoQuery.results.bindings != null && resultadoQuery.results.bindings.Count > 0)
                    {
                        offset += limit;

                        foreach (Dictionary<string, SparqlObject.Data> fila in resultadoQuery.results.bindings)
                        {
                            string revistaId = fila["revista"].value;
                            string impactIndexId = fila["impactIndex"].value;
                            string impactCategoryId = fila["impactIndex"].value;
                            int year = Int32.Parse(fila["year"].value);
                            string nombreCategoria = fila["nombreCategoria"].value;
                            int cuartil = Int32.Parse(fila["cuartil"].value);
                            int posicion = 0;
                            int numCategoria = 0;
                            if (fila.ContainsKey("posicion"))
                            {
                                posicion = Int32.Parse(fila["posicion"].value);
                            }
                            if (fila.ContainsKey("numCategoria"))
                            {
                                numCategoria = Int32.Parse(fila["numCategoria"].value);
                            }

                            dicResultado[revistaId].indicesImpacto.First(x => x.idImpactIndex == impactIndexId).categorias.Add(new Categoria()
                            {
                                idImpactCategory = impactCategoryId,
                                nomCategoria = nombreCategoria,
                                cuartil = cuartil,
                                posicionPublicacion = posicion,
                                numCategoria = numCategoria,
                                anyo = year
                            });

                        }

                        if (resultadoQuery.results.bindings.Count < limit)
                        {
                            salirBucle = true;
                        }
                    }
                    else
                    {
                        salirBucle = true;
                    }

                } while (!salirBucle);
            }
            #endregion

            return dicResultado;
        }

        /// <summary>
        /// Método para dividir listas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pItems">Listado</param>
        /// <param name="pSize">Tamaño</param>
        /// <returns></returns>
        public static IEnumerable<List<T>> SplitList<T>(List<T> pItems, int pSize)
        {
            for (int i = 0; i < pItems.Count; i += pSize)
            {
                yield return pItems.GetRange(i, Math.Min(pSize, pItems.Count - i));
            }
        }

        /// <summary>
        /// Comprueba que el Excel de Scopus esté bien formado.
        /// </summary>
        /// <param name="pDataSet">Dataset.</param>
        /// <param name="pAnyo">Año.</param>
        /// <returns></returns>
        public static bool ComprobarExcelScopus(DataSet pDataSet, int pAnyo)
        {
            // Comprobación de los nombres de las páginas. La página ha de estar formada con "CiteScore [AÑO]".
            DataTable tabla = pDataSet.Tables[$@"CiteScore {pAnyo}"];
            if (tabla == null)
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. El excel de Scopus no contiene el título 'CiteScore {pAnyo}'.");
                return false;
            }

            // Comprobación de los nombres de las columnas. 
            if (!tabla.Columns.Contains("Title"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'Title'.");
                return false;
            }

            if (!tabla.Columns.Contains("Publisher"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'Publisher'.");
                return false;
            }

            if (!tabla.Columns.Contains("Print ISSN"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'Print ISSN'.");
                return false;
            }

            if (!tabla.Columns.Contains("E-ISSN"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'E-ISSN'.");
                return false;
            }

            if (!tabla.Columns.Contains("SJR"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'SJR'.");
                return false;
            }

            if (!tabla.Columns.Contains("Scopus Sub-Subject Area"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'Scopus Sub-Subject Area'.");
                return false;
            }

            if (!tabla.Columns.Contains("RANK"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'RANK'.");
                return false;
            }

            if (!tabla.Columns.Contains("Rank Out Of"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'Rank Out Of'.");
                return false;
            }

            if (!tabla.Columns.Contains("Quartile"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'Quartile'.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Comprueba que el Excel genérico esté bien formado.
        /// </summary>
        /// <param name="pDataSet">Dataset.</param>
        /// <param name="pNombreHoja">Nombre de la hoja.</param>
        /// <returns></returns>
        public static bool ComprobarColumnasExcel(DataSet pDataSet, string pNombreHoja)
        {
            DataTable tabla = pDataSet.Tables[$@"{pNombreHoja}"];
            if (tabla == null)
            {
                Console.WriteLine($@"{DateTime.Now} Hoja excel inválida. El excel de no contiene el título '{pNombreHoja}'.");
                return false;
            }

            // Comprobación de los nombres de las columnas. 
            if (!tabla.Columns.Contains("TITLE"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'TITLE'.");
                return false;
            }

            if (!tabla.Columns.Contains("PUBLISHER_NAME"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'PUBLISHER_NAME'.");
                return false;
            }

            if (!tabla.Columns.Contains("ISSN"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'ISSN'.");
                return false;
            }

            if (!tabla.Columns.Contains("EISSN"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'EISSN'.");
                return false;
            }

            if (!tabla.Columns.Contains("IMPACT_FACTOR"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'IMPACT_FACTOR'.");
                return false;
            }

            if (!tabla.Columns.Contains("CATEGORY_DESCRIPTION"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'CATEGORY_DESCRIPTION'.");
                return false;
            }

            if (!tabla.Columns.Contains("RANK"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'RANK'.");
                return false;
            }

            if (!tabla.Columns.Contains("RANK_OUT_OF"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'RANK_OUT_OF'.");
                return false;
            }

            if (!tabla.Columns.Contains("QUARTILE_RANK"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'QUARTILE_RANK'.");
                return false;
            }

            if (!tabla.Columns.Contains("SOURCE"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'SOURCE'.");
                return false;
            }

            if (!tabla.Columns.Contains("YEAR"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'YEAR'.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Comprueba que el Excel de Scopus esté bien formado.
        /// </summary>
        /// <param name="pDataSet">Dataset.</param>
        /// <param name="pAnyo">Año.</param>
        /// <returns></returns>
        public static bool ComprobarExcelWOS(DataSet pDataSet, int pAnyo, string pTipo)
        {
            // Comprobación de los nombres de las páginas. La página ha de estar formada con "CiteScore [AÑO]".
            DataTable tabla = pDataSet.Tables[$@"JCR_{pTipo}_{pAnyo}"];
            if (tabla == null)
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. El excel de WoS no contiene el título 'JCR_{pTipo}_{pAnyo}'.");
                return false;
            }

            // Comprobación de los nombres de las columnas. 
            if (!tabla.Columns.Contains("TITLE"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'TITLE'.");
                return false;
            }

            if (!tabla.Columns.Contains("PUBLISHER_NAME"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'PUBLISHER_NAME'.");
                return false;
            }

            if (!tabla.Columns.Contains("ISSN"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'ISSN'.");
                return false;
            }

            if (!tabla.Columns.Contains("EISSN"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'EISSN'.");
                return false;
            }

            if (!tabla.Columns.Contains("IMPACT_FACTOR"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'IMPACT_FACTOR'.");
                return false;
            }

            if (!tabla.Columns.Contains("CATEGORY_DESCRIPTION"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'CATEGORY_DESCRIPTION'.");
                return false;
            }

            if (!tabla.Columns.Contains("CATEGORY_RANKING"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'CATEGORY_RANKING'.");
                return false;
            }

            if (!tabla.Columns.Contains("QUARTILE_RANK"))
            {
                Console.WriteLine($@"{DateTime.Now} Revista inválida. No contiene la columna 'QUARTILE_RANK'.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Lectura del excel.
        /// </summary>
        /// <param name="pRuta">Ruta del fichero.</param>
        /// <returns></returns>
        public static DataSet LecturaExcel(string pRuta)
        {
            DataSet dataSet = new DataSet();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var stream = File.Open(pRuta, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true,
                        }
                    });
                }
            }

            return dataSet;
        }

        /// <summary>
        /// Obtiene los años de las revistas. En Scopus mira las páginas y en WoS el nombre de lo directorios.
        /// </summary>
        /// <param name="pDataSet">Data Scopus.</param>
        /// <returns></returns>
        public static List<int> ObtenerAnyosColumna(DataSet pDataSet, string pNombreHoja)
        {
            HashSet<int> listaAnyosRevistas = new HashSet<int>();

            foreach (DataRow fila in pDataSet.Tables[pNombreHoja].Rows)
            {
                listaAnyosRevistas.Add(Int32.Parse(fila["YEAR"].ToString()));
            }

            List<int> listaDevolver = listaAnyosRevistas.ToList();
            listaDevolver.Sort();

            return listaDevolver;
        }

        /// <summary>
        /// Obtiene los años de las revistas. En Scopus mira las páginas y en WoS el nombre de lo directorios.
        /// </summary>
        /// <param name="pDataSetScopus">Data Scopus.</param>
        /// <returns></returns>
        public static List<int> ObtenerAnyos(DataSet pDataSetScopus)
        {
            HashSet<int> listaAnyosRevistas = new HashSet<int>();

            // Obtención años de Scopus.
            foreach (DataTable pagina in pDataSetScopus.Tables)
            {
                string nombrePagina = pagina.TableName.Trim();
                if (nombrePagina.Contains("CiteScore"))
                {
                    try
                    {
                        listaAnyosRevistas.Add(Int32.Parse(nombrePagina.Split(' ')[1]));
                    }
                    catch
                    {
                        Console.WriteLine($@"{DateTime.Now} [Scopus] Nombre de la página '{nombrePagina}' inválido...");
                        continue;
                    }
                }
            }

            // Obtención años de WoS.
            string[] directories = Directory.GetDirectories($@"C:\GNOSS\Proyectos\HerculesMA\src\Hercules.MA.Journals\Hercules.MA.Journals\Revistas\wos\").Select(Path.GetFileName).ToArray();
            foreach (string anyo in directories)
            {
                try
                {
                    listaAnyosRevistas.Add(Int32.Parse(anyo));
                }
                catch
                {
                    Console.WriteLine($@"{DateTime.Now} [WoS] Nombre de la página '{anyo}' inválido...");
                    continue;
                }
            }

            List<int> listaDevolver = listaAnyosRevistas.ToList();
            listaDevolver.Sort();

            return listaDevolver;
        }

        /// <summary>
        /// Permite crear el objeto a cargar de las revistas.
        /// </summary>
        /// <param name="pListaRevistas">Listado de revistas a cargar.</param>
        /// <param name="pListaRecursos">Listado de recursos a cargar.</param>
        /// <returns></returns>
        public static void ObtenerRevistas(List<Journal> pListaRevistas, List<ComplexOntologyResource> pListaRecursos)
        {
            foreach (Journal revista in pListaRevistas)
            {
                MaindocumentOntology.MainDocument revistaCargar = new MaindocumentOntology.MainDocument();
                revistaCargar.Roh_title = revista.titulo;
                revistaCargar.Bibo_issn = revista.issn;
                revistaCargar.Bibo_eissn = revista.eissn;
                revistaCargar.Bibo_editor = revista.publicador;
                revistaCargar.IdRoh_format = $@"{mResourceApi.GraphsUrl}items/documentformat_057";
                revistaCargar.Roh_impactIndex = new List<MaindocumentOntology.ImpactIndex>();
                foreach (IndiceImpacto indice in revista.indicesImpacto)
                {
                    MaindocumentOntology.ImpactIndex indiceCargar = new MaindocumentOntology.ImpactIndex();
                    switch (indice.fuente)
                    {
                        case "wos":
                            indiceCargar.IdRoh_impactSource = $@"{mResourceApi.GraphsUrl}items/referencesource_000";
                            break;
                        case "scopus":
                            indiceCargar.IdRoh_impactSource = $@"{mResourceApi.GraphsUrl}items/referencesource_010";
                            break;
                        case "inrecs":
                            indiceCargar.IdRoh_impactSource = $@"{mResourceApi.GraphsUrl}items/referencesource_020";
                            break;
                    }
                    indiceCargar.Roh_impactIndexInYear = indice.indiceImpacto;
                    indiceCargar.Roh_year = indice.anyo;

                    indiceCargar.Roh_impactCategory = new List<MaindocumentOntology.ImpactCategory>();
                    foreach (Categoria categoria in indice.categorias)
                    {
                        MaindocumentOntology.ImpactCategory categoriaCargar = new MaindocumentOntology.ImpactCategory();
                        categoriaCargar.Roh_title = categoria.nomCategoria;
                        categoriaCargar.Roh_publicationPosition = categoria.posicionPublicacion;
                        categoriaCargar.Roh_journalNumberInCat = categoria.numCategoria;
                        categoriaCargar.Roh_quartile = categoria.cuartil;
                        indiceCargar.Roh_impactCategory.Add(categoriaCargar);
                    }

                    revistaCargar.Roh_impactIndex.Add(indiceCargar);
                }

                //Creamos el recurso.
                ComplexOntologyResource resource = revistaCargar.ToGnossApiResource(mResourceApi, null);
                pListaRecursos.Add(resource);
            }
        }

        /// <summary>
        /// Permite cargar los recursos.
        /// </summary>
        /// <param name="pListaRecursosCargar">Lista de recursos a cargar.</param>
        private static void CargarDatos(List<ComplexOntologyResource> pListaRecursosCargar)
        {
            // Carga.
            Parallel.ForEach(pListaRecursosCargar, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, recursoCargar =>
            {
                int numIntentos = 0;
                while (!recursoCargar.Uploaded)
                {
                    numIntentos++;

                    if (numIntentos > MAX_INTENTOS)
                    {
                        break;
                    }

                    if (pListaRecursosCargar.Last() == recursoCargar)
                    {
                        mResourceApi.LoadComplexSemanticResource(recursoCargar, false, true);
                    }
                    else
                    {
                        mResourceApi.LoadComplexSemanticResource(recursoCargar);
                    }

                    if (!recursoCargar.Uploaded)
                    {
                        Thread.Sleep(1000 * numIntentos);
                    }
                }
            });
        }

        /// <summary>
        /// Permite modificar los recursos.
        /// </summary>
        /// <param name="pListaRecursosModificar">Lista de recursos a modificar.</param>
        private static void ModificarDatos(List<ComplexOntologyResource> pListaRecursosModificar)
        {
            // Modificación.
            Parallel.ForEach(pListaRecursosModificar, new ParallelOptions { MaxDegreeOfParallelism = NUM_HILOS }, recursoModificar =>
            {
                int numIntentos = 0;
                while (!recursoModificar.Modified)
                {
                    numIntentos++;

                    if (numIntentos > MAX_INTENTOS)
                    {
                        break;
                    }

                    if (pListaRecursosModificar.Last() == recursoModificar)
                    {
                        mResourceApi.ModifyComplexOntologyResource(recursoModificar, false, true);
                    }
                    else
                    {
                        mResourceApi.ModifyComplexOntologyResource(recursoModificar, false, false);
                    }

                    if (!recursoModificar.Modified)
                    {
                        Thread.Sleep(1000 * numIntentos);
                    }
                }
            });
        }
    }
}
