using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace _1Fix.Utility
{
    public partial class WebHelper
    {
        /// <summary>
        /// Get Url Referrer
        /// </summary>
        /// <returns></returns>
        public static string GetUrlReferrer()
        {
            string _referrerUrl = string.Empty;
            if (null != HttpContext.Current &&
                null != HttpContext.Current.Request &&
                null != HttpContext.Current.Request.UrlReferrer)
                _referrerUrl = HttpContext.Current.Request.UrlReferrer.ToString();

            return _referrerUrl;
        }

        /// <summary>
        /// 获取当前请求的IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentIpAddress()
        {
            string _ipAddress = string.Empty;
            if (null != HttpContext.Current &&
                null != HttpContext.Current.Request &&
                null != HttpContext.Current.Request.UserHostAddress)
                _ipAddress = HttpContext.Current.Request.UserHostAddress;

            return _ipAddress;
        }

        /// <summary>
        /// 获取当请求的DNS
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentHostName()
        {
            string _hostName = string.Empty;
            if (null != HttpContext.Current &&
                null != HttpContext.Current.Request &&
                null != HttpContext.Current.Request.UserHostName)
                _hostName = HttpContext.Current.Request.UserHostName;

            return _hostName;
        }

        /// <summary>
        ///  获取当前页面URL
        /// </summary>
        /// <param name="includeQueryString"></param>
        /// <returns></returns>
        public static string GetThisPageUrl(bool includeQueryString)
        {
            bool _useSsl = IsCurrentConnectionSecured();
            return GetThisPageUrl(includeQueryString, _useSsl);
        }

        /// <summary>
        /// 获取当前页面URL
        /// </summary>
        /// <param name="includeQueryString">是否包含查询串</param>
        /// <param name="useSsl">是否需要SSL保护页面</param>
        /// <returns></returns>
        public static string GetThisPageUrl(bool includeQueryString, bool useSsl)
        {
            string _url = string.Empty;
            if (null == HttpContext.Current)
                return _url;

            if (includeQueryString)
            {
                string _host = GetSslHost(useSsl);
                if (_host.EndsWith("/"))
                    _host = _host.Substring(0, _host.Length - 1);

                _url = _host + HttpContext.Current.Request.RawUrl;
            }
            else
            {
                _url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
            }
            _url = _url.ToLowerInvariant();

            return _url;
        }

        /// <summary>
        /// 当前连接是否使用安全套接字（即HTTPS）
        /// </summary>
        /// <returns></returns>
        public static bool IsCurrentConnectionSecured()
        {
            bool _useSsl = false;
            if (null != HttpContext.Current && null != HttpContext.Current.Request)
                _useSsl = HttpContext.Current.Request.IsSecureConnection;

            return _useSsl;
        }

        /// <summary>
        /// 获取Web服务器指定变量值
        /// </summary>
        /// <param name="name">变量名称</param>
        /// <returns></returns>
        public static string ServerVariables(string name)
        {
            string _temp = string.Empty;
            try
            {
                if (null != HttpContext.Current.Request.ServerVariables[name])
                    _temp = HttpContext.Current.Request.ServerVariables[name];
            }
            catch
            {
                _temp = string.Empty;
            }
            return _temp;
        }

        public static string GetSslHost(bool useSsl)
        {
            string _result = "http://" + ServerVariables("HTTP_HOST");

            if (useSsl)
            {
                _result = _result.Replace("http:/", "https:/");
            }

            if (!_result.EndsWith("/"))
                _result += "/";

            return _result.ToLowerInvariant();
        }

        /// <summary>
        /// 如果被请求的资源是一个典型的资源，不必由CMS引擎处理并返回true。
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsStaticResource(HttpRequest request)
        {
            if (null == request)
                throw new ArgumentNullException("request");

            string path = request.Path;
            string extension = VirtualPathUtility.GetExtension(path);
            if (null == extension) return false;

            switch (extension.ToLower())
            {
                case ".axd":
                case ".ashx":
                case ".bmp":
                case ".css":
                case ".gif":
                case ".ico":
                case ".jpeg":
                case ".jpg":
                case ".rar":
                case ".zip":
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 虚拟路径映射到服务器上的物理路径
        /// </summary>
        /// <param name="path">E.g. "~/bin"</param>
        /// <returns>E.g. "c:\inetpub\wwwroot\bin"</returns>
        public static string MapPath(string path)
        {
            if (null != HttpContext.Current)
            {
                return HostingEnvironment.MapPath(path);
            }
            else
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                int binIndex = baseDirectory.IndexOf("\\bin\\");
                if (binIndex >= 0)
                    baseDirectory = baseDirectory.Substring(0, binIndex);
                else if (baseDirectory.EndsWith("\\bin\\"))
                    baseDirectory = baseDirectory.Substring(0, baseDirectory.Length - 4);

                path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
                return Path.Combine(baseDirectory, path);
            }
        }

        /// <summary>
        /// 编辑URL参数
        /// </summary>
        /// <param name="url">被编辑的URL</param>
        /// <param name="queryStringModification"></param>
        /// <param name="targetLocationModification"></param>
        /// <returns></returns>
        public static string ModifyQueryString(string url, string queryStringModification, string targetLocationModification)
        {
            if (url == null)
                url = string.Empty;
            url = url.ToLowerInvariant();

            if (queryStringModification == null)
                queryStringModification = string.Empty;
            queryStringModification = queryStringModification.ToLowerInvariant();

            if (targetLocationModification == null)
                targetLocationModification = string.Empty;
            targetLocationModification = targetLocationModification.ToLowerInvariant();


            string str = string.Empty;
            string str2 = string.Empty;
            if (url.Contains("#"))
            {
                str2 = url.Substring(url.IndexOf("#") + 1);
                url = url.Substring(0, url.IndexOf("#"));
            }
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryStringModification))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            string[] strArray = str3.Split(new char[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    foreach (string str4 in queryStringModification.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str4))
                        {
                            string[] strArray2 = str4.Split(new char[] { '=' });
                            if (strArray2.Length == 2)
                            {
                                dictionary[strArray2[0]] = strArray2[1];
                            }
                            else
                            {
                                dictionary[str4] = null;
                            }
                        }
                    }
                    var builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
                else
                {
                    str = queryStringModification;
                }
            }
            if (!string.IsNullOrEmpty(targetLocationModification))
            {
                str2 = targetLocationModification;
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)) + (string.IsNullOrEmpty(str2) ? "" : ("#" + str2))).ToLowerInvariant();
        }

        /// <summary>
        /// 移除（URL）指定参数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static string RemoveQueryString(string url, string queryString)
        {
            if (url == null)
                url = string.Empty;
            url = url.ToLowerInvariant();

            if (queryString == null)
                queryString = string.Empty;
            queryString = queryString.ToLowerInvariant();

            string str = string.Empty;
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryString))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            string[] strArray = str3.Split(new char[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    dictionary.Remove(queryString);

                    var builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)));
        }

        /// <summary>
        /// 是否为 搜索引擎请求（网络爬虫软件）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsSearchEngine(HttpRequestBase request)
        {
            if (request == null)
                return false;

            bool result = false;
            try
            {
                result = request.Browser.Crawler;
                if (!result)
                {
                    var regEx = new Regex("Twiceler|twiceler|BaiDuSpider|baduspider|Slurp|slurp|ask|Ask|Teoma|teoma|Yahoo|yahoo");
                    result = regEx.Match(request.UserAgent).Success;
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
            }
            return result;
        }
    }
}
