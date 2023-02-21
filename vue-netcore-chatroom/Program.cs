using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using vue_netcore_chatroom.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Extensions.DependencyInjection;
using vue_netcore_chatroom.Services;
using System.Text;
using Microsoft.Extensions.Options;
using vue_netcore_chatroom.Hubs;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")
));


builder.Services.AddControllers();

builder.Services.AddAuthentication()
    .AddJwtBearer("FirebaseAuth", options =>
    {
        options.Authority = configuration["FirebaseAuth:Issuer"] + configuration["FirebaseAuth:Audience"];
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = configuration["FirebaseAuth:Issuer"] + configuration["FirebaseAuth:Audience"],
            ValidAudience = configuration["FirebaseAuth:Audience"],
        };

        options.Events = new JwtBearerEvents();
    })
    .AddJwtBearer("CustomJwtAuth", options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["CustomJwtAuth:SecretKey"])),
            ValidIssuer = configuration["CustomJwtAuth:Issuer"],
            ValidAudience = configuration["CustomJwtAuth:Audience"],
            ValidateLifetime = true,
            RequireExpirationTime = false,
            ClockSkew = new TimeSpan(0, 0, 30)
        };

        options.Events = new JwtBearerEvents();
    })
    .AddMicrosoftIdentityWebApi(configuration, "AzureAd", "AzureAdAuth");

builder.Services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes("AzureAdAuth", "FirebaseAuth", "CustomJwtAuth")
        .RequireAuthenticatedUser()
        .Build();
});

// Configure frontend built path
builder.Services.AddSpaStaticFiles(configuration => {
    configuration.RootPath = builder.Environment.IsDevelopment()
        ? "../client/" + builder.Configuration.GetValue<string>("Client:BuildFolder")
        : "wwwroot";
});

CorsPolicy corsPolicy = new CorsPolicyBuilder()
    .WithOrigins(new[] {
        "http://localhost:8080",
        "https://localhost:8081"
    })
    .AllowAnyHeader()
    .AllowCredentials()
    .AllowAnyMethod()
    .Build();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDev", corsPolicy);
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
    hubOptions.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
});

var app = builder.Build();

// Initializing the database with seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    using (var context = new ApplicationDbContext(services.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
    {
        try
        {
            await DbInitializer.Initialize(context, environment, configuration);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while initializing the database.");
        }
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors("AllowDev");

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/hubs/chat");

app.MapWhen(x => x.Request.Path.Value != null && !x.Request.Path.Value.StartsWith("/api") && !x.Request.Path.Value.StartsWith("/hubs"), builder =>
{
    builder.UseSpaStaticFiles();

    builder.UseSpa(spa =>
    {
        spa.Options.SourcePath = "client-app";
        if (environment.IsDevelopment())
        {
            spa.UseProxyToSpaDevelopmentServer(configuration.GetValue<string>("Client:DevUrl"));
        }
    });
});

app.Run();

