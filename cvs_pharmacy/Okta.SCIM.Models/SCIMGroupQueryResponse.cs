using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cvs_SCIM20.Okta.SCIM.Models
{
    public class SCIMGroupQueryResponse : SCIMResourceQueryResponse
    {
        public List<SCIMGroup> Resources { get; set; }
    }
}
