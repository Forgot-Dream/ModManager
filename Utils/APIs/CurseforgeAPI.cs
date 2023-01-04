using ModManager.Common.Structs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static ModManager.Common.Structs.CurseforgeModItem;

namespace ModManager.Utils.APIs
{
    public class CurseforgeAPI
    {
        private static CurseforgeAPI api;

        /// <summary>
        /// Cursefoge 的 API 网址
        /// </summary>
        private const string APIPrefix = "https://api.curseforge.com";
        /// <summary>
        /// Curseforge API 所需的 X-key （From HMCL）
        /// </summary>
        private const string APIXKey = "$2a$10$o8pygPrhvKBHuuh5imL2W.LCNFhB15zBYAExXx/TqTx/Zp5px2lxu";
        /// <summary>
        /// Curseforge MC 的 GameID
        /// </summary>
        private const string MinecraftGameId = "432";

        /// <summary>
        /// 获取一个静态实例
        /// </summary>
        /// <returns>Curseforge Api实例</returns>
        public static CurseforgeAPI API()
        {
            api ??= new CurseforgeAPI();
            return api;
        }

        /// <summary>
        /// 向Curseforge API提交一个模糊查询请求(异步)
        /// </summary>
        /// <param name="classId">分类</param>
        /// <param name="GameVersion">游戏版本  null->any</param>
        /// <param name="ModLoaderType">游戏加载器 0=Any 1=Forge 2=Cauldron 3=LiteLoader 4=Fabric 5=Quilt</param>
        /// <param name="index">页数</param>
        /// <param name="searchFilter">模糊查询的字符串</param>
        /// <param name="sortOrder">搜索结果的正反顺序 true=正序 false=反序</param>
        /// <param name="sortField">排序方式 1=Featured 2=Popularity 3=LastUpdated 4=Name 5=Author 6=TotalDownloads 7=Category 8=GameVersion</param>
        /// <returns>一个带有查询结果的列表</returns>
        public async Task<List<CurseforgeModItem>> SearchMods(string? GameVersion, int index, string searchFilter, int sortField, bool sortOrder = false, int ModLoaderType = 0, int classId = 6)
        {
            return await Task.Run(() =>
            {
                Dictionary<string, string> Params = new()
            {
                { "gameId", MinecraftGameId },
                { "searchFilter", searchFilter },
                { "classId", classId.ToString() },
                { "index" , index.ToString() },
                { "sortOrder" , sortOrder ? "asc":"desc" } //"asc"是正序 "desc"是倒序
            };
                if (sortField != 0)
                    Params.Add("sortField", sortField.ToString());
                if (ModLoaderType != 0)
                    Params.Add("modLoaderType", ModLoaderType.ToString());
                if (GameVersion != null)
                    Params.Add("gameVersion", GameVersion);
                var reply = APIGet("/v1/mods/search", Params);
                List<CurseforgeModItem> modItems = new();
                foreach (var item in reply["data"])
                {
                    modItems.Add(CurseforgeModItem.fromJson(item));
                }
                return modItems;
            });
        }

        /// <summary>
        /// 使用Curseforge ID查询Mod
        /// </summary>
        /// <param name="ModID">Curseforge Mod ID</param>
        /// <returns>对应ID的CurseforgeModItem实例</returns>
        public CurseforgeModItem GetModFromId(int ModID)
        {
            var reply = APIGet($"/v1/mods/{ModID}", null);
            return fromJson(reply["data"]);
        }

        /// <summary>
        /// 使用Curseforge Mod Id查询Mod的详细文件列表
        /// </summary>
        /// <param name="ModID">Curseforge Mod ID</param>
        /// <returns>对应带有文件信息的列表</returns>
        public List<CurseforgeModFileInfo> GetModFileInfos(int ModID)
        {
            var reply = APIGet($"/v1/mods/{ModID}/files", null);
            List<CurseforgeModFileInfo> curseforgeModFileInfos = new();
            foreach (var file in reply["data"])
                curseforgeModFileInfos.Add(CurseforgeModFileInfo.fromJson(file));
            return curseforgeModFileInfos;
        }

        /// <summary>
        /// 从Curseforge查询MC的版本号
        /// </summary>
        /// <returns>MC版本号的列表</returns>
        public List<string> GetMinecraftVersionList()
        {
            
            var reply = APIGet("/v1/minecraft/version", null);
            List<string> retlist = new();
            foreach (var version in reply["data"])
                retlist.Add(version["versionString"].Value<string>());
            return retlist;
        }
        /// <summary>
        /// 向Curseforge API发送带参数的Get请求
        /// </summary>
        /// <param name="path">子路径</param>
        /// <param name="Param">参数</param>
        /// <returns>响应结果的json对象</returns>
        private JObject? APIGet(string path, Dictionary<string, string>? Param)
        {
            StringBuilder builder = new();
            builder.Append(APIPrefix + path);
            if (Param != null && Param.Count > 0)
            {
                builder.Append('?');
                int i = 0;
                foreach (var item in Param)
                {
                    if (item.Value == null)
                        continue;
                    if (i > 0)
                        builder.Append('&');
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }
            HttpClient httpClient = new();
            HttpRequestMessage request = new(HttpMethod.Get, builder.ToString());
            request.Headers.Add("Accept", "application/json"); //设置接受类型
            request.Headers.Add("x-api-key", APIXKey); //设置apikey
            try
            {
                var response = httpClient.Send(request);
                response.EnsureSuccessStatusCode();
                Stream myResponseStream = response.Content.ReadAsStream();
                StreamReader myStreamReader = new(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return JObject.Parse(retString);
            }
            catch (Exception e) { Console.WriteLine(e.Message); return null; }
        }

        /// <summary>
        /// 向Curseforge API发送Post请求 用于批量查询
        /// </summary>
        /// <param name="path">子路径</param>
        /// <param name="Data">数据</param>
        /// <returns>返回查询结果的Json对象</returns>
        private JObject? APIPost(string path, Dictionary<string, List<uint>> Data)
        {
            HttpClient httpClient = new();
            HttpRequestMessage requestMessage = new(HttpMethod.Post, APIPrefix + path);
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Headers.Add("x-api-key", APIXKey);
            StringContent content = new(JsonConvert.SerializeObject(Data), Encoding.UTF8, "application/json");
            requestMessage.Content = content;
            try
            {
                var response = httpClient.Send(requestMessage);
                response.EnsureSuccessStatusCode();
                Stream myResponseStream = response.Content.ReadAsStream();
                StreamReader myStreamReader = new(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return JObject.Parse(retString);
            }
            catch (Exception e) { Console.WriteLine(e.Message); return null; }
        }
    }
}
