using System;

namespace Mockus.Contracts.Exceptions
{
    public class VersionMismatchException :Exception
    {
        public VersionMismatchException() : base("Dataversion missmatch")
        {
        }
        public VersionMismatchException(int currentDataVersion, int storedDataVersion) 
            : base($"Dataversion missmatch. Requested dataversion '{currentDataVersion}' stored data version '{storedDataVersion}'")
        {
        }

        public VersionMismatchException(string message) : base(message)
        {
        }
    }
}
