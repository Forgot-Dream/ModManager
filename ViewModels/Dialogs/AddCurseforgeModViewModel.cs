using MaterialDesignThemes.Wpf;
using ModManager.Common;
using ModManager.Common.Events;
using ModManager.Extension;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using static ModManager.Utils.RequestUtil;

namespace ModManager.ViewModels.Dialogs
{
    internal class AddCurseforgeModViewModel : BindableBase , IDialogHostAware
    {
        private readonly IEventAggregator aggregator;

        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        private string curseforgeid;

        public string CurseforgeID
        {
            get { return curseforgeid; }
            set { curseforgeid = value; }
        }


        public AddCurseforgeModViewModel(IEventAggregator aggregator)
        {
            this.aggregator = aggregator;
        }
        public void OnDialogOpend(IDialogParameters parameters)
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private void Cancel()
        {
            DialogHost.Close(DialogHostName,new DialogResult(ButtonResult.Cancel));
        }

        private async void Save()
        {
            if(CurseforgeID == null) { Cancel();return; }
            DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK));
            aggregator.GetEvent<LoadingEvent>().Publish(true);
            var item = await GetCurseforgeModItem(CurseforgeID, ConfigExt.MCVersion);
            aggregator.GetEvent<LoadingEvent>().Publish(false);
            if (item == null)
            {
                aggregator.GetEvent<MessageEvent>().Publish("获取CurseforgeMod失败\n可能是错误的CurseforgeID，或者网络错误");
                return;
            }
            aggregator.GetEvent<AddItemEvent>().Publish(item);
        }
    }
}
