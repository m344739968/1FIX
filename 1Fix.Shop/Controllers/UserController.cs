using _1Fix.BLL;
using _1Fix.Common;
using _1Fix.Common.Terminal;
using _1Fix.Entity;
using _1Fix.Entity.Model;
using _1Fix.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace _1Fix.Shop.Controllers
{
    ///微信手机扫描登陆后返回地址如下
    ///http://jy.1fix.cn/?code=041bd4a1395ba7be36231c5e7112c601&state=1438996839
    ///http://192.168.199.125：8015?code=041bd4a1395ba7be36231c5e7112c601&state=1438996839
    [TerminalAuthorization(true)]
    public class UserController : Controller
    {
        public string WeiXinUrlGetToken = "https://api.weixin.qq.com/sns/oauth2/access_token";
        public string WeiXinUrlGetUser = "https://api.weixin.qq.com/sns/userinfo";
        /// <summary>
        /// 微信登录页面
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public ActionResult WeiXinSignin(string Code, string Type)
        {
            if (!string.IsNullOrEmpty(Code))
            {
                return Valid(Code, Type);
            }
            return View();
        }
        /// <summary>
        /// 验证微信登录
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public ActionResult Valid(string Code, string Type)
        {
            WeiXinUrlGetToken = WeiXinUrlGetToken + "?appid=" + Global.WeiXinAppID + "&secret=" + Global.WeiXinSecret + "&code=" + Code + "&grant_type=authorization_code";
            string TokenStr = HttpRequestHelper.HttpGet(WeiXinUrlGetToken, "");
            if (!string.IsNullOrEmpty(TokenStr))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                try
                {
                    WeiXinToken weixintoken = js.Deserialize<WeiXinToken>(TokenStr);
                    WeiXinUrlGetUser = WeiXinUrlGetUser + "?access_token=" + weixintoken.access_token + "&openid=" + weixintoken.openid + "";
                    string UserStr = HttpRequestHelper.HttpGet(WeiXinUrlGetUser, "");
                    WeiXinUser weixinuser = js.Deserialize<WeiXinUser>(UserStr);
                    if (weixinuser.openid != null)
                    {
                        //微信登录
                        if (Type == "1")
                        {
                            //检测该微信账号是否已经绑定账号密码
                            UserInfo userinfo = UserInfoBLL.IsExists(weixinuser.openid);
                            if (userinfo == null)
                            {
                                //尚未绑定微信号不能登录
                                return Content("<script>window.alert('尚未绑定微信账号，不能使用微信登录！');location.href='/';</script>");
                            }
                            else
                            {
                                if (userinfo.username == null || userinfo.username == "")
                                {
                                    //尚未绑定微信号不能登录
                                    return Content("<script>window.alert('尚未绑定微信账号，不能使用微信登录！');location.href='/';</script>");
                                }
                                else
                                {
                                    //修改最后登录时间
                                    weixinuser.username = userinfo.username;
                                    weixinuser.nickname = userinfo.nickname;
                                    weixinuser.password = userinfo.password;
                                    WeiXinLoginUserManager.SignLoginUser(weixinuser);
                                    if (Request["fromurl"] != null)
                                    {
                                        var fromurl = Server.UrlDecode(Request["fromurl"].ToString());
                                        return Redirect(fromurl);
                                    }
                                    else
                                    {
                                        return RedirectToAction("Search", "Home");
                                    }
                                }
                            }
                        }
                        else
                        {
                            //绑定微信账号
                            if (WeiXinLoginUserManager.IsLogin)
                            {
                                //检测该微信账号是否已经绑定账号密码
                                UserInfo userinfo = UserInfoBLL.IsExists(weixinuser.openid);
                                if (userinfo != null)
                                {
                                    //一个微信号只能绑定一个账号
                                    return Content("<script>window.alert('该微信号已经绑定了其他账号，一个微信号不能重复绑定！');location.href='/';</script>");
                                }
                                else
                                {
                                    weixinuser.id = WeiXinLoginUserManager.CurrLoginUser.id;
                                    if (UserInfoBLL.BindWeixin(weixinuser))
                                    {
                                        //关联上微信Openid
                                        WeiXinLoginUserManager.CurrLoginUser.openid = weixinuser.openid;
                                        return RedirectToAction("Search", "Home");
                                    }
                                    else
                                    {
                                        return Content("<script>window.alert('发生异常，请重新绑定微信账号！');location.href='/';</script>");
                                    }
                                }
                            }
                            else
                            {
                                return RedirectToAction("Search", "Home");
                            }
                        }
                    }
                    else
                    {
                        //跳转到微信登录页面
                        return View("Signin");
                    }
                }
                catch
                {
                    //跳转到微信登录页面
                    return View("Signin");
                }
            }
            //跳转到微信登录页面
            return View("Signin");
        }
        ///// <summary>
        ///// 验证微信登录
        ///// </summary>
        ///// <param name="Code"></param>
        ///// <returns></returns>
        //public ActionResult Valid(string Code)
        //{
        //    WeiXinUrlGetToken = WeiXinUrlGetToken + "?appid=" + Global.WeiXinAppID + "&secret=" + Global.WeiXinSecret + "&code=" + Code + "&grant_type=authorization_code";
        //    string TokenStr = HttpRequestHelper.HttpGet(WeiXinUrlGetToken, "");
        //    if (!string.IsNullOrEmpty(TokenStr))
        //    {
        //        JavaScriptSerializer js = new JavaScriptSerializer();
        //        try
        //        {
        //            WeiXinToken weixintoken = js.Deserialize<WeiXinToken>(TokenStr);
        //            WeiXinUrlGetUser = WeiXinUrlGetUser + "?access_token=" + weixintoken.access_token + "&openid=" + weixintoken.openid + "";
        //            string UserStr = HttpRequestHelper.HttpGet(WeiXinUrlGetUser, "");
        //            WeiXinUser weixinuser = js.Deserialize<WeiXinUser>(UserStr);
        //            if (weixinuser.openid != null)
        //            {
        //                UserInfo userinfo = UserInfoBLL.IsExists(weixinuser.openid);
        //                if (userinfo == null)
        //                {
        //                    //注册新的微信用户
        //                    if (UserInfoBLL.SignUp(weixinuser))
        //                    {
        //                        weixinuser.VipGroup = "1";
        //                        WeiXinLoginUserManager.SignLoginUser(weixinuser);
        //                        if (Request["fromurl"] != null)
        //                        {
        //                            var fromurl = Server.UrlDecode(Request["fromurl"].ToString());
        //                            return Redirect(fromurl);
        //                        }
        //                        else
        //                        {
        //                            return RedirectToAction("Index", "Home");
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    //修改最后登录时间
        //                    UserInfoBLL.Update(weixinuser);
        //                    weixinuser.VipGroup = UserInfoBLL.GetUserInfoByOpenID(weixinuser.openid).VipGroup.ToString();
        //                    WeiXinLoginUserManager.SignLoginUser(weixinuser);
        //                    if (Request["fromurl"] != null)
        //                    {
        //                        var fromurl = Server.UrlDecode(Request["fromurl"].ToString());
        //                        return Redirect(fromurl);
        //                    }
        //                    else
        //                    {
        //                        return RedirectToAction("Index", "Home");
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                //跳转到微信登录页面
        //                return View("Signin");
        //            }
        //        }
        //        catch
        //        {
        //            //跳转到微信登录页面
        //            return View("Signin");
        //        }
        //    }
        //    //跳转到微信登录页面
        //    return View("Signin");
        //}
        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        public ActionResult Signout()
        {
            WeiXinLoginUserManager.Logout();
            return RedirectToAction("Search", "Home");
        }
        /// <summary>
        /// 个人中心
        /// </summary>
        /// <returns></returns>
        [WeiXinAuthorization(true)]
        public ActionResult Center(int id = 1, int PageIndex = 1)
        {
            if (Request.IsAjaxRequest())
            {
                var target = Request.QueryString["target"];
                if (target == "MyOrderList")
                {
                    Paging page = new Paging();
                    page.PageIndex = id;
                    page.PageSize = 5;
                    var query = UserInfoBLL.GetMySellPhoneOrderList(WeiXinLoginUserManager.CurrLoginUser.id, page);
                    return PartialView("MyOrderList", query);
                }
                else if (target == "MySellList")
                {
                    Paging page = new Paging();
                    page.PageIndex = PageIndex;
                    page.PageSize = 5;
                    var query = UserInfoBLL.GetMySellBaseInfoList(WeiXinLoginUserManager.CurrLoginUser.id, page);
                    return PartialView("MySellList", query);
                }
            }
            else
            {
                Paging page = new Paging();
                page.PageIndex = PageIndex;
                page.PageSize = 5;
                ViewBag.MyOrderList = UserInfoBLL.GetMySellPhoneOrderList(WeiXinLoginUserManager.CurrLoginUser.id, page);
                ViewBag.MySellBaseInfoList = UserInfoBLL.GetMySellBaseInfoList(WeiXinLoginUserManager.CurrLoginUser.id, page);
            }
            return View();
        }
        /// <summary>
        /// 我的寄卖
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        [WeiXinAuthorization(true)]
        public ActionResult MySellList(int PageIndex = 1)
        {
            Paging page = new Paging();
            page.PageIndex = PageIndex;
            page.PageSize = 5;
            var query = UserInfoBLL.GetMySellBaseInfoList(WeiXinLoginUserManager.CurrLoginUser.id, page);
            return View(query);
        }
        /// <summary>
        /// 我的订单
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        [WeiXinAuthorization(true)]
        public ActionResult MyOrderList(int PageIndex = 1)
        {
            Paging page = new Paging();
            page.PageIndex = PageIndex;
            page.PageSize = 5;
            var query = UserInfoBLL.GetMySellPhoneOrderList(WeiXinLoginUserManager.CurrLoginUser.id, page);
            return View(query);
        }

        /// <summary>
        /// 修改价格
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        [WeiXinAuthorization(true)]
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
                result = ConsignmentBLL.SaveSellPhonePrice(id, sn, Title, PersonPrice, EnterprisePrice, BigCustomerPrice, WeiXinLoginUserManager.CurrLoginUser.VipGroup);
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


        /// <summary>
        /// 作者：Malei
        /// 时间：2014.06.17 PM
        /// 描述：获取验证码
        /// </summary>
        public void GetSigninCheckCode()
        {
            ValidCode.GetSigninCheckCode();
        }
        /// <summary>
        /// 作者：Malei
        /// 时间：2014.06.17 PM
        /// 描述：获取验证码
        /// </summary>
        public void GetSignupCheckCode()
        {
            ValidCode.GetSignupCheckCode();
        }
        /// <summary>
        /// 注册
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
        public JsonResult SaveSignup(string username, string nickname, string password, string validcode)
        {
            int result = 0;
            if (WeiXinLoginUserManager.IsCheckSignupCode(validcode))
            {
                if (username != null && username != "")
                {
                    if (!UserInfoBLL.IsExistsUserName(username.Trim()))
                    {
                        result = UserInfoBLL.AddUser(username, nickname, SecurityHelper.Encrypt(password, Global.EncryptKey));
                    }
                    else
                    {
                        result = -2;//该账户已经存在
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
        /// 登录页面
        /// </summary>
        /// <param name="Code"></param>
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
            if (WeiXinLoginUserManager.IsCheckSigninCode(validcode))
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
                    WeiXinLoginUserManager.SignLoginUser(weixinuser);
                    result = 1;
                    msg = "登录成功";
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
    }
}
