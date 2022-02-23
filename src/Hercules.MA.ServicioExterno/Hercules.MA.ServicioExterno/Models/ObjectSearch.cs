using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hercules.MA.ServicioExterno.Models
{
    public class Person : ObjectSearch
    {
        public bool searchable { get; set; }
        public override long Search(string[] pInput)
        {
            long respuesta = 0;
            bool encontrado = true;
            foreach (string input in pInput)
            {
                encontrado = encontrado && titleAuxSearch.Contains(input);
            }
            if (encontrado)
            {
                respuesta = 1;
            }
            return respuesta;
        }
    }

    public class Publication : ObjectSearch
    {
        public List<string> tagsAuxSearch { get; set; }
        public string descriptionAuxSearch { get; set; }
        public HashSet<Person> persons { get; set; }

        public override long Search(string[] pInput)
        {            
            //Tiene 4 campos (1000,100,10,1)
            long respuesta = 0;
            bool encontradoTitulo = true;
            bool encontradoTags = true;
            int numTags = 0;
            bool encontradoDescripcion = true;
            bool encontradoAutores = true;
            int numAutores = 0;
            foreach (string input in pInput)
            {
                encontradoTitulo = encontradoTitulo && titleAuxSearch.Contains(input);
            }

            foreach (string tag in tagsAuxSearch)
            {
                bool encontradoTagAux = true;
                foreach (string input in pInput)
                {
                    encontradoTagAux = encontradoTagAux && tag.Contains(input);
                }
                if (encontradoTagAux)
                {
                    encontradoTags = true;
                    numTags++;
                }
            }

            encontradoDescripcion = true;
            foreach (string input in pInput)
            {
                encontradoDescripcion = encontradoDescripcion && descriptionAuxSearch.Contains(input);
            }


            foreach (Person person in persons)
            {
                bool encontradoAutorAux = true;
                encontradoAutorAux = encontradoAutorAux && person.Search(pInput) > 0;
                if (encontradoAutorAux)
                {
                    encontradoAutores = true;
                    numAutores++;
                }
            }

            if (encontradoTitulo)
            {
                respuesta += 1000;
            }
            if (encontradoTags)
            {
                respuesta += 100 * numTags;
            }
            if (encontradoDescripcion)
            {
                respuesta += 10;
            }
            if (encontradoAutores)
            {
                respuesta += 1 * numAutores;
            }
            return respuesta;
        }
    }

    public class ResearchObject : ObjectSearch
    {
        public List<string> tagsAuxSearch { get; set; }
        public string descriptionAuxSearch { get; set; }
        public HashSet<Person> persons { get; set; }

        public override long Search(string[] pInput)
        {
            //Tiene 4 campos (1000,100,10,1)
            long respuesta = 0;
            bool encontradoTitulo = true;
            bool encontradoTags = true;
            int numTags = 0;
            bool encontradoDescripcion = true;
            bool encontradoAutores = true;
            int numAutores = 0;
            foreach (string input in pInput)
            {
                encontradoTitulo = encontradoTitulo && titleAuxSearch.Contains(input);
            }

            foreach (string tag in tagsAuxSearch)
            {
                encontradoTags = true;
                foreach (string input in pInput)
                {
                    encontradoTags = encontradoTags && tag.Contains(input);
                }
                if (encontradoTags)
                {
                    numTags++;
                }
            }

            encontradoDescripcion = true;
            foreach (string input in pInput)
            {
                encontradoDescripcion = encontradoDescripcion && descriptionAuxSearch.Contains(input);
            }


            foreach (Person person in persons)
            {
                encontradoAutores = true;
                encontradoAutores = encontradoAutores && person.Search(pInput) > 0;
                if (encontradoAutores)
                {
                    numAutores++;
                }
            }

            if (encontradoTitulo)
            {
                respuesta += 1000;
            }
            if (encontradoTags)
            {
                respuesta += 100 * numTags;
            }
            if (encontradoDescripcion)
            {
                respuesta += 10;
            }
            if (encontradoAutores)
            {
                respuesta += 1 * numAutores;
            }
            return respuesta;
        }
    }

    public class Group : ObjectSearch
    {
        public string descriptionAuxSearch { get; set; }
        public HashSet<Person> persons { get; set; }


        public override long Search(string[] pInput)
        {
            long respuesta = 0;
            int encontradoTituloEntero = 0;
            int encontradoTitulo = 0;
            int encontradoDescripcion = 0;
            int encontradoAutores = 0;

            // Busca la palabra exacta
            if (Regex.IsMatch(titleAuxSearch, @"\b" + string.Join(" ", pInput).Trim() + @"\b", RegexOptions.IgnoreCase))
            {
                encontradoTituloEntero++;
            }

            encontradoTituloEntero = titleAuxSearch.Contains(string.Join(" ", pInput).Trim()) ? encontradoTituloEntero + 1 : encontradoTituloEntero;


            foreach (string input in pInput)
            {
                encontradoTitulo = titleAuxSearch.Contains(input) ? encontradoTitulo + 1 : encontradoTitulo;
            }

            foreach (string input in pInput)
            {
                encontradoDescripcion = descriptionAuxSearch.Contains(input) ? encontradoDescripcion + 1 : encontradoDescripcion;
            }

            var tmpEncAutores = 0;
            foreach (Person person in persons)
            {
                Int32.TryParse(person.Search(pInput).ToString(), out tmpEncAutores);
                encontradoAutores += tmpEncAutores;
            }

            // Añade la suma con el peso del resultado
            respuesta += encontradoTituloEntero * 5000;

            respuesta += encontradoTitulo * 1000;

            respuesta += encontradoDescripcion * 10;

            respuesta += encontradoAutores;

            return respuesta;
        }
    }

    public class Project : ObjectSearch
    {
        public string descriptionAuxSearch { get; set; }
        public HashSet<Person> persons { get; set; }


        public override long Search(string[] pInput)
        {
            long respuesta = 0;
            int encontradoTituloEntero = 0;
            int encontradoTitulo = 0;
            int encontradoDescripcion = 0;
            int encontradoAutores = 0;

            // Busca la palabra exacta
            if (Regex.IsMatch(titleAuxSearch, @"\b" + string.Join(" ", pInput).Trim() + @"\b", RegexOptions.IgnoreCase))
            {
                encontradoTituloEntero++;
            }

            encontradoTituloEntero = titleAuxSearch.Contains(string.Join(" ", pInput).Trim()) ? encontradoTituloEntero + 1 : encontradoTituloEntero;

            foreach (string input in pInput)
            {
                encontradoTitulo = titleAuxSearch.Contains(input) ? encontradoTitulo + 1 : encontradoTitulo;
            }

            foreach (string input in pInput)
            {
                encontradoDescripcion = descriptionAuxSearch.Contains(input) ? encontradoDescripcion + 1 : encontradoDescripcion;
            }


            var tmpEncAutores = 0;
            foreach (Person person in persons)
            {
                Int32.TryParse(person.Search(pInput).ToString(), out tmpEncAutores);
                encontradoAutores += tmpEncAutores;
            }

            // Añade la suma con el peso del resultado
            respuesta += encontradoTituloEntero * 5000;

            respuesta += encontradoTitulo * 1000;


            respuesta += encontradoDescripcion * 10;

            respuesta += encontradoAutores;

            return respuesta;
        }
    }

    public abstract class ObjectSearch
    {
        public string title { get; set; }
        public string url { get; set; }
        public string titleAuxSearch { get; set; }
        public Guid id { get; set; }
        public abstract long Search(string[] pInput);
    }
}