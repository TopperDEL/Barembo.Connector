using Barembo.Models;
using Barembo.Services;
using Barembo.StoreServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Barembo.Connector.Test.Services
{
    [TestClass]
    public class StoreAccessServiceTest
    {
        StoreAccessService _service;
        BookShareStoreService _bookShareStoreService;
        StoreService _storeService;
        StoreAccess _storeAccess;
        string _bucket;

        [TestInitialize]
        public void Init()
        {
            _bucket = "barembo-test";// + Guid.NewGuid().ToString();
            _service = new StoreAccessService(_bucket);
            _storeService = new StoreService(_bucket);
            _storeAccess = new StoreAccess(StoreServiceTest._accessGrantForTesting);
            _bookShareStoreService = new BookShareStoreService(_storeService, _service);
        }

        [TestMethod]
        [TestCategory("NeedsSpecificBinaries")]
        public async Task ShareBookAccess_Creates_UsableAccess()
        {
            Book book = new Book();
            book.Name = "TestBook";
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;

            Contributor contributor = new Contributor();
            AccessRights accessRights = new AccessRights();
            accessRights.CanEditBook = true;

            await _storeService.PutObjectAsJsonAsync(_storeAccess, StoreKey.Book(book.Id), book);

            var access = _service.ShareBookAccess(_storeAccess, bookReference, contributor, accessRights);

            await EnsureAccessIsInitialisedAsync(access);

            var result = await _storeService.GetObjectFromJsonAsync<Book>(access, StoreKey.Book(bookReference.BookId));

            Assert.AreEqual(book.Id, result.Id);
            Assert.AreEqual(book.Name, result.Name);
        }

        [TestMethod]
        [TestCategory("NeedsSpecificBinaries")]
        public async Task ShareBookAccess_Creates_UsableAccessToEditTheBook()
        {
            Book book = new Book();
            book.Name = "TestBook";
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;

            Contributor contributor = new Contributor();
            AccessRights accessRights = new AccessRights();
            accessRights.CanEditBook = true;

            await _storeService.PutObjectAsJsonAsync(_storeAccess, StoreKey.Book(book.Id), book);

            var access = _service.ShareBookAccess(_storeAccess, bookReference, contributor, accessRights);

            await EnsureAccessIsInitialisedAsync(access);

            book.Name = "New name";
            var editSuccess = await _storeService.PutObjectAsJsonAsync(access, StoreKey.Book(bookReference.BookId), book);

            Assert.IsTrue(editSuccess);

            var result = await _storeService.GetObjectFromJsonAsync<Book>(access, StoreKey.Book(bookReference.BookId));

            Assert.AreEqual(book.Id, result.Id);
            Assert.AreEqual(book.Name, result.Name);
        }

        [TestMethod]
        [TestCategory("NeedsSpecificBinaries")]
        public async Task ShareBookAccess_RestrictsBookEdit_IfCannotEditBook()
        {
            Book book = new Book();
            book.Name = "TestBook";
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;

            Contributor contributor = new Contributor();
            AccessRights accessRights = new AccessRights();
            accessRights.CanEditBook = false; //No book-edit-permision
            accessRights.CanReadEntries = true; //But allowed to see content

            await _storeService.PutObjectAsJsonAsync(_storeAccess, StoreKey.Book(book.Id), book);

            var access = _service.ShareBookAccess(_storeAccess, bookReference, contributor, accessRights);

            await EnsureAccessIsInitialisedAsync(access);

            book.Name = "New name";
            var editSuccess = await _storeService.PutObjectAsJsonAsync(access, StoreKey.Book(bookReference.BookId), book);

            Assert.IsFalse(editSuccess);

            var result = await _storeService.GetObjectFromJsonAsync<Book>(access, StoreKey.Book(bookReference.BookId));

            Assert.AreEqual(book.Id, result.Id);
            Assert.AreEqual("TestBook", result.Name); //Must be old name
        }

        [TestMethod]
        [TestCategory("NeedsSpecificBinaries")]
        public async Task ShareBookAccess_Creates_UsableAccessToSeeEntries()
        {
            Book book = new Book();
            book.Name = "TestBook";
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            Contributor contributor = new Contributor();
            AccessRights accessRights = new AccessRights();
            accessRights.CanEditBook = true;
            accessRights.CanReadEntries = true;
            Entry entry = new Entry();

            await _storeService.PutObjectAsJsonAsync(_storeAccess, StoreKey.Book(book.Id), book);
            await _storeService.PutObjectAsJsonAsync(_storeAccess, StoreKey.Entry(book.Id, entry.Id, contributor.Id), entry);

            var access = _service.ShareBookAccess(_storeAccess, bookReference, contributor, accessRights);

            await EnsureAccessIsInitialisedAsync(access);

            var entryList = await _storeService.ListObjectsAsync(access, StoreKey.Entries(bookReference.BookId));

            Assert.AreEqual(1, entryList.Count());
        }

        [TestMethod]
        [TestCategory("NeedsSpecificBinaries")]
        public async Task ShareBookAccess_RestrictsList_IfCannotReadEntries()
        {
            Book book = new Book();
            book.Name = "TestBook";
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            Contributor contributor = new Contributor();
            AccessRights accessRights = new AccessRights();
            accessRights.CanEditBook = true;
            accessRights.CanReadEntries = false; //No Read-permission
            Entry entry = new Entry();

            await _storeService.PutObjectAsJsonAsync(_storeAccess, StoreKey.Book(book.Id), book);
            await _storeService.PutObjectAsJsonAsync(_storeAccess, StoreKey.Entry(book.Id, entry.Id, contributor.Id), entry);

            var access = _service.ShareBookAccess(_storeAccess, bookReference, contributor, accessRights);

            await EnsureAccessIsInitialisedAsync(access);

            var entryList = await _storeService.ListObjectsAsync(access, StoreKey.Entries(bookReference.BookId));

            Assert.AreEqual(0, entryList.Count());
        }

        [TestMethod]
        [TestCategory("NeedsSpecificBinaries")]
        public async Task ShareBookShareAccess_Creates_UsableAccess()
        {
            BookShare bookShare = new BookShare();
            bookShare.BookId = Guid.NewGuid().ToString();

            var bookShareReference = await _bookShareStoreService.SaveBookShareAsync(_storeAccess, bookShare);

            await EnsureAccessIsInitialisedAsync(bookShareReference.StoreAccess);

            var result = await _storeService.GetObjectFromJsonAsync<BookShare>(bookShareReference.StoreAccess, bookShareReference.StoreKey);

            Assert.AreEqual(bookShare.Id, result.Id);
            Assert.AreEqual(bookShare.BookId, result.BookId);
        }

        /// <summary>
        /// As it takes some time for a satellite to register a shared access grant,
        /// wait until the Bucket gets available with the to-share access.
        /// </summary>
        /// <param name="access">The newly created access, that needs finalisation on the satellite</param>
        private async Task EnsureAccessIsInitialisedAsync(StoreAccess access)
        {
            uplink.NET.Models.Bucket bucket = null;
            int tryCount = 0;
            do
            {
                await Task.Delay(1000);
                bucket = await StoreService.GetBucketAsync(_bucket, access);
                tryCount++;
            } while (bucket == null || tryCount > 10);
        }
    }
}
