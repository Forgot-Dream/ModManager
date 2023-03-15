using ModManager.Common.Structs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static ModManager.Utils.RequestUtil;

namespace ModManager.Utils
{
    public static class ParseUtil
    {
        public static Regex Github_Regex = new Regex(@"https://github.com/[-\w]*/[-\w]*");
        public static Regex Curseforge_Regex = new Regex(@"https://www.curseforge.com/minecraft/mc-mods/[-\w]*");
        public static Regex Name_Regex = new Regex(@"\[.*\]");

        [Obsolete("已被废弃")]
        public static async Task<ObservableCollection<ModItem>> GetDataFromAPI(ObservableCollection<ModItem> sourceItems, string MCVer)
        {
            return await Task.Run(() =>
            {
                int GetDataSuccess = 0;
                int GetDataError = 0;
                ObservableCollection<ModItem> retsi = new();
                foreach (ModItem sourceItem in sourceItems)
                {
                    switch (sourceItem.Type)
                    {
                        case "Github":
                            {
                                if (TryChangeSourceToCurseforge(sourceItem, MCVer)) { GetDataSuccess++; break; }
                                var response = GetGithubMod(sourceItem, MCVer);
                                _ = response ? GetDataSuccess++ : GetDataError++;
                                break;
                            }
                        case "Curseforge":
                            {
                                if (sourceItem.VersionList.Count > 0) { break; }
                                var response = GetCurseforgeMod(sourceItem, MCVer);
                                _ = response ? GetDataSuccess++ : GetDataError++;
                                break;
                            }
                        default:
                            {
                                if (TryChangeSourceToCurseforge(sourceItem, MCVer)) { GetDataSuccess++; break; }
                                break;
                            }
                    }
                    retsi.Add(sourceItem);
                }
                return retsi;
            });
        }

        [Obsolete("已被废弃")]
        public static ModItem? ParseModJson(JObject ModJson, FileInfo file)
        {
            ModItem si;
            if (ModJson["contact"] == null || (ModJson["contact"]["homepage"] == null && ModJson["contact"]["sources"] == null))
            {
                return null;
            }
            else
            {
                try { si = ParseFileLine(ModJson["contact"]["homepage"].Value<string>()); }
                catch (ArgumentNullException) { si = ParseFileLine(ModJson["contact"]["sources"].Value<string>()); }
                if (si == null)
                {
                    return null;
                }
                else
                {
                    var name_match = Name_Regex.Match(file.Name);
                    si.Comment = name_match.Success ? name_match.Groups[0].Value : ModJson["name"].Value<String>();
                    si.Name = ModJson["name"].Value<String>();
                    return si;
                }
            }
        }

        [Obsolete("已被废弃")]
        public static ModItem? ParseFileLine(string Content)
        {
            var name_match = Name_Regex.Match(Content);
            var name = name_match.Success ? name_match.Groups[0].Value : null;
            var github_match = Github_Regex.Match(Content);
            if (github_match.Success)
            {
                var source_url = github_match.Groups[0].Value;
                Regex regex = new Regex(@"https://github.com/[-\w]*/[-\w]*/");
                var match = regex.Match(source_url);
                var url = match.Success ? match.Groups[0].Value : source_url;
                var type = "Github";
                var url_split = url.Split('/');
                return new ModItem() { Comment = name, Type = type, Name = url_split[url_split.Length - 1], URL = url };
            }
            var curseforge_match = Curseforge_Regex.Match(Content);
            if (curseforge_match.Success)
            {
                var url = curseforge_match.Groups[0].Value;
                var type = "Curseforge";
                var url_split = url.Split('/');
                return new ModItem() { Comment = name, Type = type, URL = url, Name = url_split[url_split.Length - 1] };
            }
            return null;
        }

        [Obsolete("已被废弃")]
        public static ModItem? ParseCurseforgeResponse(JToken content,string MCVer)
        {
            ModItem sourceitem = new();
            sourceitem.URL = content["links"]["websiteUrl"].Value<string>();
            sourceitem.CurseforgeID = content["Id"].Value<string>();
            foreach (var fileitem in content["latestFilesIndexes"])
            {
                if (sourceitem.VersionList.Count == 0 || fileitem["fileId"].Value<int>() != sourceitem.VersionList.Last().fileId)
                {
                    sourceitem.VersionList.Add(new FileItem()
                    {
                        fileId = (int)fileitem["fileId"],
                        gameVersion = (string)fileitem["gameVersion"],
                        filename = (string)fileitem["filename"]
                    });
                }

                if ((MCVer == null || fileitem["gameVersion"].ToString() == MCVer) && sourceitem.Version == null)
                {
                    sourceitem.Version = sourceitem.VersionList.Last();
                }
            }
            return sourceitem;
        }
    }
}
