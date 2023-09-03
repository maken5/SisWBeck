using Modelo.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modelo.Tipos
{
    public interface IDialogServicePesagem : IDialogService
    {
        void ShowPesagem(Lotes lote);
    }
}
