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
        public override long SearchAutocompletar(HashSet<string> pInput, string pLastInput)
        {
            long respuestaPeso = 0;
            if (SearchForAutocomplete(titleAuxSearch, pInput, pLastInput))
            {
                respuestaPeso += 1;
            }
            return respuestaPeso;
        }


        public override bool SearchBuscador(HashSet<string> pInput, string pLastInput)
        {            
            return SearchForSearcher(titleAuxSearch, pInput, pLastInput);
        }
    }

}
