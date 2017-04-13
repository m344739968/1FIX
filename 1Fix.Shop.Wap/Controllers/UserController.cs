using _1Fix.BLL;
using _1Fix.Common;
using _1Fix.Common.Terminal;
using _1Fix.Common.Wap;
using _1Fix.Entity;
using _1Fix.Entity.Model;
using _1Fix.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _1Fix.Shop.Wap.Controllers
{
    [TerminalAuthorization(true)]
    public class UserController : Controller
    {
        //
        // GET: /User/
        [WapAuthorization(true)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 作者：Malei
        /// 时间：2014.06.17 PM
        /// 描述：获取登录验证码
        /// </summary>
        public void GetSigninCheckCode()
        {
            ValidCode.GetSigninCheckCode();
        }
        /// <summary>
        /// 作者：Malei
        /// 时间：2014.06.17 PM
        /// 描述：获取注册验证码
        /// </summary>
        public void GetSignupCheckCode()
        {
            ValidCode.GetSignupCheckCode();
        }
        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Signin()
        {
            return View();
        }
        /// <summary>
        /// 登录操作
        /// </summary>
        /// <param name="usernmae"></param>
        /// <param name="password"></param>
        /// <param name="validcode"></param>
        /// <returns></returns>
        public JsonResult SaveSignin(string username, string password, string validcode)
        {
            int result = 0;
            string msg = string.Empty;
            if (WapLoginUserManager.IsCheckSigninCode(validcode))
            {
                UserInfo userinfo = UserInfoBLL.IsExistsUser(username, SecurityHelper.Encrypt(password, Global.EncryptKey));
                if (userinfo != null)
                {
                    WeiXinUser weixinuser = new WeiXinUser();
                    weixinuser.id = userinfo.ID;
                    weixinuser.openid = userinfo.openid;
                    weixinuser.username = userinfo.username;
                    weixinuser.nickname = userinfo.nickname;
                    weixinuser.password = userinfo.password;
                    weixinuser.VipGroup = userinfo.VipGroup.ToString();
                    //手机端session保存
                    WapLoginUserManager.SignLoginUser(weixinuser);
                    string fromurl = "/Home/Index"; //首页面
                    if (Request["fromurl"] != null && Request["fromurl"].ToString() != "")
                    {
                        fromurl = Server.UrlDecode(Request["fromurl"].ToString());
                    }
                    result = 1;
                    msg = fromurl;
                }
                else
                {
                    result = -2; //登录账号或密码错误
                    msg = "账号或密码输入不正确";
                }
            }
            else
            {
                result = -1;//验证码不正确
                msg = "验证码输入不正确";
            }
            return Json(new { status = result, msg = msg });
        }
        /// <summary>
        /// 注册页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Signup()
        {
            return View();
        }
        /// <summary>
        /// 保存注册信息
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveSignup(string username, string nickname, string password, string validcode, string phonevalidcode)
        {
            int result = 0;
            if (WapLoginUserManager.IsCheckSignupCode(validcode))
            {
                PhoneValidCode pvc = Session["1FixUserPhoneWapValidCode"] as PhoneValidCode;
                if (pvc != null)
                {
                    if (pvc.ValidCode != phonevalidcode)
                    {
                        result = -2;//手机验证码错误
                    }
                    else
                    {
                        if (username != null && username != "")
                        {
                            if (!UserInfoBLL.IsExistsUserName(username.Trim()))
                            {
                                result = UserInfoBLL.AddUser(username, nickname, SecurityHelper.Encrypt(password, Global.EncryptKey));
                            }
                            else
                            {
                                result = -3;//该账户已经存在
                            }
                        }
                    }
                }
            }
            else
            {
                result = -1;//验证码不正确
            }
            return Json(new { status = result, msg = "" });
        }
        /// <summary>
        /// 验证注册用户名是否可用
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public ContentResult ValidUserName(string username)
        {
            int result = 0;
            result = UserInfoBLL.IsExistsUserName(username) ? 1 : 0;
            return Content(result.ToString());
        }

        /// <summary>
        /// 手机发送验证码
        /// </summary>
        /// <param name="userphone"></param>
        /// <returns></returns>
        public JsonResult SendPhoneValidCode(string userphone)
        {
            //http://sdk.zhongguowuxian.com:98/ws/BatchSend.aspx?CorpID=GZLKJ0003338&Pwd=zj@666&Mobile=13972174567&Content=短信内容
            string sendurl = Global.MsgUrl;
            int result = 0;
            if (!string.IsNullOrEmpty(userphone))
            {
                PhoneValidCode phonevalidcode = new PhoneValidCode()
                {
                    SendTime = DateTime.Now,
                    ValidCode = ValidCode.GenerateMsgCode(4)
                };
                sendurl = sendurl + "?CorpID=" + Global.MsgCorpID + "&Pwd=" + Global.MsgPwd + "&Mobile=" + userphone + "&Content=" + phonevalidcode.ValidCode;
                string r = HttpRequestHelper.HttpGet(sendurl, "");
                if (r == "1")
                {
                    Session["1FixUserPhoneWapValidCode"] = phonevalidcode;
                    Session.Timeout = 3; //3分钟时间
                    result = 1;
                }
                else
                {
                    result = -1;
                }
            }
            return Json(new { status = result, msg = "" });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WapAuthorization(true)]
        public ActionResult MyOrder(int PageIndex = 1)
        {
            Paging page = new Paging();
            page.PageIndex = PageIndex;
            page.PageSize = 100;
            var query = UserInfoBLL.GetMySellPhoneOrderList(WapLoginUserManager.CurrLoginUser.id, page);
            return View(query);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WapAuthorization(true)]
        public ActionResult MySell(int PageIndex = 1)
        {
            Paging page = new Paging();
            page.PageIndex = PageIndex;
            page.PageSize = 100;
            var query = UserInfoBLL.GetMySellBaseInfoList(WapLoginUserManager.CurrLoginUser.id, page);
            return View(query);
        }


        /// <summary>
        /// 修改价格
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        [WapAuthorization(true)]
        public ActionResult Price(int? id)
        {
            CuSellPhone cs = null;
            if (id > 0)
            {
                cs = ConsignmentBLL.GetSellPhoneBySellBaseInfoID(Convert.ToInt32(id));
            }
            return View(cs);
        }
        /// <summary>
        /// 保存价格
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="PersonPrice"></param>
        /// <param name="EnterprisePrice"></param>
        /// <param name="BigCustomerPrice"></param>
        /// <returns></returns>
        [WapAuthorization(true)]
        public JsonResult SavePrice(int id, string sn, string Title, decimal? PersonPrice, decimal? EnterprisePrice, decimal? BigCustomerPrice)
        {
            int result = 0;
            CuSellPhone sellphone = ConsignmentBLL.GetSellPhoneBySellBaseInfoID(id);
            if (sellphone.SellPhone.Status > 0)
            {
                //已被下单或已被付款
                result = -1;
            }
            else
            {
                result = ConsignmentBLL.SaveSellPhonePrice(id, sn, Title, PersonPrice, EnterprisePrice, BigCustomerPrice, WapLoginUserManager.CurrLoginUser.VipGroup);
            }
            return Json(new { status = result, msg = "" });
        }
        /// <summary>
        /// 上下架操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult Updown(int id, int status)
        {
            int result = 0;
            CuSellPhone sellphone = ConsignmentBLL.GetSellPhoneBySellBaseInfoID(id);
            if (status == 0)
            {
                if (sellphone.SellPhone.Status != -1 && sellphone.SellPhone.Status != 5)
                {
                    //只有未上架,下架才可以执行上架操作
                    result = -1;
                    return Json(new { status = result, msg = "" });
                }
            }
            else if (status == 5)
            {
                if (sellphone.SellPhone.Status != 0)
                {
                    //只有刚上架的才可以下架
                    result = -2;
                    return Json(new { status = result, msg = "" });
                }
            }
            result = ConsignmentBLL.SaveSellPhoneUpDown(id, status);
            return Json(new { status = result, msg = "" });
        }
    }
}
