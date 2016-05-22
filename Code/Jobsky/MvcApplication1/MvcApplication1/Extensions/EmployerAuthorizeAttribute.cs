using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcApplication1.Repository;

namespace System.Web.Mvc
{
    /// <summary>
    /// 用户权限验证
    /// </summary>
    public class EmployerAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 核心【验证用户是否登陆】
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //检查Cookies["User"]是否存在
            if (httpContext.Request.Cookies["Employer"] == null) return false;
            //验证用户名密码是否正确
            HttpCookie _cookie = httpContext.Request.Cookies["Employer"];
            string _employerAccount = _cookie["EmployerAccount"];
            string _employerPwd = _cookie["EmployerPwd"];//cookie里存的先自己加密，再url加密的密码   
            string _isDelete = _cookie["IsDelete"];
            if (_employerAccount == "" || _employerPwd == "") return false;
            //if (Employer.Authentication(_employerAccount, httpContext.Server.UrlDecode(_employerPwd)))
            //{
            //    if (_isDelete != "1")
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            EmployerRepository _employerRsy = new EmployerRepository();
            if (_employerRsy.Authentication(_employerAccount, httpContext.Server.UrlDecode(_employerPwd)) == 1)
            {
                if (_isDelete != "1")
                {
                    //只要登录账号密码匹配，并且不是被删除的记录，都有这个权限
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else return false;
        }
    }
}