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
    public class BookServiceTest
    {
        BookService _bookService;
        Moq.Mock<IBookStoreService> _bookStoreServiceMock;
        Moq.Mock<IBookShareStoreService> _bookShareStoreServiceMock;

        [TestInitialize]
        public void Init()
        {
            _bookStoreServiceMock = new Moq.Mock<IBookStoreService>();
            _bookShareStoreServiceMock = new Moq.Mock<IBookShareStoreService>();

            _bookService = new BookService(_bookStoreServiceMock.Object, _bookShareStoreServiceMock.Object);
        }

        [TestMethod]
        public async Task SaveBook_Saves_Book()
        {
            BookReference bookReference = new BookReference();
            Book book = new Book();

            _bookStoreServiceMock.Setup(s => s.SaveAsync(bookReference, book)).Returns(Task.FromResult(true)).Verifiable();

            var result = await _bookService.SaveBookAsync(bookReference, book);

            Assert.IsTrue(result);
            _bookStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task SaveBook_ReturnsFalse_IfBookCouldNotBeSaved()
        {
            BookReference bookReference = new BookReference();
            Book book = new Book();

            _bookStoreServiceMock.Setup(s => s.SaveAsync(bookReference, book)).Returns(Task.FromResult(false)).Verifiable();

            var result = await _bookService.SaveBookAsync(bookReference, book);

            Assert.IsFalse(result);
            _bookStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task LoadBook_Loads_Book()
        {
            BookReference bookReference = new BookReference();
            Book book = new Book();
            bookReference.BookId = book.Id;

            _bookStoreServiceMock.Setup(s => s.LoadAsync(bookReference)).Returns(Task.FromResult(book)).Verifiable();

            var result = await _bookService.LoadBookAsync(bookReference);

            Assert.AreEqual(book, result);
            _bookStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task LoadBook_RaisesError_IfBookCouldNotBeFound()
        {
            BookReference bookReference = new BookReference();
            Book book = new Book();
            bookReference.BookId = book.Id;

            _bookStoreServiceMock.Setup(s => s.LoadAsync(bookReference)).Throws(new BookNotExistsException()).Verifiable();

            try
            {
                var result = await _bookService.LoadBookAsync(bookReference);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(BookNotExistsException));
            }

            _bookStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task LoadBook_Refreshes_BookAccessOnForeignBooks()
        {
            StoreAccess newAccess = new StoreAccess("new access");
            AccessRights newAccessRights = new AccessRights();
            BookShareReference bookShareReference = new BookShareReference();
            BookReference bookReference = new BookReference();
            Book book = new Book();
            bookReference.BookId = book.Id;
            bookReference.BookShareReference = bookShareReference;
            BookShare bookShare = new BookShare();
            bookShare.Access = newAccess;
            bookShare.AccessRights = newAccessRights;

            _bookShareStoreServiceMock.Setup(s => s.LoadBookShareAsync(bookShareReference)).Returns(Task.FromResult(bookShare)).Verifiable();
            _bookStoreServiceMock.Setup(s => s.LoadAsync(bookReference)).Returns(Task.FromResult(book)).Verifiable();

            var result = await _bookService.LoadBookAsync(bookReference);

            Assert.AreEqual(book, result);
            Assert.AreEqual(newAccess.AccessGrant, bookReference.AccessGrant);
            Assert.AreEqual(newAccessRights, bookReference.AccessRights);
            _bookStoreServiceMock.Verify();
            _bookShareStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task CreateBook_Creates_Book()
        {
            var result = await _bookService.CreateBookAsync("bookName", "bookDescription");

            Assert.AreEqual("bookName", result.Name);
            Assert.AreEqual("bookDescription", result.Description);
            Assert.IsFalse(string.IsNullOrEmpty(result.Id));
        }
    }
}
