using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Organizas.Dtos;
using Organizas.Exceptions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

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
            ApiResponseDto<object> response;

            LogException(httpContext, exception, traceId);

            switch (exception)
            {
                case ValidationException validationException:
                    statusCode = StatusCodes.Status400BadRequest;

                    response = ApiResponseDto<object>.Fail(
                        "Corrija os campos informados e tente novamente",
                        traceId,
                        GetValidationErrors(validationException)
                    );

                    break;

                case ItemNotFoundException itemNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;

                    response = ApiResponseDto<object>.Fail(
                        itemNotFoundException.Message,
                        traceId
                    );

                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;

                    response = ApiResponseDto<object>.Fail(
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

        private void LogException(HttpContext httpContext, Exception exception, string traceId)
        {
            switch (exception)
            {
                case ValidationException:
                case ItemNotFoundException:
                    _logger.LogWarning(
                        "Requisição rejeitada por {ExceptionType} em {Method} {Path}. TraceId: {TraceId}",
                        exception.GetType().Name,
                        httpContext.Request.Method,
                        httpContext.Request.Path,
                        traceId);
                    break;

                default:
                    _logger.LogError(
                        exception,
                        "Falha inesperada ao processar {Method} {Path}. TraceId: {TraceId}",
                        httpContext.Request.Method,
                        httpContext.Request.Path,
                        traceId);
                    break;
            }
        }

    }
}