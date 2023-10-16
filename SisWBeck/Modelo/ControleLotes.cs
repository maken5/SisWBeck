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

        public ControleLotes(Lotes lote, SISWBeckContext db)
        {
            this.Lote = lote;
            this.db = db;
        }
        public string Nome => this.Lote?.Nome;
        public string IdentificacaoLote => $"Lote: {Lote?.Nome} ({Lote?.Data.ToString("dd/MM/yyyy")})";
        public string DadosLote => $"Nr Pesagem:{Lote?.NrPesagem} - Animais:{Lote?.NrAnimais??0}";

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
    }
}
