﻿using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Services
{
    public class BookService : IBookService
    {
        private IStoreService _storeService;

        public BookService(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public async Task<Book> LoadAsync(BookReference bookReference)
        {
            var bookShelfInfo = await _storeService.GetObjectInfoAsync(new StoreAccess(bookReference.AccessGrant), StoreKey.Book(bookReference.BookId));

            if (!bookShelfInfo.ObjectExists)
                throw new BookNotExistException();

            return await _storeService.GetObjectFromJsonAsync<Book>(new StoreAccess(bookReference.AccessGrant), StoreKey.Book(bookReference.BookId));
        }

        public async Task<bool> SaveAsync(BookReference bookReference, Book bookToSave)
        {
            return await _storeService.PutObjectAsJsonAsync<Book>(new StoreAccess(bookReference.AccessGrant), StoreKey.Book(bookReference.BookId), bookToSave);
        }
    }
}
