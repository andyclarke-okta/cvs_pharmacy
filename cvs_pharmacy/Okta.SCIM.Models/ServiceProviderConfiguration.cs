using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cvs_SCIM20.Okta.SCIM.Models
{
    public class Patch
    {
        public bool supported { get; set; }
    }

    public class Bulk
    {
        public bool supported { get; set; }
        public int maxOperations { get; set; }
        public int maxPayloadSize { get; set; }
    }

    public class Filter
    {
        public bool supported { get; set; }
        public int maxResults { get; set; }
    }

    public class ChangePassword
    {
        public bool supported { get; set; }
    }

    public class Sort
    {
        public bool supported { get; set; }
    }

    public class Etag
    {
        public bool supported { get; set; }
    }

    public class XmlDataFormat
    {
        public bool supported { get; set; }
    }

    public class AuthenticationScheme
    {
        public string name { get; set; }
        public string description { get; set; }
        public string specUrl { get; set; }
        public string documentationUrl { get; set; }
        public string type { get; set; }
        public bool primary { get; set; }
    }

    public class ServiceProviderConfiguration : SCIMResource
    {
        public List<string> schemas { get; set; }
        public string documentationUrl { get; set; }
        public Patch patch { get; set; }
        public Bulk bulk { get; set; }
        public Filter filter { get; set; }
        public ChangePassword changePassword { get; set; }
        public Sort sort { get; set; }
        public Etag etag { get; set; }
        public XmlDataFormat xmlDataFormat { get; set; }
        public List<AuthenticationScheme> authenticationSchemes { get; set; }
    }
}
