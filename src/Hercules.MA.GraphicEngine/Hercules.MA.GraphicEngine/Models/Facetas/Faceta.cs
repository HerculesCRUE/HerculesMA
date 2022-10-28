using Hercules.MA.GraphicEngine.Models.Facetas;
using System.Collections.Generic;

namespace Hercules.MA.GraphicEngine.Models
{
    public class Faceta
    {
        public string id { get; set; }
        public bool isDate { get; set; }
        public string nombre { get; set; }
        public int numeroItemsFaceta { get; set; }
        public bool ordenAlfaNum { get; set; }
        public bool verTodos { get; set; }
        public bool tesauro { get; set; }
        public string reciproca { get; set; }
        public List<ItemFaceta> items { get; set; }

        public Faceta()
        {
            this.items = new List<ItemFaceta>();
        }

        public Faceta(string id, bool isDate, string nombre, bool ordenAlfaNum, bool verTodos, bool tesauro, string reciproca)
        {
            this.id = id;
            this.isDate = isDate;
            this.nombre = nombre;
            this.numeroItemsFaceta = 10000;
            this.ordenAlfaNum = ordenAlfaNum;
            this.verTodos = verTodos;
            this.tesauro = tesauro;
            this.reciproca = reciproca;
            this.items = new List<ItemFaceta>();
        }

        public Faceta(string id, bool isDate, string nombre, int numeroItemsFaceta, bool ordenAlfaNum, bool verTodos, bool tesauro, string reciproca, List<ItemFaceta> items)
        {
            this.id = id;
            this.isDate = isDate;
            this.nombre = nombre;
            this.numeroItemsFaceta = numeroItemsFaceta;
            this.ordenAlfaNum = ordenAlfaNum;
            this.verTodos = verTodos;
            this.tesauro = tesauro;
            this.reciproca = reciproca;
            this.items = items;
        }
    }

}
