using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication1.Models;
using Sysem.Web.Mvc;

namespace MvcApplication1.Controllers
{
    public class StudentController : Controller
    {
        private JobskyDBContext db = new JobskyDBContext();

        //
        // GET: /Student/
        [StudentAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        //登录
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(StudentLogin login)
        {
            if (ModelState.IsValid)
            {
                //检验验证码
                if (Session["VerificationCode"] == null || Session["VerificationCode"].ToString() == "")
                {
                    Error _err = new Error { Title = "验证码不存在", Details = "用户在登录时服务器端验证码为空，或向服务器提交的验证码为空" };
                    return RedirectToAction("Error", "Prompt", _err);
                }
                else if (Session["VerificationCode"].ToString() != login.VerificationCode.ToUpper())
                {
                    ModelState.AddModelError("VerificationCode", "×");
                    return View();
                }
                //检验登录账号和密码
                DataRow dr = Student.Authenciation(login.StudentID, Common.Text.EnCrypt(login.StudentPwd));
                if (dr != null)
                {
                    HttpCookie _cookie = new HttpCookie("Student");
                    _cookie.Values.Add("StudentID", login.StudentID);
                    _cookie.Values.Add("StudentPwd", Server.UrlEncode(Common.Text.EnCrypt(login.StudentPwd)));
                    _cookie.Values.Add("StudentName", dr["StudentName"].ToString());
                    Response.Cookies.Add(_cookie);
                    if (Request.QueryString["ReturnUrl"] != null)
                    {
                        return Redirect(Request.QueryString["ReturnUrl"]);
                    }
                    else return RedirectToAction("Index", "Student");
                }
                else
                {
                    ModelState.AddModelError("Message", "账号或密码错误");
                    return View();
                }
            }
            return View();
        }


        //
        // GET: /Student/Details/5

        public ActionResult Details(string id=null)
        {
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //
        // GET: /Student/Create

        public ActionResult Create()
        {

            return View();
        }

        //
        // POST: /Student/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(student);
        }

        //
        // GET: /Student/Edit/5

        public ActionResult Edit(string id = null)
        {
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //
        // POST: /Student/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        //
        // GET: /Student/Delete/5

        public ActionResult Delete(string id = null)
        {
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //
        // POST: /Student/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}