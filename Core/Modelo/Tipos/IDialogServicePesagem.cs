using Modelo.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Tipos
{
    public interface IDialogServicePesagem : IDialogService
    {
        Task ShowPesagem(Lotes lote);
    }
}
