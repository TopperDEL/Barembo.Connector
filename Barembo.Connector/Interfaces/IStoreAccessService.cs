using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    public interface IStoreAccessService
    {
        Task<StoreAccess> ShareBookAccessAsync(StoreAccess baseAccess, BookReference bookReferenceToShare, Contributor contributor, AccessRights accessRights);

        Task<StoreAccess> ShareBookShareAccessAsync(StoreAccess baseAccess, StoreKey storeKey);
    }
}
