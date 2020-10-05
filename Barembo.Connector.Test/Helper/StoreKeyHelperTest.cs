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
            StoreKey key = new StoreKey(StoreKeyTypes.BookShelf);

            var result = StoreKeyHelper.Convert(key);

            Assert.AreEqual("BookShelf.json", result);
        }
    }
}
