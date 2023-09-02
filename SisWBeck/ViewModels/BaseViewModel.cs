namespace SisWBeck.ViewModels;

public partial class BaseViewModel : ObservableObject
{
	private string _titulo;

	public string Titulo
	{
		get =>_titulo; 
		set =>SetProperty(ref _titulo, value);
	}

}
