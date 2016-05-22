using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sysem.Web.Mvc
{
    public class StudentAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //检查Cookies["Student"]是否存在
            if (httpContext.Request.Cookies["Student"] == null)
            {
                return false;
            }
            HttpCookie _cookie = httpContext.Request.Cookies["Student"];
            string _studentID = _cookie["StudentID"];
            string _studentName = _cookie["StudentName"];
            string _studentPwd = _cookie["StudentPwd"];
            if (_studentID == null || _studentPwd == null)
            {
                return false;
            }
            return true;
        }
        //处理false的请求 
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)  //授权失败处理
        {
            string path = filterContext.HttpContext.Request.Path;
            var routeValue = new RouteValueDictionary { 
                { "Controller", "Student"}, 
                { "Action", "Login"},
                { "ReturnUrl", path}
            };
            filterContext.Result = new RedirectToRouteResult(routeValue);
        }
    }
}