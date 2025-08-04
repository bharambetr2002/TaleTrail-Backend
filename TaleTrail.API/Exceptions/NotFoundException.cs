namespace TaleTrail.API.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException() { }
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}