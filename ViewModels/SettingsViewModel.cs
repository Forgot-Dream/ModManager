using ModManager.Common;
using ModManager.Common.Events;
using ModManager.Common.Structs;
using ModManager.Extension;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static ModManager.Utils.RequestUtil;

namespace ModManager.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public SettingsViewModel(IEventAggregator aggregator, IDialogHostService dialogHostService)
        {
            aggregator.GetEvent<LoadDataEvent>().Subscribe(SetSourceItems);
            aggregator.GetEvent<AddItemEvent>().Subscribe(AddItem);
            AddCommand = new DelegateCommand<string>(Add);
            DeleteCommand = new DelegateCommand<string>(Delete);
            RefreshCommand = new DelegateCommand<string>(Refresh);
            this.aggregator = aggregator;
            this.dialogHostService = dialogHostService;
        }

        private async void Refresh(string obj)
        {
            if (SourceItems == null || SourceItems.Count == 0) return;
            switch (obj)
            {
                case "Selected":
                    {
                        var list = SourceItems.Where(item => item.IsSelected).Distinct().ToList();
                        if (list.Count == 0 || await Confirm($"确定刷新所选{list.Count}项？") == false) return;
                        aggregator.GetEvent<LoadingEvent>().Publish(true);
                        SourceItems = await RefreshAsync(list);
                        aggregator.GetEvent<LoadingEvent>().Publish(false);
                        aggregator.GetEvent<MessageEvent>().Publish($"刷新完成!\n总数:{list.Count}");
                        break;
                    }
                case "All":
                    {
                        if (await Confirm("确定刷新所有项？") == false) return;
                        aggregator.GetEvent<LoadingEvent>().Publish(true);
                        SourceItems = await RefreshAsync(SourceItems.Distinct().ToList());
                        aggregator.GetEvent<LoadingEvent>().Publish(false);
                        aggregator.GetEvent<MessageEvent>().Publish($"刷新完成!\n总数:{SourceItems.Count}");
                        break;
                    }
            }
        }

        private async Task<ObservableCollection<ModItem>> RefreshAsync(List<ModItem> list)
        {
            return await Task.Run(() =>
            {
                ObservableCollection<ModItem> result = new();
                foreach (var item in list)
                {
                    var index = SourceItems.IndexOf(item);
                    switch (item.Type)
                    {
                        case "Github":
                            {
                                GetGithubMod(item, ConfigExt.MCVersion);
                                break;
                            }
                        case "Curseforge":
                            {
                                GetCurseforgeMod(item, ConfigExt.MCVersion);
                                break;
                            }
                    }
                    result.Add(item);
                }
                return result;
            });
        }

        private async void Delete(string obj)
        {
            if (SourceItems == null || SourceItems.Count == 0) return;
            switch (obj)
            {
                case "Selected":
                    {
                        var list = SourceItems.Where(item => item.IsSelected).Distinct().ToList();
                        if (list.Count == 0 || await Confirm("确定删除所选项？") == false) return;
                        foreach (var item in list)
                            SourceItems.Remove(item);
                        RaisePropertyChanged(nameof(IsAllItemsSelected));
                        break;
                    }
                case "Invalid":
                    {
                        if (await Confirm("确定删除无效项（版本空白项）？") == false) return;
                        var list = sourceItems.Where(item => item.Version == null).Distinct().ToList();
                        foreach (var item in list)
                            SourceItems.Remove(item);
                        RaisePropertyChanged(nameof(IsAllItemsSelected));
                        break;
                    }
                case "All":
                    {
                        if (await Confirm("确定清空列表？") == false) return;
                        SourceItems.Clear(); break;
                    }
            }
        }

        private void AddItem(ModItem obj)
        {
            SourceItems ??= new ObservableCollection<ModItem>();
            obj.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ModItem.IsSelected))
                    RaisePropertyChanged(nameof(IsAllItemsSelected));
            };
            SourceItems.Add(obj);
        }

        private void Add(string obj)
        {
            switch (obj)
            {
                case "AddCurseforgeMod": dialogHostService.ShowDialog("AddCurseforgeModView", null); break;
                case "AddGithubMod": dialogHostService.ShowDialog("AddGithubModView", null); break;
                case "AddLocalFile": dialogHostService.ShowDialog("AddLocalFileView", null); break;
            }
        }

        async Task<bool> Confirm(string hint)
        {
            var param = new DialogParameters
            {
                { "args", hint }
            };
            var result = await dialogHostService.ShowDialog("MessageView", param);
            if (result.Result == ButtonResult.Cancel)
                return false;
            else
                return true;
        }

        public DelegateCommand<string> AddCommand { get; private set; }
        public DelegateCommand<string> DeleteCommand { get; private set; }
        public DelegateCommand<string> RefreshCommand { get; private set; }

        public bool? IsAllItemsSelected
        {
            get
            {
                if (SourceItems == null || SourceItems.Count == 0)
                    return false;
                var selected = SourceItems.Select(item => item.IsSelected).Distinct().ToList();
                return selected.Count == 1 ? selected.Single() : null;
            }
            set
            {
                if (value.HasValue)
                {
                    SelectAll(value.Value, SourceItems);
                }
                RaisePropertyChanged();
            }
        }

        private static void SelectAll(bool select, ObservableCollection<ModItem> models)
        {
            if (models == null || models.Count == 0)
                return;
            foreach (var model in models)
            {
                model.IsSelected = select;
            }
        }
        void SetSourceItems(ObservableCollection<ModItem> sourceItems)
        {
            SourceItems = sourceItems;
            foreach (var item in SourceItems)
            {
                item.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(ModItem.IsSelected))
                        RaisePropertyChanged(nameof(IsAllItemsSelected));
                };
            }
        }

        private ObservableCollection<ModItem> sourceItems;
        private readonly IEventAggregator aggregator;
        private readonly IDialogHostService dialogHostService;

        public ObservableCollection<ModItem> SourceItems
        {
            get { return sourceItems; }
            set { sourceItems = value; ConfigExt.SourceItems = value; RaisePropertyChanged(); }
        }


    }
}
