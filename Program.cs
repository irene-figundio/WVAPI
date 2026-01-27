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
using System.IdentityModel.Tokens.Jwt;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Aggiungi i servizi al contenitore.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen();
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
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
        };

        // Aggiungi un evento per gestire la verifica dell'autorizzazione
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                // Eseguire la logica di autorizzazione qui
                // Per esempio, verifica altre informazioni contenute nel token
                if (context.SecurityToken is JsonWebToken token)
                {
                    //controlla eventuali restrizioni specifiche dell'applicazione
                    var tok = context.SecurityToken as JsonWebToken;

                    // Esempio: verifica se l'utente ha l'autorizzazione ad accedere a una risorsa specifica
                    if (!IsAuthorized(tok))
                    {
                        context.Fail("Unauthorized");
                    }
                }
                else
                {
                    // Gestisci il caso in cui SecurityToken non è un JwtSecurityToken
                    context.Fail("Invalid token");
                }
              

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
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".mkv"] = "video/x-matroska";
provider.Mappings[".mov"] = "video/quicktime";
provider.Mappings[".avi"] = "video/x-msvideo";
provider.Mappings[".flv"] = "video/x-flv";
provider.Mappings[".wmv"] = "video/x-ms-wmv";
provider.Mappings[".mp4"] = "video/mp4";



app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/Media/portrait",
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.WebRootPath, "MediaStore", "Portrait")
    ),
    ContentTypeProvider = provider
});

// /Media/landscape -> wwwroot/MediaStore/Landscape
app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/Media/landscape",
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.WebRootPath, "MediaStore", "Landscape")
    ),
    ContentTypeProvider = provider
}); 

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

