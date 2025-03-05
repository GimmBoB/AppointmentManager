using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AppointmentManager.API.config;
using AppointmentManager.API.Models;
using AppointmentManager.API.Repositories;
using AppointmentManager.API.Security;
using AppointmentManager.Shared;
using Microsoft.IdentityModel.Tokens;

namespace AppointmentManager.API.ControllerServices;

public class AuthenticationService
{
    private readonly AdminRepository _adminRepository;
    private readonly AdminConfiguration _adminConfiguration;
    private readonly TokenValidationConfig _tokenValidationConfig;
    private readonly CertificateProvider _certificateProvider;
    private readonly BearerTokenAuthenticator _bearerTokenAuthenticator;

    public AuthenticationService(AdminRepository adminRepository, AdminConfiguration adminConfiguration, TokenValidationConfig tokenValidationConfig, CertificateProvider certificateProvider, BearerTokenAuthenticator bearerTokenAuthenticator)
    {
        _adminRepository = adminRepository;
        _adminConfiguration = adminConfiguration;
        _tokenValidationConfig = tokenValidationConfig;
        _certificateProvider = certificateProvider;
        _bearerTokenAuthenticator = bearerTokenAuthenticator;
    }

    public async Task<ApiResult> LoginAsync(LoginDto dto, CancellationToken ct)
    {
        var admin = await _adminRepository.GetByEmailAsync(dto.Email, ct);

        if (admin is null)
            return NotFoundApiResult.NotFound();

        if (!string.Equals(dto.Password, StringCipher.Decrypt(admin.Password, _adminConfiguration.SecretKey)))
            return ApiResult.Failure(new List<string> { "Wrong password"});

        var token = CreateToken(admin);

        return ItemApiResult<TokenDto>.Succeeded(token);
    }

    public async Task<ApiResult> RefreshAsync(RefreshDto dto, CancellationToken ct)
    {
        var tokenResult = await _bearerTokenAuthenticator.GetTokenResultAsync(dto.RefreshToken, ct, "refresh");

        if (!tokenResult.IsValid)
            return ApiResult.Failure(new[] { "Invalid refresh token" });

        var token = CreateToken(tokenResult.Admin);
        
        return ItemApiResult<TokenDto>.Succeeded(token);
    }

    private TokenDto CreateToken(Admin admin)
    {
        var accessTokenClaims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, admin.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("D")),
            new Claim("Id", admin.Id.ToString("D")),
            new Claim("TokenType", "access")
        };

        var refreshTokenClaims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, admin.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("D")),
            new Claim("Id", admin.Id.ToString("D")),
            new Claim("TokenType", "refresh")
        };

        var cert = _certificateProvider.Load();

        var utcNow = DateTime.UtcNow;
        
        var accessLifetimeInSeconds = _tokenValidationConfig.AccessLifetimeInSeconds;
        var accessExpiresDateTime = utcNow.AddSeconds(accessLifetimeInSeconds);

        var refreshLifetimeInSeconds = _tokenValidationConfig.RefreshLifetimeInSeconds;
        var refreshExpiresDateTime = utcNow.AddSeconds(refreshLifetimeInSeconds);

        var accessToken = new JwtSecurityToken(
            issuer: _tokenValidationConfig.ValidIssuer,
            audience: _tokenValidationConfig.ValidAudience,
            expires: accessExpiresDateTime,
            claims: accessTokenClaims,
            signingCredentials: new X509SigningCredentials(cert)
        );

        var refreshToken = new JwtSecurityToken(
            issuer: _tokenValidationConfig.ValidIssuer,
            audience: _tokenValidationConfig.ValidAudience,
            expires: refreshExpiresDateTime,
            claims: refreshTokenClaims,
            signingCredentials: new X509SigningCredentials(cert)
        );
        
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var encodedAccessToken = jwtSecurityTokenHandler.WriteToken(accessToken);
        var encodedRefreshToken = jwtSecurityTokenHandler.WriteToken(refreshToken);

        return new TokenDto(encodedAccessToken, encodedRefreshToken, accessToken.ValidTo, refreshToken.ValidTo,
            accessLifetimeInSeconds, refreshLifetimeInSeconds);
    }
}