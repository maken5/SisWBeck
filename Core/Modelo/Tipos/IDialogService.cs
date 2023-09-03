using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Tipos
{
    public interface IDialogService
    {
        Task<string> InputDialog(string title, string message);
        Task<bool> InputAlert(string title, string message);

        Task MessageError(string title, string message);
    }
}
