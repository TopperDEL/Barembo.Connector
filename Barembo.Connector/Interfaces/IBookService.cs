using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    /// <summary>
    /// A BookService is responsible for creating, saving and loading a Book
    /// </summary>
    public interface IBookService
    {
        /// <summary>
        /// Creates a new Book. The book still needs to be added to a BookShelf and saved.
        /// </summary>
        /// <param name="name">The name of the Book</param>
        /// <param name="description">The description of the Book</param>
        /// <returns>The new Book</returns>
        Book CreateBook(string name, string description);

        /// <summary>
        /// Saves a Book via the given BookReference
        /// </summary>
        /// <param name="bookReference">The BookReference for the Book</param>
        /// <param name="book">The Book to save</param>
        /// <returns>true, if the Book could be saved; false if not</returns>
        Task<bool> SaveBookAsync(BookReference bookReference, Book book);

        /// <summary>
        /// Loads a Book from the given BookReference
        /// </summary>
        /// <param name="bookReference">The BookReference for the Book</param>
        /// <returns>The Book if it could be loaded; raises BookNotExistsException if not</returns>
        Task<Book> LoadBookAsync(BookReference bookReference);
    }
}
