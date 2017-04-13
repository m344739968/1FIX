using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace _1Fix.Common
{
    public class AuthorizationAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        private bool IsValid { get; set; }
        public AuthorizationAttribute()
            : this(false)
        {
        }

        public AuthorizationAttribute(bool isValid)
        {
            IsValid = isValid;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //登录、权限 验证
            if (IsValid)
            {
                //是否登录（验证）
                if (!LoginUserManager.IsLogin)
                {
                    if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.HttpContext.Response.Write("<script>alert('用户登录超时，请重新登录后再操作');window.location.href='/';</script>");
                        filterContext.HttpContext.Response.Flush();
                        filterContext.HttpContext.Response.End();
                    }
                    else
                    {
                        filterContext.Result = new EmptyResult();
                        filterContext.HttpContext.Response.Redirect(Global.LoginPageUrl, true);
                        filterContext.HttpContext.Response.Flush();
                        filterContext.HttpContext.Response.End();
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
