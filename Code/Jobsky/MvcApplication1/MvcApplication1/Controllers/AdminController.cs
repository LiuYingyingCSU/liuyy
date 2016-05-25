using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication1.Models;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;

namespace MvcApplication1.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /News/
        [AdminAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        /**************************登陆部分****************************/
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(AdminLogin login)
        {
            if (ModelState.IsValid)
            {
                //验证验证码
                if (Session["VerificationCode"] == null || Session["VerificationCode"].ToString() == "")
                {
                    Error _e = new Error { Title = "验证码不存在", Details = "在用户注册时，服务器端的验证码为空，或向服务器提交的验证码为空", Cause = Server.UrlEncode("<li>你注册时在注册页面停留的时间过久页已经超时</li><li>您绕开客户端验证向服务器提交数据</li>"), Solution = Server.UrlEncode("返回<a href='" + Url.Action("Register", "Employer") + "'>注册</a>页面，刷新后重新注册") };
                    return RedirectToAction("Error", "Prompt", _e);
                }
                else if (Session["VerificationCode"].ToString() != login.VerificationCode.ToUpper())
                {
                    ModelState.AddModelError("VerificationCode", "×");
                    return View();
                }
                //验证账号密码
                DataRow dr = Admin.Authentication(login.AdminAccount, Common.Text.EnCrypt(login.AdminPwd));
                if (dr != null)
                {
                    //登录成功则记录账号，密码，管理员类型
                    HttpCookie _cookie = new HttpCookie("Admin");
                    _cookie.Values.Add("AdminAccount", login.AdminAccount);
                    //密码先用自己的加密方法，再url加密（防止自己加密后有特殊字符），再存到cookie里
                    //取用的时候，先url解密，再用自己方法解密
                    _cookie.Values.Add("AdminPwd", Server.UrlEncode(Common.Text.EnCrypt(login.AdminPwd)));
                    _cookie.Values.Add("AdminType", dr["AdminType"].ToString());
                    Response.Cookies.Add(_cookie);
                    if (Request.QueryString["ReturnUrl"] != null) return Redirect(Request.QueryString["ReturnUrl"]);
                    else return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ModelState.AddModelError("Message", "账号或密码错误，登陆失败！");
                    return View();
                }
            }
            return View();
        }

        #region 注销登录及获取登录账号
        /// <summary>
        /// 退出系统
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            if (Request.Cookies["Admin"] != null)
            {
                HttpCookie _cookie = Request.Cookies["Admin"];
                _cookie.Expires = DateTime.Now.AddHours(-1);
                Response.Cookies.Add(_cookie);
            }
            Notice _n = new Notice { Title = "成功退出", Details = "您已经成功退出！", DwellTime = 5, NavigationName = "登录界面", NavigationUrl = Url.Action("Login", "Admin") };
            return RedirectToAction("NoticeAdmin", "Prompt", _n);
        }
        #endregion

        #region 绘制验证码
        /// <summary>
        /// 绘制验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult VerificationCode()
        {
            int _verificationLength = 6;
            int _width = 100, _height = 20;
            SizeF _verificationTextSize;
            Bitmap _bitmap = new Bitmap(Server.MapPath("~/Skins/Common/Texture.jpg"), true);
            TextureBrush _brush = new TextureBrush(_bitmap);
            //获取验证码
            string _verificationText = Common.Text.VerificationText(_verificationLength);
            //存储验证码
            Session["VerificationCode"] = _verificationText.ToUpper();
            Font _font = new Font("Arial", 14, FontStyle.Bold);
            Bitmap _image = new Bitmap(_width, _height);
            Graphics _g = Graphics.FromImage(_image);
            //清空背景色
            _g.Clear(Color.White);
            //绘制验证码
            _verificationTextSize = _g.MeasureString(_verificationText, _font);
            _g.DrawString(_verificationText, _font, _brush, (_width - _verificationTextSize.Width) / 2, (_height - _verificationTextSize.Height) / 2);
            _image.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            return null;
        }
        #endregion

        /**************************文章部分****************************/

        #region 添加文章
        //
        // GET: /Admin/NewsCreate
        [AdminAuthorize]
        public ActionResult NewsCreate()
        {
            HttpCookie _cookie = Request.Cookies["Admin"];
            string _adminType = _cookie["AdminType"];   //0为本部，1为湘雅，2为铁道
            if (_adminType == "1" || _adminType == "2")
            {
                CreateNewsTypeByAdminTypeDDL();
            }
            else
            {
                CreateNewsTypeDDL();
            } 
            return View();
        }

        //
        // POST: /Admin/NewsCreate
        [AdminAuthorize]
        [HttpPost]
        [ValidateInput(false)]          //这儿相当于给字段加[AllowHtml]，允许传入html
        public ActionResult NewsCreate(FormCollection collection)
        {
            HttpCookie _cookie = Request.Cookies["Admin"];
            string _adminType = _cookie["AdminType"];   //0为本部，1为湘雅，2为铁道
            if (_adminType == "1" || _adminType == "2")
            {
                CreateNewsTypeByAdminTypeDDL();
            }
            else
            {
                CreateNewsTypeDDL();
            } 
            News news = new News();
            if (ModelState.IsValid)
            {
                //得到Request过来的值
                news.TypeID = Int32.Parse(Request["TypeID"]);
                news.Title = Request["Title"];
                news.Editor = Request["Editor"];
                news.NewsTime = DateTime.Parse(Request["NewsTime"]);
                news.Content = Request["Content"];

                //先给附件默认赋空值
                news.FileAddr = "";

                #region 附件上传
                //处理附件上传
                if (Request.Files["FileAddr"].FileName != "")
                {
                    //支持的类型
                    List<string> FileType = new List<string>();
                    FileType.Add("pdf");
                    FileType.Add("txt");
                    FileType.Add("xls");
                    FileType.Add("xlsx");
                    FileType.Add("doc");
                    FileType.Add("docx");

                    FileType.Add("rar");
                    FileType.Add("zip");

                    //获取文件
                    HttpPostedFileBase uploadFile = Request.Files["FileAddr"];
                    //判断是否为空文件
                    if (uploadFile == null)
                    {
                        return Json(new { message = "附件为空" });
                    }

                    //截取文件后缀名,判断文件类型是否被支持
                    if (FileType.Contains(uploadFile.FileName.Substring(uploadFile.FileName.LastIndexOf(".") + 1)))
                    {
                        var fileTime = DateTime.Now.ToFileTime().ToString() + Path.GetFileName(uploadFile.FileName);//新命名的文件名（包含时间的整数形式）
                        var tofileName = Path.Combine(Request.MapPath("~/NewsUploadFiles"), fileTime);//把两个名字连接起来
                        try
                        {
                            if (Directory.Exists(Server.MapPath("~/NewsUploadFiles")) == false)//如果不存在就创建file文件夹
                            {
                                Directory.CreateDirectory(Server.MapPath("~/NewsUploadFiles"));
                            }
                            uploadFile.SaveAs(tofileName);
                            news.FileAddr = fileTime;//获得上传后的文件名
                        }
                        catch
                        {
                            return Json(new { message = "附件保存失败" });
                        }
                    }
                    else
                    {
                        return Json(new { message = Request.Files.Count + uploadFile.FileName + "附件格式错误" });//不支持此格式文件上传
                    }
                }
                #endregion

                //插入数据库
                int id = -1;
                if ((id = News.Insert(news)) > 0)
                {
                    //Notice _n = new Notice { Title = "添加文章成功", Details = "您已经成功添加一篇文章，文章标题为：" + Request["Title"], DwellTime = 5, NavigationName = "文章列表", NavigationUrl = Url.Action("NewsList", "Admin") };                                       
                    Notice _n = new Notice { Title = "添加文章成功", Details = "您已经成功添加一篇文章，文章标题为：" + Request["Title"] +"<a href='" + Url.Action("NewsDetails/" + id, "Home") + "' target='_blank'>预览文章</a>", DwellTime = 60, NavigationName = "文章列表", NavigationUrl = Url.Action("NewsList", "Admin") };
                    return RedirectToAction("NoticeAdmin", "Prompt", _n);
                }
                //返回-1说明出错
                else
                {
                    Error _e = new Error { Title = "添加文章失败", Details = "添加文章失败!请重新添加", Cause = Server.UrlEncode("<li>你添加文章时在添加文章页面停留的时间过久页已经超时</li><li>您绕开客户端验证向服务器提交数据</li>"), Solution = Server.UrlEncode("返回<a href='" + Url.Action("NewsCreate", "Admin") + "'>文章添加</a>页面，刷新后重新添加") };
                    return RedirectToAction("ErrorAdmin", "Prompt", _e);
                }
            }
            return View();
        }
        #endregion

        #region 添加图片新闻
        [AdminAuthorize]
        public ActionResult PictureNewsCreate()
        {
            return View();
        }

        [AdminAuthorize]
        [HttpPost]
        [ValidateInput(false)]          //这儿相当于给字段加[AllowHtml]，允许传入html
        public ActionResult PictureNewsCreate(FormCollection collection)
        {
            //判断字段是否符合标准
            //第一部分：文章
            News news = new News();

            news.Title = Request["Title"].Trim();
            news.TypeID = 6;
            news.Content = Request["Content"].Trim();
            news.Editor = Request["Editor"].Trim();
            news.NewsTime = Convert.ToDateTime(Request["NewsTime"].Trim());
            news.FileAddr = "";//下面上传文件时会赋值
            string ImgAddr = "";//下面图片文件上传处理时会赋值
            int Rank = Convert.ToInt32(Request["Rank"].Trim());

            //处理图片文件上传
            if (Request.Files.Count > 0)
            {
                //支持的类型
                List<string> FileType = new List<string>();
                FileType.Add("jpg");
                FileType.Add("gif");
                FileType.Add("png");
                FileType.Add("bmp");
                FileType.Add("jpeg");

                //获取文件
                HttpPostedFileBase uploadFile = Request.Files["ImgAddr"];
                //判断是否为空文件
                if (uploadFile == null)
                {
                    return Json(new { message = "附件为空" });
                }

                //截取文件后缀名,判断文件类型是否被支持
                if (FileType.Contains(uploadFile.FileName.Substring(uploadFile.FileName.LastIndexOf(".") + 1)))
                {
                    var fileTime = DateTime.Now.ToFileTime().ToString() + Path.GetFileName(uploadFile.FileName);//新命名的文件名（包含时间的整数形式）
                    var tofileName = Path.Combine(Request.MapPath("~/NewsUploadFiles"), fileTime);//把两个名字连接起来
                    try
                    {
                        if (Directory.Exists(Server.MapPath("~/NewsUploadFiles")) == false)//如果不存在就创建file文件夹
                        {
                            Directory.CreateDirectory(Server.MapPath("~/NewsUploadFiles"));
                        }
                        uploadFile.SaveAs(tofileName);
                        ImgAddr = fileTime;//获得上传后的文件名
                    }
                    catch
                    {
                        return Json(new { message = "附件保存失败" });
                    }
                }
                else
                {
                    return Json(new { message = Request.Files.Count + uploadFile.FileName + "附件格式错误" });//不支持此格式文件上传
                }
            }

            //处理附件上传
            if (Request.Files.Count > 1)//此处要大于1才行，第一个一定是图片
            {
                //支持的类型
                List<string> FileType = new List<string>();
                FileType.Add("pdf");
                FileType.Add("txt");
                FileType.Add("xls");
                FileType.Add("xlsx");
                FileType.Add("doc");
                FileType.Add("docx");

                FileType.Add("rar");
                FileType.Add("zip");

                //获取文件
                HttpPostedFileBase uploadFile = Request.Files["FileAddr"];
                //判断是否为空文件
                if (uploadFile == null)
                {
                    return Json(new { message = "附件为空" });
                }

                //截取文件后缀名,判断文件类型是否被支持
                if (FileType.Contains(uploadFile.FileName.Substring(uploadFile.FileName.LastIndexOf(".") + 1)))
                {
                    var fileTime = DateTime.Now.ToFileTime().ToString() + Path.GetFileName(uploadFile.FileName);//新命名的文件名（包含时间的整数形式）
                    var tofileName = Path.Combine(Request.MapPath("~/NewsUploadFiles"), fileTime);//把两个名字连接起来
                    try
                    {
                        if (Directory.Exists(Server.MapPath("~/NewsUploadFiles")) == false)//如果不存在就创建file文件夹
                        {
                            Directory.CreateDirectory(Server.MapPath("~/NewsUploadFiles"));
                        }
                        uploadFile.SaveAs(tofileName);
                        news.FileAddr = fileTime;//获得上传后的文件名
                    }
                    catch
                    {
                        return Json(new { message = "附件保存失败" });
                    }
                }
                else
                {
                    return Json(new { message = Request.Files.Count + uploadFile.FileName + "附件格式错误" });//不支持此格式文件上传
                }
            }

            try
            {
                int NewsID = News.Insert(news);
                News.PictureNewsInsert(NewsID, ImgAddr, Rank);
                News.PictureNewsUpRank(NewsID, Rank);
                return Json(new { message = "success" });
            }
            catch
            {
                return Json(new { message = "后台运行错误！" });
            }
        }
        #endregion

        #region 修改图片新闻
        // GET: 
        [AdminAuthorize]
        public ActionResult PictureNewsEdit(int id)
        {
            ViewBag.PictureNews = News.PictureNewsSelect(id);
            if (ViewBag.PictureNews.Rows.Count > 0)
            {
                ViewBag.News = News.Select(Convert.ToInt32(ViewBag.PictureNews.Rows[0]["NewsID"]));
            }
            return View();
        }

        [AdminAuthorize]
        [HttpPost]
        [ValidateInput(false)]          //这儿相当于给字段加[AllowHtml]，允许传入html
        public ActionResult PictureNewsEdit(int id, FormCollection collection)
        {
            //判断字段是否符合标准
            //第一部分：文章
            News news = new News();
            news.NewsID = id;
            news.Title = Request["Title"].Trim();
            news.TypeID = 6;
            news.Content = Request["Content"].Trim();
            news.Editor = Request["Editor"].Trim();
            news.NewsTime = Convert.ToDateTime(Request["NewsTime"].Trim());
            news.FileAddr = "";//下面上传文件时会赋值
            string ImgAddr = "";//下面图片文件上传处理时会赋值
            int Rank = Convert.ToInt32(Request["Rank"].Trim());

            //处理图片文件上传
            if (Request.Files.Count > 0)
            {
                //支持的类型
                List<string> FileType = new List<string>();
                FileType.Add("jpg");
                FileType.Add("gif");
                FileType.Add("png");
                FileType.Add("bmp");
                FileType.Add("jpeg");

                //获取文件
                HttpPostedFileBase uploadFile = Request.Files["ImgAddr"];
                //判断是否为空文件
                if (uploadFile == null)
                {
                    //ImgAddr = "";
                }
                else
                {
                    //截取文件后缀名,判断文件类型是否被支持
                    if (FileType.Contains(uploadFile.FileName.Substring(uploadFile.FileName.LastIndexOf(".") + 1)))
                    {
                        var fileTime = DateTime.Now.ToFileTime().ToString() + Path.GetFileName(uploadFile.FileName);//新命名的文件名（包含时间的整数形式）
                        var tofileName = Path.Combine(Request.MapPath("~/NewsUploadFiles"), fileTime);//把两个名字连接起来
                        try
                        {
                            if (Directory.Exists(Server.MapPath("~/NewsUploadFiles")) == false)//如果不存在就创建file文件夹
                            {
                                Directory.CreateDirectory(Server.MapPath("~/NewsUploadFiles"));
                            }
                            uploadFile.SaveAs(tofileName);
                            ImgAddr = fileTime;//获得上传后的文件名
                        }
                        catch
                        {
                            return Json(new { message = "附件保存失败" });
                        }
                    }
                    else
                    {
                        return Json(new { message = Request.Files.Count + uploadFile.FileName + "附件格式错误" });//不支持此格式文件上传
                    }
                }
            }

            if (Request["IsDeleteFile"].ToString().Trim() == "yes")//yes既表示删除原文件（重新上传），也表示第一次上传
            {
                //处理附件上传
                if (Request.Files.Count > 0)//此处要大于0就可以了，第一个不一定是图片
                {
                    //支持的类型
                    List<string> FileType = new List<string>();
                    FileType.Add("pdf");
                    FileType.Add("txt");
                    FileType.Add("xls");
                    FileType.Add("xlsx");
                    FileType.Add("doc");
                    FileType.Add("docx");

                    FileType.Add("rar");
                    FileType.Add("zip");

                    //获取文件
                    HttpPostedFileBase uploadFile = Request.Files["FileAddr"];
                    //判断是否为空文件
                    if (uploadFile == null)
                    {
                        news.FileAddr = "";//return Json(new { message = "附件为空" });
                    }
                    else
                    {
                        //截取文件后缀名,判断文件类型是否被支持
                        if (FileType.Contains(uploadFile.FileName.Substring(uploadFile.FileName.LastIndexOf(".") + 1)))
                        {
                            var fileTime = DateTime.Now.ToFileTime().ToString() + Path.GetFileName(uploadFile.FileName);//新命名的文件名（包含时间的整数形式）
                            var tofileName = Path.Combine(Request.MapPath("~/NewsUploadFiles"), fileTime);//把两个名字连接起来
                            try
                            {
                                if (Directory.Exists(Server.MapPath("~/NewsUploadFiles")) == false)//如果不存在就创建file文件夹
                                {
                                    Directory.CreateDirectory(Server.MapPath("~/NewsUploadFiles"));
                                }
                                uploadFile.SaveAs(tofileName);
                                news.FileAddr = fileTime;//获得上传后的文件名
                            }
                            catch
                            {
                                return Json(new { message = "附件保存失败" });
                            }
                        }
                        else
                        {
                            return Json(new { message = Request.Files.Count + uploadFile.FileName + "附件格式错误" });//不支持此格式文件上传
                        }
                    }
                }
            }
            else
            {
                news.FileAddr = Request["IsDeleteFile"].ToString().Trim();//没有删除，则获得到的就是原来的文件名
            }

            try
            {
                if (News.Update(news))
                {
                    if (ImgAddr == "")
                    {
                        News.PictureNewsUpRank(news.NewsID, Rank);
                    }
                    else {
                        News.PictureNewsUpdate(news.NewsID, ImgAddr, Rank);
                        News.PictureNewsUpRank(news.NewsID, Rank);
                    }
                }
                
                return Json(new { message = "success" });
            }
            catch
            {
                return Json(new { message = "后台运行错误！" });
            }
        }
        #endregion

        #region 图片新闻排序
        // GET: 
        [AdminAuthorize]
        public ActionResult PictureNewsUpRank()
        {
            ViewBag.PictureNews = News.PictureNewsSelectRank();
            return View();
        }

        [AdminAuthorize]
        [HttpPost]
        [ValidateInput(false)]          //这儿相当于给字段加[AllowHtml]，允许传入html
        public ActionResult PictureNewsUpRank(int id,string title,FormCollection collection)
        {
            //int id = Convert.ToInt32(Request["NewsID"]);
            //Response.Write(id);
            DataTable dt = News.PictureNewsSelect(id);
            int rank = Convert.ToInt32(dt.Rows[0]["Rank"]);
            if (rank != 1)
            {
                DataTable dt2 = News.GetPictureNewsByRank(rank - 1);
                if (dt2.Rows.Count > 0)
                {
                    News.PictureNewsUpRank(id, rank - 1);
                    News.PictureNewsUpRank(Convert.ToInt32(dt2.Rows[0]["NewsID"]), rank);
                }
                else
                {
                    News.PictureNewsUpRank(id, rank - 1);
                }
            }
            //RedirectToRoute("PictureNewsUpRank"); 
            Notice _n = new Notice { Title = "图片新闻排序成功", Details = "您已经成功修改图片新闻，标题为：" + title, DwellTime = 5, NavigationName = "图片新闻列表", NavigationUrl = Url.Action("PictureNewsUpRank", "Admin") };
            return RedirectToAction("NoticeAdmin", "Prompt", _n);
            //return View();
        }
        #endregion

        #region 添加大型招聘会
        [AdminAuthorize]
        public ActionResult BigArticleCreate()
        {
            return View();
        }

        [AdminAuthorize]
        [HttpPost]
        [ValidateInput(false)]          //这儿相当于给字段加[AllowHtml]，允许传入html
        public ActionResult BigArticleCreate(FormCollection collection)
        {
            //判断字段是否符合标准
            //第一部分：文章
            News news = new News();

            news.Title = Request["Title"].Trim();
            news.TypeID = 6;
            news.Content = Request["Content"].Trim();
            news.Editor = Request["Editor"].Trim();
            news.NewsTime = Convert.ToDateTime(Request["NewsTime"].Trim());
            news.FileAddr = "";//下面上传文件时会赋值
            string ImgAddr = "";//下面图片文件上传处理时会赋值
            int Rank = Convert.ToInt32(Request["Rank"].Trim());

            //处理图片文件上传
            if (Request.Files.Count > 0)
            {
                //支持的类型
                List<string> FileType = new List<string>();
                FileType.Add("jpg");
                FileType.Add("gif");
                FileType.Add("png");
                FileType.Add("bmp");
                FileType.Add("jpeg");

                //获取文件
                HttpPostedFileBase uploadFile = Request.Files["ImgAddr"];
                //判断是否为空文件
                if (uploadFile == null)
                {
                    return Json(new { message = "附件为空" });
                }

                //截取文件后缀名,判断文件类型是否被支持
                if (FileType.Contains(uploadFile.FileName.Substring(uploadFile.FileName.LastIndexOf(".") + 1)))
                {
                    var fileTime = DateTime.Now.ToFileTime().ToString() + Path.GetFileName(uploadFile.FileName);//新命名的文件名（包含时间的整数形式）
                    var tofileName = Path.Combine(Request.MapPath("~/NewsUploadFiles"), fileTime);//把两个名字连接起来
                    try
                    {
                        if (Directory.Exists(Server.MapPath("~/NewsUploadFiles")) == false)//如果不存在就创建file文件夹
                        {
                            Directory.CreateDirectory(Server.MapPath("~/NewsUploadFiles"));
                        }
                        uploadFile.SaveAs(tofileName);
                        ImgAddr = fileTime;//获得上传后的文件名
                    }
                    catch
                    {
                        return Json(new { message = "附件保存失败" });
                    }
                }
                else
                {
                    return Json(new { message = Request.Files.Count + uploadFile.FileName + "附件格式错误" });//不支持此格式文件上传
                }
            }

            //处理附件上传
            if (Request.Files.Count > 1)//此处要大于1才行，第一个一定是图片
            {
                //支持的类型
                List<string> FileType = new List<string>();
                FileType.Add("pdf");
                FileType.Add("txt");
                FileType.Add("xls");
                FileType.Add("xlsx");
                FileType.Add("doc");
                FileType.Add("docx");

                FileType.Add("rar");
                FileType.Add("zip");

                //获取文件
                HttpPostedFileBase uploadFile = Request.Files["FileAddr"];
                //判断是否为空文件
                if (uploadFile == null)
                {
                    return Json(new { message = "附件为空" });
                }

                //截取文件后缀名,判断文件类型是否被支持
                if (FileType.Contains(uploadFile.FileName.Substring(uploadFile.FileName.LastIndexOf(".") + 1)))
                {
                    var fileTime = DateTime.Now.ToFileTime().ToString() + Path.GetFileName(uploadFile.FileName);//新命名的文件名（包含时间的整数形式）
                    var tofileName = Path.Combine(Request.MapPath("~/NewsUploadFiles"), fileTime);//把两个名字连接起来
                    try
                    {
                        if (Directory.Exists(Server.MapPath("~/NewsUploadFiles")) == false)//如果不存在就创建file文件夹
                        {
                            Directory.CreateDirectory(Server.MapPath("~/NewsUploadFiles"));
                        }
                        uploadFile.SaveAs(tofileName);
                        news.FileAddr = fileTime;//获得上传后的文件名
                    }
                    catch
                    {
                        return Json(new { message = "附件保存失败" });
                    }
                }
                else
                {
                    return Json(new { message = Request.Files.Count + uploadFile.FileName + "附件格式错误" });//不支持此格式文件上传
                }
            }

            try
            {
                int NewsID = News.Insert(news);
                News.PictureNewsInsert(NewsID, ImgAddr, Rank);
                News.PictureNewsUpRank(NewsID, Rank);
                return Json(new { message = "success" });
            }
            catch
            {
                return Json(new { message = "后台运行错误！" });
            }
        }
        #endregion

        #region 修改大型招聘会
        // GET: 
        [AdminAuthorize]
        public ActionResult BigArticleEdit(int id)
        {
            ViewBag.PictureNews = News.PictureNewsSelect(id);
            if (ViewBag.PictureNews.Rows.Count > 0)
            {
                ViewBag.News = News.Select(Convert.ToInt32(ViewBag.PictureNews.Rows[0]["NewsID"]));
            }
            return View();
        }

        [AdminAuthorize]
        [HttpPost]
        [ValidateInput(false)]          //这儿相当于给字段加[AllowHtml]，允许传入html
        public ActionResult BigArticleEdit(int id, FormCollection collection)
        {
            //判断字段是否符合标准
            //第一部分：文章
            News news = new News();
            news.NewsID = id;
            news.Title = Request["Title"].Trim();
            news.TypeID = 6;
            news.Content = Request["Content"].Trim();
            news.Editor = Request["Editor"].Trim();
            news.NewsTime = Convert.ToDateTime(Request["NewsTime"].Trim());
            news.FileAddr = "";//下面上传文件时会赋值
            string ImgAddr = "";//下面图片文件上传处理时会赋值
            int Rank = Convert.ToInt32(Request["Rank"].Trim());

            //处理图片文件上传
            if (Request.Files.Count > 0)
            {
                //支持的类型
                List<string> FileType = new List<string>();
                FileType.Add("jpg");
                FileType.Add("gif");
                FileType.Add("png");
                FileType.Add("bmp");
                FileType.Add("jpeg");

                //获取文件
                HttpPostedFileBase uploadFile = Request.Files["ImgAddr"];
                //判断是否为空文件
                if (uploadFile == null)
                {
                    //ImgAddr = "";
                }
                else
                {
                    //截取文件后缀名,判断文件类型是否被支持
                    if (FileType.Contains(uploadFile.FileName.Substring(uploadFile.FileName.LastIndexOf(".") + 1)))
                    {
                        var fileTime = DateTime.Now.ToFileTime().ToString() + Path.GetFileName(uploadFile.FileName);//新命名的文件名（包含时间的整数形式）
                        var tofileName = Path.Combine(Request.MapPath("~/NewsUploadFiles"), fileTime);//把两个名字连接起来
                        try
                        {
                            if (Directory.Exists(Server.MapPath("~/NewsUploadFiles")) == false)//如果不存在就创建file文件夹
                            {
                                Directory.CreateDirectory(Server.MapPath("~/NewsUploadFiles"));
                            }
                            uploadFile.SaveAs(tofileName);
                            ImgAddr = fileTime;//获得上传后的文件名
                        }
                        catch
                        {
                            return Json(new { message = "附件保存失败" });
                        }
                    }
                    else
                    {
                        return Json(new { message = Request.Files.Count + uploadFile.FileName + "附件格式错误" });//不支持此格式文件上传
                    }
                }
            }

            if (Request["IsDeleteFile"].ToString().Trim() == "yes")//yes既表示删除原文件（重新上传），也表示第一次上传
            {
                //处理附件上传
                if (Request.Files.Count > 0)//此处要大于0就可以了，第一个不一定是图片
                {
                    //支持的类型
                    List<string> FileType = new List<string>();
                    FileType.Add("pdf");
                    FileType.Add("txt");
                    FileType.Add("xls");
                    FileType.Add("xlsx");
                    FileType.Add("doc");
                    FileType.Add("docx");

                    FileType.Add("rar");
                    FileType.Add("zip");

                    //获取文件
                    HttpPostedFileBase uploadFile = Request.Files["FileAddr"];
                    //判断是否为空文件
                    if (uploadFile == null)
                    {
                        news.FileAddr = "";//return Json(new { message = "附件为空" });
                    }
                    else
                    {
                        //截取文件后缀名,判断文件类型是否被支持
                        if (FileType.Contains(uploadFile.FileName.Substring(uploadFile.FileName.LastIndexOf(".") + 1)))
                        {
                            var fileTime = DateTime.Now.ToFileTime().ToString() + Path.GetFileName(uploadFile.FileName);//新命名的文件名（包含时间的整数形式）
                            var tofileName = Path.Combine(Request.MapPath("~/NewsUploadFiles"), fileTime);//把两个名字连接起来
                            try
                            {
                                if (Directory.Exists(Server.MapPath("~/NewsUploadFiles")) == false)//如果不存在就创建file文件夹
                                {
                                    Directory.CreateDirectory(Server.MapPath("~/NewsUploadFiles"));
                                }
                                uploadFile.SaveAs(tofileName);
                                news.FileAddr = fileTime;//获得上传后的文件名
                            }
                            catch
                            {
                                return Json(new { message = "附件保存失败" });
                            }
                        }
                        else
                        {
                            return Json(new { message = Request.Files.Count + uploadFile.FileName + "附件格式错误" });//不支持此格式文件上传
                        }
                    }
                }
            }
            else
            {
                news.FileAddr = Request["IsDeleteFile"].ToString().Trim();//没有删除，则获得到的就是原来的文件名
            }

            try
            {
                if (News.Update(news))
                {
                    if (ImgAddr == "")
                    {
                        News.PictureNewsUpRank(news.NewsID, Rank);
                    }
                    else
                    {
                        News.PictureNewsUpdate(news.NewsID, ImgAddr, Rank);
                        News.PictureNewsUpRank(news.NewsID, Rank);
                    }
                }

                return Json(new { message = "success" });
            }
            catch
            {
                return Json(new { message = "后台运行错误！" });
            }
        }
        #endregion

        #region 文章列表

        static string key = "";
        static string CompanyNature = "";
        static string CompanyBusiness = "";
        [AdminAuthorize]
        public ActionResult NewsList(FormCollection collection)
        {
            key = Request["key"];
            if (key == null)
            {
                key = "";
            }
            int pageindex = 1;
            int pagesize = 15;

            //获取文章
            //ViewBag.News = News.GetListByKey(pageindex, pagesize, key);
            //给插件的四个参数
            int totalpage = News.GetNewsRecordCountByKey(key);
            int count = totalpage % pagesize == 0 ? totalpage / pagesize : totalpage / pagesize + 1;
            if (count == 0) count = 1;
            ViewBag.Count = count;           
            ViewBag.pageindex = pageindex;
            ViewBag.pagesize = pagesize;
            ViewBag.key = key;
            //Response.Write(key);
            return View();
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult PartialArticleListByKey(FormCollection collection)
        {
            int pageindex = Convert.ToInt32(Request["pageindex"]);
            int pagesize = Convert.ToInt32(Request["pagesize"]);
            key = Request["key"].ToString();
            ViewBag.key = key;
            //获取文章

            ViewBag.News = News.GetListByKey(pageindex, pagesize, key);
            //Response.Write(key);
            return PartialView();
        }

        #endregion

        #region 查看文章
        [AdminAuthorize]
        public ActionResult NewsDetails(int id)
        {
            //根据id得到News的信息
            News news = new News();
            DataTable dt = News.Select(id);
            news = news.ConvertDtToNews(dt);

            CreateNewsTypeDDL();
            //设置各个ddl个默认选项
            ViewBag.NewsTypeDDL = new SelectList(ViewBag.NewsTypeDDL, "Value", "Text", news.TypeID);
            return View(news);
        }
        #endregion

        #region 查看待定文章
        [AdminAuthorize]
        public ActionResult TempArticleDetails(int id)
        {
            ViewBag.tb_Article = TempArticle.GetTempArticleByTempArticleID(id);
            ViewBag.tb_Demand = TempDemandInfo.GetTempDemandInfoByTempArticleID(id);
            ViewBag.tb_Employer = Employer.Select(ViewBag.tb_Article.Rows[0]["EditorAccount"].ToString());//雇主信息
            return View();
        }
        #endregion        

        #region 修改文章

        //
        // GET: /News/NewsEdit/5
        [AdminAuthorize]
        public ActionResult NewsEdit(int id)
        {
            //根据id得到News的信息
            News news = new News();
            DataTable dt = News.Select(id);
            news = news.ConvertDtToNews(dt);

            CreateNewsTypeDDL();
            //设置各个ddl个默认选项
            ViewBag.NewsTypeDDL = new SelectList(ViewBag.NewsTypeDDL, "Value", "Text", news.TypeID);
            return View(news);
        }

        //
        // POST: /News/NewsEdit/5
        [AdminAuthorize]
        [HttpPost]
        [ValidateInput(false)]          //这儿相当于给字段加[AllowHtml]，允许传入html
        public ActionResult NewsEdit(int id, FormCollection collection)
        {
            News news = new News();
            if (ModelState.IsValid)
            {
                news.NewsID = id;
                //得到Request过来的值
                news.TypeID = Int32.Parse(Request["TypeID"]);
                news.Title = Request["Title"];
                news.Editor = Request["Editor"];
                news.NewsTime = DateTime.Parse(Request["NewsTime"]);
                news.Content = Request["Content"];

                //先给附件默认赋原来的值
                DataTable dt = News.Select(id);
                news.FileAddr = dt.Rows[0]["FileAddr"].ToString();

                #region 附件上传
                //处理附件上传
                if (Request.Files["FileAddr"].FileName != "")
                {
                    //支持的类型
                    List<string> FileType = new List<string>();
                    FileType.Add("pdf");
                    FileType.Add("txt");
                    FileType.Add("xls");
                    FileType.Add("xlsx");
                    FileType.Add("doc");
                    FileType.Add("docx");

                    FileType.Add("rar");
                    FileType.Add("zip");

                    //获取文件
                    HttpPostedFileBase uploadFile = Request.Files["FileAddr"];
                    //判断是否为空文件
                    if (uploadFile == null)
                    {
                        return Json(new { message = "附件为空" });
                    }

                    //截取文件后缀名,判断文件类型是否被支持
                    if (FileType.Contains(uploadFile.FileName.Substring(uploadFile.FileName.LastIndexOf(".") + 1)))
                    {
                        var fileTime = DateTime.Now.ToFileTime().ToString() + Path.GetFileName(uploadFile.FileName);//新命名的文件名（包含时间的整数形式）
                        var tofileName = Path.Combine(Request.MapPath("~/NewsUploadFiles"), fileTime);//把两个名字连接起来
                        try
                        {
                            if (Directory.Exists(Server.MapPath("~/NewsUploadFiles")) == false)//如果不存在就创建file文件夹
                            {
                                Directory.CreateDirectory(Server.MapPath("~/NewsUploadFiles"));
                            }
                            uploadFile.SaveAs(tofileName);
                            news.FileAddr = fileTime;//获得上传后的文件名
                            //删除之前上传的文件
                            string updateFilePath = "~/NewsUploadFiles/";
                            string FolderPath = System.Web.HttpContext.Current.Server.MapPath(updateFilePath);
                            if (System.IO.File.Exists(FolderPath + dt.Rows[0]["FileAddr"].ToString()))
                            {
                                System.IO.File.Delete(FolderPath + dt.Rows[0]["FileAddr"].ToString());
                            }
                        }
                        catch
                        {
                            return Json(new { message = "附件保存失败" });
                        }
                    }
                    else
                    {
                        return Json(new { message = Request.Files.Count + uploadFile.FileName + "附件格式错误" });//不支持此格式文件上传
                    }
                }
                #endregion

                //更新操作
                if (News.Update(news))
                {
                    //Notice _n = new Notice { Title = "文章修改成功", Details = "您已经成功修改文章，文章标题为：" + Request["Title"], DwellTime = 5, NavigationName = "文章列表", NavigationUrl = Url.Action("NewsList", "Admin") };                   
                    Notice _n = new Notice { Title = "文章修改成功", Details = "您已经成功修改文章，文章标题为：" + Request["Title"] + "  <a href='" + Url.Action("NewsDetails/" + news.NewsID, "Home") + "' target='_blank'>预览文章</a>", DwellTime = 60, NavigationName = "文章列表", NavigationUrl = Url.Action("NewsList", "Admin") };
                    return RedirectToAction("NoticeAdmin", "Prompt", _n);
                }
                //返回false说明出错
                else
                {
                    Error _e = new Error { Title = "修改文章失败", Details = "修改文章失败!请重新修改", Cause = Server.UrlEncode("<li>你修改文章时在修改文章页面停留的时间过久页已经超时</li><li>您绕开客户端验证向服务器提交数据</li>"), Solution = Server.UrlEncode("返回<a href='" + Url.Action("NewsList", "Admin") + "'>文章列表</a>页面，刷新后重新修改") };
                    return RedirectToAction("ErrorAdmin", "Prompt", _e);
                }

            }
            return View();
        }

        #endregion

        #region 删除文章
        [AdminSuperAuthorize]
        //Get
        public void NewsDelete(int id, int databaseID)
        {
            HttpCookie _cookie = Request.Cookies["Admin"];
            string _adminType = _cookie["AdminType"];
            if (Int32.Parse(_adminType.Trim()) <= 2)
            {
                DataTable dt = null;
                if (databaseID == 2)
                {
                    //得到原来的信息，用于删除原来的文件
                    dt = News.Select(id);
                }
                if (News.DeleteByDatabase(id, databaseID))
                {
                    if (databaseID == 2)
                    {
                        //删除之前上传的文件
                        string updateFilePath = "~/NewsUploadFiles/";
                        string FolderPath = System.Web.HttpContext.Current.Server.MapPath(updateFilePath);
                        if (System.IO.File.Exists(FolderPath + dt.Rows[0]["FileAddr"].ToString()))
                        {
                            System.IO.File.Delete(FolderPath + dt.Rows[0]["FileAddr"].ToString());
                        }
                    }
                    Response.Write("<script>alert('删除成功!');location='" + Request.UrlReferrer + "';</script>");
                }
                else
                {
                    Response.Write("<script>alert('删除失败!');location='" + Request.UrlReferrer + "';</script>");
                }
            }
        }
        #endregion

        /**************************用户部分****************************/

        #region 添加用户
        //
        // GET: /Admin/NewsCreate
        [AdminSuperAuthorize]
        public ActionResult AdminCreate()
        {
            CreateAdminTypeDDL();
            return View();
        }

        //
        // POST: /Admin/NewsCreate
        [AdminSuperAuthorize]
        [HttpPost]
        public ActionResult AdminCreate(FormCollection collection)
        {
            CreateAdminTypeDDL();
            AdminRegister adminRegister = new AdminRegister();
            if (ModelState.IsValid)
            {
                //得到Request过来的值
                adminRegister.AdminAccount = Request["AdminAccount"];
                adminRegister.AdminName = Request["AdminName"];
                adminRegister.AdminPwd = Request["AdminPwd"];  //这里密码是解密的
                adminRegister.AdminType = Int32.Parse(Request["AdminType"]);

                //密码加密
                adminRegister.AdminPwd = Common.Text.EnCrypt(adminRegister.AdminPwd);

                //得到Admin
                Admin admin = adminRegister.GetAdmin();

                //插入数据库
                if (Admin.Insert(admin) > 0)
                {
                    Notice _n = new Notice { Title = "添加用户成功", Details = "您已经成功添加一位用户，用户名称为：" + Request["AdminName"], DwellTime = 5, NavigationName = "用户列表", NavigationUrl = Url.Action("AdminList", "Admin") };
                    return RedirectToAction("NoticeAdmin", "Prompt", _n);
                }
                //返回-1说明出错
                else
                {
                    Error _e = new Error { Title = "添加用户失败", Details = "添加用户失败!请重新添加", Cause = Server.UrlEncode("<li>你添加用户时在添加页面停留的时间过久页已经超时</li><li>您绕开客户端验证向服务器提交数据</li>"), Solution = Server.UrlEncode("返回<a href='" + Url.Action("AdminCreate", "Admin") + "'>用户添加</a>页面，刷新后重新添加") };
                    return RedirectToAction("ErrorAdmin", "Prompt", _e);
                }
            }
            return View();
        }
        #endregion

        #region 用户列表
        [AdminSuperAuthorize]
        public ActionResult AdminList(int id = 1)
        {
            int pageindex = id;//当前是第几页，第一页为1
            int pagesize = 10;       //每页显示多少记录
            int totalnum = Admin.GetAdminRecordCount();//获取总共多少管理员记录
            ViewBag.totalpage = (totalnum % pagesize == 0 && totalnum >= pagesize) ? (totalnum / pagesize) : ((totalnum / pagesize) + 1);//获取总页数
            ViewBag.pageindex = pageindex;//当前页
            ViewBag.Admins = Admin.GetAdminList(pageindex, pagesize);//列表数据

            return View();
        }
        #endregion

        #region 修改用户

        //
        // GET: /News/NewsEdit/5
        [AdminSuperAuthorize]
        public ActionResult AdminEdit(int id)
        {
            //根据id得到AdminRegister的信息
            AdminRegister adminRegister = new AdminRegister();
            DataTable dt = Admin.Select(id);
            //根据dt得到AdminRegister对象
            adminRegister = adminRegister.ConvertDtToAdminRegister(dt);

            CreateAdminTypeDDL();
            //设置各个ddl个默认选项
            ViewBag.AdminTypeDDL = new SelectList(ViewBag.AdminTypeDDL, "Value", "Text", adminRegister.AdminType);
            return View(adminRegister);
        }

        //
        // POST: /News/NewsEdit/5
        [AdminSuperAuthorize]
        [HttpPost]
        public ActionResult AdminEdit(int id, FormCollection collection)
        {
            AdminRegister adminRegister = new AdminRegister();
            if (ModelState.IsValid)
            {
                adminRegister.AdminID = id;
                //得到Request过来的值
                adminRegister.AdminName = Request["AdminName"];
                adminRegister.AdminPwd = Request["AdminPwd"];  //这里密码是解密的
                adminRegister.AdminType = Int32.Parse(Request["AdminType"]);

                if (adminRegister.AdminPwd == "") //没有设置新的密码
                {
                    //根据id得到AdminRegister的信息
                    AdminRegister temp = new AdminRegister();
                    DataTable dt = Admin.Select(id);
                    //根据dt得到AdminRegister对象
                    temp = temp.ConvertDtToAdminRegister(dt);

                    //密码用原来的密码
                    adminRegister.AdminPwd = temp.AdminPwd;
                }
                else
                {
                    //密码加密
                    adminRegister.AdminPwd = Common.Text.EnCrypt(adminRegister.AdminPwd);
                }

                //得到Admin
                Admin admin = adminRegister.GetAdmin();

                //更新操作
                if (Admin.Update(admin))
                {
                    Notice _n = new Notice { Title = "用户修改成功", Details = "您已经成功修改用户，用户名称为：" + Request["AdminName"], DwellTime = 5, NavigationName = "用户列表", NavigationUrl = Url.Action("AdminList", "Admin") };
                    return RedirectToAction("NoticeAdmin", "Prompt", _n);
                }
                //返回false说明出错
                else
                {
                    Error _e = new Error { Title = "修改用户失败", Details = "修改用户失败!请重新修改", Cause = Server.UrlEncode("<li>你修改时在修改页面停留的时间过久页已经超时</li><li>您绕开客户端验证向服务器提交数据</li>"), Solution = Server.UrlEncode("返回<a href='" + Url.Action("AdminList", "Admin") + "'>用户列表</a>页面，刷新后重新修改") };
                    return RedirectToAction("ErrorAdmin", "Prompt", _e);
                }

            }
            return View();
        }

        #endregion

        #region 删除用户

        //Get
        [AdminSuperAuthorize]
        public void AdminDelete(int id)
        {
            if (Admin.Delete(id))
            {
                Response.Write("<script>alert('删除成功!');location='" + Request.UrlReferrer + "';</script>");
            }
            else
            {
                Response.Write("<script>alert('删除失败!');location='" + Request.UrlReferrer + "';</script>");
            }
        }
        #endregion

        /**************************雇主部分****************************/

        #region 雇主——查询密码

        //
        // GET: /News/NewsEdit/5
        [AdminSuperAuthorize]
        public ActionResult QueryEmployerPwd()
        {
            Employer employer = new Employer(); ;
            return View(employer);
        }

        //
        // POST: /News/NewsEdit/5
        [AdminSuperAuthorize]
        [HttpPost]
        public ActionResult QueryEmployerPwd(FormCollection collection)
        {
            Employer employer = new Employer();
            if (ModelState.IsValid)
            {
                string email = Request["Email"];
                employer = Employer.GetEmployerPwdByEmail(email);
                return View(employer);
            }
            return View();
        }

        #endregion

        #region 各个DropDownList的初始化
        public void CreateDropDownList()
        {
            #region 单位性质DropDownList
            List<SelectListItem> itemsCompanyNature = new List<SelectListItem>();
            itemsCompanyNature.Add(new SelectListItem { Text = "请选择单位性质(ALL)", Value = null });
            itemsCompanyNature.Add(new SelectListItem { Text = "党政机关", Value = "党政机关" });
            itemsCompanyNature.Add(new SelectListItem { Text = "高等教育单位", Value = "高等教育单位" });
            itemsCompanyNature.Add(new SelectListItem { Text = "科研单位", Value = "科研单位" });
            itemsCompanyNature.Add(new SelectListItem { Text = "医疗卫生单位", Value = "医疗卫生单位" });
            itemsCompanyNature.Add(new SelectListItem { Text = "其他事业单位", Value = "其他事业单位" });
            itemsCompanyNature.Add(new SelectListItem { Text = "国有企业", Value = "国有企业" });
            itemsCompanyNature.Add(new SelectListItem { Text = "民营企业", Value = "民营企业" });
            itemsCompanyNature.Add(new SelectListItem { Text = "三资企业", Value = "三资企业" });
            itemsCompanyNature.Add(new SelectListItem { Text = "部队", Value = "部队" });
            ViewBag.CompanyNature = itemsCompanyNature;
           
            //ViewData["itemsCompanyNature"] = (List<SelectListItem>)itemsCompanyNature;
            #endregion
       

            #region 所属行业DropDownList
            List<SelectListItem> itemsCompanyBusiness = new List<SelectListItem>();
            itemsCompanyBusiness.Add(new SelectListItem { Text = "请选择单位行业(ALL)", Value = null });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "制造业", Value = "制造业" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "信息传输、软件和信息技术服务业", Value = "信息传输、软件和信息技术服务业" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "教育", Value = "教育" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "卫生和社会工作", Value = "卫生和社会工作" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "科学研究和技术服务业", Value = "科学研究和技术服务业" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "建筑业", Value = "建筑业" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "交通运输、仓储和邮政业", Value = "交通运输、仓储和邮政业" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "金融业", Value = "金融业" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "采矿业", Value = "采矿业" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "公共管理、社会保障和社会组织", Value = "公共管理、社会保障和社会组织" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "电力、热力、燃气及水生产和供应业", Value = "电力、热力、燃气及水生产和供应业" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "房地产业", Value = "房地产业" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "租赁和商务服务业", Value = "租赁和商务服务业" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "批发和零售业", Value = "批发和零售业" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "文化、体育和娱乐业", Value = "文化、体育和娱乐业" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "居民服务、修理和其他服务业", Value = "居民服务、修理和其他服务业" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "军队", Value = "军队" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "水利、环境和公告设施管理业", Value = "水利、环境和公告设施管理业" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "住宿和餐饮业", Value = "住宿和餐饮业" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "农、林、牧、渔业", Value = "农、林、牧、渔业" });
            itemsCompanyBusiness.Add(new SelectListItem { Text = "国际组织", Value = "国际组织" });
            ViewBag.CompanyBusiness = itemsCompanyBusiness;
          
            //ViewData["CompanyBusiness"] = (List<SelectListItem>)itemsCompanyBusiness;
            #endregion
        }
        #endregion

        #region 雇主——已审核雇主列表
        [AdminAuthorize]
        public ActionResult EmployerList(int id = 1)
        {
            CreateDropDownList();
            AllModel employerSearch = new AllModel(new Admin(), new News(), new ArticleType(), new Article(), new List<DemandInfo>(), new TempArticle(), new List<TempDemandInfo>(), new PlaceListFirst(), new PlaceListSecond(), new Employer());

            string key = Request["key"];
            string companynature = Request["employer.CompanyNature"];
            string companybusiness = Request["employer.CompanyBusiness"];
            
            #region 设置初始值
            if (key == null)
            {
                key = "";
            }
            if (companynature == null || companynature == "请选择单位性质(ALL)") companynature = "";
            else ViewBag.CompanyNature = new SelectList(ViewBag.CompanyNature, "Value", "Text", companynature);
            if (companybusiness == null || companybusiness == "请选择单位行业(ALL)") companybusiness = "";
            else ViewBag.CompanyBusiness = new SelectList(ViewBag.CompanyBusiness, "Value", "Text", companybusiness);
            #endregion

            employerSearch.employer.CompanyNature = CompanyNature;
            employerSearch.employer.CompanyBusiness = CompanyBusiness;

            int isDelete = 0;//已审核
            //显示全部信息
            int pageindex = 1;
            int pagesize = 10;

            ViewBag.isDelete = isDelete;//记录雇主是否审核
            //获取雇主列表
            ViewBag.Employers = Employer.GetEmployerListByTypeBykeyByCNatureByCBusiness(pageindex, pagesize, isDelete, key, companynature, companybusiness);//列表数据
            //给插件的四个参数
            int totalpage = Employer.GetEmployerRecordCountByTypeByKeyByCNatureByCBusiness(isDelete, key,companynature,companybusiness);//获取总共多少雇主记录
            int count = totalpage % pagesize == 0 ? totalpage / pagesize : totalpage / pagesize + 1;
            if (count == 0) count = 1;
            ViewBag.Count = count;
            ViewBag.pageindex = pageindex;
            ViewBag.pagesize = pagesize;
            ViewBag.key = key;
            ViewBag.nature = companynature;
            ViewBag.business = companybusiness;
            return View(employerSearch);
        }
        #endregion

        #region 雇主详细信息页面
        [AdminAuthorize]
        public ActionResult EmployerDetails(int id)
        {
            Employer employer = Employer.GetEmployerDetailsByID(id);
            return View(employer);
        }
        #endregion

        #region 雇主——待审核雇主列表
        [AdminAuthorize]
        public ActionResult EmployerWaitList(int id = 1)
        {
            CreateDropDownList();
            AllModel employerSearch = new AllModel(new Admin(), new News(), new ArticleType(), new Article(), new List<DemandInfo>(), new TempArticle(), new List<TempDemandInfo>(), new PlaceListFirst(), new PlaceListSecond(), new Employer());
            string key = Request["key"];
            string CompanyNature = Request["employer.CompanyNature"];
            string CompanyBusiness = Request["employer.CompanyBusiness"];
            if (key == null)
            {
                key = "";
            }
            if (CompanyNature == null || CompanyNature == "请选择单位性质(ALL)") CompanyNature = "";
            else ViewBag.CompanyNature = new SelectList(ViewBag.CompanyNature, "Value", "Text", CompanyNature);
            if (CompanyBusiness == null || CompanyBusiness == "请选择单位行业(ALL)") CompanyBusiness = "";
            else ViewBag.CompanyBusiness = new SelectList(ViewBag.CompanyBusiness, "Value", "Text", CompanyBusiness);
            int isDelete = 2;//待审核
            //显示全部信息
            int pageindex = 1;
            int pagesize = 10;

            employerSearch.employer.CompanyNature = CompanyNature;
            employerSearch.employer.CompanyBusiness = CompanyBusiness;

            ViewBag.isDelete = isDelete;//记录雇主是否审核
            //获取雇主列表
            ViewBag.Employers = Employer.GetEmployerListByTypeBykeyByCNatureByCBusiness(pageindex, pagesize, isDelete, key,CompanyNature,CompanyBusiness);//列表数据
            //给插件的四个参数
            int totalpage = Employer.GetEmployerRecordCountByTypeByKeyByCNatureByCBusiness(isDelete, key, CompanyNature, CompanyBusiness);//获取总共多少雇主记录
            int count = totalpage % pagesize == 0 ? totalpage / pagesize : totalpage / pagesize + 1;
            if (count == 0) count = 1;
            ViewBag.Count = count;
            ViewBag.pageindex = pageindex;
            ViewBag.pagesize = pagesize;
            ViewBag.key = key;
            ViewBag.nature = CompanyNature;
            ViewBag.business = CompanyBusiness;
            return View(employerSearch);
        }
        #endregion

        #region 审核雇主信息页面

        //Get
        [AdminAuthorize]
        public ActionResult EmployerAudit(int id)
        {
            Employer employer = Employer.GetEmployerDetailsByID(id);
            return View(employer);
        }

        //Post
        [AdminAuthorize]
        [HttpPost]
        [ValidateInput(false)]          //这儿相当于给字段加[AllowHtml]，允许传入html
        public ActionResult EmployerAudit(int id, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                if (Employer.GetEmployerAuditResult(id) == 1)
                {
                    Notice _n = new Notice { Title = "雇主审核成功", Details = "您已经成功审核雇主信息", DwellTime = 5, NavigationName = "待审核雇主列表", NavigationUrl = Url.Action("EmployerWaitList", "Admin") };
                    return RedirectToAction("NoticeAdmin", "Prompt", _n);
                }
                //返回false说明出错
                else
                {
                    Error _e = new Error { Title = "雇主审核失败", Details = "雇主审核失败!请重新审核", Cause = Server.UrlEncode("<li>你在该页面停留的时间过久页已经超时</li><li>您绕开客户端验证向服务器提交数据</li>"), Solution = Server.UrlEncode("返回<a href='" + Url.Action("EmployerWaitList", "Admin") + "'>待审核雇主列表</a>页面，刷新后重新审核") };
                    return RedirectToAction("ErrorAdmin", "Prompt", _e);
                }
            }
            return View();
        }
        #endregion

        #region 删除雇主信息
        [AdminSuperAuthorize]
        public void EmployerDelete(int id)
        {
            HttpCookie _cookie = Request.Cookies["Admin"];
            string _adminType = _cookie["AdminType"];
            if (Int32.Parse(_adminType.Trim()) <= 2)
            {
                if (Employer.Delete(id))
                {
                    Response.Write("<script>alert('删除成功!');location='" + Request.UrlReferrer + "';</script>");
                }
                else
                {
                    Response.Write("<script>alert('删除失败!');location='" + Request.UrlReferrer + "';</script>");
                }
            }
        }
        #endregion

        #region 把那几个页面合一起
        [AdminAuthorize]
        public ActionResult EmployerArticleWaitList(int id = 1,int isAudit=3)
        {
            isAudit = Convert.ToInt32(Request["isAudit"]);
            string key = Request["key"];
            if(isAudit==0){
                ViewBag.Title = "待审核文章";
            }
            else if(isAudit==1){
                 ViewBag.Title = "已审核文章";
            }
            else
            {
                ViewBag.Title = "已回绝文章";
            }
           
            if (key == null)
            {
                key = "";
            }
            //int isAudit = 1;
            //单位名称，文章标题，文章类型，操作
            //显示全部信息
            DateTime dtbegin, dtend;
            dtbegin = DateTime.Now.AddDays(-10000);
            dtend = DateTime.Now.AddDays(10000);

            int pageindex = 0;
            int pagesize = 10;

            int dttype = -1;   //-1表示显示全部文章

            ViewBag.isAudit = isAudit;//记录文章是否审核
            //获取文章
            HttpCookie _cookie = Request.Cookies["Admin"];
            string _adminType = _cookie["AdminType"];   //0为本部，1为湘雅，2为铁道

            ViewBag.EmployerArticles = Article.GetArticleByAuditByDttypeByPageByKey(pagesize, pageindex, isAudit, dtbegin, dtend, dttype, key, _adminType);//列表数据
            //给插件的四个参数
            int totalpage = Article.GetArticleRecordCountByAuditByDttypeByKey(isAudit, dtbegin, dtend, dttype, key, _adminType);//数据数量
            int count = totalpage % pagesize == 0 ? totalpage / pagesize : totalpage / pagesize + 1;
            if (count == 0) count = 1;
            ViewBag.Count = count;
            ViewBag.pageindex = pageindex + 1;
            ViewBag.pagesize = pagesize;
            ViewBag.key = key;
            return View();
        }
        #endregion

        #region 文章——待审核雇主文章列表
        //[AdminAuthorize]
        //public ActionResult EmployerArticleWaitList(int id = 1)
        //{
        //    string key = Request["key"];
        //    if (key == null)
        //    {
        //        key = "";
        //    }
        //    int isAudit = 1;
        //    //单位名称，文章标题，文章类型，操作
        //    //显示全部信息
        //    DateTime dtbegin, dtend;
        //    dtbegin = DateTime.Now.AddDays(-10000);
        //    dtend = DateTime.Now.AddDays(10000);

        //    int pageindex = 0;
        //    int pagesize = 10;

        //    int dttype = -1;   //-1表示显示全部文章

        //    ViewBag.isAudit = isAudit;//记录文章是否审核
        //    //获取文章
        //    HttpCookie _cookie = Request.Cookies["Admin"];
        //    string _adminType = _cookie["AdminType"];   //0为本部，1为湘雅，2为铁道

        //    ViewBag.EmployerArticles = Article.GetArticleByAuditByDttypeByPageByKey(pagesize, pageindex, isAudit, dtbegin, dtend, dttype, key,_adminType);//列表数据
        //    //给插件的四个参数
        //    int totalpage = Article.GetArticleRecordCountByAuditByDttypeByKey(isAudit, dtbegin, dtend, dttype, key,_adminType);//数据数量
        //    int count = totalpage % pagesize == 0 ? totalpage / pagesize : totalpage / pagesize + 1;
        //    if (count == 0) count = 1;
        //    ViewBag.Count = count;
        //    ViewBag.pageindex = pageindex + 1;
        //    ViewBag.pagesize = pagesize;
        //    ViewBag.key = key;
        //    return View();
        //}
        #endregion

        #region 审核文章
        [AdminAuthorize]
        public ActionResult EmployerArticleAudit(int id, string typeName)
        {
            CreatePlaceListFirstDDL();
            AllModel auditArticle = new AllModel(new Admin(), new News(), new ArticleType(), new Article(), new List<DemandInfo>(), new TempArticle(), new List<TempDemandInfo>(), new PlaceListFirst(), new PlaceListSecond(), new Employer());
            DataTable dt = TempArticle.GetAuditArticleByTempArticleID(id);
            auditArticle.tempArticle.Title = dt.Rows[0]["Title"].ToString();    //文章标题
            auditArticle.employer.MobilePhone = dt.Rows[0]["MobilePhone"].ToString();   //联系人电话
            auditArticle.tempArticle.ArticleDescription = dt.Rows[0]["ArticleDescription"].ToString();    //描述信息（备注）
            auditArticle.tempArticle.AuditInfo = dt.Rows[0]["AuditInfo"].ToString();    //审核未通过的信息

            if (typeName.ToString().Trim() == "专场招聘")
            {
                //判断是否已经选过场地
                DataTable dtTempArticle = TempArticle.GetTempArticleByTempArticleID(id);    //根据TempArticleID得到TempArticle文章信息
                if (dtTempArticle.Rows[0]["PlaceSecondID"].ToString() != "")
                {
                    //如果以前PlaceSecondID字段不为空，则添加场地字段区域默认赋值
                    DataTable dtPlaceSecond = Article.GetPlaceListSecondById(Int32.Parse(dtTempArticle.Rows[0]["PlaceSecondID"].ToString()));
                    auditArticle.placeListFirst.PlaceFirstID = Int32.Parse(dtPlaceSecond.Rows[0]["PlaceFirstID"].ToString());
                    auditArticle.placeListSecond.PlaceName = dtPlaceSecond.Rows[0]["PlaceName"].ToString();
                    auditArticle.placeListSecond.PlaceTime = DateTime.Parse(dtPlaceSecond.Rows[0]["PlaceTime"].ToString());
                }
                else  //以前没选过场地，看雇主是否请求了场地
                {
                    if (dtTempArticle.Rows[0]["PlaceFirstID"].ToString() != "")
                    {
                        //公司请求了校区
                        auditArticle.placeListFirst.PlaceFirstID = Int32.Parse(dtTempArticle.Rows[0]["PlaceFirstID"].ToString());
                    }
                    if (dtTempArticle.Rows[0]["RecruitTime"].ToString() != "")
                    {
                        //公司请求了时间
                        auditArticle.placeListSecond.PlaceTime = DateTime.Parse(dtTempArticle.Rows[0]["RecruitTime"].ToString());
                    }
                    if (dtTempArticle.Rows[0]["RecruitPlace"].ToString() != "")
                    {
                        //公司请求了招聘地点
                        auditArticle.placeListSecond.PlaceName = dtTempArticle.Rows[0]["RecruitPlace"].ToString();
                    }
                }
            }
            //设置ddl默认选项
            ViewBag.PlaceListFirstDDL = new SelectList(ViewBag.PlaceListFirstDDL, "Value", "Text", auditArticle.placeListFirst.PlaceFirstID);

            ViewBag.ArticleID = id;
            ViewBag.TypeName = typeName;
            ViewBag.BigTitle = dt.Rows[0]["BigTitle"].ToString();    //大型招聘会标题
            //Response.Write(id+typeName);
            return View(auditArticle);
        }

        //
        // POST: /News/NewsEdit/5
        [AdminAuthorize]
        [HttpPost]
        public ActionResult EmployerArticleAudit(int id, string typeName, int isAgree, FormCollection collection)
        {
            AllModel auditArticle = new AllModel(new Admin(), new News(), new ArticleType(), new Article(), new List<DemandInfo>(), new TempArticle(), new List<TempDemandInfo>(), new PlaceListFirst(), new PlaceListSecond(), new Employer());
            if (ModelState.IsValid)
            {
                auditArticle.tempArticle.Title = Request["tempArticle.Title"];
                auditArticle.tempArticle.AuditInfo = Request["tempArticle.AuditInfo"];
                if (typeName == "专场招聘")
                {
                    auditArticle.placeListSecond.PlaceFirstID = Int32.Parse(Request["placeListFirst.PlaceFirstID"]);
                    auditArticle.placeListSecond.PlaceName = Request["placeListSecond.PlaceName"];
                    try { DateTime.Parse(Request["placeListSecond.PlaceTime"]); }
                    catch
                    {
                        Error _e = new Error { Title = "审核失败", Details = "审核失败!请重新审核", Cause = Server.UrlEncode("<li>日期格式不正确</li><li>您绕开客户端验证向服务器提交数据</li>"), Solution = Server.UrlEncode("返回<a href='" + Url.Action("EmployerArticleWaitList", "Admin") + "'>待审核雇主文章列表</a>页面，刷新后重新审核") };
                        return RedirectToAction("ErrorAdmin", "Prompt", _e);
                    }
                    auditArticle.placeListSecond.PlaceTime = DateTime.Parse(Request["placeListSecond.PlaceTime"]);
                }
                
                //更新操作
                if (Article.AuditArticle(auditArticle, id, typeName, isAgree))
                {
                    Notice _n = new Notice { Title = "审核成功", Details = "您已经成功审核文章，文章标题为：" + Request["tempArticle.Title"], DwellTime = 5, NavigationName = "待审核雇主文章列表", NavigationUrl = Url.Action("EmployerArticleWaitList", "Admin") };
                    return RedirectToAction("NoticeAdmin", "Prompt", _n);                    
                }
                //返回false说明出错
                else
                {
                    Error _e = new Error { Title = "审核失败", Details = "审核失败!请重新审核", Cause = Server.UrlEncode("<li>你审核时在审核页面停留的时间过久页已经超时</li><li>您绕开客户端验证向服务器提交数据</li>"), Solution = Server.UrlEncode("返回<a href='" + Url.Action("EmployerArticleWaitList", "Admin") + "'>待审核雇主文章列表</a>页面，刷新后重新审核") };
                    return RedirectToAction("ErrorAdmin", "Prompt", _e);
                }
            }
            return View();
        }
        #endregion

        #region 文章——已审核雇主文章列表
        [AdminAuthorize]
        public ActionResult EmployerArticleList(int id = 1)
        {
            string key = Request["key"];
            if (key == null)
            {
                key = "";
            }
            int isAudit = 2;
            //单位名称，文章标题，文章类型，操作
            //显示全部信息
            DateTime dtbegin, dtend;
            dtbegin = DateTime.Now.AddDays(-10000);
            dtend = DateTime.Now.AddDays(10000);

            int pageindex = 0;
            int pagesize = 10;

            int dttype = -1;   //-1表示显示全部文章

            ViewBag.isAudit = isAudit;//记录文章是否审核
            //获取文章
            HttpCookie _cookie = Request.Cookies["Admin"];
            string _adminType = _cookie["AdminType"];   //0为本部，1为湘雅，2为铁道

            ViewBag.EmployerArticles = Article.GetArticleByAuditByDttypeByPageByKey(pagesize, pageindex, isAudit, dtbegin, dtend, dttype, key,_adminType);//列表数据
            //给插件的四个参数
            int totalpage = Article.GetArticleRecordCountByAuditByDttypeByKey(isAudit, dtbegin, dtend, dttype, key,_adminType);//数据数量
            int count = totalpage % pagesize == 0 ? totalpage / pagesize : totalpage / pagesize + 1;
            if (count == 0) count = 1;
            ViewBag.Count = count;
            ViewBag.pageindex = pageindex + 1;
            ViewBag.pagesize = pagesize;
            ViewBag.key = key;
            return View();
        }
        #endregion

        #region 修改已审核雇主文章
        [AdminAuthorize]
        public ActionResult EmployerArticleUpdate(int id, string typeName)
        {
            CreatePlaceListFirstDDL();
            AllModel updateArticle = new AllModel(new Admin(), new News(), new ArticleType(), new Article(), new List<DemandInfo>(), new TempArticle(), new List<TempDemandInfo>(), new PlaceListFirst(), new PlaceListSecond(), new Employer());
            DataTable dt = Article.GetArticleByIDForUpdate(id);
            updateArticle.article.Title = dt.Rows[0]["Title"].ToString();
            if (typeName.Trim() == "专场招聘")
            {
                updateArticle.placeListFirst.PlaceFirstID = Int32.Parse(dt.Rows[0]["PlaceFirstID"].ToString());
                updateArticle.placeListSecond.PlaceName = dt.Rows[0]["PlaceName"].ToString();
                updateArticle.placeListSecond.PlaceTime = DateTime.Parse(dt.Rows[0]["PlaceTime"].ToString());
            }

            //设置ddl默认选项
            ViewBag.PlaceListFirstDDL = new SelectList(ViewBag.PlaceListFirstDDL, "Value", "Text", updateArticle.placeListFirst.PlaceFirstID);

            ViewBag.ArticleID = id;
            ViewBag.TypeName = typeName;
            //Response.Write(id+typeName);
            return View(updateArticle);
        }

        //
        // POST: /News/NewsEdit/5
        [AdminAuthorize]
        [HttpPost]
        public ActionResult EmployerArticleUpdate(int id, string typeName, FormCollection collection)
        {
            AllModel updateArticle = new AllModel(new Admin(), new News(), new ArticleType(), new Article(), new List<DemandInfo>(), new TempArticle(), new List<TempDemandInfo>(), new PlaceListFirst(), new PlaceListSecond(), new Employer());
            if (ModelState.IsValid)
            {
                updateArticle.article.Title = Request["article.Title"];
                if (typeName == "专场招聘")
                {
                    updateArticle.placeListSecond.PlaceFirstID = Int32.Parse(Request["placeListFirst.PlaceFirstID"]);
                    updateArticle.placeListSecond.PlaceName = Request["placeListSecond.PlaceName"];
                    try { DateTime.Parse(Request["placeListSecond.PlaceTime"]); }
                    catch
                    {
                        Error _e = new Error { Title = "修改失败", Details = "修改失败!请重新修改", Cause = Server.UrlEncode("<li>日期格式不正确</li><li>您绕开客户端验证向服务器提交数据</li>"), Solution = Server.UrlEncode("返回<a href='" + Url.Action("EmployerArticleList", "Admin") + "'>已审核雇主文章列表</a>页面，刷新后重新修改") };
                        return RedirectToAction("ErrorAdmin", "Prompt", _e);
                    }
                    updateArticle.placeListSecond.PlaceTime = DateTime.Parse(Request["placeListSecond.PlaceTime"]);
                }

                //更新操作
                if (Article.UpdateArticle(updateArticle, id, typeName))
                {
                    Notice _n = new Notice { Title = "修改成功", Details = "您已经成功修改文章，文章标题为：" + Request["article.Title"], DwellTime = 5, NavigationName = "已审核雇主文章列表", NavigationUrl = Url.Action("EmployerArticleList", "Admin") };
                    return RedirectToAction("NoticeAdmin", "Prompt", _n);
                }
                //返回false说明出错
                else
                {
                    Error _e = new Error { Title = "修改失败", Details = "修改失败!请重新修改", Cause = Server.UrlEncode("<li>你修改时在修改页面停留的时间过久页已经超时</li><li>您绕开客户端验证向服务器提交数据</li>"), Solution = Server.UrlEncode("返回<a href='" + Url.Action("EmployerArticleList", "Admin") + "'>已审核雇主文章列表</a>页面，刷新后重新修改") };
                    return RedirectToAction("ErrorAdmin", "Prompt", _e);
                }
            }
            return View();
        }
        #endregion

        #region 文章——已回绝雇主文章列表
        [AdminAuthorize]
        public ActionResult EmployerArticleDisagreeList(int id = 1)
        {
            string key = Request["key"];
            if (key == null)
            {
                key = "";
            }
            int isAudit = 0;
            //单位名称，文章标题，文章类型，操作
            //显示全部信息
            DateTime dtbegin, dtend;
            dtbegin = DateTime.Now.AddDays(-10000);
            dtend = DateTime.Now.AddDays(10000);

            int pageindex = 0;
            int pagesize = 10;

            int dttype = -1;   //-1表示显示全部文章

            ViewBag.isAudit = isAudit;//记录文章是否审核
            //获取文章
            HttpCookie _cookie = Request.Cookies["Admin"];
            string _adminType = _cookie["AdminType"];   //0为本部，1为湘雅，2为铁道

            ViewBag.EmployerArticles = Article.GetArticleByAuditByDttypeByPageByKey(pagesize, pageindex, isAudit, dtbegin, dtend, dttype, key,_adminType);//列表数据
            //给插件的四个参数
            int totalpage = Article.GetArticleRecordCountByAuditByDttypeByKey(isAudit, dtbegin, dtend, dttype, key,_adminType);//数据数量
            int count = totalpage % pagesize == 0 ? totalpage / pagesize : totalpage / pagesize + 1;
            if (count == 0) count = 1;
            ViewBag.Count = count;
            ViewBag.pageindex = pageindex + 1;
            ViewBag.pagesize = pagesize;
            ViewBag.key = key;
            return View();
        }
        #endregion

        #region 文章搜索——雇主各种文章搜索
        [AdminAuthorize]
        [HttpPost]
        public ActionResult PartialEmployerArticleListByKey(FormCollection collection)
        {
            int pageindex = Convert.ToInt32(Request["pageindex"]) - 1;
            int pagesize = Convert.ToInt32(Request["pagesize"]);
            key = Request["key"].ToString();
            int isAudit = Int32.Parse(Request["isAudit"].ToString()); //0表示已经拒绝，1表示待审核，2表示已经通过审核
            ViewBag.key = key;
            ViewBag.isAudit = isAudit;
            //获取文章
            HttpCookie _cookie = Request.Cookies["Admin"];
            string _adminType = _cookie["AdminType"];   //0为本部，1为湘雅，2为铁道
            //显示全部信息
            DateTime dtbegin, dtend;
            dtbegin = DateTime.Now.AddDays(-10000);
            dtend = DateTime.Now.AddDays(10000);
            int dttype = -1;   //-1表示显示全部文章
            ViewBag.EmployerArticles = Article.GetArticleByAuditByDttypeByPageByKey(pagesize, pageindex, isAudit, dtbegin, dtend, dttype, key,_adminType);//列表数据
            //Response.Write(key);
            return PartialView();
        }
        #endregion

        #region 雇主搜索——待审核雇主，已审核雇主
        [AdminAuthorize]
        [HttpPost]
        public ActionResult PartialEmployerListByKey(FormCollection collection)
        {
            CreateDropDownList();
            int pageindex = Convert.ToInt32(Request["pageindex"]);
            int pagesize = Convert.ToInt32(Request["pagesize"]);
            key = Request["key"].ToString();
            CompanyNature = Request["CompanyNature"].ToString();
            CompanyBusiness = Request["CompanyBusiness"].ToString();
            int isDelete = Int32.Parse(Request["isDelete"].ToString()); //0表示已经通过审核，2表示待审核

            ViewBag.key = key;
            ViewBag.nature = CompanyNature;
            ViewBag.business = CompanyBusiness;
            ViewBag.isDelete = isDelete;
            //ViewBag.nature = nature;
            //ViewBag.business = business;
            //获取雇主
            ViewBag.Employers = Employer.GetEmployerListByTypeBykeyByCNatureByCBusiness(pageindex, pagesize, isDelete, key,CompanyNature,CompanyBusiness);//列表数据
            return PartialView();
        }
        #endregion

        #region 删除雇主文章信息
        [AdminSuperAuthorize]
        public void EmployerArticleDelete(int id, int isAudit)
        {
            HttpCookie _cookie = Request.Cookies["Admin"];
            string _adminType = _cookie["AdminType"];
            if (Int32.Parse(_adminType.Trim()) <= 2)
            {
                if (Article.DeleteEmployerArticle(id, isAudit))
                {
                    Response.Write("<script>alert('删除成功!');location='" + Request.UrlReferrer + "';</script>");
                }
                else
                {
                    Response.Write("<script>alert('删除失败!');location='" + Request.UrlReferrer + "';</script>");
                }
            }
        }
        #endregion

        /**************************其他****************************/

        #region 帮助信息

        // 用于显示“帮助信息”和“添加成功”，“更新成功”
        // GET: /News/Create
        [AdminAuthorize]
        public ActionResult HelpMessage(int id)
        {
            ViewBag.id = id;
            return View();
        }

        #endregion

        #region DropDownList的初始化

        #region 文章类型DropDownList（！！！注意这里没有图片新闻！！！）
        public void CreateNewsTypeDDL()
        {
            List<SelectListItem> itemsDepartment = new List<SelectListItem>();
            itemsDepartment.Add(new SelectListItem { Text = "请点击选择文章类型", Value = "" });
            SqlConnection conn = DBLink.GetConnection();
            string sqlstr = "select TypeID,TypeName from NewsType";
            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            try
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                //遍历绑定每一个类型记录
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        //不显示图片新闻
                        if (dr["TypeName"].ToString().Trim() != "图片新闻")
                        {
                            itemsDepartment.Add(new SelectListItem { Text = dr["TypeName"].ToString(), Value = dr["TypeID"].ToString() });
                        }
                    }
                }
                ViewBag.NewsTypeDDL = itemsDepartment;
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }
        #endregion

        #region 根据管理员类型区分的文章类型DropDownList（！！！注意这里没有图片新闻！！！）
        public void CreateNewsTypeByAdminTypeDDL()
        {
            List<SelectListItem> itemsDepartment = new List<SelectListItem>();
            itemsDepartment.Add(new SelectListItem { Text = "请点击选择文章类型", Value = "" });
            SqlConnection conn = DBLink.GetConnection();
            string sqlstr = "select TypeID,TypeName from NewsType where TypeID=7 or TypeID=8";  //7新闻动态，8通知公告
            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            try
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                //遍历绑定每一个类型记录
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        //不显示图片新闻
                        if (dr["TypeName"].ToString().Trim() != "图片新闻")
                        {
                            itemsDepartment.Add(new SelectListItem { Text = dr["TypeName"].ToString(), Value = dr["TypeID"].ToString() });
                        }
                    }
                }
                ViewBag.NewsTypeDDL = itemsDepartment;
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }
        #endregion

        #region 管理员类型DDL
        public void CreateAdminTypeDDL()
        {
            List<SelectListItem> itemsAdminType = new List<SelectListItem>();
            itemsAdminType.Add(new SelectListItem { Text = "一级管理员", Value = "1" });
            itemsAdminType.Add(new SelectListItem { Text = "二级管理员", Value = "2" });
            itemsAdminType.Add(new SelectListItem { Text = "三级管理员", Value = "3" });
            itemsAdminType.Add(new SelectListItem { Text = "四级管理员", Value = "4" });
            itemsAdminType.Add(new SelectListItem { Text = "五级管理员", Value = "5" });
            ViewBag.AdminTypeDDL = itemsAdminType;
        }
        #endregion

        #region 校区DropDownList
        public void CreatePlaceListFirstDDL()
        {
            List<SelectListItem> itemsPlaceListFirst = new List<SelectListItem>();
            itemsPlaceListFirst.Add(new SelectListItem { Text = "请点击选择校区", Value = "" });
            SqlConnection conn = DBLink.GetConnection();
            string sqlstr = "select PlaceFirstID,PlaceName from PlaceListFirst";
            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            try
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                //遍历绑定每一个类型记录
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        itemsPlaceListFirst.Add(new SelectListItem { Text = dr["PlaceName"].ToString(), Value = dr["PlaceFirstID"].ToString() });
                    }
                }
                ViewBag.PlaceListFirstDDL = itemsPlaceListFirst;
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }
        #endregion

        #endregion

    }
}
