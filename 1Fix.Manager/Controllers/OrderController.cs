using _1Fix.BLL;
using _1Fix.Common;
using _1Fix.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _1Fix.Manager.Controllers
{
    public class OrderController : Controller
    {
        //
        // GET: /Order/
        [Authorization(true)]
        public ActionResult Index(string nickname, string orderno, int PageIndex = 1)
        {
            Paging page = new Paging();
            page.PageIndex = PageIndex;
            var query = SellPhoneOrderBLL.GetSellPhoneOrderList(nickname, orderno, page);

            //若为ajax请求则返回部分视图，实现异步加载数据
            if (Request.IsAjaxRequest())
            {
                return PartialView("_List", query);
            }
            return View(query);
        }
        public ActionResult OrderDetail(string orderno)
        {
            List<CuSellPhoneOrderDetail> query = null;
            if (!string.IsNullOrEmpty(orderno))
            {
                query = SellPhoneOrderBLL.GetSellPhoneOrderDetailByOrderNo(orderno);
                return View(query);
            }
            return View(query);
        }
        /// <summary>
        /// 发货页面
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public ActionResult OrderSend(string orderno)
        {
            ViewBag.OrderNo = orderno;
            return View();
        }
        /// <summary>
        /// 保存发货操作
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveOrderSend(string orderno, string sendperson, string sendcourierno)
        {
            int result = SellPhoneOrderBLL.OrderSend(orderno, sendperson, sendcourierno);
            return Json(new { status = result, msg = "" });
        }
    }
}
