using Prism.Mvvm;
using System.Collections.Generic;

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
        /// Mod加载器类型
        /// </summary>
        public string LoaderType
        {
            get { return loadertype; }
            set { loadertype = value; RaisePropertyChanged(); }
        }

        public string Icon
        {
            get
            {
                return $"/Assets/{LoaderType}.png";
            }
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

        private List<LocalModItem>? moditems;
        /// <summary>
        /// 包含Mod实例的存储 Null=未加载
        /// </summary>
        public List<LocalModItem>? ModItems
        {
            get { return moditems; }
            set { moditems = value; RaisePropertyChanged(); }
        }

    }

}
