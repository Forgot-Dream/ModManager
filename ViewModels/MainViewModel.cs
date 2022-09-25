using ModManager.Common;
using ModManager.Common.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;

namespace ModManager.ViewModels
{
    public class MainViewModel : BindableBase, IConfigureService
    {
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator aggregator;
        private readonly IDialogHostService dialogHostService;

        public MainViewModel(IRegionManager regionManager, IEventAggregator aggregator, IDialogHostService dialogHostService)
        {
            MenuBars = new ObservableCollection<MenuBar>();
            this.regionManager = regionManager;
            this.aggregator = aggregator;
            this.dialogHostService = dialogHostService;
            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);
            aggregator.GetEvent<MessageEvent>().Subscribe(MessageDialogOpen);
        }

        private void MessageDialogOpen(string obj)
        {
            var param = new DialogParameters();
            param.Add("args", obj);
            dialogHostService.ShowDialog("MessageView", param);
        }

        private ObservableCollection<MenuBar> menuBars;
        public DelegateCommand<MenuBar> NavigateCommand { get; private set; }

        private void Navigate(MenuBar obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.Name_Space))
                return;
            regionManager.Regions["MainViewRegion"].RequestNavigate(obj.Name_Space);
        }

        public ObservableCollection<MenuBar> MenuBars
        {
            get { return menuBars; }
            set { menuBars = value; RaisePropertyChanged(); }
        }

        void CreateMenu()
        {
            MenuBars.Add(new MenuBar() { Icon = "Minecraft", Name = "导入", Name_Space = "ImportView" });
            MenuBars.Add(new MenuBar() { Icon = "CogOutline", Name = "设置", Name_Space = "SettingsView" });
            MenuBars.Add(new MenuBar() { Icon = "Download", Name = "导出", Name_Space = "ExportView" });
        }

        public void Configure()
        {
            CreateMenu();
            regionManager.Regions["MainViewRegion"].RequestNavigate("ImportView");
        }
    }
}
