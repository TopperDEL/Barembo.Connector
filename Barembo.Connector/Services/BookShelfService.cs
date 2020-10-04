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
        public Task<BookShelf> LoadAsync()
        {
            throw new NoBookShelfExistsException();
        }

        public Task<bool> SaveAsync(BookShelf bookShelf)
        {
            throw new NotImplementedException();
        }
    }
}
