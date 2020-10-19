using Barembo.Interfaces;
using Barembo.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Services
{
    internal class StoreBuffer : IStoreBuffer
    {
        private static bool IsInitialized = false;
        private static SQLiteAsyncConnection _dataBase;
        private static async Task InitAsync()
        {
            if (IsInitialized)
                return;

            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BaremboBuffer.db");

            _dataBase = new SQLiteAsyncConnection(databasePath);

            await _dataBase.CreateTableAsync<BufferEntry>();
        }

        public async Task<Stream> GetObjectAsStreamFromBufferAsync(StoreAccess access, StoreKey keyToCheck)
        {
            var bufferedEntry = await _dataBase.GetAsync<BufferEntry>(keyToCheck.ToString());

            return new MemoryStream(bufferedEntry.BufferedContent);
        }

        public async Task<T> GetObjectFromBufferAsync<T>(StoreAccess access, StoreKey keyToCheck)
        {
            var bufferedEntry = await _dataBase.GetAsync<BufferEntry>(keyToCheck.ToString());

            MemoryStream ms = new MemoryStream(bufferedEntry.BufferedContent);
            using (BsonDataReader reader = new BsonDataReader(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }

        public async Task<bool> IsBufferedAsync(StoreAccess access, StoreKey keyToCheck)
        {
            await InitAsync();

            try
            {
                var result = await _dataBase.GetAsync<BufferEntry>(keyToCheck.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task PutObjectFromStreamToBufferAsync(StoreAccess access, StoreKey keyToCheck, Stream objectToAdd)
        {
            await InitAsync();

            try
            {
                BufferEntry entry = new BufferEntry();
                entry.Id = keyToCheck.ToString();
                using (var streamReader = new MemoryStream())
                {
                    objectToAdd.CopyTo(streamReader);
                    entry.BufferedContent = streamReader.ToArray();
                }
                await _dataBase.InsertOrReplaceAsync(entry);
            }
            catch
            {
            }
        }

        public async Task PutObjectToBufferAsync<T>(StoreAccess access, StoreKey keyToCheck, T objectToAdd)
        {
            await InitAsync();

            try
            {
                MemoryStream ms = new MemoryStream();
                using (BsonDataWriter writer = new BsonDataWriter(ms))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, objectToAdd);
                }

                BufferEntry entry = new BufferEntry();
                entry.Id = keyToCheck.ToString();
                entry.BufferedContent = ms.ToArray();
                await _dataBase.InsertOrReplaceAsync(entry);
            }
            catch
            {
            }
        }

        internal async Task RemoveDatabaseAsync()
        {
            await InitAsync();
            await _dataBase.DeleteAllAsync<BufferEntry>();
        }
    }
}
