using Barembo.Models;
using Barembo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Connector.Test.Services
{
    [TestClass]
    public class StoreBufferTest
    {
        StoreBuffer _storeBuffer;
        StoreAccess _storeAccess;

        [TestInitialize]
        public void Init()
        {
            _storeBuffer = new StoreBuffer();
            _storeAccess = new StoreAccess();
        }

        [TestMethod]
        public async Task RemoveDatabase_Clears_Buffer()
        {
            await _storeBuffer.RemoveDatabaseAsync();

            Book book = new Book();
            StoreKey existing = StoreKey.Book(book.Id);

            await _storeBuffer.PutObjectToBufferAsync<Book>(_storeAccess, existing, book);

            var resultBefore = await _storeBuffer.IsBufferedAsync(_storeAccess, existing);
            await _storeBuffer.RemoveDatabaseAsync();
            var resultAtfer = await _storeBuffer.IsBufferedAsync(_storeAccess, existing);

            Assert.IsTrue(resultBefore);
            Assert.IsFalse(resultAtfer);
        }

        [TestMethod]
        public async Task IsBuffered_ReturnsFalse_IfNotInBuffer()
        {
            await _storeBuffer.RemoveDatabaseAsync();

            StoreKey notExisting = StoreKey.Book("book");

            var result = await _storeBuffer.IsBufferedAsync(_storeAccess, notExisting);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsBuffered_ReturnsTrue_IfInBuffer()
        {
            await _storeBuffer.RemoveDatabaseAsync();

            Book book = new Book();
            StoreKey existing = StoreKey.Book(book.Id);

            await _storeBuffer.PutObjectToBufferAsync<Book>(_storeAccess, existing, book);

            var result = await _storeBuffer.IsBufferedAsync(_storeAccess, existing);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task GetObjectFromBuffer_ReturnsNull_IfBufferIsEmpty()
        {
            await _storeBuffer.RemoveDatabaseAsync();

            Book book = new Book();
            StoreKey notExisting = StoreKey.Book(book.Id);

            var result = await _storeBuffer.GetObjectFromBufferAsync<Book>(_storeAccess, notExisting);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task BufferObject_Roundtrip_IsWorking()
        {
            await _storeBuffer.RemoveDatabaseAsync();

            Book book = new Book();
            book.Name = "myName";
            book.Description = "myDescription";
            StoreKey existing = StoreKey.Book(book.Id);

            await _storeBuffer.PutObjectToBufferAsync<Book>(_storeAccess, existing, book);

            var result = await _storeBuffer.GetObjectFromBufferAsync<Book>(_storeAccess, existing);

            Assert.AreEqual(book.Id, result.Id);
            Assert.AreEqual(book.Name, result.Name);
            Assert.AreEqual(book.Description, result.Description);
        }

        [TestMethod]
        public async Task BufferStream_Roundtrip_IsWorking()
        {
            await _storeBuffer.RemoveDatabaseAsync();

            StoreKey existing = StoreKey.Book("book");

            MemoryStream mstream = new MemoryStream(Encoding.UTF8.GetBytes("my stream data"));

            await _storeBuffer.PutObjectFromStreamToBufferAsync(_storeAccess, existing, mstream);

            var result = await _storeBuffer.GetObjectAsStreamFromBufferAsync(_storeAccess, existing);
            MemoryStream mstreamResult = new MemoryStream((int)result.Length);
            result.CopyTo(mstreamResult);

            var streamData = Encoding.UTF8.GetString(mstreamResult.GetBuffer());
            Assert.AreEqual("my stream data", streamData);
        }
    }
}
