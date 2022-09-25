using Prism.Commands;
using Prism.Services.Dialogs;

namespace ModManager.Common
{
    public interface IDialogHostAware
    {
        string DialogHostName { get; set; }
        void OnDialogOpend(IDialogParameters parameters);
        DelegateCommand SaveCommand { get; set; }
        DelegateCommand CancelCommand { get; set; }
    }
}
