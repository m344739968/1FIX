using _1Fix.BLL;
using _1Fix.Common;
using _1Fix.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _1Fix.Manager.Controllers
{
    public class MicroMessageController : Controller
    {
        //
        // GET: /WeiXin/
        /// <summary>
        /// 列表页面
        /// </summary>
        /// <param name="phonename"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        [Authorization(true)]
        public ActionResult Index(string phonename, int PageIndex = 1)
        {
            Paging page = new Paging();
            page.PageIndex = PageIndex;
            page.PageSize = 100;
            var query = MicroMessageBLL.GetMicroMessageList(phonename, page);

            //若为ajax请求则返回部分视图，实现异步加载数据
            if (Request.IsAjaxRequest())
            {
                return PartialView("List", query);
            }
            return View(query);
        }
        /// <summary>
        /// 编辑页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            MicroMessage_Money query = null;
            if (id > 0)
            {
                query = MicroMessageBLL.GetMicroMessageByID(id);
            }
            return View(query);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveMicroMessage(MicroMessage_Money model)
        {
            int result = 0;
            if (model != null)
            {
                result = MicroMessageBLL.Update(model);
            }
            return Json(new { status = result, msg = "" });
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult Delete(int id)
        {
            int result = 0;
            if (id > 0)
            {
                result = MicroMessageBLL.Delete(id);
            }
            return Json(new { status = result, msg = "" });
        }
    }
}
