using Abp.Domain.Uow;
using AI_Integration.Controllers;
using AI_Integration.DataAccess;
using AI_Integration.DataAccess.Database.Repositories;
using AI_Integration.Helpers;
using AI_Integration.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Aggiungi i servizi al contenitore.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Vitinerario API", Version = "v1" });
    c.CustomSchemaIds(type => type.FullName);

    // Configurazione JWT per Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Inserisci SOLO il token JWT. Non inserire 'Bearer ' davanti."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddCors(options =>
{   
    options.AddPolicy("AllowAnyOrigin", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<OpenAISettings>(
    builder.Configuration.GetSection("OpenAI")
);
//builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddSingleton<VideoHelper>();
builder.Services.AddSingleton<PromptModel>();
builder.Services.AddScoped<IAuthService, AuthService>();

// File Upload Services
builder.Services.AddSingleton<AI_Integration.Services.FileUpload.Interfaces.IProgressiveResolver, AI_Integration.Services.FileUpload.Implementations.ProgressiveResolver>();
builder.Services.AddScoped<AI_Integration.Services.FileUpload.Interfaces.IStorageMappingService, AI_Integration.Services.FileUpload.Implementations.StorageMappingService>();
builder.Services.AddScoped<AI_Integration.Services.FileUpload.Interfaces.IFileStorageService, AI_Integration.Services.FileUpload.Implementations.FileStorageService>();
builder.Services.AddScoped<AI_Integration.Services.FileUpload.Interfaces.IImageConversionService, AI_Integration.Services.FileUpload.Implementations.ImageConversionService>();
builder.Services.AddScoped<AI_Integration.Services.FileUpload.Interfaces.IUploadNamingStrategy, AI_Integration.Services.FileUpload.Implementations.EventNamingStrategy>();
builder.Services.AddScoped<AI_Integration.Services.FileUpload.Interfaces.IUploadNamingStrategy, AI_Integration.Services.FileUpload.Implementations.ContentNamingStrategy>();
builder.Services.AddScoped<AI_Integration.Services.FileUpload.Interfaces.IUploadNamingStrategy, AI_Integration.Services.FileUpload.Implementations.HeroNamingStrategy>();
builder.Services.AddScoped<AI_Integration.Services.FileUpload.Interfaces.IUploadNamingStrategy, AI_Integration.Services.FileUpload.Implementations.PodcastNamingStrategy>();
builder.Services.AddScoped<AI_Integration.Services.FileUpload.Interfaces.IParentResolver, AI_Integration.Services.FileUpload.Implementations.ParentResolver>();
builder.Services.AddScoped<AI_Integration.Services.FileUpload.Interfaces.IFileUploadService, AI_Integration.Services.FileUpload.Implementations.FileUploadService>();

// add db context
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddScoped<AI_Integration.DataAccess.Database.Repositories.interfaces.IUnitOfWork, UnitOfWork>();


var key = Encoding.ASCII.GetBytes("Vml0aW5lcmFyaW9AMjAyNCxXZWJBcGlAMjAyNC0hY3JlYXppb25lVG9rZW4hVmVyc2lvbmUwLjEuMS58QXV0aG9yOkBHcmFsZGV2LmNvbUBJcmVuZUZpZ3VuZGlvQDIwMjQ");

// Bind options
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();


// Configura l'autenticazione JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "Vitinerario_API",
            ValidAudience = "Vitinerario_API",
            IssuerSigningKey = new SymmetricSecurityKey(key),
        };

        // Aggiungi un evento per gestire la verifica dell'autorizzazione
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError("Authentication failed: {Message}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Token validated for user: {User}", context.Principal?.Identity?.Name);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning("Authentication challenge triggered: {Error}, {ErrorDescription}", context.Error, context.ErrorDescription);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

var webEnvironment = app.Services.GetRequiredService<IWebHostEnvironment>();
webEnvironment.WebRootPath = Path.Combine(webEnvironment.ContentRootPath, "wwwroot");
app.UseStaticFiles(); // wwwroot

// Serve images from external folder
string? configuredPath = builder.Configuration["FileStorage:BasePhysicalPath"];
string basePhysicalPath;

// Fallback logic if path is missing or looks like a URL
if (string.IsNullOrEmpty(configuredPath) || configuredPath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
{
    basePhysicalPath = Path.Combine(builder.Environment.ContentRootPath, "VitinerarioImages");
}
else
{
    basePhysicalPath = configuredPath;
}

// Ensure directory exists with absolute path
if (!Path.IsPathRooted(basePhysicalPath))
{
    basePhysicalPath = Path.GetFullPath(basePhysicalPath);
}

if (!Directory.Exists(basePhysicalPath))
{
    try
    {
        Directory.CreateDirectory(basePhysicalPath);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"WARNING: Could not create images directory {basePhysicalPath}: {ex.Message}");
        // Ultimate fallback to wwwroot if creation failed
        basePhysicalPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "Images");
        if (!Directory.Exists(basePhysicalPath)) Directory.CreateDirectory(basePhysicalPath);
    }
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(basePhysicalPath),
    RequestPath = "/Images"
});

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".mkv"] = "video/x-matroska";
provider.Mappings[".mov"] = "video/quicktime";
provider.Mappings[".avi"] = "video/x-msvideo";
provider.Mappings[".flv"] = "video/x-flv";
provider.Mappings[".wmv"] = "video/x-ms-wmv";
provider.Mappings[".mp4"] = "video/mp4";



//app.UseStaticFiles(new StaticFileOptions
//{
//    RequestPath = "/Media/portrait",
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(app.Environment.WebRootPath, "MediaStore", "Portrait")
//    ),
//    ContentTypeProvider = provider
//});

//// /Media/landscape -> wwwroot/MediaStore/Landscape
//app.UseStaticFiles(new StaticFileOptions
//{
//    RequestPath = "/Media/landscape",
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(app.Environment.WebRootPath, "MediaStore", "Landscape")
//    ),
//    ContentTypeProvider = provider
//});

// Configura il pipeline della richiesta HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseAuthentication();
}
app.UseRouting();
app.UseCors("AllowAnyOrigin");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();




bool IsAuthorized(JsonWebToken token)
{

    // Verifica che il token non sia scaduto
    if (token.ValidTo < DateTime.UtcNow)
    {
        // Il token è scaduto
        return false;
    }

    // Verifica il claim personalizzato
    var customClaim = token.Claims.FirstOrDefault(c => c.Type == "custom_claim")?.Value;
    if (customClaim != "V!tin3rar!0@2024-allowed")
    {
        // Il claim personalizzato non ha il valore desiderato
        return false;
    }

    // Ritorna true se il token è valido e il claim personalizzato ha il valore desiderato
    return true;
}

