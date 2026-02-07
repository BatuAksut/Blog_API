using API.Mappings;
using API.Middlewares;
using DataAccess;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Microsoft.OpenApi.Models;
using Sieve.Services;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// Serilog Setup
// -----------------------------------------------------------------------------
var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// -----------------------------------------------------------------------------
// [FIXED] CORS Setup: Configurable from Environment Variables
// -----------------------------------------------------------------------------
var allowedOrigins = builder.Configuration["AllowedOrigins"]?
    .Split(",", StringSplitOptions.RemoveEmptyEntries)
    .Select(o => o.Trim())
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            if (allowedOrigins != null && allowedOrigins.Any())
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            }
            else
            {
                // Fallback for safety (e.g. Localhost) if env var is missing
                policy.WithOrigins("http://localhost:5173")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            }
        });
});

builder.Services.AddScoped<ISieveProcessor, SieveProcessor>();

// -----------------------------------------------------------------------------
// Repositories
// -----------------------------------------------------------------------------
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

// -----------------------------------------------------------------------------
// [FIXED] JWT Key Security
// -----------------------------------------------------------------------------
var jwtKey = builder.Configuration["Jwt:Key"];

// Enforce at least 32 chars (256 bits) for security
if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException("JWT Key is missing or too short. It must be at least 32 characters long. Please set 'Jwt__Key' in environment variables.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey))
        };

        opt.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsJsonAsync(new { message = "You are not authenticated." });
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsJsonAsync(new { message = "You do not have permission to perform this action." });
            }
        };
    });

// -----------------------------------------------------------------------------
// Authorization Policies
// -----------------------------------------------------------------------------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Reader", policy => policy.RequireRole("Reader"));
    options.AddPolicy("Writer", policy => policy.RequireRole("Writer"));
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

// -----------------------------------------------------------------------------
// [FIXED] Identity Options: Strong Password Policy
// -----------------------------------------------------------------------------
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;           // Rakam zorunlu
    options.Password.RequiredLength = 10;           // En az 10 karakter
    options.Password.RequireNonAlphanumeric = true; // Sembol zorunlu (!?*.)
    options.Password.RequireUppercase = true;       // Büyük harf zorunlu
    options.Password.RequireLowercase = true;       // Küçük harf zorunlu
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// -----------------------------------------------------------------------------
// Swagger Setup
// -----------------------------------------------------------------------------
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Blog Web API",
        Version = "v1",
        Description = "This API provides the core backend services for a blog platform.",
        Contact = new OpenApiContact
        {
            Name = "Batuhan Aksut",
            Email = "batuhanaksut@hotmail.com",
            Url = new Uri("https://github.com/batuaksut")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter your JWT token.\n\r\rExample: 12345abcdef",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddDataProtection();

// -----------------------------------------------------------------------------
// Database Configuration with Retry Logic
// -----------------------------------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BlogAuthConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        }));

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole<Guid>>()
    .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("Blog")
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// -----------------------------------------------------------------------------
// Static Files Setup
// -----------------------------------------------------------------------------
var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
if (!Directory.Exists(uploadPath))
{
    Directory.CreateDirectory(uploadPath);
}

// -----------------------------------------------------------------------------
// Environment Setup
// -----------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

// Updated CORS Policy Usage
app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// -----------------------------------------------------------------------------
// Automatic Database Migration
// -----------------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var roles = new[] { "Reader", "Writer", "Admin" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }
    catch (Exception ex)
    {
        var loggerService = services.GetRequiredService<ILogger<Program>>();
        loggerService.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();