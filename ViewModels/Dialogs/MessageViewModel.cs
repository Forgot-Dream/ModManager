using MaterialDesignThemes.Wpf;
using ModManager.Common;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace ModManager.ViewModels.Dialogs
{
    public class MessageViewModel : BindableBase, IDialogHostAware
    {

        private string hint;

        public string Hint
        {
            get { return hint; }
            set { hint = value; }
        }

        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public MessageViewModel()
        {
        }
        public void OnDialogOpend(IDialogParameters parameters)
        {
            Hint = parameters.GetValue<string>("args");
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private void Cancel()
        {
            DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.Cancel));
        }

        private void Save()
        {
            DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK));
        }
    }

}
