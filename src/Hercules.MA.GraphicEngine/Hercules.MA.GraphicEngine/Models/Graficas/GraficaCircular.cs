﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hercules.MA.GraphicEngine.Models.Graficas
{
    public class GraficaCircular : GraficaBase
    {
        public string type { get; set; }
        public Options options { get; set; }
        public DataCircular data { get; set; }
        public override byte[] GenerateCSV()
        {
            StringBuilder csv = new StringBuilder("");
            csv.AppendLine("\"" + String.Join(";", data.labels).Replace("\"", "\"\"").Replace(";", "\";\"") + "\"");
            csv.AppendLine("\"" + String.Join(";", data.datasets.FirstOrDefault().data).Replace("\"", "\"\"").Replace(";", "\";\"") + "\"");
            return Encoding.Latin1.GetBytes(csv.ToString());
        }
    }
    public class DataCircular
    {
        public List<string> labels { get; set; }
        public ConcurrentBag<DatasetCircular> datasets { get; set; }
    }
    public class DatasetCircular
    {
        public string label { get; set; }
        public List<float> data { get; set; }
        public List<string> backgroundColor { get; set; }
        public int hoverOffset { get; set; }
    }
}
