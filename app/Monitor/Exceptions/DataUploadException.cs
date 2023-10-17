namespace Monitor.Exceptions;

public class DataUploadException : Exception
{
    public DataUploadException(string message, Exception innerException) : base(message, innerException) { }
}
