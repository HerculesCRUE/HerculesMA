using Hercules.MA.GraphicEngine.Models.Graficas;
using System.Collections.Generic;

namespace Hercules.MA.GraphicEngine.Models
{
    public class ConfigModel
    {
        public int Orden { get; set; }
        public Dictionary<string, string> Nombre { get; set; }
        public string Filtro { get; set; }
        public string Identificador { get; set; }
        public List<Grafica> Graficas { get; set; }
        public List<FacetaConf> Facetas { get; set; }
    }

    public class FacetaConf
    {
        public Dictionary<string, string> Nombre { get; set; }
        public bool RangoAnio { get; set; }
        public string Filtro { get; set; }
        public bool OrdenAlfaNum { get; set; }
        public bool Tesauro { get; set; }
        public int NumeroItemsFaceta { get; set; }
        public bool VerTodos { get; set; }
        public string Reciproca { get; set; }
    }

    public class Grafica
    {
        public string Identificador { get; set; }
        public Dictionary<string, string> Nombre { get; set; }
        public EnumGraficas Tipo { get; set; }
        public int Anchura { get; set; }
        public string IdGrupo { get; set; }
        public string PropCategoryPath { get; set; }
        public bool IsPrivate { get; set; }

        public Config Config { get; set; }
    }

    public class Config
    {
        public bool DatosNodos { get; set; }
        public int NumMaxNodos { get; set; }
        public bool OrientacionVertical { get; set; }
        public string EjeX { get; set; }
        public string Reciproco { get; set; }
        public bool Rango { get; set; }
        public string[] Rangos { get; set; }
        public bool Porcentual { get; set; }
        public bool Abreviar { get; set; }
        public bool OcultarLeyenda { get; set; }
        public bool OrderDesc { get; set; }
        public string Color { get; set; }
        public bool RellenarEjeX { get; set; }
        public List<EjeYConf> YAxisPrint { get; set; }
        public List<EjeXConf> XAxisPrint { get; set; }
        public List<Dimension> Dimensiones { get; set; }
    }

    public class EjeYConf
    {
        public string YAxisID { get; set; }
        public string Posicion { get; set; }
        public Dictionary<string, string> NombreEje { get; set; }
    }
    public class EjeXConf
    {
        public string XAxisID { get; set; }
        public string Posicion { get; set; }
        public Dictionary<string, string> NombreEje { get; set; }
    }

    public class Dimension
    {
        public Dictionary<string, string> Nombre { get; set; }        
        public string Filtro { get; set; }
        public int Limite { get; set; }
        public string Color { get; set; }
        public string ColorMaximo { get; set; }
        public string TipoDimension { get; set; }
        public bool DividirDatos { get; set; }
        public string Minus { get; set; }
        public bool Exterior { get; set; }
        public string Calculo { get; set; }
        public string Stack { get; set; }
        public float Anchura { get; set; }
        public string YAxisID { get; set; }
        public string XAxisID { get; set; }
        public int Orden { get; set; }
        public int NumMaxNodos { get; set; }
        public string ColorNodo { get; set; }
        public string ColorLinea { get; set; }
        public Dimension DeepCopy()
        {
            Dimension copia = new ();
            copia.Nombre = new Dictionary<string, string>(this.Nombre);
            copia.Filtro = this.Filtro;
            copia.Limite = this.Limite;
            copia.Color = this.Color;
            copia.ColorMaximo = this.ColorMaximo;
            copia.TipoDimension = this.TipoDimension;
            copia.DividirDatos = this.DividirDatos;
            copia.Minus = this.Minus;
            copia.Exterior = this.Exterior;
            copia.Calculo = this.Calculo;
            copia.Stack = this.Stack;
            copia.Anchura = this.Anchura;
            copia.YAxisID = this.YAxisID;
            copia.XAxisID = this.XAxisID;
            copia.Orden = this.Orden;
            copia.NumMaxNodos = this.NumMaxNodos;
            copia.ColorNodo = this.ColorNodo;
            copia.ColorLinea = this.ColorLinea;
            return copia;
        }
    }
}
