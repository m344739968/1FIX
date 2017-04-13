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
        /// 填写寄售信息的页面
        /// </summary>
        /// <returns></returns>
        [WapAuthorization(true)]
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
        [WapAuthorization(true)]
        public JsonResult SaveSellBaseInfo(SellBaseInfo sellbaseinfo)
        {
            sellbaseinfo.OpenID = WapLoginUserManager.CurrLoginUser.openid;
            sellbaseinfo.UserInfoID = WapLoginUserManager.CurrLoginUser.id;
            sellbaseinfo.Status = 0; //未审核
            sellbaseinfo.IPAddress = Request.UserHostAddress;
            sellbaseinfo.AddDate = DateTime.Now;
            int result = SellBaseInfoBLL.AddSellPhoneOrder(sellbaseinfo);
            return Json(new { status = result, msg = "" });
        }
    }
}