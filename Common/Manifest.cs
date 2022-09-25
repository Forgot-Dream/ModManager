using System.Collections.Generic;

namespace ModManager.Common
{
    public class Manifest
    {
        public Minecraft minecraft { get; set; }
        public string manifestType { get; set; }
        public string overrides { get; set; }
        public int manifestVersion { get; set; }
        public string version { get; set; }
        public string author { get; set; }
        public string name { get; set; }
        public List<File> files { get; set; }

        public Manifest(string MCVer, string LoaderVer, string Name, string Ver, string Author)
        {
            manifestType = "minecraftModpack";
            manifestVersion = 1;
            overrides = "overrides";
            files = new List<File>();
            version = Ver;
            author = Author;
            name = Name;
            minecraft = new Minecraft() { version = MCVer, modLoaders = new List<Modloader>() };
            minecraft.modLoaders.Add(new Modloader() { id = "fabric-" + LoaderVer, primary = true });
        }

    }

    public class Minecraft
    {
        public string version { get; set; }
        public List<Modloader> modLoaders { get; set; }
    }

    public class Modloader
    {
        public string id { get; set; }
        public bool primary { get; set; }
    }

    public class File
    {
        public int projectID { get; set; }
        public int fileID { get; set; }
        public bool required { get; set; }
    }
}
