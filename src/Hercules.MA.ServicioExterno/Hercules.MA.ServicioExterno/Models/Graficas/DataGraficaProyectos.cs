using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.MA.ServicioExterno.Models.Graficas.DataGraficaProyectos
{
    public class ObjGrafica
    {
        public string type { get; set; }
        public DataGraficaProyectos data { get; set; }
        public Options options { get; set; }
        public ObjGrafica (string pType, DataGraficaProyectos pData, Options pOptions)
        {
            this.type = pType;
            this.data = pData;
            this.options = pOptions;
        }
    }

    public class Options
    {
        public int barValueSpacing { get; set; }
        public Scales scales { get; set; }
        public Options (int pBarValueSpacing, Scales pScales)
        {
            this.barValueSpacing = pBarValueSpacing;
            this.scales = pScales;
        }
    }

    public class Scales
    {
        public List<YAxes> yAxes { get; set; }
        public Scales (List<YAxes> pYAxes)
        {
            this.yAxes = pYAxes;
        }
    }

    public class YAxes
    {
        public Ticks ticks { get; set; }
        public YAxes (Ticks pTicks)
        {
            this.ticks = pTicks;
        }
    }

    public class Ticks
    {
        public int min { get; set; }
        public Ticks(int pMin)
        {
            this.min = pMin;
        }
    }

    public class DataGraficaProyectos
    {
        public List<string> labels { get; set; }
        public List<DatosAnyo> datasets { get; set; }
        public DataGraficaProyectos (List<string> pLabels, List<DatosAnyo> pDatasets)
        {
            this.labels = pLabels;
            this.datasets = pDatasets;
        }
    }

    public class DatosAnyo
    {
        public string label { get; set; }
        public string backgroundColor { get; set; }
        public List<int> data { get; set; }
        public DatosAnyo (string pLabel, string pBackgroundColor, List<int> pData)
        {
            this.label = pLabel;
            this.backgroundColor = pBackgroundColor;
            this.data = pData;
        }
    }
}
