using Modelo.Entidades;
using SisWBeck.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisWBeck.Modelo
{
    public class ControleLotes : ObservableObject
    {
        private Lotes Lote;
        private SISWBeckContext db;
        private ObservableCollection<Pesagens> pesagens;

        public ControleLotes(Lotes lote, SISWBeckContext db)
        {
            this.Lote = lote;
            this.db = db;
            pesagens = new ObservableCollection<Pesagens>();
            if (lote.Pesagens?.Where(p => p != null && p.NrPesagem == lote.NrPesagem)?.Any() ?? false)
            {
                pesagens = new ObservableCollection<Pesagens>(lote.Pesagens?.Where(p => p != null && p.NrPesagem == lote.NrPesagem));
            } else
            {
                pesagens = new ObservableCollection<Pesagens>();
            }
            pesagens.Add(new Pesagens() { NrPesagem = lote.NrPesagem, Codigo = "123abc", Lote = Lote, Peso = 100 });
            pesagens.Add(new Pesagens() { NrPesagem = lote.NrPesagem, Codigo = "123xyz", Lote = Lote, Peso = 102 });
        }

        public string Nome => this.Lote?.Nome;
        public string IdentificacaoLote => $"Lote: {Lote?.Nome} ({Lote?.Data.ToString("dd/MM/yyyy")})";
        public string DadosLote => $"Nr Pesagem:{Lote?.NrPesagem} - Animais:{Lote?.NrAnimais ?? 0}";

        public ObservableCollection<Pesagens> Pesagens => pesagens;

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
                if (Lote.Pesagens == null)
                    Lote.Pesagens = new List<Pesagens>();
                Lote.Pesagens.Add(pesagem);
                db.SaveChanges();
            }
        }

        public async Task RemovePesagem(Pesagens pesagem)
        {
            await Task.Delay(1);
        }

    }
}
