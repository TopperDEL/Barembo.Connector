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
        /// Gets the ObjectInfo of a StoreKey
        /// </summary>
        /// <returns>The ObjectInfo</returns>
        Task<StoreObjectInfo> GetObjectInfoAsync(StoreKey storeKey);

        /// <summary>
        /// Returns the object of the given StoreKey by converting from Json. 
        /// </summary>
        /// <typeparam name="T">The type to convert the json to</typeparam>
        /// <param name="storeKey">The StoreKey of that object</param>
        /// <returns>The object if the conversion threw no error and the object exists and is accessable</returns>
        Task<T> GetObjectFromJsonAsync<T>(StoreKey storeKey);
    }
}
