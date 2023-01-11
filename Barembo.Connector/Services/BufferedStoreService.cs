using Barembo.Helper;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using uplink.NET.Interfaces;
using uplink.NET.Services;

namespace Barembo.Services
{
    public class BufferedStoreService : IStoreService
    {
        readonly IStoreBuffer _storeBuffer;
        readonly IStoreService _storeService;
        readonly IUploadQueueService _uploadQueueService;

        readonly string _bucketName;

        public BufferedStoreService(IStoreBuffer storeBuffer, IStoreService storeService, IUploadQueueService uploadQueueService) : this(storeBuffer, storeService, uploadQueueService, "barembo")
        {
        }

        public BufferedStoreService(IStoreBuffer storeBuffer, IStoreService storeService, IUploadQueueService uploadQueueService, string bucketName)
        {
            _storeBuffer = storeBuffer;
            _storeService = storeService;
            _uploadQueueService = uploadQueueService;
            _bucketName = bucketName;
        }

        public async Task<Stream> GetObjectAsStreamAsync(StoreAccess access, StoreKey storeKey)
        {
            var fromBuffer = await _storeBuffer.GetObjectAsStreamFromBufferAsync(access, storeKey).ConfigureAwait(false);

            if (fromBuffer != null)
                return fromBuffer;
            else
                return await _storeService.GetObjectAsStreamAsync(access, storeKey).ConfigureAwait(false);
        }

        public async Task<T> GetObjectFromJsonAsync<T>(StoreAccess access, StoreKey storeKey)
        {
            var fromBuffer = await _storeBuffer.GetObjectFromBufferAsync<T>(access, storeKey).ConfigureAwait(false);

            if (typeof(T).IsClass && fromBuffer != null)
            {
                return fromBuffer;
            }
            else if (!typeof(T).IsClass && fromBuffer != null && !fromBuffer.Equals(default(T)))
            {
                return fromBuffer;
            }
            else
            {
                var result = await _storeService.GetObjectFromJsonAsync<T>(access, storeKey).ConfigureAwait(false);
                await _storeBuffer.PutObjectToBufferAsync(access, storeKey, result).ConfigureAwait(false);
                return result;
            }
        }

        public async Task<T> GetObjectFromJsonAsync<T>(StoreAccess access, StoreKey storeKey, bool ignoreBuffer)
        {
            if(ignoreBuffer)
            {
                var result = await _storeService.GetObjectFromJsonAsync<T>(access, storeKey).ConfigureAwait(false);
                await _storeBuffer.PutObjectToBufferAsync(access, storeKey, result).ConfigureAwait(false);
                return result;
            }
            else
            {
                return await GetObjectFromJsonAsync<T>(access, storeKey);
            }
        }

        public async Task<StoreObjectInfo> GetObjectInfoAsync(StoreAccess access, StoreKey storeKey)
        {
            return await _storeService.GetObjectInfoAsync(access, storeKey).ConfigureAwait(false);
        }

        public async Task<IEnumerable<StoreObject>> ListObjectsAsync(StoreAccess access, StoreKey storeKey)
        {
            return await _storeService.ListObjectsAsync(access, storeKey).ConfigureAwait(false);
        }

        public async Task<IEnumerable<StoreObject>> ListObjectsAsync(StoreAccess access, StoreKey storeKey, bool withMetaData)
        {
            return await _storeService.ListObjectsAsync(access, storeKey, withMetaData).ConfigureAwait(false);
        }

        public async Task<bool> PutObjectAsJsonAsync<T>(StoreAccess access, StoreKey storeKey, T objectToPut)
        {
            try
            {
                var JSONBytes = JSONHelper.SerializeToJSON(objectToPut);

                await _uploadQueueService.AddObjectToUploadQueueAsync(_bucketName, storeKey.ToString(), access.AccessGrant, JSONBytes, null).ConfigureAwait(false);

                await _storeBuffer.PutObjectToBufferAsync<T>(access, storeKey, objectToPut).ConfigureAwait(false);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> PutObjectAsJsonAsync<T>(StoreAccess access, StoreKey storeKey, T objectToPut, StoreMetaData metaData)
        {
            var saved = await _storeService.PutObjectAsJsonAsync<T>(access, storeKey, objectToPut, metaData).ConfigureAwait(false);

            if (saved)
                await _storeBuffer.PutObjectToBufferAsync<T>(access, storeKey, objectToPut).ConfigureAwait(false);

            return saved;
        }

        public async Task<bool> PutObjectFromStreamAsync(StoreAccess access, StoreKey storeKey, Stream objectToPut, string filePath)
        {
            try
            {
                var bytes = ReadFully(objectToPut);
                await _uploadQueueService.AddObjectToUploadQueueAsync(_bucketName, storeKey.ToString(), access.AccessGrant, bytes, filePath).ConfigureAwait(false);

                await _storeBuffer.PutObjectFromStreamToBufferAsync(access, storeKey, objectToPut).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                input.Position = 0; //Rewind stream
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                input.Position = 0; //Rewind stream
                return ms.ToArray();
            }
        }
    }
}
