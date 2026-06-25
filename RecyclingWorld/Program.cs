using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecyclingWorld.Data;
using RecyclingWorld.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDistributedMemoryCache(); // Adds a distributed in-memory cache service to the application, which can be used for caching data across multiple requests and sessions. This is particularly useful for improving performance by storing frequently accessed data in memory, reducing the need for repeated database queries or expensive computations.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Sets the idle timeout for sessions to 30 minutes, meaning that if a user is inactive for 30 minutes, their session will expire and they will need to log in again. This helps enhance security by limiting the duration of user sessions and reducing the risk of unauthorized access if a user forgets to log out.
    options.Cookie.HttpOnly = true; // Configures the session cookie to be HTTP-only, which means that it cannot be accessed or modified by client-side scripts. This helps protect against cross-site scripting (XSS) attacks by preventing malicious scripts from stealing or manipulating session cookies.
    options.Cookie.IsEssential = true; // Marks the session cookie as essential, indicating that it is necessary for the application to function properly. This is important for compliance with privacy regulations, as it allows the application to set cookies without requiring explicit user consent, while still ensuring that essential functionality is maintained.
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;// Ensures that the session cookie is only transmitted over secure HTTPS connections, enhancing the security of the application by preventing the cookie from being sent over unencrypted HTTP connections where it could be intercepted by attackers.
});
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Adds authentication middleware to the request pipeline, enabling the application to authenticate users based on their credentials (e.g., username and password) and establish their identity. This is essential for securing access to protected resources and ensuring that only authorized users can perform certain actions within the application.
app.UseAuthorization();

app.UseSession(); // Adds session middleware to the request pipeline, enabling the application to manage user sessions and store session data across multiple requests. This allows for features such as user authentication, shopping carts, and other stateful interactions that require maintaining information about the user's session throughout their interaction with the application.

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
