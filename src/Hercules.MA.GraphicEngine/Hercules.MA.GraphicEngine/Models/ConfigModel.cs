using System.Collections.Generic;

namespace Hercules.MA.GraphicEngine.Models
{
    public class ConfigModel
    {
        public int orden { get; set; }
        public Dictionary<string, string> nombre { get; set; }
        public string filtro { get; set; }
        public string identificador { get; set; }
        public List<Grafica> graficas { get; set; }
        public List<FacetaConf> facetas { get; set; }
    }

    public class FacetaConf
    {
        public Dictionary<string, string> nombre { get; set; }
        public bool rangoAnio { get; set; }
        public string filtro { get; set; }
        public bool ordenAlfaNum { get; set; }
        public bool tesauro { get; set; }
        public int numeroItemsFaceta { get; set; }
        public bool verTodos { get; set; }
        public string reciproca { get; set; }
    }

    public class Grafica
    {
        public string identificador { get; set; }
        public Dictionary<string, string> nombre { get; set; }
        public EnumGraficas tipo { get; set; }
        public int anchura { get; set; }
        public string idGrupo { get; set; }
        public string propCategoryPath { get; set; }
        public bool isPrivate { get; set; }
        public Config config { get; set; }
    }

    public class Config
    {
        public bool datosNodos { get; set; }
        public int numMaxNodos { get; set; }
        public bool orientacionVertical { get; set; }
        public string ejeX { get; set; }
        public string reciproco { get; set; }
        public bool rango { get; set; }
        public bool porcentual { get; set; }
        public bool abreviar { get; set; }
        public bool orderDesc { get; set; }
        public string color { get; set; }
        public bool rellenarEjeX { get; set; }
        public List<EjeYConf> yAxisPrint { get; set; }
        public List<EjeXConf> xAxisPrint { get; set; }
        public List<Dimension> dimensiones { get; set; }
    }

    public class EjeYConf
    {
        public string yAxisID { get; set; }
        public string posicion { get; set; }
        public Dictionary<string, string> nombreEje { get; set; }
    }
    public class EjeXConf
    {
        public string xAxisID { get; set; }
        public string posicion { get; set; }
        public Dictionary<string, string> nombreEje { get; set; }
    }

    public class Dimension
    {
        public Dictionary<string, string> nombre { get; set; }        
        public string filtro { get; set; }
        public int limite { get; set; }
        public string color { get; set; }
        public string colorMaximo { get; set; }
        public string tipoDimension { get; set; }
        public bool dividirDatos { get; set; }
        public string minus { get; set; }
        public bool exterior { get; set; }
        public string calculo { get; set; }
        public string stack { get; set; }
        public float anchura { get; set; }
        public string yAxisID { get; set; }
        public string xAxisID { get; set; }
        public int orden { get; set; }
        public int numMaxNodos { get; set; }
        public string colorNodo { get; set; }
        public string colorLinea { get; set; }
        public Dimension DeepCopy()
        {
            Dimension copia = new Dimension();
            copia.nombre = new Dictionary<string, string>(this.nombre);
            copia.filtro = this.filtro;
            copia.limite = this.limite;
            copia.color = this.color;
            copia.colorMaximo = this.colorMaximo;
            copia.tipoDimension = this.tipoDimension;
            copia.dividirDatos = this.dividirDatos;
            copia.minus = this.minus;
            copia.exterior = this.exterior;
            copia.calculo = this.calculo;
            copia.stack = this.stack;
            copia.anchura = this.anchura;
            copia.yAxisID = this.yAxisID;
            copia.xAxisID = this.xAxisID;
            copia.orden = this.orden;
            copia.numMaxNodos = this.numMaxNodos;
            copia.colorNodo = this.colorNodo;
            copia.colorLinea = this.colorLinea;
            return copia;
        }
    }
}
