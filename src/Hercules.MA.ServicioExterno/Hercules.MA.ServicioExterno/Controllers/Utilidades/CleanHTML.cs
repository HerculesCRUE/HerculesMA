using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;



namespace Hercules.MA.ServicioExterno.Controllers.Utilidades
{
    public class CleanHTML
    {

        static Regex _AttrStyleRegex = new Regex("[^style]+[ ]?=[ ]?\"[^\"]*\"", RegexOptions.Compiled);

        /// <summary>
        /// Método para limpiar un string de tags, a excepción de los permitidos
        /// El método también elimina los atributos menos el "style"
        /// </summary>
        /// <param name="source">Texto de entrada.</param>
        /// <param name="tagsExceptions">Array con los tags "excepcionales".</param>
        /// <returns>string resultante.</returns>
        public static string StripTagsCharArray(string source, string[] tagsExceptions)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;
            List<char> currentTag = new();
            List<char> attrs = new();
            string tag = "";
            string attrsTag = "";

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                // Comprueba si es empieza un tag y lo guarda ahí
                // Los caracteres siguientes se guardarán como tag o atributo hasta el cierre del tag
                if (let == '<')
                {
                    inside = true;
                    currentTag = new();
                    attrs = new();
                    currentTag.Add(let);
                    continue;
                }
                if (let == '>')
                {
                    // Vuelve a guardar el texto, no lo guarda como tag
                    inside = false;
                    currentTag.Add(let);
                    tag = String.Join("", currentTag);
                    attrsTag = String.Join("", attrs);
                    if (tagsExceptions.Contains(tag) && !tag.Contains("script") && !attrsTag.Contains("script")) {
                        for (int n = 0; n < currentTag.Count -1; n++)
                        {
                            array[arrayIndex] = currentTag[n];
                            arrayIndex++;
                        }
                        // Añade los atributos (si es style)
                        if (attrsTag.Length > 0)
                        {
                            attrsTag = _AttrStyleRegex.Replace(attrsTag, string.Empty);
                            attrsTag = attrsTag.Contains('"') ? attrsTag : "";
                            for (int n = 0; n < attrsTag.Length; n++)
                            {
                                array[arrayIndex] = attrsTag[n];
                                arrayIndex++;
                            }
                        }
                        array[arrayIndex] = let;
                        arrayIndex++;
                    }
                    continue;
                }
                // !inside signofica que no estamos dentro de un tag
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                } else
                {
                    // Estamos dentro de un tag, por lo que se guardará como tag o atributo
                    if (attrs != null && attrs.Count > 0)
                    {
                        attrs.Add(let);
                    } else if (let == ' ')
                    {
                        attrs.Add(let);
                    } else
                    {
                        currentTag.Add(let);
                    }
                }
            }
            return new string(array, 0, arrayIndex);
        }
    }
}