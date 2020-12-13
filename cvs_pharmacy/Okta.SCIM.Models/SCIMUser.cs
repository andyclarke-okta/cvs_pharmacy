using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cvs_SCIM20.Okta.SCIM.Models
{
    public class OktaId
    {
        public string internalId { get; set; }
    }



    public class Name
    {
        public string formatted { get; set; }
        public string familyName { get; set; }
        public string givenName { get; set; }
        public string middleName { get; set; }
        public string honorificPrefix { get; set; }
        public string honorificSuffix { get; set; }
    }

    public class Email
    {
        public string value { get; set; }
        public string type { get; set; }
        public bool primary { get; set; }
    }

    public class Address
    {
        public string type { get; set; }
        public string streetAddress { get; set; }
        public string locality { get; set; }
        public string region { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
        public string formatted { get; set; }
        public bool primary { get; set; }
    }

    public class PhoneNumber
    {
        public string value { get; set; }
        public string type { get; set; }
        public bool primary { get; set; }
    }

    public class Im
    {
        public string value { get; set; }
        public string type { get; set; }
    }

    public class Photo
    {
        public string value { get; set; }
        public string type { get; set; }
    }

    public class X509Certificates
    {
        public string value { get; set; }
    }



    public class SCIMUser : SCIMResource
    {
        public string userName { get; set; }
        public Name name { get; set; }
        public string displayName { get; set; }
        public string nickName { get; set; }
        public string profileUrl { get; set; }
        public List<Email> emails { get; set; }
        public List<Address> addresses { get; set; }
        public List<PhoneNumber> phoneNumbers { get; set; }
        public List<Im> ims { get; set; }
        public List<Photo> photos { get; set; }
        public string userType { get; set; }
        public string title { get; set; }
        public string preferredLanguage { get; set; }
        public string locale { get; set; }
        public string timezone { get; set; }
        public bool active { get; set; }
        public string password { get; set; }
        public List<Member> groups { get; set; }
        public List<X509Certificates> x509Certificates { get; set; }

    }
}
