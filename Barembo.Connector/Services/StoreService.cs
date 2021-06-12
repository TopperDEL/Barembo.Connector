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
using System.Threading;

namespace Barembo.Services
{
    public class StoreService : IStoreService
    {
        private static Dictionary<string, IBucketService> _bucketServiceInstances = new Dictionary<string, IBucketService>();
        private static Dictionary<string, IObjectService> _objectServiceInstances = new Dictionary<string, IObjectService>();
        private static Dictionary<string, Bucket> _bucketInstances = new Dictionary<string, Bucket>();
        private static Dictionary<string, Access> _accessInstances = new Dictionary<string, Access>();

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
            var objectService = await GetObjectServiceAsync(access).ConfigureAwait(false);
            var bucket = await GetBucketAsync(_bucketName, access).ConfigureAwait(false);

            var download = await objectService.DownloadObjectAsync(bucket, storeKey.ToString(), new DownloadOptions(), false).ConfigureAwait(false);
            await download.StartDownloadAsync().ConfigureAwait(false);

            if (download.Completed)
            {
                return JSONHelper.DeserializeFromJSON<T>(download.DownloadedBytes);
            }
            else
                return default;
        }

        public async Task<StoreObjectInfo> GetObjectInfoAsync(StoreAccess access, StoreKey storeKey)
        {
            var objectService = await GetObjectServiceAsync(access).ConfigureAwait(false);
            var bucket = await GetBucketAsync(_bucketName, access).ConfigureAwait(false);

            try
            {
                var objectInfo = await objectService.GetObjectAsync(bucket, storeKey.ToString()).ConfigureAwait(false);
                return new StoreObjectInfo { ObjectExists = true, Size = objectInfo.SystemMetadata.ContentLength };
            }
            catch (Exception ex)
            {
                return new StoreObjectInfo { ObjectExists = false, NotExistsErrorMessage = ex.Message };
            }
        }

        public async Task<IEnumerable<StoreObject>> ListObjectsAsync(StoreAccess access, StoreKey storeKey)
        {
            return await ListObjectsAsync(access, storeKey, false).ConfigureAwait(false);
        }

        public async Task<IEnumerable<StoreObject>> ListObjectsAsync(StoreAccess access, StoreKey storeKey, bool withMetaData)
        {
            var objectService = await GetObjectServiceAsync(access).ConfigureAwait(false);
            var bucket = await GetBucketAsync(_bucketName, access).ConfigureAwait(false);

            var listObjectsOption = new ListObjectsOptions();
            listObjectsOption.Prefix = storeKey.ToString();
            listObjectsOption.Recursive = true;
            listObjectsOption.Custom = withMetaData;

            var objects = await objectService.ListObjectsAsync(bucket, listObjectsOption).ConfigureAwait(false);

            return objects.Items.Select(i =>
            {
                if (i.CustomMetadata.Entries.Count == 0)
                {
                    return new StoreObject(i.Key, StoreKeyHelper.GetStoreObjectId(i.Key), new StoreMetaData());
                }
                else
                {
                    return new StoreObject(i.Key, StoreKeyHelper.GetStoreObjectId(i.Key), new StoreMetaData(i.CustomMetadata.Entries[0].Key, i.CustomMetadata.Entries[0].Value));
                }
            });
        }

        public async Task<bool> PutObjectAsJsonAsync<T>(StoreAccess access, StoreKey storeKey, T objectToPut)
        {
            var objectService = await GetObjectServiceAsync(access).ConfigureAwait(false);
            var bucket = await GetBucketAsync(_bucketName, access).ConfigureAwait(false);

            var JSONBytes = JSONHelper.SerializeToJSON(objectToPut);

            var upload = await objectService.UploadObjectAsync(bucket, storeKey.ToString(), new UploadOptions(), JSONBytes, false).ConfigureAwait(false);
            await upload.StartUploadAsync();

            return upload.Completed;
        }

        public async Task<bool> PutObjectAsJsonAsync<T>(StoreAccess access, StoreKey storeKey, T objectToPut, StoreMetaData metaData)
        {
            var objectService = await GetObjectServiceAsync(access).ConfigureAwait(false);
            var bucket = await GetBucketAsync(_bucketName, access).ConfigureAwait(false);

            var JSONBytes = JSONHelper.SerializeToJSON(objectToPut);

            CustomMetadata customMetaData = new CustomMetadata();
            customMetaData.Entries.Add(new CustomMetadataEntry { Key = metaData.Key, Value = metaData.Value });

            var upload = await objectService.UploadObjectAsync(bucket, storeKey.ToString(), new UploadOptions(), JSONBytes, customMetaData, false).ConfigureAwait(false);
            await upload.StartUploadAsync();

            return upload.Completed;
        }

