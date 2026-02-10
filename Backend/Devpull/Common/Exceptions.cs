namespace Devpull.Controllers;

public class AuthenticationException : Exception
{
    public AuthenticationException(string message)
        : base(message) { }

    public AuthenticationException() { }
}

public class AuthorizationException : Exception
{
    public AuthorizationException(string message)
        : base(message) { }
}

public class NotFoundException : Exception
{
    public NotFoundException(string entityId)
        : base($"Запись с id {entityId} не найдена или удалена") { }
}
