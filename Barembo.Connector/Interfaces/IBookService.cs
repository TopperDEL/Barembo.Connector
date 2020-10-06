using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    /// <summary>
    /// A BookService is responsible for loading and saving a Book.
    /// </summary>
    public interface IBookService
    {
        /// <summary>
        /// Loads a Book from a given BookReference.
        /// </summary>
        /// <param name="bookReference">The BookReference to a Book</param>
        /// <returns>The Book if it exists, otherwise throws a BookNotExistsException</returns>
        Task<Book> LoadAsync(BookReference bookReference);

        /// <summary>
        /// Saves a book using a given BookReference.
        /// </summary>
        /// <param name="bookReference">The BookReference to the Book</param>
        /// <param name="bookToSave">The Book to save</param>
        /// <returns>true, if the Book could be saved</returns>
        Task<bool> SaveAsync(BookReference bookReference, Book bookToSave);
    }
}
