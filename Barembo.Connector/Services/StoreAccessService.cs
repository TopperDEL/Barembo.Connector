using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using uplink.NET.Models;

namespace Barembo.Services
{
    public class StoreAccessService : IStoreAccessService
    {
        public StoreAccess GenerateAccesFromLogin(LoginData loginData)
        {
            var access = new Access(loginData.SatelliteAddress, loginData.ApiKey, loginData.Secret);

            return new StoreAccess(access.Serialize());
        }

        public StoreAccess ShareBookAccess(StoreAccess baseAccess, BookReference bookReferenceToShare, Contributor contributor, AccessRights accessRights)
        {
            var access = new Access(baseAccess.AccessGrant);
            Permission permission = new Permission();
            permission.AllowDelete = true;
            permission.AllowDownload = true;
            permission.AllowList = true;
            permission.AllowUpload = true;

            List<SharePrefix> prefixes = new List<SharePrefix>();

            var sharedAccess = access.Share(permission, prefixes);

            return new StoreAccess(sharedAccess.Serialize());
        }

        public StoreAccess ShareBookShareAccess(StoreAccess baseAccess, StoreKey storeKey)
        {
            var access = new Access(baseAccess.AccessGrant);
            Permission permission = new Permission();
            permission.AllowDelete = false;
            permission.AllowDownload = true;
            permission.AllowList = false;
            permission.AllowUpload = false;

            List<SharePrefix> prefixes = new List<SharePrefix>();
            prefixes.Add(new SharePrefix() { Bucket = "", Prefix = "" });

            var sharedAccess = access.Share(permission, prefixes);

            return new StoreAccess(sharedAccess.Serialize());
        }
    }
}
