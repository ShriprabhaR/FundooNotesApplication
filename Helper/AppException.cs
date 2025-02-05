using System;

namespace FundooNotesApp.Helper
{
    public class AppException : Exception
    {

        // Custom exception class for throwing application specific exceptions 
        // that can be caught and handled within the application

        public AppException() : base() { }

        public AppException(string message) : base(message) { }

    }
}
