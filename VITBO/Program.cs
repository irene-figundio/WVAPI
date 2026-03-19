using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromHours(8);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<VITBO.Services.ApiService>();
builder.Services.AddScoped<VITBO.Services.Interfaces.IAuthService, VITBO.Services.AuthService>();
builder.Services.AddScoped<VITBO.Services.Interfaces.IUsersService, VITBO.Services.UsersService>();
builder.Services.AddScoped<VITBO.Services.Interfaces.IAiVideoService, VITBO.Services.AiVideoService>();
builder.Services.AddScoped<VITBO.Services.Interfaces.IEventsService, VITBO.Services.EventsService>();
builder.Services.AddScoped<VITBO.Services.Interfaces.IContentsService, VITBO.Services.ContentsService>();

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

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
