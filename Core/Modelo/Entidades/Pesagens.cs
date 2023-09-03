using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo.Entidades
{
    public class Pesagens
    {
        public int Id { get; set; }

        public string Codigo { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public int Peso { get; set; }

        public int LoteId { get; set; }
        public virtual Lotes Lote { get; set; }
    }
}
