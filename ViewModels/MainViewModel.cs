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
            Projects = new ObservableCollection<ProjectItem>();
            this.regionManager = regionManager;
            this.aggregator = aggregator;
            this.dialogHostService = dialogHostService;
            NavigateCommand = new DelegateCommand<ProjectItem>(Navigate);
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

        private ObservableCollection<ProjectItem> projects;
        public DelegateCommand<ProjectItem> NavigateCommand { get; private set; }
        public DelegateCommand<string> ChangeViewCommand { get; private set; }

        private void ChangeView(string obj)
        {
            if (string.IsNullOrEmpty(obj))
                return;
            regionManager.Regions["MainViewRegion"].RequestNavigate(obj);
        }
        private void Navigate(ProjectItem obj)
        {
            regionManager.Regions["MainViewRegion"].RequestNavigate("ProjectView");
        }

        public ObservableCollection<ProjectItem> Projects
        {
            get { return projects; }
            set { projects = value; RaisePropertyChanged(); }
        }

        void CreateMenu()
        {
            Projects.Add(new ProjectItem() { Name = "TEST",LoaderType="fabric" });
        }

        public void Configure()
        {
            CreateMenu();
            regionManager.Regions["MainViewRegion"].RequestNavigate("ImportView");
        }
    }
}
