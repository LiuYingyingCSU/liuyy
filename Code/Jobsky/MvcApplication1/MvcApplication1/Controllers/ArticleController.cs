using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication1.Models;
using System.IO;
using System.Data;

namespace MvcApplication1.Controllers
{
    public class ArticleController : Controller
    {
        //已审核文章列表
        // GET: /Article/
        [AgreeEmployerAuthorize]
        public ActionResult Index(int id = 1)
        {
            int pageindex = id;//当前是第几页，第一页为1
            int controlLength = 30; //文章标题显示字数限制，30个字符=15个汉字
            int pagesize = 10;       //每页显示多少记录
            HttpCookie cookie = Request.Cookies["Employer"];
            if (cookie != null)
            {
                string EmployerAccount = cookie.Values["EmployerAccount"];
                int totalnum = Article.GetArticleRecordCountForEmployer(EmployerAccount, 0);//获取总共多少条记录
                ViewBag.totalpage = (totalnum%pagesize==0 && totalnum>pagesize) ? (totalnum/pagesize): ((totalnum/pagesize)+1);//获取总页数
                ViewBag.pageindex = pageindex;//当前页
                ViewBag.Articles = Article.GetArticleListForEmployer(EmployerAccount, pageindex, pagesize, 0);//列表数据
            }

            return View();
        }

        //
        // GET: /Article/Create
        [AgreeEmployerAuthorize]
        public ActionResult Create()
        {
            List<ArticleType> typelist = ArticleType.Select();
            DataTable bigarticleTable2 = Article.SelectArticleFromArticle(2,DateTime.Now);//双选会
            DataTable bigarticleTable3 = Article.SelectArticleFromArticle(3, DateTime.Now);//组团招聘

            ViewBag.typelist = typelist;//传给View
            ViewBag.PlaceListFirst = Article.GetPlaceListFirst();
            ViewBag.bigarticleTable2 = bigarticleTable2;
            ViewBag.bigarticleTable3 = bigarticleTable3;
            return View();
        }

        //
        // POST: /Article/Create

