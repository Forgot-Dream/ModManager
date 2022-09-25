using ModManager.Common;
using System.Collections.ObjectModel;

namespace ModManager.Extension
{
    public static class ConfigExt
    {
        public static string MCVersion;
        public static string LoaderVersion;
        public static ObservableCollection<SourceItem> SourceItems;
    }
}
