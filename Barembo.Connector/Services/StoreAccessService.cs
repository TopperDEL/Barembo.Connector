using Barembo.Helper;
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
        readonly string _bucketName;

        public StoreAccessService() : this("barembo")
        {
        }

        public StoreAccessService(string bucketName)
        {
            _bucketName = bucketName;
        }

        public StoreAccess GenerateAccessFromLogin(LoginData loginData)
        {
            var access = new Access(loginData.SatelliteAddress, loginData.ApiKey, loginData.Secret);

            return new StoreAccess(access.Serialize());
        }

        public StoreAccess ShareBookAccess(StoreAccess baseAccess, BookReference bookReferenceToShare, Contributor contributor, AccessRights accessRights)
        {
            var access = new Access(baseAccess.AccessGrant);
            Permission permission = new Permission();
            permission.AllowDelete = accessRights.CanDeleteEntries || accessRights.CanDeleteForeignEntries;
            permission.AllowDownload = accessRights.CanReadEntries || accessRights.CanReadForeignEntries || accessRights.CanEditBook || accessRights.CanEditForeignEntries || accessRights.CanEditOwnEntries;
            permission.AllowList = true;
            permission.AllowUpload = true;

            var bookStoreKey = StoreKey.Book(bookReferenceToShare.BookId);

            List<SharePrefix> prefixes = new List<SharePrefix>();
            prefixes.Add(new SharePrefix { Bucket = _bucketName, Prefix = StoreKeyHelper.Convert(bookStoreKey) });

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
            prefixes.Add(new SharePrefix { Bucket = _bucketName, Prefix = StoreKeyHelper.Convert(storeKey) });

            var sharedAccess = access.Share(permission, prefixes);

            return new StoreAccess(sharedAccess.Serialize());
        }

        public string GetVersionInfo()
        {
            return Access.GetStorjVersion();
        }
    }
}
