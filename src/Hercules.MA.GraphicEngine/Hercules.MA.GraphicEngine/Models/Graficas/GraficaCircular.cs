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
            if (data != null && data.datasets != null && data.datasets.Count == 1)
            {
                DatasetCircular dataset = data.datasets.FirstOrDefault();
                if (dataset == null)
                {
                    return null;
                }
                StringBuilder csv = new("");
                csv.AppendLine("\"" + string.Join(";", data.labels).Replace("\"", "\"\"").Replace(";", "\";\"") + "\"");
                csv.AppendLine("\"" + string.Join(";", dataset.data).Replace("\"", "\"\"").Replace(";", "\";\"") + "\"");
                return Encoding.Latin1.GetBytes(csv.ToString());
            }
            else
            {
                StringBuilder csv = new("");
                csv.AppendLine(";\"" + string.Join(";", data.labels).Replace("\"", "\"\"").Replace(";", "\";\"") + "\"");
                Dictionary<string, List<float>> dic = new();

                foreach (DatasetCircular datasetCircular in data.datasets.Where(x => x.grupos != null))
                {
                    for (int i = 0; i < datasetCircular.data.Count; i++)
                    {
                        int grupo = datasetCircular.grupos[i];
                        string quitar = " " + data.labels[grupo].ToLower();
                        string label = datasetCircular.label.Split('|')[i].Split(quitar)[0];
                        if (!dic.ContainsKey(label))
                        {
                            dic.Add(label, new List<float>());
                        }
                        if (dic[label].Count != grupo)
                        {
                            dic[label].Add(0);
                        }
                        dic[label].Add(datasetCircular.data[i]);
                    }
                }
                foreach (KeyValuePair<string, List<float>> item in dic)
                {
                    csv.AppendLine("\"" + item.Key + "\";\"" + string.Join(";", item.Value).Replace("\"", "\"\"").Replace(";", "\";\"") + "\"");
                }
                return Encoding.Latin1.GetBytes(csv.ToString());
            }
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
        public List<int> grupos { get; set; }
        public int hoverOffset { get; set; }
    }
}
