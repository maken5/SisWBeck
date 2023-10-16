using Modelo.Tipos;
using SisWBeck.DB;
using System.Diagnostics.CodeAnalysis;

namespace SisWBeck.ViewModels;

public partial class BaseViewModel : ObservableObject
{
	private string _titulo;
	protected SISWBeckContext context;
	protected IDialogService dialogService;

	protected bool IsModificado = false;

    protected bool Set<T>([NotNullIfNotNull(nameof(newValue))] ref T field, T newValue, [CallerMemberName] string? propertyName = null)
	{
        IsModificado |= SetProperty(ref field, newValue, propertyName);
		return IsModificado;
	}


    public BaseViewModel(SISWBeckContext context, 
						 IDialogService dialogService)
	{
		this.context = context;
		this.dialogService = dialogService;
	}

    public string Titulo
	{
		get =>_titulo; 
		set =>SetProperty(ref _titulo, value);
	}



}
