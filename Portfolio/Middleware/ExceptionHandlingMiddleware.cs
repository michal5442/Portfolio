using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Portfolio.Middleware
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = ResolveCorrelationId(context);
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception occurred while processing request. CorrelationId: {CorrelationId}", correlationId);
                    await HandleExceptionAsync(context, correlationId);
                }
            }
        }

        private static string ResolveCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(CorrelationIdMiddleware.HeaderName, out var incoming) && !string.IsNullOrWhiteSpace(incoming))
            {
                return incoming.ToString();
            }

            if (context.Request.Headers.TryGetValue("X-Request-ID", out incoming) && !string.IsNullOrWhiteSpace(incoming))
            {
                return incoming.ToString();
            }

            return Guid.NewGuid().ToString("N");
        }

        private static Task HandleExceptionAsync(HttpContext context, string correlationId)
        {
            if (context.Response.HasStarted)
            {
                context.Response.Headers[CorrelationIdMiddleware.HeaderName] = correlationId;
                return Task.CompletedTask;
            }

            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            context.Response.Headers[CorrelationIdMiddleware.HeaderName] = correlationId;

            var payload = JsonSerializer.Serialize(new { error = "An unexpected error occurred." });
            return context.Response.WriteAsync(payload);
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
