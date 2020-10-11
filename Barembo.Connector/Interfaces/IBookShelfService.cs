using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    public interface IBookShelfService
    {
        Task<BookShelf> CreateBookShelfAsync(string ownerName);
        Task<BookShelf> LoadBookShelfAsync(StoreAccess access);
        Task<bool> AddOwnBookToBookShelfAsync(Book book);
        Task<bool> AddSharedBookToBookShelfAsync(BookShareInfo shareInfo);
    }
}
