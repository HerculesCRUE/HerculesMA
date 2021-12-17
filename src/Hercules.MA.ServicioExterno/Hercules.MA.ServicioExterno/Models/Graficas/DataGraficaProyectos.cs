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
        public ObjGrafica (string pType, DataGraficaProyectos pData)
        {
            this.type = pType;
            this.data = pData;
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
        public float barPercentage { get; set; }
        public DatosAnyo (string pLabel, string pBackgroundColor, List<int> pData,float pBarPercentage)
        {
            this.label = pLabel;
            this.backgroundColor = pBackgroundColor;
            this.data = pData;
            this.barPercentage = pBarPercentage;
        }
    }
}
