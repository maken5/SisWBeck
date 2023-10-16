using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Tipos
{
    public interface IMainNavigationService : IDialogService
    {
        Task NavigateToMain();
    }
}
