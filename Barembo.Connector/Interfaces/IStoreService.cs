using Barembo.Models;
using System;
using System.Collections.Generic;
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
        /// Puts an object as Json to the given StoreKey in the store
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="access">The access to use</param>
        /// <param name="storeKey">The StoreKey of that object</param>
        /// <param name="objectToPut">The object itself</param>
        /// <returns>True, if the put was successfull - false if not</returns>
        Task<bool> PutObjectAsJsonAsync<T>(StoreAccess access, StoreKey storeKey, T objectToPut);

        /// <summary>
        /// Lists all available objects for a particular StoreKey
        /// </summary>
        /// <param name="access">The access to use</param>
        /// <param name="storeKey">The StoreKey of the objects</param>
        /// <returns>An enumerable of StoreObjects</returns>
        Task<IEnumerable<StoreObject>> ListObjectsAsync(StoreAccess access, StoreKey storeKey);
    }
}
