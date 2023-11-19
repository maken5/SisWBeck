using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Security.Cryptography;

namespace Modelo.Entidades
{
    public class Lotes
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        private DateTime _data = DateTime.Now;
        public DateTime Data
        {
            get => _data;
            set => _data = value;
        }
        public virtual ICollection<Pesagens> Pesagens { get; set; }

        private int _nrPesagem = 1;
        public int NrPesagem
        {
            get => _nrPesagem < 1 ? 1 : _nrPesagem;
            set => _nrPesagem = value;
        }


        public DateTime? UltimaDataPesagem
        {
            get
            {
                DateTime? data = null;
                if (Pesagens != null && Pesagens.Any())
                    data = Pesagens.Max(p => p.Data);
                return data;
            }
        }
        public int? UltimoNrPesagem
        {
            get
            {
                int? nr = null;
                if (Pesagens != null && Pesagens.Any())
                    return Pesagens.Max(p => p.NrPesagem);
                return nr;
            }
        }

        public int NrAnimais
        {
            get
            {
                int? nr = null;
                if (Pesagens != null && Pesagens.Any())
                    nr = Pesagens.GroupBy(a => a.Codigo).Count();
                return nr ?? 0;
            }
        }
        
    }
}
