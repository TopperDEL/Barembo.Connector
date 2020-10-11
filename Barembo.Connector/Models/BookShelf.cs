using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    /// <summary>
    /// A bookshelf is a collection of BookReferences to books the user has acces to.
    /// </summary>
    public class BookShelf
    {
        /// <summary>
        /// The name of the owner of this BookShelf
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// The content of this bookshelf.
        /// </summary>
        public List<BookReference> Content { get; set; } = new List<BookReference>();
    }
}
