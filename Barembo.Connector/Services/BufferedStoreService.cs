using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Services
{
    public class BufferedStoreService : IStoreService
    {
        readonly IStoreBuffer _storeBuffer;
        readonly IStoreService _storeService;

        public BufferedStoreService(IStoreBuffer storeBuffer, IStoreService storeService)
        {
            _storeBuffer = storeBuffer;
            _storeService = storeService;
        }

        public async Task<Stream> GetObjectAsStreamAsync(StoreAccess access, StoreKey storeKey)
        {
            var fromBuffer = await _storeBuffer.GetObjectAsStreamFromBufferAsync(access, storeKey);

            if (fromBuffer != null)
                return fromBuffer;
            else
                return await _storeService.GetObjectAsStreamAsync(access, storeKey);
        }

        public async Task<T> GetObjectFromJsonAsync<T>(StoreAccess access, StoreKey storeKey)
        {
            var fromBuffer = await _storeBuffer.GetObjectFromBufferAsync<T>(access, storeKey);

            if (fromBuffer != null)
                return fromBuffer;
            else
            {
                var result = await _storeService.GetObjectFromJsonAsync<T>(access, storeKey);
                await _storeBuffer.PutObjectToBufferAsync(access, storeKey, result);
                return result;
            }
        }

        public async Task<StoreObjectInfo> GetObjectInfoAsync(StoreAccess access, StoreKey storeKey)
        {
            return await _storeService.GetObjectInfoAsync(access, storeKey);
        }

        public async Task<IEnumerable<StoreObject>> ListObjectsAsync(StoreAccess access, StoreKey storeKey)
        {
            return await _storeService.ListObjectsAsync(access, storeKey);
        }

        public async Task<bool> PutObjectAsJsonAsync<T>(StoreAccess access, StoreKey storeKey, T objectToPut)
        {
            var saved = await _storeService.PutObjectAsJsonAsync<T>(access, storeKey, objectToPut);

            if (saved)
                await _storeBuffer.PutObjectToBufferAsync<T>(access, storeKey, objectToPut);

            return saved;
        }

        public async Task<bool> PutObjectFromStreamAsync(StoreAccess access, StoreKey storeKey, Stream objectToPut)
        {
            var saved = await _storeService.PutObjectFromStreamAsync(access, storeKey, objectToPut);

            if (saved)
                await _storeBuffer.PutObjectFromStreamToBufferAsync(access, storeKey, objectToPut);

            return saved;
        }
    }
}
