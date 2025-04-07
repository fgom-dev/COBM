namespace Cobm.Application.Errors;

public record AppError(string Message, ErrorType ErrorType);

public record AlreadyExistsError<T>() : AppError($"{typeof(T).Name} already exists.", ErrorType.Validation);
public record NotFoundError<T>() : AppError($"{typeof(T).Name} not found.", ErrorType.Validation);
public record InvalidEmailOrPasswordError(): AppError("Email or password invalid.", ErrorType.Validation);
public record InvalidTokenError() : AppError("Invalid Token", ErrorType.Validation);
