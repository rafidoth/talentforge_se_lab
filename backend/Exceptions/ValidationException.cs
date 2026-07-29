namespace server.Exceptions;
public class ValidationException : AppException
{
    public ValidationException(string field, string message) : base(message, 400) {}
}
