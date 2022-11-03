using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hercules.MA.GraphicEngine.Models.Graficas
{
    public class GraficaBarras : GraficaBase
    {
        public string Type { get; set; }
        public Options Options { get; set; }
        public DataBarras Data { get; set; }
        public override byte[] GenerateCSV()
        {
            StringBuilder csv = new ("");
            csv.AppendLine(";\"" + string.Join(";", Data.Labels).Replace("\"", "\"\"").Replace(";", "\";\"") + "\"");
            foreach (DatasetBarras datasetBarras in Data.Datasets)
            {
                csv.AppendLine("\"" + datasetBarras.Label + "\";\"" + string.Join(";", datasetBarras.Data).Replace("\"", "\"\"").Replace(";", "\";\"") + "\"");
            }
            return Encoding.Latin1.GetBytes(csv.ToString());
        }
    }
    public class DataBarras
    {
        public List<string> Labels { get; set; }
        public ConcurrentBag<DatasetBarras> Datasets { get; set; }
        public string Type { get; set; }
    }
    public class DatasetBarras
    {
        public string Label { get; set; }
        public List<float> Data { get; set; }
        public List<string> BackgroundColor { get; set; }
        public string Type { get; set; }
        public string Stack { get; set; }
        public float BarPercentage { get; set; }
        public float MaxBarThickness { get; set; }
        public string YAxisID { get; set; }
        public int Order { get; set; }
    }
}
