using System.Net;

namespace App.Services;

public class ServiceResult<T>
{
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public bool IsSuccess => Errors == null || Errors.Count == 0;
    public bool IsFailure => !IsSuccess;
    public HttpStatusCode StatusCode { get; set; }

    public static ServiceResult<T> Success(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ServiceResult<T>
        {
            Data = data,
            StatusCode = statusCode
        };
    }

    public static ServiceResult<T> Failure(List<string> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ServiceResult<T>
        {
            Errors = errors,
            StatusCode = statusCode
        };
    }

    public static ServiceResult<T> Failure(string error, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ServiceResult<T>
        {
            Errors = new List<string> { error },
            StatusCode = statusCode
        };
    }

}
