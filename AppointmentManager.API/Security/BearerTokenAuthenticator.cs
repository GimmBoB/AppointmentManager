using AppointmentManager.API.Repositories;
using Microsoft.IdentityModel.JsonWebTokens;

namespace AppointmentManager.API.Security;

public class BearerTokenAuthenticator
{
    private readonly AdminRepository _adminRepository;
    private readonly JwtValidator _jwtValidator;

    public BearerTokenAuthenticator(AdminRepository adminRepository, JwtValidator jwtValidator)
    {
        _adminRepository = adminRepository;
        _jwtValidator = jwtValidator;
    }

    public async Task<TokenResult> GetTokenResultAsync(string token, CancellationToken ct, string expectedTokenType = "access")
    {
        JsonWebToken jwt;
        try
        {
            jwt = new JsonWebToken(token);
        }
        catch
        {
            return TokenResult.InValid();
        }
        var isValid = _jwtValidator.IsTokenValid(jwt, expectedTokenType);
        if (!isValid)
            return TokenResult.InValid();

        var idAsString = jwt.TryGetClaim("Id", out var idClaim) ? idClaim.Value : string.Empty;
        var id = Guid.TryParse(idAsString, out var parsedId) ? parsedId : Guid.Empty;

        var admin =  await _adminRepository.GetByIdAsync(id, ct);

        return admin is not null 
            ? TokenResult.Valid(admin)
            : TokenResult.InValid();
    }
}