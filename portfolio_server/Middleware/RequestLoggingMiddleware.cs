using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Linq;

namespace portfolio_server.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;
        public const string HeaderName = "X-Correlation-ID";

        public const string ItemsKey = "CorrelationId";

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = ResolveCorrelationId(context);

            context.Items[ItemsKey] = correlationId;

            context.Response.Headers[HeaderName] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            using (LogContext.PushProperty("RequestId", context.TraceIdentifier))
            {
                _logger.LogDebug("Correlation id attached to request. CorrelationId: {CorrelationId}", correlationId);
                await _next(context);
            }
        }

        private static string ResolveCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(HeaderName, out var incoming) && !string.IsNullOrWhiteSpace(incoming.ToString()))
            {
                return incoming.ToString();
            }

            if (context.Request.Headers.TryGetValue("X-Request-ID", out incoming) && !string.IsNullOrWhiteSpace(incoming.ToString()))
            {
                return incoming.ToString();
            }

            return Guid.NewGuid().ToString("N");
        }
    }

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var routeData = context.GetRouteData();
            var sanitizedRouteValues = GetSanitizedRouteValues(routeData?.Values);
            var sanitizedQueryString = GetSanitizedQueryString(request.Query);

            _logger.LogInformation(
                "Request received. Method: {RequestMethod}, Path: {RequestPath}, QueryString: {RequestQuery}, RouteValues: {RouteValues}",
                request.Method,
                request.Path,
                sanitizedQueryString,
                sanitizedRouteValues);

            await _next(context);
        }

        private static string GetSanitizedQueryString(IQueryCollection query)
        {
            if (query == null || !query.Any())
            {
                return string.Empty;
            }

            return string.Join("&", query.Select(pair => $"{pair.Key}=[REDACTED]"));
        }

        private static string GetSanitizedRouteValues(RouteValueDictionary? values)
        {
            if (values == null || values.Count == 0)
            {
                return string.Empty;
            }

            var safeValues = values
                .Where(pair => pair.Value is not null)
                .Select(pair => (pair.Key, Value: pair.Value!.ToString()))
                .Select(pair => (pair.Key, Value: GetSafeRouteValue(pair.Key, pair.Value)))
                .Where(pair => !string.IsNullOrEmpty(pair.Value))
                .Select(pair => $"{pair.Key}={pair.Value}")
                .ToList();

            return safeValues.Count > 0 ? string.Join(", ", safeValues) : string.Empty;
        }

        private static string GetSafeRouteValue(string key, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            if (key.Equals("id", StringComparison.OrdinalIgnoreCase)
                || key.EndsWith("Id", StringComparison.OrdinalIgnoreCase)
                || key.EndsWith("Code", StringComparison.OrdinalIgnoreCase))
            {
                return value;
            }

            return "[REDACTED]";
        }
    }

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CorrelationIdMiddleware>();
        }

        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
