using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Fido2NetLib;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Strategies;
using Finbuckle.Utilities.AspNetCore;
using idpMultiTenant1.Data;
using idpMultiTenant1.FIDO2;
using idpMultiTenant1.Models;
using idpMultiTenant1.Providers;
using idpMultiTenant1.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Serilog;

namespace idpMultiTenant1
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            var migrationsAssembly = typeof(Program).Assembly.GetName().Name;

            builder.Services.AddDbContext<ApplicationDbContext>();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<Fido2UserTwoFactorTokenProvider>("FIDO2");

            builder.Services.Configure<Fido2Configuration>(builder.Configuration.GetSection("fido2"));
            builder.Services.AddScoped<Fido2Store>();

            builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
            builder.Services.AddRazorPages()
                .AddRazorPagesOptions(options =>
            {
                options.Conventions.Add(new MultiTenantPageRouteModelConvention());

                // Since we are using the route multitenant strategy we must add the
                // route parameter to the Pages conventions used by Identity.
                //options.Conventions.AddAreaFolderRouteModelConvention("Identity", "/Account", model =>
                //{
                //    foreach (var selector in model.Selectors)
                //    {
                //        selector.AttributeRouteModel.Template =
                //            AttributeRouteModel.CombineTemplates("{__tenant__}", selector.AttributeRouteModel.Template);
                //    }
                //});
            });

            builder.Services.DecorateService<LinkGenerator, AmbientValueLinkGenerator>(new List<string> { "__tenant__" });

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            
            var configurationStoreOptions = new Duende.IdentityServer.EntityFramework.Options.ConfigurationStoreOptions();
            builder.Services.AddSingleton(configurationStoreOptions);
            
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            builder.Services
                .AddIdentityServer(
                    options =>
                    {
                        options.Events.RaiseErrorEvents = true;
                        options.Events.RaiseInformationEvents = true;
                        options.Events.RaiseFailureEvents = true;
                        options.Events.RaiseSuccessEvents = true;
                        //options.UserInteraction.LoginUrl = "/__tenant__/Identity/Account/Login/Index";
                        //options.UserInteraction.LogoutUrl = "/__tenant__/Identity/Account/Logout";

                        // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                        options.EmitStaticAudienceClaim = true;
                    })
                .AddConfigurationStore(
                    options =>
                    {
                        options.ResolveDbContextOptions = (sp, dbContextOptions) =>
                        {
                            dbContextOptions.UseSqlServer(
                                builder.Configuration.GetConnectionString("ConfigurationStoreConnection"),
                                sqlOptions =>
                                {
                                    sqlOptions.MigrationsAssembly(migrationsAssembly);
                                    sqlOptions.EnableRetryOnFailure(
                                        maxRetryCount: 15,
                                        maxRetryDelay: TimeSpan.FromSeconds(30),
                                        errorNumbersToAdd: null);
                                });

                        };
                    })
                .AddOperationalStore(
                    options =>
                    {
                        options.ConfigureDbContext = b => b.UseSqlServer(
                            builder.Configuration.GetConnectionString("OperationalStoreConnection"),
                            sqlOptions =>
                            {
                                sqlOptions.MigrationsAssembly(migrationsAssembly);
                                sqlOptions.EnableRetryOnFailure(
                                    maxRetryCount: 15,
                                    maxRetryDelay: TimeSpan.FromSeconds(30),
                                    errorNumbersToAdd: null);
                            });

                        // this enables automatic token cleanup. this is optional.
                        options.EnableTokenCleanup = true;
                        options.RemoveConsumedTokens = true;
                    })
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<NovacProfileService>();

            //builder.Services.ConfigureApplicationCookie(options =>
            //{
            //    options.LoginPath = "/__tenant__/Identity/Account/Login/Index";
            //    options.LogoutPath = "/__tenant__/Identity/Account/Logout";
            //    options.AccessDeniedPath = "/__tenant__/Identity/Error";
            //});

            builder.Services.AddMultiTenant<TenantInfo>()
                .WithConfigurationStore()
                .WithRouteStrategy()
                .WithPerTenantAuthentication();

            builder.Services.AddAuthentication();

            return builder.Build();
        }



        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.Use(async (context, next) =>
            //{
            //    var mtc = context.GetMultiTenantContext<TenantInfo>();
            //    var tenant = mtc?.TenantInfo;
            //    if (tenant != null && mtc.StrategyInfo.StrategyType == typeof(BasePathStrategy))
            //    {
            //        context.Request.Path.StartsWithSegments("/" + tenant.Identifier, out var matched, out var newPath);
            //        context.Request.PathBase = Path.Join(context.Request.PathBase, matched);
            //        context.Request.Path = newPath;
            //    }

            //    await next.Invoke();
            //});

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseMultiTenant();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(options =>
            {
                options.MapControllers();
                options.MapControllerRoute("default", "{__tenant__=}/{controller=Home}/{action=Index}");
                //options.MapDefaultControllerRoute();
                options.MapRazorPages();
            });

            return app;
        }
    }
}