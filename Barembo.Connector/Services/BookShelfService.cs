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
        private IStoreService _storeService;

        public BookShelfService(IStoreService storeService)
        {
            _storeService = storeService;
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
