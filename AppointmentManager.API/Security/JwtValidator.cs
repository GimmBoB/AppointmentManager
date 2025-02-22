using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using AppointmentManager.API.config;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AppointmentManager.API.Security;

public class JwtValidator
{
    private readonly TokenValidationConfig _tokenValidationConfig;
    private readonly CertificateProvider _certificateProvider;
    public JwtValidator(
        TokenValidationConfig tokenValidationConfig,
        CertificateProvider certificateProvider
    )
    {
        _tokenValidationConfig = tokenValidationConfig;
        _certificateProvider = certificateProvider;
    }

    public bool IsTokenValid(JsonWebToken jwt)
    {
        var utcNow = DateTime.UtcNow;

        if (utcNow > jwt.ValidTo)
            return false;

        var idAsString = jwt.TryGetClaim("Id", out var idClaim) ? idClaim.Value : null;
        if (string.IsNullOrWhiteSpace(idAsString))
            return false;

        if (!Guid.TryParse(idAsString, out _))
            return false;

        var tokenType = jwt.TryGetClaim("TokenType", out var tokenTypeClaim) ? tokenTypeClaim.Value : null;
        if (string.IsNullOrWhiteSpace(tokenType) || tokenType != "access")
            return false;

        var certs = _certificateProvider.LoadCollection();
        if (!certs.Any())
            return false;

        var encodedToken = jwt.EncodedToken;
        var tokenHandler = new JwtSecurityTokenHandler();

        foreach (var validationParameters in certs.Select(CreateTokenValidationParameters))
        {
            try
            {
                tokenHandler.ValidateToken(encodedToken, validationParameters, out var validatedToken);
                if (validatedToken != null) return true;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return false;
    }

    private TokenValidationParameters CreateTokenValidationParameters(X509Certificate2 cert)
    {
        return new TokenValidationParameters()
        {
            ValidateAudience = _tokenValidationConfig.ValidateAudience,
            ValidateIssuer = _tokenValidationConfig.ValidateIssuer,
            ValidateLifetime = _tokenValidationConfig.ValidateLifetime,
            ValidAudience = _tokenValidationConfig.ValidAudience,
            ValidIssuer = _tokenValidationConfig.ValidIssuer,
            IssuerSigningKey = new X509SecurityKey(cert)
        };
    }
}