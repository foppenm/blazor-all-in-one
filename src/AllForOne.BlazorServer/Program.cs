using AllForOne.BlazorServer.Services;
using AllForOne.Models.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{builder.Configuration["AzureAdJwt:Instance"]}{builder.Configuration["AzureAdJwt:Domain"]}/{builder.Configuration["AzureAdJwt:SignUpSignInPolicyId"]}/v2.0/";
        options.Audience = builder.Configuration["AzureAdJwt:ClientId"];
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                // NOTE: You can optionally take action when the OAuth 2.0 bearer token was validated.
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                // NOTE: You can optionally take action when the OAuth 2.0 bearer token was rejected.
                return Task.CompletedTask;
            }
        };
    })
    .AddMicrosoftIdentityWebApp(builder.Configuration);

builder.Services.AddControllersWithViews()
.AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    // by default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();

// Link the interface from the razor to an actual implementation for wasm
builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
