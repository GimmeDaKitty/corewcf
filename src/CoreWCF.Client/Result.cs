namespace CoreWCF.Client;

public class Result
{
    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
    
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }
    
    public static Result Ok = new Result(true, null);
    public static Result NOk(string errorMessage) => new Result(false, errorMessage);
}

public sealed class Result<T> : Result
{
    private Result(bool isSuccess, T? value, string? error) : base(isSuccess, error)
    {
        Value = value;
    }
    
    public T? Value { get; init; }
    public static Result<T> OkResult(T value) => new(true, value, null);
    public static Result<T> NOkResult(string errorMessage) => new(false, default, errorMessage);
}