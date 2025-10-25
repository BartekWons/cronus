namespace Cronus.Exceptions;

public class AttributeNotFoundException : Exception
{
    public AttributeNotFoundException()
    : base("Attribute not found") { }

    public AttributeNotFoundException(string message)
        : base(message) { }
}
