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
    public class StoreBuffer : IStoreBuffer
    {
        private static bool _isInitialized;
        private static SQLiteAsyncConnection _dataBase;
        public static string BaseFolder;

        private static async Task InitAsync()
        {
            if (_isInitialized)
                return;

            if (string.IsNullOrEmpty(BaseFolder))
            {
                BaseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Barembo");
            }
            Directory.CreateDirectory(BaseFolder);

#if DEBUG
            var databasePath = Path.Combine(BaseFolder, "BaremboBuffer_Debug.db");
#else
            var databasePath = Path.Combine(BaseFolder, "BaremboBuffer.db");
#endif

            _dataBase = new SQLiteAsyncConnection(databasePath);

            await _dataBase.CreateTableAsync<BufferEntry>();

            _isInitialized = true;
        }

        public async Task<Stream> GetObjectAsStreamFromBufferAsync(StoreAccess access, StoreKey keyToCheck)
        {
            await InitAsync().ConfigureAwait(false);

            var bufferedEntry = await _dataBase.GetAsync<BufferEntry>(keyToCheck.ToString());

            return new MemoryStream(bufferedEntry.BufferedContent);
        }

        public async Task<T> GetObjectFromBufferAsync<T>(StoreAccess access, StoreKey keyToCheck)
        {
            await InitAsync().ConfigureAwait(false);

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
            await InitAsync().ConfigureAwait(false);

            try
            {
                await _dataBase.GetAsync<BufferEntry>(keyToCheck.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task PutObjectFromStreamToBufferAsync(StoreAccess access, StoreKey keyToCheck, Stream objectToAdd)
        {
            await InitAsync().ConfigureAwait(false);

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
                //If the object could not be buffered, that's ok
            }
        }

        public async Task PutObjectToBufferAsync<T>(StoreAccess access, StoreKey keyToCheck, T objectToAdd)
        {
            await InitAsync().ConfigureAwait(false);

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
                //If the object could not be buffered, that's ok
            }
        }

        internal async Task RemoveDatabaseAsync()
        {
            await InitAsync().ConfigureAwait(false);
            await _dataBase.DeleteAllAsync<BufferEntry>();
        }
    }
}
