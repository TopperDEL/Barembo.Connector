using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Services
{
    public class BookShelfService : IBookShelfService
    {
        IBookShelfStoreService _bookShelfStoreService;
        IBookShareStoreService _bookShareStoreService;
        IContributorStoreService _contributorStoreService;
        IStoreAccessService  _storeAccessService;

        public BookShelfService(IBookShelfStoreService bookShelfStoreService, IBookShareStoreService bookShareStoreService, IStoreAccessService storeAccessService, IContributorStoreService contributorStoreService)
        {
            _bookShelfStoreService = bookShelfStoreService;
            _bookShareStoreService = bookShareStoreService;
            _contributorStoreService = contributorStoreService;
            _storeAccessService = storeAccessService;
        }

        public async Task<bool> AddOwnBookToBookShelfAndSaveAsync(StoreAccess access, Book book, Contributor contributor)
        {
            try
            {
                var bookShelf = await _bookShelfStoreService.LoadAsync(access);

                var success = _bookShelfStoreService.AddBookToBookShelf(bookShelf, book.Id, bookShelf.OwnerName, access, AccessRights.Full, contributor.Id);

                if (success)
                {
                    return await _bookShelfStoreService.SaveAsync(access, bookShelf);
                }
                else
                    return false;
            }
            catch(NoBookShelfExistsException)
            {
                return false;
            }
        }

        public async Task<bool> AddSharedBookToBookShelfAndSaveAsync(StoreAccess access, BookShareReference bookShareReference)
        {
            try
            {
                var bookShare = await _bookShareStoreService.LoadBookShareAsync(bookShareReference);

                var bookShelf = await _bookShelfStoreService.LoadAsync(access);

                var success = _bookShelfStoreService.AddBookToBookShelf(bookShelf, bookShare.BookId, bookShare.OwnerName, bookShare.Access, bookShare.AccessRights, bookShare.ContributorId);

                if (success)
                {
                    return await _bookShelfStoreService.SaveAsync(access, bookShelf);
                }
                else
                    return false;
            }
            catch (NoBookShelfExistsException)
            {
                return false;
            }
            catch (BookShareNotFoundException)
            {
                return false;
            }
        }

        public async Task<BookShelf> CreateAndSaveBookShelfAsync(StoreAccess storeAccess, string ownerName)
        {
            BookShelf bookShelf = new BookShelf();
            bookShelf.OwnerName = ownerName;

            var success = await _bookShelfStoreService.SaveAsync(storeAccess, bookShelf);

            if (success)
                return bookShelf;
            else
                throw new BookShelfCouldNotBeSavedException();
        }

        public async Task<BookShelf> LoadBookShelfAsync(StoreAccess access)
        {
            return await _bookShelfStoreService.LoadAsync(access);
        }

        public async Task<BookShareReference> ShareBookAsync(StoreAccess access, BookReference bookReferenceToShare, string contributorName, AccessRights accessRights)
        {
            try
            {
                var bookShelf = await _bookShelfStoreService.LoadAsync(access);

                Contributor contributor = new Contributor();
                contributor.AccessRights = accessRights;
                contributor.Name = contributorName;
                var contributorSaved = await _contributorStoreService.SaveAsync(bookReferenceToShare, contributor);
                if (!contributorSaved)
                    throw new CouldNotShareBookException(CouldNotShareBookReason.CouldNotSaveContributor);

                BookShare bookShare = new BookShare();
                bookShare.Access = await _storeAccessService.ShareBookAccessAsync(access, bookReferenceToShare, contributor, accessRights);
                bookShare.AccessRights = accessRights;
                bookShare.BookId = bookReferenceToShare.BookId;
                bookShare.ContributorId = contributor.Id;
                bookShare.OwnerName = bookShelf.OwnerName;

                var reference = await _bookShareStoreService.SaveBookShareAsync(access, bookShare);

                return reference;
            }
            catch(NoBookShelfExistsException)
            {
                throw new CouldNotShareBookException(CouldNotShareBookReason.BookShelfNotFound);
            }
            catch (BookShareCouldNotBeSavedException)
            {
                throw new CouldNotShareBookException(CouldNotShareBookReason.BookShareCouldNotBeSaved);
            }
        }
    }
}
