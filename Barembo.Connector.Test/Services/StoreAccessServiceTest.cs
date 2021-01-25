using Barembo.Models;
using Barembo.Services;
using Barembo.StoreServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Connector.Test.Services
{
    [TestClass]
    public class StoreAccessServiceTest
    {
        StoreAccessService _service;
        BookShareStoreService _bookShareStoreService;
        StoreService _storeService;
        StoreAccess _storeAccess;

        [TestInitialize]
        public void Init()
        {
            _service = new StoreAccessService("barembo-test");
            _storeService = new StoreService("barembo-test");
            _storeAccess = new StoreAccess(StoreServiceTest._accessGrantForTesting);
            _bookShareStoreService = new BookShareStoreService(_storeService, _service);
        }

        [TestMethod]
        [TestCategory("NeedsSpecificBinaries")]
        public void ShareBookAccess_Creates_UsableAccess()
        {
            //ToDo
        }

        [TestMethod]
        [TestCategory("NeedsSpecificBinaries")]
        public async Task ShareBookShareAccess_Creates_UsableAccess()
        {
            BookShare bookShare = new BookShare();
            bookShare.BookId = Guid.NewGuid().ToString();

            var bookShareReference = await _bookShareStoreService.SaveBookShareAsync(_storeAccess, bookShare);

            //var accessToBookShareReference = _service.ShareBookShareAccess(_storeAccess, bookShareReference.StoreKey);

            var result = await _storeService.GetObjectFromJsonAsync<BookShare>(bookShareReference.StoreAccess, bookShareReference.StoreKey);

            Assert.AreEqual(bookShare.Id, result.Id);
            Assert.AreEqual(bookShare.BookId, result.BookId);
        }
    }
}
