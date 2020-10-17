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
        IBookStoreService _bookStoreService;
        IBookShareStoreService _bookShareStoreService;

        public BookService(IBookStoreService bookStoreService, IBookShareStoreService bookShareStoreService)
        {
            _bookStoreService = bookStoreService;
            _bookShareStoreService = bookShareStoreService;
        }

        public async Task<Book> CreateBookAsync(string name, string description)
        {
            Book newBook = new Book();
            newBook.Name = name;
            newBook.Description = description;

            return newBook;
        }

        public async Task<Book> LoadBookAsync(BookReference bookReference)
        {
            if(bookReference.BookShareReference != null)
            {
                var bookShare = await _bookShareStoreService.LoadBookShareAsync(bookReference.BookShareReference);
                bookReference.AccessGrant = bookShare.Access.AccessGrant;
                bookReference.AccessRights = bookShare.AccessRights;
            }

            return await _bookStoreService.LoadAsync(bookReference);
        }

        public async Task<bool> SaveBookAsync(BookReference bookReference, Book book)
        {
            return await _bookStoreService.SaveAsync(bookReference, book);
        }
    }
}
