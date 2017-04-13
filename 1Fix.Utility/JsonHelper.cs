using System;
using System.Web;
using System.Web.Script.Serialization;

namespace _1Fix.Utility
{
    /// <summary>
    /// Json Helper 
    /// </summary>
    public class JsonHelper
    {
        public static string ToJson(object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(obj);
            return json;
        }

        public static T FromJson<T>(string json)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            T obj = serializer.Deserialize<T>(json);
            return obj;
        }
    }
}
