using System.Collections.Generic;

namespace Hercules.MA.GraphicEngine.Models.Paginas
{
    public class Pagina
    {
        public string id { get; set; }
        public string nombre { get; set; }
        public List<ConfigPagina> listaConfigGraficas { get; set; }
        public List<string> listaIdsFacetas { get; set; }
    }
}
