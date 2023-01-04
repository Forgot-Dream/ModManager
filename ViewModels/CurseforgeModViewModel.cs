using ModManager.Common.Structs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModManager.ViewModels
{
    public class CurseforgeModViewModel:BindableBase,INavigationAware
    {
        private readonly IRegionManager regionManager;

		private CurseforgeModItem moditem;
		/// <summary>
		/// 引用的CurseforgeMod实例
		/// </summary>
		public CurseforgeModItem ModItem
		{
			get { return moditem; }
			set { moditem = value; RaisePropertyChanged(); }
		}

        /// <summary>
        /// 返回命令
        /// </summary>
        public DelegateCommand<object> BackCommand { get; private set; }
        CurseforgeModViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            BackCommand = new DelegateCommand<object>(Back);
        }

        void Back(object sender)
        {
            regionManager.RequestNavigate("MainViewRegion", "SearchView");
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("ModItem"))
            {
                var item = navigationContext.Parameters.GetValue<CurseforgeModItem>("ModItem");
                if (item != null)
                {
                    item.AcquireFileInfo();
                    ModItem = item;
                }
            }
                
        }
    }
}
