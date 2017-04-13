using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace _1Fix.Common.Wap
{
    /// <summary>
    /// 手机端当前用户
    /// </summary>
    public class WapLoginUserManager
    {/// 作者：Malei
        /// 时间：2014.04.25
        /// 描述：当前登录用户
        /// </summary>
        public static WeiXinUser CurrLoginUser
        {
            get
            {
                return HttpContext.Current.Session[Global.WapLoginUserKey] as WeiXinUser;
            }
        }

        /// <summary>
        /// 作者：Malei
        /// 时间：2013.11.14 PM
        /// 描述：验证用户是否登录 （true/false）
        /// </summary>
        public static bool IsLogin
        {
            get { return (HttpContext.Current.Session[Global.WapLoginUserKey] != null); }
        }

        /// <summary>
        /// 作者：Malei
        /// 时间：2014.04.25
        /// 描述：标记当前登录用户
        /// </summary>
        /// <param name="user"></param>
        public static void SignLoginUser(WeiXinUser user)
        {
            //设置当前登录用户
            HttpContext.Current.Session[Global.WapLoginUserKey] = user;

            //标记当前会话
            Set(string.Format("{0}", user.openid), HttpContext.Current.Session.SessionID);
        }

        /// <summary>
        /// 作者：Malei
        /// 时间：2013.11.14 PM 
        /// 描述：设置用户登录信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        public static void Set(string userId, string sessionId)
        {
            var loginUsers = (HttpContext.Current.Application[Global.WapLoginUserKey] as NameValueCollection) ?? new NameValueCollection();

            loginUsers.Set(userId, sessionId);
            HttpContext.Current.Application[Global.WapLoginUserKey] = loginUsers;
        }

        /// <summary>
        /// 作者：Malei
        /// 时间：2013.11.14 PM
        /// 描述：移除指定用户的登录信息及日志
        /// </summary>
        /// <param name="userId"></param>
        public static void Remove(string userId)
        {
            var loginUsers = HttpContext.Current.Application[Global.WapLoginUserKey] as NameValueCollection;
            if (loginUsers != null)
            {
                loginUsers.Remove(string.Format("{0}", userId));
            }
            HttpContext.Current.Application[Global.WapLoginUserKey] = loginUsers;

            //注销日志 (未完待续...)
        }

        /// <summary>
        /// 作者：Malei 
        /// 时间：2013.11.16
        /// 描述：注销（当前用户）
        /// </summary>
        public static void Logout()
        {
            if (null != CurrLoginUser)
            {
                Remove(CurrLoginUser.openid);
            }
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.Session.Clear();
        }
        /// <summary>
        /// 作者：Malei
        /// 日期：2013-10-30
        /// 描述：登录验证码是否相同 
        /// -------------------------------------
        /// </summary>
        /// <param name="requestCode"></param>
        /// <returns></returns>
        public static bool IsCheckSigninCode(string requestCode)
        {
            var cookie = HttpContext.Current.Request.Cookies[Global.ValidSigninCodeKey];
            if (null == cookie)
            {
                return false;
            }
            var sysCode = cookie.Value;
            return requestCode.Equals(sysCode, StringComparison.CurrentCultureIgnoreCase);
        }
        /// <summary>
        /// 作者：Malei
        /// 日期：2013-10-30
        /// 描述：注册验证码是否相同 
        /// -------------------------------------
        /// </summary>
        /// <param name="requestCode"></param>
        /// <returns></returns>
        public static bool IsCheckSignupCode(string requestCode)
        {
            var cookie = HttpContext.Current.Request.Cookies[Global.ValidSignupCodeKey];
            if (null == cookie)
            {
                return false;
            }
            var sysCode = cookie.Value;
            return requestCode.Equals(sysCode, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
