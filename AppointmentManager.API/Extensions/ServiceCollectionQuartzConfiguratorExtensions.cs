using AppointmentManager.API.config;
using Quartz;

namespace AppointmentManager.API.Extensions;

public static class ServiceCollectionQuartzConfiguratorExtensions
{
    // example cron expression     "0 0 8 ? * SUN *"    =>  Every Sunday at 8 in the morning
    public static void AddJobAndTrigger<T>(
        this IServiceCollectionQuartzConfigurator quartz,
        IConfiguration configuration)
        where T : IJob
    {

        var quartzConfig = new QuartsConfig();
        configuration.GetSection(nameof(QuartsConfig)).Bind(quartzConfig);

        var jobName = typeof(T).Name;

        var cronSchedule = quartzConfig.Jobs.SingleOrDefault(j => j.Name.Equals(jobName))?.WithCronSchedule;

        if (string.IsNullOrWhiteSpace(cronSchedule))
        {
            throw new Exception($"No Quartz.NET cron expression found for job {jobName}");
        }

        var jobKey = new JobKey(jobName);
        quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

        quartz.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity(jobName + "-trigger")
            .WithCronSchedule(cronSchedule));
    }
}