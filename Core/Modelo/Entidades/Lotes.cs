using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
        
        private int _nrPesagem=1;
        public int NrPesagem
        {
            get => _nrPesagem <1 ? 1 : _nrPesagem;
            set => _nrPesagem = value;
        }


        public DateTime? UltimaDataPesagem => Pesagens?.Max(p => p.Data);
        public int? UltimoNrPesagem => Pesagens?.Max(p => p.NrPesagem);
        
    }
}
