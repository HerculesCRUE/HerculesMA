using Gnoss.ApiWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hercules.MA.Load
{
    class Program
    {
        private static ResourceApi mResourceApi = new ResourceApi($@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Config\configOAuth\OAuthV3.config");
        static void Main(string[] args)
        {
            CargaNormaCVN.mResourceApi = mResourceApi;
            CargaXMLMurcia.mResourceApi = mResourceApi;
            CargaNormaCVN.CargarEntidadesSecundarias();
            CargaXMLMurcia.CargarEntidadesPrincipales();
        }
    }
}
