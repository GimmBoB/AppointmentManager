using System.Diagnostics.CodeAnalysis;
using AppointmentManager.API.Models;

namespace AppointmentManager.API.Security;

public class TokenResult
{
    private TokenResult(Admin admin)
    {
        IsValid = true;
        Admin = admin;
    }

    private TokenResult()
    {
        IsValid = false;
    }
    
    [MemberNotNullWhen(true, nameof(Admin))]
    public bool IsValid { get; }
    public Admin? Admin { get; }

    public static TokenResult Valid(Admin admin) => new(admin);
    public static TokenResult InValid() => new();
}