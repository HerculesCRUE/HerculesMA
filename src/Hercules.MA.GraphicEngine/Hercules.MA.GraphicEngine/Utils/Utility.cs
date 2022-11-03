using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Hercules.MA.GraphicEngine.Models;
using Hercules.MA.GraphicEngine.Models.Graficas;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Hercules.MA.GraphicEngine.Utils
{
    public static class Utility
    {
        private static string mPrefijos = string.Join(" ", JsonConvert.DeserializeObject<List<string>>(File.ReadAllText($@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config{Path.DirectorySeparatorChar}configJson{Path.DirectorySeparatorChar}prefijos.json")));
        private static ResourceApi mResourceApi = new ($@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config{Path.DirectorySeparatorChar}ConfigOAuth{Path.DirectorySeparatorChar}OAuthV3.config");
        private static CommunityApi mCommunityApi = new ($@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config{Path.DirectorySeparatorChar}ConfigOAuth{Path.DirectorySeparatorChar}OAuthV3.config");
        private static Guid mCommunityID = mCommunityApi.GetCommunityId();

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

    }
}
