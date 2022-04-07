using System.Collections.Generic;

namespace Hercules.MA.ServicioExterno.Models.Cluster
{
    public class Cluster
    {
        public string entityID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<string> terms { get; set; }
        public List<PerfilCluster> profiles { get; set; }
    }

    public class PerfilCluster
    {
        public string entityID { get; set; }
        public string name { get; set; }
        public List<string> terms { get; set; }
        public List<string> tags { get; set; }
        public List<UserCluster> users { get; set; }
    }

    public class UserCluster
    {
        public string userID { get; set; }
        public string name { get; set; }
        public int publications { get; set; }
    }
}
