namespace Mockus.Contracts.Abstractions.Integration
{
    public interface IResponseOptions
    {
        public int StatusCode { get; set; }
        public int? DelaySeconds { get; set; }
    }
}