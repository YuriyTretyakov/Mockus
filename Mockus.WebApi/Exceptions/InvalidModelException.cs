using System;

namespace Mockus.WebApi.Exceptions
{
    public class InvalidModelException : Exception
    {
        public InvalidModelException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidModelException(string message) : base(message)
        {
        }

        public InvalidModelException(string message, string[] errors) : base(
            $" {message}  Details: {string.Join(Environment.NewLine, errors)}")
        {
        }
    }
}