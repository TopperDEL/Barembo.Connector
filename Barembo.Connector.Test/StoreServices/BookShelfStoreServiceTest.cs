using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using Barembo.StoreServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Barembo.Connector.Test.StoreServices
{
    [TestClass]
    public class BookShelfStoreServiceTest
    {
        BookShelfStoreService _service;
        Moq.Mock<IStoreService> _storeServiceMock;
        StoreAccess _storeAccess;

        [TestInitialize]
        public void Init()
        {
            _storeAccess = new StoreAccess("NoRealAccess");
            _storeServiceMock = new Moq.Mock<IStoreService>();
            _service = new BookShelfStoreService(_storeServiceMock.Object);
        }

        [TestMethod]
        public async Task Load_ThrowsException_IfNoBookShelfExists()
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
        public async Task Load_LoadsABookShelf_IfItExists()
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
        public async Task Save_SavesABookShelf_ToTheStore()
        {
            var bookShelf = new BookShelf();

            _storeServiceMock.Setup(m => m.PutObjectAsJsonAsync<BookShelf>(_storeAccess, Moq.It.Is<StoreKey>(s => s.StoreKeyType == StoreKeyTypes.BookShelf), bookShelf))
                             .Returns(Task.FromResult(true)).Verifiable();

            var result = await _service.SaveAsync(_storeAccess, bookShelf);

            _storeServiceMock.Verify();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddNewBook_Adds_OwnBook()
        {
            var bookShelf = new BookShelf();
            var bookToAdd = new Book();
            var accessRights = new AccessRights();
            var contributor = new Contributor();

            var result =  _service.AddBookToBookShelf(bookShelf, bookToAdd.Id, "Goethe", _storeAccess, accessRights, contributor.Id, null);

            Assert.IsTrue(result);
            Assert.AreEqual("Goethe", bookShelf.Content[0].OwnerName);
            Assert.AreEqual(bookToAdd.Id, bookShelf.Content[0].BookId);
            Assert.AreEqual(accessRights, bookShelf.Content[0].AccessRights);
            Assert.AreEqual(_storeAccess.AccessGrant, bookShelf.Content[0].AccessGrant);
            Assert.AreEqual(contributor.Id, bookShelf.Content[0].ContributorId);
        }

        [TestMethod]
        public void AddNewBook_Fails_IfBookAlreadyExists()
        {
            var bookShelf = new BookShelf();
            var bookToAdd = new Book();
            var accessRights = new AccessRights();
            var contributor = new Contributor();

            _service.AddBookToBookShelf(bookShelf, bookToAdd.Id, "Goethe", _storeAccess, accessRights, contributor.Id);
            var result = _service.AddBookToBookShelf(bookShelf, bookToAdd.Id, "Goethe not again", _storeAccess, accessRights, contributor.Id);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AddNewBook_Adds_ForeignBook()
        {
            var bookShelf = new BookShelf();
            var bookToAdd = new Book();
            var accessRights = new AccessRights();
            var contributor = new Contributor();
            var bookShareReference = new BookShareReference();

            var result = _service.AddBookToBookShelf(bookShelf, bookToAdd.Id, "Goethe", _storeAccess, accessRights, contributor.Id, bookShareReference);

            Assert.IsTrue(result);
            Assert.AreEqual("Goethe", bookShelf.Content[0].OwnerName);
            Assert.AreEqual(bookToAdd.Id, bookShelf.Content[0].BookId);
            Assert.AreEqual(accessRights, bookShelf.Content[0].AccessRights);
            Assert.AreEqual(_storeAccess.AccessGrant, bookShelf.Content[0].AccessGrant);
            Assert.AreEqual(contributor.Id, bookShelf.Content[0].ContributorId);
            Assert.AreEqual(bookShareReference, bookShelf.Content[0].BookShareReference);
        }
    }
}
