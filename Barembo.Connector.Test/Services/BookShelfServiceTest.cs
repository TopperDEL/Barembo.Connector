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
        Moq.Mock<IContributorStoreService> _contributorStoreService;

        [TestInitialize]
        public void Init()
        {
            _bookShelfStoreServiceMock = new Moq.Mock<IBookShelfStoreService>();
            _bookShareStoreServiceMock = new Moq.Mock<IBookShareStoreService>();
            _contributorStoreService = new Moq.Mock<IContributorStoreService>();
            _storeAccessService = new Moq.Mock<IStoreAccessService>();

            _bookShelfService = new BookShelfService(_bookShelfStoreServiceMock.Object, _bookShareStoreServiceMock.Object, _storeAccessService.Object, _contributorStoreService.Object);
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
            catch (Exception ex)
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
            _bookShelfStoreServiceMock.Setup(s => s.AddBookToBookShelf(bookShelf, bookShare.BookId, bookShare.OwnerName, bookShare.Access, bookShare.AccessRights, bookShare.ContributorId, bookShareReference)).Returns(true).Verifiable();
            _bookShelfStoreServiceMock.Setup(s => s.SaveAsync(storeAccess, bookShelf)).Returns(Task.FromResult(true)).Verifiable();

            var result = await _bookShelfService.AddSharedBookToBookShelfAndSaveAsync(storeAccess, bookShareReference);

            Assert.IsTrue(result);
            _bookShelfStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task ShareBook_Creates_ContributorAndBookShareAndBookShareReference()
        {
            Book sharedBook = new Book();
            Contributor contributor = new Contributor();
            contributor.Name = "my significant other";
            StoreAccess storeAccess = new StoreAccess("use this access");
            BookShelf bookShelf = new BookShelf();
            bookShelf.OwnerName = "it's me";
            BookShareReference bookShareReference = new BookShareReference();
            AccessRights accessRights = new AccessRights();
            StoreAccess sharedStoreAccess = new StoreAccess("use this restricted access");
            BookReference bookReference = new BookReference();
            bookReference.BookId = sharedBook.Id;
            bookReference.OwnerName = bookShelf.OwnerName;

            _contributorStoreService.Setup(s => s.SaveAsync(bookReference, Moq.It.Is<Contributor>(c => c.Name == contributor.Name))).Returns(Task.FromResult(true)).Verifiable();
            _bookShelfStoreServiceMock.Setup(s => s.LoadAsync(storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();
            _storeAccessService.Setup(s => s.ShareBookAccess(storeAccess, bookReference, Moq.It.Is<Contributor>(c => c.Name == contributor.Name), accessRights)).Returns(sharedStoreAccess).Verifiable();
            _bookShareStoreServiceMock.Setup(s => s.SaveBookShareAsync(storeAccess,
                                                                       Moq.It.Is<BookShare>(b => b.BookId == sharedBook.Id &&
                                                                                            b.OwnerName == bookShelf.OwnerName &&
                                                                                            b.AccessRights == accessRights))).Returns(Task.FromResult(bookShareReference)).Verifiable();
            var result = await _bookShelfService.ShareBookAsync(storeAccess, bookReference, "my significant other", accessRights);

            Assert.AreEqual(bookShareReference, result);
            _storeAccessService.Verify();
            _bookShareStoreServiceMock.Verify();
            _bookShelfStoreServiceMock.Verify();
            _contributorStoreService.Verify();
        }

        [TestMethod]
        public async Task ShareBook_RaisesError_IfBookShelfNotExists()
        {
            Book sharedBook = new Book();
            Contributor contributor = new Contributor();
            contributor.Name = "my significant other";
            StoreAccess storeAccess = new StoreAccess("use this access");
            BookShelf bookShelf = new BookShelf();
            bookShelf.OwnerName = "it's me";
            BookShareReference bookShareReference = new BookShareReference();
            AccessRights accessRights = new AccessRights();
            StoreAccess sharedStoreAccess = new StoreAccess("use this restricted access");
            BookReference bookReference = new BookReference();
            bookReference.BookId = sharedBook.Id;
            bookReference.OwnerName = bookShelf.OwnerName;

            _bookShelfStoreServiceMock.Setup(s => s.LoadAsync(storeAccess)).Throws(new NoBookShelfExistsException()).Verifiable();
            try
            {
                var result = await _bookShelfService.ShareBookAsync(storeAccess, bookReference, "my significant other", accessRights);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(CouldNotShareBookException));
                Assert.AreEqual(CouldNotShareBookReason.BookShelfNotFound, ((CouldNotShareBookException)ex).Reason);
            }

            _storeAccessService.Verify();
            _bookShareStoreServiceMock.Verify();
            _bookShelfStoreServiceMock.Verify();
            _contributorStoreService.Verify();
        }

        [TestMethod]
        public async Task ShareBook_RaisesError_IfContributorCouldNotBeSaved()
        {
            Book sharedBook = new Book();
            Contributor contributor = new Contributor();
            contributor.Name = "my significant other";
            StoreAccess storeAccess = new StoreAccess("use this access");
            BookShelf bookShelf = new BookShelf();
            bookShelf.OwnerName = "it's me";
            BookShareReference bookShareReference = new BookShareReference();
            AccessRights accessRights = new AccessRights();
            StoreAccess sharedStoreAccess = new StoreAccess("use this restricted access");
            BookReference bookReference = new BookReference();
            bookReference.BookId = sharedBook.Id;
            bookReference.OwnerName = bookShelf.OwnerName;

            _contributorStoreService.Setup(s => s.SaveAsync(bookReference, Moq.It.Is<Contributor>(c => c.Name == contributor.Name))).Returns(Task.FromResult(false)).Verifiable();
            _bookShelfStoreServiceMock.Setup(s => s.LoadAsync(storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();
            try
            {
                var result = await _bookShelfService.ShareBookAsync(storeAccess, bookReference, "my significant other", accessRights);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(CouldNotShareBookException));
                Assert.AreEqual(CouldNotShareBookReason.CouldNotSaveContributor, ((CouldNotShareBookException)ex).Reason);
            }

            _storeAccessService.Verify();
            _bookShareStoreServiceMock.Verify();
            _bookShelfStoreServiceMock.Verify();
            _contributorStoreService.Verify();
        }

        [TestMethod]
        public async Task ShareBook_RaisesError_IfBookShareCouldNotBeSaved()
        {
            Book sharedBook = new Book();
            Contributor contributor = new Contributor();
            contributor.Name = "my significant other";
            StoreAccess storeAccess = new StoreAccess("use this access");
            BookShelf bookShelf = new BookShelf();
            bookShelf.OwnerName = "it's me";
            BookShareReference bookShareReference = new BookShareReference();
            AccessRights accessRights = new AccessRights();
            StoreAccess sharedStoreAccess = new StoreAccess("use this restricted access");
            BookReference bookReference = new BookReference();
            bookReference.BookId = sharedBook.Id;
            bookReference.OwnerName = bookShelf.OwnerName;

            _contributorStoreService.Setup(s => s.SaveAsync(bookReference, Moq.It.Is<Contributor>(c => c.Name == contributor.Name))).Returns(Task.FromResult(true)).Verifiable();
            _bookShelfStoreServiceMock.Setup(s => s.LoadAsync(storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();
            _storeAccessService.Setup(s => s.ShareBookAccess(storeAccess, bookReference, Moq.It.Is<Contributor>(c => c.Name == contributor.Name), accessRights)).Returns(sharedStoreAccess).Verifiable();
            _bookShareStoreServiceMock.Setup(s => s.SaveBookShareAsync(storeAccess,
                                                                       Moq.It.Is<BookShare>(b => b.BookId == sharedBook.Id &&
                                                                                            b.OwnerName == bookShelf.OwnerName &&
                                                                                            b.AccessRights == accessRights))).Throws(new BookShareCouldNotBeSavedException()).Verifiable();
            try
            {
                var result = await _bookShelfService.ShareBookAsync(storeAccess, bookReference, "my significant other", accessRights);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(CouldNotShareBookException));
                Assert.AreEqual(CouldNotShareBookReason.BookShareCouldNotBeSaved, ((CouldNotShareBookException)ex).Reason);
            }

            _storeAccessService.Verify();
            _bookShareStoreServiceMock.Verify();
            _bookShelfStoreServiceMock.Verify();
            _contributorStoreService.Verify();
        }

        [TestMethod]
        public async Task ShareBook_RaisesError_IfActionNotAllowed()
        {
            Book sharedBook = new Book();
            Contributor contributor = new Contributor();
            contributor.Name = "my significant other";
            StoreAccess storeAccess = new StoreAccess("use this access");
            BookShelf bookShelf = new BookShelf();
            bookShelf.OwnerName = "it's me";
            BookShareReference bookShareReference = new BookShareReference();
            AccessRights accessRights = new AccessRights();
            StoreAccess sharedStoreAccess = new StoreAccess("use this restricted access");
            BookReference bookReference = new BookReference();
            bookReference.BookId = sharedBook.Id;
            bookReference.OwnerName = bookShelf.OwnerName;
            bookReference.AccessRights.CanShareBook = false;

            try
            {
                var result = await _bookShelfService.ShareBookAsync(storeAccess, bookReference, "my significant other", accessRights);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ActionNotAllowedException));
            }

            _storeAccessService.Verify();
            _bookShareStoreServiceMock.Verify();
            _bookShelfStoreServiceMock.Verify();
            _contributorStoreService.Verify();
        }

        [TestMethod]
        public async Task ListBookShares_Lists_BookShareReferences()
        {
            StoreAccess storeAccess = new StoreAccess("use this access");
            BookReference bookReference = new BookReference();
            List<BookShareReference> list = new List<BookShareReference>();

            _bookShareStoreServiceMock.Setup(s => s.ListBookSharesAsync(storeAccess, bookReference)).Returns(Task.FromResult(list as IEnumerable<BookShareReference>)).Verifiable();
            var result = await _bookShelfService.ListBookSharesAsync(storeAccess, bookReference);

            Assert.AreEqual(list, result);

            _bookShelfStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task RefreshBook_RefreshesBookAccess_ForForeignBook()
        {
            StoreAccess newAccess = new StoreAccess("new Access");
            AccessRights newAccessRights = new AccessRights();
            BookShelf bookShelf = new BookShelf();
            BookShareReference bookShareReference = new BookShareReference();
            BookReference bookReference = new BookReference();
            bookReference.BookShareReference = bookShareReference;
            BookShare bookShare = new BookShare();
            bookShare.Access = newAccess;
            bookShare.AccessRights = newAccessRights;

            _bookShareStoreServiceMock.Setup(s => s.LoadBookShareAsync(bookShareReference)).Returns(Task.FromResult(bookShare)).Verifiable();          
            await _bookShelfService.RefreshBookAccessAsync(bookReference);

            Assert.AreEqual(newAccess.AccessGrant, bookReference.AccessGrant);
            Assert.AreEqual(newAccessRights, bookReference.AccessRights);
            _bookShelfStoreServiceMock.Verify();
            _bookShareStoreServiceMock.Verify();
        }
    }
}
