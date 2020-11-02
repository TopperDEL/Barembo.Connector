using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Barembo.Services
{
    public class BookShelfService : IBookShelfService
    {
        readonly IBookShelfStoreService _bookShelfStoreService;
        readonly IBookStoreService _bookStoreService;
        readonly IBookShareStoreService _bookShareStoreService;
        readonly IContributorStoreService _contributorStoreService;
        readonly IStoreAccessService _storeAccessService;

        public BookShelfService(IBookShelfStoreService bookShelfStoreService, IBookStoreService bookStoreService, IBookShareStoreService bookShareStoreService, IStoreAccessService storeAccessService, IContributorStoreService contributorStoreService)
        {
            _bookShelfStoreService = bookShelfStoreService;
            _bookStoreService = bookStoreService;
            _bookShareStoreService = bookShareStoreService;
            _contributorStoreService = contributorStoreService;
            _storeAccessService = storeAccessService;
        }

        public async Task<bool> AddOwnBookToBookShelfAndSaveAsync(StoreAccess access, Book book, Contributor contributor)
        {
            //First: load the BookShelf and add the new Book. Then we have a BookReference
            //we can use to save the book itself. If everything went ok, save the BookShelf.
            try
            {
                var bookShelf = await _bookShelfStoreService.LoadAsync(access);

                var success = _bookShelfStoreService.AddBookToBookShelf(bookShelf, book.Id, bookShelf.OwnerName, access, AccessRights.Full, contributor.Id);

                if (success)
                {
                    var bookReference = bookShelf.Content.Where(r => r.BookId == book.Id).FirstOrDefault();
                    if (bookReference == null)
                        return false;
                    else
                    {
                        var bookSaved = await _bookStoreService.SaveAsync(bookReference, book);
                        if (!bookSaved)
                            return false;
                        else
                        {
                            var contributorSaved = await _contributorStoreService.SaveAsync(bookReference, contributor);
                            if (!contributorSaved)
                                return false;
                            else
                            {
                                return await _bookShelfStoreService.SaveAsync(access, bookShelf);
                            }
                        }
                    }
                }
                else
                    return false;
            }
            catch (NoBookShelfExistsException)
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

                var success = _bookShelfStoreService.AddBookToBookShelf(bookShelf, bookShare.BookId, bookShare.OwnerName, bookShare.Access, bookShare.AccessRights, bookShare.ContributorId, bookShareReference);

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

        public async Task<IEnumerable<BookShareReference>> ListBookSharesAsync(StoreAccess access, BookReference bookReference)
        {
            return await _bookShareStoreService.ListBookSharesAsync(access, bookReference);
        }

        public async Task<BookShelf> LoadBookShelfAsync(StoreAccess access)
        {
            return await _bookShelfStoreService.LoadAsync(access);
        }

        public async Task RefreshBookAccessAsync(BookReference bookReference)
        {
            if (bookReference.BookShareReference != null)
            {
                BookShare bookShare = await _bookShareStoreService.LoadBookShareAsync(bookReference.BookShareReference);
                bookReference.AccessGrant = bookShare.Access.AccessGrant;
                bookReference.AccessRights = bookShare.AccessRights;
            }
        }

        public async Task<BookShareReference> ShareBookAsync(StoreAccess access, BookReference bookReferenceToShare, string contributorName, AccessRights accessRights)
        {
            if (!bookReferenceToShare.AccessRights.CanShareBook)
                throw new ActionNotAllowedException();

            try
            {
                var bookShelf = await _bookShelfStoreService.LoadAsync(access);

                Contributor contributor = new Contributor();
                contributor.Name = contributorName;
                var contributorSaved = await _contributorStoreService.SaveAsync(bookReferenceToShare, contributor);
                if (!contributorSaved)
                    throw new CouldNotShareBookException(CouldNotShareBookReason.CouldNotSaveContributor);

                BookShare bookShare = new BookShare();
                bookShare.Access = _storeAccessService.ShareBookAccess(access, bookReferenceToShare, contributor, accessRights);
                bookShare.AccessRights = accessRights;
                bookShare.BookId = bookReferenceToShare.BookId;
                bookShare.ContributorId = contributor.Id;
                bookShare.OwnerName = bookShelf.OwnerName;

                var reference = await _bookShareStoreService.SaveBookShareAsync(access, bookShare);

                return reference;
            }
            catch (NoBookShelfExistsException)
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
