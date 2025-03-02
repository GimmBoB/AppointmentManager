using AppointmentManager.API.Extensions;
using AppointmentManager.API.Models;
using AppointmentManager.API.Repositories;
using AppointmentManager.Shared;

namespace AppointmentManager.API.ControllerServices;
public class AppointmentExtensionService
{
    private readonly AppointmentExtensionRepository _repository;
    private readonly AppointmentRepository _appointmentRepository;

    public AppointmentExtensionService(AppointmentExtensionRepository repository, AppointmentRepository appointmentRepository)
    {
        _repository = repository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<ApiResult> AddFileAsync(Guid appointmentId, IFormFile file, CancellationToken ct)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, ct);
        if (appointment is null)
            return NotFoundApiResult.NotFound();

        var path = GetOrCreateAppointmentImagesFolderPath();
        var explicitImagePath = GetOrCreateAppointmentImagePath(path, appointment);

        var filePath = await SaveFileAsync(file, explicitImagePath);

        await _repository.AddAsync(new AppointmentExtension { AppointmentId = appointmentId, FilePath = filePath }, ct);
        
        return ApiResult.Succeeded();
    }

    public async Task<ImageInfoDto?> GetImageInfoAsync(Guid id, CancellationToken ct)
    {
        var extension = await _repository.GetByIdAsync(id, ct);
        

        var result = extension is not null 
            ? CreateImageInfoDto(extension)
            : default;

        return result;
    }

    private ImageInfoDto CreateImageInfoDto(AppointmentExtension extension) => new(extension.FilePath,
        $"image/{Path.GetExtension(extension.FilePath).SubstringFromChar('.')}");

    private static string GetOrCreateAppointmentImagesFolderPath()
    {
        var assemblyFolderPath = new AppInfo().GetAssemblyFolderPath();
        var dataFolderPath = Path.Combine(assemblyFolderPath, "Data");
        var assetsFolderPath = Path.Combine(dataFolderPath, "Assets");
        var projectImagesFolderPath = Path.Combine(assetsFolderPath, "AppointmentImages");
        
        var directoryInfo = new DirectoryInfo(projectImagesFolderPath);
        if (!directoryInfo.Exists)
            Directory.CreateDirectory(projectImagesFolderPath);

        return projectImagesFolderPath;
    }

    private static string GetOrCreateAppointmentImagePath(string path, Appointment appointment)
    {
        var appointmentPath = Path.Combine(path, appointment.Id.ToString("N"));
        
        var directoryInfo = new DirectoryInfo(appointmentPath);
        if (!directoryInfo.Exists)
            Directory.CreateDirectory(appointmentPath);

        return appointmentPath;
    }

    private static async Task<string> SaveFileAsync(IFormFile file, string explicitImagePath)
    {
        var filePath = Path.Combine(explicitImagePath, file.FileName);
        await using Stream fileStream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(fileStream);

        return filePath;
    }
}