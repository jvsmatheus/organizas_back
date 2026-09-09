namespace Organizas.Dtos
{
    public sealed class ApiResponseDto<T> where T : class
    {
        public bool Success { get; }
        public T? Data { get; }
        public string Message { get; } = string.Empty;
        public IReadOnlyDictionary<string, string[]>? Errors { get; }
        public string? TraceId { get; }

        private ApiResponseDto(
            bool success,
            string message,
            string? traceId,
            T? data,
            IReadOnlyDictionary<string, string[]>? errors
        )
        {
            Success = success;
            Data = data;
            Message = message;
            Errors = errors;
            TraceId = traceId;
        }

        public static ApiResponseDto<T> Ok(T? data, string message) => new(true, message, null, data, null);

        public static ApiResponseDto<T> Fail(string message, string traceId, IReadOnlyDictionary<string, string[]>? errors = null)
            => new(false, message, traceId, null, errors);
    }
}
