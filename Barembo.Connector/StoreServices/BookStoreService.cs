using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.StoreServices
{
    internal class BookStoreService : IBookStoreService
    {
        private IStoreService _storeService;

        public BookStoreService(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public async Task<Book> LoadAsync(BookReference bookReference)
        {
            var bookInfo = await _storeService.GetObjectInfoAsync(new StoreAccess(bookReference.AccessGrant), StoreKey.Book(bookReference.BookId));

            if (!bookInfo.ObjectExists)
                throw new BookNotExistsException();

            return await _storeService.GetObjectFromJsonAsync<Book>(new StoreAccess(bookReference.AccessGrant), StoreKey.Book(bookReference.BookId));
        }

        public async Task<bool> SaveAsync(BookReference bookReference, Book bookToSave)
        {
            return await _storeService.PutObjectAsJsonAsync<Book>(new StoreAccess(bookReference.AccessGrant), StoreKey.Book(bookReference.BookId), bookToSave);
        }
    }
}
