using System;
using Mockus.Contracts.Exceptions;
using Polly;

namespace Mockus.Client.Policies
{
    public static class PollyPolicies
    {
        public static AsyncPolicy<T> GetRetryToMatchDataVersionPolicy<T>(int retryCount, Action onRetry = null)
        {
            var jitter = new Random();

            return Policy.Handle<VersionMismatchException>()
                .OrResult<T>(result => result == null)
                .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromMilliseconds(retryAttempt * 50) +
                                                               TimeSpan.FromMilliseconds(jitter.Next(0, 100)));
        }
    }
}
