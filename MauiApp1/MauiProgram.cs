using IdentityModel.OidcClient;
namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddSingleton(new OidcClient(new()
            {
                Authority = "https://gamma.megasyshms.com:5001/tenant-1",
                ClientId = "Portfolio.Client",
                ClientSecret = ".S<K2P,v7)w-]2f*VQ",
                Scope = "openid profile",
                RedirectUri = "myapp://callback",

                Browser = new MauiAuthenticationBrowser()
            }));

            return builder.Build();
        }
    }
}