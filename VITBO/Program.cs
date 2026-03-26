using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;
using VITBO.Services;

using VITBO.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<TokenExpirationFilter>();
});
builder.Services.AddSingleton<HttpService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromHours(8);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

builder.Services.AddHttpContextAccessor(); 
builder.Services.AddHttpClient("HttpClient", client =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];
    if (string.IsNullOrEmpty(baseUrl))
    {
        throw new Exception("ApiSettings:BaseUrl is missing in appsettings.json");
    }
    var fullUrl = baseUrl;
    if (!Uri.IsWellFormedUriString(fullUrl, UriKind.Absolute))
    {
        throw new Exception($"ApiSettings:BaseUrl must be an absolute URL. Current value: {fullUrl}");
    }
    client.BaseAddress = new Uri(fullUrl);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Add("User-Agent", "BackOffice-App");
});
//builder.Services.AddHttpClient<VITBO.Services.ApiService>();
builder.Services.AddScoped<VITBO.Services.ApiService>();
builder.Services.AddScoped<VITBO.Services.Interfaces.IAuthService, VITBO.Services.AuthService>();
builder.Services.AddScoped<VITBO.Services.Interfaces.IUsersService, VITBO.Services.UsersService>();
builder.Services.AddScoped<VITBO.Services.Interfaces.IAiVideoService, VITBO.Services.AiVideoService>();
builder.Services.AddScoped<VITBO.Services.Interfaces.IEventsService, VITBO.Services.EventsService>();
builder.Services.AddScoped<VITBO.Services.Interfaces.ITripsService, VITBO.Services.TripsService>();
builder.Services.AddScoped<VITBO.Services.Interfaces.IContentsService, VITBO.Services.ContentsService>();
builder.Services.AddScoped<VITBO.Services.Interfaces.IExpertsService, VITBO.Services.ExpertsService>();
builder.Services.AddScoped<VITBO.Services.Interfaces.IMediaService, VITBO.Services.MediaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseHttpMethodOverride(new HttpMethodOverrideOptions
{
    FormFieldName = "X-HTTP-Method-Override"
});

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
