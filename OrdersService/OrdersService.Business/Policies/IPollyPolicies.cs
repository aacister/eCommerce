﻿using Polly;

namespace OrdersService.Business.Policies;

public interface IPollyPolicies
{
    IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount);
    IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak);
    IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeout);
}