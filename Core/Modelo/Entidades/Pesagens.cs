using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo.Entidades
{
    public class Pesagens
    {
        public int Id { get; set; }

        public int NrPesagem { get; set; }
        public string Codigo { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public int Peso { get; set; }

        public int LoteId { get; set; }

        private Lotes _lote;

        public virtual Lotes Lote
        {
            get => _lote;
            set
            {
                _lote = value;
                NrPesagem = value?.NrPesagem ??0;
            }
        }
    }
}
