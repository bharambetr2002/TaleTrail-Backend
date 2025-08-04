namespace TaleTrail.API.Exceptions
{
    public class ValidationException : AppException
    {
        public ValidationException() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception innerException) : base(message, innerException) { }
        public ValidationException(Dictionary<string, string[]> errors) : base("Validation failed")
        {
            Errors = errors;
        }

        public Dictionary<string, string[]>? Errors { get; }
    }
}