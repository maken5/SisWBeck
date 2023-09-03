using Modelo.Entidades;
using Modelo.Tipos;
using SisWBeck.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SisWBeck.ViewModels;

public partial class MainViewModel : BaseViewModel
{

    public MainViewModel(SISWBeckContext context, IDialogService dialogService) : base(context, dialogService)
    {
    }

    [RelayCommand]
    public async Task AdicionarLote()
    {
        string lote = await dialogService.InputDialog("Novo lote de animais", "Digite o nome do novo lote");
        if (!String.IsNullOrWhiteSpace(lote))
        {
            Lotes l = new Lotes() { Nome = lote };
            context.Lotes.Add(l);
            await context.SaveChangesAsync();
            OnPropertyChanged(nameof(Lotes));
            OnPropertyChanged(nameof(Lote));
        }
    }

    [RelayCommand]
    public async Task RemoverLote()
    {
        if (this.Lote != null)
        {
            bool remover = await dialogService.InputAlert("Remover Lote?", $"Deseja remover o lote {Lote.Nome} criado em {Lote.Data}?");
            if (remover)
            {
                context.Lotes.Remove(this.Lote);
                await context.SaveChangesAsync(true);
                OnPropertyChanged(nameof(Lotes));
                OnPropertyChanged(nameof(Lote));
            }
        }
    }


    public List<Lotes> Lotes
    {
        get
        {
            return context.Lotes.ToList();
        }
    }

    [ObservableProperty]
    private Lotes lote;

}
