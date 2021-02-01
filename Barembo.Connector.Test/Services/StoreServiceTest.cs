using Barembo.Models;
using Barembo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Barembo.Exceptions;
using Barembo.Interfaces;

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
        internal static string _accessGrantForTesting;

        IStoreService _storeService;
        StoreAccess _storeAccess;

        [TestInitialize]
        public void Init()
        {
            _storeService = new StoreService("barembo-test");
            _storeAccess = new StoreAccess(_accessGrantForTesting);
        }

        [TestMethod]
        [TestCategory("NeedsSpecificBinaries")]
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
        [TestCategory("NeedsSpecificBinaries")]
        public async Task PutAsJsonWithMetaData_Uploads_AsJsonWithMetaData()
        {
            Book book = new Book();
            book.Name = "Test-Book - " + DateTime.Now.ToString();
            Entry entry1 = new Entry();
            StoreMetaData meta = new StoreMetaData() { Key = StoreMetaData.STOREMETADATA_TIMESTAMP, Value = "123" };

            await _storeService.PutObjectAsJsonAsync<Entry>(_storeAccess, StoreKey.Entry(book.Id, entry1.Id, "contrib1"), entry1, meta);

            var list = await _storeService.ListObjectsAsync(_storeAccess, StoreKey.Entries(book.Id), true);

            Assert.AreEqual(StoreMetaData.STOREMETADATA_TIMESTAMP, list.FirstOrDefault().MetaData.Key);
            Assert.AreEqual("123", list.FirstOrDefault().MetaData.Value);
        }

        [TestMethod]
        [TestCategory("NeedsSpecificBinaries")]
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
        }

        [TestMethod]
        [TestCategory("NeedsSpecificBinaries")]
        public async Task PutAsStream_Uploads_AsStream()
        {
            string stringToStream = "This is a stream test";
            string testGuid = Guid.NewGuid().ToString();
            MemoryStream mstream = new MemoryStream(Encoding.UTF8.GetBytes(stringToStream));

            var result = await _storeService.PutObjectFromStreamAsync(_storeAccess, StoreKey.Book(testGuid), mstream, "filePath");

            Assert.IsTrue(result);

            var downloaded = await _storeService.GetObjectAsStreamAsync(_storeAccess, StoreKey.Book(testGuid));
            byte[] downloadedBytes = new byte[mstream.Length];
            downloaded.Read(downloadedBytes, 0, (int)mstream.Length);
            MemoryStream downloadStream = new MemoryStream(downloadedBytes);

            var downloadedString = Encoding.UTF8.GetString(downloadStream.ToArray());

            Assert.AreEqual(stringToStream, downloadedString);
        }

        [TestMethod]
        public void GetObjectService_DoesNotCrash_IfStoreAccessIsEmpty()
        {
            try
            {
                StoreService.GetObjectService(new StoreAccess());
                Assert.IsTrue(false);
            }
            catch(Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(StoreAccessInvalidException));
            }
        }

        [TestMethod]
        public void GetBucketService_DoesNotCrash_IfStoreAccessIsEmpty()
        {
            try
            {
                StoreService.GetBucketService(new StoreAccess());
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(StoreAccessInvalidException));
            }
        }

        [TestMethod]
        public async Task GetBucket_DoesNotCrash_IfStoreAccessIsEmpty()
        {
            try
            {
                await StoreService.GetBucketAsync("bucketName", new StoreAccess());
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(StoreAccessInvalidException));
            }
        }
    }
}
