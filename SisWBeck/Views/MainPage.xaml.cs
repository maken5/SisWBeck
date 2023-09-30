

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

    //private string GetErrorMessagePermissionNeeded(List<string> permissions)
    //{
    //    string mensagem = "Atenção!";
    //    bool read_msg = false;
    //    bool write_msg = false;
    //    foreach (string permission in permissions)
    //    {
    //        mensagem += "\r\n";
    //        switch (permission)
    //        {
    //            case Android.Manifest.Permission.ReadMediaAudio:
    //            case Android.Manifest.Permission.ReadMediaVideo:
    //            case Android.Manifest.Permission.ReadMediaImages:
    //            case Android.Manifest.Permission.ReadExternalStorage:
    //                if (!read_msg)
    //                {
    //                    mensagem += "Permissão para leitura de mídia (áudio, vídeo, imagens, armazenamento externo) é necessário para ler arquivos CSV de pesagens";
    //                }
    //                read_msg = true;
    //                break;
    //            case Android.Manifest.Permission.WriteExternalStorage:
    //                if (!write_msg)
    //                {
    //                    mensagem += "Permissão para escrita de mídia (áudio, vídeo, imagens, armazenamento externo) é necessário para salvar arquivos CSV de pesagens";
    //                }
    //                write_msg = true;
    //                break;
    //            case Android.Manifest.Permission.BluetoothConnect:
    //                mensagem += "Permissão de conexão com dispositivos (Bluetooth) é necessária para conectar à balanças";
    //                break;
    //            default:
    //                mensagem += permission + " é requirido pelo aplicativo";
    //                break;
    //        }
    //    }
    //    mensagem += "\r\n\r\n";
    //    mensagem += "Para ativar as permissões requeridas:\r\n-Vá em Settings - App\r\nSelecione SiwSBEck.Android\r\nSelecione Permissions\r\nLibere as permissões negadas\r\nFinalize a aplicação e abra novamente";
    //    return mensagem;
    //}

    protected override async void OnAppearing()
    {
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
