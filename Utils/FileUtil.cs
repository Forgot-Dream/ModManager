using ModManager.Common;
using ModManager.Common.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static ModManager.Utils.DownloadUtil;
using static ModManager.Utils.ParseUtil;
using static ModManager.Utils.RequestUtil;

namespace ModManager.Utils
{
    public class FileUtil
    {
        public static ObservableCollection<SourceItem>? ImportModJson(string path) // 导入Json文件
        {
            try
            {
                StreamReader streamReader = new StreamReader(path);
                string jsonstr = streamReader.ReadToEnd();
                streamReader.Close();
                var items = JsonConvert.DeserializeObject<ObservableCollection<SourceItem>>(jsonstr);
                ObservableCollection<SourceItem> result = new ObservableCollection<SourceItem>();
                foreach(var item in items)
                {
                    foreach(var version in item.VersionList)
                    {
                        if(item.Version == null)
                        {
                            break;
                        }
                        if (version.filename == item.Version.filename)
                        {
                            item.Version = version;
                            break;
                        }
                    }
                    result.Add(item);
                }
                return result;
            }
            catch (Exception e)
            {
                MessageBox.Show("导入失败\n" + e.Message);
                return null;
            }
        }
        public static void ExportModJson(ObservableCollection<SourceItem> sourceItems, string path) // 导出Json文件
        {
            try
            {
                var itemlist_str = JsonConvert.SerializeObject(sourceItems);
                System.IO.File.WriteAllText(path + "/mods.json", itemlist_str);
                Process process = new();
                process.StartInfo.FileName = "explorer.exe";
                process.StartInfo.Arguments = @" /select, " + path.Replace('\\','/') + "/mods.json";
                process.StartInfo.UseShellExecute = true;
                process.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show("导出失败\n" + e.Message);
            }
        }
        public static async void ExportAsZipFile(ObservableCollection<SourceItem> sourceItems, string MCVer, string LoaderVer, string Name, string Ver, string Author, IDialogHostService dialogHostService, IEventAggregator aggregator) // 导出为ZIP压缩文件
        {
            aggregator.GetEvent<LoadingEvent>().Publish(true);
            DirectoryInfo TmpDir = new DirectoryInfo(System.Environment.CurrentDirectory + "/tmp");
            if (!TmpDir.Exists) { TmpDir.Create(); }
            else
            {
                TmpDir.Delete(true);
                TmpDir.Create();
            }
            var OvrDir = TmpDir.CreateSubdirectory("overrides");
            var ModDir = TmpDir.CreateSubdirectory("overrides/mods");
            var ResPackDir = TmpDir.CreateSubdirectory("overrides/resourcepacks");
            var ShaderDir = TmpDir.CreateSubdirectory("overrides/shaderpacks");
            Manifest manifest = new(MCVer, LoaderVer, Name, Ver, Author);
            List<string> fails = new List<string>();
            if (sourceItems == null || sourceItems.Count == 0)
            {
                aggregator.GetEvent<LoadingEvent>().Publish(false);
                aggregator.GetEvent<MessageEvent>().Publish("版本列表为空！");
                return;
            }
            foreach (SourceItem item in sourceItems)
            {
                switch (item.Type)
                {
                    case "Curseforge":
                        {
                            if (item.CurseforgeID == null || item.Version == null || item.Version.fileId == 0) { fails.Add(item.Name); break; }
                            manifest.files.Add(new Common.File() { fileID = item.Version.fileId, projectID = int.Parse(item.CurseforgeID), required = true });
                            break;
                        }
                    case "Github":
                        {
                            if (item.Version == null || item.Version.fileUrl == null) { fails.Add(item.Name); break; }
                            if (await DownloadFromURL(item.Version.fileUrl, ModDir.FullName + "/" + item.Version.filename) == false)
                                fails.Add(item.Name);
                            break;
                        }
                    case "本地Mod文件":
                        {
                            try { System.IO.File.Copy(item.URL, ModDir.FullName + "/" + item.Version.filename, true); }
                            catch { fails.Add(item.Name); break; }
                            break;
                        }
                    case "本地资源包":
                        {
                            try { System.IO.File.Copy(item.URL, ResPackDir.FullName + "/" + item.Version.filename, true); }
                            catch { fails.Add(item.Name); break; }
                            break;
                        }
                    case "本地光影包":
                        {
                            try { System.IO.File.Copy(item.URL, ShaderDir.FullName + "/" + item.Version.filename, true); }
                            catch { fails.Add(item.Name); break; }
                            break;
                        }
                }
            }
            if (fails.Count > 1)
            {
                StringBuilder failstr = new StringBuilder();
                foreach (var item in fails)
                {
                    failstr.Append(item);
                    failstr.AppendLine();
                }
                var param = new DialogParameters();
                param.Add("args", failstr.ToString() + "下载/复制失败，请手动下载到/tmp/mods文件夹 \n 点击\"确定\"以继续打包");
                aggregator.GetEvent<LoadingEvent>().Publish(false);
                var result = await dialogHostService.ShowDialog("MessageView", param);
                if (result.Result == ButtonResult.Cancel) { return; }
            }
            var manifest_str = JsonConvert.SerializeObject(manifest);
            System.IO.File.WriteAllText(TmpDir.FullName + "/manifest.json", manifest_str);
            FileInfo OutputFile = new FileInfo(System.Environment.CurrentDirectory + "/" + Name + ".zip");
            if (OutputFile.Exists) { OutputFile.Delete(); }
            ZipFile.CreateFromDirectory(TmpDir.FullName, OutputFile.FullName, CompressionLevel.Optimal, false);
            aggregator.GetEvent<LoadingEvent>().Publish(false);
            aggregator.GetEvent<MessageEvent>().Publish("已经导出到同目录下" + OutputFile.Name);
            TmpDir.Delete(true);
            Process process = new();
            process.StartInfo.FileName = "explorer.exe";
            process.StartInfo.Arguments = @" /select, " + OutputFile.FullName;
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }

