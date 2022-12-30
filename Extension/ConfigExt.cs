using ModManager.Common.Structs;
using System.Collections.ObjectModel;

namespace ModManager.Extension
{
    public static class ConfigExt
    {
        public static string MCVersion;
        public static string LoaderVersion;
        public static ObservableCollection<ModItem> SourceItems;
    }
}
