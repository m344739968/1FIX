using _1Fix.BLL;
using _1Fix.Common;
using _1Fix.Common.Terminal;
using _1Fix.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _1Fix.Shop.Controllers
{
    [TerminalAuthorization(true)]
    public class ConsignController : Controller
    {
        //
        // GET: /Consign/
        /// <summary>
        /// 寄售首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 填写接受信息
        /// </summary>
        /// <returns></returns>
        [WeiXinAuthorization(true)]
        public ActionResult Edit()
        {
            ViewBag.SellAdress = SellAddressBLL.GetDefaultSellAdress();
            ViewBag.SellPhoneModelList = SellPhoneModelBLL.GetSellPhoneModelList();
            return View();
        }
        /// <summary>
        /// 保存寄售用户信息
        /// </summary>
        /// <param name="sellbaseinfo"></param>
        /// <returns></returns>
        [WeiXinAuthorization(true)]
        public JsonResult SaveSellBaseInfo(SellBaseInfo sellbaseinfo)
        {
            sellbaseinfo.OpenID = WeiXinLoginUserManager.CurrLoginUser.openid;
            sellbaseinfo.UserInfoID = WeiXinLoginUserManager.CurrLoginUser.id;
            sellbaseinfo.Status = 0; //未审核
            sellbaseinfo.IPAddress = Request.UserHostAddress;
            sellbaseinfo.AddDate = DateTime.Now;
            int result = SellBaseInfoBLL.AddSellPhoneOrder(sellbaseinfo);
            return Json(new { status = result, msg = "" });
        }
    }
}
