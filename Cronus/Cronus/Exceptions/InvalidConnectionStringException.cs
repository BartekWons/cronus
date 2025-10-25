namespace Cronus.Exceptions;

public class InvalidConnectionStringException : Exception
{
    public InvalidConnectionStringException()
        : base("Invalid connection string") { }

    public InvalidConnectionStringException(string message)
        : base(message) { }
}
