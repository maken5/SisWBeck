﻿using Modelo.Entidades;
using Modelo.Tipos;
using SisWBeck.DB;

namespace SisWBeck.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private IDialogServicePesagem dialogServicePesagem;

    public MainViewModel(SISWBeckContext context, IDialogServicePesagem dialogService) : base(context, dialogService)
    {
        this.dialogServicePesagem = dialogService; 
    }

    [RelayCommand]
    public async Task AdicionarLote()
    {
        string lote = await dialogService.InputDialog("Novo lote de animais", "Digite o nome do novo lote");
        if (!String.IsNullOrWhiteSpace(lote))
        {
            Lotes l = new Lotes() { Nome = lote };
            context.Add(l);
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
                try 
                {
                    context.Remove(this.Lote);
                }catch (Exception ex)
                {
                    await dialogService.MessageError("Erro!", $"Não foi possível remover o lote {Lote.Nome}\r\n{ex.Message}");
                }
                OnPropertyChanged(nameof(Lotes));
                OnPropertyChanged(nameof(Lote));
            }
        }
        else
        {
            await dialogService.MessageError("Nenhum lote selecionado", "Selecione um lote para remover");
        }
    }

    [RelayCommand]
    public async Task Pesar()
    {
        if (Lote != null)
        {
            dialogServicePesagem.ShowPesagem(Lote);
        }
        else
        {
            await dialogService.MessageError("Nenhum lote selecionado", "Selecione um lote para efetuar pesagens");
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
