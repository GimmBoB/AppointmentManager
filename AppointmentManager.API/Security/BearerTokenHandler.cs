using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace AppointmentManager.API.Security;

public class BearerTokenHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly BearerTokenAuthenticator _bearerTokenAuthenticator;

    public BearerTokenHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        BearerTokenAuthenticator bearerTokenAuthenticator) : base(options, logger, encoder, clock)
    {
        _bearerTokenAuthenticator = bearerTokenAuthenticator;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return await Task.Run(async () =>
        {
            var bearerToken = GetBearerToken();
            return await ValidateTokenAsync(bearerToken);
        });
    }

    private string? GetBearerToken()
    {
        var authHeaderValue = Context.Request.Headers[HeaderNames.Authorization];
        if (string.IsNullOrWhiteSpace(authHeaderValue))
        {
            return null;
        }

        var token = AuthenticationHeaderValue.Parse(authHeaderValue).Parameter;

        return token;
    }

    private async Task<AuthenticateResult> ValidateTokenAsync(string? token)
    {
        if (token == null)
        {
            return AuthenticateResult.Fail(
                $"{HeaderNames.Authorization} is missing or format of bearer token is not valid.");
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return AuthenticateResult.Fail($"{HeaderNames.Authorization} contains only whitespaces.");
        }

        var tokenResult = await _bearerTokenAuthenticator.GetTokenResultAsync(token, CancellationToken.None);

        if (tokenResult.IsValid)
        {
            var claimPrincipal = ClaimsPrincipalFactory.Create(tokenResult.Admin);

            var authTicket = new AuthenticationTicket(claimPrincipal, JwtBearerDefaults.AuthenticationScheme);

            return AuthenticateResult.Success(authTicket);
        }

        return AuthenticateResult.Fail("Authentication failure.");
    }
}