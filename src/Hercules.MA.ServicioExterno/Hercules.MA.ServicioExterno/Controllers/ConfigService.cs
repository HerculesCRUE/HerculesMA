using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.IO;

namespace Hercules.MA.ServicioExterno.Controllers
{
    public class ConfigService
    {
        // Archivo de configuración.
        public static IConfigurationRoot configuracion;
        private string RabbitConnectionString { get; set; }
        private string QueueRabbit { get; set; }
        private string UrlSimilarity { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConfigService()
        {
            configuracion = new ConfigurationBuilder().AddJsonFile($@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}appsettings.json").Build();
        }

        /// <summary>
        /// Contruye el objeto de lectura con la configuración del JSON.
        /// </summary>
        /// <returns></returns>
        public static IConfiguration GetBuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

            return builder.Build();
        }

        /// <summary>
        /// Obtiene la cadena de conexión de Rabbit configurada.
        /// </summary>
        /// <returns>Cadena de conexión de Rabbit.</returns>
        public string GetRabbitConnectionString()
        {
            if (string.IsNullOrEmpty(RabbitConnectionString))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string rabbitConnectionString = "";
                if (environmentVariables.Contains("colaFuentesExternas"))
                {
                    rabbitConnectionString = environmentVariables["colaFuentesExternas"] as string;
                }
                else
                {
                    rabbitConnectionString = configuracion["RabbitMQ:colaFuentesExternas"];
                }
                RabbitConnectionString = rabbitConnectionString;
            }
            return RabbitConnectionString;
        }

        /// <summary>
        /// Obtiene la el nombre de la cola Rabbit de configuración.
        /// </summary>
        /// <returns>Nombre de la cola Rabbit.</returns>
        public string GetQueueRabbit()
        {
            if (string.IsNullOrEmpty(QueueRabbit))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string queue = "";
                if (environmentVariables.Contains("QueueRabbit"))
                {
                    queue = environmentVariables["QueueRabbit"] as string;
                }
                else
                {
                    queue = configuracion["QueueRabbit"];
                }
                QueueRabbit = queue;
            }
            return QueueRabbit;
        }

        /// <summary>
        /// Obtiene la URL del servicio de similitud
        /// </summary>
        /// <returns>Url del servicio</returns>
        public string GetUrlSimilarity()
        {
            if (string.IsNullOrEmpty(UrlSimilarity))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                string url = "";
                if (environmentVariables.Contains("UrlSimilarity"))
                {
                    url = environmentVariables["UrlSimilarity"] as string;
                }
                else
                {
                    url = configuracion["UrlSimilarity"];
                }
                UrlSimilarity = url;
            }
            return UrlSimilarity;
        }
    }
}
