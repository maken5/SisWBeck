﻿using Microsoft.EntityFrameworkCore;
using MKDComm.communication.interfaces;
using Modelo.Tipos;
using SisWBeck.DB;
using SisWBeck.ViewModels;
using SisWBeck.Views;
using CommunityToolkit.Maui;
using SisWBeck.Platforms;
using SisWBeck.Comm;
using Microsoft.Extensions.DependencyInjection;

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
		builder.Services.AddSingleton<IShare>(Share.Default);

		//builder.Services.AddTransient<IFileSaver>

        return builder.Build();
	}
}
