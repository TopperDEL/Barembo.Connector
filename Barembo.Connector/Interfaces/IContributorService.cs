using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    public interface IContributorService
    {
        /// <summary>
        /// Saves a Contributor using a BookReference.
        /// </summary>
        /// <param name="bookRef">The BookReference for a Contributor</param>
        /// <param name="contributorToSave">The Contributor to save</param>
        /// <returns>true, if the Contributor could be saved</returns>
        Task<bool> SaveAsync(BookReference bookRef, Contributor contributorToSave);

        /// <summary>
        /// Lists all Contributors of a BookReference
        /// </summary>
        /// <param name="bookRef">The BookReference where the contributors should be listed</param>
        /// <returns>The list of contributors</returns>
        Task<IEnumerable<Contributor>> ListAsync(BookReference bookRef);
    }
}
