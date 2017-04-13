using System;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace _1Fix.Utility
{
    public class SerializeHelper
    {
        #region XML serialziation

        public static string SerializeObjectToXml<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                serializer.Serialize(sw, obj);
            }
            return sb.ToString();
        }

        public static T DeserializeFromXml<T>(string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(xmlString))
            {
                T obj = (T)serializer.Deserialize(stringReader);

                return obj;
            }
        }

        #endregion

        #region binary serialization

        /// <summary>
        /// 将对象使用二进制格式序列化成byte数组
        /// </summary>
        /// <param name="obj">待保存的对象</param>
        /// <returns>byte数组</returns>
        public static byte[] SerializeObjectToBinaryBytes(object obj)
        {
            //将对象序列化到MemoryStream中
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                //从MemoryStream中获取获取byte数组
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 将使用二进制格式保存的byte数组反序列化成对象
        /// </summary>
        /// <param name="bytes">byte数组</param>
        /// <returns>对象</returns>
        public static object DeserializeFromBinaryBytes(byte[] bytes)
        {
            object result = null;
            BinaryFormatter formatter = new BinaryFormatter();
            if (bytes != null)
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    result = formatter.Deserialize(ms);
                }
            }
            return result;
        }

        #endregion
    }
}
