using Asp.Versioning;
using Backend.Extensions;
using BugTrackerBackend;
using BugTrackerBackend.ExternalAPIs;
using BugTrackerBackend.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
const string domain = "https://firebranchdev.au.auth0.com/";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.Authority = domain;
    options.Audience = "https://bugtracker-backend/";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
    options.MapInboundClaims = false;
});

builder.Services.AddAuthorizationBuilder();

builder.Services.AddControllers();

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
});

builder.Services.AddBackendServices();

builder.Services.AddHttpClient();

Auth0Options auth0Options = new();
builder.Configuration.GetSection(nameof(Auth0Options)).Bind(auth0Options);
builder.Services.AddSingleton(c => new Auth0ManagementAPI(auth0Options, c.GetRequiredService<IHttpClientFactory>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
