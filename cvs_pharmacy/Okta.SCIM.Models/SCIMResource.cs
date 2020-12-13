using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cvs_SCIM20.Okta.SCIM.Models
{
    public class SCIMResource
    {
        public class Meta
        {
            public string created { get; set; }
            public string lastModified { get; set; }
            public string version { get; set; }
            public string location { get; set; }
        }

        public string id { get; set; }
        public string externalId { get; set; }
        public Meta meta { get; set; }
        public List<string> schemas { get; set; }

        // this is required to ensure that undefined properties (e.g., new or extension values in the json) are captured.
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }
    }
}
