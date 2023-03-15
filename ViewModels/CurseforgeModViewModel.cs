using ModManager.Common.Events;
using ModManager.Common.Structs;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace ModManager.ViewModels
{
    public class CurseforgeModViewModel:BindableBase,INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator aggregator;

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
        CurseforgeModViewModel(IRegionManager regionManager,IEventAggregator aggregator)
        {
            this.regionManager = regionManager;
            this.aggregator = aggregator;
            BackCommand = new DelegateCommand<object>(Back);
        }

        void Back(object sender)
        {
            regionManager.RequestNavigate("MainViewRegion", "CurseforgeSearchView");
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
            if (navigationContext != null && navigationContext.Parameters.ContainsKey("ModItem"))
            {
                var item = navigationContext.Parameters.GetValue<CurseforgeModItem>("ModItem");
                if (item != null && !item.Equals(ModItem))
                {
                    ModItem = item;
                    if (ModItem.ModInfo.HtmlDescription != null)
                        aggregator.GetEvent<SetCurseforgeModBodyEvent>().Publish(ModItem.ModInfo.HtmlDescription);
                }

            }

        }
    }
}
