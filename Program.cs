using System.Text;
using FoodMarket.Data;
using FoodMarket.Models;
using FoodMarket.Repositories;
using FoodMarket.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.Services.AddCors(policy =>
{
    policy.AddPolicy("Default", builder =>
    {
        builder
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("Set-Cookie", "Content-Disposition")
            .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString), ServiceLifetime.Scoped);

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("JWT key missing");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

Console.WriteLine(jwtKey);
Console.WriteLine(jwtIssuer);
Console.WriteLine(jwtAudience);

builder.Services.AddAuthentication(options => {
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
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.HttpContext.Request.Cookies["accessToken"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            },
            // OnForbidden = async context =>
            // {
            //     var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
            //     var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
            //     
            //     
            //     var accessToken = context.Request.Cookies["accessToken"];
            //     var refreshToken = context.Request.Cookies["refreshToken"];
            //     
            //     var username = await tokenService.GetNameFromToken(accessToken);
            //
            //     var result = await authService.RefreshTokenAsync(new RefreshTokenRequest(username, refreshToken));
            //
            //     context.Response.Cookies.Append("accessToken", result.accessToken);
            //     context.Response.Cookies.Append("refreshToken", result.refreshToken);
            //
            //     context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            // }
        };
    });

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    options.Cookie.HttpOnly = true;

    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization();
// {
    // options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    // options.AddPolicy("UserPolicy", policy => policy.RequireRole("User", "Admin"));
// });

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IEmailSender, EmailSender>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    await EnsureAdminRoleAndUser(userManager, roleManager);
}

async Task EnsureAdminRoleAndUser(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
{
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    var email = "ismailqasimov71@gmail.com";
    var user = await userManager.FindByEmailAsync(email);

    if (user != null && !await userManager.IsInRoleAsync(user, "Admin"))
    {
        await userManager.AddToRoleAsync(user, "Admin");
        Console.WriteLine($"Пользователю {email} добавлена роль Admin");
    }
    else if (user == null)
    {
        Console.WriteLine($"Пользователь с email {email} не найден.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Default");
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
