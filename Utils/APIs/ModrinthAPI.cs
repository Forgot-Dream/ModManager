using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        


        /// <summary>
        /// 向Modrinth的API发送Get请求
        /// </summary>
        /// <param name="Path">子路由</param>
        /// <param name="Params">参数</param>
        /// <returns>Json对象</returns>
        private JObject? APIGet(string Path,Dictionary<string,string>? Params)
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
            request.Headers.Add("User-Agent", "Forgot-Dream/ModManager/1.56.0"); //按文档要求写入User-Agent
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
    }
}
