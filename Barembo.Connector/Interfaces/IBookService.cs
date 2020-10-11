using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    public interface IBookService
    {
        Task<Book> CreateAndSaveBookAsync(string name, string description);
        Task<bool> SaveBookAsync(Book book);
        Task<Book> LoadBookAsync(BookReference bookReference);
    }
}
