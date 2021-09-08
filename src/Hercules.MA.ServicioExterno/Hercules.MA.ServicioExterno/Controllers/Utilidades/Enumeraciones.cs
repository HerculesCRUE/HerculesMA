using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hercules.MA.ServicioExterno.Controllers.Utilidades
{
    public class UtilsCadenas
    {
        public static string PALABRAS_DESCARTADAS = ",de,en,y,del,la,";
        public static string ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(string pTexto)
        {
            pTexto = pTexto.ToLower();
            string textoFinal = "";

            if (pTexto != null && !string.IsNullOrWhiteSpace(pTexto))
            {
                char[] separador = { ' ' };
                string[] palabras = pTexto.Split(separador);
                string palabra2;

                foreach (string palabra in palabras)
                {
                    palabra2 = palabra;

                    if (palabra.Contains("+") && palabra.Length >= palabra.IndexOf("+") + 2)
                    {
                        palabra2 = palabra.Substring(0, palabra.IndexOf("+") + 1) + palabra.Substring(palabra.IndexOf("+") + 1, 1).ToUpper() + palabra.Substring(palabra.IndexOf("+") + 2) + " ";
                    }

                    if (!PALABRAS_DESCARTADAS.Contains("," + palabra2 + ","))
                    {
                        if (palabra2.Length > 1)
                        {
                            textoFinal += palabra2.Substring(0, 1).ToUpper() + palabra2.Substring(1) + " ";
                        }
                        else if (palabra2.Length == 1)
                        {
                            textoFinal += palabra2.ToUpper() + " ";
                        }
                    }
                    else
                    {
                        textoFinal += palabra2 + " ";
                    }
                }

                if (textoFinal.Length > 0 && textoFinal[textoFinal.Length - 1] == ' ')
                {
                    textoFinal = textoFinal.Substring(0, textoFinal.Length - 1);
                }
            }
            else
            {
                textoFinal = pTexto;
            }

            return textoFinal;
        }
    }
}