using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hercules.MA.ServicioExterno.ModelsDataGraficaColaboradores
{
    public class DataGraficaColaboradores
    {
        public Data data { get; set; }
        public bool? selectable { get; set; }
        public bool? grabbable { get; set; }

        public DataGraficaColaboradores(Data pData, bool? pSelectable, bool? pGrabbable)
        {
            this.data = pData;
            this.selectable = pSelectable;
            this.grabbable = pGrabbable;
        }
    }
    public class Data
    {
        public string id { get; set; }
        public string name { get; set; }      
        public string source { get; set; }
        public string target { get; set; }
        public double? weight { get; set; }
        public string group { get; set; }  
        
        public Data (string pId, string pName, string pSource, string pTarget, double? pWeight, string pGroup)
        {
            this.id = pId;
            this.name = pName;
            this.source = pSource;
            this.target = pTarget;
            this.weight = pWeight;
            this.group = pGroup;
        }
    }
}