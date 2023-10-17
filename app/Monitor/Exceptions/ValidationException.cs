namespace Monitor.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string message, Exception innerException) : base(message, innerException) { }
}
