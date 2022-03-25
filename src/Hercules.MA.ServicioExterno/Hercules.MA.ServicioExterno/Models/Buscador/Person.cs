using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hercules.MA.ServicioExterno.Models.Buscador
{
    public class Person : ObjectSearch
    {
        public bool searchable { get; set; }
        
        /**
         * Método para buscar dentro de Person
         */
        public override long Search(string[] pInput)
        {
            long respuesta = 0;
            bool encontradoTitulo = true;
            // Busca si TODAS las palabas se encuentran en el título
            foreach (string input in pInput)
            {
                encontradoTitulo = encontradoTitulo && titleAuxSearch.Contains(input);
            }
            // Resultados
            if (encontradoTitulo)
            {
                respuesta += 1000;
            }
            return respuesta;
        }
    }

}
