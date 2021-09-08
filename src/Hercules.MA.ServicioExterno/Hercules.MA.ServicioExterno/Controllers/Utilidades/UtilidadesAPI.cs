using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hercules.MA.ServicioExterno.Controllers.Utilidades
{
    public class UtilidadesAPI
    {
        public static string GetValorFilaSparqlObject(Dictionary<string, SparqlObject.Data> pFila, string pParametro)
        {
            if (pFila.ContainsKey(pParametro) && !string.IsNullOrEmpty(pFila[pParametro].value))
            {
                return pFila[pParametro].value;
            }

            return null;
        }

        public static string GetValorFilaSparqlDia(Dictionary<string, SparqlObject.Data> pFila, string pParametro)
        {
            if (pFila.ContainsKey(pParametro) && !string.IsNullOrEmpty(pFila[pParametro].value))
            {
                return pFila[pParametro].value.Substring(0, 8) + "000000";
            }

            return null;
        }

        public static bool ModificarTriplesRecurso(ResourceApi pResourceApi, Guid pRecursoID, List<TriplesToModify> pTriples)
        {
            List<TriplesToModify> triplesInsertar = new List<TriplesToModify>();

            foreach (TriplesToModify triple in pTriples)
            {
                if (triple.NewValue == "")
                {
                    triple.NewValue = null;
                }

                if (triple.OldValue == "")
                {
                    triple.OldValue = null;
                }

                if (triple.OldValue != triple.NewValue)
                {
                    triplesInsertar.Add(triple);
                }
            }

            Dictionary<Guid, List<TriplesToModify>> dicTriplesModificar = new Dictionary<Guid, List<TriplesToModify>>();
            dicTriplesModificar.Add(pRecursoID, triplesInsertar);
            Dictionary<Guid, bool> dicInsertado = pResourceApi.ModifyPropertiesLoadedResources(dicTriplesModificar);

            return (dicInsertado != null && dicInsertado.ContainsKey(pRecursoID) && dicInsertado[pRecursoID]);
        }
    }
}