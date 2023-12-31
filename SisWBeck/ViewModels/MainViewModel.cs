﻿using Microsoft.EntityFrameworkCore;
using Modelo.Entidades;
using Modelo.Tipos;
using SisWBeck.Converter;
using SisWBeck.DB;

namespace SisWBeck.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private IDialogServicePesagem dialogServicePesagem;
    private IShare share;
    public MainViewModel(SISWBeckContext context, 
                         IDialogServicePesagem dialogService,
                         IShare share) : base(context, dialogService)
    {
        this.dialogServicePesagem = dialogService;
        this.share = share; 
    }


    public async Task UpdateLotesList()
    {
        this.Lotes = await context.Lotes.ToListAsync();
        this.Lote = null;
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
            await UpdateLotesList();
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
        IsBusy = true;
        if (Lote != null)
        {
            if (Lote.Pesagens == null )
            {
                var pesagem = await context.Pesagens.Where(p => p.LoteId == Lote.Id).ToListAsync();
                int nr_pesagem;
                if (pesagem == null) pesagem = new List<Pesagens>();
                if (pesagem.Any())
                    nr_pesagem = pesagem.Max(p => p.NrPesagem);
                else
                    nr_pesagem = 1;
                Lote.Pesagens = pesagem;
                if (Lote.NrPesagem < nr_pesagem)
                {
                    Lote.NrPesagem = nr_pesagem;
                }
            }
            if (Lote.UltimaDataPesagem == null)
            {
                if (Lote.NrPesagem<=0) Lote.NrPesagem = 1;
            }
            else
            {
                int nrDiasDesdeUltimaPesagem = (int)(DateTime.Now.Date - Lote.UltimaDataPesagem.Value.Date).TotalDays;
                if (nrDiasDesdeUltimaPesagem > 1 &&
                    (Lote.UltimoNrPesagem ?? 0) >= Lote.NrPesagem)
                {
                    bool incrementarNrPesagem = await dialogService.InputAlert("Nova Pesagem?",
                        $"O lote {Lote.Nome} está na sessão de pesagem número {Lote.NrPesagem}.\r\n A última pesagem foi feita em {Lote.UltimaDataPesagem.Value.Date}\r\n\r\nDeseja abrir uma nova sessão de pesagem ou continuar a sessão de pesagem anterior?",
                        $"Iniciar Pesagem {Lote.NrPesagem + 1}", $"Continuar Pesagem {Lote.NrPesagem}");
                    if (incrementarNrPesagem)
                    {
                        Lote.NrPesagem++;
                        await this.context.SaveChangesAsync();
                    }
                }
            }
            await dialogServicePesagem.ShowPesagem(Lote);
        }
        else
        {
            await dialogService.MessageError("Nenhum lote selecionado", "Selecione um lote para efetuar pesagens");
        }
        IsBusy = false;
    }

    [RelayCommand]
    public async Task CompartilharLote()
    {
        if (Lote != null)
        {
            string file;
            if (this.context.GetConfig().ExportarSomentePesagensUltimoNrPesagem)
                file = await SalvarUltimaPesagem(Lote);
            else
                file = await SalvarUltimas5Pesagens(Lote);
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = $"Pesagens do lote {Lote.Nome}",
                File = new ShareFile(file)
            });
        }
        else
        {
            await dialogService.MessageError("Nenhum lote selecionado", "Selecione um lote para compartilhar/exportar");
        }
    }

    private async Task<string> SalvarUltimaPesagem(Lotes Lote)
    {
        string fileName = ($"{Lote.Nome}_Nr_Pesagem_{Lote.NrPesagem}").ToValidFileName() + ".csv";
        string file = Path.Combine(FileSystem.CacheDirectory, fileName);
        var pesagens = this.context.Pesagens.Where(p => p.LoteId == Lote.Id).OrderBy(p => p.Data).ToList();
        await File.WriteAllTextAsync(file, pesagens.UltimaPesagemToCSVString(this.context.GetConfig().UsarPontoVirgula));
        return file;
    }


    private async Task<string> SalvarUltimas5Pesagens(Lotes Lote)
    {
        string fileName = ($"{Lote.Nome}_{Lote.NrPesagem}").ToValidFileName() + ".csv";
        string file = Path.Combine(FileSystem.CacheDirectory, fileName);
        int nrPesagem = this.context.Pesagens.Where(p => p.LoteId == Lote.Id).Max(p => p.NrPesagem);
        nrPesagem -= 5;
        var pesagens = this.context.Pesagens.Where(p => p.LoteId == Lote.Id && p.NrPesagem >= nrPesagem).GroupBy(p => p.Codigo).ToDictionary(g => g.Key, g => g.OrderBy(p => p.NrPesagem).ToList());
        await File.WriteAllTextAsync(file, pesagens.Ultimas5PesagemToCSVString(nrPesagem, this.context.GetConfig().UsarPontoVirgula));
        return file;
    }

    [ObservableProperty]
    private List<Lotes> lotes;

    [ObservableProperty]
    private Lotes lote;

    [ObservableProperty]
    private bool isBusy = false;

}
