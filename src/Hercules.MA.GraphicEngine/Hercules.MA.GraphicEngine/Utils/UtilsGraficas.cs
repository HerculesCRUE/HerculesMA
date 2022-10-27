using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using Hercules.MA.GraphicEngine.Models;
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

namespace Hercules.MA.GraphicEngine.Utils
{
    public class UtilsGraficas
    {
        public static List<string> GetFiltrosBarras(Grafica pGrafica, Dimension itemGrafica, string pFiltroFacetas, string pFiltroBase, List<string> pListaDates)
        {
            // Determina si en el filtro contiene '=' para tratarlo de manera especial.
            bool filtroEspecial = IsFiltroEspecial(itemGrafica);
            // Filtro de página.
            List<string> filtros = new List<string>();

            if (!string.IsNullOrEmpty(pGrafica.config.reciproco))
            {
                filtros.AddRange(ObtenerFiltros(new List<string>() { pGrafica.config.ejeX }, "ejeX", null, pGrafica.config.reciproco));
            }
            else
            {
                filtros.AddRange(ObtenerFiltros(new List<string>() { pGrafica.config.ejeX }, "ejeX"));
            }
            filtros.AddRange(ObtenerFiltros(new List<string>() { pFiltroBase }));
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
                                filtros.AddRange(ObtenerFiltros(new List<string>() { filtro.Split("(((")[0] }, pListaDates: pListaDates, pReciproco: filtro.Split("(((")[1]));
                            }
                            else
                            {
                                filtros.AddRange(ObtenerFiltros(new List<string>() { filtro }, pListaDates: pListaDates));
                            }
                        }
                    }
                    else
                    {
                        filtros.AddRange(ObtenerFiltros(new List<string>() { pFiltroFacetas.Split("(((")[0] }, pListaDates: pListaDates, pReciproco: pFiltroFacetas.Split("(((")[1]));
                    }
                }
                else
                {
                    filtros.AddRange(ObtenerFiltros(new List<string>() { pFiltroFacetas }, pListaDates: pListaDates));
                }
            }
            if (filtroEspecial)
            {
                filtros.AddRange(ObtenerFiltros(new List<string>() { itemGrafica.filtro }, "aux"));
            }
            else if (!string.IsNullOrEmpty(itemGrafica.filtro))
            {
                filtros.AddRange(ObtenerFiltros(new List<string>() { itemGrafica.filtro }));
            }

            return filtros;
        }
        public static bool IsFiltroEspecial(Dimension itemGrafica) {
            // Determina si en el filtro contiene '=' para tratarlo de manera especial.
            return !string.IsNullOrEmpty(itemGrafica.filtro) && !itemGrafica.filtro.Contains("=");
        }

        /// <summary>
        /// Splitea los filtros para tratarlos.
        /// </summary>
        /// <param name="pListaFiltros">Listado de filtros.</param>
        /// <param name="pNombreVar">Nombre a poner a la última variable.</param>
        /// <param name="pListaDates">Lista de valores que corresponden a fechas</param>
        /// <param name="pReciproco">Valor de la propiedad recíproca</param>
        /// <returns></returns>
        public static List<string> ObtenerFiltros(List<string> pListaFiltros, string pNombreVar = null, List<string> pListaDates = null, string pReciproco = null)
        {
            // Split por filtro.
            List<string> listaAux = new List<string>();
            foreach (string filtro in pListaFiltros)
            {
                // --- ÑAPA
                string aux = filtro.Replace(" & ", "|||");
                string[] array = aux.Split("&", StringSplitOptions.RemoveEmptyEntries);
                List<string> lista = array.Select(x => x.Replace("|||", " & ")).ToList();
                listaAux.AddRange(lista);
            }

            List<string> filtrosQuery = new List<string>();

            // Split por salto de ontología.
            int i = 0;
            foreach (string item in listaAux)
            {
                bool isDate = false;
                if (pListaDates != null && pListaDates.Any() && pListaDates.Contains(item.Split("=")[0]))
                {
                    isDate = true;
                }

                filtrosQuery.Add(TratarParametros(item, "?s", i, pNombreVar, isDate, pReciproco));

                i += 10;
            }

            return filtrosQuery;
        }

        /// <summary>
        /// Según el tipo de parametros, los trata de una manera u otra para el filtro.
        /// </summary>
        /// <param name="pFiltro">Filtro a tratar.</param>
        /// <param name="pVarAnterior">Sujeto.</param>
        /// <param name="pAux">Iterador incremental.</param>
        /// <param name="pNombreVar">Nombre de la última variable.</param>
        /// <param name="pIsDate">Indica si es una fecha</param>
        /// <param name="pReciproco">Indica si la consulta es recíproca</param>
        /// <returns></returns>
        public static string TratarParametros(string pFiltro, string pVarAnterior, int pAux, string pNombreVar = null, bool pIsDate = false, string pReciproco = null)
        {
            StringBuilder filtro = new StringBuilder();
            string[] filtros = pFiltro.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
            int i = 0;

            string[] filtrosReciproco = null;
            // TODO Revisar con más casos o ejemplos para ver si funciona totalmente bien
            if (!string.IsNullOrEmpty(pReciproco))
            {
                filtrosReciproco = pReciproco.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                filtro.Append($@"?aux {filtrosReciproco[0].Split('=').FirstOrDefault()} {filtrosReciproco[0].Split('=').LastOrDefault()}. ");
                int j = 0;
                pVarAnterior = "?aux";
                foreach (string filtroReciproco in filtrosReciproco)
                {
                    j++;
                    if (j == 1)
                    {
                        continue;
                    }
                    int pAuxR = pAux;
                    if (!filtroReciproco.Contains("="))
                    {
                        string varActual = $@"?{filtroReciproco.Substring(filtroReciproco.IndexOf(":") + 1)}{pAuxR}";
                        filtro.Append($@"{pVarAnterior} ");
                        filtro.Append($@"{filtroReciproco} ");
                        // Si es el último, le asignamos el nombre que queramos.
                        if (j == filtrosReciproco.Length)
                        {
                            filtro.Append($@"?s. ");
                        }
                        else
                        {
                            filtro.Append($@"{varActual}. ");
                        }
                        pVarAnterior = varActual;
                        pAuxR++;
                    }
                }
                pVarAnterior = "?aux";
            }
            foreach (string parteFiltro in filtros)
            {
                i++;
                if (!parteFiltro.Contains("="))
                {
                    string varActual = $@"?{parteFiltro.Substring(parteFiltro.IndexOf(":") + 1)}{pAux}";
                    filtro.Append($@"{pVarAnterior} ");
                    filtro.Append($@"{parteFiltro} ");
                    // Si es el último, le asignamos el nombre que queramos.
                    if (i == filtros.Length && !string.IsNullOrEmpty(pNombreVar))
                    {
                        filtro.Append($@"?{pNombreVar}. ");
                    }
                    else
                    {
                        filtro.Append($@"{varActual}. ");
                    }
                    pVarAnterior = varActual;
                    pAux++;
                }
                else
                {
                    string varActual = $@"{parteFiltro.Split("=")[1]}";
                    if (varActual.StartsWith("'"))
                    {
                        filtro.Append($@"{pVarAnterior} ");
                        filtro.Append($@"{parteFiltro.Split("=")[0]} ");

                        if (pNombreVar != null)
                        {
                            filtro.Append($@"?{pNombreVar}. ");
                        }
                        else
                        {
                            filtro.Append($@"{varActual}. ");
                        }
                    }
                    else if (pIsDate && (varActual.Contains("-") || varActual.Equals("lastyear") || varActual.Equals("fiveyears")))
                    {
                        string fechaInicio = "";
                        string fechaFin = "";
                        string varActualAux = $@"?{parteFiltro.Split("=")[0].Substring(parteFiltro.IndexOf(":") + 1)}{pAux}";
                        if (varActual.Contains("-"))
                        {
                            fechaInicio = varActual.Split("-")[0];
                            fechaFin = varActual.Split("-")[1];
                        }
                        else if (varActual.Equals("lastyear"))
                        {
                            fechaInicio = DateTime.Now.Year.ToString();
                            fechaFin = DateTime.Now.Year.ToString();
                        }
                        else if (varActual.Equals("fiveyears"))
                        {
                            fechaInicio = (DateTime.Now.Year - 4).ToString();
                            fechaFin = DateTime.Now.Year.ToString();
                        }
                        filtro.Append($@"{pVarAnterior} ");
                        filtro.Append($@"{parteFiltro.Split("=")[0]} ");
                        filtro.Append($@"{varActualAux}. ");
                        filtro.Append($@"FILTER({varActualAux} >= {fechaInicio} && {varActualAux} <= {fechaFin}) ");
                    }
                    else
                    {
                        // Si el valor es númerico, se le asigna con el FILTER.
                        string varActualAux = $@"?{parteFiltro.Split("=")[0].Substring(parteFiltro.IndexOf(":") + 1)}{pAux}";
                        filtro.Append($@"{pVarAnterior} ");
                        filtro.Append($@"{parteFiltro.Split("=")[0]} ");
                        filtro.Append($@"{varActualAux}. ");

                        // Si es un tesauro.
                        if (varActual.StartsWith($@"http://"))
                        {
                            filtro.Append($@"FILTER({varActualAux} = <{varActual}>) ");
                        }
                        else
                        {
                            filtro.Append($@"FILTER({varActualAux} = {varActual}) ");
                        }
                    }

                }
            }

            return filtro.ToString();
        }

    }
}
