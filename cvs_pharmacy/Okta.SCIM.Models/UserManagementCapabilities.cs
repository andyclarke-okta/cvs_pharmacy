using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cvs_SCIM20.Okta.SCIM.Models
{
    public class UserManagementCapabilities
    {
        public static string GROUP_PUSH = "GROUP_PUSH";
        public static string IMPORT_NEW_USERS = "IMPORT_NEW_USERS";
        public static string IMPORT_PROFILE_UPDATES = "IMPORT_PROFILE_UPDATES";
        public static string PUSH_NEW_USERS = "PUSH_NEW_USERS";
        public static string PUSH_PASSWORD_UPDATES = "PUSH_PASSWORD_UPDATES";
        public static string PUSH_PENDING_USERS = "PUSH_PENDING_USERS";
        public static string PUSH_PROFILE_UPDATES = "PUSH_PROFILE_UPDATES";
        public static string PUSH_USER_DEACTIVATION = "PUSH_USER_DEACTIVATION";
        public static string REACTIVATE_USERS = "REACTIVATE_USERS";
    }
}
