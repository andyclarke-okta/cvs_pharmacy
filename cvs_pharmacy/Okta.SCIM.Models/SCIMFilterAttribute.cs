using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cvs_SCIM20.Okta.SCIM.Models
{
    public class SCIMFilterAttribute
    {
        public String AttributeName { get; set; }

        public String Schema { get; set; }

        public String SubAttributeName { get; set; }
    }
}
