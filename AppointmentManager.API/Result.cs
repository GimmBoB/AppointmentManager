namespace AppointmentManager.API;

public class Result
{
    protected Result()
    {
    }

    protected Result(IEnumerable<string> errors, ResultTypes resultType)
    {
        Errors.AddRange(errors);
        Success = false;
        ResultType = resultType;
    }

    protected Result(ResultTypes resultType)
    {
        Success = false;
        ResultType = resultType;
    }
    
    public virtual bool Success { get; }

    protected ResultTypes ResultType { get; } = ResultTypes.Succeeded;

    public List<string> Errors { get; } = new();
    
    public static Result Succeeded() => new();
    public static Result Failure(IEnumerable<string> errors) => new(errors, ResultTypes.Failure);
    public static Result NotFound() => new(ResultTypes.NotFound);
}