using Cobm.Application.Errors;
using OneOf;

namespace Cobm.Application.Extensions;

public static class OneOfExtensions
{
    public static bool IsSuccess<TResult>(this OneOf<TResult, AppError> obj) => obj.IsT0;
    public static TResult GetSuccessResult<TResult>(this OneOf<TResult, AppError> obj) => obj.AsT0;
    public static AppError GetError<TResult>(this OneOf<TResult, AppError> obj) => obj.AsT1;
}