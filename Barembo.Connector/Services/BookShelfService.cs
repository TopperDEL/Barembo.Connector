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
        IStoreAccessService  _storeAccessService;

        public BookShelfService(IBookShelfStoreService bookShelfStoreService, IBookShareStoreService bookShareStoreService, IStoreAccessService storeAccessService)
        {
            _bookShelfStoreService = bookShelfStoreService;
            _bookShareStoreService = bookShareStoreService;
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

        public async Task<BookShareReference> ShareBookAsync(StoreAccess access, Book bookToShare, Contributor contributor, AccessRights accessRights)
        {
            var bookShelf = await _bookShelfStoreService.LoadAsync(access);

            BookShare bookShare = new BookShare();
            bookShare.Access = await _storeAccessService.ShareBookAccessAsync(access, bookToShare, contributor, accessRights);
            bookShare.AccessRights = accessRights;
            bookShare.BookId = bookToShare.Id;
            bookShare.ContributorId = contributor.Id;
            bookShare.OwnerName = bookShelf.OwnerName;

            var reference = await _bookShareStoreService.SaveBookShareAsync(access, bookShare);

            return reference;
        }
    }
}
