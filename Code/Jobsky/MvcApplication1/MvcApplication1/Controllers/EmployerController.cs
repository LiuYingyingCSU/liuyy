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
    public class EmployerController : Controller
    {
        //文件名
        string fileName = "";

        //
        // GET: /Employer/
        [EmployerAuthorize]
        public ActionResult Index()
        {
            //检查Cookies["Employer"]是否存在
            if (Request.Cookies["Employer"] != null)
            {
                //验证用户名密码是否正确
                HttpCookie _cookie = Request.Cookies["Employer"];
                string _account = _cookie["EmployerAccount"];
                string _password = _cookie["EmployerPwd"];//cookie里原本存的就是加密后的密码
                string _isDelete = _cookie["IsDelete"];
                //Authentication是自己写的函数，验证账号密码是否正确
                if (Employer.Authentication(_account, Server.UrlDecode(_password)))
                {
                    if (_isDelete == "2")
                    {
                        ViewBag.AuthorityInfo = "<div class='alert alert-warning' style='margin-top:30px;'>等待审核,请您耐心等待，我们将在一个工作日之内审核完毕！（当前可查看注册信息和修改注册信息）</div>";
                    }
                    else if (_isDelete == "0")
                    {
                        ViewBag.AuthorityInfo = "<div class='alert alert-success' style='margin-top:30px;'>已通过审核（可以添加文章，修改文章了）</div>";
                    }
                }
            }
            return View();
        }

        //
        // GET: /Employer/Edit/5
        [EmployerAuthorize]
        public ActionResult Edit()
        {
            Employer employer = GetEmployerDetailsByAccount();
            EmployerRegister employerRegister = employer.GetEmployerRegister();
            CreateDropDownList();   //各个DropDownList生成
            //设置各个ddl个默认选项
            ViewBag.CompanyNature = new SelectList(ViewBag.CompanyNature, "Value", "Text", employer.CompanyNature);
            ViewBag.CompanyBusiness = new SelectList(ViewBag.CompanyBusiness, "Value", "Text", employer.CompanyBusiness);
            ViewBag.CompanySize = new SelectList(ViewBag.CompanySize, "Value", "Text", employer.CompanySize);
            ViewBag.CityClass = new SelectList(ViewBag.CityClass, "Value", "Text", employer.CityClass);
            ViewBag.IsTop500 = new SelectList(ViewBag.IsTop500, "Value", "Text", employer.IsTop500);
            //性别radio选择值
            ViewBag.ContactPersonSex = employerRegister.ContactPersonSex;
            //单位所在地select选择值
            ViewBag.CompanyAreaProvince = employerRegister.CompanyAreaProvince;
            ViewBag.CompanyAreaCity = employerRegister.CompanyAreaCity;
            //给密码和确认密码解密再显示
            employerRegister.EmployerPwd = Common.Text.DeCrypt(employerRegister.EmployerPwd);
            employerRegister.ConfirmEmployerPwd = employerRegister.EmployerPwd;
            if (employerRegister == null)
            {
                return HttpNotFound();
            }
            return View(employerRegister);
        }

        //
        // POST: /Employer/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EmployerAuthorize]
        public ActionResult Edit(FormCollection collection)
        {
            CreateDropDownList();   //各个DropDownList生成
            if (ModelState.IsValid)
            {
                //更新操作
                Employer employerOld = GetEmployerDetailsByAccount();    //根据账号得到修改之前的详细信息
                Employer employer = new Employer(); //准备存放修改后，新的Employer
                #region 得到Request过来的修改信息（22个）
                //账号信息
                employer.EmployerPwd = Request["EmployerPwd"];  //注意这里的密码是解密后的，原始密码是加密的
                //联系人信息
                employer.ContactPersonName = Request["ContactPersonName"];
                employer.ContactPersonSex = Int16.Parse(Request["ContactPersonSex"]);
                employer.FixedTelephone = Request["FixedTelephone"];
                employer.MobilePhone = Request["MobilePhone"];
                employer.Email = Request["Email"];
                //单位基本信息
                employer.CompanyName = Request["CompanyName"];
                employer.ParentCompanyName = Request["ParentCompanyName"];
                employer.CompanyIntroduction = Request["CompanyIntroduction"];
                employer.CompanyPhone = Request["CompanyPhone"];
                employer.OrganizationCode = Request["OrganizationCode"];

                employer.ValidPeriod = DateTime.Parse(Request["ValidPeriod"]);
                employer.CompanyNature = Request["CompanyNature"];
                employer.CompanyBusiness = Request["CompanyBusiness"];
                employer.CompanySize = Request["CompanySize"];
                employer.RegisteredCapital = decimal.Parse(Request["RegisteredCapital"]);

                employer.IsTop500 = Int32.Parse(Request["IsTop500"]);
                employer.CompanyAreaProvince = Request["CompanyAreaProvince"];
                employer.CompanyAreaCity = Request["CompanyAreaCity"];
                employer.CompanyAddress = Request["CompanyAddress"];
                employer.CityClass = Request["CityClass"];

                employer.Remark = Request["Remark"];

                #endregion

                //ID和注册时间是不能修改的信息
                employer.EmployerID = employerOld.EmployerID;
                employer.RegisterTime = employerOld.RegisterTime;
                //密码加密
                employer.EmployerPwd = Common.Text.EnCrypt(employer.EmployerPwd);
                //审核状态，证件照片路径。默认保持以前状态
                employer.IsDelete = employerOld.IsDelete;
                employer.CredentialsDir = employerOld.CredentialsDir;

                //组织机构代码，证件有效期，证件照片（下面上传文件里面考虑）  修改时，需要重新审核
                if ((employer.OrganizationCode != employerOld.OrganizationCode) || (employer.ValidPeriod != employerOld.ValidPeriod))
                {
                    employer.IsDelete = 2;  //改成待审核状态
                }
                //尝试上传证件照片
                if (FileUploads(EmployerAccount))
                {
                    //上传证件照片成功                   
                    HttpPostedFileBase uploadFile = Request.Files["file1"] as HttpPostedFileBase;
                    if (uploadFile != null && uploadFile.ContentLength > 0)
                    {
                        employer.CredentialsDir = fileName;
                        employer.IsDelete = 2;//状态改为待审核                        
                    }
                }
                //这里开始更新的存储过程
                if (Employer.UpdateEmployerInfo(employer))
                {
                    //更新成功
                    //如果更新成功并且成功上传了新的证件照片，则删除以前的
                    if (employer.CredentialsDir != employerOld.CredentialsDir)
                    {
                        //删除原来的证件照片
                        string updateFilePath = "~/Uploads/";
                        string FolderPath = System.Web.HttpContext.Current.Server.MapPath(updateFilePath);
                        if (System.IO.File.Exists(FolderPath + employerOld.CredentialsDir))
                        {
                            System.IO.File.Delete(FolderPath + employerOld.CredentialsDir);
                        }
                    }
                    Notice _n = new Notice { Title = "修改成功", Details = "您已经成功更新信息", DwellTime = 5, NavigationName = "首页", NavigationUrl = Url.Action("Index", "Employer") };
                    //跳转到修改成功页面之前，如果状态改为了待审核，cookie清理一下
                    if (employer.IsDelete == 2)
                    {
                        //检查Cookies["Employer"]是否存在
                        if (Request.Cookies["Employer"] != null)
                        {
                            //先删除cookie
                            HttpCookie _cookie = Request.Cookies["Employer"];
                            string _account = _cookie["EmployerAccount"];
                            string _password = _cookie["EmployerPwd"];//cookie里存的先自己加密，再url加密的密码   
                            string _isDelete = _cookie["IsDelete"];
                            _cookie.Expires = DateTime.Now.AddHours(-1);
                            Response.Cookies.Add(_cookie);
                            //加入新的cookie
                            HttpCookie _cookieNew = new HttpCookie("Employer");
                            _cookieNew.Values.Add("EmployerAccount", _account);
                            //密码先用自己的加密方法，再url加密（防止自己加密后有特殊字符），再存到cookie里
                            //取用的时候，先url解密，再用自己方法解密
                            _cookieNew.Values.Add("EmployerPwd", _password);
                            _cookieNew.Values.Add("IsDelete", employer.IsDelete.ToString());
                            Response.Cookies.Add(_cookieNew);
                        }
                    }
                    return RedirectToAction("Notice", "Prompt", _n);
                }
                else
                {
                    Error _e = new Error { Title = "修改失败", Details = "修改失败!请重新修改", Cause = Server.UrlEncode("<li>你修改时在修改页面停留的时间过久页已经超时</li><li>您绕开客户端验证向服务器提交数据</li>"), Solution = Server.UrlEncode("返回<a href='" + Url.Action("Edit", "Employer") + "'>修改</a>页面，刷新后重新修改") };
                    return RedirectToAction("Error", "Prompt", _e);
                }
            }
            return View();
        }

        //
        // GET: /Employer/Details
        //[AgreeEmployerAuthorize]
        [EmployerAuthorize]
        public ActionResult Details()
        {
            Employer employer = GetEmployerDetailsByAccount();
            if (employer == null)
            {
                return HttpNotFound();
            }
            return View(employer);
        }     

        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(EmployerLogin login)
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
            int ans = Employer.LoginAuthentication(login.EmployerAccount, Common.Text.EnCrypt(login.EmployerPwd));
            if (ans == 1||ans==2)
            {
                //登录成功则根据账号得到雇主信息
                Employer employer = GetEmployerDetailsByAccount(login.EmployerAccount);
                HttpCookie _cookie = new HttpCookie("Employer");
                _cookie.Values.Add("EmployerAccount", login.EmployerAccount);
                //密码先用自己的加密方法，再url加密（防止自己加密后有特殊字符），再存到cookie里
                //取用的时候，先url解密，再用自己方法解密
                _cookie.Values.Add("EmployerPwd", Server.UrlEncode(Common.Text.EnCrypt(login.EmployerPwd)));
                _cookie.Values.Add("IsDelete", (employer.IsDelete).ToString());
                Response.Cookies.Add(_cookie);
                if (Request.QueryString["ReturnUrl"] != null) return Redirect(Request.QueryString["ReturnUrl"]);
                else return RedirectToAction("Index", "Employer");                
            }
            else if (ans == 0)
            {
                ModelState.AddModelError("Message", "账号或密码错误，登陆失败！");
                return View();
            }
            return View();

        }

        //
        // GET: /Employer/WaitAudit

        public ActionResult WaitAudit()
        {
            //注册第二步—等待审核页面
            return View();
        }

        //
        // GET: /Employer/Register

        public ActionResult Register()
        {
            CreateDropDownList();
            EmployerRegister employerRegister = new EmployerRegister();
            return View(employerRegister);
        }

        //
        // POST: /Employer/Register

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(FormCollection collection)
        {
            CreateDropDownList();
            EmployerRegister employerRegister = new EmployerRegister();
            if (ModelState.IsValid)
            {
                #region 得到Request过来的修改信息（22个）
                //账号信息
                employerRegister.EmployerAccount = Request["EmployerAccount"];
                employerRegister.EmployerPwd = Request["EmployerPwd"];  //注意这里的密码是解密后的，原始密码是加密的
                //联系人信息
                employerRegister.ContactPersonName = Request["ContactPersonName"];
                employerRegister.ContactPersonSex = Int16.Parse(Request["ContactPersonSex"]);
                employerRegister.FixedTelephone = Request["FixedTelephone"];
                employerRegister.MobilePhone = Request["MobilePhone"];
                employerRegister.Email = Request["Email"];
                //单位基本信息
                employerRegister.CompanyName = Request["CompanyName"];
                employerRegister.ParentCompanyName = Request["ParentCompanyName"];
                employerRegister.CompanyIntroduction = Request["CompanyIntroduction"];
                employerRegister.CompanyPhone = Request["CompanyPhone"];
                employerRegister.OrganizationCode = Request["OrganizationCode"];

                employerRegister.ValidPeriod = DateTime.Parse(Request["ValidPeriod"]);
                employerRegister.CompanyNature = Request["CompanyNature"];
                employerRegister.CompanyBusiness = Request["CompanyBusiness"];
                employerRegister.CompanySize = Request["CompanySize"];
                employerRegister.RegisteredCapital = decimal.Parse(Request["RegisteredCapital"]);

                employerRegister.IsTop500 = Int32.Parse(Request["IsTop500"]);
                employerRegister.CompanyAreaProvince = Request["CompanyAreaProvince"];
                employerRegister.CompanyAreaCity = Request["CompanyAreaCity"];
                employerRegister.CompanyAddress = Request["CompanyAddress"];
                employerRegister.CityClass = Request["CityClass"];

                employerRegister.Remark = Request["Remark"];
                //其他字段
                //注册时间。因为默认用datetime2，所以不能直接DateTime.Now
                employerRegister.RegisterTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                employerRegister.IsDelete = 2;
                

                #endregion
                if (FileUploads())
                {
                    //密码加密
                    employerRegister.EmployerPwd = Common.Text.EnCrypt(employerRegister.EmployerPwd);
                    //上传文件成功
                    HttpPostedFileBase uploadFile = Request.Files["file1"] as HttpPostedFileBase;
                    if (uploadFile != null && uploadFile.ContentLength > 0)
                    {
                        //组织机构代码证字段
                        employerRegister.CredentialsDir = fileName;
                        //得到Employer                      
                        Employer employer = employerRegister.GetEmployer();
                        if (Employer.EmployerInsert(employer))
                        {
                            //Notice _n = new Notice { Title = "注册成功", Details = "您已经成功注册，用户为：" + Request["CompanyName"] + " ，请牢记您的密码！", DwellTime = 5, NavigationName = "列表", NavigationUrl = Url.Action("Index", "Employer") };
                            //return RedirectToAction("Notice", "Prompt", _n);
                            return RedirectToAction("WaitAudit", "Employer");
                        }
                        else
                        {
                            Error _e = new Error { Title = "注册失败", Details = "注册失败!请重新注册", Cause = Server.UrlEncode("<li>你注册时在注册页面停留的时间过久页已经超时</li><li>您绕开客户端验证向服务器提交数据</li>"), Solution = Server.UrlEncode("返回<a href='" + Url.Action("Register", "Employer") + "'>注册</a>页面，刷新后重新注册") };
                            return RedirectToAction("Error", "Prompt", _e);
                        }
                    }
                }
                else
                {
                    return View(employerRegister);
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
            if (Request.Cookies["Employer"] != null)
            {
                HttpCookie _cookie = Request.Cookies["Employer"];
                _cookie.Expires = DateTime.Now.AddHours(-1);
                Response.Cookies.Add(_cookie);
            }
            Notice _n = new Notice { Title = "成功退出", Details = "您已经成功退出！", DwellTime = 5, NavigationName = "单位登录页", NavigationUrl = Url.Action("Login", "Employer") };
            return RedirectToAction("Notice", "Prompt", _n);
        }

        /// <summary>
        /// 获取用户名
        /// </summary>
        public string EmployerAccount
        {
            get
            {
                HttpCookie _cookie = Request.Cookies["Employer"];
                if (_cookie == null) return "";
                else return _cookie["EmployerAccount"];
            }
        }
        #endregion

        #region 根据登录账号得到详细信息
        public Employer GetEmployerDetailsByAccount(string Account = "")
        {
            string account = "";
            if (Account == "")
            {
                account = EmployerAccount;
            }
            else
            {
                account = Account;
            }
            Employer employer = new Employer();
            SqlConnection conn = DBLink.GetConnection();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetEmployerDetailsByAccount";
            cmd.Parameters.Add(new SqlParameter("@EmployerAccount", account));
            try
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                //共27个字段，Emploeyr表所有内容
                //ID
                employer.EmployerID = Int32.Parse(dt.Rows[0]["EmployerID"].ToString());
                //账号信息部分
                employer.EmployerAccount = dt.Rows[0]["EmployerAccount"].ToString();
                employer.EmployerPwd = dt.Rows[0]["EmployerPwd"].ToString();
                //联系人信息部分
                employer.ContactPersonName = dt.Rows[0]["ContactPersonName"].ToString();
                employer.ContactPersonSex = Int16.Parse(dt.Rows[0]["ContactPersonSex"].ToString());
                employer.FixedTelephone = dt.Rows[0]["FixedTelephone"].ToString();
                employer.MobilePhone = dt.Rows[0]["MobilePhone"].ToString();
                employer.Email = dt.Rows[0]["Email"].ToString();
                //单位基本信息部分
                employer.CompanyName = dt.Rows[0]["CompanyName"].ToString();
                employer.ParentCompanyName = dt.Rows[0]["ParentCompanyName"].ToString();
                employer.CompanyIntroduction = dt.Rows[0]["CompanyIntroduction"].ToString();
                employer.CompanyPhone = dt.Rows[0]["CompanyPhone"].ToString();
                employer.OrganizationCode = dt.Rows[0]["OrganizationCode"].ToString();

                employer.ValidPeriod = DateTime.Parse(dt.Rows[0]["ValidPeriod"].ToString());
                employer.CompanyNature = dt.Rows[0]["CompanyNature"].ToString();
                employer.CompanyBusiness = dt.Rows[0]["CompanyBusiness"].ToString();
                employer.CompanySize = dt.Rows[0]["CompanySize"].ToString();
                employer.RegisteredCapital = decimal.Parse(dt.Rows[0]["RegisteredCapital"].ToString());

                employer.IsTop500 = Int32.Parse(dt.Rows[0]["IsTop500"].ToString());
                employer.CompanyAreaProvince = dt.Rows[0]["CompanyAreaProvince"].ToString();
                employer.CompanyAreaCity = dt.Rows[0]["CompanyAreaCity"].ToString();
                employer.CompanyAddress = dt.Rows[0]["CompanyAddress"].ToString();
                employer.CityClass = dt.Rows[0]["CityClass"].ToString();

                employer.Remark = dt.Rows[0]["Remark"].ToString();
                //证件照片
                employer.CredentialsDir = dt.Rows[0]["CredentialsDir"].ToString();
                //其他字段
                employer.RegisterTime = DateTime.Parse(dt.Rows[0]["RegisterTime"].ToString());
                employer.IsDelete = Int32.Parse(dt.Rows[0]["IsDelete"].ToString());

                return employer;
            }
            catch (Exception)
            {
                return new Employer();
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
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
            Bitmap _bitmap = new Bitmap(Server.MapPath("~/Skins/Common/Texture2.jpg"), true);
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

        #region 上传文件
        public bool FileUploads(string account = "")
        {
            string pathForSaving = Server.MapPath("~/Uploads");
            if (this.CreateFolderIfNeeded(pathForSaving))
            {
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase uploadFile = Request.Files[file] as HttpPostedFileBase;
                    if (uploadFile != null && uploadFile.ContentLength > 0)
                    {
                        //取得文件的扩展名,并转换成小写
                        string fileExtension = System.IO.Path.GetExtension(uploadFile.FileName).ToLower();
                        //限定只能上传jpg,jpeg,png和gif图片
                        string[] allowExtension = { ".jpg", ".png", ".gif", ".jpeg" };
                        //判断文件类型是否符合
                        bool fileOK = false;
                        //对上传的文件的类型进行判断
                        for (int i = 0; i < allowExtension.Length; i++)
                        {
                            if (fileExtension == allowExtension[i])
                            {
                                fileOK = true;
                                break;
                            }
                        }
                        if (fileOK)
                        {
                            //获得上传文件的大小
                            int fileSize = uploadFile.ContentLength;
                            if (fileSize < 1024 * 1024 * 10)
                            {
                                //将时间转换成字符串作为文件名称
                                fileName = account + DateTime.Now.ToString().Replace(":", "").Replace("/", "").Replace(" ", "").Replace("-", "");
                                fileName += fileExtension;
                                var path = Path.Combine(pathForSaving, fileName);
                                uploadFile.SaveAs(path);
                                ViewBag.Message = "";
                                return true;
                            }
                            else
                            {
                                ViewBag.Message = "营业执照或组织机构代码证大小不能超过10M!";
                                return false;
                            }
                        }
                        else
                        {
                            ViewBag.Message = "营业执照或组织机构代码证必须为图片格式!";
                            return false;
                        }
                    }
                    else
                    {
                        ViewBag.Message = "请上传一张营业执照或组织机构代码证!";
                        return false;
                    }
                }
                return false;
            }
            else
            {
                ViewBag.Message = "营业执照或组织机构代码证存放路径创建失败!";
                return false;
            }
        }

        // 检查是否要创建上传文件夹
        private bool CreateFolderIfNeeded(string path)
        {
            bool result = true;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    //TODO：处理异常
                    result = false;
                }
            }
            return result;
        }
        #endregion

        #region 各个DropDownList的初始化
        public void CreateDropDownList()
        {
            #region 单位性质DropDownList
            List<SelectListItem> itemsCompanyNature = new List<SelectListItem>();
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
            //ViewData["IsTop500"] = (List<SelectListItem>)itemsIsTop500;
            #endregion

            #region 所属行业DropDownList
            List<SelectListItem> itemsCompanyBusiness = new List<SelectListItem>();
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

            #region 单位规模DropDownList
            List<SelectListItem> itemsCompanySize = new List<SelectListItem>();
            itemsCompanySize.Add(new SelectListItem { Text = "1-49人", Value = "1-49人" });
            itemsCompanySize.Add(new SelectListItem { Text = "50-99人", Value = "50-99人" });
            itemsCompanySize.Add(new SelectListItem { Text = "100-499人", Value = "100-499人" });
            itemsCompanySize.Add(new SelectListItem { Text = "500-999人", Value = "500-999人" });
            itemsCompanySize.Add(new SelectListItem { Text = "1000人以上", Value = "1000人以上" });
            ViewBag.CompanySize = itemsCompanySize;
            #endregion

            #region 所在城市类别DropDownList
            List<SelectListItem> itemsCityClass = new List<SelectListItem>();
            itemsCityClass.Add(new SelectListItem { Text = "直辖市", Value = "直辖市" });
            itemsCityClass.Add(new SelectListItem { Text = "省会城市", Value = "省会城市" });
            itemsCityClass.Add(new SelectListItem { Text = "地级城市", Value = "地级城市" });
            itemsCityClass.Add(new SelectListItem { Text = "县乡城镇", Value = "县乡城镇" });
            itemsCityClass.Add(new SelectListItem { Text = "其它地区", Value = "其它地区" });
            ViewBag.CityClass = itemsCityClass;
            //ViewData["IsTop500"] = (List<SelectListItem>)itemsIsTop500;
            #endregion

            #region 五百强DropDownList
            List<SelectListItem> itemsIsTop500 = new List<SelectListItem>();
            itemsIsTop500.Add(new SelectListItem { Text = "非五百强", Value = "0" });
            itemsIsTop500.Add(new SelectListItem { Text = "五百强", Value = "1" });
            ViewBag.IsTop500 = itemsIsTop500;
            //ViewData["IsTop500"] = (List<SelectListItem>)itemsIsTop500;
            #endregion
        }
        #endregion

        #region 远端验证账号是否已经存在
        public JsonResult CheckLoginAccount(string employeraccount)
        {
            var result = false;
            if (employeraccount.Trim() == "jobsky")
                result = false;
            else
            {
                int count = 0;
                SqlConnection cnn = DBLink.GetConnection();
                //string sqlstr = "Select Count(*) FROM Employer where EmployerAccount='" + employeraccount + "' and IsDelete!=1";
                string sqlstr = "Select Count(*) FROM Employer where EmployerAccount='" + employeraccount + "'";
                SqlCommand cmm = new SqlCommand(sqlstr, cnn);
                cnn.Open();
                count = int.Parse(cmm.ExecuteScalar().ToString());
                cmm.Dispose();
                cnn.Close();
                result = (count == 0);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 远端验证邮箱是否已经存在
        public JsonResult CheckEmailUnique(string email)
        {
            var result = false;
            int count = 0;
            SqlConnection cnn = DBLink.GetConnection();
            //string sqlstr = "Select Count(*) FROM Employer where Email='" + email + "' and IsDelete!=1";
            string sqlstr = "Select Count(*) FROM Employer where Email='" + email + "'";
            //检查Cookies["Employer"]是否存在（如果用户已经登陆可能是修改信息）
            if (Request.Cookies["Employer"] != null)
            {
                //用户已经登陆
                //验证用户名密码是否正确
                HttpCookie _cookie = Request.Cookies["Employer"];
                string _account = _cookie["EmployerAccount"];
                string _password = _cookie["EmployerPwd"];//cookie里存的先自己加密，再url加密的密码   
                string _isDelete = _cookie["IsDelete"];
                //Authentication是自己写的函数，验证账号密码是否正确
                if (Employer.Authentication(_account, Server.UrlDecode(_password)))
                {
                    string url = HttpContext.Request.UrlReferrer.ToString();
                    int pos = url.LastIndexOf("/") + 1;
                    string actionName = url.Substring(pos, url.Length - pos);
                    //防止登陆用户再注册一个相同邮箱账号
                    if (actionName.ToLower() == "edit")
                    {
                        Employer employer = GetEmployerDetailsByAccount(_account);
                        //防止修改信息时误判断邮箱已经存在
                        sqlstr += " and Email!='" + employer.Email + "'";
                    } 
                }
            }
            SqlCommand cmm = new SqlCommand(sqlstr, cnn);
            cnn.Open();
            count = int.Parse(cmm.ExecuteScalar().ToString());
            cmm.Dispose();
            cnn.Close();
            result = (count == 0);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion  
        
        #region Cookie的获取以及验证方法
        /// <summary>
        /// Cookie验证
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public bool AuthenticationByCookie(HttpContextBase httpContext)
        {
            //检查Cookies["Employer"]是否存在
            if (httpContext.Request.Cookies["Employer"] == null) return false;
            //验证用户名密码是否正确
            HttpCookie _cookie = httpContext.Request.Cookies["Employer"];
            string _account = _cookie["EmployerAccount"];
            string _password = _cookie["EmployerPwd"];//cookie里存的就是解密后的密码
            if (_account == "" || _password == "") return false;
            //Authentication是自己写的函数，验证账号密码是否正确
            if (Employer.Authentication(_account, Common.Text.EnCrypt(_password))) return true;
            else return false;
        }
        #endregion
    }
}
