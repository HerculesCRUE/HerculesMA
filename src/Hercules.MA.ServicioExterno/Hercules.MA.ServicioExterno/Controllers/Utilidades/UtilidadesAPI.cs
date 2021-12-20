﻿using Gnoss.ApiWrapper.Model;
using Gnoss.ApiWrapper.ApiModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gnoss.ApiWrapper;
using System.Text;
using System.Web;
using Hercules.MA.ServicioExterno.Models;

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
            filtrosEnteros.Add("roh:citationCount");

            // Filtros de inversas.
            Dictionary<string, int> filtrosReciprocos = new Dictionary<string, int>();
            filtrosReciprocos.Add("foaf:member@@@roh:roleOf@@@roh:title", 2);

            //Filtros personalizados
            Dictionary<string, string> filtrosPersonalizados = new Dictionary<string, string>();
            filtrosPersonalizados.Add("searchColaboradoresPorGrupo",
                        @$"
                            {{
                                SELECT DISTINCT {pVarAnterior}
	                            WHERE 
	                            {{	
                                    {pVarAnterior} a 'person'	
		                            {{
			                            {{
				                            #Documentos
				                            SELECT *
				                            WHERE {{
					                            ?documento <http://w3id.org/roh/isProducedBy> <http://gnoss/[PARAMETRO]>.
					                            ?documento a 'document'.
					                            ?documento <http://purl.org/ontology/bibo/authorList> ?listaAutores.
					                            ?listaAutores <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> {pVarAnterior}.
				                            }}
			                            }} 
			                            UNION 
			                            {{
				                            #Proyectos
				                            SELECT *
				                            WHERE {{
					                            ?proy <http://w3id.org/roh/publicGroupList> <http://gnoss/[PARAMETRO]>.
					                            ?proy a 'project'.
					                            ?proy ?propRol ?role.
					                            FILTER(?propRol in (<http://vivoweb.org/ontology/core#relates>,<http://w3id.org/roh/mainResearchers>))
					                            ?role <http://www.w3.org/1999/02/22-rdf-syntax-ns#member> {pVarAnterior}.
				                            }}
			                            }}
		                            }}		
		                            MINUS
		                            {{
			                            {pVarAnterior} <http://vivoweb.org/ontology/core#relates> <http://gnoss/[PARAMETRO]>
		                            }}
	                            }}
                            }}
                        ");

            filtrosPersonalizados.Add("searchDocumentosRelacionadosConProyecto",
                        @$"
                            {{
                                SELECT DISTINCT {pVarAnterior}
	                            WHERE 
	                            {{	
                                    {pVarAnterior} a 'person'	
		                            FILTER(?item=<http://gnoss/[PARAMETRO]>)
                                    {pVarAnterior} <http://w3id.org/roh/project> ?item.
	                            }}
                            }}
                        ");


            string varInicial = pVarAnterior;
            string pVarAnteriorAux = string.Empty;

            if (pDicFiltros != null && pDicFiltros.Count > 0)
            {
                StringBuilder filtro = new StringBuilder();

                foreach (KeyValuePair<string, List<string>> item in pDicFiltros)
                {
                    if (filtrosPersonalizados.ContainsKey(item.Key))
                    {
                        filtro.Append(filtrosPersonalizados[item.Key].Replace("[PARAMETRO]", item.Value.First()));
                    }
                    else
                    {

                        foreach (string valorFiltroIn in item.Value)
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
                                        //filtro.Append($@"'{HttpUtility.UrlDecode(item.Value[0])}'. ");
                                        filtro.Append($@"'{HttpUtility.UrlDecode(valorFiltroIn)}'. ");
                                    }
                                }
                            }

                            // Filtro de fechas.
                            if (filtrosFecha.Contains(item.Key)) 
                            {
                                //foreach (string fecha in item.Value)
                                //{
                                //    filtro.Append($@"FILTER({pVarAnterior} >= {fecha.Split('-')[0]}000000) ");
                                //    filtro.Append($@"FILTER({pVarAnterior} <= {fecha.Split('-')[1]}000000) ");
                                //}
                                filtro.Append($@"FILTER({pVarAnterior} >= {valorFiltroIn.Split('-')[0]}000000) ");
                                filtro.Append($@"FILTER({pVarAnterior} <= {valorFiltroIn.Split('-')[1]}000000) ");
                            }
                            else if (filtrosEnteros.Contains(item.Key))
                            {

                                // Comprueba si es un rango
                                if (valorFiltroIn.Contains('-'))
                                {
                                    filtro.Append($@"FILTER({pVarAnterior} >= {valorFiltroIn.Split('-')[0]}) ");
                                    filtro.Append($@"FILTER({pVarAnterior} <= {valorFiltroIn.Split('-')[1]}) ");
                                }
                                else 
                                {
                                    // Si no es un rango...

                                    string valorFiltro = string.Empty;
                                    valorFiltro += $@",{valorFiltroIn}";

                                    if (valorFiltro.Length > 0)
                                    {
                                        valorFiltro = valorFiltro.Substring(1);
                                    }

                                    if (!filtrosReciprocos.ContainsKey(item.Key))
                                    {
                                        filtro.Append($@"FILTER({pVarAnterior} IN ({HttpUtility.UrlDecode(valorFiltro)})) ");
                                    }
                                }
                                
                            }
                            else
                            {
                                string valorFiltro = string.Empty;
                                //foreach (string valor in item.Value)
                                //{
                                Uri uriAux = null;
                                bool esUri = Uri.TryCreate(valorFiltroIn, UriKind.Absolute, out uriAux);
                                if (esUri)
                                {
                                    valorFiltro += $@",<{valorFiltroIn}>";
                                }
                                else
                                {
                                    //MultiIdioma.
                                    if (valorFiltroIn.Length > 3 && valorFiltroIn[valorFiltroIn.Length - 3] == '@')
                                    {
                                        valorFiltro += $@",'{valorFiltroIn.Substring(0, valorFiltroIn.Length - 3)}'{valorFiltroIn.Substring(valorFiltroIn.Length - 3)}";
                                    }
                                    else
                                    {
                                        valorFiltro += $@",'{valorFiltroIn}'";
                                    }
                                }
                                //}

                                if (valorFiltro.Length > 0)
                                {
                                    valorFiltro = valorFiltro.Substring(1);
                                }

                                if (!filtrosReciprocos.ContainsKey(item.Key))
                                {
                                    filtro.Append($@"FILTER({pVarAnterior} IN ({HttpUtility.UrlDecode(valorFiltro.Replace("+", "%2B"))})) ");
                                }
                            }
                            pVarAnterior = varInicial;
                        }
                    }
                }
                return filtro.ToString();
            }
            return string.Empty;
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
                pParametros = pParametros.Trim().Trim('#');
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
            }
           

            return null;
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

        /// <summary>
        /// Mediante el ID del recurso en el grafo de búsqueda a través de su ID en el grafo de la ontología
        /// </summary>
        /// <param name="pRsourceApi">API</param>
        /// <param name="pIdOntologia">ID del grafo de la ontología.</param>
        /// <returns>ID del grafo de búsqueda.</returns>
        public static string ObtenerIdBusqueda(ResourceApi pRsourceApi, string pIdOntologia)
        {
            Guid idCorto = pRsourceApi.GetShortGuid(pIdOntologia);
            return $@"http://gnoss/{idCorto.ToString().ToUpper()}";
        }


        public static void ProcessRelations(string pNombreRelacion, Dictionary<string, HashSet<string>> pItems, ref Dictionary<string, List<DataQueryRelaciones>> pDicRelaciones)
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
                    if (itemA != itemB)
                    {
                        if (string.Compare(itemA, itemB, StringComparison.OrdinalIgnoreCase) > 0)
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
                }
                if (dataQueryRelaciones.idRelacionados.Count == 0)
                {
                    pDicRelaciones[itemA].Remove(dataQueryRelaciones);
                }
            }
        }
    }
}
