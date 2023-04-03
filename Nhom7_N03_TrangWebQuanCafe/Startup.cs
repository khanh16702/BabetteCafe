using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using Nhom7_N03_TrangWebQuanCafe.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Quartz;

public class Startup
{
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            // base Quartz scheduler, job and trigger configuration
        });

        // ASP.NET Core hosting
        services.AddQuartzServer(options =>
        {
            // when shutting down we want jobs to complete gracefully
            options.WaitForJobsToComplete = true;
        });

    }

    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        app.UseStaticFiles();
    }
}