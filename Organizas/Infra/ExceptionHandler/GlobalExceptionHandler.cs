using System.Diagnostics;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Organizas.Entities.ApiResponse;

namespace Organizas.Infra.Errors
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            int statusCode;
            ApiResponse<object> response;

            switch (exception)
            {
                case ValidationException validationException: statusCode = StatusCodes.Status400BadRequest;

                    response = ApiResponse<object>.Fail(
                        "Corrija os campos informados e tente novamente",
                        traceId,
                        GetValidationErrors(validationException));

                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;

                    _logger.LogError(
                        exception,
                        "Falha inesperada ao processar {Method} {Path}. TraceId: {TraceId}",
                        httpContext.Request.Method,
                        httpContext.Request.Path,
                        traceId);

                    response = ApiResponse<object>.Fail(
                        "Não foi possível concluir a operação",
                        traceId
                    );

                    break;
            }

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(
                response,
                cancellationToken: cancellationToken
            );

            return true;
        }

        private static IReadOnlyDictionary<string, string[]> GetValidationErrors(
            ValidationException exception)
        {
            return exception.Errors
                .GroupBy(error =>
                    JsonNamingPolicy.CamelCase.ConvertName(error.PropertyName))
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(error => error.ErrorMessage)
                        .Distinct()
                        .ToArray());
        }
    }
}