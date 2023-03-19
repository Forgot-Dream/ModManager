using ModManager.Common;
using ModManager.ViewModels;
using ModManager.ViewModels.Dialogs;
using ModManager.Views;
using ModManager.Views.Dialogs;
using ModManager.Views.SearchView;
using Prism.DryIoc;
using Prism.Ioc;
using System;
using System.Diagnostics;
using System.IO;
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
            containerRegistry.Register<IDialogHostService, DialogHostService>();

            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
            containerRegistry.RegisterForNavigation<ImportView, ImportViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
            containerRegistry.RegisterForNavigation<ExportView, ExportViewModel>();

            //弹窗类注册
            containerRegistry.RegisterForNavigation<AddCurseforgeModView, AddCurseforgeModViewModel>();
            containerRegistry.RegisterForNavigation<AddGithubModView, AddGithubModViewModel>();
            containerRegistry.RegisterForNavigation<AddLocalFileView, AddLocalFileViewModel>();
            containerRegistry.RegisterForNavigation<AddLocalFolderView, AddLocalFolderViewModel>();
            containerRegistry.RegisterForNavigation<MessageView, MessageViewModel>();
            containerRegistry.RegisterForNavigation<AddProjectView, AddProjectViewModel>();

            containerRegistry.RegisterForNavigation<ProjectView, ProjectViewModel>();

            //Curseforge界面注册
            containerRegistry.RegisterForNavigation<CurseforgeSearchView, CurseforgeSearchViewModel>();
            containerRegistry.RegisterForNavigation<CurseforgeModView, CurseforgeModViewModel>();

            //Modrinth界面注册
            containerRegistry.RegisterForNavigation<ModrinthSearchView, ModrinthSearchViewModel>();
            containerRegistry.RegisterForNavigation<ModrinthModView, ModrinthModViewModel>();

            //关于界面注册
            containerRegistry.RegisterForNavigation<AboutView>();

            //进度条界面注册
            containerRegistry.RegisterForNavigation<ProgressView>();
        }

        protected override void OnInitialized()
        {
            IConfigureService? service = Current.MainWindow.DataContext as IConfigureService;
            service?.Configure();
            base.OnInitialized();
        }
        protected override void OnExit(ExitEventArgs e)//关闭时删除cache缓存(如果在搜索界面会删不掉，下次打开再说/)
        {
            string appDirectory = Directory.GetCurrentDirectory();
            string folderPath = Path.Combine(appDirectory, "caches");
            base.OnExit(e);
            // 删除指定文件夹
            if (Directory.Exists(folderPath))
            {
                try
                {
                    Directory.Delete(folderPath, true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            }

        }
    }
}
