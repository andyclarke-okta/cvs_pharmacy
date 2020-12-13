using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cvs_SCIM20.Okta.SCIM.Models
{
    public class PaginationProperties
    {
        public int count { get; set; }
        public int startIndex { get; set; }

        public PaginationProperties(int c, int s)
        {
            count = c;
            startIndex = s;
        }

        public PaginationProperties(string c, string s)
        {
            count = Convert.ToInt32(c);
            startIndex = Convert.ToInt32(s);
        }
    }
}
