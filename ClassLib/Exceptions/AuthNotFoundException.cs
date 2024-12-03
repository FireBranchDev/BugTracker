namespace ClassLib.Exceptions;

public class AuthNotFoundException : Exception
{
    public AuthNotFoundException()
    {
    }

    public AuthNotFoundException(string message) : base(message)
    {
    }

    public AuthNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }
}
