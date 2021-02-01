using Barembo.Interfaces;
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
    public class BufferedStoreServiceTest
    {
        BufferedStoreService _bufferedStoreService;
        Moq.Mock<IStoreBuffer> _storeBufferMock;
        Moq.Mock<IStoreService> _storeServiceMock;

        StoreAccess _access;
        StoreKey _storeKey;

        [TestInitialize]
        public void Init()
        {
            _access = new StoreAccess("use this access");
            _storeKey = StoreKey.Entry("book", "entry", "contributor");

            _storeBufferMock = new Moq.Mock<IStoreBuffer>();
            _storeServiceMock = new Moq.Mock<IStoreService>();

            _bufferedStoreService = new BufferedStoreService(_storeBufferMock.Object, _storeServiceMock.Object);
        }

        [TestMethod]
        public async Task List_Does_NoBuffering()
        {
            List<StoreObject> objects = new List<StoreObject>();

            _storeServiceMock.Setup(s => s.ListObjectsAsync(_access, _storeKey)).Returns(Task.FromResult(objects as IEnumerable<StoreObject>)).Verifiable();

            var result = await _bufferedStoreService.ListObjectsAsync(_access, _storeKey);

            Assert.AreEqual(objects, result);

            _storeBufferMock.VerifyNoOtherCalls();
            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task PutObjectAsJson_PutsToBuffer_IfAddedToStore()
        {
            Entry entry = new Entry();

            _storeServiceMock.Setup(s => s.PutObjectAsJsonAsync<Entry>(_access, _storeKey, entry)).Returns(Task.FromResult(true)).Verifiable();
            _storeBufferMock.Setup(s => s.PutObjectToBufferAsync<Entry>(_access, _storeKey, entry)).Returns(Task.FromResult(true)).Verifiable();

            var result = await _bufferedStoreService.PutObjectAsJsonAsync<Entry>(_access, _storeKey, entry);

            Assert.IsTrue(result);

            _storeBufferMock.Verify();
            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task PutObjectAsJson_PutsNotToBuffer_IfNotAddedToStore()
        {
            Entry entry = new Entry();

            _storeServiceMock.Setup(s => s.PutObjectAsJsonAsync<Entry>(_access, _storeKey, entry)).Returns(Task.FromResult(false)).Verifiable();

            var result = await _bufferedStoreService.PutObjectAsJsonAsync<Entry>(_access, _storeKey, entry);

            Assert.IsFalse(result);

            _storeBufferMock.VerifyNoOtherCalls();
            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task PutObjectAsStream_PutsToBuffer_IfAddedToStore()
        {
            Stream stream = new MemoryStream();

            _storeServiceMock.Setup(s => s.PutObjectFromStreamAsync(_access, _storeKey, stream, "filePath")).Returns(Task.FromResult(true)).Verifiable();
            _storeBufferMock.Setup(s => s.PutObjectFromStreamToBufferAsync(_access, _storeKey, stream)).Returns(Task.FromResult(true)).Verifiable();

            var result = await _bufferedStoreService.PutObjectFromStreamAsync(_access, _storeKey, stream, "filePath");

            Assert.IsTrue(result);

            _storeBufferMock.Verify();
            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task PutObjectAsStream_PutsNotToBuffer_IfNotAddedToStore()
        {
            Stream stream = new MemoryStream();

            _storeServiceMock.Setup(s => s.PutObjectFromStreamAsync(_access, _storeKey, stream, "filePath")).Returns(Task.FromResult(false)).Verifiable();

            var result = await _bufferedStoreService.PutObjectFromStreamAsync(_access, _storeKey, stream, "filePath");

            Assert.IsFalse(result);

            _storeBufferMock.VerifyNoOtherCalls();
            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task GetInfo_Does_NoBuffering()
        {
            StoreObjectInfo info = new StoreObjectInfo();

            _storeServiceMock.Setup(s => s.GetObjectInfoAsync(_access, _storeKey)).Returns(Task.FromResult(info)).Verifiable();

            var result = await _bufferedStoreService.GetObjectInfoAsync(_access, _storeKey);

            Assert.AreEqual(info, result);

            _storeBufferMock.VerifyNoOtherCalls();
            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task GetObjectAsJson_GetsFromBuffer()
        {
            Entry entry = new Entry();

            _storeBufferMock.Setup(s => s.GetObjectFromBufferAsync<Entry>(_access, _storeKey)).Returns(Task.FromResult(entry)).Verifiable();

            var result = await _bufferedStoreService.GetObjectFromJsonAsync<Entry>(_access, _storeKey);

            Assert.AreEqual(entry, result);

            _storeBufferMock.Verify();
            _storeServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetObjectAsJson_GetsFromStore_IfNotAvailableInBuffer()
        {
            Entry entry = new Entry();

            _storeBufferMock.Setup(s => s.GetObjectFromBufferAsync<Entry>(_access, _storeKey)).Returns(Task.FromResult<Entry>(null)).Verifiable();
            _storeServiceMock.Setup(s => s.GetObjectFromJsonAsync<Entry>(_access, _storeKey)).Returns(Task.FromResult(entry)).Verifiable();

            var result = await _bufferedStoreService.GetObjectFromJsonAsync<Entry>(_access, _storeKey);

            Assert.AreEqual(entry, result);

            _storeBufferMock.Verify();
            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task GetObjectAsJson_GetsFromStoreIfNotAvailableInBuffer_AndPutsToBuffer()
        {
            Entry entry = new Entry();

            _storeBufferMock.Setup(s => s.GetObjectFromBufferAsync<Entry>(_access, _storeKey)).Returns(Task.FromResult<Entry>(null)).Verifiable();
            _storeServiceMock.Setup(s => s.GetObjectFromJsonAsync<Entry>(_access, _storeKey)).Returns(Task.FromResult(entry)).Verifiable();
            _storeBufferMock.Setup(s => s.PutObjectToBufferAsync<Entry>(_access, _storeKey, entry)).Returns(Task.FromResult(true)).Verifiable();

            var result = await _bufferedStoreService.GetObjectFromJsonAsync<Entry>(_access, _storeKey);

            Assert.AreEqual(entry, result);

            _storeBufferMock.Verify();
            _storeServiceMock.Verify();
        }

        [TestMethod]
        public async Task GetObjectAsStream_GetsFromBuffer()
        {
            Stream stream = new MemoryStream();

            _storeBufferMock.Setup(s => s.GetObjectAsStreamFromBufferAsync(_access, _storeKey)).Returns(Task.FromResult(stream)).Verifiable();

            var result = await _bufferedStoreService.GetObjectAsStreamAsync(_access, _storeKey);

            Assert.AreEqual(stream, result);

            _storeBufferMock.Verify();
            _storeServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GetObjectAsStream_GetsFromStore_IfNotAvailableInBuffer()
        {
            Stream stream = new MemoryStream();

            _storeBufferMock.Setup(s => s.GetObjectAsStreamFromBufferAsync(_access, _storeKey)).Returns(Task.FromResult<Stream>(null)).Verifiable();
            _storeServiceMock.Setup(s => s.GetObjectAsStreamAsync(_access, _storeKey)).Returns(Task.FromResult(stream)).Verifiable();

            var result = await _bufferedStoreService.GetObjectAsStreamAsync(_access, _storeKey);

            Assert.AreEqual(stream, result);

            _storeBufferMock.Verify();
            _storeServiceMock.Verify();
        }
    }
}
