using System.Net;
using System.Text.Json.Serialization;

namespace App.Services;

public class ServiceResult<T>
{
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    [JsonIgnore]
    public bool IsSuccess => Errors == null || Errors.Count == 0;
    [JsonIgnore]
    public bool IsFailure => !IsSuccess;
    [JsonIgnore]
    public HttpStatusCode StatusCode { get; set; }
    [JsonIgnore]
    public string? UrlAsCreated { get; set; }

    public static ServiceResult<T> Success(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ServiceResult<T>
        {
            Data = data,
            StatusCode = statusCode
        };
    }

    public static ServiceResult<T> SuccessAsCreated(T data, string urlAsCreated)
    {
        return new ServiceResult<T>
        {
            Data = data,
            StatusCode = HttpStatusCode.Created,
            UrlAsCreated = urlAsCreated
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

//No Return Any Data Version
public class ServiceResult
{
    public List<string>? Errors { get; set; }
    [JsonIgnore]
    public bool IsSuccess => Errors == null || Errors.Count == 0;
    [JsonIgnore]
    public bool IsFailure => !IsSuccess;
    [JsonIgnore]
    public HttpStatusCode StatusCode { get; set; }

    public static ServiceResult Success(HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ServiceResult()
        {
            StatusCode = statusCode
        };
    }

    public static ServiceResult Failure(List<string> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ServiceResult()
        {
            Errors = errors,
            StatusCode = statusCode
        };
    }

    public static ServiceResult Failure(string error, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ServiceResult()
        {
            Errors = new List<string> { error },
            StatusCode = statusCode
        };
    }

}