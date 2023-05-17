using Mockus.Contracts.Abstractions.Integration;

namespace Mockus.Contracts.Response
{
    public class ResponseOptions : IResponseOptions
    {
        public int StatusCode { get; set; }
        public int? DelaySeconds { get; set; }

        public ResponseOptions()
        {
            StatusCode = 200;
        }

        public override bool Equals(object? obj)
        {
            var other = (obj as ResponseOptions);

            if (other == null)
                return false;

            return other.DelaySeconds.Equals(DelaySeconds) && 
                   other.StatusCode.Equals(StatusCode);
        }
    }
}