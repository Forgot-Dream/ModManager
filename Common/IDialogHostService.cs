using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModManager.Common
{
    public interface IDialogHostService : IDialogService
    {
        string DialogHostName { get; set; }

        Task<IDialogResult> ShowDialog(string name, IDialogParameters? parameters, string dialogHostName = "Root");
    }
}
