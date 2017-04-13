using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _1Fix.Entity;
using _1Fix.BLL;
using _1Fix.Common;


namespace _1Fix.Manager.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /Login/
        public ActionResult Signin()
        {
            return View();
        }
        public ActionResult Signup()
        {
            return View();
        }
        /// <summary>
        /// 用户首页
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        [Authorization(true)]
        public ActionResult Index(string nickname, int PageIndex = 1)
        {
            Paging page = new Paging();
            page.PageIndex = PageIndex;
            var query = UserInfoBLL.GetUserInfoList(nickname, page);

            //若为ajax请求则返回部分视图，实现异步加载数据
            if (Request.IsAjaxRequest())
            {
                return PartialView("_List", query);
            }
            return View(query);
        }
        public ActionResult Edit(int ID)
        {
            if (ID > 0)
            {
                var query = UserInfoBLL.GetUserInfoByID(ID);
                return View(query);
            }
            return View(new UserInfo());
        }
        public JsonResult Save(int id, int vipgroup)
        {
            int result = UserInfoBLL.Update(id, vipgroup);
            return Json(new { status = result, msg = "" });
        }
        /// <summary>
        /// 登录操作
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Signin(string username, string password)
        {
            FunResult result = new FunResult();
            SuperUser user = LoginBLL.Login(username, password);
            if (user != null)
            {
                if (user.active == true)
                {
                    result.Status = true;
                    result.Msg = "登录成功.";
                    LoginUser loginUser = new LoginUser();
                    loginUser.UserId = user.id;
                    loginUser.UserName = username;
                    loginUser.NickName = user.NickName;
                    loginUser.City = user.City;
                    loginUser.TopLevel = user.TopLevel;
                    LoginUserManager.SignLoginUser(loginUser);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    result.Status = false;
                    result.Msg = "账号已被禁用，请联系管理人员.";
                }
            }
            else
            {
                result.Status = false;
                result.Msg = "账号或密码错误.";
            }
            ViewBag.Data = result;
            return View();
        }
        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        public ActionResult Signout()
        {
            LoginUserManager.Logout();
            return RedirectToAction("Signin", "User");
        }
    }
}
