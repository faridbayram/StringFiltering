namespace StringFiltering.Application.Exceptions;

public class BusinessException : Exception
{
    public string ClientMessage { get; }

    public BusinessException(string clientMessage, string exceptionMessage)
        : base(exceptionMessage)
    {
        ClientMessage = clientMessage;
    }
}