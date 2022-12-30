using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace ModManager.Common.Structs
{
    public class ProjectItem:BindableBase
    {
		private string name;
		/// <summary>
		/// 工程文件名称
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; RaisePropertyChanged(); }
		}

        private string loadertype;
        /// <summary>
        /// Mod加载器名称
        /// </summary>
        public string LoaderType
        {
            get { return loadertype; }
            set { loadertype = value; RaisePropertyChanged(); }
        }

        private string loaderversion;
        /// <summary>
        /// Mod加载器版本
        /// </summary>
        public string LoaderVersion
        {
            get { return loaderversion; }
            set { loaderversion = value; RaisePropertyChanged(); }
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

        private string folderpath;
        /// <summary>
        /// 对应Mods文件夹的路径
        /// </summary>
        public string FolderPath
        {
            get { return folderpath; }
            set { folderpath = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<ModItem>? moditems;
        /// <summary>
        /// 包含Mod实例的存储 Null=未加载
        /// </summary>
        public ObservableCollection<ModItem>? ModItems
        {
            get { return moditems; }
            set { moditems = value; RaisePropertyChanged(); }
        }

    }
}
