using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cvs_SCIM20.Okta.SCIM.Models;
using System.Runtime.Caching;

namespace cvs_SCIM_SAML.Services
{
    public class CacheService : ICacheService
    {

        private ObjectCache _cache;
        private DateTimeOffset expiration;


        public CacheService()
        {
            ObjectCache cache = MemoryCache.Default;
            this._cache = cache;
            this.expiration = new DateTimeOffset(DateTime.UtcNow).AddMinutes(1000);
        }


        public bool AddSCIMUser(string externalId, SCIMUser user)
        {
            bool response = true;
            _cache.Set(externalId, user, expiration);
            return response;
        }

        public SCIMUser GetSCIMUser(string externalId)
        {

            if (_cache.Contains(externalId))
            {
                return (SCIMUser)_cache[externalId];
            }
            else
            {
                return null;
            }
        }

        public bool UpdateSCIMUser(string externalId, SCIMUser user)
        {
            bool response = true;
            SCIMUser temp = new SCIMUser();
            if (_cache.Contains(externalId))
            {
                _cache.Remove(externalId);
                _cache.Set(externalId, user, expiration);
            }
            else
            {
                _cache.Set(externalId, user, expiration);
            }
            return response;
        }


        public List<SCIMUser> GetAllSCIMUsers()
        {
            List<SCIMUser> uList = new List<SCIMUser>();

            var mylist = MemoryCache.Default.ToList();

            foreach (var item in mylist)
            {

                if (item.Value is cvs_SCIM20.Okta.SCIM.Models.SCIMUser)
                {
                    uList.Add((SCIMUser)item.Value);
                }
            }

            return uList;
        }

        //////////  Groups //////////////

        public bool AddSCIMGroup(string groupId, SCIMGroup group)
        {
            bool response = true;
            _cache.Set(groupId, group, expiration);
            return response;
        }

        public SCIMGroup GetSCIMGroup(string groupId)
        {

            if (_cache.Contains(groupId))
            {
                return (SCIMGroup)_cache[groupId];
            }
            else
            {
                return null;
            }
        }

        public bool UpdateSCIMGroup(string groupId, SCIMGroup group)
        {
            bool response = true;
            SCIMGroup temp = new SCIMGroup();
            if (_cache.Contains(groupId))
            {
                _cache.Remove(groupId);
                _cache.Set(groupId, group, expiration);
            }
            else
            {
                _cache.Set(groupId, group, expiration);
            }
            return response;
        }

        public List<SCIMGroup> GetAllSCIMGroups()
        {
            List<SCIMGroup> uList = new List<SCIMGroup>();

            var mylist = MemoryCache.Default.ToList();

            foreach (var item in mylist)
            {
                if (item.Value is cvs_SCIM20.Okta.SCIM.Models.SCIMGroup)
                {
                    uList.Add((SCIMGroup)item.Value);
                }
            }

            return uList;
        }

        public bool DeleteSCIMGroup(string groupId)
        {
            bool response = true;
            if (_cache.Contains(groupId))
            {
                _cache.Remove(groupId);
                response = true;
            }

            return response;
        }

        /////////////////  Group membership   /////////////

        public bool AddMemberToSCIMGroup(string groupId, Member member)
        {
            bool response = true;
            bool added = false;
            SCIMGroup tempGroup = new SCIMGroup();
            if (_cache.Contains(groupId))
            {
                tempGroup = (SCIMGroup)_cache[groupId];
                _cache.Remove(groupId);

                foreach (var item in tempGroup.members)
                {
                    if (item.value == member.value)
                    {
                        tempGroup.members.Remove(item);
                        tempGroup.members.Add(member);
                        added = true;
                        break;
                    }

                }

                if (!added)
                {
                    tempGroup.members.Add(member);
                }

                _cache.Set(groupId, tempGroup, expiration);
            }

            return response;
        }


        public bool RemoveMemberFromSCIMGroup(string groupId, Member member)
        {
            bool response = true;
            SCIMGroup tempGroup = new SCIMGroup();
            if (_cache.Contains(groupId))
            {
                tempGroup = (SCIMGroup)_cache[groupId];
                _cache.Remove(groupId);

                // tempGroup.members.Remove(member);

                foreach (var item in tempGroup.members)
                {
                    if (item.value == member.value)
                    {
                        tempGroup.members.Remove(item);
                        break;
                    }

                }

                _cache.Set(groupId, tempGroup, expiration);
            }

            return response;
        }

    }
}
