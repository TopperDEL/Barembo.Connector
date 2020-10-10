using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Barembo.Services
{
    public class BookShelfStoreService : IBookShelfStoreService
    {
        private IStoreService _storeService;

        public BookShelfStoreService(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public bool AddNewBook(BookShelf bookShelf, Book book, string ownerName, StoreAccess storeAccess, AccessRights accessRights)
        {
            BookReference reference = new BookReference();
            reference.AccessGrant = storeAccess.AccessGrant;
            reference.AccessRights = accessRights;
            reference.BookId = book.Id;
            reference.OwnerName = ownerName;

            if (bookShelf.Content.Where(r => r.BookId == book.Id).Count() > 0)
                return false;

            bookShelf.Content.Add(reference);

            return true;
        }

        public async Task<BookShelf> LoadAsync(StoreAccess access)
        {
            var bookShelfInfo = await _storeService.GetObjectInfoAsync(access, StoreKey.BookShelf());

            if (!bookShelfInfo.ObjectExists)
                throw new NoBookShelfExistsException();

            return await _storeService.GetObjectFromJsonAsync<BookShelf>(access, StoreKey.BookShelf());
        }

        public async Task<bool> SaveAsync(StoreAccess access, BookShelf bookShelf)
        {
            return await _storeService.PutObjectAsJsonAsync<BookShelf>(access, StoreKey.BookShelf(), bookShelf);
        }
    }
}
