﻿using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using Barembo.StoreServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Barembo.Connector.Test.StoreServices
{
    [TestClass]
    public class BookShareStoreServiceTest
    {
        BookShareStoreService _service;
        Moq.Mock<IStoreService> _storeServiceMock;
        Moq.Mock<IStoreAccessService> _storeAccessServiceMock;

        [TestInitialize]
        public void Init()
        {
            _storeServiceMock = new Moq.Mock<IStoreService>();
            _storeAccessServiceMock = new Moq.Mock<IStoreAccessService>();
            _service = new BookShareStoreService(_storeServiceMock.Object, _storeAccessServiceMock.Object);
        }

        [TestMethod]
        public async Task Save_Saves_BookShare()
        {
            StoreAccess access = new StoreAccess("use this access");
            StoreAccess sharedAccess = new StoreAccess("use this restricted access");
            BookShare bookShare = new BookShare();

            _storeServiceMock.Setup(s => s.PutObjectAsJsonAsync(access, Moq.It.Is<StoreKey>(k => k.StoreKeyType == StoreKeyTypes.BookShare), bookShare)).Returns(Task.FromResult(true)).Verifiable();
            _storeAccessServiceMock.Setup(s => s.ShareBookShareAccessAsync(access, Moq.It.Is<StoreKey>(k => k.StoreKeyType == StoreKeyTypes.BookShare))).Returns(Task.FromResult(sharedAccess)).Verifiable();

            var result = await _service.SaveBookShareAsync(access, bookShare);

            _storeServiceMock.Verify();
            _storeAccessServiceMock.Verify();
        }

        [TestMethod]
        public async Task Save_ThrowsError_IfSaveFailed()
        {
            StoreAccess access = new StoreAccess("use this access");
            StoreAccess sharedAccess = new StoreAccess("use this restricted access");
            BookShare bookShare = new BookShare();

            _storeServiceMock.Setup(s => s.PutObjectAsJsonAsync(access, Moq.It.Is<StoreKey>(k => k.StoreKeyType == StoreKeyTypes.BookShare), bookShare)).Returns(Task.FromResult(false)).Verifiable();

            try
            {
                var result = await _service.SaveBookShareAsync(access, bookShare);
                Assert.IsTrue(false);
            }
            catch(Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(BookShareCouldNotBeSavedException));
            }

            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task Load_Loads_BookShare()
        {
            BookShare bookShare = new BookShare();
            BookShareReference reference = new BookShareReference();
            reference.StoreAccess = new StoreAccess("i got this access");
            reference.StoreKey = StoreKey.BookShare("myBookId", "myBookShareId");

            _storeServiceMock.Setup(s => s.GetObjectInfoAsync(reference.StoreAccess, reference.StoreKey)).Returns(Task.FromResult(new StoreObjectInfo() { ObjectExists = true })).Verifiable();
            _storeServiceMock.Setup(s => s.GetObjectFromJsonAsync<BookShare>(reference.StoreAccess, reference.StoreKey)).Returns(Task.FromResult(bookShare)).Verifiable();

            var result = await _service.LoadBookShareAsync(reference);

            Assert.AreEqual(bookShare, result);

            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task Load_ThrowsError_IfBookShareNotExists()
        {
            BookShare bookShare = new BookShare();
            BookShareReference reference = new BookShareReference();
            reference.StoreAccess = new StoreAccess("i got this access");
            reference.StoreKey = StoreKey.BookShare("myBookId", "myBookShareId");

            _storeServiceMock.Setup(s => s.GetObjectInfoAsync(reference.StoreAccess, reference.StoreKey)).Returns(Task.FromResult(new StoreObjectInfo() { ObjectExists = false })).Verifiable();

            try
            {
                var result = await _service.LoadBookShareAsync(reference);
                Assert.IsTrue(false);
            }
            catch(Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(BookShareNotFoundException));
            }

            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task List_Lists_AllBookShares()
        {
            StoreAccess access = new StoreAccess("use this access");
            Book book = new Book();
            var bookSharesToLoad = new List<StoreObject>();
            var share1 = new StoreObject(book.Id + "/Shares/share1.json", "share1");
            bookSharesToLoad.Add(share1);
            var share2 = new StoreObject(book.Id + "/Shares/share2.json", "share2");
            bookSharesToLoad.Add(share2);
            var share3 = new StoreObject(book.Id + "/Shares/share3.json", "share3");
            bookSharesToLoad.Add(share3);

            _storeServiceMock.Setup(s => s.ListObjectsAsync(access, Moq.It.Is<StoreKey>(k => k.StoreKeyType == StoreKeyTypes.BookShares)))
            .Returns(Task.FromResult(bookSharesToLoad as IEnumerable<StoreObject>)).Verifiable();

            var entries = (await _service.ListBookSharesAsync(access, book)).ToList();

            Assert.AreEqual(3, entries.Count());
            //Assert.AreEqual(share1.Id, entries[0].);
            //Assert.AreEqual(share2, entries[1]);
            //Assert.AreEqual(share3, entries[2]);

            _storeServiceMock.Verify();
        }
    }
}