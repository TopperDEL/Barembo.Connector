﻿using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Barembo.StoreServices
{
    public class BookShelfStoreService : IBookShelfStoreService
    {
        readonly IStoreService _storeService;

        public BookShelfStoreService(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public bool AddBookToBookShelf(BookShelf bookShelf, string bookId, string ownerName, StoreAccess storeAccess, AccessRights accessRights, string contributorId)
        {
            return AddBookToBookShelf(bookShelf, bookId, ownerName, storeAccess, accessRights, contributorId, null);
        }

        public bool AddBookToBookShelf(BookShelf bookShelf, string bookId, string ownerName, StoreAccess storeAccess, AccessRights accessRights, string contributorId, BookShareReference bookShareReference)
        {
            if (bookShelf.Content.Where(r => r.BookId == bookId).Count() > 0)
            {
                return false;
            }

            BookReference reference = new BookReference();
            reference.AccessGrant = storeAccess.AccessGrant;
            reference.AccessRights = accessRights;
            reference.BookId = bookId;
            reference.OwnerName = ownerName;
            reference.ContributorId = contributorId;
            reference.BookShareReference = bookShareReference;

            bookShelf.Content.Add(reference);

            return true;
        }

        public async Task<BookShelf> LoadAsync(StoreAccess access)
        {
            var bookShelfInfo = await _storeService.GetObjectInfoAsync(access, StoreKey.BookShelf());

            if (!bookShelfInfo.ObjectExists)
            {
                throw new NoBookShelfExistsException(access.AccessGrant, StoreKey.BookShelf().ToString(), bookShelfInfo.NotExistsErrorMessage);
            }

            return await _storeService.GetObjectFromJsonAsync<BookShelf>(access, StoreKey.BookShelf());
        }

        public async Task<bool> SaveAsync(StoreAccess access, BookShelf bookShelf)
        {
            return await _storeService.PutObjectAsJsonAsync<BookShelf>(access, StoreKey.BookShelf(), bookShelf);
        }
    }
}
