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

        public async Task<BookShelf> LoadAsync()
        {
            var bookShelfInfo = await _storeService.GetObjectInfoAsync(new StoreKey(StoreKeyTypes.BookShelf));

            throw new NoBookShelfExistsException();
        }

        public Task<bool> SaveAsync(BookShelf bookShelf)
        {
            throw new NotImplementedException();
        }
    }
}
