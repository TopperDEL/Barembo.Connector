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
        public Task<Book> CreateAndSaveBookAsync(string name, string description)
        {
            throw new NotImplementedException();
        }

        public Task<Book> LoadBookAsync(BookReference bookReference)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveBookAsync(Book book)
        {
            throw new NotImplementedException();
        }
    }
}
