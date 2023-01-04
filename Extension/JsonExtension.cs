using Newtonsoft.Json.Linq;

namespace ModManager.Extension
{
    public static class JsonExtension
    {
        /// <summary>
        /// 拓展方法:校验jtoken是否为空值
        /// </summary>
        /// <param name="token"></param>
        /// <returns>空=true</returns>
        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
               (token.Type == JTokenType.Array && !token.HasValues) ||
               (token.Type == JTokenType.Object && !token.HasValues) ||
               (token.Type == JTokenType.String && token.ToString() == string.Empty) ||
               (token.Type == JTokenType.Null);
        }
    }
}
