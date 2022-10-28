using System.Collections.Generic;

namespace Hercules.MA.GraphicEngine.Models.Facetas
{
    public class ItemFaceta
    {
        public string nombre { get; set; }
        public int numero { get; set; }
        public string filtro { get; set; }
        public string idTesauro { get; set; }
        public List<ItemFaceta> childsTesauro { get; set; }

    }
}
