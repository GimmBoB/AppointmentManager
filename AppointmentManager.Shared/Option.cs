namespace AppointmentManager.Shared;

public class Option<T>
{
    private bool _isSome;
    private bool _isError;

    private T _some;
    private string[] _errors = Array.Empty<string>();

    private Option()
    {
        _isSome = false;
        _isError = false;
        _some = default!;
    }

    private Option(T some)
    {
        _isSome = true;
        _isError = false;
        _some = some;
    }

    private Option(IEnumerable<string> errors)
    {
        _isSome = false;
        _isError = true;
        _some = default!;
        _errors = errors.ToArray();
    }

    public static Option<T> None => new();
    public static Option<T> Some(T some) => new(some);
    public static Option<T> Error(IEnumerable<string> errors) => new(errors);

    public TResult ContinueWith<TResult>(Func<T, TResult> useSome, Func<IEnumerable<string>, TResult> useError, Func<TResult> useNothing)
    {
        if (_isSome)
            return useSome(_some);
        if (_isError)
            return useError(_errors);

        return useNothing();
    }
    
    public async Task<TResult> ContinueWithAsync<TResult>(Func<T, Task<TResult>> useSome, Func<IEnumerable<string>, Task<TResult>> useError, Func<Task<TResult>> useNothing)
    {
        if (_isSome)
            return await useSome(_some);
        if (_isError)
            return await useError(_errors);

        return await useNothing();
    }
}