        public async Task<bool> PutObjectFromStreamAsync(StoreAccess access, StoreKey storeKey, Stream objectToPut, string filePath)
        {
            var objectService = await GetObjectServiceAsync(access).ConfigureAwait(false);
            var bucket = await GetBucketAsync(_bucketName, access).ConfigureAwait(false);

            var upload = await objectService.UploadObjectAsync(bucket, storeKey.ToString(), new UploadOptions(), objectToPut, false).ConfigureAwait(false);
            await upload.StartUploadAsync();

            return upload.Completed;
        }

        private static readonly SemaphoreSlim _getBucketServiceAsyncLock = new SemaphoreSlim(1, 1);

        internal static async Task<IBucketService> GetBucketServiceAsync(StoreAccess access)
        {
            await _getBucketServiceAsyncLock.WaitAsync();

            try
            {
                if (string.IsNullOrEmpty(access.AccessGrant))
                    throw new StoreAccessInvalidException();

                if (_bucketServiceInstances.ContainsKey(access.AccessGrant))
                    return _bucketServiceInstances[access.AccessGrant];

                Access storjAccess = await GetAccessFromAccessGrantAsync(access.AccessGrant).ConfigureAwait(false);
                var bucketService = new BucketService(storjAccess);
                _bucketServiceInstances.Add(access.AccessGrant, bucketService);

                return bucketService;
            }
            finally
            {
                _getBucketServiceAsyncLock.Release();
            }
        }

        private static readonly SemaphoreSlim _getObjectServiceAsyncLock = new SemaphoreSlim(1, 1);

        internal static async Task<IObjectService> GetObjectServiceAsync(StoreAccess access)
        {
            await _getObjectServiceAsyncLock.WaitAsync();

            try
            {
                if (string.IsNullOrEmpty(access.AccessGrant))
                    throw new StoreAccessInvalidException();

                if (_objectServiceInstances.ContainsKey(access.AccessGrant))
                    return _objectServiceInstances[access.AccessGrant];

                Access storjAccess = await GetAccessFromAccessGrantAsync(access.AccessGrant).ConfigureAwait(false);
                var objectService = new ObjectService(storjAccess);
                _objectServiceInstances.Add(access.AccessGrant, objectService);

                return objectService;
            }
            finally
            {
                _getObjectServiceAsyncLock.Release();
            }
        }

        private static readonly SemaphoreSlim _getAccessAsyncLock = new SemaphoreSlim(1, 1);

        private static async Task<Access> GetAccessFromAccessGrantAsync(string accessGrant)
        {
            await _getAccessAsyncLock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (!_accessInstances.ContainsKey(accessGrant))
                {
                    Access storjAccess = new Access(accessGrant);
                    _accessInstances.Add(accessGrant, storjAccess);
                }
                return _accessInstances[accessGrant];
            }
            finally
            {
                _getAccessAsyncLock.Release();
            }
        }

        private static readonly SemaphoreSlim _getBucketAsyncLock = new SemaphoreSlim(1, 1);

        internal static async Task<Bucket> GetBucketAsync(string bucketName, StoreAccess access)
        {
            await _getBucketAsyncLock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (string.IsNullOrEmpty(access.AccessGrant))
                    throw new StoreAccessInvalidException();

                if (_bucketInstances.ContainsKey(access.AccessGrant))
                    return _bucketInstances[access.AccessGrant];

                var bucketService = await GetBucketServiceAsync(access).ConfigureAwait(false);
                try
                {
                    var bucket = await bucketService.EnsureBucketAsync(bucketName);
                    _bucketInstances.Add(access.AccessGrant, bucket);

                    return bucket;
                }
                catch
                {
                    try
                    {
                        var bucket = await bucketService.GetBucketAsync(bucketName).ConfigureAwait(false);
                        _bucketInstances.Add(access.AccessGrant, bucket);

                        return bucket;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            finally
            {
                _getBucketAsyncLock.Release();
            }
        }
    }
}
