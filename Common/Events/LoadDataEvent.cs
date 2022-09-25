using Prism.Events;
using System.Collections.ObjectModel;

namespace ModManager.Common.Events
{
    public class LoadDataEvent : PubSubEvent<ObservableCollection<SourceItem>> { }
}
