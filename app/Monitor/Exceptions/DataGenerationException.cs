namespace Monitor.Exceptions;

public class DataGenerationException : Exception
{
    public DataGenerationException(string message, Exception innerException) : base(message, innerException) { }
}
