using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _1Fix.Entity;
using _1Fix.BLL;
using _1Fix.Common;
using _1Fix.Entity.Model;

namespace _1Fix.Manager.Controllers
{
    [Authorization(true)]
    public class ConsignmentController : Controller
    {
        //
        // GET: /Consignment/
        [HttpGet]
        public ActionResult Index(string nickname, string sn, string phonename, int PageIndex = 1)
        {
            Paging page = new Paging();
            page.PageIndex = PageIndex;
            var query = ConsignmentBLL.GetSellPhoneList(nickname, sn, phonename, page);

            //若为ajax请求则返回部分视图，实现异步加载数据
            if (Request.IsAjaxRequest())
            {
                return PartialView("_List", query);
            }
            return View(query);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public ActionResult PhoneInfo(string PhoneID)
        {
            if (!string.IsNullOrEmpty(PhoneID))
            {
                var query = ConsignmentBLL.GetCheckFacilityList(PhoneID);
                return View(query);
            }
            return View(new CheckFacility());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public ActionResult Phone(string sn)
        {
            CheckFacility cf = null;
            if (!string.IsNullOrEmpty(sn))
            {
                var query = ConsignmentBLL.GetCheckFacility(sn);
                return View(query);
            }
            return View(cf);
        }
        /// <summary>
        /// 修改价格
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public ActionResult Price(int id)
        {
            CheckFacility cf = null;
            if (id > 0)
            {
                var query = ConsignmentBLL.GetSellPhoneBySellBaseInfoID(id);
                return View(query);
            }
            return View(cf);
        }
        /// <summary>
        /// 保存价格
        /// </summary>
        /// <param name="id">sellbaseinfoid</param>
        /// <param name="PersonPrice"></param>
        /// <param name="EnterprisePrice"></param>
        /// <param name="BigCustomerPrice"></param>
        /// <returns></returns>
        public JsonResult SavePrice(int id, decimal PersonPrice, decimal EnterprisePrice, decimal BigCustomerPrice)
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
                result = ConsignmentBLL.SaveSellPhonePrice(id, PersonPrice, EnterprisePrice, BigCustomerPrice);
            }
            return Json(new { status = result, msg = "" });
        }
        [HttpGet]
        public ActionResult SellBaseInfo(string contactname, string contactphone, string sn, string courierno, int PageIndex = 1)
        {
            Paging page = new Paging();
            page.PageIndex = PageIndex;
            var query = ConsignmentBLL.GetSellUserList(contactname, contactphone, sn, courierno, page);

            //若为ajax请求则返回部分视图，实现异步加载数据
            if (Request.IsAjaxRequest())
            {
                return PartialView("_SellBaseInfoList", query);
            }
            return View(query);
        }
        /// <summary>
        /// 审核用户寄卖信息
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public ActionResult Audit(int id)
        {
            CheckFacility cf = null;
            if (id > 0)
            {
                var query = ConsignmentBLL.GetSellBaseInfoByID(id);
                return View(query);
            }
            return View(cf);
        }
        /// <summary>
        /// 保存用户审核信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sn"></param>
        /// <param name="remark"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult SaveAudit(int id, string sn, string remark, int status)
        {
            int result = ConsignmentBLL.SellBaseInfoAudit(id, sn, remark, status, LoginUserManager.CurrLoginUser.UserId);
            return Json(new { status = result, msg = "" });
        }
        /// <summary>
        /// 上下架
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public ActionResult UpDown(int id)
        {
            CheckFacility cf = null;
            if (id > 0)
            {
                var query = ConsignmentBLL.GetSellPhoneBySellBaseInfoID(id);
                return View(query);
            }
            return View(cf);
        }
        /// <summary>
        /// 保存上下架记录
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionResult SaveUpDown(int id, int status)
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
        /// 编辑图片页面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        public ActionResult PhoneImage(int id, string sn)
        {
            ViewBag.ID = id;
            ViewBag.Sn = sn;
            var query = CheckImagesBLL.GetCheckImagesListBySn(sn);
            return View(query);
        }
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        public ActionResult SavePhoneImage(int id, string sn)
        {
            int result = 0;
            return Json(new { status = result, msg = "" });
        }
        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DelPhoneImage(int id)
        {
            int result = 0;
            if (id > 0)
            {
                result = CheckImagesBLL.DelCheckImages(id);
            }
            return Json(new { status = result, msg = "" });
        }
    }
}
