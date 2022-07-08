using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hercules.MA.Journals.Models
{
    public class Journal
    {
        public string idJournal { get; set; }
        public string titulo { get; set; }
        public string issn { get; set; }
        public string eissn { get; set; }
        public string publicador { get; set; }
        public List<IndiceImpacto> indicesImpacto { get; set; }
        public List<Categoria> categorias { get; set; }
    }

    public class IndiceImpacto
    {
        public string idImpactIndex { get; set; }
        public string fuente { get; set; }
        public int anyo { get; set; }
        public float indiceImpacto { get; set; }
    }

    public class Categoria
    {
        public string idImpactCategory { get; set; }
        public string fuente { get; set; }
        public int anyo { get; set; }
        public string nomCategoria { get; set; }
        public int posicionPublicacion { get; set; }
        public int numCategoria { get; set; }
        public int cuartil { get; set; }
    }
}
