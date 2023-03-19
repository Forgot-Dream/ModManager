using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using ModManager.Common;
using ModManager.Common.Structs;
using ModManager.Utils;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.Generic;

namespace ModManager.ViewModels.Dialogs
{
    public class AddProjectViewModel : BindableBase, IDialogHostAware
    {
        private readonly IEventAggregator aggregator;

        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get;  set; }
        public DelegateCommand CancelCommand { get;  set; }
        public DelegateCommand LoadCommand { get;  set; }

        private string projectname;
        /// <summary>
        /// 项目名字
        /// </summary>
        public string ProjectName
        {
            get { return projectname; }
            set { projectname = value; RaisePropertyChanged(); }
        }

        private string folderpath;
        /// <summary>
        /// 文件夹路径
        /// </summary>
        public string FolderPath
        {
            get { return folderpath; }
            set { folderpath = value; RaisePropertyChanged(); }
        }

        private string mcversion;
        /// <summary>
        /// MC版本
        /// </summary>
        public string MCVersion
        {
            get { return mcversion; }
            set { mcversion = value; RaisePropertyChanged(); }
        }

        private string loadertype;
        /// <summary>
        /// 加载器版本
        /// </summary>
        public string LoaderType
        {
            get { return loadertype; }
            set { loadertype = value; }
        }

        public List<MinecraftGameVersion> MinecraftGameVersions
        {
            get { return MinecraftVersionManager.INSTANCE.GetMajorVersion(); }
        }

        public AddProjectViewModel(IEventAggregator aggregator)
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
            CommonOpenFileDialog openFileDialog = new("请选择要添加的本地文件...")
            {
                IsFolderPicker = true
            };
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Cancel)
                return;
            FolderPath = openFileDialog.FileName;
        }

        private void Cancel()
        {
            DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.Cancel));
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(MCVersion) ||
                string.IsNullOrEmpty(LoaderType) ||
                string.IsNullOrEmpty(FolderPath) ||
                string.IsNullOrEmpty(ProjectName))
                return;
            ProjectItem project = new()
            {
                FolderPath = FolderPath,
                LoaderType = LoaderType,
                Name = ProjectName,
                MCVersion = MCVersion,
            };

            IDialogParameters parameters = new DialogParameters
            {
                { "ProjectItem", project }
            };

            DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, parameters));
        }
    }
}

