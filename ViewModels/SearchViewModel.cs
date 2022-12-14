using ModManager.Common.Structs;
using ModManager.Extension;
using ModManager.Utils.APIs;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModManager.ViewModels
{
    public class SearchViewModel : BindableBase
    {
        private readonly CurseforgeAPI Api;
        private readonly IEventAggregator aggregator;
        private readonly IRegionManager regionManager;
        private string searchfilter;
        private string gameversion;
        private int classid;
        private int sortfield = -1;
        private List<CurseforgeModItem> moditems;
        private List<string> gameversions;

        /// <summary>
        /// 搜索的字符串
        /// </summary>
        public string searchFilter { get { return searchfilter; } set { searchfilter = value; RaisePropertyChanged(); } }
        /// <summary>
        /// 游戏版本
        /// </summary>
        public string GameVersion { get { return gameversion; } set { gameversion = value; RaisePropertyChanged(); } }
        /// <summary>
        /// 大类ID
        /// </summary>
        public int classId { get { return classid; } set { classid = value; RaisePropertyChanged(); } }
        /// <summary>
        /// 排序方式
        /// </summary>
        public int sortField { get { return sortfield; } set { sortfield = value; RaisePropertyChanged(); } }
        /// <summary>
        /// 游戏版本的列表 在初始化的时候从CurseforgeApi拿数据，获取之前从本地缓存拿数据
        /// </summary>
        public List<string> GameVersions { get { return gameversions; } private set { gameversions = value; RaisePropertyChanged(); } }
        public List<CurseforgeModItem> ModItems
        {
            get { return moditems; }
            set { moditems = value; RaisePropertyChanged(); }
        }

        SearchViewModel(IEventAggregator aggregator,IRegionManager regionManager)
        {
            this.aggregator = aggregator;
            this.regionManager = regionManager;
            Api = CurseforgeAPI.API();
            SearchCommand = new DelegateCommand<object>(Search);
            ResetCommand = new DelegateCommand<object>(Reset);
            ViewDetailsCommand = new DelegateCommand<object>(ViewDetails);
            Init();
            Search(null); //进入的时候先加载一波mod
        }

        async void Init() //加载部分数据
        {
            await Task.Run(() => {
                GameVersions = Api.GetMinecraftVersionList();
            });
        }
        public DelegateCommand<object> SearchCommand { get; private set; }
        public DelegateCommand<object> ResetCommand { get; private set; }
        public DelegateCommand<object> ViewDetailsCommand { get; private set; }

        async void Search(object? obj) {
            aggregator.ShowProgressBar(true);
            ModItems = await Api.SearchMods(GameVersion, 0, searchFilter, sortField+1);
            aggregator.ShowProgressBar(false);
        }
        void Reset(object? obj) { searchFilter = ""; GameVersion = ""; classId = 0; sortField = -1; }

        void ViewDetails(object index)
        {
            if((int)index == -1)
                return;
            var param = new NavigationParameters
            {
                { "ModItem", ModItems[(int)index] }
            };
            regionManager.RequestNavigate("MainViewRegion", "CurseforgeModView", param);
        }
    }
}
