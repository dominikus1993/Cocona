namespace Cocona;

public sealed class CoconaException : Exception
{
    public CoconaException(string message)
        : base(message)
    {
    }

    public CoconaException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
