using System;
using System.Text;
using System.Security.Cryptography;

namespace _1Fix.Utility
{
    public class SecurityHelper
    {
        #region ========加密========

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Encrypt(string text)
        {
            return Encrypt(text, "DEFAULTKEY");
        }
        /// <summary> 
        /// 加密数据 
        /// </summary> 
        /// <param name="text">待加密的串</param> 
        /// <param name="key">密钥</param> 
        /// <returns></returns> 
        public static string Encrypt(string text, string key)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray;
                inputByteArray = Encoding.Default.GetBytes(text);

                des.Key = ASCIIEncoding.ASCII.GetBytes(GetMd5(key).Substring(0, 8));
                des.IV = ASCIIEncoding.ASCII.GetBytes(GetMd5(key).Substring(0, 8));
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    StringBuilder ret = new StringBuilder();
                    foreach (byte b in ms.ToArray())
                    {
                        ret.AppendFormat("{0:X2}", b);
                    }
                    return ret.ToString();
                }
            }
        }
        /// <summary>
        /// 获取MD5加密串
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetMd5(string text)
        {
            MD5 md5 = MD5.Create();
            byte[] bs = Encoding.UTF8.GetBytes(text);
            byte[] hs = md5.ComputeHash(bs);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hs)
            {
                // 以十六进制格式格式化
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 获取SHA加密串
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetSHA(string text)
        {
            return GetSHA(text);
        }

        #endregion

        #region ========解密========

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Decrypt(string text)
        {
            return Decrypt(text, "DEFAULTKEY");
        }
        /// <summary> 
        /// 解密数据 
        /// </summary> 
        /// <param name="text">待解密的串</param> 
        /// <param name="key">密钥</param> 
        /// <returns></returns> 
        public static string Decrypt(string text, string key)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                int len;
                len = text.Length / 2;
                byte[] inputByteArray = new byte[len];
                int x, i;
                for (x = 0; x < len; x++)
                {
                    i = Convert.ToInt32(text.Substring(x * 2, 2), 16);
                    inputByteArray[x] = (byte)i;
                }

                des.Key = ASCIIEncoding.ASCII.GetBytes(GetMd5(key).Substring(0, 8));
                des.IV = ASCIIEncoding.ASCII.GetBytes(GetMd5(key).Substring(0, 8));
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    return Encoding.Default.GetString(ms.ToArray());
                }
            }
        }

        #endregion
    }
}
