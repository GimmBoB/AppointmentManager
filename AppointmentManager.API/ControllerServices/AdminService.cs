using AppointmentManager.API.config;
using AppointmentManager.API.Models;
using AppointmentManager.API.Repositories;
using AppointmentManager.API.Security;
using AppointmentManager.API.Utilities;

namespace AppointmentManager.API.ControllerServices;

public class AdminService
{
    private readonly AdminRepository _repository;
    private readonly AdminConfiguration _configuration;

    public AdminService(AdminRepository repository, AdminConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<ApiResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var admin = await _repository.GetByIdAsync(id, ct);

        if (admin is null)
            return NotFoundApiResult.NotFound();

        return ItemApiResult<AdminDto>.Succeeded(MapToDto(admin));
    }

    public async Task<ApiResult> UpdateAsync(Guid id, AdminDto dto, CancellationToken ct)
    {
        var admin = await _repository.GetByIdAsync(id, ct);

        if (admin is null)
            return NotFoundApiResult.NotFound();

        var errors = Validate(dto);

        if (errors.Count > 0)
            return ApiResult.Failure(errors);

        admin.Name = dto.Name.Trim();
        admin.Email = dto.Email.Trim();

        var result = await _repository.UpdateAsync(admin, ct);

        return ItemApiResult<AdminDto>.Succeeded(MapToDto(result));
    }

    public async Task<ApiResult> UpdatePasswordAsync(Guid id, string password, CancellationToken ct)
    {
        var admin = await _repository.GetByIdAsync(id, ct);

        if (admin is null)
            return NotFoundApiResult.NotFound();

        if (string.IsNullOrWhiteSpace(password))
            return ApiResult.Failure(new []{"Password not set"});
        
        var encrypt = StringCipher.Encrypt(password, _configuration.SecretKey);
        admin.Password = encrypt;

        await _repository.UpdateAsync(admin, ct);

        return ApiResult.Succeeded();
    }

    private static ICollection<string> Validate(AdminDto dto)
    {
        var errors = new List<string>();

        if (!RegExUtil.IsValidEmail(dto.Email.Trim()))
            errors.Add($"'{dto.Email.Trim()}' is not a valid email");
        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add($"{nameof(dto.Name)} can not be empty");

        return errors;
    }

    private static AdminDto MapToDto(Admin admin)
    {
        return new AdminDto(admin.Id, admin.Name, admin.Email);
    }
}