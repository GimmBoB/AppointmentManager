using System.Collections.Concurrent;
using System.Text;
using AppointmentManager.API.config;
using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace AppointmentManager.API.Email;

public class MailService
{
    private readonly SmtpConfig _smtpConfig;
    private readonly IFluentEmailFactory _emailFactory;
    private static readonly ConcurrentQueue<ExpirableMail> Queue = new();

    public MailService(SmtpConfig smtpConfig, IFluentEmailFactory emailFactory)
    {
        _smtpConfig = smtpConfig;
        _emailFactory = emailFactory;
    }
    
    public async Task CreateAndSendMailFromTemplateAsync(string templateKey, string subject, string recipient, object model)
    {
        var mail = _emailFactory
                .Create()
                .Subject(subject)
                .To(recipient)
                .Body("Nutte")
            ;// .UsingTemplate(_resourceProvider.GetTemplateAsString(_mailLocalizer[templateKey]), model);

        var expirableMail = new ExpirableMail(mail);

        Queue.Enqueue(expirableMail);
        await Task.Run(RetrySendMail);
    }
    
    private void RetrySendMail()
    {
        TrySendMail();
    }
    private async void TrySendMail()
    {
        while (Queue.TryDequeue(out var expirableMail))
        {
            try
            {
                var sendResponse = await expirableMail.Mail.SendAsync();
                if (!sendResponse.Successful)
                {
                    await CheckResendMailAsync(expirableMail, sendResponse);
                }
            }
            catch (Exception)
            {
                var resendMail = await CheckResendMailAsync(expirableMail);
                if (!resendMail)
                    throw;
            }
        }
    }

    private async Task<bool> CheckResendMailAsync(ExpirableMail expirableMail, SendResponse? sendResponse = default)
    {
        expirableMail.TryCount++;
        if (expirableMail.TryCount > _smtpConfig.MailSendRetries)
        {
            if (sendResponse != null)
            {
                var stringBuilder = new StringBuilder();
                sendResponse.ErrorMessages.ToList().ForEach(errorMessage => stringBuilder.Append($" \n {errorMessage}"));
                var errorMessage = stringBuilder.ToString();

                throw new Exception(
                    $"And error occured while trying to send an e-mail to {expirableMail.Mail.Data.ToAddresses} {errorMessage}");
            }
            return false;
        }

        Queue.Enqueue(expirableMail);
        var task = new Task(RetrySendMail);
        await Task.Delay(_smtpConfig.MillisecondsTillRetry);
        task.Start();
        return true;
    }
}