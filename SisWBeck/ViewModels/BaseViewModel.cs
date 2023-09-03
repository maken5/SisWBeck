using Modelo.Tipos;
using SisWBeck.DB;

namespace SisWBeck.ViewModels;

public partial class BaseViewModel : ObservableObject
{
	private string _titulo;
	protected SISWBeckContext context;
	protected IDialogService dialogService;

	public BaseViewModel(SISWBeckContext context, IDialogService dialogService)
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
