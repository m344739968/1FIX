using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace _1Fix.Common
{
    public class LoginUserManager
    {        /// <summary>
        /// 作者：Malei
        /// 时间：2014.04.25
        /// 描述：当前登录用户
        /// </summary>
        public static LoginUser CurrLoginUser
        {
            get
            {
                return HttpContext.Current.Session[Global.LoginUserKey] as LoginUser;
            }
        }

        /// <summary>
        /// 作者：Malei
        /// 时间：2013.11.14 PM
        /// 描述：验证用户是否登录 （true/false）
        /// </summary>
        public static bool IsLogin
        {
            get { return (HttpContext.Current.Session[Global.LoginUserKey] != null); }
        }

        /// <summary>
        /// 作者：Malei
        /// 时间：2014.04.25
        /// 描述：标记当前登录用户
        /// </summary>
        /// <param name="user"></param>
        public static void SignLoginUser(LoginUser user)
        {
            //设置当前登录用户
            HttpContext.Current.Session[Global.LoginUserKey] = user;

            //标记当前会话
            Set(string.Format("{0}", user.UserId), HttpContext.Current.Session.SessionID);
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
            var loginUsers = (HttpContext.Current.Application[Global.LoginUserKey] as NameValueCollection) ?? new NameValueCollection();

            loginUsers.Set(userId, sessionId);
            HttpContext.Current.Application[Global.LoginUserKey] = loginUsers;
        }

        /// <summary>
        /// 作者：Malei
        /// 时间：2013.11.14 PM
        /// 描述：移除指定用户的登录信息及日志
        /// </summary>
        /// <param name="userId"></param>
        public static void Remove(int userId)
        {
            var loginUsers = HttpContext.Current.Application[Global.LoginUserKey] as NameValueCollection;
            if (loginUsers != null)
            {
                loginUsers.Remove(string.Format("{0}", userId));
            }
            HttpContext.Current.Application[Global.LoginUserKey] = loginUsers;

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
                Remove(CurrLoginUser.UserId);
            }
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.Session.Clear();
        }

        /// <summary>
        /// 作者：Malei
        /// 时间：2013.11.14 PM
        /// 描述：登录用户是否已改变
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="sessionId">Session ID</param>
        /// <returns>结果(true:已改变,false:未改变)</returns>
        public static bool IsChange(string userId, string sessionId)
        {
            var loginUsers = HttpContext.Current.Application[Global.LoginUserKey] as NameValueCollection;
            if (null == loginUsers)
            { return false; }

            var temp = loginUsers.GetValues(userId);
            if (temp == null || temp.Length > 0)
            {
                return false;
            }

            var oldSessionId = temp[0];
            return !sessionId.Equals(oldSessionId, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 作者：Malei
        /// 日期：2013-10-30
        /// 描述：验证码是否相同 
        /// -------------------------------------
        /// 更新：Malei
        /// 时间：2013.11.16 AM
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
    }
}
