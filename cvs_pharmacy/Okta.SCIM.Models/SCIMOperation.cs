using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cvs_SCIM20.Okta.SCIM.Models
{
    public class SCIMUserOperation
    {

        public List<string> schemas { get; set; }
        public List<UserOperation> Operations { get; set; }
        public class UserOperation
        {
            public string op { get; set; }
            public UserValue value { get; set; }
        }

        public class UserValue
        {
            public bool active { get; set; }
        }

    }



    public class SCIMGroupOperation
    {
        public string[] schemas { get; set; }
        public GroupOperation[] Operations { get; set; }
    }

    public class GroupOperation
    {
        public string op { get; set; }
        public string path { get; set; }
        public GroupValue[] value { get; set; }
    }

    public class GroupValue
    {
        public string value { get; set; }
        public string display { get; set; }
    }
}
