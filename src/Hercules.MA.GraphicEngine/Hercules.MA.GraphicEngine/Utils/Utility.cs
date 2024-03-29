﻿using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Hercules.MA.GraphicEngine.Models;
using Hercules.MA.GraphicEngine.Models.Graficas;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Hercules.MA.GraphicEngine.Utils
{
    public static class Utility
    {
        private static readonly string mPrefijos = string.Join(" ", JsonConvert.DeserializeObject<List<string>>(File.ReadAllText($@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config{Path.DirectorySeparatorChar}configJson{Path.DirectorySeparatorChar}prefijos.json")));
        private static readonly ResourceApi mResourceApi = new ($@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config{Path.DirectorySeparatorChar}ConfigOAuth{Path.DirectorySeparatorChar}OAuthV3.config");
        private static readonly CommunityApi mCommunityApi = new ($@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config{Path.DirectorySeparatorChar}ConfigOAuth{Path.DirectorySeparatorChar}OAuthV3.config");
        private static readonly Guid mCommunityID = mCommunityApi.GetCommunityId();

        /// <summary>
        /// Devuelve un string con el color en hexadecimal
        /// Opciones:
        /// -1 Verde
        /// -2 Amarillo
        /// -3 Naranja
        /// -4 Rojo
        /// En otro caso devuelve nulo
        /// </summary>
        /// <param name="opcion">Opción a seleccionar</param>
        /// <returns>String del color en hexadecimal</returns>
        public static string SeleccionarColor(string opcion)
        {
            switch (opcion)
            {
                case "1":
                    return "#45DCB4";
                case "2":
                    return "#EAF112";
                case "3":
                    return "#DE921E";
                case "4":
                    return "#DC4545";
                default:
                    return null;
            }
        }

        public static void GetDataNodes(Dictionary<string,string> dicNodos, Dictionary<string,int> scoreNodes, ref ConcurrentBag<DataItemRelacion> itemsRelacion)
        {
            foreach (KeyValuePair<string, string> nodo in dicNodos)
            {
                string clave = nodo.Key;
                Data data = new(clave, nodo.Value, null, null, null, "nodes", Data.Type.icon_area);
                if (scoreNodes.ContainsKey(clave))
                {
                    data.score = scoreNodes[clave];
                    data.name = $"{data.name} ({data.score})";
                }
                DataItemRelacion dataColabo = new(data, true, true);
                itemsRelacion.Add(dataColabo);
            }
        }

        public static void ObtenerFiltros(string pFiltroFacetas, List<string> pListaDates, ref List<string> filtros)
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

    }
}
