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
            var resultAfter = await _storeBuffer.IsBufferedAsync(_storeAccess, existing);

            Assert.IsTrue(resultBefore);
            Assert.IsFalse(resultAfter);
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

        [TestMethod]
        public async Task GetNextBackgroundAction_ReturnsNull_IfThereIsNone()
        {
            await _storeBuffer.RemoveDatabaseAsync();

            var result = await _storeBuffer.GetNextBackgroundAction();

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetNextBackgroundAction_ReturnsOnlyOne_IfThereIsOnlyOne()
        {
            await _storeBuffer.RemoveDatabaseAsync();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("FilePath", "value1"); //strings have to be names FilePath

            BackgroundAction action = new BackgroundAction(BackgroundActionTypes.AddAttachment, parameters);
            
            await _storeBuffer.AddBackgroundAction(action);
            var result = await _storeBuffer.GetNextBackgroundAction();

            Assert.IsNotNull(result);
            Assert.AreEqual(action.Id, result.Id);
            Assert.AreEqual(action.ActionType, result.ActionType);
            Assert.AreEqual(parameters["FilePath"], result.GetParameters()["FilePath"]);
        }

        [TestMethod]
        public async Task GetNextBackgroundAction_ReturnsOnlyOneFirstOne_IfThereAreMany()
        {
            await _storeBuffer.RemoveDatabaseAsync();
            
            Dictionary<string, object> parameters1 = new Dictionary<string, object>();
            parameters1.Add("FilePath", "value1");
            BackgroundAction action1 = new BackgroundAction(BackgroundActionTypes.AddAttachment, parameters1);
            action1.Id = "First";

            Dictionary<string, object> parameters2 = new Dictionary<string, object>();
            parameters2.Add("FilePath", "value2");
            BackgroundAction action2 = new BackgroundAction(BackgroundActionTypes.AddAttachment, parameters2);
            action2.Id = "Second";

            Dictionary<string, object> parameters3 = new Dictionary<string, object>();
            parameters3.Add("FilePath", "value3");
            BackgroundAction action3 = new BackgroundAction(BackgroundActionTypes.AddAttachment, parameters3);
            action3.Id = "Third";

            await _storeBuffer.AddBackgroundAction(action2);
            await _storeBuffer.AddBackgroundAction(action1);
            await _storeBuffer.AddBackgroundAction(action3);
            var result = await _storeBuffer.GetNextBackgroundAction();

            Assert.IsNotNull(result);
            Assert.AreEqual("First", result.Id);
            Assert.AreEqual(action1.ActionType, result.ActionType);
            Assert.AreEqual(parameters1["FilePath"], result.GetParameters()["FilePath"]);
        }

        [TestMethod]
        public async Task RemoveBackgroundAction_Removes_BackgroundAction()
        {
            await _storeBuffer.RemoveDatabaseAsync();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("param1", "value1");

            BackgroundAction action = new BackgroundAction(BackgroundActionTypes.AddAttachment, parameters);

            await _storeBuffer.AddBackgroundAction(action);
            var result = await _storeBuffer.GetNextBackgroundAction();
            Assert.IsNotNull(result);
            await _storeBuffer.RemoveBackgroundAction(result);
            var result2 = await _storeBuffer.GetNextBackgroundAction();
            Assert.IsNull(result2);
        }
    }
}
