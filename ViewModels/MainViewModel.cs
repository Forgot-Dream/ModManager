using ModManager.Common;
using ModManager.Common.Events;
using ModManager.Common.Structs;
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
            ChangeViewCommand = new DelegateCommand<string>(ChangeView);
            aggregator.GetEvent<MessageEvent>().Subscribe(MessageDialogOpen);
        }

        private void MessageDialogOpen(string obj)
        {
            var param = new DialogParameters
            {
                { "args", obj }
            };
            dialogHostService.ShowDialog("MessageView", param);
        }

        private ObservableCollection<MenuBar> menuBars;
        public DelegateCommand<MenuBar> NavigateCommand { get; private set; }
        public DelegateCommand<string> ChangeViewCommand { get; private set; }

        private void ChangeView(string obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj))
                return;
            regionManager.Regions["MainViewRegion"].RequestNavigate(obj);
        }
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
            MenuBars.Add(new MenuBar() { Icon = "Download", Name = "Test", Name_Space = "SearchView" });
        }

        public void Configure()
        {
            CreateMenu();
            regionManager.Regions["MainViewRegion"].RequestNavigate("ImportView");
        }
    }
}
