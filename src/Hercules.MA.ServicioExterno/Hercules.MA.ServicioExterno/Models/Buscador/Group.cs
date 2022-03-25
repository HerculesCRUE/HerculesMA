using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hercules.MA.ServicioExterno.Models.Buscador
{
    public class Group : ObjectSearch
    {
        public string descriptionAuxSearch { get; set; }
        public HashSet<Person> persons { get; set; }


        public override long Search(string[] pInput)
        {
            long respuesta = 0;
            bool encontradoTitulo = true;
            bool encontradoDescripcion = true;
            int numAutores = 0;


            // Busca si TODAS las palabas se encuentran en el título
            foreach (string input in pInput)
            {
                encontradoTitulo = encontradoTitulo && titleAuxSearch.Contains(input);
            }

            // Buscar en las descripciones que aparezca TODAS las palabras de la cadena de texto
            foreach (string input in pInput)
            {
                encontradoDescripcion = encontradoDescripcion && descriptionAuxSearch.Contains(input);
            }

            foreach (Person person in persons)
            {
                if (person.Search(pInput) > 0)
                {
                    numAutores++;
                }
            }

            // Resultados
            if (encontradoTitulo)
            {
                respuesta += 100;
            }

            if (encontradoDescripcion)
            {
                respuesta += 10;
            }

            respuesta += numAutores;

            return respuesta;
        }
    }
}
