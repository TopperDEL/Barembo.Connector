﻿using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    /// <summary>
    /// A BookShelfStoreService is responsible for loading and saving a BookShelf and adding content to it.
    /// </summary>
    public interface IBookShelfStoreService
    {
        /// <summary>
        /// Loads the BookShelf of the user. If none exists, a NoBookShelfExistsException gets thrown.
        /// </summary>
        /// <param name="access">The access to use</param>
        /// <returns>The BookShelf or a NoBookShelfExistsException</returns>
        Task<BookShelf> LoadAsync(StoreAccess access);

        /// <summary>
        /// Saves the BookShelf of the user.
        /// </summary>
        /// <param name="access">The access to use</param>
        /// <param name="bookShelf">The BookShelf to save</param>
        /// <returns>True, if the BookShelf could be saved - false if not</returns>
        Task<bool> SaveAsync(StoreAccess access, BookShelf bookShelf);

        /// <summary>
        /// Adds a Book to a BookShelf
        /// </summary>
        /// <param name="bookShelf">The BookShelf to add the Book to</param>
        /// <param name="bookId">The Id of the Book to add</param>
        /// <param name="ownerName">The name of the owner of that Book</param>
        /// <param name="storeAccess">The StoreAccess for this Book</param>
        /// <param name="accessRights">The AccessRights for this Book</param>
        /// <param name="contributorId">The Id of the Contributor for this Book</param>
        /// <returns>True, if the BookShelf could be saved - false if not</returns>
        bool AddBookToBookShelf(BookShelf bookShelf, string bookId, string ownerName, StoreAccess storeAccess, AccessRights accessRights, string contributorId);

        /// <summary>
        /// Adds a Book to a BookShelf for a shared Book
        /// </summary>
        /// <param name="bookShelf">The BookShelf to add the Book to</param>
        /// <param name="bookId">The Id of the Book to add</param>
        /// <param name="ownerName">The name of the owner of that Book</param>
        /// <param name="storeAccess">The StoreAccess for this Book</param>
        /// <param name="accessRights">The AccessRights for this Book</param>
        /// <param name="contributorId">The Id of the Contributor for this Book</param>
        /// <param name="bookShareReference">The BookShareReference if the Book is a foreign Book</param>
        /// <returns>True, if the BookShelf could be saved - false if not</returns>
        bool AddBookToBookShelf(BookShelf bookShelf, string bookId, string ownerName, StoreAccess storeAccess, AccessRights accessRights, string contributorId, BookShareReference bookShareReference);
    }
}
