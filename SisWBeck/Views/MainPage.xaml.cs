

using SisWBeck.Comm;

namespace SisWBeck.Views;

public partial class MainPage : ContentPage
{

    public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
        //VerificaPermissoes();
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await VerificaPermissoes();
    }






    private async Task<bool> VerificaPermissao<T>() where T : Permissions.BasePermission, new()
    {
        var check = await Permissions.CheckStatusAsync<T>();
        if (check != PermissionStatus.Granted)
        {
            await Permissions.RequestAsync<T>();
            check = await Permissions.CheckStatusAsync<T>();
        }
        return check == PermissionStatus.Granted;

        //var taskCheck = Permissions.CheckStatusAsync<T>();
        //taskCheck.Wait();
        //if (taskCheck.Result != PermissionStatus.Granted)
        //{
        //    var taskRequest = Permissions.RequestAsync<T>();
        //    taskRequest.Wait(); 
        //    taskCheck = Permissions.CheckStatusAsync<T>();
        //    taskCheck.Wait();
        //}
        //return taskCheck.Result == PermissionStatus.Granted;
    } 

    private async Task VerificaPermissoes()
    {
        await VerificaPermissao<Permissions.StorageRead>();
        await VerificaPermissao<Permissions.StorageWrite>();
        await VerificaPermissao<BluetoothPermissions>();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        //VerificaPermissoes();
    }
}
