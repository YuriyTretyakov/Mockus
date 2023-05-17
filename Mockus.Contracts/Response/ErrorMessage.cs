using System;

namespace Mockus.Contracts.Response
{
    public class ErrorMessage
    {
        public string Error { get; set; }
        public string InnerError { get; set; }

        public static ErrorMessage Convert(Exception ex)
        {
            return new()
            {
                Error = ex.Message,
                InnerError = ex.InnerException?.Message
            };
        }
    }
}