        public static async Task<ObservableCollection<SourceItem>> LoadFromModsFolderFile(string FolderPath, string MCVer) //从Mods文件夹加载
        {
            return await Task.Run(async () =>
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(FolderPath);
                ObservableCollection<SourceItem> sourceItems = new ObservableCollection<SourceItem>();
                FileInfo[] files = directoryInfo.GetFiles("*.jar");
                List<uint> fingerprints = new();
                foreach (FileInfo file in files)
                {
                    fingerprints.Add(MurmurHash2.ComputeHash(file.FullName));  //计算每个文件的指纹
                }
                var data = new Dictionary<string, List<uint>>
                {
                    { "fingerprints", fingerprints }
                };
                var response = PostCurseforge("/v1/fingerprints", data); //提交查询请求
                var responsejson = JObject.Parse(response)["data"];
                var exactmatch = (JArray)responsejson["exactFingerprints"];
                var j = 0;
                for (int i = 0; i < fingerprints.Count; i++)
                {
                    Console.WriteLine(i);
                    Console.WriteLine(j);
                    if (j < exactmatch.Count && fingerprints[i] == exactmatch[j].Value<uint>())
                    {
                        var item = await GetCurseforgeModItem(responsejson["exactMatches"][j]["id"].Value<string>(), MCVer);
                        if (item == null)
                        {
                            j++;
                            continue;
                        }
                        item.Comment = item.Name;
                        sourceItems.Add(item);
                        j++;
                    }
                    else
                    {
                        var file = files[i];
                        JObject fabricmodjson;
                        using (var jar = ZipFile.OpenRead(file.FullName))
                        {
                            var entry = jar.GetEntry("fabric.mod.json");
                            using (var stream = entry.Open())
                            using (var reader = new StreamReader(stream))
                            {
                                fabricmodjson = JObject.Parse(reader.ReadToEnd());
                                var ret = ParseModJson(fabricmodjson, file);
                                if (ret == null)
                                {
                                    var item = new SourceItem() { URL = file.FullName, Type = "本地Mod文件", Comment = fabricmodjson["name"].Value<string>(), Name = fabricmodjson["name"].Value<string>(), Version = new FileItem() { filename = file.Name } };
                                    item.VersionList.Add(item.Version);
                                    sourceItems.Add(item);
                                }
                                else
                                {
                                    sourceItems.Add(ret);
                                }
                            }
                        }
                    }
                }
                return await GetDataFromAPI(sourceItems, MCVer);
            });
        }
        public static ObservableCollection<SourceItem> LoadFromTxtFile(string FilePath) //从txt文件加载
        {
            StreamReader streamReader = new StreamReader(FilePath);
            ObservableCollection<SourceItem> sourceItems = new ObservableCollection<SourceItem>();
            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                var item = ParseFileLine(line);
                if (item != null) { sourceItems.Add(item); }
            }
            streamReader.Close();
            return sourceItems;
        }
    }
}
