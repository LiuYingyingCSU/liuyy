using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication1.Models;

namespace MvcApplication1.Controllers
{
    public class PromptController : Controller
    {
        //
        // GET: /Notice/
        //雇主后台提示
        public ActionResult Notice(Notice notice)
        {
            return View(notice);
        }
        public ActionResult Error(Error error)
        {
            error.Details = Server.UrlDecode(error.Details);
            error.Cause = Server.UrlDecode(error.Cause);
            error.Solution = Server.UrlDecode(error.Solution);
            return View(error);
        }

        //管理员后台提示
        [ValidateInput(false)]          //这儿相当于给字段加[AllowHtml]，允许传入html
        public ActionResult NoticeAdmin(Notice notice)
        {
            return View(notice);
        }
        public ActionResult ErrorAdmin(Error error)
        {
            error.Details = Server.UrlDecode(error.Details);
            error.Cause = Server.UrlDecode(error.Cause);
            error.Solution = Server.UrlDecode(error.Solution);
            return View(error);
        }
    }
}
