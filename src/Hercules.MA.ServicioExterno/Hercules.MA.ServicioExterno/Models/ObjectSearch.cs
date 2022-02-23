using System;
using System.Collections.Generic;

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
            if(encontrado)
            {
                respuesta = 1;
            }
            return respuesta;
        }
    }

    public class Publication : ObjectSearch
    {
        public HashSet<string> tagsAuxSearch { get; set; }
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
                encontradoAutores = encontradoAutores && person.Search(pInput)>0;
                if (encontradoAutores)
                {
                    numAutores++;
                }
            }

            if(encontradoTitulo)
            {
                respuesta += 1000;
            }
            if (encontradoTags)
            {
                respuesta += 100*numTags;
            }
            if (encontradoDescripcion)
            {
                respuesta += 10;
            }
            if (encontradoAutores)
            {
                respuesta += 1*numAutores;
            }
            return respuesta;
        }
    }

    public class ResearchObject : ObjectSearch
    {
        public HashSet<string> tagsAuxSearch { get; set; }
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
            throw new NotImplementedException();
        }
    }

    public class Project : ObjectSearch
    {
        public string descriptionAuxSearch { get; set; }
        public HashSet<Person> persons { get; set; }

        public override long Search(string[] pInput)
        {
            throw new NotImplementedException();
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
