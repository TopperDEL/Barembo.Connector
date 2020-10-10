using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Barembo.Services
{
    public class ContributorStoreService : IContributorStoreService
    {
        private IStoreService _storeService;

        public ContributorStoreService(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public async Task<IEnumerable<Contributor>> ListAsync(BookReference bookRef)
        {
            var access = new StoreAccess(bookRef.AccessGrant);
            var entries = await _storeService.ListObjectsAsync(access, StoreKey.Contributors(bookRef.BookId));

            var contributors = new List<Contributor>();
            foreach (var contributor in entries)
                contributors.Add(await _storeService.GetObjectFromJsonAsync<Contributor>(access, StoreKey.Contributor(bookRef.BookId, contributor.Id)));

            return contributors;
        }

        public async Task<bool> SaveAsync(BookReference bookRef, Contributor contributorToSave)
        {
            return await _storeService.PutObjectAsJsonAsync<Contributor>(new StoreAccess(bookRef.AccessGrant), StoreKey.Contributor(bookRef.BookId, contributorToSave.Id), contributorToSave);
        }
    }
}
