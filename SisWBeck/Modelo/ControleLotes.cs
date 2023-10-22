using Modelo.Entidades;
using SisWBeck.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisWBeck.Modelo
{
    public partial class ControleLotes : ObservableObject
    {
        private Lotes Lote;
        private SISWBeckContext db;

        public ControleLotes(Lotes lote, SISWBeckContext db)
        {
            this.Lote = lote;
            this.db = db;
            int? LoteId = lote?.Id;
            var lista = db.Pesagens.Where(p => p.LoteId == LoteId && p.NrPesagem == NrPesagem).OrderByDescending(p => p.Data).ToList();
            Pesagens = new ObservableCollection<Pesagens>(lista);
            var lista_animais = db.Pesagens.GroupBy(p => p.Codigo).Select(g => g.Key).ToList();
            Animais = new ObservableCollection<string>(lista_animais);
        }

        public string Nome => this.Lote?.Nome;
        public int? NrPesagem => this.Lote?.NrPesagem;
        public string IdentificacaoLote => $"Lote: {Lote?.Nome} ({Lote?.Data.ToString("dd/MM/yyyy")})";
        public string DadosLote => $"Nr Pesagem:{Lote?.NrPesagem} - Animais:{Lote?.NrAnimais ?? 0}";

        [ObservableProperty]
        private ObservableCollection<Pesagens> pesagens;

        [ObservableProperty]
        private ObservableCollection<string> animais;

        private Pesagens _pesagemSelecionada = null;
        public Pesagens PesagemSelecionada
        {
            get => _pesagemSelecionada;
            set
            {
                if (_pesagemSelecionada != value)
                {
                    _pesagemSelecionada = value;
                    OnPropertyChanged(nameof(PesagemSelecionada));
                }
            }
        }

        public bool IsPesagemSelecionada => PesagemSelecionada != null;


        public async Task AddPesagem(string identificacao, int peso)
        {
            if (!String.IsNullOrWhiteSpace(identificacao))
            {
                Pesagens pesagem = new Pesagens();
                pesagem.Peso = peso;
                pesagem.Codigo = identificacao;
                pesagem.Lote = this.Lote;
                await db.Add(pesagem);
                await db.SaveChangesAsync();
                if (Lote.Pesagens == null)
                    Lote.Pesagens = new List<Pesagens>();
                Lote.Pesagens.Add(pesagem);
                Pesagens.Insert(0, pesagem);
                if (!Animais.Contains(identificacao))
                {
                    Animais.Add(identificacao);
                    this.OnPropertyChanged(nameof(DadosLote));
                }
            }
        }

        public async Task RemovePesagem(Pesagens pesagem)
        {
            if (PesagemSelecionada != null)
            {
                db.Pesagens.Remove(PesagemSelecionada);
                Pesagens.Remove(PesagemSelecionada);
                await db.SaveChangesAsync();
            }
        }

    }
}
