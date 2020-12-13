using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cvs_SCIM20.Okta.SCIM.Models;
using cvs_SCIM_SAML.Services;
using cvs_SCIM20.Exceptions;


using System.Collections.Concurrent;

using System.Web;

//using System.Web.Http;
//using System.Web.Http.Tracing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
//using log4net;
using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace cvs_SCIM_SAML.Connectors
{
    public class InMemorySCIMConnector : ISCIMConnector
    {
        private readonly ILogger<InMemorySCIMConnector> _logger;
        private readonly IConfiguration _config;

        private ICacheService _cacheService;

        private ConcurrentDictionary<string, SCIMUser> _users = null;
        private ConcurrentDictionary<string, SCIMGroup> _groups = null;
        private string _custom_externtion_urn;

        public static List<string> listResponseSchemas;
        private static List<string> userSchemas = null;
        private static List<string> groupSchemas = null;
        public InMemorySCIMConnector(ICacheService cacheService, ILogger<InMemorySCIMConnector> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _cacheService = cacheService;
            _custom_externtion_urn = _config.GetValue<string>("Scim:custom_externtion_urn");

            listResponseSchemas = new List<string>();
            listResponseSchemas.Add("urn:ietf:params:scim:schemas:core:2.0");
            listResponseSchemas.Add("urn:ietf:params:scim:api:messages:2.0:ListResponse");

            userSchemas = new List<string>();
            userSchemas.Add("urn:ietf:params:scim:schemas:core:2.0");
            userSchemas.Add("urn:ietf:params:scim:schemas:core:2.0:User");
            userSchemas.Add("urn:ietf:params:scim:schemas:extension:enterprise:2.0");
            userSchemas.Add("urn:ietf:params:scim:schemas:extension:enterprise:2.0:User");

            groupSchemas = new List<string>();
            groupSchemas.Add("urn:ietf:params:scim:schemas:core:2.0");
            groupSchemas.Add("urn:ietf:params:scim:schemas:core:2.0:Group");



            //SCIM1.1 and SCIM2.0 use different schemas
            //schemas are in the json files
            //string user_json = appSettings["okta.scim_user"];
            //string group_json = appSettings["okta.scim_group"];

            string user_json = _config.GetValue<string>("Scim:Users");
            string group_json = _config.GetValue<string>("scim:Groups");


            //check for users assume group
            if (_users == null)
            {
                _users = new ConcurrentDictionary<string, SCIMUser>();
                _groups = new ConcurrentDictionary<string, SCIMGroup>();

                FileStream fileStream = new FileStream("App_Data/2Users_SCIM20.json", FileMode.Open);
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    SCIMUserQueryResponse response =
                        (SCIMUserQueryResponse)serializer.Deserialize(reader, typeof(SCIMUserQueryResponse));
                    foreach (SCIMUser u in response.Resources)
                    {
                        createUser(u);
                    }
                    //userSchemas = response.schemas.ToList<string>();
                }


                FileStream fileStream1 = new FileStream("App_Data/2Groups_SCIM20.json", FileMode.Open);
                using (StreamReader reader = new StreamReader(fileStream1))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    SCIMGroupQueryResponse response =
                        (SCIMGroupQueryResponse)serializer.Deserialize(reader, typeof(SCIMGroupQueryResponse));
                    foreach (SCIMGroup g in response.Resources)
                    {
                        createGroup(g);
                    }
                    //groupSchemas = response.schemas.ToList<string>();
                }
            }
        }

        //Users
        public SCIMUser createUser(SCIMUser user)
        {
            SCIMUser tempUser = new SCIMUser();
            bool result = false;
            try
            {
                if (user.id == null)
                {
                    user.id = Guid.NewGuid().ToString();
                }

                tempUser = _cacheService.GetSCIMUser(user.id);
                if (tempUser != null)
                {
                    // duplicate user
                    throw new OnPremUserManagementException("User id already exists");
                }
                else
                {
                    result = _cacheService.AddSCIMUser(user.id, user);
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            return user;
        }
        public SCIMUser getUser(string id)
        {
            SCIMUser user = null;
            try
            {
                //users.TryGetValue(id, out user);
                user = _cacheService.GetSCIMUser(id);
            }
            catch (Exception e)
            {
                throw new OnPremUserManagementException("getUser", e);
            }
            if (user == null)
            {
                throw new EntityNotFoundException(id);
            }
            return user;
        }
        private List<SCIMUser> filterUsers(List<SCIMUser> users, SCIMFilter filter)
        {
            //System.Diagnostics.Debug.WriteLine(filter.FilterAttribute.AttributeName);
            _logger.LogDebug("filterUsers type " + filter.FilterAttribute + " value " + filter.FilterValue);
            List<SCIMUser> filteredUsers = new List<SCIMUser>();
            string criteria = filter.FilterValue.Replace("\"", "");


            // simple attribute
            // dictionary attribute
            // equals query
            // or query

            if (filter.FilterType == SCIMFilterType.EQUALS)
            {
                if (filter.FilterAttribute.AttributeName == "userName")
                {
                    //check userName equals any existing users
                    foreach (var item in users)
                    {
                        if (item.userName == criteria)
                        {
                            filteredUsers.Add(item);
                        }
                    }

                }
            }

            return filteredUsers;
        }
        public SCIMUserQueryResponse getUsers(PaginationProperties pageProperties, SCIMFilter filter)
        {
            _logger.LogDebug("getUsers by filter ");
            int totalRecords = 0;
            try
            {

                //List<SCIMUser> uList = users.Values.ToList<SCIMUser>();

                List<SCIMUser> uList = _cacheService.GetAllSCIMUsers();
                List<SCIMUser> fList = null;

                if (filter != null)
                {
                    fList = filterUsers(uList, filter);
                    totalRecords = fList.Count;
                }
                else
                {
                    totalRecords = uList.Count;
                }


                int startIndex = pageProperties.startIndex > 0 && pageProperties.startIndex <= totalRecords ? pageProperties.startIndex : 1;
                int recordCount = startIndex + pageProperties.count <= totalRecords ? pageProperties.count : totalRecords - startIndex + 1;

                SCIMUserQueryResponse response = new SCIMUserQueryResponse();
                //response.schemas = userSchemas;
                response.schemas = listResponseSchemas;
                response.totalResults = totalRecords;
                response.startIndex = startIndex;
                response.itemsPerPage = recordCount;
                if (filter != null)
                {
                    response.Resources = fList.GetRange(startIndex - 1, recordCount);
                }
                else
                {
                    response.Resources = uList.GetRange(startIndex - 1, recordCount);
                }



                return response;
            }
            catch (Exception e)
            {
                throw new OnPremUserManagementException("getUsers", e);
            }
        }
        public SCIMUser updateUser(SCIMUser user)
        {
            bool result = false;
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            SCIMUser existingUser;
            //if (users.TryGetValue(user.id, out existingUser))
            //{
            //    result = users.TryUpdate(user.id, user, existingUser);
            //}
            existingUser = _cacheService.GetSCIMUser(user.id);
            if (existingUser == null)
            {
                throw new OnPremUserManagementException("User Not Found for Update");
            }
            else
            {
                result = _cacheService.UpdateSCIMUser(user.id, user);
            }

            return user;
        }
        public SCIMUser deactivateUser(string id)
        {
            bool result = false;
            SCIMUser existingUser;

            existingUser = _cacheService.GetSCIMUser(id);
            if (existingUser == null)
            {
                //throw new OnPremUserManagementException("User Not Found for Deactivate");
                result = true;
            }
            else
            {
                //do not remove user, chnage active flag to false
                existingUser.active = false;
                result = _cacheService.UpdateSCIMUser(id, existingUser);
                //retieve updated user for API return
                existingUser = _cacheService.GetSCIMUser(id);
            }

            return existingUser;
        }
        public SCIMUser reactivateUser(string id)
        {
            bool result = false;
            SCIMUser existingUser;

            existingUser = _cacheService.GetSCIMUser(id);
            if (existingUser == null)
            {
                //throw new OnPremUserManagementException("User Not Found for Reactivate");
                result = true;
            }
            else
            {
                existingUser.active = true;
                result = _cacheService.UpdateSCIMUser(id, existingUser);
                //retieve updated user for API return
                existingUser = _cacheService.GetSCIMUser(id);
            }
            return existingUser;
        }
        // Groups
        public SCIMGroup createGroup(SCIMGroup group)
        {
            SCIMGroup tempGroup = new SCIMGroup();

            try
            {
                if (group.id == null)
                {
                    group.id = Guid.NewGuid().ToString();
                }

                tempGroup = _cacheService.GetSCIMGroup(group.id);
                if (tempGroup != null)
                {
                    // duplicate user
                    throw new OnPremUserManagementException("Group id already exists");
                }
                else
                {
                    _cacheService.AddSCIMGroup(group.id, group);
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            return group;
        }


        public SCIMGroup getGroup(string id)
        {
            SCIMGroup group = null;
            try
            {
                group = _cacheService.GetSCIMGroup(id);
            }
            catch (Exception e)
            {
                throw new OnPremUserManagementException("getGroup", e);
            }
            if (group == null)
            {
                throw new EntityNotFoundException(id);
            }
            return group;
        }
        public SCIMGroupQueryResponse getGroups(PaginationProperties pp)
        {
            int totalRecords = 0;
            try
            {

                List<SCIMGroup> uList = _cacheService.GetAllSCIMGroups();

                totalRecords = uList.Count;

                int startIndex = pp.startIndex > 0 && pp.startIndex <= totalRecords ? pp.startIndex : 1;
                int recordCount = startIndex + pp.count <= totalRecords ? pp.count : totalRecords - startIndex + 1;

                SCIMGroupQueryResponse response = new SCIMGroupQueryResponse();

                response.schemas = listResponseSchemas;
                response.totalResults = totalRecords;
                response.startIndex = startIndex;
                response.itemsPerPage = recordCount;

                response.Resources = uList.GetRange(startIndex - 1, recordCount);

                return response;
            }
            catch (Exception e)
            {
                throw new OnPremUserManagementException("getGroups", e);
            }

        }

        //
        public bool deleteGroup(string id)
        {

            bool result = false;
            result = _cacheService.DeleteSCIMGroup(id);

            return result;
        }

        //
        public SCIMGroup updateGroup(SCIMGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            SCIMGroup existingGroup;

            existingGroup = _cacheService.GetSCIMGroup(group.id);
            if (existingGroup == null)
            {
                throw new OnPremUserManagementException("Group Not Found for Update");
            }
            else
            {
                _cacheService.UpdateSCIMGroup(group.id, group);
            }

            return group;

        }

        //
        public bool addGroupMember(string id, Member member)
        {
            bool rspMember = false;

            rspMember = _cacheService.AddMemberToSCIMGroup(id, member);

            return rspMember;
        }

        //
        public bool removeGroupMember(string id, Member member)
        {
            bool rspMember = false;

            rspMember = _cacheService.RemoveMemberFromSCIMGroup(id, member);

            return rspMember;
        }


        //
        public ServiceProviderConfiguration getServiceProviderConfig()
        {
            //throw new NotImplementedException();
            //var appSettings = ConfigurationManager.AppSettings;
            ServiceProviderConfiguration cfg = new ServiceProviderConfiguration();


            cfg.schemas = new List<string>() { "urn:schemas:core:1.0", "urn:okta:schemas:scim:providerconfig:1.0" };
            List<string> usermgmt = new List<string>() {
                UserManagementCapabilities.PUSH_PASSWORD_UPDATES,
                UserManagementCapabilities.GROUP_PUSH,
                UserManagementCapabilities.IMPORT_NEW_USERS,
                UserManagementCapabilities.IMPORT_PROFILE_UPDATES,
                UserManagementCapabilities.PUSH_NEW_USERS,
                UserManagementCapabilities.PUSH_PROFILE_UPDATES
                };
            cfg.ExtensionData = new Dictionary<string, object>();
            //cfg.ExtensionData.Add(appSettings["okta.custom_extension_urn"], usermgmt);
            cfg.ExtensionData.Add(_custom_externtion_urn, usermgmt);

            

            //cfg.schemas = new List<string>() { "urn:schemas:core:1.0", "urn:okta:schemas:scim:providerconfig:1.0" };
            //List<string> usermgmt = new List<string>() {
            //    UserManagementCapabilities.PUSH_PASSWORD_UPDATES,
            //    UserManagementCapabilities.GROUP_PUSH,
            //    UserManagementCapabilities.IMPORT_NEW_USERS,
            //    UserManagementCapabilities.IMPORT_PROFILE_UPDATES,
            //    UserManagementCapabilities.PUSH_NEW_USERS,
            //    UserManagementCapabilities.PUSH_PROFILE_UPDATES
            //    };
            //cfg.ExtensionData = new Dictionary<string, object>();
            //cfg.ExtensionData.Add("urn:okta:schemas:scim:providerconfig:1.0", usermgmt);


            return cfg;
        }
    }
}
