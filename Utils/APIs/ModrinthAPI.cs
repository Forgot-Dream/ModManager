using ModManager.Common.Structs;
using ModManager.Extension;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ModManager.Utils.APIs
{
    public class ModrinthAPI
    {
        /// <summary>
        /// Modrinth的API网址
        /// </summary>
        private const string APIPrefix = "https://api.modrinth.com";

        private static ModrinthAPI api;

        /// <summary>
        /// 获取一个静态实例
        /// </summary>
        /// <returns>Modrinth Api实例</returns>
        public static ModrinthAPI API()
        {
            api ??= new ModrinthAPI();
            return api;
        }

        /// <summary>
        /// 向Modrinth发送一个查询请求（异步）
        /// </summary>
        /// <param name="query">查询的字符串</param>
        /// <param name="index">排序方式 relevance=相似度 downloads=下载数 follows=关注数 newest=项目创建时间 updated=更新时间</param>
        /// <param name="offset">偏移量</param>
        /// <param name="facets">查询条件</param>
        /// <param name="limit">每页限制</param>
        /// <returns>带有搜索结果的列表</returns>
        public async Task<List<ModrinthModItem>?> SearchMods(string query, string? index, int? offset, Facets? facets, int limit = 20)
        {
            return await Task.Run(() =>
            {
                Dictionary<string, string> Params = new()
            {
                { "query", query },
                { "limit", limit.ToString() }
            };

                if (index != null)
                    Params.Add("index", index);
                if (offset != null)
                    Params.Add("offset", offset.ToString());
                if (facets != null)
                    Params.Add("facets", facets.ToString());

                var response = APIGet("/v2/search", Params);
                if (response != null && !response["hits"].IsNullOrEmpty())
                {
                    List<ModrinthModItem> Items = new();
                    foreach (var json in response["hits"])
                        Items.Add(ModrinthModItem.fromJson(json));
                    return Items;
                }
                return null;
            });
            
        }

        /// <summary>
        /// 从Modrinth查询MC的版本号
        /// </summary>
        /// <returns>MC版本号的列表</returns>
        public List<MinecraftGameVersion>? GetMinecraftVersionList()
        {

            var reply = APIGet("/v2/tag/game_version", null);
            if (reply == null)
                return null;
            List<MinecraftGameVersion> retlist = new();
            foreach (var version in reply)
                retlist.Add(new MinecraftGameVersion() { date = DateTime.Parse(version["date"].Value<string>()),version = version["version"].Value<string?>(),version_type = version["version_type"].Value<string?>(),major = version["major"].Value<bool>() });
            return retlist;
        }

        /// <summary>
        /// 获取Mod的版本信息
        /// </summary>
        /// <param name="id">Mod的唯一ID</param>
        /// <returns>Mod的版本信息列表</returns>
        public List<ModrinthModFileInfo>? GetModVersions(string id)
        {
            var response = APIGet($"/v2/project/{id}/version", null);
            if (response == null || response.IsNullOrEmpty())
                return null;
            List<ModrinthModFileInfo> Items = new();
            foreach (var json in response)
            {
                Items.Add(ModrinthModFileInfo.fromJson(json));
            }
            return Items;
        }


        /// <summary>
        /// 向Modrinth的API发送Get请求
        /// </summary>
        /// <param name="Path">子路由</param>
        /// <param name="Params">参数</param>
        /// <returns>Json对象</returns>
        private JToken? APIGet(string Path, Dictionary<string, string>? Params)
        {
            StringBuilder builder = new();
            builder.Append(APIPrefix + Path);
            if (Params != null && Params.Count > 0)
            {
                builder.Append('?');
                int i = 0;
                foreach (var item in Params)
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
            request.Headers.Add("User-Agent", @"Forgot-Dream/ModManager"); //按文档要求写入User-Agent
            try
            {
                var response = httpClient.Send(request);
                response.EnsureSuccessStatusCode();
                Stream myResponseStream = response.Content.ReadAsStream();
                StreamReader myStreamReader = new(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return JToken.Parse(retString);
            }
            catch (Exception e) { Debug.WriteLine(e.StackTrace); return null; }
        }
    }
}
