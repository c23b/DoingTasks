using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.Timeout;
using System.Net;

namespace DoingTasks.Infrastructure.ExternalServices.Common;

internal static class OAuthResiliencePipeline
{
    internal static IHttpClientBuilder AddOAuthResilience(this IHttpClientBuilder builder)
    {
        builder.AddResilienceHandler("oauth", pipeline =>
        {
            // 1. Retry — apenas erros transientes
            pipeline.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 2,
                Delay = TimeSpan.FromMilliseconds(500),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .Handle<TimeoutRejectedException>()
                    .HandleResult(r =>
                        r.StatusCode == HttpStatusCode.InternalServerError ||    // 500
                        r.StatusCode == HttpStatusCode.BadGateway ||             // 502
                        r.StatusCode == HttpStatusCode.ServiceUnavailable ||     // 503
                        r.StatusCode == HttpStatusCode.GatewayTimeout ||         // 504
                        r.StatusCode == HttpStatusCode.TooManyRequests ||        // 429
                        r.StatusCode == HttpStatusCode.RequestTimeout)           // 408
            });

            // 2. Timeout por tentativa — 10 segundos
            pipeline.AddTimeout(TimeSpan.FromSeconds(10));

            // 3. Circuit Breaker
            pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(30),
                MinimumThroughput = 5,
                BreakDuration = TimeSpan.FromSeconds(15),
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .Handle<TimeoutRejectedException>()
                    .HandleResult(r =>
                        r.StatusCode == HttpStatusCode.InternalServerError ||
                        r.StatusCode == HttpStatusCode.BadGateway ||
                        r.StatusCode == HttpStatusCode.ServiceUnavailable ||
                        r.StatusCode == HttpStatusCode.GatewayTimeout)
            });
        });

        return builder;
    }
}
