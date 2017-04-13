using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Fix.Common
{
    public class WeiXinResult
    {

    }
    /// <summary>
    /// 获取微信接口返回Token类
    /// </summary>
    public class WeiXinToken
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
        public string openid { get; set; }
        public string scope { get; set; }
        public string unionid { get; set; }
    }
    public class WeiXinUser
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string openid { get; set; }
        public string nickname { get; set; }
        public string sex { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string headimgurl { get; set; }
        public string[] privilege { get; set; }
        public string unionid { get; set; }

        public string VipGroup { get; set; }

    }
    /// <summary>
    /// 获取微信接口返回Error类
    /// </summary>
    public class WeiXinError
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
    }
}
