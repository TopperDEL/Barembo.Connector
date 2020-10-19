using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    public interface IStoreAccessService
    {
        StoreAccess ShareBookAccess(StoreAccess baseAccess, BookReference bookReferenceToShare, Contributor contributor, AccessRights accessRights);

        StoreAccess ShareBookShareAccess(StoreAccess baseAccess, StoreKey storeKey);
    }
}
