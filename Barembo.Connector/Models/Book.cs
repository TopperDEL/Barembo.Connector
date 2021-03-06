﻿using Barembo.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    /// <summary>
    /// A book is the main part of Barembo. It is the holder of Entries and
    /// has a Name and a Description. It may have CoverImages to visualize it
    /// in the app.
    /// </summary>
    public class Book
    {
        /// <summary>
        /// The ID of the book.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The Name of the book.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description for the book.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The version of the book - in case there needs to happen some migration for future adjustments.
        /// </summary>
        public string Version { get; set; } = VersionHelper.CURRENT_VERSION;

        /// <summary>
        /// The base64-encoded CoverImage for this book
        /// </summary>
        public string CoverImageBase64 { get; set; }

        internal Book() { }
    }
}
