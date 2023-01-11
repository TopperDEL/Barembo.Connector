using Barembo.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Barembo.Models;

namespace Barembo.Services
{
    public class BackgroundActionService : IBackgroundActionService, IDisposable
    {
        CancellationTokenSource _source;
        Task _uploadTask;
        readonly IStoreBuffer _storeBuffer;
        readonly IEntryService _entryService;

        public bool Processing
        {
            get
            {
                return _uploadTask != null && (_uploadTask.Status == TaskStatus.Running || _uploadTask.Status == TaskStatus.WaitingToRun || _uploadTask.Status == TaskStatus.WaitingForActivation);
            }
        }

        public BackgroundActionService(IStoreBuffer storeBuffer, IEntryService entryService)
        {
            _storeBuffer = storeBuffer;
            _entryService = entryService;
        }

        public void ProcessActionsInBackground()
        {
            if (_uploadTask == null || _uploadTask.Status == TaskStatus.RanToCompletion)
            {
                _source = new CancellationTokenSource();
                var uploadCancelToken = _source.Token;

                _uploadTask = Task.Run(() => DoProcessAsync(uploadCancelToken), uploadCancelToken);
            }
        }

        public void StopProcessingInBackground()
        {
            if (_source != null && _source.Token.CanBeCanceled)
            {
                _source.Cancel();
            }
        }

        private async Task DoProcessAsync(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    var actionToDo = await _storeBuffer.GetNextBackgroundAction();

                    if (actionToDo == null)
                        return; //Stop background processing

                    bool success = false;
                    switch(actionToDo.ActionType)
                    {
                        case Models.BackgroundActionTypes.AddAttachment:
                            success = await _entryService.AddAttachmentFromBackgroundActionAsync(actionToDo);
                            break;
                        case Models.BackgroundActionTypes.SetThumbnail:
                            success = await _entryService.SetThumbnailFromBackgroundActionAsync(actionToDo);
                            break;
                        default:
                            throw new NotImplementedException("Unknown action-type: " + actionToDo.ActionType.ToString());
                    }

                    if (success)
                    {
                        await _storeBuffer.RemoveBackgroundAction(actionToDo);
                    }
                }
            }
            catch
            {
                //That's ok, simply quit. The next run should fix it.
            }
        }

        public async Task<bool> AddAttachmentInBackgroundAsync(EntryReference entryReference, Attachment attachment, string filePath)
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "EntryReference", entryReference },
                    { "Attachment", attachment },
                    { "FilePath", filePath }
                };

                BackgroundAction addAttachmentAction = new BackgroundAction(BackgroundActionTypes.AddAttachment, parameters);

                await _storeBuffer.AddBackgroundAction(addAttachmentAction);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SetThumbnailInBackgroundAsync(EntryReference entryReference, Attachment attachment, string filePath)
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "EntryReference", entryReference },
                    { "Attachment", attachment },
                    { "FilePath", filePath }
                };

                BackgroundAction addAttachmentAction = new BackgroundAction(BackgroundActionTypes.SetThumbnail, parameters);

                await _storeBuffer.AddBackgroundAction(addAttachmentAction);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            StopProcessingInBackground();
            _source.Dispose();
        }
    }
}
