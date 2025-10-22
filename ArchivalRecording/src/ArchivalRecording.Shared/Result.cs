namespace Shared;

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Model { get; set; }
    public string? ErrorMessage { get; set; }

    public static Result<T> Success(T model) => new() { IsSuccess = true, Model = model };

    public static Result<T> Failure(string errorMessage) => new() { IsSuccess = false, ErrorMessage = errorMessage };
}
