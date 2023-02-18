using ModManager.Common.Structs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace ModManager.ViewModels
{
    public class ModrinthModViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;

        private ModrinthModItem moditem;
        /// <summary>
        /// 引用的ModrinthMod实例
        /// </summary>
        public ModrinthModItem ModItem
        {
            get { return moditem; }
            set { moditem = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 返回命令
        /// </summary>
        public DelegateCommand<object> BackCommand { get; private set; }
        ModrinthModViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            BackCommand = new DelegateCommand<object>(Back);
        }

        void Back(object sender)
        {
            regionManager.RequestNavigate("MainViewRegion", "ModrinthSearchView");
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
                var item = navigationContext.Parameters.GetValue<ModrinthModItem>("ModItem");
                if (item != null)
                {
                    item.AcquireFileInfo();
                    ModItem = item;
                }
            }

        }
    }
}
