using System;
using IdentityNetCore;
using IdentityNetCore.Data;
using IdentityNetCore.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var configuration = new Configuration(builder);

builder.Services.AddSingleton(configuration);
builder.Services.AddScoped<IEMailService, GMailService>();
builder.Services.AddDbContext<ApplicationDbContext>(
    o => o.UseSqlServer(
        configuration.IdentityDbContextConnectionString)
    );
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 3;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;

    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

    options.SignIn.RequireConfirmedEmail = true;
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Signin";
    options.AccessDeniedPath = "/Identity/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(10);
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MemberDep", p =>
    {
        p.RequireClaim("Department", "Tech").RequireRole("Member");
    });
    options.AddPolicy("AdminDep", p =>
    {
        p.RequireClaim("Department", "Tech").RequireRole("Admin");
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: default,
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();