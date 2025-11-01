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

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowLocalhost5173",
          policy => policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});




builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
     opt.TokenValidationParameters = new TokenValidationParameters
     {
       ValidateIssuer = true,
       ValidateAudience = true,
       ValidateLifetime = true,
       ValidateIssuerSigningKey = true,
       ValidIssuer = builder.Configuration["Jwt:Issuer"],
       ValidAudience = builder.Configuration["Jwt:Audience"],
       // FIXME: fix this warning, in general all of the other warnings must be treated as errors.
       IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
     }
    );

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("Reader", policy => policy.RequireRole("Reader"));
  options.AddPolicy("Writer", policy => policy.RequireRole("Writer"));
  options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});






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


builder.Services.AddSwaggerGen(options =>
{

  options.SwaggerDoc("v1", new OpenApiInfo
  {
    Title = "Blog Web API",
    Version = "v1",
    Description = "This API provides the core backend services for a blog platform. " +
                "It manages user registration (Register) and login (Login) operations. " +
                "Authenticated users can create new blog posts (`BlogPostsController`) or comments (`CommentsController`), " +
                "and update or delete their own content. " +
                "Users with the Admin role can delete any content.",
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
    Description = "Please enter 'Bearer' followed by a space and then your JWT token.\n\r\rExample: 'Bearer 12345abcdef'",
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
  options.IncludeXmlComments(xmlPath);
});




builder.Services.AddDataProtection();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BlogAuthConnection")));

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole<Guid>>()
    .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("Blog")
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


var app = builder.Build();


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

app.Run();
