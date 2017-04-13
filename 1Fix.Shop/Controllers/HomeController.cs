using _1Fix.BLL;
using _1Fix.Common;
using _1Fix.Common.Terminal;
using _1Fix.Entity;
using _1Fix.Entity.Model;
using _1Fix.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ThoughtWorks.QRCode.Codec;
using WxPayAPI;

namespace _1Fix.Shop.Controllers
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
            var query = ConsignmentBLL.GetSellPhoneList();
            return View(query);
        }
        /// <summary>
        /// 手机详细信息(手机详情页面)
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public ActionResult Detail(string sn)
        {

            ConsignmentBLL.UpdateUserClickNum(sn);

            CuSellPhone query = ConsignmentBLL.GetSellPhoneBySn(sn);
            //获取三张
            ViewBag.CheckImagesList = ConsignmentBLL.GetCheckImagesList(sn);


            return View(query);
        }
        /// <summary>
        /// 进入下订单页面
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        [WeiXinAuthorization(true)]
        public ActionResult Order(string sn)
        {
            int userinfoid = WeiXinLoginUserManager.CurrLoginUser.id;
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
        [WeiXinAuthorization(true)]
        public JsonResult SaveUserAddress(UserAddress useraddress, string valid_code)
        {
            int result = 0;
            PhoneValidCode phonevalidcode = Session["1FixUserPhoneValidCode"] as PhoneValidCode;
            if (phonevalidcode != null)
            {
                if (phonevalidcode.ValidCode != valid_code)
                {
                    result = -2;//手机验证码错误
                }
                else
                {
                    useraddress.openid = WeiXinLoginUserManager.CurrLoginUser.openid;
                    useraddress.UserInfoID = WeiXinLoginUserManager.CurrLoginUser.id;
                    useraddress.AddDate = DateTime.Now;
                    useraddress.is_default = 1;
                    result = UserAddressBLL.AddUserAddress(useraddress);
                }
            }
            else
            {
                result = -1;//手机验证码失效(3分钟有效)
            }
            return Json(new { status = result, msg = useraddress.ID });
        }
        /// <summary>
        /// 保存订单
        /// </summary>
        /// <param name="useraddressid"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        [WeiXinAuthorization(true)]
        public JsonResult SaveOrder(int useraddressid, string sn, decimal orderprice, int ordernum)
        {
            int result = 0;
            SellPhoneOrder sellphoneorder = new SellPhoneOrder();
            //订单主表
            sellphoneorder.UserAddressID = useraddressid;
            sellphoneorder.OrderPrice = orderprice;
            sellphoneorder.OrderNum = ordernum;
            sellphoneorder.OpenID = WeiXinLoginUserManager.CurrLoginUser.openid;
            sellphoneorder.UserInfoID = WeiXinLoginUserManager.CurrLoginUser.id;
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
            return Json(new { status = result, msg = sellphoneorder.OrderNo });
        }
        /// <summary>
        /// 生成订单，进入待支付页面
        /// </summary>
        /// <returns></returns>
        [WeiXinAuthorization(true)]
        public ActionResult Payment(string orderno)
        {
            var query = SellPhoneOrderBLL.GetSellPhoneOrder(orderno);
            return View(query);
        }
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
        /// 搜索页面分页
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult SearchList(string key, string jb, string phonemodelid, string color, int? orderbyprice, int? orderbydate, int? orderbyclicknum, int PageIndex = 1)
        {
            Paging page = new Paging();
            page.PageIndex = PageIndex;
            page.PageSize = 16;
            color = Server.UrlDecode(color);
            var query = ConsignmentBLL.GetSellPhoneListByKey(key, jb, phonemodelid, color, orderbyprice, orderbydate, orderbyclicknum, page);
            return View(query);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
