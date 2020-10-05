using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using Barembo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Barembo.Connector.Test
{
    [TestClass]
    public class BookShelfServiceTest
    {
        BookShelfService _service;
        Moq.Mock<IStoreService> _storeServiceMock;
        StoreAccess _storeAccess;

        [TestInitialize]
        public void Init()
        {
            _storeAccess = new StoreAccess("NoRealAccess");
            _storeServiceMock = new Moq.Mock<IStoreService>();
            _service = new BookShelfService(_storeServiceMock.Object);
        }

        [TestMethod]
        public async Task ThrowsException_If_NoBookShelfExists()
        {

            _storeServiceMock.Setup(m => m.GetObjectInfoAsync(_storeAccess, Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.BookShelf)))
                             .Returns(Task.FromResult(new StoreObjectInfo() { ObjectExists = false }));
            try
            {
                await _service.LoadAsync(_storeAccess);
                Assert.IsTrue(false, "No exception thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(NoBookShelfExistsException));
            }
        }

        [TestMethod]
        public async Task LoadsAnExistingBookShelfFromTheStore()
        {
            var bookShelf = new BookShelf();

            _storeServiceMock.Setup(m => m.GetObjectInfoAsync(_storeAccess, Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.BookShelf)))
                             .Returns(Task.FromResult(new StoreObjectInfo() { ObjectExists = true })).Verifiable();

            _storeServiceMock.Setup(m => m.GetObjectFromJsonAsync<BookShelf>(_storeAccess, Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.BookShelf)))
                             .Returns(Task.FromResult(bookShelf)).Verifiable();

            var result = await _service.LoadAsync(_storeAccess);

            _storeServiceMock.Verify();

            Assert.AreEqual(bookShelf, result);
        }

        [TestMethod]
        public async Task SavesABookShelfToTheStore()
        {
            var bookShelf = new BookShelf();

            _storeServiceMock.Setup(m => m.PutObjectAsJsonAsync<BookShelf>(_storeAccess, Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.BookShelf), bookShelf))
                             .Returns(Task.FromResult(true)).Verifiable();

            var result = await _service.SaveAsync(_storeAccess, bookShelf);

            _storeServiceMock.Verify();

            Assert.IsTrue(result);
        }
    }
}
