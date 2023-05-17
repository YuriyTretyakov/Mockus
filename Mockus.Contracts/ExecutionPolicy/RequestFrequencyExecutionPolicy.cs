using System;
using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts.Response;
using Mockus.Contracts.Enums;


namespace Mockus.Contracts.ExecutionPolicy
{
    public class RequestFrequencyExecutionPolicy : IExecutionPolicy
    {
        private DateTime LastCallTime { get; set; }
        public TimeSpan AllowedInterval { get; set; }
        public PolicyStatus Status { get; set; }
        public MockedResponse Response { get; set; }

        public MockedResponse Run()
        {
            if (Status == PolicyStatus.Active && DateTime.UtcNow < LastCallTime.Add(AllowedInterval))
            {
                LastCallTime = DateTime.UtcNow;
                return Response;
            }
            LastCallTime = DateTime.UtcNow;
            return default;
        }

    }
}
