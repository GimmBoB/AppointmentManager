namespace AppointmentManager.Web.Extensions;

public static class ListExtensions
{
    public static bool TryGetElement<T>(this List<T>? collection, int index, out T element)
    {
        if(collection == null || index < 0 || index >= collection.Count)
        {
            element = default!;
            return false;
        }

        element = collection[index];
        return true;
    }
}