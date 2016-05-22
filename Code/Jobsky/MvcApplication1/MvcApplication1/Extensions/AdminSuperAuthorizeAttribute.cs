using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcApplication1.Repository;
using System.Web.Routing;

namespace System.Web.Mvc
{
    public class AdminSuperAuthorizeAttribute : AuthorizeAttribute
    {
        static int a;
        /// <summary>
        /// 核心【验证用户是否登陆】
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //检查Cookies["Admin"]是否存在
            if (httpContext.Request.Cookies["Admin"] == null)
            {
                a = -1;
                //未登录
                return false;
            }
            //验证用户名密码是否正确
            HttpCookie _cookie = httpContext.Request.Cookies["Admin"];
            string _adminAccount = _cookie["AdminAccount"];
            string _adminPwd = _cookie["AdminPwd"];//cookie里存的先自己加密，再url加密的密码   
            string _adminType = _cookie["AdminType"];
            if (_adminAccount == "" || _adminPwd == "")
            {
                a = -1;
                //Cookie不存在
                return false;
            }
            if (Int32.Parse(_adminType) > 2)
            {
                a = 1;
                //不是超级管理员
                return false;
            }
            AdminRepository _adminRsy = new AdminRepository();
            int ans = _adminRsy.Authentication(_adminAccount, httpContext.Server.UrlDecode(_adminPwd));
            if (ans != 1)
            {
                a = -1;
                //账号密码不正确
                return false;
            }
            return true;
        }

        //处理false的请求
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (a == 1)
            {
                string url = filterContext.HttpContext.Request.UrlReferrer.ToString();
                filterContext.HttpContext.Response.Write("<script>alert('对不起，权限不足!');location='/Admin/Index';</script>");
                //filterContext.HttpContext.Response.Redirect(url);
                return;
            }
            else
            {
                string path = filterContext.HttpContext.Request.Path;
                var routeValue = new RouteValueDictionary { 
                    { "Controller", "Admin"}, 
                    { "Action", "Login"},
                    { "ReturnUrl", path}
                };
                filterContext.Result = new RedirectToRouteResult(routeValue);
            }

        }
    }
}