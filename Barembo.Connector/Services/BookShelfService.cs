using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Services
{
    public class BookShelfService : IBookShelfService
    {
        IBookShelfStoreService _bookShelfStoreService;

        public BookShelfService(IBookShelfStoreService bookShelfStoreService)
        {
            _bookShelfStoreService = bookShelfStoreService;
        }

        public async Task<bool> AddOwnBookToBookShelfAndSaveAsync(StoreAccess access, Book book)
        {
            var bookShelf = await _bookShelfStoreService.LoadAsync(access);

            var success = _bookShelfStoreService.AddBookToBookShelf(bookShelf, book.Id, bookShelf.OwnerName, access, AccessRights.Full);

            if (success)
            {
                return await _bookShelfStoreService.SaveAsync(access, bookShelf);
            }
            else
                return false;
        }

        public Task<bool> AddSharedBookToBookShelfAndSaveAsync(StoreAccess access, BookShareInfo shareInfo)
        {
            throw new NotImplementedException();
        }

        public async Task<BookShelf> CreateAndSaveBookShelfAsync(StoreAccess storeAccess, string ownerName)
        {
            BookShelf bookShelf = new BookShelf();
            bookShelf.OwnerName = ownerName;

            var success = await _bookShelfStoreService.SaveAsync(storeAccess, bookShelf);

            if (success)
                return bookShelf;
            else
                throw new BookShelfCouldNotBeSavedException();
        }

        public Task<BookShelf> LoadBookShelfAsync(StoreAccess access)
        {
            throw new NotImplementedException();
        }
    }
}
