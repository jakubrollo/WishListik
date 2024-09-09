using Microsoft.EntityFrameworkCore;
using WishListikAPI.Data;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Register application services
builder.Services.AddScoped<UserService>(); // Adds the UserService to the dependency injection container
builder.Services.AddScoped<TokenService>(); // Adds the TokenService to the dependency injection container

// Configure the database context with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    // Configure cookie-based authentication
    options.Cookie.Name = "WishListik"; // Name of the authentication cookie
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddGoogle(options =>
{
    // Configure Google authentication
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!; // Google Client ID from configuration
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!; // Google Client Secret from configuration
    options.Scope.Add("openid"); // Request OpenID scope
    options.Scope.Add("profile"); // Request profile scope
    options.Scope.Add("email"); // Request email scope
    options.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
    {
        OnTicketReceived = context =>
        {
            // Handle events when a ticket is received
            return Task.CompletedTask;
        },
        OnAccessDenied = context =>
        {
            // Handle events when access is denied
            return Task.CompletedTask;
        }
    };
})
.AddJwtBearer(options =>
{
    // Configure JWT Bearer authentication
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"], // Valid issuer from configuration
        ValidAudience = builder.Configuration["JwtSettings:Audience"], // Valid audience from configuration
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)), // Signing key from configuration
        ClockSkew = TimeSpan.Zero
    };
});

// Configure authorization services
builder.Services.AddAuthorization(options =>
{
    // Define authorization policies here (currently commented out)
    // options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
    // options.AddPolicy("RequireUser", policy => policy.RequireClaim(ClaimTypes.NameIdentifier));
});

// Configure JSON serialization settings for controllers
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; // Prevent circular references
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Use property names as defined in models
});

// Configure Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware for development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enable Swagger UI for API exploration
    app.UseSwaggerUI(); // Configure Swagger UI
}

// Configure middleware for HTTPS redirection and authentication
app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseAuthentication();
app.UseAuthorization(); 

// Map controller routes and default route
app.MapControllers();
app.MapGet("/", () => "Welcome to Wishlistik"); // Default route

app.Run(); // Run the application
