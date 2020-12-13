using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cvs_SCIM20.Okta.SCIM.Models
{
    public class Member
    {
        public string value { get; set; }
        public string display { get; set; }
    }

    public class SCIMGroup : SCIMResource
    {
        public string displayName { get; set; }
        public List<Member> members { get; set; }
    }
}
