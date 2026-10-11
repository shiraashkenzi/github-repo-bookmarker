using System.Text;
using GitApp.Api;
using GitApp.Api.Endpoints;
using GitApp.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
    ?? throw new InvalidOperationException("Missing 'Jwt' configuration.");
var demoUsers = builder.Configuration.GetSection("DemoUsers").Get<DemoUser[]>() ?? [];

builder.Services.AddSingleton(jwtOptions);
builder.Services.AddSingleton<IReadOnlyList<DemoUser>>(demoUsers);
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<SessionStore>();

builder.Services.AddHttpClient<GitHubService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["GitHub:BaseUrl"]!);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("GitApp");
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapRepoEndpoints();
app.MapBookmarkEndpoints();

app.Run();
