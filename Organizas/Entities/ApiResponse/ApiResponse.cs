namespace Organizas.Entities.ApiResponse
{
    public sealed class ApiResponse<T> where T : class
    {
        public bool Success { get; }
        public T? Data { get; }
        public string Message { get; } = string.Empty;
        public IReadOnlyDictionary<string, string[]>? Errors { get; }
        public string? TraceId { get; }

        private ApiResponse(
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

        public static ApiResponse<T> Ok(T data, string message) => new(true, message, null, data, null);

        public static ApiResponse<T> Fail(string message, string traceId, IReadOnlyDictionary<string, string[]>? errors = null)
            => new(false, message, traceId, null, errors);
    }
}
