using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using Barembo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Connector.Test.Services
{
    [TestClass]
    public class BookShelfServiceTest
    {
        BookShelfService _bookShelfService;
        Moq.Mock<IBookShelfStoreService> _bookShelfStoreServiceMock;

        [TestInitialize]
        public void Init()
        {
            _bookShelfStoreServiceMock = new Moq.Mock<IBookShelfStoreService>();

            _bookShelfService = new BookShelfService(_bookShelfStoreServiceMock.Object);
        }

        [TestMethod]
        public async Task CreateAndSave_CreatesAndSaves_BookShelf()
        {
            StoreAccess storeAccess = new StoreAccess("use this access");

            _bookShelfStoreServiceMock.Setup(s => s.SaveAsync(storeAccess, Moq.It.Is<BookShelf>(b => b.OwnerName == "i_am_the_owner"))).Returns(Task.FromResult(true)).Verifiable();
            var result = await _bookShelfService.CreateAndSaveBookShelfAsync(storeAccess, "i_am_the_owner");

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.AreEqual("i_am_the_owner", result.OwnerName);
            _bookShelfStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task CreateAndSave_RaisesError_IfBookShelfCouldNotBeSaved()
        {
            StoreAccess storeAccess = new StoreAccess("use this access");

            _bookShelfStoreServiceMock.Setup(s => s.SaveAsync(storeAccess, Moq.It.Is<BookShelf>(b => b.OwnerName == "i_am_the_owner"))).Returns(Task.FromResult(false)).Verifiable();

            try
            {
                var result = await _bookShelfService.CreateAndSaveBookShelfAsync(storeAccess, "i_am_the_owner");
                Assert.IsTrue(false);
            }
            catch(Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(BookShelfCouldNotBeSavedException));
            }

            _bookShelfStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task AddOwnBookAndSave_AddsBookAndSaves()
        {
            StoreAccess storeAccess = new StoreAccess("use this access");
            Book bookToAdd = new Book();
            BookShelf bookShelf = new BookShelf();

            _bookShelfStoreServiceMock.Setup(s => s.LoadAsync(storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();
            _bookShelfStoreServiceMock.Setup(s => s.AddBookToBookShelf(bookShelf, bookToAdd.Id, bookShelf.OwnerName, storeAccess, Moq.It.IsAny<AccessRights>())).Returns(true).Verifiable();
            _bookShelfStoreServiceMock.Setup(s => s.SaveAsync(storeAccess, bookShelf)).Returns(Task.FromResult(true)).Verifiable();
            var result = await _bookShelfService.AddOwnBookToBookShelfAndSaveAsync(storeAccess, bookToAdd);

            Assert.IsTrue(result);
            _bookShelfStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task AddOwnBookAndSave_ReturnsFalse_IfBookShelfNotFound()
        {
            StoreAccess storeAccess = new StoreAccess("use this access");
            Book bookToAdd = new Book();
            BookShelf bookShelf = new BookShelf();

            _bookShelfStoreServiceMock.Setup(s => s.LoadAsync(storeAccess)).Throws(new NoBookShelfExistsException()).Verifiable();
            var result = await _bookShelfService.AddOwnBookToBookShelfAndSaveAsync(storeAccess, bookToAdd);

            Assert.IsFalse(result);
            _bookShelfStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task AddOwnBookAndSave_ReturnsFalse_IfBookIsAlreadyInBookShelf()
        {
            StoreAccess storeAccess = new StoreAccess("use this access");
            Book bookToAdd = new Book();
            BookShelf bookShelf = new BookShelf();

            _bookShelfStoreServiceMock.Setup(s => s.LoadAsync(storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();
            _bookShelfStoreServiceMock.Setup(s => s.AddBookToBookShelf(bookShelf, bookToAdd.Id, bookShelf.OwnerName, storeAccess, Moq.It.IsAny<AccessRights>())).Returns(false).Verifiable();
            var result = await _bookShelfService.AddOwnBookToBookShelfAndSaveAsync(storeAccess, bookToAdd);

            Assert.IsFalse(result);
            _bookShelfStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task AddOwnBookAndSave_ReturnsFalse_IfBookShelfCouldNotBeSavedAfterwards()
        {
            StoreAccess storeAccess = new StoreAccess("use this access");
            Book bookToAdd = new Book();
            BookShelf bookShelf = new BookShelf();

            _bookShelfStoreServiceMock.Setup(s => s.LoadAsync(storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();
            _bookShelfStoreServiceMock.Setup(s => s.AddBookToBookShelf(bookShelf, bookToAdd.Id, bookShelf.OwnerName, storeAccess, Moq.It.IsAny<AccessRights>())).Returns(true).Verifiable();
            _bookShelfStoreServiceMock.Setup(s => s.SaveAsync(storeAccess, bookShelf)).Returns(Task.FromResult(false)).Verifiable();
            var result = await _bookShelfService.AddOwnBookToBookShelfAndSaveAsync(storeAccess, bookToAdd);

            Assert.IsFalse(result);
            _bookShelfStoreServiceMock.Verify();
        }
    }
}
