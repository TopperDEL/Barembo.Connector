using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    /// <summary>
    /// A StoreService connects to the underlying store.
    /// </summary>
    public interface IStoreService
    {
        /// <summary>
        /// Gets the StoreObjectInfo of a StoreKey
        /// </summary>
        /// <param name="access">The access to use</param>
        /// <param name="storeKey">The StoreKey of the object</param>
        /// <returns>The StoreObjectInfo</returns>
        Task<StoreObjectInfo> GetObjectInfoAsync(StoreAccess access, StoreKey storeKey);

        /// <summary>
        /// Returns the object of the given StoreKey by converting from Json. 
        /// </summary>
        /// <typeparam name="T">The type to convert the json to</typeparam>
        /// <param name="access">The access to use</param>
        /// <param name="storeKey">The StoreKey of that object</param>
        /// <returns>The object if the conversion threw no error and the object exists and is accessable</returns>
        Task<T> GetObjectFromJsonAsync<T>(StoreAccess access, StoreKey storeKey);

        /// <summary>
        /// Returns the object of the given StoreKey by converting from Json. 
        /// </summary>
        /// <typeparam name="T">The type to convert the json to</typeparam>
        /// <param name="access">The access to use</param>
        /// <param name="storeKey">The StoreKey of that object</param>
        /// <param name="ignoreBuffer">Ignore any buffering</param>
        /// <returns>The object if the conversion threw no error and the object exists and is accessable</returns>
        Task<T> GetObjectFromJsonAsync<T>(StoreAccess access, StoreKey storeKey, bool ignoreBuffer);

        /// <summary>
        /// Returns the stream to an object of the given StoreKey. 
        /// </summary>
        /// <param name="access">The access to use</param>
        /// <param name="storeKey">The StoreKey of that object</param>
        /// <returns>The stream to the object if the object exists and is accessable</returns>
        Task<Stream> GetObjectAsStreamAsync(StoreAccess access, StoreKey storeKey);

        /// <summary>
        /// Puts an object from a stream to the given StoreKey in the store
        /// </summary>
        /// <param name="access">The access to use</param>
        /// <param name="storeKey">The StoreKey of that object</param>
        /// <param name="objectToPut">The stream of the object</param>
        /// <param name="filePath">The filePath of the object</param>
        /// <returns>True, if the put was successfull - false if not</returns>
        Task<bool> PutObjectFromStreamAsync(StoreAccess access, StoreKey storeKey, Stream objectToPut, string filePath);

        /// <summary>
        /// Puts an object as Json to the given StoreKey in the store
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="access">The access to use</param>
        /// <param name="storeKey">The StoreKey of that object</param>
        /// <param name="objectToPut">The object itself</param>
        /// <returns>True, if the put was successfull - false if not</returns>
        Task<bool> PutObjectAsJsonAsync<T>(StoreAccess access, StoreKey storeKey, T objectToPut);

        /// <summary>
        /// Puts an object as Json to the given StoreKey in the store with an attached metaData
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="access">The access to use</param>
        /// <param name="storeKey">The StoreKey of that object</param>
        /// <param name="objectToPut">The object itself</param>
        /// <param name="metaData">The StoreMetaData to attach</param>
        /// <returns>True, if the put was successfull - false if not</returns>
        Task<bool> PutObjectAsJsonAsync<T>(StoreAccess access, StoreKey storeKey, T objectToPut, StoreMetaData metaData);

        /// <summary>
        /// Lists all available objects for a particular StoreKey
        /// </summary>
        /// <param name="access">The access to use</param>
        /// <param name="storeKey">The StoreKey of the objects</param>
        /// <returns>An enumerable of StoreObjects</returns>
        Task<IEnumerable<StoreObject>> ListObjectsAsync(StoreAccess access, StoreKey storeKey);

        /// <summary>
        /// Lists all available objects for a particular StoreKey with attached StoreMetaData
        /// </summary>
        /// <param name="access">The access to use</param>
        /// <param name="storeKey">The StoreKey of the objects</param>
        /// <param name="withMetaData">True if the metadata should be read, too - false if not</param>
        /// <returns>An enumerable of StoreObjects</returns>
        Task<IEnumerable<StoreObject>> ListObjectsAsync(StoreAccess access, StoreKey storeKey, bool withMetaData);
    }
}
