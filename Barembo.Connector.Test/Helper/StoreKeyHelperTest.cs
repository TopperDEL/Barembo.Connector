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
        public void GetId_Book()
        {
            StoreKey key = StoreKey.Book("myBookId");

            var result = StoreKeyHelper.GetStoreObjectId(key);

            Assert.AreEqual("Book", result);
        }

        [TestMethod]
        public void Convert_Entry()
        {
            StoreKey key = StoreKey.Entry("myBookId", "myEntryId", "myContributorId");

            var result = StoreKeyHelper.Convert(key);

            Assert.AreEqual("myBookId/Entries/myContributorId/myEntryId.json", result);
        }

        [TestMethod]
        public void GetId_Entry()
        {
            StoreKey key = StoreKey.Entry("myBookId", "myEntryId", "myContributorId");

            var result = StoreKeyHelper.GetStoreObjectId(key);

            Assert.AreEqual("myEntryId", result);
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

        [TestMethod]
        public void GetId_Attachment()
        {
            StoreKey key = StoreKey.Attachment("myBookId", "myEntryId", "myAttachmentId");

            var result = StoreKeyHelper.GetStoreObjectId(key);

            Assert.AreEqual("myAttachmentId", result);
        }

        [TestMethod]
        public void Convert_Contributor()
        {
            StoreKey key = StoreKey.Contributor("myBookId", "myContributorId");

            var result = StoreKeyHelper.Convert(key);

            Assert.AreEqual("myBookId/Contributors/myContributorId.json", result);
        }

        [TestMethod]
        public void GetId_Contributor()
        {
            StoreKey key = StoreKey.Contributor("myBookId", "myContributorId");

            var result = StoreKeyHelper.GetStoreObjectId(key);

            Assert.AreEqual("myContributorId", result);
        }

        [TestMethod]
        public void Convert_Contributors()
        {
            StoreKey key = StoreKey.Contributors("myBookId");

            var result = StoreKeyHelper.Convert(key);

            Assert.AreEqual("myBookId/Contributors/", result);
        }

        [TestMethod]
        public void Convert_BookShare()
        {
            StoreKey key = StoreKey.BookShare("myBookId", "myBookShareId");

            var result = StoreKeyHelper.Convert(key);

            Assert.AreEqual("myBookId/Shares/myBookShareId.json", result);
        }

        [TestMethod]
        public void GetId_BookShare()
        {
            StoreKey key = StoreKey.BookShare("myBookId", "myBookShareId");

            var result = StoreKeyHelper.GetStoreObjectId(key);

            Assert.AreEqual("myBookShareId", result);
        }

        [TestMethod]
        public void Convert_BookShares()
        {
            StoreKey key = StoreKey.BookShares("myBookId");

            var result = StoreKeyHelper.Convert(key);

            Assert.AreEqual("myBookId/Shares/", result);
        }
    }
}
