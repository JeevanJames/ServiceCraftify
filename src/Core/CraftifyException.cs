namespace Jeevan.ServiceCraftify;

public sealed class CraftifyException : Exception
{
    public CraftifyException()
    {
    }

    public CraftifyException(string? message) : base(message)
    {
    }

    public CraftifyException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
