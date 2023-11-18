using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Tipos
{
    public interface IDialogService
    {
        Task<string> InputDialog(string title, string message);

        Task<string> SelectDialog(string title, List<string> options);

        Task<bool> InputAlert(string title, string message, string confirmar = "SIM", string cancelar = "NÃO");

        Task MessageError(string title, string message);

        Task NavigateBack();
    }
}
