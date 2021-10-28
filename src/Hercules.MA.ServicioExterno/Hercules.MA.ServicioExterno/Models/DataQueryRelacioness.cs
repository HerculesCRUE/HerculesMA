using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.MA.ServicioExterno.Models.DataQueryRelaciones
{
    public class DataQueryRelaciones
    {
        public List<Datos> idRelacionados { get; set; }
        public DataQueryRelaciones(List<Datos> pidRelacionados)
        {
            this.idRelacionados = pidRelacionados;
        }
    }
    public class Datos
    {
        public string idRelacionado { get; set; }
        public int numVeces { get; set; }
        public Datos(string pIdRelacionado, int pNumVeces)
        {
            this.idRelacionado = pIdRelacionado;
            this.numVeces = pNumVeces;
        }
    }
}
