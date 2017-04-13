using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace _1Fix.Common
{
    public class Global
    {
        #region 系统配置参数
        /// <summary>
        /// 登录页
        /// </summary>
        public static string LoginPageUrl = WebConfigurationManager.AppSettings["LoginPageUrl"];
        /// <summary>
        /// 异常跳转页
        /// </summary>
        public static string ErrorPageUrl = WebConfigurationManager.AppSettings["ErrorPageUrl"];
        /// <summary>
        /// 文件服务站(地址)
        /// </summary>
        public static string FileDomain = "";
        /// <summary>
        /// 形象照处理URL
        /// </summary>
        public static string SetUserIconUrl = FileDomain + "Photo/SetUserIcon";
        /// <summary>
        /// 文件上传请求URL
        /// </summary>
        public static string UploadFileUrl = FileDomain + "File/Upload";

        /// <summary>
        /// 文件下载请求URL
        /// </summary>
        public static string DownloadFileUrl = FileDomain + "File/Download";
        /// <summary>
        /// 后台管理登录用户标识（键）
        /// </summary>
        public static string LoginUserKey = "1FixLoginUserKey";

        public static string EncryptKey = "1FixEncryptKey";
        /// <summary>
        /// PC端登录验证码
        /// </summary>
        public static string ValidSigninCodeKey = "1FixValidSigninCode";
        public static string ValidSignupCodeKey = "1FixValidSignupCode";
        public static string MenuPath = "/xml/menu.xml";//菜单配置路径
        public static string UploadImage = WebConfigurationManager.AppSettings["UploadImage"];
        public static string ShopDomain = WebConfigurationManager.AppSettings["ShopDomain"];

        /// <summary>
        /// 描述：PC端商城配置
        /// </summary>
        //public static string InitPassword = Utility.SecurityHelper.GetMd5(WebConfigurationManager.AppSettings["InitPassword"]);
        public static string WeiXinLoginUserKey = "WeiXin1FixLoginUserKey";
        public static string WeiXinValidCodeKey = "WeiXin1FixLoginUserValidCode";
        public static string WeiXinAppID = WebConfigurationManager.AppSettings["WeiXinAppID"];
        public static string WeiXinSecret = WebConfigurationManager.AppSettings["WeiXinSecret"];
        /// <summary>
        /// PC端微信登录页
        /// </summary>
        public static string WeiXinLoginPageUrl = WebConfigurationManager.AppSettings["WeiXinLoginPageUrl"];

        //短信配置
        public static string MsgUrl = WebConfigurationManager.AppSettings["MsgUrl"];
        public static string MsgCorpID = WebConfigurationManager.AppSettings["MsgCorpID"];
        public static string MsgPwd = WebConfigurationManager.AppSettings["MsgPwd"];
        //支付宝配置
        public static string AlipayPartner = WebConfigurationManager.AppSettings["AlipayExpire"];
        public static string AlipayKey = WebConfigurationManager.AppSettings["AlipayExpire"];
        public static string AlipaySellerEmail = WebConfigurationManager.AppSettings["AlipayExpire"];
        public static string AlipayExpire = WebConfigurationManager.AppSettings["AlipayExpire"];

        /// <summary>
        /// 手机端商城配置
        /// </summary>
        public static string WapLoginUserKey = "Wap1FixLoginUserKey";

        /// <summary>
        /// 优惠券验证地址
        /// </summary>
        public static string ValidCouponsUrl = "http://client.1fix.cn/bindid/shopticket.asp";

        #endregion
        /// <summary>
        /// 作者：malei
        /// 时间：2013.12.26
        /// 描述：获取天气信息
        /// </summary>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        public static string GetWeatherInfo(string cityCode)
        {
            var result = string.Empty;
            var webRequest = WebRequest.Create(string.Format("http://www.weather.com.cn/data/cityinfo/{0}.html", string.IsNullOrEmpty(cityCode) ? "101010100" : cityCode)) as HttpWebRequest;
            if (null != webRequest)
            {
                webRequest.ServicePoint.Expect100Continue = false;
                using (var stream = webRequest.GetResponse().GetResponseStream())
                {
                    if (null != stream)
                    {
                        using (var responseReader = new StreamReader(stream))
                        {
                            result = responseReader.ReadToEnd();
                        }
                    }
                }
            }

            return result;
        }
    }
}
