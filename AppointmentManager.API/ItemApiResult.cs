using System.Diagnostics.CodeAnalysis;
using AppointmentManager.Shared;

namespace AppointmentManager.API;

public class ItemApiResult<T> : ApiResult where T : class
{
    private ItemApiResult(T item) : base(true)
    {
        Item = item;
        Added = false;
    }

    private ItemApiResult(IEntityDto item, bool added) : base(true)
    {
        Item = (T)item;
        Added = added;
    }
    
    public T Item { get; }
    [MemberNotNullWhen(true, nameof(Id))]
    public bool Added { get; }
    public Guid? Id => (Item as IEntityDto)?.Id;
    
    public static ItemApiResult<T> Succeeded(T item) => new(item);
    public static ItemApiResult<T> Created(IEntityDto item) => new(item, true);
}