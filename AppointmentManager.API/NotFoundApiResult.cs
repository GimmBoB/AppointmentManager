namespace AppointmentManager.API;

public class NotFoundApiResult : ApiResult
{
    private NotFoundApiResult() : base(false)
    {
    }

    public static NotFoundApiResult NotFound() => new();
}