﻿using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    /// <summary>
    /// A StoreBuffer bufferes get- and put-operations to the Store.
    /// </summary>
    public interface IStoreBuffer
    {
        /// <summary>
        /// Checks if a StoreKey is available in the buffer
        /// </summary>
        /// <param name="access">The access to use</param>
        /// <param name="keyToCheck">The StoreKey to check</param>
        /// <returns>True, if the object is in the buffer, false if not</returns>
        Task<bool> IsBufferedAsync(StoreAccess access, StoreKey keyToCheck);

        /// <summary>
        /// Gets an object from the buffer
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="access">The access to use</param>
        /// <param name="keyToCheck">The StoreKey to retrieve</param>
        /// <returns>The object from the buffer</returns>
        Task<T> GetObjectFromBufferAsync<T>(StoreAccess access, StoreKey keyToCheck);

        /// <summary>
        /// Puts an object to the buffer
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="access">The access to use</param>
        /// <param name="keyToCheck">The StoreKey to set</param>
        /// <param name="objectToAdd">The object to add</param>
        /// <returns>The task</returns>
        Task PutObjectToBufferAsync<T>(StoreAccess access, StoreKey keyToCheck, T objectToAdd);

        /// <summary>
        /// Gets an object as Stream from the buffer
        /// </summary>
        /// <param name="access">The access to use</param>
        /// <param name="keyToCheck">The StoreKey to set</param>
        /// <returns>The Stream</returns>
        Task<Stream> GetObjectAsStreamFromBufferAsync(StoreAccess access, StoreKey keyToCheck);

        /// <summary>
        /// Puts an object from Stream to the buffer
        /// </summary>
        /// <param name="access">The access to use</param>
        /// <param name="keyToCheck">The StoreKey to retrieve</param>
        /// <param name="objectToAdd">The Object-Stream to add</param>
        /// <returns>The task</returns>
        Task PutObjectFromStreamToBufferAsync(StoreAccess access, StoreKey keyToCheck, Stream objectToAdd);
    }
}
