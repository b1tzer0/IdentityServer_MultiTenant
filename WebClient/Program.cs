using Finbuckle.MultiTenant;
using Finbuckle.Utilities.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using WebClient;
using WebClient.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorPages()
    .AddRazorPagesOptions(
        options =>
        {
            options.Conventions.Add(new MultiTenantPageRouteModelConvention());
        });

builder.Services.DecorateService<LinkGenerator, AmbientValueLinkGenerator>(new List<string> { "__tenant__" });

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = "https://localhost:5001/";

        options.ClientId = "interactive";
        options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
        options.ResponseType = "code";

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");

        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        //options.Events.OnRedirectToIdentityProvider = ctx =>
        //{
        //    //ctx.ProtocolMessage.AcrValues = $"idp:demoidsrv tenant:{ctx.Request.Host.Value}";
        //    ctx.ProtocolMessage.AcrValues = $"tenant:tenant-1";
        //    return Task.FromResult(0);
        //};
    });

builder.Services.AddMultiTenant<AppTenantInfo>()
    .WithConfigurationStore()
    .WithRouteStrategy()
    .WithPerTenantAuthentication();

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
app.UseMultiTenant();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{__tenant__=}/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapRazorPages().RequireAuthorization();
});

app.Run();
