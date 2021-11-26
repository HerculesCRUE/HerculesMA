using Gnoss.ApiWrapper.Model;
using Gnoss.ApiWrapper.ApiModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gnoss.ApiWrapper;
using System.Text;
using System.Web;
using Hercules.MA.ServicioExterno.Models.DataFechas;

namespace Hercules.MA.ServicioExterno.Controllers.Utilidades
{
    public class UtilidadesAPI
    {
        public static string GetValorFilaSparqlObject(Dictionary<string, SparqlObject.Data> pFila, string pParametro)
        {
            if (pFila.ContainsKey(pParametro) && !string.IsNullOrEmpty(pFila[pParametro].value))
            {
                return pFila[pParametro].value;
            }

            return null;
        }

        public static string GetValorFilaSparqlDia(Dictionary<string, SparqlObject.Data> pFila, string pParametro)
        {
            if (pFila.ContainsKey(pParametro) && !string.IsNullOrEmpty(pFila[pParametro].value))
            {
                return pFila[pParametro].value.Substring(0, 8) + "000000";
            }

            return null;
        }

        public static bool ModificarTriplesRecurso(ResourceApi pResourceApi, Guid pRecursoID, List<TriplesToModify> pTriples)
        {
            List<TriplesToModify> triplesInsertar = new List<TriplesToModify>();

            foreach (TriplesToModify triple in pTriples)
            {
                if (triple.NewValue == "")
                {
                    triple.NewValue = null;
                }

                if (triple.OldValue == "")
                {
                    triple.OldValue = null;
                }

                if (triple.OldValue != triple.NewValue)
                {
                    triplesInsertar.Add(triple);
                }
            }

            Dictionary<Guid, List<TriplesToModify>> dicTriplesModificar = new Dictionary<Guid, List<TriplesToModify>>();
            dicTriplesModificar.Add(pRecursoID, triplesInsertar);
            Dictionary<Guid, bool> dicInsertado = pResourceApi.ModifyPropertiesLoadedResources(dicTriplesModificar);

            return (dicInsertado != null && dicInsertado.ContainsKey(pRecursoID) && dicInsertado[pRecursoID]);
        }


