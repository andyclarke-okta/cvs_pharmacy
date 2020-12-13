using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cvs_SCIM20.Okta.SCIM.Models;

namespace cvs_SCIM_SAML.Connectors
{
    public interface ISCIMConnector
    {
        SCIMUser createUser(SCIMUser user);
        SCIMUser getUser(String id);
        SCIMUserQueryResponse getUsers(PaginationProperties pageProperties, SCIMFilter filter);
        SCIMUser deactivateUser(String id);
        SCIMUser reactivateUser(String id);
        SCIMUser updateUser(SCIMUser user);

        //Groups
        SCIMGroup createGroup(SCIMGroup group);
        SCIMGroup getGroup(String id);
        SCIMGroupQueryResponse getGroups(PaginationProperties pageProperties);
        bool deleteGroup(String id);
        SCIMGroup updateGroup(SCIMGroup group);

        //Group Membeship
        bool addGroupMember(string id, Member member);
        bool removeGroupMember(string id, Member member);
        ServiceProviderConfiguration getServiceProviderConfig();
    }
}
