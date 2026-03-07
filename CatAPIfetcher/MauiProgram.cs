using Microsoft.Extensions.Logging;
using CatAPIfetcher.Services;
using CatAPIfetcher.Views;

namespace CatAPIfetcher
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

            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<CatAPIservice>();
            builder.Services.AddSingleton<DatabaseService>();

            builder.Services.AddTransient<CatsListPage>();
            builder.Services.AddTransient<MyCatsPage>();
            builder.Services.AddTransient<CatDetailPage>();
            builder.Services.AddTransient<AddCatPage>();
            builder.Services.AddTransient<SettingsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}