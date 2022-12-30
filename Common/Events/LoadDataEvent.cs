using ModManager.Common.Structs;
using Prism.Events;
using System.Collections.ObjectModel;

namespace ModManager.Common.Events
{
    public class LoadDataEvent : PubSubEvent<ObservableCollection<ModItem>> { }
}
