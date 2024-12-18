using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BarberShop.Data;
using BarberShop.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure the database connection
builder.Services.AddDbContext<BarberDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection")));

// Configure Identity services
builder.Services.AddIdentity<Kullanici, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false; // No special characters required
    options.Password.RequiredLength = 3;             // Minimum length of 3
    options.Password.RequireUppercase = false;       // No uppercase letters required
    options.Password.RequireLowercase = false;       // No lowercase letters required
    options.Password.RequireDigit = false;           // No numbers required
    options.User.RequireUniqueEmail = true;          // Enforce unique email
})
.AddEntityFrameworkStores<BarberDbContext>()
.AddDefaultTokenProviders();

// Configure cookie settings for authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";                // Redirect to login page for unauthenticated users
    options.AccessDeniedPath = "/Account/AccessDenied";  // Redirect to access denied page
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);   // Session timeout (20 minutes)
    options.SlidingExpiration = true;                    // Sliding expiration enabled
});

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Session timeout (20 minutes)
    options.Cookie.HttpOnly = true;                // Enhance security
    options.Cookie.IsEssential = true;             // Cookie is essential for the app
});

// Add memory cache (required for session)
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Initialize roles and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await UserRoleInitializer.InitializeAsync(services); // Ensure roles and admin exist
}

// Configure error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");  // Use custom error page in production
    app.UseHsts();                           // Enable HSTS for HTTPS
}

// Middleware pipeline
app.UseHttpsRedirection();    // Redirect HTTP to HTTPS
app.UseStaticFiles();         // Serve static files from wwwroot

app.UseRouting();             // Enable endpoint routing

app.UseSession();             // Enable session middleware
app.UseAuthentication();      // Enable authentication middleware
app.UseAuthorization();       // Enable authorization middleware

// Map controller routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}");

// Run the application
app.Run();
