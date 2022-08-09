using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace IntellitectTerminal.Web.Hangfire;

public class HangfireStartup : IStartupFilter
{
    private ILogger<HangfireStartup> _Logger;

    public HangfireStartup(ILogger<HangfireStartup> logger)
    {
        _Logger = logger;
    }

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.UseSwagger();

            next(app);

            try
            {
                RecurringJob.AddOrUpdate<RecurringJobService>(RecurringJobService
                    => RecurringJobService.RemoveExpiredUsers(), "0 1 * * *");
            }
            catch (Exception exception)
            {
                _Logger.LogError(exception, "An error occurred during Hangfire job initialization.");
            }
        };
    }
}
