using ModManager.Common.Structs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ModManager.Utils
{
    public static class RequestUtil
    {
        private static string mc_ver_url = "https://meta.fabricmc.net/v2/versions/game"; //fabric的MC版本查询接口
        private static string loader_ver_url = "https://meta.fabricmc.net/v2/versions/loader";
        private static string curseforge_api_url_prefix = "https://api.curseforge.com";
        private static string github_api_url_prefix = "https://api.github.com";
        private static string curseforge_api_key = "$2a$10$o8pygPrhvKBHuuh5imL2W.LCNFhB15zBYAExXx/TqTx/Zp5px2lxu"; //HMCL的api-key
        private static string mc_curseforge_id = "432";


        public static bool GetGithubMod(ModItem sourceItem, string? MCVer) //获取github来源Mod的Release
        {
            var request_url = sourceItem.URL.Replace("https://github.com", github_api_url_prefix + "/repos") + "/releases";
            Console.WriteLine(request_url);
            var response = GetGithubResponse(request_url);
            if (response != null)
            {
                var ResponseJson = JsonConvert.DeserializeObject<JArray>(response);
                foreach (var item in ResponseJson)
                {
                    foreach (var filejson in item["assets"])
                    {
                        sourceItem.VersionList.Add(new FileItem() { filename = filejson["name"].Value<string>(), fileUrl = filejson["browser_download_url"].Value<string>() });
                        if ((MCVer == null || filejson["name"].Value<string>().Contains(MCVer)) && sourceItem.Version == null) { sourceItem.Version = sourceItem.VersionList.Last(); }
                    }
                }
                return true;
            }
            return false;
        }

        public static bool TryChangeSourceToCurseforge(ModItem sourceItem, string? MCVer) // 尝试换源为Curseforge
        {
            Dictionary<string, string> Param = new Dictionary<string, string>();
            Param.Add("gameId", mc_curseforge_id); //Curseforge MC ID
            Param.Add("searchFilter", sourceItem.Name); //查询字符串
            Param.Add("classId", "6"); // Mod类
            Param.Add("sortOrder", "desc"); //倒序
            Param.Add("modLoaderType", "4");//Fabric Mod
            var response = GetCurseforge("/v1/mods/search", Param);
            if (response != null)
            {
                var ResponseJson = JObject.Parse(response);
                if (ResponseJson["pagination"]["totalCount"].Value<int>() > 0)
                {
                    foreach (var item in ResponseJson["data"])
                    {
                        if (item["name"].Value<string>().Contains(sourceItem.Name))
                        {
                            bool is_fabric = false;
                            foreach (var fileitem in item["latestFilesIndexes"])
                            {
                                try { if (fileitem["modLoader"] != null && fileitem["modLoader"].Value<int>() == 4) { is_fabric = true; break; } }
                                catch { continue; }
                            }
                            if (!is_fabric) { break; }
                            sourceItem.CurseforgeID = item["id"].Value<string>();
                            sourceItem.URL = item["links"]["websiteUrl"].ToString();
                            sourceItem.Type = "Curseforge";
                            sourceItem.VersionList.Clear();
                            sourceItem.Version = null;
                            foreach (var fileitem in item["latestFilesIndexes"])
                            {
                                if (sourceItem.VersionList.Count == 0 || fileitem["fileId"].Value<int>() != sourceItem.VersionList.Last().fileId)
                                {
                                    sourceItem.VersionList.Add(new FileItem()
                                    {
                                        fileId = fileitem["fileId"].Value<int>(),
                                        gameVersion = fileitem["gameVersion"].Value<string>(),
                                        filename = fileitem["filename"].Value<string>()
                                    });
                                }

                                if (fileitem["gameVersion"].ToString() == MCVer && sourceItem.Version == null)
                                {
                                    sourceItem.Version = sourceItem.VersionList.Last();
                                }
                            }
                            return true;
                        }
                        return false;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }


        public static async Task<ModItem?> GetCurseforgeModItem(string CurseforgeID, string? MCVer)
        {
            return await Task.Run(() =>
            {
                ModItem ret_item = new ModItem() { CurseforgeID = CurseforgeID, Type = "Curseforge" };
                if (!GetCurseforgeMod(ret_item, MCVer))
                    return null;
                return ret_item;
            });
        }

        public static async Task<ModItem> GetGithubModItem(string url, string? MCVer)
        {
            return await Task.Run(() =>
            {
                ModItem ret_item = new ModItem() { Type = "Github", URL = url };
                GetGithubMod(ret_item, MCVer);
                return ret_item;
            });
        }

        public static bool GetCurseforgeMod(ModItem sourceitem, string? MCVer) //获取Curseforge的版本信息
        {
            string response;
            if (sourceitem.CurseforgeID == null)
            {
                string CurseForgeName = sourceitem.URL.Replace("https://www.curseforge.com/minecraft/mc-mods/", "");
                Dictionary<string, string> Param = new Dictionary<string, string>();
                Param.Add("gameId", mc_curseforge_id); //Curseforge MC ID
                Param.Add("searchFilter", CurseForgeName.ToLower()); //查询字符串
                Param.Add("classId", "6"); // Mod类
                Param.Add("sortOrder", "desc"); //倒序
                Param.Add("modLoaderType", "4");//Fabric Mod
                response = GetCurseforge("/v1/mods/search", Param);
            }
            else
            {
                response = GetCurseforge($"/v1/mods/{sourceitem.CurseforgeID}", null);
            }
            if (response != null)
            {
                var ResponseJson = JObject.Parse(response);
                if (sourceitem.CurseforgeID == null && ResponseJson["pagination"]["totalCount"].Value<int>() > 0)
                {
                    foreach (var item in ResponseJson["data"])
                    {
                        if (item["links"]["websiteUrl"].ToString() == sourceitem.URL)
                        {
                            sourceitem.CurseforgeID = item["id"].Value<string>();
                            sourceitem.VersionList.Clear();
                            foreach (var fileitem in item["latestFilesIndexes"])
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
                            return true;
                        }
                        return false;
                    }
                    return false;
                }
                else if (sourceitem.CurseforgeID != null)
                {
                    sourceitem.VersionList.Clear();
                    sourceitem.URL = ResponseJson["data"]["links"]["websiteUrl"].Value<string>();
                    sourceitem.Name = ResponseJson["data"]["name"].Value<string>();
                    foreach (var fileitem in ResponseJson["data"]["latestFilesIndexes"])
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
                    return true;
                }
                return false;
            }
            return false;
        }
        public static async Task<ObservableCollection<string>?> getMCVersion() // 异步获取MC版本
        {
            return await Task.Run(() =>
            {
                var response = GetHttpResponse(mc_ver_url);
                if (response == null) { return null; }
                var infoArray = JsonConvert.DeserializeObject<JArray>(response);
                var mc_version = new ObservableCollection<string>();
                for (int i = 0; i < infoArray.Count; i++)
                {
                    var info = infoArray[i];
                    mc_version.Add(info.Value<string>("version"));
                }
                return mc_version;
            });
        }

        public static async Task<ObservableCollection<string>?> getLoaderVerison()//异步获取Fabric版本
        {
            return await Task.Run(() =>
            {
                var response = GetHttpResponse(loader_ver_url);
                if (response == null) { return null; }
                var infoArray = JsonConvert.DeserializeObject<JArray>(response);
                var loader_verison = new ObservableCollection<string>();
                for (int i = 0; i < infoArray.Count; i++)
                {
                    var info = infoArray[i];
                    loader_verison.Add(info.Value<String>("version"));
                }
                return loader_verison;
            });
        }

        public static string? GetCurseforge(string path, Dictionary<string, string>? Param) //发送带参数的curseforge请求
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(curseforge_api_url_prefix + path);
            if (Param != null && Param.Count > 0)
            {
                builder.Append('?');
                int i = 0;
                foreach (var item in Param)
                {
                    if (i > 0)
                        builder.Append('&');
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, builder.ToString());
            request.Headers.Add("Accept", "application/json"); //设置接受类型
            request.Headers.Add("x-api-key", curseforge_api_key); //设置apikey
            try
            {
                var response = httpClient.Send(request);
                response.EnsureSuccessStatusCode();
                Stream myResponseStream = response.Content.ReadAsStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            catch (Exception e) { Console.WriteLine(e.Message); return null; }
        }

        public static string? PostCurseforge(string path, Dictionary<string, List<uint>> Data)//发送带数据的curseforge post请求
        {
            HttpClient httpClient = new();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, curseforge_api_url_prefix + path);
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Headers.Add("x-api-key", curseforge_api_key);
            StringContent content = new(JsonConvert.SerializeObject(Data), Encoding.UTF8, "application/json");
            requestMessage.Content = content;
            try
            {
                var response = httpClient.Send(requestMessage);
                response.EnsureSuccessStatusCode();
                Stream myResponseStream = response.Content.ReadAsStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            catch (Exception e) { Console.WriteLine(e.Message); return null; }
        }

        public static string? GetGithubResponse(string url) // 获取Github Get请求的回应
        {
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.5112.102 Safari/537.36 Edg/104.0.1293.70");
            try
            {
                var response = httpClient.Send(request);
                response.EnsureSuccessStatusCode();
                Stream myResponseStream = response.Content.ReadAsStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            catch (Exception e) { Console.WriteLine(e.Message); return null; }
        }
        public static string? GetHttpResponse(string url) // 获取HTTP Get请求的回应
        {
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.5112.102 Safari/537.36 Edg/104.0.1293.70");

            try
            {
                var response = httpClient.Send(request);
                response.EnsureSuccessStatusCode();
                Stream myResponseStream = response.Content.ReadAsStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            catch (Exception e) { Console.WriteLine(e.Message); return null; }
        }
    }
}

