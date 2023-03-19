using ModManager.Common.Structs;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ModManager.Utils
{
    public static class ConfigUtil
    {
        readonly static string ProjectsConfigPath = Environment.CurrentDirectory + "projects.json";
        public static ObservableCollection<ProjectItem>? ReadProjectsFromConfig()
        {
            try
            {
                if (System.IO.File.Exists(ProjectsConfigPath))
                {
                    return JsonConvert.DeserializeObject<ObservableCollection<ProjectItem>>(System.IO.File.ReadAllText(ProjectsConfigPath));
                }
                else
                {
                    return new ObservableCollection<ProjectItem>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public static void WriteProjectsToConfig(ObservableCollection<ProjectItem> projectItems)
        {
            try
            {
                System.IO.File.WriteAllText(ProjectsConfigPath, JsonConvert.SerializeObject(projectItems));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
