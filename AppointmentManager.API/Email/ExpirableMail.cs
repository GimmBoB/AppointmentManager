using FluentEmail.Core;

namespace AppointmentManager.API.Email;

public record ExpirableMail(IFluentEmail Mail)
{
  public int TryCount { get; set; }
}