using Barembo.Interfaces;
using Barembo.Models;
using Barembo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Connector.Test.Services
{
    [TestClass]
    public class BackgroundActionServiceTest
    {
        IBackgroundActionService _backgroundActionService;
        Moq.Mock<IStoreBuffer> _storeBufferMock;
        Moq.Mock<IEntryService> _entryServiceMock;

        [TestInitialize]
        public void Init()
        {
            _storeBufferMock = new Moq.Mock<IStoreBuffer>();
            _entryServiceMock = new Moq.Mock<IEntryService>();

            _backgroundActionService = new BackgroundActionService(_storeBufferMock.Object, _entryServiceMock.Object);
        }

        [TestMethod]
        public async Task ProcessInBackground_AddsAttachmentInBackground()
        {
            BackgroundAction actionToDo = new BackgroundAction();
            actionToDo.ActionType = BackgroundActionTypes.AddAttachment;

            _storeBufferMock.SetupSequence(s => s.GetNextBackgroundAction()).Returns(Task.FromResult(actionToDo)).Returns(Task.FromResult(null as BackgroundAction));
            _storeBufferMock.Setup(s => s.RemoveBackgroundAction(actionToDo)).Returns(Task.FromResult(true)).Verifiable();
            _entryServiceMock.Setup(e => e.AddAttachmentFromBackgroundActionAsync(actionToDo)).Returns(Task.FromResult(true)).Verifiable();

            _backgroundActionService.ProcessActionsInBackground();
            while (_backgroundActionService.Processing)
                await Task.Delay(100);

            _storeBufferMock.Verify();
            _entryServiceMock.Verify();
        }

        [TestMethod]
        public async Task ProcessInBackground_SetsThumbnailInBackground()
        {
            BackgroundAction actionToDo = new BackgroundAction();
            actionToDo.ActionType = BackgroundActionTypes.SetThumbnail;

            _storeBufferMock.SetupSequence(s => s.GetNextBackgroundAction()).Returns(Task.FromResult(actionToDo)).Returns(Task.FromResult(null as BackgroundAction));
            _storeBufferMock.Setup(s => s.RemoveBackgroundAction(actionToDo)).Returns(Task.FromResult(true)).Verifiable();
            _entryServiceMock.Setup(e => e.SetThumbnailFromBackgroundActionAsync(actionToDo)).Returns(Task.FromResult(true)).Verifiable();

            _backgroundActionService.ProcessActionsInBackground();
            while (_backgroundActionService.Processing)
                await Task.Delay(100);

            _storeBufferMock.Verify();
            _entryServiceMock.Verify();
        }

        [TestMethod]
        public async Task AddAttachment_Creates_BackgroundAction()
        {
            EntryReference entryReference = new EntryReference();
            entryReference.EntryId = "myEntry";
            Attachment attachment = new Attachment();
            attachment.FileName = "myPic.jpg";

            _storeBufferMock.Setup(s => s.AddBackgroundAction(It.Is<BackgroundAction>(b=>b.ActionType == BackgroundActionTypes.AddAttachment &&
                                                                                         b.ParameterDictionaryAsJson != null
                                                                                         ))).Verifiable();
            var result = await _backgroundActionService.AddAttachmentInBackgroundAsync(entryReference, attachment, "filepath");

            Assert.IsTrue(result);
            _storeBufferMock.Verify();
        }

        [TestMethod]
        public async Task SetThumbnail_SetsThumbnailForImage_AndSavesEntry()
        {
            EntryReference entryReference = new EntryReference();
            entryReference.EntryId = "myEntry";
            Attachment attachment = new Attachment();
            attachment.FileName = "myPic.jpg";

            _storeBufferMock.Setup(s => s.AddBackgroundAction(It.Is<BackgroundAction>(b => b.ActionType == BackgroundActionTypes.SetThumbnail &&
                                                                                         b.ParameterDictionaryAsJson != null
                                                                                         ))).Verifiable();
            var result = await _backgroundActionService.SetThumbnailInBackgroundAsync(entryReference, attachment, "filepath");

            Assert.IsTrue(result);
            _storeBufferMock.Verify();
        }
    }
}
