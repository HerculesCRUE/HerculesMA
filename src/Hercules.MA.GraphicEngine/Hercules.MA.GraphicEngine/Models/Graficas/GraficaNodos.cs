using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hercules.MA.GraphicEngine.Models.Graficas
{
    public class GraficaNodos : GraficaBase
    {
        public string title { get; set; }
        public string container { get; set; }
        public Layout layout { get; set; }
        public List<Style> style { get; set; }
        public List<DataItemRelacion> elements { get; set; }
        public bool userZoomingEnabled { get; set; }
        public bool zoomingEnabled { get; set; }
        public float minZoom { get; set; }
        public float maxZoom { get; set; }

        public override byte[] GenerateCSV()
        {
            StringBuilder csv = new ("");
            // Nombre/Id de los nodos
            List<string> nombres = new ();
            List<string> ids = new ();

            // Recorro los elementos para obtener los nombres e ids
            foreach (DataItemRelacion item in elements)
            {
                if (item.Data.group == "nodes")
                {
                    nombres.Add(item.Data.name.Split('(').First().TrimEnd());
                    ids.Add(item.Data.id);
                }
            }

            double?[,] valorRelaciones = new double?[nombres.Count, nombres.Count];
            int cont = 0;

            // Recorro los elementos para obtener los valores
            foreach (DataItemRelacion item in elements)
            {
                if (item.Data.group == "nodes")
                {
                    valorRelaciones[cont, cont] = item.Data.score;
                    cont++;
                }
                else
                {
                    int source = ids.IndexOf(item.Data.source);
                    int target = ids.IndexOf(item.Data.target);
                    double valor = double.Parse(item.Data.id.Split('~').Last());
                    valorRelaciones[source, target] = valor;
                    valorRelaciones[target, source] = valor;
                }
            }

            // Paso los valores de la gráfica a una lista
            List<List<string>> valores = new();
            for (int i = 0; i < nombres.Count; i++)
            {
                List<string> aux = new();
                for (int j = 0; j < nombres.Count; j++)
                {
                    if (valorRelaciones[i, j] == null)
                    {
                        aux.Add("0");
                    }
                    else
                    {
                        aux.Add(valorRelaciones[i, j].ToString());
                    }
                }
                valores.Add(aux);
            }

            // Añado al string los nombres de los nodos
            csv.AppendLine(";\"" + string.Join(";", nombres).Replace("\"", "\"\"").Replace(";", "\";\"") + "\"");

            // Añado al string los valores de los nodos y sus relaciones
            for (int i = 0; i < nombres.Count; i++)
            {
                csv.AppendLine("\"" + nombres[i] + "\";\"" + string.Join(";", valores[i]).Replace("\"", "\"\"").Replace(";", "\";\"") + "\"");
            }
            return Encoding.Latin1.GetBytes(csv.ToString());
        }
    }

    public class Layout
    {
        public string name { get; set; }
        public int idealEdgeLength { get; set; }
        public int nodeOverlap { get; set; }
        public int refresh { get; set; }
        public bool fit { get; set; }
        public int padding { get; set; }
        public bool randomize { get; set; }
        public int componentSpacing { get; set; }
        public int nodeRepulsion { get; set; }
        public int edgeElasticity { get; set; }
        public int nestingFactor { get; set; }
        public int gravity { get; set; }
        public int numIter { get; set; }
        public int initialTemp { get; set; }
        public float coolingFactor { get; set; }
        public float minTemp { get; set; }
    }

    public class Style
    {
        public string Selector { get; set; }
        public LayoutStyle style { get; set; }
    }

    public class LayoutStyle
    {
        public string Width { get; set; }
        public string Content { get; set; }
        [JsonPropertyName("font-size")]
        public string FontSize { get; set; }
        [JsonPropertyName("font-family")]
        public string FontFamily { get; set; }
        [JsonPropertyName("background-color")]
        public string BackgroundColor { get; set; }
        [JsonPropertyName("overlay-padding")]
        public string OverlayPadding { get; set; }
        [JsonPropertyName("z-index")]
        public string ZIndex { get; set; }
        public string Height { get; set; }
        [JsonPropertyName("text-outline-width")]
        public string TextOutlineWidth { get; set; }
        [JsonPropertyName("curve-style")]
        public string CurveStyle { get; set; }
        [JsonPropertyName("haystack-radius")]
        public string HaystackRadius { get; set; }
        public string Opacity { get; set; }
        [JsonPropertyName("line-color")]
        public string LineColor { get; set; }
    }

    public class DataItemRelacion
    {
        public Data Data { get; set; }
        public bool? Selectable { get; set; }
        public bool? Grabbable { get; set; }

        public DataItemRelacion(Data pData, bool? pSelectable, bool? pGrabbable)
        {
            this.Data = pData;
            this.Selectable = pSelectable;
            this.Grabbable = pGrabbable;
        }
    }

    public class Data
    {
        public enum Type
        {
            none,
            icon_member,
            icon_ip,
            icon_area,
            icon_group,
            relation_document,
            relation_project
        }
        public string id { get; set; }
        public string name { get; set; }
        public string source { get; set; }
        public string target { get; set; }
        public double? weight { get; set; }
        public string group { get; set; }
        public string type { get; set; }
        public double? score { get; set; }

        public Data(string pId, string pName, string pSource, string pTarget, double? pWeight, string pGroup, Type pType)
        {
            this.id = pId;
            this.name = pName;
            this.source = pSource;
            this.target = pTarget;
            this.weight = pWeight;
            this.group = pGroup;
            this.type = pType.ToString();
        }
    }

    public class DataQueryRelaciones
    {
        public string nombreRelacion { get; set; }
        public List<Datos> idRelacionados { get; set; }
    }
    public class Datos
    {
        public string idRelacionado { get; set; }
        public int numVeces { get; set; }
    }
}
