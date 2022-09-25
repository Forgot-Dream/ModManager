using Microsoft.WindowsAPICodePack.Dialogs;
using ModManager.Common.Events;
using ModManager.Extension;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using static ModManager.Utils.FileUtil;
using static ModManager.Utils.RequestUtil;

namespace ModManager.ViewModels
{
    public class ImportViewModel: BindableBase
    {
		private ObservableCollection<string>? mcversions;
		private ObservableCollection<string>? loaderversions;
        private readonly IEventAggregator aggregator;
        private readonly IRegionManager regionManager;
        public static string MCVersion { get { return ConfigExt.MCVersion; } set { ConfigExt.MCVersion = value; } }
        public static string LoaderVersion { get { return ConfigExt.LoaderVersion; } set { ConfigExt.LoaderVersion = value; } }

        public ImportViewModel(IEventAggregator aggregator,IRegionManager regionManager)
        {
            GetMcAndLoaderCommand = new DelegateCommand<object?>(GetMcAndLoaderVersion);
            LoadDataCommand = new DelegateCommand<string>(LoadData);
            this.aggregator = aggregator;
            this.regionManager = regionManager;
            GetMcAndLoaderVersion(null);
        }

        public DelegateCommand<object?> GetMcAndLoaderCommand { get; private set; }
        public DelegateCommand<string> LoadDataCommand { get; private set; }


        private async void LoadData(string param)
        {
            if(MCVersion == null || LoaderVersion == null) { aggregator.GetEvent<MessageEvent>().Publish("请先选择MC版本和Fabric Loader版本！"); return; }
            switch (param)
            {
                case "LoadFromModsFolder": {
                        CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog("请选择Mods文件夹......");
                        openFileDialog.IsFolderPicker = true;
                        if (openFileDialog.ShowDialog() == CommonFileDialogResult.Cancel)
                            return;
                        regionManager.Regions["MainViewRegion"].RequestNavigate("SettingsView");
                        aggregator.GetEvent<LoadingEvent>().Publish(true);
                        aggregator.GetEvent<LoadDataEvent>().Publish(await LoadFromModsFolderFile(openFileDialog.FileName, MCVersion));
                        aggregator.GetEvent<LoadingEvent>().Publish(false);
                        break; }
                case "LoadFromJsonFile": {
                        CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog("请选择Json配置文件......");
                        openFileDialog.Filters.Add(new CommonFileDialogFilter("Json配置文件", "*.json"));
                        if (openFileDialog.ShowDialog() == CommonFileDialogResult.Cancel)
                            return;
                        regionManager.Regions["MainViewRegion"].RequestNavigate("SettingsView");
                        aggregator.GetEvent<LoadDataEvent>().Publish(ImportModJson(openFileDialog.FileName));
                        break; }
            }
        }
        private async void GetMcAndLoaderVersion(object? obj)
        {
            aggregator.GetEvent<LoadingEvent>().Publish(true);
            McVersions = await getMCVersion();
            LoaderVersions = await getLoaderVerison();
            aggregator.GetEvent<LoadingEvent>().Publish(false);
        }


        public ObservableCollection<string>? LoaderVersions
        {
            get { return loaderversions; }
            set { loaderversions = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<string>? McVersions
        {
            get { return mcversions; }
            set { mcversions = value; RaisePropertyChanged(); }
        }

	}
}
