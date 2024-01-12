namespace Monitor.Shared.Exceptions;

public class MissingPermissionsException : Exception
{
    public MissingPermissionsException(string message) : base(message)
    {
    }
}

public class ExtraPermissionsException : Exception
{
    public ExtraPermissionsException(string message) : base(message)
    {
    }
}
