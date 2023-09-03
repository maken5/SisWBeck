using Microsoft.EntityFrameworkCore;
using Modelo.Tipos;
using SisWBeck.DB;
using SisWBeck.ViewModels;
using SisWBeck.Views;

namespace SisWBeck;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddDbContext<SISWBeckContext>(options => options.UseSqlite(Constantes.ConnectionString));

		builder.Services.AddTransient<IDialogService, DialogService>();
        builder.Services.AddTransient<IDialogServicePesagem, DialogService>();

        builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddSingleton<MainPage>();

		builder.Services.AddTransient<ConfiguracaoViewModel>();
		builder.Services.AddTransient<ConfiguracaoPage>();

        builder.Services.AddTransient<PesagemViewModel>();
        builder.Services.AddTransient<PesagemPage>();

        return builder.Build();
	}
}
