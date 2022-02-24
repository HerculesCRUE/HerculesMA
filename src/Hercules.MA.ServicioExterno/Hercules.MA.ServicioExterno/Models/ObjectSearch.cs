using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hercules.MA.ServicioExterno.Models
{
    public class Person : ObjectSearch
    {
        public bool searchable { get; set; }
        public string descriptionAuxSearch { get; set; }
        public List<string> researchAreasAuxSearch { get; set; }
        /**
         * Método para buscar dentro de otros ObjectSearch
         */
        public long SearchOwn(string[] pInput)
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
        /**
         * Método para buscar dentro de Person
         */
        public override long Search(string[] pInput)
        {
            long respuesta = 0;
            int encontradoTituloEntero = 0;
            bool encontradoTitulo = true;
            bool encontradoDescripcion = true;
            bool encontradoKnowledgeAreas = true;
            int numKnowledgeAreas = 0;


            // Busca la palabra exacta
            if (Regex.IsMatch(titleAuxSearch, @"\b" + string.Join(" ", pInput).Trim() + @"\b", RegexOptions.IgnoreCase))
            {
                encontradoTituloEntero++;
            }
            // Busca que contenga esta palabra cadena exacta aunque esté precedido por caracteres que no sean espacios
            encontradoTituloEntero = titleAuxSearch.Contains(string.Join(" ", pInput).Trim()) ? encontradoTituloEntero + 1 : encontradoTituloEntero;

            // Busca si TODAS las palabas se encuentran en el título
            foreach (string input in pInput)
            {
                encontradoTitulo = encontradoTitulo && titleAuxSearch.Contains(input);
            }

            foreach (string tag in researchAreasAuxSearch)
            {
                encontradoKnowledgeAreas = true;
                foreach (string input in pInput)
                {
                    encontradoKnowledgeAreas = encontradoKnowledgeAreas && tag.Contains(input);
                }
                if (encontradoKnowledgeAreas)
                {
                    numKnowledgeAreas++;
                }
            }

            // Buscar en las descripciones que aparezca TODAS las palabras de la cadena de texto
            foreach (string input in pInput)
            {
                encontradoDescripcion = encontradoDescripcion && descriptionAuxSearch.Contains(input);
            }


            // Resultados
            respuesta += encontradoTituloEntero * 5000;

            if (encontradoTitulo)
            {
                respuesta += 1000;
            }

            if (numKnowledgeAreas > 0)
            {
                respuesta += 100 * numKnowledgeAreas;
            }

            if (encontradoDescripcion)
            {
                respuesta += 10;
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
            long respuesta = 0;
            int encontradoTituloEntero = 0;
            bool encontradoTitulo = true;
            bool encontradoDescripcion = true;
            int encontradoAutores = 0;
            bool encontradoTags = true;
            int numTags = 0;


            // Busca la palabra exacta
            if (Regex.IsMatch(titleAuxSearch, @"\b" + string.Join(" ", pInput).Trim() + @"\b", RegexOptions.IgnoreCase))
            {
                encontradoTituloEntero++;
            }
            // Busca que contenga esta palabra cadena exacta aunque esté precedido por caracteres que no sean espacios
            encontradoTituloEntero = titleAuxSearch.Contains(string.Join(" ", pInput).Trim()) ? encontradoTituloEntero + 1 : encontradoTituloEntero;

            // Busca si TODAS las palabas se encuentran en el título
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

            // Buscar en las descripciones que aparezca TODAS las palabras de la cadena de texto
            foreach (string input in pInput)
            {
                encontradoDescripcion = encontradoDescripcion && descriptionAuxSearch.Contains(input);
            }


            var tmpEncAutores = 0;
            foreach (Person person in persons)
            {
                Int32.TryParse(person.SearchOwn(pInput).ToString(), out tmpEncAutores);
                encontradoAutores += tmpEncAutores;
            }


            // Resultados
            respuesta += encontradoTituloEntero * 5000;

            if (encontradoTitulo)
            {
                respuesta += 1000;
            }

            if (numTags > 0)
            {
                respuesta += 100 * numTags;
            }

            if (encontradoDescripcion)
            {
                respuesta += 10;
            }

            respuesta += encontradoAutores;

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
            long respuesta = 0;
            int encontradoTituloEntero = 0;
            bool encontradoTitulo = true;
            bool encontradoDescripcion = true;
            int encontradoAutores = 0;
            bool encontradoTags = true;
            int numTags = 0;


            // Busca la palabra exacta
            if (Regex.IsMatch(titleAuxSearch, @"\b" + string.Join(" ", pInput).Trim() + @"\b", RegexOptions.IgnoreCase))
            {
                encontradoTituloEntero++;
            }
            // Busca que contenga esta palabra cadena exacta aunque esté precedido por caracteres que no sean espacios
            encontradoTituloEntero = titleAuxSearch.Contains(string.Join(" ", pInput).Trim()) ? encontradoTituloEntero + 1 : encontradoTituloEntero;

            // Busca si TODAS las palabas se encuentran en el título
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

            // Buscar en las descripciones que aparezca TODAS las palabras de la cadena de texto
            foreach (string input in pInput)
            {
                encontradoDescripcion = encontradoDescripcion && descriptionAuxSearch.Contains(input);
            }


            var tmpEncAutores = 0;
            foreach (Person person in persons)
            {
                Int32.TryParse(person.SearchOwn(pInput).ToString(), out tmpEncAutores);
                encontradoAutores += tmpEncAutores;
            }


            // Resultados
            respuesta += encontradoTituloEntero * 5000;

            if (encontradoTitulo)
            {
                respuesta += 1000;
            }

            if (numTags > 0)
            {
                respuesta += 100 * numTags;
            }

            if (encontradoDescripcion)
            {
                respuesta += 10;
            }

            respuesta += encontradoAutores;

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
            bool encontradoTitulo = true;
            bool encontradoDescripcion = true;
            int encontradoAutores = 0;


            // Busca la palabra exacta
            if (Regex.IsMatch(titleAuxSearch, @"\b" + string.Join(" ", pInput).Trim() + @"\b", RegexOptions.IgnoreCase))
            {
                encontradoTituloEntero++;
            }
            // Busca que contenga esta palabra cadena exacta aunque esté precedido por caracteres que no sean espacios
            encontradoTituloEntero = titleAuxSearch.Contains(string.Join(" ", pInput).Trim()) ? encontradoTituloEntero + 1 : encontradoTituloEntero;

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


            var tmpEncAutores = 0;
            foreach (Person person in persons)
            {
                Int32.TryParse(person.SearchOwn(pInput).ToString(), out tmpEncAutores);
                encontradoAutores += tmpEncAutores;
            }


            // Resultados
            respuesta += encontradoTituloEntero * 5000;

            if (encontradoTitulo)
            {
                respuesta += 1000;
            }

            if (encontradoDescripcion)
            {
                respuesta += 10;
            }

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
            bool encontradoTitulo = true;
            bool encontradoDescripcion = true;
            int encontradoAutores = 0;


            // Busca la palabra exacta
            if (Regex.IsMatch(titleAuxSearch, @"\b" + string.Join(" ", pInput).Trim() + @"\b", RegexOptions.IgnoreCase))
            {
                encontradoTituloEntero++;
            }
            // Busca que contenga esta palabra cadena exacta aunque esté precedido por caracteres que no sean espacios
            encontradoTituloEntero = titleAuxSearch.Contains(string.Join(" ", pInput).Trim()) ? encontradoTituloEntero + 1 : encontradoTituloEntero;

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


            var tmpEncAutores = 0;
            foreach (Person person in persons)
            {
                Int32.TryParse(person.SearchOwn(pInput).ToString(), out tmpEncAutores);
                encontradoAutores += tmpEncAutores;
            }


            // Resultados
            respuesta += encontradoTituloEntero * 5000;

            if (encontradoTitulo)
            {
                respuesta += 1000;
            }

            if (encontradoDescripcion)
            {
                respuesta += 10;
            }

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