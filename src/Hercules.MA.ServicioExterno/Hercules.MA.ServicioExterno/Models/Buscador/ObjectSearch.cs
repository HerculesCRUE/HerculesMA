using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hercules.MA.ServicioExterno.Models.Buscador
{    
    public abstract class ObjectSearch
    {
        public string title { get; set; }
        public string url { get; set; }
        public string titleAuxSearch { get; set; }
        public Guid id { get; set; }
        public abstract long Search(string[] pInput);
    }
}