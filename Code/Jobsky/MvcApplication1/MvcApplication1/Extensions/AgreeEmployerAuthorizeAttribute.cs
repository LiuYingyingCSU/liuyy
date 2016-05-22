using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcApplication1.Repository;

namespace System.Web.Mvc
{
    public class AgreeEmployerAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 核心【验证用户是否登陆并且已经通过审核】
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
            string _employerPwd = _cookie["EmployerPwd"];   //cookie里存的先自己加密，再url加密的密码   
            string _isDelete = _cookie["IsDelete"];
            if (_employerAccount == "" || _employerPwd == "") return false;
            EmployerRepository _employerRsy = new EmployerRepository();
            if (_employerRsy.Authentication(_employerAccount, httpContext.Server.UrlDecode(_employerPwd)) == 1)
            {
                //再检验是否通过审核
                if (_isDelete == "0")
                {
                    return true;
                }
                else
                {
                    //账号密码正确但是没通过审核，就跳转到待审核页面
                    httpContext.Response.Redirect("~/Employer/WaitAudit?Time=-1");
                    return false;
                }
            }
            else return false;
        }
    }
}