using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication1.Models;
using System.Data;
namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            int pagesize = 8;
            //int currentpageindex  = 1;
            //DateTime dtbegin = DateTime.Now.AddDays(-3650);
            //DateTime dtend = DateTime.Now.AddDays(200);
            //轮播图
            ViewBag.tb_Carousel = News.PictureNewsSelectRank();
            //招聘
            //TypeID	TypeName
            //1	本部招聘
            //2	湘雅招聘
            //3	铁道信息
            //4	同城招聘
            //5	在线招聘
            //6	图片新闻（用上面的轮播图）
            //7	新闻动态
            //8	通知公告
            //9	基层项目
            ViewBag.tb_1 = News.GetRecentRecord(pagesize, 1);//本部
            ViewBag.tb_2 = News.GetRecentRecord(pagesize, 2);//湘雅
            ViewBag.tb_3 = News.GetRecentRecord(pagesize, 3);//铁道
            ViewBag.tb_4 = News.GetRecentRecord(pagesize, 4);//同城
            ViewBag.tb_5 = News.GetRecentRecord(pagesize, 5);//在线            
            ViewBag.tb_13 = News.GetRecentRecord(pagesize, 13);//基层项目
            
            ViewBag.tb_7 = News.GetRecentRecord( 6, 7);//新闻
            ViewBag.tb_8 = News.GetRecentRecord( 6, 8);//通知公告
            return View();
        }

        public ActionResult NewsDetails(int id)//新数据库News
        {
            //获取文章
            ViewBag.tb_News = News.Select(id);
            if(ViewBag.tb_News.Rows.Count >= 1){
                ViewBag.TypeName = News.GetTypeNameByTypeID(Convert.ToInt32(ViewBag.tb_News.Rows[0]["TypeID"]));
                News.NewsClickTimesPlus(id);

                int pagesize = 8;
                ViewBag.tb_NewsList = News.GetRecentRecord(pagesize, Convert.ToInt32(ViewBag.tb_News.Rows[0]["TypeID"]));//相关文章列表
                foreach (System.Data.DataRow dt in ViewBag.tb_NewsList.Rows)
                {
                    dt["Title"] = News.LengthSub(dt["Title"].ToString(), 35);
                }
            }
            return View();
        }

        public ActionResult ArticleDetails(int id)//新数据库Article
        {
            //获取文章
            ViewBag.tb_Article = Article.Select(id);//文章信息
            if (ViewBag.tb_Article.Rows.Count >= 1)
            {
                ViewBag.tb_Demand = Article.GetDemandInfoByArticleID(id);//需求信息
                ViewBag.tb_Employer = Employer.Select(ViewBag.tb_Article.Rows[0]["EditorAccount"].ToString());//雇主信息
                if (Convert.ToInt32(ViewBag.tb_Article.Rows[0]["TypeID"]) == 1) //专场招聘
                {
                    ViewBag.dt_PlaceListSecond = Article.GetPlaceListSecondById(Convert.ToInt32(ViewBag.tb_Article.Rows[0]["PlaceSecondID"]));//时间和地点

                    ViewBag.TypeID = ArticleType.GetTypeIDFromView_ArticleTypeToNewsType(id);
                    ViewBag.TypeName = News.GetTypeNameByTypeID(Convert.ToInt32(ViewBag.TypeID));
                }
                else if (Convert.ToInt32(ViewBag.tb_Article.Rows[0]["TypeID"]) == 2 || Convert.ToInt32(ViewBag.tb_Article.Rows[0]["TypeID"]) == 3)  //双选会  组团招聘
                {
                    if (ViewBag.tb_Article.Rows[0]["EditorAccount"].ToString().Trim() != "jobsky")//公司发表的大型招聘情况
                    {
                        int BigArticleID = Article.GetBigArticleByArticleID(id);//获得大型招聘会ID  jobsky发布的
                        DataTable dt_BigArticle = Article.Select(BigArticleID);//获得单位文章信息，为了获得 PlaceSecondID
                        ViewBag.dt_PlaceListSecond = Article.GetPlaceListSecondById(Convert.ToInt32(dt_BigArticle.Rows[0]["PlaceSecondID"]));//时间和地点

                        ViewBag.TypeID = ViewBag.dt_PlaceListSecond.Rows[0]["PlaceFirstID"].ToString().Trim();
                        ViewBag.TypeName = News.GetTypeNameByTypeID(Convert.ToInt32(ViewBag.TypeID));
                    }
                    else
                    {//jobsky发的大型招聘的情况
                        ViewBag.dt_PlaceListSecond = Article.GetPlaceListSecondById(Convert.ToInt32(ViewBag.tb_Article.Rows[0]["PlaceSecondID"]));//时间和地点
                        ViewBag.dt_BigArticleList = Article.GetCompanyEmployerBigArticleIDByBigArticleID(id);
                        ViewBag.TypeID = ArticleType.GetTypeIDFromView_ArticleTypeToNewsType(id);
                        ViewBag.TypeName = News.GetTypeNameByTypeID(Convert.ToInt32(ViewBag.TypeID));
                    }
                }
                else if (Convert.ToInt32(ViewBag.tb_Article.Rows[0]["TypeID"]) == 4)    //在线招聘
                { 
                    //没有时间和地点

                    ViewBag.TypeID = ArticleType.GetTypeIDFromView_ArticleTypeToNewsType(id);
                    ViewBag.TypeName = News.GetTypeNameByTypeID(Convert.ToInt32(ViewBag.TypeID));
                }
                    

                Article.ArticleClickTimesPlus(id);
            }
            return View();
        }

        public ActionResult OldArticleDetails(int id)//老数据库Article
        {
            //获取文章
            ViewBag.tb_Article = News.SelectFromOldArticle(id);
            if (ViewBag.tb_Article.Rows.Count >= 1)
            {
                int typeid = Convert.ToInt32(ViewBag.tb_Article.Rows[0]["TypeID"]);
                if (typeid == 6)
                {//老数据库的TypeID=6对应新数据库的TypeID=8
                    typeid = 8;
                }
                else if (typeid == 4)
                {//老数据库的TypeID=4对应新数据库的TypeID=5
                    typeid = 5;
                }
                else if (typeid == 5)
                {//老数据库的TypeID=5对应新数据库的TypeID=4
                    typeid = 4;
                }

                ViewBag.TypeID = typeid;
                ViewBag.TypeName = News.GetTypeNameByTypeID(typeid);
                News.OldArticleClickTimesPlus(id);

                int pagesize = 8;
                ViewBag.tb_NewsList = News.GetRecentRecord(pagesize, typeid);//相关文章列表
                foreach (System.Data.DataRow dt in ViewBag.tb_NewsList.Rows)
                {
                    dt["Title"] = News.LengthSub(dt["Title"].ToString(),35);
                }
            }
            return View();
        } 

        public ActionResult ArticleList(int id)//列表
        {
            int pageindex = 1;
            int pagesize = 15;
            int typeid = id;
            //获取文章
            //ViewBag.tb_ArticleList = News.GetListByTypeID(pageindex, pagesize, typeid);
            //给插件的四个参数
            int totalpage = News.GetNewsRecordCountByType(typeid);
            ViewBag.Count = totalpage % pagesize == 0 ? totalpage / pagesize : totalpage / pagesize + 1 ;
            ViewBag.pageindex = pageindex;
            ViewBag.pagesize = pagesize;
            ViewBag.typeid = typeid;

            if (ViewBag.typeid == 1 || ViewBag.typeid == 2 || ViewBag.typeid == 3 || ViewBag.typeid == 4 || ViewBag.typeid == 5 || ViewBag.typeid == 13)
            {
                DateTime dtbegin = DateTime.Now;
                DateTime dtend = Convert.ToDateTime(DateTime.Now.AddDays(0).ToShortDateString());
                totalpage = News.GetNewsRecordCountByTypeIDAndDateTime(typeid, dtbegin, dtend);
                ViewBag.Count_today = totalpage % pagesize == 0 ? totalpage / pagesize : totalpage / pagesize + 1;

                dtend = Convert.ToDateTime(DateTime.Now.AddDays(2).ToShortDateString());
                totalpage = News.GetNewsRecordCountByTypeIDAndDateTime(typeid, dtbegin, dtend);
                ViewBag.Count_three = totalpage % pagesize == 0 ? totalpage / pagesize : totalpage / pagesize + 1 ;

                dtend = Convert.ToDateTime(DateTime.Now.AddDays(6).ToShortDateString());
                totalpage = News.GetNewsRecordCountByTypeIDAndDateTime(typeid, dtbegin, dtend);
                ViewBag.Count_week = totalpage % pagesize == 0 ? totalpage / pagesize : totalpage / pagesize + 1;

            }

            ViewBag.TypeName = News.GetTypeNameByTypeID(typeid);
            return View();
        }

        public ActionResult SearchDate()
        {
            DateTime date;
            if (DateTime.TryParse(Request["Date"].ToString(), out date))
            {
                //获取文章列表
                ViewBag.tb_ArticleList = News.GetListByDate(date);

                if (ViewBag.tb_ArticleList.Rows.Count > 0)
                {
                    ViewBag.Title = date.ToLongDateString().ToString() + "的招聘信息";//标题
                    int strLength = 50;
                    foreach (System.Data.DataRow dt in ViewBag.tb_ArticleList.Rows)
                    {
                        dt["Title"] = News.LengthSub(dt["Title"].ToString(), strLength);
                    }
                }
                else {
                    ViewBag.Title = date.ToLongDateString().ToString() + "暂无招聘信息";//标题
                }
            }
            else { 
                ViewBag.tb_ArticleList = new DataTable();
                ViewBag.Title = "错误的参数";
            }
            return View();
        }
        [HttpPost]
        public ActionResult SearchDateAllMonth()
        {
            DateTime date;
            if (DateTime.TryParse(Request["Date"].ToString(), out date))
            {
                DataTable src = News.GetListAllMouth(date);
                if (src.Rows.Count > 0){
                    string resultJson = JsonHelper.ToJson(src);
                    return Content(resultJson);//返回json结构的值，但不是json
                }
                return Content("{message : 'empty' }");
            }
            else {
                return Content("{message : 'error' }");
            }
        }

        static string key = "";
        public ActionResult Search(FormCollection collection)
        {
            key = Request["key"];
            int pageindex = 1;
            int pagesize = 15;
            //获取文章
            ViewBag.tb_ArticleList = News.GetListByKey(pageindex, pagesize, key);
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

        [HttpPost]
        public ActionResult PartialArticleListByKey(FormCollection collection)
        {
            int pageindex = Convert.ToInt32(Request["pageindex"]);
            int pagesize = Convert.ToInt32(Request["pagesize"]);
            key = Request["key"].ToString();
            ViewBag.key = key;
            //获取文章
            ViewBag.tb_ArticleList = News.GetListByKey(pageindex, pagesize,key);
            //Response.Write(key);
            return PartialView();
        }

        //高级搜索
        public ActionResult AdvancedSearch()
        {
            CreateDropDownList();
            AllModel advancedSearch = new AllModel(new Admin(), new News(), new ArticleType(), new Article(), new List<DemandInfo>(), new TempArticle(), new List<TempDemandInfo>(), new PlaceListFirst(), new PlaceListSecond(), new Employer(),new Student());

            #region 设置初始值
            //设置初始值
            string companyName = Request["employer.CompanyName"];
            if (companyName == null) companyName = "";

            string companyNature = Request["employer.CompanyNature"];
            if (companyNature == null || companyNature == "--请选择--") companyNature = "";
            else ViewBag.CompanyNature = new SelectList(ViewBag.CompanyNature, "Value", "Text", companyNature);

            string companyBusiness = Request["employer.CompanyBusiness"];
            if (companyBusiness == null || companyBusiness == "--请选择--") companyBusiness = "";
            else ViewBag.CompanyBusiness = new SelectList(ViewBag.CompanyBusiness, "Value", "Text", companyBusiness);

            string companySize = Request["employer.CompanySize"];
            if (companySize == null || companySize == "--请选择--") companySize = "";
            else ViewBag.CompanySize = new SelectList(ViewBag.CompanySize, "Value", "Text", companySize);

            string companyAreaProvince = Request["employer.CompanyAreaProvince"];
            if (companyAreaProvince == null || companyAreaProvince == "--请选择--") companyAreaProvince = "";

            string companyAreaCity = Request["employer.CompanyAreaCity"];
            if (companyAreaCity == null || companyAreaCity == "--请选择--") companyAreaCity = "";

            string major = Request["Major"];
            if (major == null) major = "";    

            #endregion

            ViewBag.employer_CompanyName = companyName;
            ViewBag.employer_CompanyNature = companyNature;
            ViewBag.employer_CompanyBusiness = companyBusiness;
            ViewBag.employer_CompanySize = companySize;
            ViewBag.employer_CompanyAreaProvince = companyAreaProvince;
            ViewBag.employer_CompanyAreaCity = companyAreaCity;
            ViewBag.Major = major;

            #region 设置model值
            //设置Model的值
            advancedSearch.employer.CompanyName = companyName;
            advancedSearch.employer.CompanyNature = companyNature;
            advancedSearch.employer.CompanyBusiness = companyBusiness;
            advancedSearch.employer.CompanySize = companySize;
            advancedSearch.employer.CompanyAreaProvince = companyAreaProvince;
            advancedSearch.employer.CompanyAreaCity = companyAreaCity;
            #endregion

            //单位名称，单位性质，单位行业，单位规模，单位所在省份，单位所在城市
            int pageindex = 1;
            int pagesize = 15;
            //获取文章
            ViewBag.EmployerArticles = Article.GetEmployerArticleByAdvancedKey(pagesize, pageindex,
                companyName, companyNature, companyBusiness, companySize,
                companyAreaProvince, companyAreaCity,major);//列表数据

            //给插件的四个参数
            int totalpage = Article.GetEmployerArticleCountByAdvancedKey(companyName, companyNature,
                companyBusiness, companySize, companyAreaProvince, companyAreaCity,major);//数据数量
            int count = totalpage % pagesize == 0 ? totalpage / pagesize : totalpage / pagesize + 1;
            if (count == 0) count = 1;
            ViewBag.Count = count;
            ViewBag.pageindex = pageindex;
            ViewBag.pagesize = pagesize;
            //ViewBag.key = key;           
            return View(advancedSearch);
        }

        [HttpPost]
        public ActionResult PartialAdvancedSearchList(FormCollection collection)
        {
            #region 获取值
            int pageindex = Convert.ToInt32(Request["pageindex"]);
            int pagesize = Convert.ToInt32(Request["pagesize"]);
            string companyName = Request["employer.CompanyName"];
            if (companyName == null) companyName = "";

            string companyNature = Request["employer.CompanyNature"];
            if (companyNature == null || companyNature == "--请选择--") companyNature = "";

            string companyBusiness = Request["employer.CompanyBusiness"];
            if (companyBusiness == null || companyBusiness == "--请选择--") companyBusiness = "";

            string companySize = Request["employer.CompanySize"];
            if (companySize == null || companySize == "--请选择--") companySize = "";

            string companyAreaProvince = Request["employer.CompanyAreaProvince"];
            if (companyAreaProvince == null || companyAreaProvince == "--请选择--") companyAreaProvince = "";

            string companyAreaCity = Request["employer.CompanyAreaCity"];
            if (companyAreaCity == null || companyAreaCity == "--请选择--") companyAreaCity = "";

            string major = Request["Major"];
            if (major == null) major = "";
            #endregion

            ViewBag.employer_CompanyName = companyName;
            ViewBag.employer_CompanyNature = companyNature;
            ViewBag.employer_CompanyBusiness = companyBusiness;
            ViewBag.employer_CompanySize = companySize;
            ViewBag.employer_CompanyAreaProvince = companyAreaProvince;
            ViewBag.employer_CompanyAreaCity = companyAreaCity;
            ViewBag.Major = major;

            //获取文章
            ViewBag.EmployerArticles = Article.GetEmployerArticleByAdvancedKey(pagesize, pageindex,
                companyName, companyNature, companyBusiness, companySize,
                companyAreaProvince, companyAreaCity,major);//列表数据
            //Response.Write(key);

            return PartialView();
        }

        [HttpPost]
        public ActionResult PartialArticleList(FormCollection collection)
        {
            int pageindex = Convert.ToInt32(collection["pageindex"]);
            int pagesize = Convert.ToInt32(collection["pagesize"]);
            int typeid = Convert.ToInt32(collection["typeid"]);
            int followingdates = Convert.ToInt32(collection["followingdates"]);//0表示今天，1表示近两天，以此类推
            //获取文章
            if (followingdates >= 0) {
                DateTime dtbegin = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                DateTime dtend = Convert.ToDateTime(DateTime.Now.AddDays(followingdates).ToShortDateString());
                ViewBag.tb_ArticleList = News.GetListByTypeIDAndDateTime(pageindex, pagesize, typeid, dtbegin, dtend);
            }
            else
            {
                ViewBag.tb_ArticleList = News.GetListByTypeID(pageindex, pagesize, typeid);
            }
            
            return PartialView();
        } 

        /// <summary>
        /// 关于网站
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            return View();
        }
        /// <summary>
        /// 联系我们
        /// </summary>
        /// <returns></returns>
        public ActionResult Contact()
        {
            return View();
        }
        /// <summary>
        /// 院系风采列表
        /// </summary>
        /// <returns></returns>
        public ActionResult CollegeList()
        {
            ViewBag.tb_CollegeList = News.GetCollegeList();
            return View();
        }
        /// <summary>
        /// 院系风采文章
        /// </summary>
        /// <returns></returns>
        public ActionResult CollegeDetails(int id)
        {
            ViewBag.tb_College = News.GetCollege(id);
            return View();
        }
        /// <summary>
        /// 友情链接
        /// </summary>
        /// <returns></returns>
        public ActionResult ApplyLink()
        {
            return View();
        }

        /// <summary>
        /// 单位导航
        /// </summary>
        /// <returns></returns>
        public ActionResult UnitNav()
        {
            return View();
        }

        /// <summary>
        /// 学生导航
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentNav()
        {
            return View();
        }

        #region 各个DropDownList的初始化
        public void CreateDropDownList()
        {
            #region 单位性质DropDownList
            List<SelectListItem> itemsCompanyNature = new List<SelectListItem>();
            itemsCompanyNature.Add(new SelectListItem { Text = "--请选择--", Value = null });
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
            itemsCompanyBusiness.Add(new SelectListItem { Text = "--请选择--", Value = null });
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
            itemsCompanySize.Add(new SelectListItem { Text = "--请选择--", Value = null });
            itemsCompanySize.Add(new SelectListItem { Text = "1-49人", Value = "1-49人" });
            itemsCompanySize.Add(new SelectListItem { Text = "50-99人", Value = "50-99人" });
            itemsCompanySize.Add(new SelectListItem { Text = "100-499人", Value = "100-499人" });
            itemsCompanySize.Add(new SelectListItem { Text = "500-999人", Value = "500-999人" });
            itemsCompanySize.Add(new SelectListItem { Text = "1000人以上", Value = "1000人以上" });
            ViewBag.CompanySize = itemsCompanySize;
            #endregion
        }
        #endregion
    }
}
