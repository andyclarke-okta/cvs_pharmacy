using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cvs_SCIM20.Okta.SCIM.Models;

namespace cvs_SCIM_SAML.Services
{
    public interface ICacheService
    {

        bool AddSCIMUser(string externalId, SCIMUser user);

        SCIMUser GetSCIMUser(string externalId);

        bool UpdateSCIMUser(string externalId, SCIMUser user);

        List<SCIMUser> GetAllSCIMUsers();

        /////////////////  Group  /////////////
        bool AddSCIMGroup(string groupId, SCIMGroup group);

        SCIMGroup GetSCIMGroup(string groupId);

        bool UpdateSCIMGroup(string groupId, SCIMGroup group);

        List<SCIMGroup> GetAllSCIMGroups();

        bool DeleteSCIMGroup(string groupId);

        /////////////////  Group membership   /////////////
        bool AddMemberToSCIMGroup(string groupId, Member member);

        bool RemoveMemberFromSCIMGroup(string groupId, Member member);
    }
}
