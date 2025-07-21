namespace StringFiltering.Application.Results;

public abstract class Result
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }

    private Result(bool isSuccess, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }
    
    public static Result Succeeded() => new SuccessResult();
    public static Result Failed(string errorMessage) => new ErrorResult(errorMessage);



    private sealed class SuccessResult : Result
    {
        public SuccessResult()
            : base(true, null)
        {
        }
    }

    private sealed class ErrorResult : Result
    {
        public ErrorResult(string errorMessage)
            : base(false, errorMessage)
        {
        }
    }
}