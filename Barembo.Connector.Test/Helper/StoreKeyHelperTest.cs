﻿using Barembo.Helper;
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
    }
}
