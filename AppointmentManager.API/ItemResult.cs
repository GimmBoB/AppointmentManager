using System.Diagnostics.CodeAnalysis;

namespace AppointmentManager.API;

public class ItemResult<T> : Result where T : class
{
    private ItemResult(T item)
    {
        Item = item;
        Success = true;
    }
    
    public T? Item { get; }
    
    [MemberNotNullWhen(true, nameof(Item))]
    public override bool Success { get; }


    public static ItemResult<T> Succeeded(T item) => new(item);
}