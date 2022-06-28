using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hercules.MA.ServicioExterno.Models.Similarity
{
    public class EnrichmentSimilarityGetResponse
    {
        public List<object> similar_ros { get; set; }
        public Dictionary<string, Dictionary<string, float>> similar_ros_calculado
        {
            get
            {
                Dictionary<string, Dictionary<string, float>> dic = new Dictionary<string, Dictionary<string, float>>();
                foreach (var item in similar_ros)
                {
                    string id = (string)((Newtonsoft.Json.Linq.JArray)item)[0];
                    if (!dic.ContainsKey(id))
                    {
                        dic.Add(id, new Dictionary<string, float>());
                    }
                    var tags = ((Newtonsoft.Json.Linq.JArray)item)[1];
                    foreach (var tag in tags)
                    {
                        string tagNombre = (string)((Newtonsoft.Json.Linq.JArray)tag)[0];
                        float tagPeso = (float)((Newtonsoft.Json.Linq.JArray)tag)[1];
                        if (!dic[id].ContainsKey(tagNombre))
                        {
                            dic[id].Add(tagNombre, tagPeso);
                        }
                    }
                }

                return dic;
            }
        }
    }
}
