using Barembo.Exceptions;
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

        public BookService(IBookStoreService bookStoreService)
        {
            _bookStoreService = bookStoreService;
        }

        public Book CreateBook(string name, string description)
        {
            Book newBook = new Book();
            newBook.Name = name;
            newBook.Description = description;

            return newBook;
        }

        public async Task<Book> LoadBookAsync(BookReference bookReference)
        {
            return await _bookStoreService.LoadAsync(bookReference);
        }

        public async Task<bool> SaveBookAsync(BookReference bookReference, Book book)
        {
            if (!bookReference.AccessRights.CanEditBook)
                throw new ActionNotAllowedException();

            return await _bookStoreService.SaveAsync(bookReference, book);
        }
    }
}
