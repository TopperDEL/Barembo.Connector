using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    public interface IStoreAccessService
    {
        /// <summary>
        /// Generate a StoreAccess from LoginData
        /// </summary>
        /// <param name="loginData">The LoginData to use</param>
        /// <returns>A StoreAccess</returns>
        StoreAccess GenerateAccesFromLogin(LoginData loginData);

        StoreAccess ShareBookAccess(StoreAccess baseAccess, BookReference bookReferenceToShare, Contributor contributor, AccessRights accessRights);

        StoreAccess ShareBookShareAccess(StoreAccess baseAccess, StoreKey storeKey);
    }
}
