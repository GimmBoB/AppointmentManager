namespace AppointmentManager.API;

public class ApiResult
{
    private ApiResult(IEnumerable<string> errors)
    {
        Errors.AddRange(errors);
        Success = false;
    }

    protected ApiResult(bool success)
    {
        Success = success;
    }
    
    public bool Success { get; }

    public List<string> Errors { get; } = new();
    
    public static ApiResult Succeeded() => new(true);
    public static ApiResult Failure(IEnumerable<string> errors) => new(errors);
    public static ApiResult NotFound() => new(false);
}