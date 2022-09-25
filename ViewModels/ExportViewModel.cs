using Microsoft.WindowsAPICodePack.Dialogs;
using ModManager.Common;
using ModManager.Common.Events;
using ModManager.Extension;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using static ModManager.Utils.FileUtil;

namespace ModManager.ViewModels
{
    public class ExportViewModel : BindableBase
    {
        private string packversion;

        public string PackVersion
        {
            get { return packversion; }
            set { packversion = value; RaisePropertyChanged(); }
        }

        private string packauthor;

        public string PackAuthor
        {
            get { return packauthor; }
            set { packauthor = value; RaisePropertyChanged(); }
        }

        private string packname;

        public string PackName
        {
            get { return packname; }
            set { packname = value; RaisePropertyChanged(); }
        }

        public DelegateCommand<string> ExportCommand { get; private set; }
        private readonly IEventAggregator aggregator;
        private readonly IDialogHostService dialogHostService;

        public ExportViewModel(IEventAggregator aggregator, IDialogHostService dialogHostService)
        {
            ExportCommand = new DelegateCommand<string>(Export);
            this.aggregator = aggregator;
            this.dialogHostService = dialogHostService;
        }

        private void Export(string obj)
        {
            if (PackAuthor == null || PackName == null || PackVersion == null || PackAuthor == string.Empty || PackName == string.Empty || PackVersion == string.Empty || ConfigExt.MCVersion == null || ConfigExt.LoaderVersion == null) { aggregator.GetEvent<MessageEvent>().Publish("请确认填写上方所有信息以及选择了MC和Loader版本！"); return; }
            switch (obj)
            {
                case "Curseforge": { ExportAsZipFile(ConfigExt.SourceItems, ConfigExt.MCVersion, ConfigExt.LoaderVersion, PackName, PackVersion, PackAuthor, dialogHostService, aggregator); break; }
                case "All": { break; }
                case "Json": {
                        CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog("请选择要保存的文件夹");
                        openFileDialog.IsFolderPicker = true;
                        if (openFileDialog.ShowDialog() == CommonFileDialogResult.Cancel)
                            return;
                        ExportModJson(ConfigExt.SourceItems, openFileDialog.FileName);
                        break; }
            }
        }
    }
}
