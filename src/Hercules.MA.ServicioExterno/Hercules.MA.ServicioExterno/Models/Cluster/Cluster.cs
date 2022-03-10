using System.Collections.Generic;

namespace Hercules.MA.ServicioExterno.Models.Cluster
{
    public class Cluster
    {
        public string entityID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<string> terms { get; set; }
        public IEnumerable<PerfilCluster> profiles { get; set; }
    }

    public class PerfilCluster
    {
        public string entityID { get; set; }
        public string name { get; set; }
        public List<string> terms { get; set; }
        public List<string> tags { get; set; }
        public List<string> users { get; set; }
    }
}
