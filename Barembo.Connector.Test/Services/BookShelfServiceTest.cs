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
        Moq.Mock<IBookShareStoreService> _bookShareStoreServiceMock;
        Moq.Mock<IStoreAccessService> _storeAccessService;

        [TestInitialize]
        public void Init()
        {
            _bookShelfStoreServiceMock = new Moq.Mock<IBookShelfStoreService>();
            _bookShareStoreServiceMock = new Moq.Mock<IBookShareStoreService>();
            _storeAccessService = new Moq.Mock<IStoreAccessService>();

            _bookShelfService = new BookShelfService(_bookShelfStoreServiceMock.Object, _bookShareStoreServiceMock.Object, _storeAccessService.Object);
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
            Contributor contributor = new Contributor();

            _bookShelfStoreServiceMock.Setup(s => s.LoadAsync(storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();
            _bookShelfStoreServiceMock.Setup(s => s.AddBookToBookShelf(bookShelf, bookToAdd.Id, bookShelf.OwnerName, storeAccess, Moq.It.IsAny<AccessRights>(), contributor.Id)).Returns(true).Verifiable();
            _bookShelfStoreServiceMock.Setup(s => s.SaveAsync(storeAccess, bookShelf)).Returns(Task.FromResult(true)).Verifiable();
            var result = await _bookShelfService.AddOwnBookToBookShelfAndSaveAsync(storeAccess, bookToAdd, contributor);

            Assert.IsTrue(result);
            _bookShelfStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task AddOwnBookAndSave_ReturnsFalse_IfBookShelfNotFound()
        {
            StoreAccess storeAccess = new StoreAccess("use this access");
            Book bookToAdd = new Book();
            BookShelf bookShelf = new BookShelf();
            Contributor contributor = new Contributor();

            _bookShelfStoreServiceMock.Setup(s => s.LoadAsync(storeAccess)).Throws(new NoBookShelfExistsException()).Verifiable();
            var result = await _bookShelfService.AddOwnBookToBookShelfAndSaveAsync(storeAccess, bookToAdd, contributor);

            Assert.IsFalse(result);
            _bookShelfStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task AddOwnBookAndSave_ReturnsFalse_IfBookIsAlreadyInBookShelf()
        {
            StoreAccess storeAccess = new StoreAccess("use this access");
            Book bookToAdd = new Book();
            BookShelf bookShelf = new BookShelf();
            Contributor contributor = new Contributor();

            _bookShelfStoreServiceMock.Setup(s => s.LoadAsync(storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();
            _bookShelfStoreServiceMock.Setup(s => s.AddBookToBookShelf(bookShelf, bookToAdd.Id, bookShelf.OwnerName, storeAccess, Moq.It.IsAny<AccessRights>(), contributor.Id)).Returns(false).Verifiable();
            var result = await _bookShelfService.AddOwnBookToBookShelfAndSaveAsync(storeAccess, bookToAdd, contributor);

            Assert.IsFalse(result);
            _bookShelfStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task AddOwnBookAndSave_ReturnsFalse_IfBookShelfCouldNotBeSavedAfterwards()
        {
            StoreAccess storeAccess = new StoreAccess("use this access");
            Book bookToAdd = new Book();
            BookShelf bookShelf = new BookShelf();
            Contributor contributor = new Contributor();

            _bookShelfStoreServiceMock.Setup(s => s.LoadAsync(storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();
            _bookShelfStoreServiceMock.Setup(s => s.AddBookToBookShelf(bookShelf, bookToAdd.Id, bookShelf.OwnerName, storeAccess, Moq.It.IsAny<AccessRights>(), contributor.Id)).Returns(true).Verifiable();
            _bookShelfStoreServiceMock.Setup(s => s.SaveAsync(storeAccess, bookShelf)).Returns(Task.FromResult(false)).Verifiable();
            var result = await _bookShelfService.AddOwnBookToBookShelfAndSaveAsync(storeAccess, bookToAdd, contributor);

            Assert.IsFalse(result);
            _bookShelfStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task Load_Loads_BookShelf()
        {
            BookShelf bookShelf = new BookShelf();

            StoreAccess storeAccess = new StoreAccess("use this access");
            _bookShelfStoreServiceMock.Setup(s => s.LoadAsync(storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();

            var result = await _bookShelfService.LoadBookShelfAsync(storeAccess);

            Assert.AreEqual(bookShelf, result);
        }

        [TestMethod]
        public async Task AddSharedBookAndSave_FetchesInfoAndAddsBookAndSaves()
        {
            Book sharedBook = new Book();
            Contributor contributor = new Contributor();
            BookShareReference bookShareReference = new BookShareReference();
            BookShare bookShare = new BookShare();
            bookShare.BookId = sharedBook.Id;
            bookShare.OwnerName = "foreign owner";
            bookShare.Access = new StoreAccess("foreign access");
            bookShare.AccessRights = AccessRights.Full;
            bookShare.ContributorId = contributor.Id;

            StoreAccess storeAccess = new StoreAccess("use this access");
            BookShelf bookShelf = new BookShelf();

            _bookShareStoreServiceMock.Setup(s => s.LoadBookShareAsync(bookShareReference)).Returns(Task.FromResult(bookShare)).Verifiable();
            _bookShelfStoreServiceMock.Setup(s => s.LoadAsync(storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();
            _bookShelfStoreServiceMock.Setup(s => s.AddBookToBookShelf(bookShelf, bookShare.BookId, bookShare.OwnerName, bookShare.Access, bookShare.AccessRights, bookShare.ContributorId)).Returns(true).Verifiable();
            _bookShelfStoreServiceMock.Setup(s => s.SaveAsync(storeAccess, bookShelf)).Returns(Task.FromResult(true)).Verifiable();

            var result = await _bookShelfService.AddSharedBookToBookShelfAndSaveAsync(storeAccess, bookShareReference);

            Assert.IsTrue(result);
            _bookShelfStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task ShareBook_Creates_BookShareAndBookShareReference()
        {
            Book sharedBook = new Book();
            Contributor contributor = new Contributor();
            StoreAccess storeAccess = new StoreAccess("use this access");
            BookShelf bookShelf = new BookShelf();
            bookShelf.OwnerName = "it's me";
            BookShareReference bookShareReference = new BookShareReference();
            AccessRights accessRights = new AccessRights();
            StoreAccess sharedStoreAccess = new StoreAccess("use this restricted access");


            _bookShelfStoreServiceMock.Setup(s => s.LoadAsync(storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();
            _storeAccessService.Setup(s => s.ShareBookAccessAsync(storeAccess, sharedBook, contributor, accessRights)).Returns(Task.FromResult(sharedStoreAccess)).Verifiable();
            _bookShareStoreServiceMock.Setup(s => s.SaveBookShareAsync(storeAccess,
                                                                       Moq.It.Is<BookShare>(b => b.BookId == sharedBook.Id &&
                                                                                            b.ContributorId == contributor.Id &&
                                                                                            b.OwnerName == bookShelf.OwnerName &&
                                                                                            b.AccessRights == accessRights))).Returns(Task.FromResult(bookShareReference)).Verifiable();
            var result = await _bookShelfService.ShareBookAsync(storeAccess, sharedBook, contributor, accessRights);

            Assert.AreEqual(bookShareReference, result);
            _storeAccessService.Verify();
            _bookShareStoreServiceMock.Verify();
            _bookShelfStoreServiceMock.Verify();
        }
    }
}
