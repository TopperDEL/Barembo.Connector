using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Barembo.StoreServices
{
    internal class BookShareStoreService : IBookShareStoreService
    {
        private IStoreService _storeService;
        IStoreAccessService _storeAccessService;

        public BookShareStoreService(IStoreService storeService, IStoreAccessService storeAccessService)
        {
            _storeService = storeService;
            _storeAccessService = storeAccessService;
        }

        public async Task<IEnumerable<BookShareReference>> ListBookSharesAsync(StoreAccess storeAccess, BookReference bookReference)
        {
            var shares = await _storeService.ListObjectsAsync(storeAccess, StoreKey.BookShares(bookReference.BookId));

            return shares.Select(e => new BookShareReference() { StoreAccess = storeAccess, StoreKey = StoreKey.BookShare(bookReference.BookId, e.Id) });
        }

        public async Task<BookShare> LoadBookShareAsync(BookShareReference bookShareReference)
        {
            var bookShareInfo = await _storeService.GetObjectInfoAsync(bookShareReference.StoreAccess, bookShareReference.StoreKey);

            if (!bookShareInfo.ObjectExists)
                throw new BookShareNotFoundException();

            return await _storeService.GetObjectFromJsonAsync<BookShare>(bookShareReference.StoreAccess, bookShareReference.StoreKey);
        }

        public async Task<BookShareReference> SaveBookShareAsync(StoreAccess storeAccess, BookShare bookShare)
        {
            var storeKey = StoreKey.BookShare(bookShare.BookId, bookShare.Id);

            var success = await _storeService.PutObjectAsJsonAsync<BookShare>(storeAccess, storeKey, bookShare);

            if (success)
            {
                BookShareReference reference = new BookShareReference();
                reference.StoreKey = storeKey;
                reference.StoreAccess = await _storeAccessService.ShareBookShareAccessAsync(storeAccess, storeKey);

                return reference;
            }
            else
                throw new BookShareCouldNotBeSavedException();
        }
    }
}
