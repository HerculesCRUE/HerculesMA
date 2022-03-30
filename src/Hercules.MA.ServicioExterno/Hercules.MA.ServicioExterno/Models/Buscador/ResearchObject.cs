using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hercules.MA.ServicioExterno.Models.Buscador
{
    public class ResearchObject : ObjectSearch
    {
        public List<HashSet<string>> tagsAuxSearch { get; set; }
        public HashSet<string> descriptionAuxSearch { get; set; }
        public HashSet<Person> persons { get; set; }

        public override long SearchAutocompletar(HashSet<string> pInput, string pLastInput)
        {
            long respuestaPeso = 0;
            bool encontradoTitulo = SearchForAutocomplete(titleAuxSearch, pInput, pLastInput);
            bool encontradoDescripcion = SearchForAutocomplete(descriptionAuxSearch, pInput, pLastInput);
            int numAutores = 0;
            int numTags = 0;

            foreach (HashSet<string> tag in tagsAuxSearch)
            {
                if (SearchForAutocomplete(tag, pInput, pLastInput))
                {
                    numTags++;
                }
            }

            foreach (Person person in persons)
            {
                if (person.SearchAutocompletar(pInput, pLastInput) > 0)
                { 
                    numAutores++;
                }
            }

            // Resultados
            if (encontradoTitulo)
            {
                respuestaPeso += 1000000000;
            }

            if (numTags > 0)
            {
                respuestaPeso += 1000000 * numTags;
            }

            if (encontradoDescripcion)
            {
                respuestaPeso += 1000;
            }

            respuestaPeso += numAutores;

            return respuestaPeso;
        }


        public override bool SearchBuscador(HashSet<string> pInput, string pLastInput)
        {
            bool respuesta = false;
            respuesta = respuesta || SearchForSearcher(titleAuxSearch, pInput,pLastInput);
            respuesta = respuesta || SearchForSearcher(descriptionAuxSearch, pInput, pLastInput);
            foreach (HashSet<string> tag in tagsAuxSearch)
            {
                respuesta = respuesta || SearchForSearcher(tag, pInput, pLastInput);
            }
            foreach (Person person in persons)
            {
                respuesta = respuesta || person.SearchBuscador(pInput, pLastInput);
            }
            return respuesta;
        }
    }
}
