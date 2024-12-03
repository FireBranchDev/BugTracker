namespace ClassLib.Exceptions;

public class UserInvalidDisplayNameException : Exception
{
    public UserInvalidDisplayNameException()
    {
    }

    public UserInvalidDisplayNameException(string message) : base(message)
    {
    }
}
