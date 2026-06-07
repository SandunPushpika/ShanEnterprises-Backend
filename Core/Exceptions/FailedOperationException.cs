namespace Core.Exceptions;

public class FailedOperationException(string message) : Exception(message)
{
}