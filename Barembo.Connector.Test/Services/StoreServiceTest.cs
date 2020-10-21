using Barembo.Models;
using Barembo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Barembo.Connector.Test.Services
{
    [TestClass]
    public partial class StoreServiceTest
    {
        /// <summary>
        /// Create another CS-file "StoreServiceTestSecret.cs" and add the following:
        /// public partial class StoreServiceTest
        /// {
        ///     static StoreServiceTest()
        ///     {
        ///         _accessGrantForTesting = ""; //Add an AccessGrant for your tests
        ///     }
        /// }
        /// 
        /// Be sure to exclude this file from Git to now expose your Access!
        /// </summary>
        static string _accessGrantForTesting;

        StoreService _storeService;
        StoreAccess _storeAccess;

        [TestInitialize]
        public void Init()
        {
            _storeService = new StoreService("barembo-test");
            _storeAccess = new StoreAccess(_accessGrantForTesting);
        }

        [TestMethod]
        public async Task PutAsJson_Uploads_AsJson()
        {
            Book book = new Book();
            book.Name = "Test-Book - " + DateTime.Now.ToString();

            var result = await _storeService.PutObjectAsJsonAsync<Book>(_storeAccess, StoreKey.Book(book.Id), book);

            Assert.IsTrue(result);

            var downloaded = await _storeService.GetObjectFromJsonAsync<Book>(_storeAccess, StoreKey.Book(book.Id));

            Assert.AreEqual(book.Id, downloaded.Id);
            Assert.AreEqual(book.Name, downloaded.Name);
        }

        [TestMethod]
        public async Task List_Finds_Objects()
        {
            Book book = new Book();
            book.Name = "Test-Book - " + DateTime.Now.ToString();
            Entry entry1 = new Entry();
            Entry entry2 = new Entry();

            await _storeService.PutObjectAsJsonAsync<Entry>(_storeAccess, StoreKey.Entry(book.Id, entry1.Id, "contrib1"), entry1);
            await _storeService.PutObjectAsJsonAsync<Entry>(_storeAccess, StoreKey.Entry(book.Id, entry2.Id, "contrib2"), entry2);

            var list = await _storeService.ListObjectsAsync(_storeAccess, StoreKey.Entries(book.Id));

            Assert.AreEqual(2, list.Count());
            Assert.AreEqual(entry1.Id, list.Last().Id); //List is inverse
            Assert.AreEqual(entry2.Id, list.First().Id);
        }

        [TestMethod]
        public async Task PutAsStream_Uploads_AsStream()
        {
            string stringToStream = "This is a stream test";
            string testGuid = Guid.NewGuid().ToString();
            MemoryStream mstream = new MemoryStream(Encoding.UTF8.GetBytes(stringToStream));

            var result = await _storeService.PutObjectFromStreamAsync(_storeAccess, StoreKey.Book(testGuid), mstream);

            Assert.IsTrue(result);

            var downloaded = await _storeService.GetObjectAsStreamAsync(_storeAccess, StoreKey.Book(testGuid));
            byte[] downloadedBytes = new byte[mstream.Length];
            downloaded.Read(downloadedBytes, 0, (int)mstream.Length);
            MemoryStream downloadStream = new MemoryStream(downloadedBytes);

            var downloadedString = Encoding.UTF8.GetString(downloadStream.ToArray());

            Assert.AreEqual(stringToStream, downloadedString);
        }
    }
}
