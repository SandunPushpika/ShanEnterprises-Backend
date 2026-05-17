namespace Core.Exceptions.Auth;

public class UserAlreadyExistsException(string email) : Exception($"User with email {email} already exists");