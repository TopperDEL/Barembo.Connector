﻿using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using Barembo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Moq;

namespace Barembo.Connector.Test.Services
{
    [TestClass]
    public class EntryServiceTest
    {
        EntryService _entryService;
        Moq.Mock<IEntryStoreService> _entryStoreServiceMock;
        Moq.Mock<IThumbnailGeneratorService> _thumbnailGeneratorService;
        Moq.Mock<IAttachmentStoreService> _attachmentStoreServiceMock;
        Moq.Mock<IAttachmentPreviewStoreService> _attachmentPreviewStoreServiceMock;
        Moq.Mock<IAttachmentPreviewGeneratorService> _attachmentPreviewGeneratorServiceMock;
        Moq.Mock<IFileAccessHelper> _fileAccessHelperMock;

        [TestInitialize]
        public void Init()
        {
            _entryStoreServiceMock = new Moq.Mock<IEntryStoreService>();
            _attachmentStoreServiceMock = new Moq.Mock<IAttachmentStoreService>();
            _attachmentPreviewStoreServiceMock = new Moq.Mock<IAttachmentPreviewStoreService>();
            _attachmentPreviewGeneratorServiceMock = new Moq.Mock<IAttachmentPreviewGeneratorService>();
            _thumbnailGeneratorService = new Moq.Mock<IThumbnailGeneratorService>();
            _fileAccessHelperMock = new Moq.Mock<IFileAccessHelper>();

            _entryService = new EntryService(_entryStoreServiceMock.Object, _attachmentStoreServiceMock.Object, _attachmentPreviewStoreServiceMock.Object, _attachmentPreviewGeneratorServiceMock.Object, _thumbnailGeneratorService.Object, _fileAccessHelperMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _entryService.Dispose();
        }

        [TestMethod]
        public void CreateEntry_Creates_Entry()
        {
            var result = _entryService.CreateEntry("header", "body");

            Assert.AreEqual("header", result.Header);
            Assert.AreEqual("body", result.Body);
        }

        [TestMethod]
        public void CreateEntryWithEmptyBody_Creates_EntryWithEmptyBody()
        {
            var result = _entryService.CreateEntry("header");

            Assert.AreEqual("header", result.Header);
            Assert.AreEqual("", result.Body);
        }

        [TestMethod]
        public async Task AddEntryToBook_AddsAndSaves_EntryInBook()
        {
            Entry entry = _entryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;

            _entryStoreServiceMock.Setup(s => s.SaveAsync(Moq.It.Is<EntryReference>(e => e.BookReference == bookReference && e.EntryId == entry.Id), entry))
                                  .Returns(Task.FromResult(true)).Verifiable();

            var result = await _entryService.AddEntryToBookAsync(bookReference, entry);

            Assert.AreEqual(bookReference, result.BookReference);
            Assert.AreEqual(entry.Id, result.EntryId);
            Assert.AreEqual(StoreKey.Entry(book.Id, entry.Id, contributor.Id).ToString(), result.EntryKey);

            _entryStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task AddEntryToBook_ThrowsError_IfEntryCouldNotBeSaved()
        {
            Entry entry = _entryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;

            _entryStoreServiceMock.Setup(s => s.SaveAsync(Moq.It.Is<EntryReference>(e => e.BookReference == bookReference && e.EntryId == entry.Id), entry))
                                  .Returns(Task.FromResult(false)).Verifiable();

            try
            {
                var result = await _entryService.AddEntryToBookAsync(bookReference, entry);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(EntryCouldNotBeSavedException));
            }

            _entryStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task AddEntryToBook_ThrowsError_IfActionNotAllowed()
        {
            Entry entry = _entryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;
            bookReference.AccessRights.CanAddEntries = false;

            try
            {
                var result = await _entryService.AddEntryToBookAsync(bookReference, entry);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ActionNotAllowedException));
            }

            _entryStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task SaveEntry_Saves_Entry()
        {
            Entry entry = _entryService.CreateEntry("test");
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = new BookReference();

            _entryStoreServiceMock.Setup(s => s.SaveAsync(entryReference, entry))
                                  .Returns(Task.FromResult(true)).Verifiable();

            var result = await _entryService.SaveEntryAsync(entryReference, entry);

            Assert.IsTrue(result);

            _entryStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task SaveEntry_RaisesError_IfEntryCouldNotBeSaved()
        {
            Entry entry = _entryService.CreateEntry("test");
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = new BookReference();

            _entryStoreServiceMock.Setup(s => s.SaveAsync(entryReference, entry))
                                  .Returns(Task.FromResult(false)).Verifiable();

            var result = await _entryService.SaveEntryAsync(entryReference, entry);

            Assert.IsFalse(result);

            _entryStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task SaveEntry_RaisesError_IfEditOwnIsNotAllowed()
        {
            Entry entry = _entryService.CreateEntry("test");
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = new BookReference();
            entryReference.BookReference.AccessRights.CanEditOwnEntries = false;

            try
            {
                await _entryService.SaveEntryAsync(entryReference, entry);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ActionNotAllowedException));
            }

            _entryStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task SaveEntry_RaisesError_IfEditForeignIsNotAllowed()
        {
            Entry entry = _entryService.CreateEntry("test");
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = new BookReference();
            entryReference.BookReference.BookShareReference = new BookShareReference();
            entryReference.BookReference.AccessRights.CanEditForeignEntries = false;

            try
            {
                await _entryService.SaveEntryAsync(entryReference, entry);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ActionNotAllowedException));
            }

            _entryStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task DeleteEntry_Deletes_Entry()
        {
            Entry entry = _entryService.CreateEntry("test");
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = new BookReference();

            _entryStoreServiceMock.Setup(s => s.LoadAsync(entryReference))
                                  .Returns(Task.FromResult(entry)).Verifiable();
            _entryStoreServiceMock.Setup(s => s.DeleteAsync(entryReference))
                                  .Returns(Task.FromResult(true)).Verifiable();

            var result = await _entryService.DeleteEntryAsync(entryReference);

            Assert.IsTrue(result);

            _entryStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task DeleteEntry_ReturnsFalse_IfEntryCouldNotBeDeleted()
        {
            Entry entry = _entryService.CreateEntry("test");
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = new BookReference();

            _entryStoreServiceMock.Setup(s => s.LoadAsync(entryReference))
                                  .Returns(Task.FromResult(entry)).Verifiable();
            _entryStoreServiceMock.Setup(s => s.DeleteAsync(entryReference))
                                  .Returns(Task.FromResult(false)).Verifiable();

            var result = await _entryService.DeleteEntryAsync(entryReference);

            Assert.IsFalse(result);

            _entryStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task DeleteEntry_Deletes_EntryWithSingleAttachment()
        {
            Entry entry = _entryService.CreateEntry("test");
            var attachment = new Attachment { Id = "attachment1" };
            entry.Attachments.Add(attachment);
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = new BookReference();

            _entryStoreServiceMock.Setup(s => s.LoadAsync(entryReference))
                                  .Returns(Task.FromResult(entry)).Verifiable();
            _attachmentStoreServiceMock.Setup(s => s.DeleteAsync(entryReference, attachment)).Returns(Task.FromResult(true)).Verifiable();
            _attachmentPreviewStoreServiceMock.Setup(s => s.DeleteAsync(entryReference, attachment)).Returns(Task.FromResult(true)).Verifiable();
            _entryStoreServiceMock.Setup(s => s.DeleteAsync(entryReference))
                                  .Returns(Task.FromResult(true)).Verifiable();

            var result = await _entryService.DeleteEntryAsync(entryReference);

            Assert.IsTrue(result);

            _entryStoreServiceMock.Verify();
            _attachmentStoreServiceMock.Verify();
            _attachmentPreviewStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task DeleteEntry_Deletes_EntryWithMultipleAttachment()
        {
            Entry entry = _entryService.CreateEntry("test");
            var attachment1 = new Attachment { Id = "attachment1" };
            var attachment2 = new Attachment { Id = "attachment2" };
            entry.Attachments.Add(attachment1);
            entry.Attachments.Add(attachment2);
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = new BookReference();

            _entryStoreServiceMock.Setup(s => s.LoadAsync(entryReference))
                                  .Returns(Task.FromResult(entry)).Verifiable();
            _attachmentStoreServiceMock.Setup(s => s.DeleteAsync(entryReference, attachment1)).Returns(Task.FromResult(true)).Verifiable();
            _attachmentPreviewStoreServiceMock.Setup(s => s.DeleteAsync(entryReference, attachment1)).Returns(Task.FromResult(true)).Verifiable();
            _attachmentStoreServiceMock.Setup(s => s.DeleteAsync(entryReference, attachment2)).Returns(Task.FromResult(true)).Verifiable();
            _attachmentPreviewStoreServiceMock.Setup(s => s.DeleteAsync(entryReference, attachment2)).Returns(Task.FromResult(true)).Verifiable();
            _entryStoreServiceMock.Setup(s => s.DeleteAsync(entryReference))
                                  .Returns(Task.FromResult(true)).Verifiable();

            var result = await _entryService.DeleteEntryAsync(entryReference);

            Assert.IsTrue(result);

            _entryStoreServiceMock.Verify();
            _attachmentStoreServiceMock.Verify();
            _attachmentPreviewStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task DeleteEntry_ReturnsFalse_IfAttachmentCouldNotBeDeleted()
        {
            Entry entry = _entryService.CreateEntry("test");
            var attachment1 = new Attachment { Id = "attachment1" };
            var attachment2 = new Attachment { Id = "attachment2" };
            entry.Attachments.Add(attachment1);
            entry.Attachments.Add(attachment2);
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = new BookReference();

            _entryStoreServiceMock.Setup(s => s.LoadAsync(entryReference))
                                  .Returns(Task.FromResult(entry)).Verifiable();
            _attachmentStoreServiceMock.Setup(s => s.DeleteAsync(entryReference, attachment1)).Returns(Task.FromResult(true)).Verifiable();
            _attachmentPreviewStoreServiceMock.Setup(s => s.DeleteAsync(entryReference, attachment1)).Returns(Task.FromResult(true)).Verifiable();
            _attachmentStoreServiceMock.Setup(s => s.DeleteAsync(entryReference, attachment2)).Returns(Task.FromResult(false)).Verifiable();
            
            var result = await _entryService.DeleteEntryAsync(entryReference);

            Assert.IsFalse(result);

            _entryStoreServiceMock.Verify();
            _attachmentStoreServiceMock.Verify();
            _attachmentPreviewStoreServiceMock.Verify();
            _attachmentPreviewStoreServiceMock.VerifyNoOtherCalls();
            _entryStoreServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task DeleteEntry_ReturnsFalse_IfAttachmentPreviewCouldNotBeDeleted()
        {
            Entry entry = _entryService.CreateEntry("test");
            var attachment1 = new Attachment { Id = "attachment1" };
            var attachment2 = new Attachment { Id = "attachment2" };
            entry.Attachments.Add(attachment1);
            entry.Attachments.Add(attachment2);
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = new BookReference();

            _entryStoreServiceMock.Setup(s => s.LoadAsync(entryReference))
                                  .Returns(Task.FromResult(entry)).Verifiable();
            _attachmentStoreServiceMock.Setup(s => s.DeleteAsync(entryReference, attachment1)).Returns(Task.FromResult(true)).Verifiable();
            _attachmentPreviewStoreServiceMock.Setup(s => s.DeleteAsync(entryReference, attachment1)).Returns(Task.FromResult(false)).Verifiable();

            var result = await _entryService.DeleteEntryAsync(entryReference);

            Assert.IsFalse(result);

            _entryStoreServiceMock.Verify();
            _attachmentStoreServiceMock.Verify();
            _attachmentStoreServiceMock.VerifyNoOtherCalls();
            _attachmentPreviewStoreServiceMock.Verify();
            _attachmentPreviewStoreServiceMock.VerifyNoOtherCalls();
            _entryStoreServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task LoadEntryASAP_Loads_EntryFromQueue()
        {
            Entry entry = _entryService.CreateEntry("test");
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = new BookReference();

            _entryStoreServiceMock.Setup(s => s.LoadAsync(entryReference))
                                  .Returns(Task.FromResult(entry)).Verifiable();

            bool loaded = false;
            bool loadingFailed = false;
            Entry loadedEntry = null;
            _entryService.LoadEntryAsSoonAsPossible(entryReference, (entry) => { loaded = true; loadedEntry = entry; }, () => loadingFailed = true);

            await Task.Delay(300); //Might need more time on slower machines. We need to wait for the underlying Stack-Processing-Task to
                                   //fetch the work to do.

            Assert.IsTrue(loaded);
            Assert.IsFalse(loadingFailed);
            Assert.AreEqual(entry, loadedEntry);

            _entryStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task LoadEntry_Loads_Entry()
        {
            Entry entry = _entryService.CreateEntry("test");
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = new BookReference();

            _entryStoreServiceMock.Setup(s => s.LoadAsync(entryReference))
                                  .Returns(Task.FromResult(entry)).Verifiable();

            var result = await _entryService.LoadEntryAsync(entryReference);

            Assert.AreEqual(entry, result);

            _entryStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task LoadEntry_RaisesError_IfEntryCouldNotBeFound()
        {
            Entry entry = _entryService.CreateEntry("test");
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = new BookReference();

            _entryStoreServiceMock.Setup(s => s.LoadAsync(entryReference))
                                  .Throws(new EntryNotExistsException()).Verifiable();

            try
            {
                var result = await _entryService.LoadEntryAsync(entryReference);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(EntryNotExistsException));
            }

            _entryStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task LoadEntry_RaisesError_IfNotAllowedToReadForeign()
        {
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = new BookReference();
            entryReference.BookReference.AccessRights.CanReadForeignEntries = false;
            entryReference.BookReference.ContributorId = Guid.NewGuid().ToString();
            entryReference.BookReference.BookShareReference = new BookShareReference();

            try
            {
                var result = await _entryService.LoadEntryAsync(entryReference);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ActionNotAllowedException));
            }

            _entryStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task LoadEntry_RaisesError_IfNotAllowedToEntries()
        {
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = new BookReference();
            entryReference.BookReference.AccessRights.CanReadEntries = false;
            entryReference.BookReference.ContributorId = Guid.NewGuid().ToString();
            entryReference.BookReference.BookShareReference = new BookShareReference();

            try
            {
                var result = await _entryService.LoadEntryAsync(entryReference);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ActionNotAllowedException));
            }

            _entryStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task ListEntries_List_AllEntries()
        {
            BookReference bookReference = new BookReference();
            List<EntryReference> entries = new List<EntryReference>();

            _entryStoreServiceMock.Setup(s => s.ListAsync(bookReference)).Returns(Task.FromResult(entries as IEnumerable<EntryReference>)).Verifiable();

            var result = await _entryService.ListEntriesAsync(bookReference);

            Assert.AreEqual(entries, result);

            _entryStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task ListEntries_ListOnlyOwnEntries_IfOthersAreNotAllowed()
        {
            BookReference bookReference = new BookReference();
            bookReference.AccessRights.CanReadForeignEntries = false;
            bookReference.BookShareReference = new BookShareReference();
            bookReference.ContributorId = Guid.NewGuid().ToString();
            List<EntryReference> entries = new List<EntryReference>();
            entries.Add(new EntryReference() { EntryId = "notMine" });
            entries.Add(new EntryReference() { EntryId = bookReference.ContributorId });

            _entryStoreServiceMock.Setup(s => s.ListAsync(bookReference)).Returns(Task.FromResult(entries as IEnumerable<EntryReference>)).Verifiable();

            var result = await _entryService.ListEntriesAsync(bookReference);

            Assert.AreEqual(1, result.Count());

            _entryStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task ListEntries_RaisesError_IfActionNotAllowed()
        {
            BookReference bookReference = new BookReference();
            bookReference.AccessRights.CanReadEntries = false;
            bookReference.BookShareReference = new BookShareReference();
            bookReference.ContributorId = Guid.NewGuid().ToString();
            List<EntryReference> entries = new List<EntryReference>();
            entries.Add(new EntryReference() { EntryId = "notMine" });
            entries.Add(new EntryReference() { EntryId = bookReference.ContributorId });

            _entryStoreServiceMock.Setup(s => s.ListAsync(bookReference)).Returns(Task.FromResult(entries as IEnumerable<EntryReference>)).Verifiable();

            try
            {
                await _entryService.ListEntriesAsync(bookReference);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ActionNotAllowedException));
            }

            _entryStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task AddAttachment_AddsAndSaves_AttachmentToEntry()
        {
            Entry entry = _entryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = bookReference;
            entryReference.EntryId = entry.Id;
            Attachment attachment = new Attachment();
            MemoryStream stream = new MemoryStream();
            AttachmentPreview preview = new AttachmentPreview();

            _entryStoreServiceMock.Setup(s => s.SaveAsync(entryReference, Moq.It.Is<Entry>(e => e.Id == entry.Id && e.Attachments.Count == 1))) //Attachment has to be added before save
                                  .Returns(Task.FromResult(true)).Verifiable();
            _attachmentStoreServiceMock.Setup(s => s.SaveFromStreamAsync(entryReference, attachment, stream, "filepath")).Returns(Task.FromResult(true)).Verifiable();
            _attachmentPreviewStoreServiceMock.Setup(s => s.SaveAsync(entryReference, attachment, preview)).Returns(Task.FromResult(true)).Verifiable();

            var result = await _entryService.AddAttachmentAsync(entryReference, entry, attachment, stream, "filepath");

            Assert.IsTrue(result);
            Assert.AreEqual(entry.Attachments[0], attachment);

            _entryStoreServiceMock.Verify();
            _attachmentStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task AddAttachment_AddsAndSaves_AttachmentPreviewToEntry()
        {
            Entry entry = _entryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = bookReference;
            entryReference.EntryId = entry.Id;
            Attachment attachment = new Attachment();
            MemoryStream stream = new MemoryStream();
            AttachmentPreview preview = new AttachmentPreview();

            _entryStoreServiceMock.Setup(s => s.SaveAsync(entryReference, Moq.It.Is<Entry>(e => e.Id == entry.Id && e.Attachments.Count == 1))) //Attachment has to be added before save
                                  .Returns(Task.FromResult(true)).Verifiable();
            _attachmentStoreServiceMock.Setup(s => s.SaveFromStreamAsync(entryReference, attachment, stream, "filepath")).Returns(Task.FromResult(true)).Verifiable();
            _attachmentPreviewStoreServiceMock.Setup(s => s.SaveAsync(entryReference, attachment, preview)).Returns(Task.FromResult(true)).Verifiable();

            var result = await _entryService.AddAttachmentAsync(entryReference, entry, attachment, stream, "filepath");

            Assert.IsTrue(result);
            Assert.AreEqual(entry.Attachments[0], attachment);

            _entryStoreServiceMock.Verify();
            _attachmentStoreServiceMock.Verify();
            _attachmentPreviewStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task SetThumbnail_SetsThumbnailForImage_AndSavesEntry()
        {
            Entry entry = _entryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = bookReference;
            entryReference.EntryId = entry.Id;
            Attachment attachment = new Attachment();
            attachment.Type = AttachmentType.Image;
            MemoryStream stream = new MemoryStream();
            string thumbnail = "ABCDE";

            _entryStoreServiceMock.Setup(s => s.SaveAsync(entryReference, Moq.It.Is<Entry>(e => e.Id == entry.Id && e.ThumbnailBase64 == thumbnail)))
                                  .Returns(Task.FromResult(true)).Verifiable();
            _thumbnailGeneratorService.Setup(s => s.GenerateThumbnailBase64FromImageAsync(stream)).Returns(Task.FromResult(thumbnail));

            var result = await _entryService.SetThumbnailAsync(entryReference, entry, attachment, stream, "folder/file.mp4");

            Assert.IsTrue(result);
            Assert.AreEqual(thumbnail, entry.ThumbnailBase64);

            _entryStoreServiceMock.Verify();
            _attachmentStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task SetThumbnail_SetsThumbnailForVideo_AndSavesEntry()
        {
            Entry entry = _entryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = bookReference;
            entryReference.EntryId = entry.Id;
            Attachment attachment = new Attachment();
            attachment.Type = AttachmentType.Video;
            MemoryStream stream = new MemoryStream();
            string thumbnail = "ABCDE";

            _entryStoreServiceMock.Setup(s => s.SaveAsync(entryReference, Moq.It.Is<Entry>(e => e.Id == entry.Id && e.ThumbnailBase64 == thumbnail)))
                                  .Returns(Task.FromResult(true)).Verifiable();
            _thumbnailGeneratorService.Setup(s => s.GenerateThumbnailBase64FromVideoAsync(stream, 0f, "folder/file.mp4")).Returns(Task.FromResult(thumbnail));

            var result = await _entryService.SetThumbnailAsync(entryReference, entry, attachment, stream, "folder/file.mp4");

            Assert.IsTrue(result);
            Assert.AreEqual(thumbnail, entry.ThumbnailBase64);

            _entryStoreServiceMock.Verify();
            _attachmentStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task AddAttachment_ReturnsFalse_IfAttachmentCouldNotBeSaved()
        {
            Entry entry = _entryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = bookReference;
            entryReference.EntryId = entry.Id;
            Attachment attachment = new Attachment();
            MemoryStream stream = new MemoryStream();

            _attachmentStoreServiceMock.Setup(s => s.SaveFromStreamAsync(entryReference, attachment, stream, "filepath")).Returns(Task.FromResult(false)).Verifiable();

            var result = await _entryService.AddAttachmentAsync(entryReference, entry, attachment, stream, "filepath");

            Assert.IsFalse(result);
            Assert.AreEqual(0, entry.Attachments.Count);

            _entryStoreServiceMock.Verify();
            _attachmentStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task AddAttachment_ReturnsFalse_IfEntryCouldNotBeSaved()
        {
            Entry entry = _entryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = bookReference;
            entryReference.EntryId = entry.Id;
            Attachment attachment = new Attachment();
            MemoryStream stream = new MemoryStream();
            AttachmentPreview preview = new AttachmentPreview();

            _entryStoreServiceMock.Setup(s => s.SaveAsync(entryReference, Moq.It.Is<Entry>(e => e.Id == entry.Id && e.Attachments.Count == 1))) //Attachment has to be added before save
                                  .Returns(Task.FromResult(false)).Verifiable();
            _attachmentStoreServiceMock.Setup(s => s.SaveFromStreamAsync(entryReference, attachment, stream, "filepath")).Returns(Task.FromResult(true)).Verifiable();
            _attachmentPreviewStoreServiceMock.Setup(s => s.SaveAsync(entryReference, attachment, preview)).Returns(Task.FromResult(true)).Verifiable();

            var result = await _entryService.AddAttachmentAsync(entryReference, entry, attachment, stream, "filepath");

            Assert.IsFalse(result);
            Assert.AreEqual(0, entry.Attachments.Count);

            _entryStoreServiceMock.Verify();
            _attachmentStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task AddAttachment_ReturnsFalse_IfPreviewCouldNotBeSaved()
        {
            Entry entry = _entryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = bookReference;
            entryReference.EntryId = entry.Id;
            Attachment attachment = new Attachment();
            MemoryStream stream = new MemoryStream();
            AttachmentPreview preview = new AttachmentPreview();

            _entryStoreServiceMock.Setup(s => s.SaveAsync(entryReference, Moq.It.Is<Entry>(e => e.Id == entry.Id && e.Attachments.Count == 1))) //Attachment has to be added before save
                                  .Returns(Task.FromResult(false));
            _attachmentStoreServiceMock.Setup(s => s.SaveFromStreamAsync(entryReference, attachment, stream, "filepath")).Returns(Task.FromResult(true)).Verifiable();
            _attachmentPreviewStoreServiceMock.Setup(s => s.SaveAsync(entryReference, attachment, preview)).Returns(Task.FromResult(false)).Verifiable();

            var result = await _entryService.AddAttachmentAsync(entryReference, entry, attachment, stream, "filepath");

            Assert.IsFalse(result);
            Assert.AreEqual(0, entry.Attachments.Count);

            _entryStoreServiceMock.Verify();
            _attachmentStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task LoadAttachment_Loads_Attachment()
        {
            Entry entry = _entryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = bookReference;
            entryReference.EntryId = entry.Id;
            Attachment attachment = new Attachment();
            MemoryStream stream = new MemoryStream();

            _attachmentStoreServiceMock.Setup(s => s.LoadAsStreamAsync(entryReference, attachment)).Returns(Task.FromResult(stream as Stream)).Verifiable();

            var result = await _entryService.LoadAttachmentAsync(entryReference, attachment);

            Assert.AreEqual(stream, result);

            _entryStoreServiceMock.Verify();
            _attachmentStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task LoadAttachment_RaisesError_IfAttachmentDoesNotExist()
        {
            Entry entry = _entryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = bookReference;
            entryReference.EntryId = entry.Id;
            Attachment attachment = new Attachment();
            MemoryStream stream = new MemoryStream();

            _attachmentStoreServiceMock.Setup(s => s.LoadAsStreamAsync(entryReference, attachment)).Throws(new AttachmentNotExistsException()).Verifiable();

            try
            {
                var result = await _entryService.LoadAttachmentAsync(entryReference, attachment);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(AttachmentNotExistsException));
            }

            _entryStoreServiceMock.Verify();
            _attachmentStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task LoadAttachmentPreview_Loads_AttachmentPreview()
        {
            Entry entry = _entryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = bookReference;
            entryReference.EntryId = entry.Id;
            Attachment attachment = new Attachment();
            AttachmentPreview preview = new AttachmentPreview();

            _attachmentPreviewStoreServiceMock.Setup(s => s.LoadAsync(entryReference, attachment)).Returns(Task.FromResult(preview)).Verifiable();

            var result = await _entryService.LoadAttachmentPreviewAsync(entryReference, attachment);

            Assert.AreEqual(preview, result);

            _entryStoreServiceMock.Verify();
            _attachmentPreviewStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task LoadAttachmentPreview_RaisesError_IfAttachmentPreviewDoesNotExist()
        {
            Entry entry = _entryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = bookReference;
            entryReference.EntryId = entry.Id;
            Attachment attachment = new Attachment();

            _attachmentPreviewStoreServiceMock.Setup(s => s.LoadAsync(entryReference, attachment)).Throws(new AttachmentPreviewNotExistsException()).Verifiable();

            try
            {
                var result = await _entryService.LoadAttachmentPreviewAsync(entryReference, attachment);
                Assert.IsTrue(false);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(AttachmentPreviewNotExistsException));
            }

            _entryStoreServiceMock.Verify();
            _attachmentPreviewStoreServiceMock.Verify();
        }

        [TestMethod]
        public async Task AddAttachmentFromBackGroundAction_AddsAndSaves_AttachmentToEntry()
        {
            Entry entry = _entryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = bookReference;
            entryReference.EntryId = entry.Id;
            Attachment attachment = new Attachment();
            Stream stream = new MemoryStream();
            AttachmentPreview preview = new AttachmentPreview();

            var parameters = new Dictionary<string, object>();
            parameters.Add("EntryReference", entryReference);
            parameters.Add("Attachment", attachment);
            parameters.Add("FilePath", "filepath");
            BackgroundAction backgroundAction = new BackgroundAction(BackgroundActionTypes.AddAttachment, parameters);

            _fileAccessHelperMock.Setup(s => s.OpenFileAsync("filepath")).Returns(Task.FromResult(stream)).Verifiable();
            _entryStoreServiceMock.Setup(s => s.LoadAsync(It.IsAny<EntryReference>(), true)).Returns(Task.FromResult(entry)).Verifiable();
            _entryStoreServiceMock.Setup(s => s.SaveAsync(It.IsAny<EntryReference>(), Moq.It.Is<Entry>(e => e.Id == entry.Id && e.Attachments.Count == 1))) //Attachment has to be added before save
                                  .Returns(Task.FromResult(true)).Verifiable();
            _attachmentStoreServiceMock.Setup(s => s.SaveFromStreamAsync(It.IsAny<EntryReference>(), It.IsAny<Attachment>(), stream, "filepath")).Returns(Task.FromResult(true)).Verifiable();
            _attachmentPreviewStoreServiceMock.Setup(s => s.SaveAsync(It.IsAny<EntryReference>(), It.IsAny<Attachment>(), preview)).Returns(Task.FromResult(true)).Verifiable();

            var result = await _entryService.AddAttachmentFromBackgroundActionAsync(backgroundAction);

            Assert.IsTrue(result);
            Assert.AreEqual(entry.Attachments[0].FileName, attachment.FileName);

            _entryStoreServiceMock.Verify();
            _attachmentStoreServiceMock.Verify();
            _fileAccessHelperMock.Verify();
        }

        [TestMethod]
        public async Task SetThumbnailFromBackGroundAction_AddsAndSaves_AttachmentToEntry()
        {
            Entry entry = _entryService.CreateEntry("test");
            Contributor contributor = new Contributor();
            Book book = new Book();
            BookReference bookReference = new BookReference();
            bookReference.BookId = book.Id;
            bookReference.ContributorId = contributor.Id;
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = bookReference;
            entryReference.EntryId = entry.Id;
            Stream stream = new MemoryStream();
            AttachmentPreview preview = new AttachmentPreview();
            Attachment attachment = new Attachment();
            attachment.Type = AttachmentType.Image;
            string thumbnail = "ABCDE";

            var parameters = new Dictionary<string, object>();
            parameters.Add("EntryReference", entryReference);
            parameters.Add("Attachment", attachment);
            parameters.Add("FilePath", "filepath");
            BackgroundAction backgroundAction = new BackgroundAction(BackgroundActionTypes.SetThumbnail, parameters);

            _thumbnailGeneratorService.Setup(s => s.GenerateThumbnailBase64FromImageAsync(stream)).Returns(Task.FromResult(thumbnail));
            _fileAccessHelperMock.Setup(s => s.OpenFileAsync("filepath")).Returns(Task.FromResult(stream)).Verifiable();
            _entryStoreServiceMock.Setup(s => s.LoadAsync(It.IsAny<EntryReference>(), true)).Returns(Task.FromResult(entry)).Verifiable();
            _entryStoreServiceMock.Setup(s => s.SaveAsync(It.IsAny<EntryReference>(), Moq.It.Is<Entry>(e => e.Id == entry.Id && e.Attachments.Count == 0)))
                                  .Returns(Task.FromResult(true)).Verifiable();

            var result = await _entryService.SetThumbnailFromBackgroundActionAsync(backgroundAction);

            Assert.IsTrue(result);
            Assert.AreEqual("ABCDE", entry.ThumbnailBase64);

            _entryStoreServiceMock.Verify();
            _attachmentStoreServiceMock.Verify();
            _fileAccessHelperMock.Verify();
            _thumbnailGeneratorService.Verify();
            _attachmentPreviewStoreServiceMock.Verify();
        }
    }
}
