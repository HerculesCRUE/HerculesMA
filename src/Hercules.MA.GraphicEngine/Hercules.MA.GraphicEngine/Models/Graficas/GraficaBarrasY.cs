using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hercules.MA.GraphicEngine.Models.Graficas
{
    public class GraficaBarrasY : GraficaBase
    {
        public string type { get; set; }
        public Options options { get; set; }
        public DataBarrasY data { get; set; }

        public override byte[] GenerateCSV()
        {
            StringBuilder csv = new StringBuilder("");
            csv.AppendLine(";\"" + String.Join(";", data.labels).Replace("\"", "\"\"").Replace(";", "\";\"") + "\"");
            foreach (DatasetBarrasY datasetBarras in data.datasets)
            {
                csv.AppendLine("\"" + datasetBarras.label + "\";\"" + String.Join(";", datasetBarras.data).Replace("\"", "\"\"").Replace(";", "\";\"") + "\"");
            }
            return Encoding.Latin1.GetBytes(csv.ToString());
        }
    }
    public class DataBarrasY
    {
        public List<string> labels { get; set; }
        public ConcurrentBag<DatasetBarrasY> datasets { get; set; }
        public string type { get; set; }
    }
    public class DatasetBarrasY
    {
        public string label { get; set; }
        public List<float> data { get; set; }
        public List<string> backgroundColor { get; set; }
        public string type { get; set; }
        public string stack { get; set; }
        public float barPercentage { get; set; }
        public float maxBarThickness { get; set; }
        public string xAxisID { get; set; }
        public int order { get; set; }
    }
}
