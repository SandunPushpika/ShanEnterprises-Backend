namespace Core.Exceptions;

public class PaymentException(string error) : Exception(error)
{
    
}