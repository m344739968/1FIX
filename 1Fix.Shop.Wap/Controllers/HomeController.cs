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
    public class HomeController : Controller
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 手机分类页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Category()
        {
            return View();
        }
        /// <summary>
        /// 手机详细信息(手机详情页面)
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public ActionResult Detail(string sn)
        {
            //更新手机被点击次数(关注度)
            ConsignmentBLL.UpdateUserClickNum(sn);

            CuSellPhone query = ConsignmentBLL.GetSellPhoneBySn(sn);
            //获取6张
            ViewBag.CheckImagesList = ConsignmentBLL.GetCheckImagesList(sn);

            return View(query);
        }
        /// <summary>
        /// 进入下订单页面
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        [WapAuthorization(true)]
        public ActionResult Order(string sn)
        {
            int userinfoid = WapLoginUserManager.CurrLoginUser.id;
            ViewBag.UserAddressList = UserAddressBLL.GetUserAddressList(userinfoid);
            ViewBag.CuSellPhone = ConsignmentBLL.GetSellPhoneBySn(sn);
            return View();
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
                    Session["1FixUserPhoneValidCode"] = phonevalidcode;
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
        /// 保存地址
        /// </summary>
        /// <param name="useraddress"></param>
        /// <returns></returns>
        [WapAuthorization(true)]
        public JsonResult SaveUserAddress(UserAddress useraddress, string valid_code)
        {
            int result = 0;
            useraddress.openid = WapLoginUserManager.CurrLoginUser.openid;
            useraddress.UserInfoID = WapLoginUserManager.CurrLoginUser.id;
            useraddress.AddDate = DateTime.Now;
            useraddress.is_default = 1;
            if (useraddress.ID > 0)
            {
                result = UserAddressBLL.UpdateUserAddress(useraddress);
            }
            else
            {
                result = UserAddressBLL.AddUserAddress(useraddress);
            }
            return Json(new { status = result, msg = useraddress.ID });
        }
        public ActionResult DefaultAddress(int id)
        {
            var query = UserAddressBLL.GetUserAddress(id);
            return View(query);
        }
        public ActionResult NewAddress(int? id)
        {
            UserAddress address = new UserAddress();
            if (id > 0)
            {
                address = UserAddressBLL.GetUserAddress(Convert.ToInt32(id));
            }
            return View(address);
        }
        public ActionResult EditAddress(int userinfoid)
        {
            var query = UserAddressBLL.GetUserAddressList(userinfoid);
            return View(query);
        }
        /// <summary>
        /// 保存订单
        /// </summary>
        /// <param name="useraddressid"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        [WapAuthorization(true)]
        public JsonResult SaveOrder(int useraddressid, string sn, decimal orderprice, int ordernum)
        {
            int result = 0;
            SellPhoneOrder sellphoneorder = new SellPhoneOrder();
            //订单主表
            sellphoneorder.UserAddressID = useraddressid;
            sellphoneorder.OrderPrice = orderprice;
            sellphoneorder.OrderNum = ordernum;
            sellphoneorder.OpenID = WapLoginUserManager.CurrLoginUser.openid;
            sellphoneorder.UserInfoID = WapLoginUserManager.CurrLoginUser.id;
            sellphoneorder.OrderDate = DateTime.Now;
            sellphoneorder.IsValid = 1;
            sellphoneorder.IsDelete = 0;
            sellphoneorder.Status = 1;
            sellphoneorder.ExpireDate = DateTime.Now.AddMinutes(10);//订单未付款10分钟过期时间
            //订单详细
            SellPhoneOrderDetail sellphoneorderdetail = new SellPhoneOrderDetail();
            sellphoneorderdetail.Sn = sn;
            sellphoneorderdetail.ItemNo = 1;
            sellphoneorderdetail.PhonePrice = orderprice;
            sellphoneorderdetail.Num = ordernum;
            CuSellPhone sellphone = ConsignmentBLL.GetSellPhoneBySn(sn);
            //已下单，不能在下订单
            if (sellphone.SellPhone.Status > 0)
            {
                result = -1;
            }
            else
            {
                result = SellPhoneOrderBLL.AddSellPhoneOrder(sellphoneorder, sellphoneorderdetail);
            }
            return Json(new { status = result, msg = sellphoneorder.OrderNo, orderprice = orderprice });
        }
        /// <summary>
        /// 生成订单，进入待支付页面
        /// </summary>
        /// <returns></returns>
        [WapAuthorization(true)]
        public ActionResult Payment(string orderno)
        {
            var query = SellPhoneOrderBLL.GetSellPhoneOrder(orderno);
            return View(query);
        }
        /// <summary>
        /// 付款成功页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PaySuccess()
        {
            DateTime now = DateTime.Now;
            Session["1FixPaySuccessTemp"] = now.ToString();
            return View();
        }
        /// <summary>
        /// 搜索页面
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult Search()
        {
            return View();
        }

        /// <summary>
        /// 搜索页面
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult SearchList(string key, string jb, string phonemodelid, string color, string price, int page = 1)
        {
            Paging paging = new Paging();
            paging.PageIndex = page;
            paging.PageSize = 4;
            color = Server.UrlDecode(color);
            var query = ConsignmentBLL.GetSellPhoneListByKey(key, jb, phonemodelid, color, price, paging);
            return View(query);
        }


        #region 测试手机端分页
        public ActionResult Test(int page = 1)
        {
            Paging paging = new Paging();
            paging.PageIndex = page;
            paging.PageSize = 10;
            var query = ConsignmentBLL.GetSellPhoneListByKey("", "", "", "", 1, 1, 1, paging);
            //若为ajax请求则返回部分视图，实现异步加载数据
            if (Request.IsAjaxRequest())
            {
                return PartialView("page", query);
            }
            return View(query);
        }
        public ActionResult Page(int page = 1)
        {
            Paging paging = new Paging();
            paging.PageIndex = page;
            paging.PageSize = 10;
            var query = ConsignmentBLL.GetSellPhoneListByKey("", "", "", "", 1, 1, 1, paging);
            //若为ajax请求则返回部分视图，实现异步加载数据
            if (Request.IsAjaxRequest())
            {
                return PartialView("page", query);
            }
            return View(query);
        }
        #endregion
    }
}