        /// <summary>
        /// Crea los filtros en formato sparql.
        /// </summary>
        /// <param name="pDicFiltros">Diccionario con los filtros.</param>
        /// <param name="pVarAnterior">Variable anterior para no repetir nombre.</param>
        /// <param name="pAux">Variable auxiliar para que no se repitan los nombres.</param>
        /// <param name="pVarFechaInicio">Variable auxiliar para la fecha inicio.</param>
        /// <param name="pVarFechaFin">Variable auxiliar para la fecha fin.</param>
        /// <returns>String con los filtros creados.</returns>
        public static string CrearFiltros(Dictionary<string, List<string>> pDicFiltros, string pVarAnterior, ref int pAux)
        {
            // Filtros de fechas.
            List<string> filtrosFecha = new List<string>();
            filtrosFecha.Add("dct:issued");
            filtrosFecha.Add("vivo:start");
            filtrosFecha.Add("vivo:end");

            // Filtros de eteros.
            List<string> filtrosEnteros = new List<string>();
            filtrosEnteros.Add("roh:publicationsNumber");
            filtrosEnteros.Add("roh:projectsNumber");
            filtrosEnteros.Add("roh:publicationsNumber");

            // Filtros de inversas.
            Dictionary<string, int> filtrosReciprocos = new Dictionary<string, int>();
            filtrosReciprocos.Add("foaf:member@@@roh:roleOf@@@roh:title", 2);

            string varInicial = pVarAnterior;
            string pVarAnteriorAux = string.Empty;

            if (pDicFiltros != null && pDicFiltros.Count > 0)
            {
                StringBuilder filtro = new StringBuilder();

                foreach (KeyValuePair<string, List<string>> item in pDicFiltros)
                {
                    if (!filtrosReciprocos.ContainsKey(item.Key))
                    {
                        foreach (string parteFiltro in item.Key.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string varActual = $@"?{parteFiltro.Substring(parteFiltro.IndexOf(":") + 1)}{pAux}";
                            filtro.Append($@"{pVarAnterior} ");
                            filtro.Append($@"{parteFiltro} ");
                            filtro.Append($@"{varActual}. ");
                            pVarAnterior = varActual;
                            pAux++;
                        }
                    }
                    else
                    {
                        int index = filtrosReciprocos[item.Key];
                        pVarAnterior = "?varAuxiliar";
                        pVarAnteriorAux = pVarAnterior;
                        foreach (string parteFiltro in item.Key.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if ((pAux + 1) < index)
                            {
                                string varActual = $@"?{parteFiltro.Substring(parteFiltro.IndexOf(":") + 1)}{pAux}";
                                filtro.Append($@"{pVarAnterior} ");
                                filtro.Append($@"{parteFiltro} ");
                                filtro.Append($@"{varActual}. ");
                                pVarAnterior = varActual;
                                pAux++;
                            }
                            else if ((pAux + 1) == index)
                            {
                                string varActual = $@"?{parteFiltro.Substring(parteFiltro.IndexOf(":") + 1)}{pAux}";
                                filtro.Append($@"{pVarAnterior} ");
                                filtro.Append($@"{parteFiltro} ");
                                filtro.Append($@"{varInicial}. ");
                                pAux++;
                            }
                            else
                            {
                                filtro.Append($@"{pVarAnteriorAux} ");
                                filtro.Append($@"{parteFiltro} ");
                                filtro.Append($@"'{HttpUtility.UrlDecode(item.Value[0])}'. ");
                            }
                        }
                    }

                    // Filtro de fechas.
                    if (filtrosFecha.Contains(item.Key))
                    {
                        foreach (string fecha in item.Value)
                        {
                            filtro.Append($@"FILTER({pVarAnterior} >= {fecha.Split('-')[0]}000000) ");
                            filtro.Append($@"FILTER({pVarAnterior} <= {fecha.Split('-')[1]}000000) ");
                        }
                    }
                    else if(filtrosEnteros.Contains(item.Key))
                    {
                        string valorFiltro = string.Empty;
                        
                        foreach (string valor in item.Value)
                        {
                            valorFiltro += $@",{valor}";
                        }

                        if (valorFiltro.Length > 0)
                        {
                            valorFiltro = valorFiltro.Substring(1);
                        }

                        if (!filtrosReciprocos.ContainsKey(item.Key))
                        {
                            filtro.Append($@"FILTER({pVarAnterior} IN ({HttpUtility.UrlDecode(valorFiltro)})) ");
                        }
                    }
                    else
                    {
                        string valorFiltro = string.Empty;
                        foreach (string valor in item.Value)
                        {
                            Uri uriAux = null;
                            bool esUri = Uri.TryCreate(valor, UriKind.Absolute, out uriAux);
                            if (esUri)
                            {
                                valorFiltro += $@",<{valor}>";
                            } else if(valor.All(char.IsNumber))
                            {
                                valorFiltro += $@",{valor}";
                            }
                            else
                            {
                                //MultiIdioma.
                                if (valor.Length > 3 && valor[valor.Length - 3] == '@')
                                {
                                    valorFiltro += $@",'{valor.Substring(0,valor.Length-3)}'{valor.Substring(valor.Length - 3)}";
                                }
                                else
                                {
                                    valorFiltro += $@",'{valor}'";
                                }
                            }
                        }

                        if (valorFiltro.Length > 0)
                        {
                            valorFiltro = valorFiltro.Substring(1);
                        }

                        if (!filtrosReciprocos.ContainsKey(item.Key))
                        {
                            filtro.Append($@"FILTER({pVarAnterior} IN ({HttpUtility.UrlDecode(valorFiltro)})) ");
                        }                      
                    }
                    pVarAnterior = varInicial;
                }
                return filtro.ToString();
            }
            return string.Empty;
        }


        /// <summary>
        /// Rellenar los años faltantes del diccionario.
        /// </summary>
        /// <param name="pDicResultados">Diccionario a rellenar.</param>
        /// <param name="pFechaInicial">Primer año.</param>
        /// <param name="pFechaFinal">Último año.</param>
        public static void RellenarAnys(Dictionary<string, int> pDicResultados, string pFechaInicial, string pFechaFinal)
        {
            // Fecha inicial.
            int dia1 = int.Parse(pFechaInicial.Substring(6, 2));
            int mes1 = int.Parse(pFechaInicial.Substring(4, 2));
            int anio1 = int.Parse(pFechaInicial.Substring(0, 4));
            DateTime fecha1 = new DateTime(anio1, mes1, dia1);

            // Fecha final.
            int dia2 = int.Parse(pFechaFinal.Substring(6, 2));
            int mes2 = int.Parse(pFechaFinal.Substring(4, 2));
            int anio2 = int.Parse(pFechaFinal.Substring(0, 4));
            DateTime fecha2 = new DateTime(anio2, mes2, dia2);

            while (fecha1 <= fecha2)
            {
                // Hay que rellenar con los años intermedios.
                string fechaString = $@"{fecha1.ToString("yyyyMMdd")}010000";
                if (!pDicResultados.ContainsKey(fechaString))
                {
                    pDicResultados.Add(fechaString, 0);
                }
                fecha1 = fecha1.AddYears(1);
            }
        }

