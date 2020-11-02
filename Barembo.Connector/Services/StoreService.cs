using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using uplink.NET.Interfaces;
using uplink.NET.Models;
using uplink.NET.Services;
using System.Linq;
using Barembo.Helper;
using Barembo.Exceptions;

namespace Barembo.Services
{
    public class StoreService : IStoreService
    {
        private static Dictionary<string, IBucketService> _bucketServiceInstances = new Dictionary<string, IBucketService>();
        private static Dictionary<string, IObjectService> _objectServiceInstances = new Dictionary<string, IObjectService>();
        private static Dictionary<string, Bucket> _BucketInstances = new Dictionary<string, Bucket>();

        readonly string _bucketName;

        public StoreService() : this("barembo")
        {
        }

        public StoreService(string bucketName)
        {
            _bucketName = bucketName;
        }

        public async Task<Stream> GetObjectAsStreamAsync(StoreAccess access, StoreKey storeKey)
        {
            var bucket = await GetBucketAsync(_bucketName, access).ConfigureAwait(false);

            var objectInfo = await GetObjectInfoAsync(access, storeKey).ConfigureAwait(false);
            if (objectInfo.ObjectExists)
            {
                return new DownloadStream(bucket, (int)objectInfo.Size, storeKey.ToString());
            }
            else
                return null;
        }

        public async Task<T> GetObjectFromJsonAsync<T>(StoreAccess access, StoreKey storeKey)
        {
            var objectService = GetObjectService(access);
            var bucket = await GetBucketAsync(_bucketName, access).ConfigureAwait(false);

            var download = await objectService.DownloadObjectAsync(bucket, storeKey.ToString(), new DownloadOptions(), false);
            await download.StartDownloadAsync();

            if (download.Completed)
            {
                return DeserializeFromJSON<T>(download.DownloadedBytes);
            }
            else
                return default(T);
        }

        public async Task<StoreObjectInfo> GetObjectInfoAsync(StoreAccess access, StoreKey storeKey)
        {
            var objectService = GetObjectService(access);
            var bucket = await GetBucketAsync(_bucketName, access).ConfigureAwait(false);

            try
            {
                var objectInfo = await objectService.GetObjectAsync(bucket, storeKey.ToString());
                return new StoreObjectInfo { ObjectExists = true, Size = objectInfo.SystemMetaData.ContentLength };
            }
            catch (Exception)
            {
                return new StoreObjectInfo { ObjectExists = false };
            }
        }

        public async Task<IEnumerable<StoreObject>> ListObjectsAsync(StoreAccess access, StoreKey storeKey)
        {
            var objectService = GetObjectService(access);
            var bucket = await GetBucketAsync(_bucketName, access).ConfigureAwait(false);

            var listObjectsOption = new ListObjectsOptions();
            listObjectsOption.Prefix = storeKey.ToString();
            listObjectsOption.Recursive = true;

            var objects = await objectService.ListObjectsAsync(bucket, listObjectsOption);

            return objects.Items.Select(i => new StoreObject(i.Key, StoreKeyHelper.GetStoreObjectId(i.Key)));
        }

        public async Task<bool> PutObjectAsJsonAsync<T>(StoreAccess access, StoreKey storeKey, T objectToPut)
        {
            var objectService = GetObjectService(access);
            var bucket = await GetBucketAsync(_bucketName, access).ConfigureAwait(false);

            var JSONBytes = SerializeToJSON(objectToPut);

            var upload = await objectService.UploadObjectAsync(bucket, storeKey.ToString(), new UploadOptions(), JSONBytes, false);
            await upload.StartUploadAsync();

            return upload.Completed;
        }

        public async Task<bool> PutObjectFromStreamAsync(StoreAccess access, StoreKey storeKey, Stream objectToPut)
        {
            var objectService = GetObjectService(access);
            var bucket = await GetBucketAsync(_bucketName, access).ConfigureAwait(false);

            var upload = await objectService.UploadObjectAsync(bucket, storeKey.ToString(), new UploadOptions(), objectToPut, false);
            await upload.StartUploadAsync(); //ToDo: Place Stream in UploadQueue

            return upload.Completed;
        }

        internal static IBucketService GetBucketService(StoreAccess access)
        {
            if (string.IsNullOrEmpty(access.AccessGrant))
                throw new StoreAccessInvalidException();

            if (_bucketServiceInstances.ContainsKey(access.AccessGrant))
                return _bucketServiceInstances[access.AccessGrant];

            Access storjAccess = new Access(access.AccessGrant);
            var bucketService = new BucketService(storjAccess);
            _bucketServiceInstances.Add(access.AccessGrant, bucketService);

            return bucketService;
        }

        internal static IObjectService GetObjectService(StoreAccess access)
        {
            if (string.IsNullOrEmpty(access.AccessGrant))
                throw new StoreAccessInvalidException();

            if (_objectServiceInstances.ContainsKey(access.AccessGrant))
                return _objectServiceInstances[access.AccessGrant];

            Access storjAccess = new Access(access.AccessGrant);
            var objectService = new ObjectService(storjAccess);
            _objectServiceInstances.Add(access.AccessGrant, objectService);

            return objectService;
        }

        internal static async Task<Bucket> GetBucketAsync(string bucketName, StoreAccess access)
        {
            if (string.IsNullOrEmpty(access.AccessGrant))
                throw new StoreAccessInvalidException();

            if (_BucketInstances.ContainsKey(access.AccessGrant))
                return _BucketInstances[access.AccessGrant];

            var bucketService = GetBucketService(access);
            var bucket = await bucketService.EnsureBucketAsync(bucketName);
            _BucketInstances.Add(access.AccessGrant, bucket);

            return bucket;
        }

        private static byte[] SerializeToJSON(object objectToSerialize)
        {
            var JSONString = Newtonsoft.Json.JsonConvert.SerializeObject(objectToSerialize);
            return Encoding.UTF8.GetBytes(JSONString);
        }

        private static T DeserializeFromJSON<T>(byte[] JSONBytes)
        {
            var JSONString = Encoding.UTF8.GetString(JSONBytes);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(JSONString);
        }
    }
}
