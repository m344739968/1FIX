using _1Fix.BLL;
using _1Fix.Common;
using _1Fix.Entity;
using _1Fix.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace _1Fix.Manager.Controllers
{
    public class SuperUserController : Controller
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
        public ActionResult Index(string username, int PageIndex = 1)
        {
            Paging page = new Paging();
            page.PageIndex = PageIndex;
            var query = SuperUserBLL.GetSuperUserList(username, page);

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
            SuperUser query = null;
            if (id > 0)
            {
                query = SuperUserBLL.GetSuperUserByID(id);
            }
            return View(query);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveSuperUser(SuperUser model)
        {
            int result = 0;
            if (model != null)
            {
                result = SuperUserBLL.Update(model);
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
                result = SuperUserBLL.Delete(id);
            }
            return Json(new { status = result, msg = "" });
        }
        public ActionResult Role(int? id)
        {
            SuperUser query = null;
            XmlNodeList nodelist = XmlHelper.GetNodeList(Server.MapPath(Global.MenuPath), "/menu/group");
            List<group> menulist = null;
            if (nodelist != null && nodelist.Count > 0)
            {
                menulist = new List<group>();
                foreach (XmlNode item in nodelist)
                {
                    group group = new group();
                    group.id = item.Attributes["id"].Value.ToString();
                    group.text = item.Attributes["text"].Value.ToString();
                    group.url = item.Attributes["url"].Value.ToString();
                    menulist.Add(group);
                }
            }
            //菜单配置
            ViewBag.MenuList = menulist;
            if (id > 0)
            {
                query = SuperUserBLL.GetSuperUserByID(id);
            }
            return View(query);
        }
        /// <summary>
        /// 保存权限数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveSuperUserRole(int id, string[] toplevel)
        {
            int result = 0;
            if (id > 0)
            {
                if (toplevel != null && toplevel.Length > 0)
                {
                    string level = string.Empty;
                    foreach (string val in toplevel)
                    {
                        level += "," + val;
                    }
                    level += ",";
                    result = SuperUserBLL.UpdateRole(id, level);
                }
                else
                {
                    result = -1;
                }

            }
            return Json(new { status = result, msg = "" });
        }
    }
}