        /// <summary>
        /// Rellenar los años faltantes del diccionario.
        /// </summary>
        /// <param name="pDicResultados">Diccionario a rellenar.</param>
        /// <param name="pFechaInicial">Primer año.</param>
        /// <param name="pFechaFinal">Último año.</param>
        public static void RellenarAnys(Dictionary<string, DataFechas> pDicResultados, string pFechaInicial, string pFechaFinal)
        {
            // Fecha inicial.
            int dia1 = 01;
            int mes1 = 01;
            int anio1 = int.Parse(pFechaInicial);
            DateTime fecha1 = new DateTime(anio1, mes1, dia1);

            // Fecha final.
            int dia2 = 01;
            int mes2 = 01;
            int anio2 = int.Parse(pFechaFinal);
            DateTime fecha2 = new DateTime(anio2, mes2, dia2);

            while (fecha1 <= fecha2)
            {
                // Hay que rellenar con los años intermedios.
                string fechaString = $@"{fecha1.ToString("yyyy")}";
                if (!pDicResultados.ContainsKey(fechaString))
                {
                    DataFechas data = new();
                    data.numProyectosInicio = 0;
                    data.numProyectosFin = 0;
                    pDicResultados.Add(fechaString, data);
                }
                fecha1 = fecha1.AddYears(1);
            }
        }

        /// <summary>
        /// Obtiene los filtros por los parámetros de la URL.
        /// </summary>
        /// <param name="pParametros">String de filtros.</param>
        /// <returns>Diccionario de filtros.</returns>
        public static Dictionary<string, List<string>> ObtenerParametros(string pParametros)
        {
            if (!string.IsNullOrEmpty(pParametros))
            {
                Dictionary<string, List<string>> dicFiltros = new Dictionary<string, List<string>>();

                // Agregamos al diccionario los filtros.
                foreach (string filtro in pParametros.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string keyFiltro = filtro.Split('=')[0];
                    string valorFiltro = filtro.Split('=')[1];
                    if (dicFiltros.ContainsKey(keyFiltro))
                    {
                        dicFiltros[keyFiltro].Add(valorFiltro);
                    }
                    else
                    {
                        dicFiltros.Add(keyFiltro, new List<string> { valorFiltro });
                    }
                }

                return dicFiltros;
            }

            return null;
        }



        /// <summary>
        /// Agrupa el número de publicaciones por año.
        /// </summary>
        /// <param name="pDicResultados">Diccionario a agrupar.</param>
        /// <returns>Diccionario con los datos agrupados por año.</returns>
        public static Dictionary<string, int> AgruparAnys(Dictionary<string, int> pDicResultados)
        {
            Dictionary<string, int> pDicAgrupado = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> item in pDicResultados)
            {
                string anio = ExtraerAny(item.Key);
                int numPublicaciones = item.Value;

                if (!pDicAgrupado.ContainsKey(anio))
                {
                    pDicAgrupado.Add(anio, numPublicaciones);
                }
                else
                {
                    pDicAgrupado[anio] += numPublicaciones;
                }
            }
            return pDicAgrupado;
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

        /// <summary>
        /// Obtiene el año de un string.
        /// </summary>
        /// <param name="pFecha">String del año.</param>
        /// <returns>Año en string.</returns>
        public static string ExtraerAny(string pFecha)
        {
            return pFecha.Substring(0, 4);
        }

        /// <summary>
        /// Obtiene las llaves de un diccionario.
        /// </summary>
        /// <param name="pDic"></param>
        /// <returns>Lista de llaves.</returns>
        public static List<string> GetKeysList(Dictionary<string, int> pDic)
        {
            return pDic.Keys.ToList();
        }

        /// <summary>
        /// Obtiene los valores de un diccionario.
        /// </summary>
        /// <param name="pDic"></param>
        /// <returns>Lista de valores.</returns>
        public static List<int> GetValuesList(Dictionary<string, int> pDic)
        {
            return pDic.Values.ToList();
        }

        /// <summary>
        /// Permite crear la lista con los colores.
        /// </summary>
        /// <param name="pSize">Tamaño de la lista.</param>
        /// <param name="pColorHex">Colores asignados.</param>
        /// <returns>Lista con los colores.</returns>
        public static List<string> CrearListaColores(int pSize, string pColorHex)
        {
            List<string> listaColores = new List<string>();
            for (int i = 0; i < pSize; i++)
            {
                listaColores.Add(pColorHex);
            }
            return listaColores;
        }


    }
}
