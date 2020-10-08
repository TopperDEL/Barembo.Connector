using Barembo.Helper;
using Barembo.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Connector.Test.Helper
{
    [TestClass]
    public class StoreKeyHelperTest
    {
        [TestMethod]
        public void Convert_BookShelf()
        {
            StoreKey key = StoreKey.BookShelf();

            var result = StoreKeyHelper.Convert(key);

            Assert.AreEqual("BookShelf.json", result);
        }

        [TestMethod]
        public void Convert_Book()
        {
            StoreKey key = StoreKey.Book("myBookId");

            var result = StoreKeyHelper.Convert(key);

            Assert.AreEqual("myBookId/Book.json", result);
        }

        [TestMethod]
        public void Convert_Entry()
        {
            StoreKey key = StoreKey.Entry("myBookId", "myEntryId", "myContributorId");

            var result = StoreKeyHelper.Convert(key);

            Assert.AreEqual("myBookId/Entries/myContributorId/myEntryId.json", result);
        }

        [TestMethod]
        public void Convert_Entries()
        {
            StoreKey key = StoreKey.Entries("myBookId");

            var result = StoreKeyHelper.Convert(key);

            Assert.AreEqual("myBookId/Entries/", result);
        }

        [TestMethod]
        public void Convert_Attachment()
        {
            StoreKey key = StoreKey.Attachment("myBookId", "myEntryId", "myAttachmentId");

            var result = StoreKeyHelper.Convert(key);

            Assert.AreEqual("myBookId/myEntryId/myAttachmentId", result);
        }
    }
}
