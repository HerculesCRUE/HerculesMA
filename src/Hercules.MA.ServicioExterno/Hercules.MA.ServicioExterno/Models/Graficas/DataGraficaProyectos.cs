using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hercules.MA.ServicioExterno.Models.Graficas.GraficaBarras;

namespace Hercules.MA.ServicioExterno.Models.Graficas.DataGraficaProyectos
{
    public class GraficasProyectos
    {
        public GraficaBarras.GraficaBarras graficaBarrasAnios { get; set; }
        public GraficaBarras.GraficaBarras graficaSectoresAmbito { get; set; }
        public GraficaBarras.GraficaBarras graficaBarrasMiembros { get; set; }
    }
    
}
