using _1Fix.BLL;
using _1Fix.Common;
using _1Fix.Entity.Model;
using _1Fix.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace _1Fix.Manager.Controllers
{
    public class CheckFacilityController : Controller
    {
        //
        // GET: /CheckFacility/
        /// <summary>
        /// 用户首页
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        [Authorization(true)]
        public ActionResult Index(string key, string sn, int PageIndex = 1)
        {
            Paging page = new Paging();
            page.PageIndex = PageIndex;
            var query = CheckFacilityBLL.GetCheckFacilityList(key, sn, page);

            //若为ajax请求则返回部分视图，实现异步加载数据
            if (Request.IsAjaxRequest())
            {
                return PartialView("List", query);
            }
            return View(query);
        }
        /// <summary>
        /// 打印操作
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public ActionResult Print(int userid, string username, string sn)
        {
            CuPrintCheckFacility printResult = null;
            if (!string.IsNullOrEmpty(sn.Trim()))
            {
                Devices devices = CheckFacilityBLL.GetCheckFacilityBySn(sn);
                if (devices != null)
                {
                    printResult = new CuPrintCheckFacility();

                    Result r = new Result();
                    r.Devices = devices;

                    printResult.IsSuccess = devices.Status == 1 ? true : false;
                    printResult.Result = r;

                    //记录日志
                    WxLogBLL.AddWxLog("CheckFacility_print", "CheckFacility", "打印检测信息,sn", sn, sn, userid, username);
                }
                else
                {
                    printResult.IsSuccess = false;
                    printResult.Msg = "没有查询到数据";
                }
            }
            else
            {
                printResult.IsSuccess = false;
                printResult.Msg = "请输入IMEI";
            }
            return View(printResult);
        }
        /// <summary>
        /// 保质期首页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorization(true)]
        public ActionResult Quality(int id)
        {
            var query = CheckFacilityBLL.GetCheckFacilityByID(id);
            return View(query);
        }
        /// <summary>
        /// 修改保质期
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        [Authorization(true)]
        public JsonResult SaveQuality(int id, int quality)
        {
            int result = CheckFacilityBLL.UpdateCheckFacilityQuality(id, quality);
            return Json(new { status = result, msg = "" });
        }
    }
}
