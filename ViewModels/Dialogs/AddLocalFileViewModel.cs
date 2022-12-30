using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using ModManager.Common;
using ModManager.Common.Events;
using ModManager.Common.Structs;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.IO;
using System.Linq;
using static ModManager.Utils.MurmurHash2;

namespace ModManager.ViewModels.Dialogs
{
    public class AddLocalFileViewModel : BindableBase, IDialogHostAware
    {
        private readonly IEventAggregator aggregator;

        public string DialogHostName { get; set; }

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand LoadCommand { get; set; }
        private string url;

        public string URL
        {
            get { return url; }
            set { url = value; RaisePropertyChanged(); }
        }

        public string Type { get; set; }


        public AddLocalFileViewModel(IEventAggregator aggregator)
        {
            this.aggregator = aggregator;
        }
        public void OnDialogOpend(IDialogParameters parameters)
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            LoadCommand = new DelegateCommand(Load);
        }

        private void Load()
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog("请选择要添加的本地文件...");
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Cancel)
                return;
            URL = openFileDialog.FileName;
        }

        private void Cancel()
        {
            DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.Cancel));
        }

        private void Save()
        {
            if (Type == null || URL == null)
                return;
            FileInfo fileInfo = new FileInfo(URL);
            ModItem sourceItem = new ModItem() { Type = Type, URL = URL, Comment = fileInfo.Name };
            sourceItem.VersionList.Add(new FileItem() { filename = fileInfo.Name, fileUrl = fileInfo.FullName });
            sourceItem.Version = sourceItem.VersionList.Last();
            aggregator.GetEvent<AddItemEvent>().Publish(sourceItem);
            if (DialogHost.IsDialogOpen("Root") == true)
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK));
        }
    }
}
