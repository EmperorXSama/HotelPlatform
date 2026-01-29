using System.IdentityModel.Tokens.Jwt;
using HotelManagement.BlazorServer;
using HotelManagement.BlazorServer.Components;
using HotelManagement.BlazorServer.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);




// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddBlazorServerServices(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/login", () => Results.Challenge(
    new Microsoft.AspNetCore.Authentication.AuthenticationProperties 
    { 
        RedirectUri = "/" 
    },
    [OpenIdConnectDefaults.AuthenticationScheme]
)).AllowAnonymous();

app.MapGet("/logout", async (
    HttpContext context,
    IConfiguration configuration) =>
{
    var keycloakSettings = configuration
        .GetSection(WebKeycloakSettings.SectionName)
        .Get<WebKeycloakSettings>()!;

    // Get the ID token for Keycloak logout
    var idToken = await context.GetTokenAsync("id_token");

    // Sign out of local cookie
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    // Build Keycloak logout URL
    var logoutUrl = $"{keycloakSettings.Authority}/protocol/openid-connect/logout";
    var postLogoutRedirectUri = $"{context.Request.Scheme}://{context.Request.Host}/";

    var keycloakLogoutUrl = string.IsNullOrEmpty(idToken)
        ? $"{logoutUrl}?post_logout_redirect_uri={Uri.EscapeDataString(postLogoutRedirectUri)}&client_id={keycloakSettings.ClientId}"
        : $"{logoutUrl}?id_token_hint={idToken}&post_logout_redirect_uri={Uri.EscapeDataString(postLogoutRedirectUri)}";

    return Results.Redirect(keycloakLogoutUrl);
}).RequireAuthorization();


app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();