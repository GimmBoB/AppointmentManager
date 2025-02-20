namespace AppointmentManager.API;

public class ItemApiResult<T> : ApiResult where T : class
{
    private ItemApiResult(T item) : base(true)
    {
        Item = item;
    }
    
    public T Item { get; }
    
    public static ItemApiResult<T> Succeeded(T item) => new(item);
}