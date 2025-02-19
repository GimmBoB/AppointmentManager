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

    public async Task<Result> GetByIdAsync(Guid id)
    {
        var admin = await _repository.GetByIdAsync(id);

        if (admin is null)
            return Result.NotFound();

        return ItemResult<AdminDto>.Succeeded(new AdminDto(id, admin.Name!, admin.Email!));
    }

    public async Task<Result> UpdateAsync(Guid id, AdminDto dto)
    {
        var admin = await _repository.GetByIdAsync(id);

        if (admin is null)
            return Result.NotFound();

        var errors = Validate(dto);

        if (errors.Count > 0)
            return Result.Failure(errors);

        admin.Name = dto.Name.Trim();
        admin.Email = dto.Email.Trim();

        await _repository.UpdateAsync(admin);

        return ItemResult<AdminDto>.Succeeded(dto);
    }

    public async Task<Result> UpdatePasswordAsync(Guid id, string password)
    {
        var admin = await _repository.GetByIdAsync(id);

        if (admin is null)
            return Result.NotFound();
        
        var encrypt = StringCipher.Encrypt(password, _configuration.SecretKey);
        admin.Password = encrypt;

        await _repository.UpdateAsync(admin);

        return Result.Succeeded();
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
}