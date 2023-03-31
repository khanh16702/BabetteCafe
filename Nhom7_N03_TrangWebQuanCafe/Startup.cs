using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using Nhom7_N03_TrangWebQuanCafe.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class Startup
{
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services)
    {

    }

    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        app.UseStaticFiles();
    }
}