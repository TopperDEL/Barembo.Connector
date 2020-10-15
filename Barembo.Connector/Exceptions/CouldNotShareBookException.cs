using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Exceptions
{
    public enum CouldNotShareBookReason
    {
        BookShelfNotFound = 1,
        CouldNotSaveContributor = 2,
        BookShareCouldNotBeSaved = 3
    }
    public class CouldNotShareBookException : Exception
    {
        public CouldNotShareBookReason Reason { get; private set; }
        public CouldNotShareBookException(CouldNotShareBookReason reason)
        {
            Reason = reason;
        }
    }
}
