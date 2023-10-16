using Microsoft.EntityFrameworkCore;
using MKDComm.communication.interfaces;
using Modelo.Tipos;
using SisWBeck.DB;
using SisWBeck.ViewModels;
using SisWBeck.Views;

using SisWBeck.Platforms;
using SisWBeck.Comm;

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
        builder.Services.AddTransient<IMainNavigationService, DialogService>();

        builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddSingleton<MainPage>();

		builder.Services.AddTransient<ConfiguracaoViewModel>();
		builder.Services.AddTransient<ConfiguracaoPage>();

        builder.Services.AddTransient<PesagemViewModel>();
        builder.Services.AddTransient<PesagemPage>();
		builder.Services.AddTransient<BluetoothHelper>();

		builder.Services.AddTransient<IHALCommFactory, CommFactory>();

        return builder.Build();
	}
}
