using Microsoft.EntityFrameworkCore;
using Nhom7_N03_TrangWebQuanCafe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Quartz;
using Nhom7_N03_TrangWebQuanCafe.Classes;

var builder = WebApplication.CreateBuilder(args);

var cookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict
};

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

//builder.Services.AddQuartz(q =>
//{
//    q.UseMicrosoftDependencyInjectionScopedJobFactory();
//    // Just use the name of your job that you created in the Jobs folder.
//    var jobKey = new JobKey("UpdateDatabaseJob");
//    q.AddJob<UpdateDatabaseJob>(opts => opts.WithIdentity(jobKey));

//    q.AddTrigger(opts => opts
//        .ForJob(jobKey)
//        .WithIdentity("UpdateDatabaseJob-trigger")
//        //This Cron interval can be described as "run every minute" (when second is zero)
//        .WithCronSchedule("0 * * ? * *")
//    );
//});
//builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = new PathString("/Login/Forbidden");
    });

var connectionString = "Data Source=DESKTOP-JLDOEOK\\MAYTINH;Initial Catalog=CafeWebsite;Integrated Security=true;MultipleActiveResultSets=True";
builder.Services.AddDbContext<CafeWebsiteContext>(x => x.UseSqlServer(connectionString));

var app = builder.Build();

startup.Configure(app, app.Environment);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCookiePolicy(cookiePolicyOptions);

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
