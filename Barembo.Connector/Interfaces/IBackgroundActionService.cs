using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Interfaces
{
    public interface IBackgroundActionService
    {
        /// <summary>
        /// Returns true, if BackgroundActions are being processed
        /// </summary>
        bool Processing { get; }

        /// <summary>
        /// Starts the processing of BackgroundActions
        /// </summary>
        void ProcessActionsInBackground();

        /// <summary>
        /// Stops the processing of BackgroundActions
        /// </summary>
        void StopProcessingInBackground();
    }
}
