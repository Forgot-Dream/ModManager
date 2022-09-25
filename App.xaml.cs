using ModManager.Common;
using ModManager.ViewModels;
using ModManager.ViewModels.Dialogs;
using ModManager.Views;
using ModManager.Views.Dialogs;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ModManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDialogHostService,DialogHostService>();

            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
            containerRegistry.RegisterForNavigation<ImportView, ImportViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
            containerRegistry.RegisterForNavigation<ExportView, ExportViewModel>();
            containerRegistry.RegisterForNavigation<AddCurseforgeModView, AddCurseforgeModViewModel>();
            containerRegistry.RegisterForNavigation<AddGithubModView, AddGithubModViewModel>();
            containerRegistry.RegisterForNavigation<AddLocalFileView, AddLocalFileViewModel>();
            containerRegistry.RegisterForNavigation<MessageView,MessageViewModel>();
            containerRegistry.RegisterForNavigation<ProgressView>();
        }

        protected override void OnInitialized()
        {
            var service = App.Current.MainWindow.DataContext as IConfigureService;
            if (service != null)
                service.Configure();
            base.OnInitialized();
        }
    }
}
