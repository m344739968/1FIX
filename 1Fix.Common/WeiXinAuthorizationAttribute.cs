using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace _1Fix.Common
{
    public class WeiXinAuthorizationAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        private bool IsValid { get; set; }
        public WeiXinAuthorizationAttribute()
            : this(false)
        {
        }

        public WeiXinAuthorizationAttribute(bool isValid)
        {
            IsValid = isValid;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //登录、权限 验证
            if (IsValid)
            {
                //******************************************调试信息
                //WeiXinUser weixinuser = new WeiXinUser()
                //{
                //    openid = "oJA3Zwre-ZMZ22hmv4IWGwQLLyO0",
                //    nickname = "MALEI",
                //    sex = "1",
                //    VipGroup = "2",
                //};
                //WeiXinLoginUserManager.SignLoginUser(weixinuser);
                //*****************************************调试信息

                //是否登录（验证）
                if (!WeiXinLoginUserManager.IsLogin)
                {
                    if (filterContext.RequestContext.HttpContext.Request.UrlReferrer == null)
                    {
                        filterContext.Result = new EmptyResult();
                        filterContext.HttpContext.Response.Redirect(Global.WeiXinLoginPageUrl, true);
                        filterContext.HttpContext.Response.Flush();
                        filterContext.HttpContext.Response.End();
                    }
                    else
                    {
                        string fromurl = filterContext.RequestContext.HttpContext.Request.UrlReferrer.ToString();
                        var url = Global.WeiXinLoginPageUrl + "?fromurl=" + filterContext.RequestContext.HttpContext.Server.UrlEncode(fromurl);
                        if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                        {

                            filterContext.HttpContext.Response.Write("<script>window.location.href='" + url + "';</script>");
                            filterContext.HttpContext.Response.Flush();
                            filterContext.HttpContext.Response.End();
                        }
                        else
                        {
                            filterContext.Result = new EmptyResult();
                            filterContext.HttpContext.Response.Redirect(url, true);
                            filterContext.HttpContext.Response.Flush();
                            filterContext.HttpContext.Response.End();
                        }
                    }

                }
            }
        }
        /// <summary>
        /// 执行完页面请求后，记录页面访问日志
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
    }
}
