using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo.Entidades
{
    public class Lotes
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public virtual ICollection<Pesagens> Pesagens { get; set; }


    }
}