        [HttpPost]
        [ValidateInput(false)]          //这儿相当于给字段加[AllowHtml]，允许传入html
        [AgreeEmployerAuthorize]
        public ActionResult Create(FormCollection collection)
        {
            //判断字段是否符合标准
            //第一部分：文章
            TempArticle temparticle = new TempArticle();
            
            temparticle.Title = Request["Title"].Trim();
            temparticle.Introduction = Request["Introduction"].Trim();
            temparticle.EditTime = DateTime.Now;

            HttpCookie cookie = Request.Cookies["Employer"];
            if (cookie != null)
            {
                string EmployerAccount = cookie.Values["EmployerAccount"];
                temparticle.EditorAccount = EmployerAccount;//获取单位的Account
            }
            
            temparticle.TypeID = Convert.ToInt32(Request["TypeID"].Trim());//得到类型
            temparticle.ContactInfo = Request["ContactInfo"].Trim();
            temparticle.FileAddr = "";//下面上传文件时会赋值 
            temparticle.IsAudit = 1;//待定
            temparticle.ArticleDescription = Request["ArticleDescription"].Trim();

            int bigarticle = -1;//初始值
            //判断文章类型
            if (temparticle.TypeID == 1) {
                temparticle.PlaceFirstID = Convert.ToInt32(Request["PlaceFirstID"]);
                temparticle.RecruitTime = Convert.ToDateTime(Request["RecruitTime"]);
            }
            else if (temparticle.TypeID == 2)
            {
                if (Request["bigarticle2"] == null || Request["bigarticle2"] == "")
                {
                    return Json(new { message = "必须选择一个要加入的双选会" });//退出 
                }
                bigarticle = Convert.ToInt32(Request["bigarticle2"]);
            }
            else if (temparticle.TypeID == 3)
            {
                if (Request["bigarticle3"] == null || Request["bigarticle3"] == "")
                {
                    return Json(new { message = "必须选择一个要加入的组团招聘" });//退出 
                }
                bigarticle = Convert.ToInt32(Request["bigarticle3"]);
            }

            temparticle.BigArticleID = bigarticle;//默认-1是未选择大型招聘会文章的
            


            //第二部分：需求
            string[] PositionName = Request.Form.GetValues("PositionName");
            string[] EducationalLevel = Request.Form.GetValues("EducationalLevel");
            string[] Major = Request.Form.GetValues("Major");
            string[] DemandNum = Request.Form.GetValues("DemandNum");
            string[] PositionDec = Request.Form.GetValues("PositionDec");
            //判断需求是否符合标准

            List<TempDemandInfo> tempdemandInfo = new List<TempDemandInfo>();
            //判断有几个需求
            for (int i = 0; i < PositionName.Count(); ++i)
            {
                TempDemandInfo temp = new TempDemandInfo();
                //temp.TempArticleID = 1;//得到刚上传的文章ID,在下面的插入后会有赋值
                temp.PositionName = PositionName[i].ToString().Trim();
                temp.EducationalLevel = EducationalLevel[i].ToString().Trim();
                temp.Major = Major[i].ToString().Trim();
                temp.DemandNum = Convert.ToInt32(DemandNum[i].ToString().Trim());
                temp.PositionDec = PositionDec[i].ToString().Trim();

                tempdemandInfo.Add(temp);
            }

            //处理文件上传
            if (Request.Files.Count > 0)
            {
                //支持的类型
                List<string> FileType = new List<string>();
                FileType.Add("jpg");
                FileType.Add("gif");
                FileType.Add("png");
                FileType.Add("bmp");
                FileType.Add("jpeg");

                FileType.Add("pdf");
                FileType.Add("txt");
                FileType.Add("xls");
                FileType.Add("xlsx");
                FileType.Add("doc");
                FileType.Add("docx");

                FileType.Add("rar");
                FileType.Add("zip");

                //获取文件
                HttpPostedFileBase uploadFile = Request.Files[0];
                //判断是否为空文件
                if (uploadFile == null )//|| uploadFile.ContentLength == 0)
                {
                    return Json(new { message = "附件为空" });
                }

                //截取文件后缀名,判断文件类型是否被支持
                if (FileType.Contains(uploadFile.FileName.Substring(uploadFile.FileName.LastIndexOf(".") + 1)))
                {
                    //用jobsky作为分段符号
                    var fileTime = DateTime.Now.ToFileTime().ToString() + Path.GetFileName(uploadFile.FileName);//新命名的文件名（包含时间的整数形式）
                    var tofileName = Path.Combine(Request.MapPath("~/UploadFiles"), fileTime);//把两个名字连接起来
                    try
                    {
                        if (Directory.Exists(Server.MapPath("~/UploadFiles")) == false)//如果不存在就创建file文件夹
                        {
                            Directory.CreateDirectory(Server.MapPath("~/UploadFiles"));
                        }
                        uploadFile.SaveAs(tofileName);
                        temparticle.FileAddr = fileTime;//获得上传后的文件名
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

            //插入文章到数据库
            int ID = TempArticle.InsertTempArticle(temparticle);
            if (ID > 0)
            {
                //获取刚插入的文章ID

                //插入需求到数据库
                foreach (var t in tempdemandInfo)
                {
                    t.TempArticleID = ID;//得到刚insert的文章ID

                    if (TempDemandInfo.Insert(t))
                    {                       
                        //插入成功
                    }
                    else
                    {                     
                        return Json(new { message = "inserterror1" });
                    }
                }
                ////插入大型招聘会到数据库
                //if ((temparticle.TypeID == 2 || temparticle.TypeID == 3) && bigarticle != -1)
                //{
                //    if (Article.InsertBigArticle(ID, bigarticle))//插入成功
                //    {

                //    }
                //    else return Json(new { message = "inserterror2" });
                //}

            }
            else
            {
                return Json(new { message = "inserterror3" });
            }

            return Json(new { message = "success" , ArticleID=ID});
        }

        public ActionResult AddSuccess(int id = 1, int ArticleID = -10)
        {
            if (id == 1)
            {
                //ViewBag.info = "提交成功！";
                Notice _n = new Notice { Title = "文章提交成功", Details = "您已成功提交文章", DwellTime = 10, NavigationName = "查看文章", NavigationUrl = Url.Action("TempArticleDetails/"+ArticleID, "Article") };
                return RedirectToAction("Notice", "Prompt", _n);            
            }
            else if (id == 2)
            {
                ViewBag.info = "修改成功！";
            }
            else
            {
                ViewBag.info = "欢迎登录“中南大学就业信息网-单位后台”";
            }
            return View();
        }

        #region 文章编辑
        //
        // GET: /ArticleEdit/Edit/5
        [AgreeEmployerAuthorize]
        public ActionResult ArticleEdit(int id)//此处id表示ArticleID
        {
            HttpCookie cookie = Request.Cookies["Employer"];
            if (cookie != null)
            {
                string EmployerAccount = cookie.Values["EmployerAccount"];
                DataTable temparticle = TempArticle.GetTempArticleByArticleID(id, EmployerAccount);
                if (temparticle.Rows.Count > 0)//该文章已经被编辑过，存在于TempArticle里面
                {
                    //重定向TempArticleEdit
                    return RedirectToAction("TempArticleEdit", new { id = temparticle.Rows[0]["TempArticleID"] });
                }
                else
                {
                    ViewBag.flag = true;//文章是否存在的标志
                    DataTable article = Article.GetArticleByArticleIDAndEditorAccount(id, EmployerAccount);//得到Article表文章
                    if (article != null && article.Rows.Count > 0)
                    {
                        ViewBag.Article = article;

                        //显示大型招聘会选项
                        int typeid = Convert.ToInt32(article.Rows[0]["TypeID"]);
                        ViewBag.typename = ArticleType.GetTypeNameByTypeID(typeid);
                        if (typeid == 1)//专场招聘
                        { 
                            //获取时间和地点
                            ViewBag.PlaceList = Article.GetPlaceListSecondById(Convert.ToInt32(article.Rows[0]["PlaceSecondID"]));
                            ViewBag.PlaceListFirst = Article.GetPlaceListFirst();
                        }
                        if (typeid == 2)//双选会
                        {
                            DataTable bigarticleTable2 = Article.SelectArticleFromArticle(2, DateTime.Now);
                            ViewBag.bigarticleTable2 = bigarticleTable2;
                            ViewBag.BigArticleID = Article.GetBigArticleByArticleID(Convert.ToInt32(article.Rows[0]["ArticleID"]));
                        }
                        else if (typeid == 3)//组团招聘
                        {
                            DataTable bigarticleTable3 = Article.SelectArticleFromArticle(3, DateTime.Now);
                            ViewBag.bigarticleTable3 = bigarticleTable3;
                            ViewBag.BigArticleID = Article.GetBigArticleByArticleID(Convert.ToInt32(article.Rows[0]["ArticleID"]));
                        }
                        //其他类型的文章不用管

                        //需求部分
                        ViewBag.DemandInfos = DemandInfo.GetDemandInfoByArticleID(id);
                    }
                    else { ViewBag.flag = false; }//文章不存在
                }
            }
            else { ViewBag.flag = false; }//cookie不存在

            return View();
        }
        #endregion

        #region TempArticle文章再编辑
        //  只考虑insert就行了，不用update，因为上面get函数已经重定向过了
        // POST: /Article/ArticleEdit/5
        //TempArticle里面的所有文章再编辑
        [HttpPost]
        [ValidateInput(false)]          //这儿相当于给字段加[AllowHtml]，允许传入html
        [AgreeEmployerAuthorize]
        public ActionResult ArticleEdit(int id, FormCollection collection)//id为ArticleID
        {
            //判断这篇文章是否是已经提交过一次编辑，这个已经在get函数重定向了，这儿不用考虑了

            //判断字段是否符合标准
            //第一部分：文章
            TempArticle temparticle = new TempArticle();
            //判断文章类型
            temparticle.ArticleID = id;
            temparticle.TypeID = Article.GetTypeIDByArticleID(id);//注意，这儿是调用Article的函数，不是TempArticle
            temparticle.Title = Request["Title"].Trim();
            temparticle.Introduction = Request["Introduction"].Trim();
            temparticle.EditTime = DateTime.Now;//提交时间不再改变

            HttpCookie cookie = Request.Cookies["Employer"];
            if (cookie != null)
            {
                string EmployerAccount = cookie.Values["EmployerAccount"];
                temparticle.EditorAccount = EmployerAccount;//获取单位的Account
            }
            temparticle.ContactInfo = Request["ContactInfo"].Trim();
            temparticle.FileAddr = "";//下面上传文件时会赋值 
            temparticle.IsAudit = 1;//待定
            temparticle.ArticleDescription = Request["ArticleDescription"].Trim();

            int bigarticle = -1;//初始值
            //判断文章类型
            if (temparticle.TypeID == 1)
            {
                temparticle.PlaceFirstID = Convert.ToInt32(Request["PlaceFirstID"]);
                temparticle.RecruitTime = Convert.ToDateTime(Request["RecruitTime"]);
            }
            else if (temparticle.TypeID == 2)
            {
                if (Request["bigarticle2"] == null || Request["bigarticle2"] == "")
                {
                    return Json(new { message = "必须选择一个要加入的双选会" });//退出 
                }
                bigarticle = Convert.ToInt32(Request["bigarticle2"]);
            }
            else if (temparticle.TypeID == 3)
            {
                if (Request["bigarticle3"] == null || Request["bigarticle3"] == "")
                {
                    return Json(new { message = "必须选择一个要加入的组团招聘" });//退出 
                }
                bigarticle = Convert.ToInt32(Request["bigarticle3"]);
            }
            temparticle.BigArticleID = bigarticle;//默认-1是未选择大型招聘会文章的

            //第二部分：需求
            string[] PositionName = Request.Form.GetValues("PositionName");
            string[] EducationalLevel = Request.Form.GetValues("EducationalLevel");
            string[] Major = Request.Form.GetValues("Major");
            string[] DemandNum = Request.Form.GetValues("DemandNum");
            string[] PositionDec = Request.Form.GetValues("PositionDec");

            List<TempDemandInfo> tempdemandInfo = new List<TempDemandInfo>();
            //判断有几个需求
            for (int i = 0; i < PositionName.Count(); ++i)
            {
                TempDemandInfo temp = new TempDemandInfo();
                //temp.TempArticleID = 1;//得到刚上传的文章ID,在下面的插入后会有赋值
                temp.PositionName = PositionName[i].ToString().Trim();
                temp.EducationalLevel = EducationalLevel[i].ToString().Trim();
                temp.Major = Major[i].ToString().Trim();
                temp.DemandNum = Convert.ToInt32(DemandNum[i].ToString().Trim());
                temp.PositionDec = PositionDec[i].ToString().Trim();

                tempdemandInfo.Add(temp);
            }
            if (Request["IsDeleteFile"].ToString().Trim() == "yes")//yes既表示删除原文件（重新上传），也表示第一次上传
            {
                //处理文件上传
                if (Request.Files.Count > 0)
                {
                    //支持的类型
                    List<string> FileType = new List<string>();
                    FileType.Add("jpg");
                    FileType.Add("gif");
                    FileType.Add("png");
                    FileType.Add("bmp");
                    FileType.Add("jpeg");

                    FileType.Add("pdf");
                    FileType.Add("txt");
                    FileType.Add("xls");
                    FileType.Add("xlsx");
                    FileType.Add("doc");
                    FileType.Add("docx");

                    FileType.Add("rar");
                    FileType.Add("zip");

                    //获取文件
                    HttpPostedFileBase uploadFile = Request.Files[0];
                    //判断是否为空文件
                    if (uploadFile == null)//|| uploadFile.ContentLength == 0)
                    {
                        return Json(new { message = "fileerror" });
                    }

                    //截取文件后缀名,判断文件类型是否被支持
                    if (FileType.Contains(uploadFile.FileName.Substring(uploadFile.FileName.LastIndexOf(".") + 1)))
                    {
                        //用jobsky作为分段符号
                        var fileTime = DateTime.Now.ToFileTime().ToString() + Path.GetFileName(uploadFile.FileName);//新命名的文件名（包含时间的整数形式）
                        var tofileName = Path.Combine(Request.MapPath("~/UploadFiles"), fileTime);//把两个名字连接起来
                        try
                        {
                            if (Directory.Exists(Server.MapPath("~/UploadFiles")) == false)//如果不存在就创建file文件夹
                            {
                                Directory.CreateDirectory(Server.MapPath("~/UploadFiles"));
                            }
                            uploadFile.SaveAs(tofileName);
                            temparticle.FileAddr = fileTime;//获得上传后的文件名
                        }
                        catch
                        {
                            return Json(new { message = "fileerror" });
                        }
                    }
                    else
                    {
                        return Json(new { message = "fileerror" });//不支持此格式文件上传
                    }
                }
            }
            else
            {
                temparticle.FileAddr = Request["IsDeleteFile"].ToString().Trim();//没有删除，则获得到的就是原来的文件名
            }

            //添加文章到数据库
            int newTempArticleID = TempArticle.InsertTempArticleFromArticle(temparticle);
            if (newTempArticleID > 0)
            {
                //插入需求到数据库
                foreach (var t in tempdemandInfo)
                {
                    t.TempArticleID = newTempArticleID;
                    if (TempDemandInfo.Insert(t))
                    {
                        //插入成功
                    }
                    else
                    {
                        return Json(new { message = "inserterror1" });
                    }
                }
            }

            return Json(new { message = "success" });
        }
        #endregion
        //
        // POST: /Article/Delete/5
        [HttpPost]
        [AgreeEmployerAuthorize]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                bool flag = TempDemandInfo.Delete(id) && TempArticle.Delete(id);
                return Json(flag);//true表示删除成功
                //return RedirectToAction("Index"); 
            }
            catch
            {
                return Json(false);
                //return View();
            }
        }

        //
        // GET: /Article/Details/5
        [AgreeEmployerAuthorize]
        public ActionResult TempArticleDetails(int id)
        {
            HttpCookie cookie = Request.Cookies["Employer"];
            string EmployerAccount = cookie.Values["EmployerAccount"];
            ViewBag.tb_Article = TempArticle.GetTempArticleByTempArticleID(id, EmployerAccount);
            ViewBag.tb_Demand = TempDemandInfo.GetTempDemandInfoByTempArticleID(id);
            return View();
        }

        //待审核文章列表
        // GET: /TempArticleList/
        [AgreeEmployerAuthorize]
        public ActionResult TempArticleList(int id = 1)
        {
            int pageindex = id;//当前是第几页，第一页为1
            int controlLength = 30; //文章标题显示字数限制，30个字符=15个汉字
            int pagesize = 10;       //每页显示多少记录
            HttpCookie cookie = Request.Cookies["Employer"];
            if (cookie != null)
            {
                string EmployerAccount = cookie.Values["EmployerAccount"];
                int totalnum = TempArticle.GetTempArticleRecordCount(EmployerAccount, 0);//获取总共多少条记录
                ViewBag.totalpage = (totalnum % pagesize == 0 && totalnum > pagesize) ? (totalnum / pagesize) : ((totalnum / pagesize) + 1);//获取总页数
                ViewBag.pageindex = pageindex;//当前页
                ViewBag.Articles = TempArticle.GetArticleListFromTempArticle(EmployerAccount, pageindex, 15, 0);
            }

            return View();
        }

        //
        // GET: /Article/TempArticleEdit/5
        [AgreeEmployerAuthorize]
        public ActionResult TempArticleEdit(int id)//此处id表示TempArticleID
        {
            ViewBag.flag = true;//文章是否存在的标志

            HttpCookie cookie = Request.Cookies["Employer"];
            if (cookie != null)
            {
                string EmployerAccount = cookie.Values["EmployerAccount"];
                DataTable temparticle = TempArticle.GetTempArticleByTempArticleID(id, EmployerAccount);//得到临时表文章
                if (temparticle != null && temparticle.Rows.Count > 0)
                {
                    ViewBag.Article = temparticle;

                    //显示大型招聘会选项
                    int typeid = Convert.ToInt32(temparticle.Rows[0]["TypeID"]);
                    ViewBag.typename = ArticleType.GetTypeNameByTypeID(typeid);

                    if (typeid == 1)//专场招聘
                    {
                        ViewBag.PlaceListFirst = Article.GetPlaceListFirst();
                    }
                    else if (typeid == 2)//双选会
                    {
                        DataTable bigarticleTable2 = Article.SelectArticleFromArticle(2, DateTime.Now);
                        ViewBag.bigarticleTable2 = bigarticleTable2;
                    }
                    else if (typeid == 3)//组团招聘
                    { 
                        DataTable bigarticleTable3 = Article.SelectArticleFromArticle(3, DateTime.Now);
                        ViewBag.bigarticleTable3 = bigarticleTable3;
                    }
                    //其他类型的文章不用管

                    //需求部分
                    ViewBag.DemandInfos = TempDemandInfo.GetTempDemandInfoByTempArticleID(id);
                }
                else { ViewBag.flag = false; }//文章不存在
            }
            else { ViewBag.flag = false;}//cookie不存在
            
            return View();
        }

        //
        // POST: /Article/TempArticleEdit/5
        //TempArticle里面的所有文章再编辑
        [HttpPost]
        [ValidateInput(false)]          //这儿相当于给字段加[AllowHtml]，允许传入html
        [AgreeEmployerAuthorize]
        public ActionResult TempArticleEdit(int id, FormCollection collection)//id为TempArticleID
        {
            //判断字段是否符合标准
            //第一部分：文章
            TempArticle temparticle = new TempArticle();
            //判断文章类型
            temparticle.TempArticleID = id;
            temparticle.TypeID = TempArticle.GetTypeIDByTempArticleID(id);
            temparticle.Title = Request["Title"].Trim();
            temparticle.Introduction = Request["Introduction"].Trim();
            //temparticle.EditTime = DateTime.Now;//提交时间不再改变

            HttpCookie cookie = Request.Cookies["Employer"];
            if (cookie != null)
            {
                string EmployerAccount = cookie.Values["EmployerAccount"];
                temparticle.EditorAccount = EmployerAccount;//获取单位的Account
            }

            //temparticle.TypeID = Convert.ToInt32(Request["TypeID"].Trim());//得到类型
            temparticle.ContactInfo = Request["ContactInfo"].Trim();
            temparticle.FileAddr = "";//下面上传文件时会赋值 
            temparticle.IsAudit = 1;//待定
            temparticle.ArticleDescription = Request["ArticleDescription"].Trim();

            int bigarticle = -1;//初始值
            //判断文章类型
            if (temparticle.TypeID == 1)
            {
                temparticle.PlaceFirstID = Convert.ToInt32(Request["PlaceFirstID"]);
                temparticle.RecruitTime = Convert.ToDateTime(Request["RecruitTime"]);
            }
            else if (temparticle.TypeID == 2)
            {
                if (Request["bigarticle2"] == null || Request["bigarticle2"] == "")
                {
                    return Json(new { message = "必须选择一个要加入的双选会" });//退出 
                }
                bigarticle = Convert.ToInt32(Request["bigarticle2"]);
            }
            else if (temparticle.TypeID == 3)
            {
                if (Request["bigarticle3"] == null || Request["bigarticle3"] == "")
                {
                    return Json(new { message = "必须选择一个要加入的组团招聘" });//退出 
                }
                bigarticle = Convert.ToInt32(Request["bigarticle3"]);
            }
            temparticle.BigArticleID = bigarticle;//默认-1是未选择大型招聘会文章的

            //第二部分：需求

            string[] PositionName = Request.Form.GetValues("PositionName");
            string[] EducationalLevel = Request.Form.GetValues("EducationalLevel");
            string[] Major = Request.Form.GetValues("Major");
            string[] DemandNum = Request.Form.GetValues("DemandNum");
            string[] PositionDec = Request.Form.GetValues("PositionDec");
            //判断需求是否符合标准

            List<TempDemandInfo> tempdemandInfo = new List<TempDemandInfo>();
            //判断有几个需求
            for (int i = 0; i < PositionName.Count(); ++i)
            {
                TempDemandInfo temp = new TempDemandInfo();
                //temp.TempArticleID = 1;//得到刚上传的文章ID,在下面的插入后会有赋值
                temp.PositionName = PositionName[i].ToString().Trim();
                temp.EducationalLevel = EducationalLevel[i].ToString().Trim();
                temp.Major = Major[i].ToString().Trim();
                temp.DemandNum = Convert.ToInt32(DemandNum[i].ToString().Trim());
                temp.PositionDec = PositionDec[i].ToString().Trim();

                tempdemandInfo.Add(temp);
            }
            
            if (Request["IsDeleteFile"].ToString().Trim() == "yes")
            {
                //处理文件上传
                if (Request.Files.Count > 0)
                {
                    //支持的类型
                    List<string> FileType = new List<string>();
                    FileType.Add("jpg");
                    FileType.Add("gif");
                    FileType.Add("png");
                    FileType.Add("bmp");
                    FileType.Add("jpeg");

                    FileType.Add("pdf");
                    FileType.Add("txt");
                    FileType.Add("xls");
                    FileType.Add("xlsx");
                    FileType.Add("doc");
                    FileType.Add("docx");

                    FileType.Add("rar");
                    FileType.Add("zip");

                    //获取文件
                    HttpPostedFileBase uploadFile = Request.Files[0];
                    //判断是否为空文件
                    if (uploadFile == null)//|| uploadFile.ContentLength == 0)
                    {
                        return Json(new { message = "fileerror" });
                    }

                    //截取文件后缀名,判断文件类型是否被支持
                    if (FileType.Contains(uploadFile.FileName.Substring(uploadFile.FileName.LastIndexOf(".") + 1)))
                    {
                        //用jobsky作为分段符号
                        var fileTime = DateTime.Now.ToFileTime().ToString() + Path.GetFileName(uploadFile.FileName);//新命名的文件名（包含时间的整数形式）
                        var tofileName = Path.Combine(Request.MapPath("~/UploadFiles"), fileTime);//把两个名字连接起来
                        try
                        {
                            if (Directory.Exists(Server.MapPath("~/UploadFiles")) == false)//如果不存在就创建file文件夹
                            {
                                Directory.CreateDirectory(Server.MapPath("~/UploadFiles"));
                            }
                            uploadFile.SaveAs(tofileName);
                            temparticle.FileAddr = fileTime;//获得上传后的文件名
                        }
                        catch
                        {
                            return Json(new { message = "fileerror" });
                        }
                    }
                    else
                    {
                        return Json(new { message = "fileerror" });//不支持此格式文件上传
                    }
                }
            }
            else {
                temparticle.FileAddr = Request["IsDeleteFile"].ToString().Trim();//没有删除，则获得到的就是原来的文件名
            }

            //更新文章到数据库
            if (TempArticle.Update(temparticle)) {
                //删除需求
                TempDemandInfo.Delete(temparticle.TempArticleID);
                //插入需求到数据库
                foreach (var t in tempdemandInfo)
                {
                    t.TempArticleID = temparticle.TempArticleID;
                    if (TempDemandInfo.Insert(t))
                    {
                        //插入成功
                    }
                    else
                    {
                        return Json(new { message = "inserterror1" });
                    }
                }        
            }

            return Json(new { message = "success" });
        }

        //
        // POST: /Article/ArticleEdit/5
        //TempArticle里面的所有文章,并且属于cookie存储的雇主的文章，就可以删除
        [HttpPost]
        [ValidateInput(false)]          //这儿相当于给字段加[AllowHtml]，允许传入html
        [AgreeEmployerAuthorize]
        public ActionResult TempArticleDelete(int id)
        {
            return View();
        }

        
    }
}
