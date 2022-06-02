using System;
using System.Collections.Generic;

namespace Hercules.MA.ServicioExterno.Models.Offer
{
    public class Offer
    {
        public string entityID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<string> terms { get; set; }
    }

    public class UsersOffer
    {
        public string id { get; set; }
        public Guid shortId { get; set; }
        public string name { get; set; }
        public int numPublicaciones { get; set; }
        public int numPublicacionesTotal { get; set; }
        public int ipNumber { get; set; }
        public string organization { get; set; }
        public string hasPosition { get; set; }
        public string departamento { get; set; }
        public List<string> groups { get; set; }
    }

}
