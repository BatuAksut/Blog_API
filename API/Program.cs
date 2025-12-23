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

// FIXME: the README.md still has plenty of errors. Make sure to have a linter or formatter to work with Markdown files.
// FIXME: the JWT secret key should not be put in plain appsettings.json. Start by moving it to environment variables.
// Fixed jwt and jwt key issue.
// TODO: write unit tests. 
// Unit tests are still pending.

var builder = WebApplication.CreateBuilder(args);

// Serilog Setup
var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// CORS Setup
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowLocalhost5173",
          policy => policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

builder.Services.AddScoped<ISieveProcessor, SieveProcessor>();

// Repositories
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 16)
{
    throw new InvalidOperationException("JWT Key is missing or too short. Please set 'Jwt__Key' in environment variables.");
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

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("Reader", policy => policy.RequireRole("Reader"));
  options.AddPolicy("Writer", policy => policy.RequireRole("Writer"));
  options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

// Identity Options
builder.Services.Configure<IdentityOptions>(options =>
{
  options.Password.RequireDigit = false;
  options.Password.RequiredLength = 6;
  options.Password.RequireNonAlphanumeric = false;
  options.Password.RequireUppercase = false;
  options.Password.RequireLowercase = false;
  options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger Setup
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

  // XML Comments (if exists)
  var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
  var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
  if (File.Exists(xmlPath))
  {
    options.IncludeXmlComments(xmlPath);
  }
});

builder.Services.AddDataProtection();

// -----------------------------------------------------------------------------
// [FIX] Database Configuration with Retry Logic (For Docker Race Conditions)
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

// Static Files Setup
var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
if (!Directory.Exists(uploadPath))
{
  Directory.CreateDirectory(uploadPath);
}

// Environment Setup
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowLocalhost5173");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// -----------------------------------------------------------------------------
// [FIX] Automatic Database Migration (Fixes "dotnet ef" issues)
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