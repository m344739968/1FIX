using System;
using System.Web;

namespace _1Fix.Utility
{
    public class CookieHelper
    {
        /// <summary>
        /// 作者：Vincen
        /// 时间：2013.12.16
        /// 描述：设置Cookie
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cookieValue"></param>
        public static void SetCookie(string cookieName, string cookieValue)
        {
            SetCookie(cookieName, cookieValue, null, null);
        }

        /// <summary>
        /// 作者：Vincen
        /// 时间：2013.12.16
        /// 描述：设置Cookie
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cookieValue"></param>
        /// <param name="expireDays"></param>
        public static void SetCookie(string cookieName, string cookieValue, int? expireDays)
        {
            SetCookie(cookieName, cookieValue, expireDays, null);
        }

        /// <summary>
        /// 作者：Vincen
        /// 时间：2013.12.16
        /// 描述：设置Cookie
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cookieValue"></param>
        /// <param name="expireDays">in day unit</param>
        /// <param name="domain"></param>
        public static void SetCookie(string cookieName, string cookieValue, int? expireDays, string domain)
        {
            var cookie = HttpContext.Current.Response.Cookies.Get(cookieName) ?? new HttpCookie(cookieName, cookieValue);

            if (expireDays.HasValue && expireDays.Value > 0)
            {
                cookie.Expires = DateTime.UtcNow + new TimeSpan(expireDays.Value, 0, 0, 0);
            }
            if (!string.IsNullOrEmpty(domain))
            {
                cookie.Domain = domain;
            }
            cookie.Value = cookieValue;

            HttpContext.Current.Response.Cookies.Set(cookie);
        }

        /// <summary>
        /// 作者：Vincen
        /// 时间：2013.12.16
        /// 描述：设置Cookie
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cookieValue"></param>
        /// <param name="expires"></param>
        public static void SetCookie(string cookieName, string cookieValue, DateTime expires)
        {
            var cookie = HttpContext.Current.Response.Cookies.Get(cookieName) ?? new HttpCookie(cookieName, cookieValue);

            cookie.Expires = expires;
            HttpContext.Current.Response.Cookies.Set(cookie);
        }

        /// <summary>
        /// 作者：Vincen
        /// 时间：2013.12.16
        /// 描述：设置Cookie
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cookieValue"></param>
        /// <param name="expires"></param>
        /// <param name="domain"></param>
        public static void SetCookie(string cookieName, string cookieValue, DateTime expires, string domain)
        {
            var cookie = HttpContext.Current.Response.Cookies.Get(cookieName) ?? new HttpCookie(cookieName, cookieValue);

            cookie.Expires = expires;
            cookie.Domain = domain;
            HttpContext.Current.Response.Cookies.Set(cookie);
        }

        /// <summary>
        /// 作者：Vincen
        /// 时间：2013.12.16
        /// 描述：从Request中获取Cookie的值
        /// </summary>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public static string GetCookie(string cookieName)
        {
            var cookie = HttpContext.Current.Request.Cookies.Get(cookieName);
            if (cookie == null)
            {
                return string.Empty;
            }
            return cookie.Value;
        }
    }
}
