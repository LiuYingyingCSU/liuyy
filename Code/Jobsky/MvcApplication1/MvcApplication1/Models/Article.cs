using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace MvcApplication1.Models
{
    public class Article
    {
        public int ArticleID { get; set; }
        public int TypeID { get; set; }
        public string Title { get; set; }
        public string Introduction { get; set; }
        public int PlaceSecondID { get; set; }
        public DateTime EditTime { get; set; }
        public string EditorAccount { get; set; }
        public string ContactInfo { get; set; }
        public int ClickTimes { get; set; }
        public string FileAddr { get; set; }

        #region 添加文章
        public static int Insert(Article article)
        {
            //安全性检查，如果time格式不对，函数返回false
            //try {  }
            //catch () { return false; }

            int ID = 0;//存储执行insert存储过程返回的ID

            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "ArticleInsert";
                cmd.Parameters.Add(new SqlParameter("@Title", article.Title));
                cmd.Parameters.Add(new SqlParameter("@Introduction", article.Introduction));
                cmd.Parameters.Add(new SqlParameter("@EditTime", article.EditTime));
                cmd.Parameters.Add(new SqlParameter("@EditorAccount", article.EditorAccount));
                cmd.Parameters.Add(new SqlParameter("@TypeID", article.TypeID));
                cmd.Parameters.Add(new SqlParameter("@ContactInfo", article.ContactInfo));
                cmd.Parameters.Add(new SqlParameter("@FileAddr", article.FileAddr));
                ID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch
            {
                return -1; //如果出错，返回-1
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
            return ID;//如果没有出错，返回ID
        }
        #endregion

        public static int ArticleInsert(Article article)
        {
            SqlConnection conn = DBLink.GetConnection();//拿到新数据库的链接 
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ArticleInsert";
            cmd.Parameters.Add(new SqlParameter("@TypeID", article.TypeID));
            cmd.Parameters.Add(new SqlParameter("@Title", article.Title));
            cmd.Parameters.Add(new SqlParameter("@Introduction", article.Introduction));
            if (article.PlaceSecondID != -1)   //-1表示不是专场招聘，没有添加招聘场地
            {
                cmd.Parameters.Add(new SqlParameter("@PlaceSecondID", article.PlaceSecondID));  //这个字段默认为null
            }
            cmd.Parameters.Add(new SqlParameter("@EditTime", article.EditTime));
            cmd.Parameters.Add(new SqlParameter("@EditorAccount", article.EditorAccount));
            cmd.Parameters.Add(new SqlParameter("@ContactInfo", article.ContactInfo));
            cmd.Parameters.Add(new SqlParameter("@ClickTimes", article.ClickTimes));
            cmd.Parameters.Add(new SqlParameter("@FileAddr", article.FileAddr));
            int ArticleID = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            cmd.Dispose();
            conn.Close();
            return ArticleID;//如果没有出错，返回刚插入的ArticleID
        }

        #region 更新文章
        public static bool Update(Article article)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ArticleUpdate";
            cmd.Parameters.Add(new SqlParameter("@ArticleID", article.ArticleID));
            cmd.Parameters.Add(new SqlParameter("@Title", article.Title));
            cmd.Parameters.Add(new SqlParameter("@Introduction", article.Introduction));
            //cmd.Parameters.Add(new SqlParameter("@TypeID", article.TypeID));
            cmd.Parameters.Add(new SqlParameter("@ContactInfo", article.ContactInfo));
            if (article.FileAddr == null || article.FileAddr == "")
            {
                cmd.Parameters.Add(new SqlParameter("@FileAddr", DBNull.Value));
            }
            else { 
                cmd.Parameters.Add(new SqlParameter("@FileAddr", article.FileAddr));
            }

            try { 
                cmd.ExecuteNonQuery(); 
            }
            catch
            {
                return false; //如果出错，返回-1
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
            return true;//如果没有出错，返回ID
        }
        #endregion

        public static bool DeleteEmployerArticle(int id, int isAudit)
        {
            SqlConnection cn = DBLink.GetConnection();
            string sqlstr1 = "";
            string sqlstr2 = "";
            string sqlstr3 = "";//用于处理类型为专场招聘并且已经安排了场地的PlaceListSecond表记录
            string sqlstr4 = "";//用于删除大型招聘会的BigArticle表里，某一公司的记录
            DataTable dtArticle = null;    //根据ArticleID得到Article文章信息
            if (isAudit == 2)
            {
                dtArticle = Article.GetArticleByIDForUpdate(id);    //根据ArticleID得到Article文章信息
                //如果是已经审核的
                sqlstr1 = "delete from DemandInfo where ArticleID=" + id;
                sqlstr2 = "delete from Article where ArticleID=" + id;
                sqlstr3 = "delete from PlaceListSecond where PlaceSecondID=" + dtArticle.Rows[0]["PlaceSecondID"].ToString();
                sqlstr4 = "delete from BigArticle where EmployerBigArticleID = " + dtArticle.Rows[0]["ArticleID"].ToString();
            }
            else
            {
                //待审核或者已经拒绝的
                sqlstr1 = "delete from TempDemandInfo where TempArticleID=" + id;
                sqlstr2 = "delete from TempArticle where TempArticleID=" + id;
            }
            SqlCommand cmm1 = new SqlCommand(sqlstr1, cn);
            SqlCommand cmm2 = new SqlCommand(sqlstr2, cn);
            SqlCommand cmm3 = new SqlCommand(sqlstr3, cn);  //删除已通过审核专场招聘的场地信息
            SqlCommand cmm4 = new SqlCommand(sqlstr4, cn);  //删除已通过审核大型招聘会的BigArticle表里，某一公司的记录
            try
            {
                cn.Open();
                cmm1.ExecuteNonQuery();
                cmm2.ExecuteNonQuery();
                if (isAudit == 2)//已经审核的
                {
                    if (dtArticle.Rows[0]["TypeName"].ToString().Trim() == "专场招聘")//专场招聘
                    {
                        cmm3.ExecuteNonQuery();//删除地点信息
                    }
                    //2-双选会，3-组团招聘
                    if (Int32.Parse(dtArticle.Rows[0]["TypeID"].ToString()) == 2 || Int32.Parse(dtArticle.Rows[0]["TypeID"].ToString()) == 3)
                    {
                        cmm4.ExecuteNonQuery();//删除大型招聘会记录
                    }
                }
                return true;
            }
            catch (Exception)
            {

            }
            finally
            {
                cmm1.Dispose();
                cmm2.Dispose();
                cmm3.Dispose();
                cn.Close();
            }
            return false;
        }

        #region 查询文章
        public static DataTable Select(int articleid)
        {
            //安全性检查，如果time格式不对，函数返回false
            //try {  }
            //catch () { return false; }
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Article where Article.ArticleID = @ArticleID", conn);
            cmd.Parameters.Add(new SqlParameter("@ArticleID", articleid));

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion

        #region 新数据库Article浏览次数加一
        public static void ArticleClickTimesPlus(int ArticleID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("update [Article] set [ClickTimes]=([ClickTimes]+1) where [ArticleID]=@ArticleID",conn);
            cmd.Parameters.Add(new SqlParameter("@ArticleID", ArticleID));
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            conn.Close();
        }
        #endregion

        #region 获取PlaceListFirst表
        /// <summary>
        /// 获得时间和地点
        /// 各字段分别为
        /// </summary>
        /// <param name="id">参数，文章ID</param>
        /// <returns>文章全部信息组成的DataTable</returns>
        public static DataTable GetPlaceListFirst()
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select * from [PlaceListFirst]", conn);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion

        #region 根据PlaceSecondID获得时间和地点
        /// <summary>
        /// 获得时间和地点
        /// 各字段分别为
        /// </summary>
        /// <param name="id">参数，文章ID</param>
        /// <returns>文章全部信息组成的DataTable</returns>
        public static DataTable GetPlaceListSecondById(int PlaceSecondID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select * from [PlaceListSecond] where PlaceSecondID = @PlaceSecondID", conn);
            cmd.Parameters.Add(new SqlParameter("@PlaceSecondID", PlaceSecondID));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion

        #region 用人单位查询自己的文章
        public static DataTable GetArticleByArticleIDAndEditorAccount(int ArticleID, string EditorAccount)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Article where Article.ArticleID = @ArticleID and Article.EditorAccount = @EditorAccount", conn);
            cmd.Parameters.Add(new SqlParameter("@ArticleID", ArticleID));
            cmd.Parameters.Add(new SqlParameter("@EditorAccount", EditorAccount));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion

        public static DataTable SelectArticleFromArticle(int typeid, DateTime edittime)
        {
            //安全性检查，如果time格式不对，函数返回false
            //try {  }
            //catch () { return false; }
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Article,PlaceListSecond where Article.PlaceSecondID = PlaceListSecond.PlaceSecondID and TypeID = @typeid and PlaceListSecond.PlaceTime > @edittime", conn);
            cmd.Parameters.Add(new SqlParameter("@typeid", typeid));
            cmd.Parameters.Add(new SqlParameter("@edittime", edittime));

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }

        #region 查询文章类型
        public static int GetTypeIDByArticleID(int ArticleID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select TypeID from [Article] where ArticleID = @ArticleID", conn);
            cmd.Parameters.Add(new SqlParameter("@ArticleID", ArticleID));
            int typeid = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            conn.Close();
            return typeid;
        }
        #endregion

        #region 获取所有需求（通过文章ID）
        public static DataTable GetDemandInfoByArticleID(int ArticleID)
        {
            //try {  }
            //catch () { return false; }
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select * from [DemandInfo] where ArticleID = @ArticleID", conn);
            cmd.Parameters.Add(new SqlParameter("@ArticleID", ArticleID));

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion

        /// <summary>
        /// 获得所属jobsky发布的文章大型招聘会的单位文章
        /// 各字段分别为
        /// </summary>
        /// <param name="id">参数，文章ID</param>
        /// <returns>文章全部信息组成的DataTable</returns>
        public static DataTable GetCompanyEmployerBigArticleIDByBigArticleID(int BigArticleID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select ArticleID,Title from [BigArticle],[Article] where [BigArticle].EmployerBigArticleID = [Article].ArticleID and BigArticleID = @BigArticleID", conn);
            cmd.Parameters.Add(new SqlParameter("@BigArticleID", BigArticleID));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }


        public static DataTable GetArticleListForEmployer(string editoraccount, int pageindex, int pagesize, int typeid)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetArticlePagedRecordForEditor";

            cmd.Parameters.Add(new SqlParameter("@EditorAccount", editoraccount));
            cmd.Parameters.Add(new SqlParameter("@TypeID", typeid));
            cmd.Parameters.Add(new SqlParameter("@pagesize", pagesize));
            cmd.Parameters.Add(new SqlParameter("@pageindex", pageindex));

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }

        public static int GetArticleRecordCountForEmployer(string editoraccount, int typeid)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            string sqlstr = "";
            if(typeid == 0){
                sqlstr="SELECT Count(*) from Article where EditorAccount=@EditorAccount";
            }
            else{
                sqlstr="SELECT Count(*) from Article where EditorAccount=@EditorAccount and TypeID=@TypeID";
            }
            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            cmd.Parameters.Add(new SqlParameter("@EditorAccount", editoraccount));
            cmd.Parameters.Add(new SqlParameter("@TypeID", typeid));

            int totalpagecount = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            conn.Close();
            return totalpagecount;
        }

        #region 分页并且根据日期类型（显示几天）和是否审核得到文章列表
        public static DataTable GetArticleByAuditByDttypeByPage(int PageSize, int CurrentPageIndex, int isAudit, DateTime dtbegin, DateTime dtend, int dttype)
        {
            SqlConnection cn = DBLink.GetConnection();
            SqlCommand cmm = new SqlCommand();
            cmm.Connection = cn;
            //调用存储过程
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "GetArticleByAuditByPage";
            //存储过程参数
            cmm.Parameters.Add(new SqlParameter("@pagesize", PageSize));
            cmm.Parameters.Add(new SqlParameter("@pageindex", CurrentPageIndex));
            cmm.Parameters.Add(new SqlParameter("@isAudit", isAudit));
            cmm.Parameters.Add(new SqlParameter("@dtBegin", dtbegin));
            cmm.Parameters.Add(new SqlParameter("@dtEnd", dtend));
            cmm.Parameters.Add(new SqlParameter("@dtType", dttype));
            SqlDataAdapter da = new SqlDataAdapter(cmm);
            DataTable dt = new DataTable();
            try
            {
                cn.Open();
                da.Fill(dt);
                cn.Close();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
            }
            return dt;
        }
        #endregion      

        #region 分页并且根据日期类型（显示几天）和是否审核和关键字得到文章列表
        public static DataTable GetArticleByAuditByDttypeByPageByKey(int PageSize, int CurrentPageIndex, int isAudit, DateTime dtbegin, DateTime dtend, int dttype, string key,string adminType="0")
        {
            SqlConnection cn = DBLink.GetConnection();
            SqlCommand cmm = new SqlCommand();
            cmm.Connection = cn;
            //调用存储过程
            cmm.CommandType = CommandType.StoredProcedure;
            if (adminType == "1" || adminType == "2")
            {
                int typeID = 2;//1为湘雅，对应PlaceListFirstID为2
                if (adminType == "2") typeID = 3;
                cmm.CommandText = "GetArticleByAuditByPageByKeyByAdminType";
                cmm.Parameters.Add(new SqlParameter("@typeID", typeID));
            }
            else
            {
                cmm.CommandText = "GetArticleByAuditByPageByKey";
            }          
            //存储过程参数
            cmm.Parameters.Add(new SqlParameter("@pagesize", PageSize));
            cmm.Parameters.Add(new SqlParameter("@pageindex", CurrentPageIndex));
            cmm.Parameters.Add(new SqlParameter("@isAudit", isAudit));
            cmm.Parameters.Add(new SqlParameter("@dtBegin", dtbegin));
            cmm.Parameters.Add(new SqlParameter("@dtEnd", dtend));
            cmm.Parameters.Add(new SqlParameter("@dtType", dttype));
            cmm.Parameters.Add(new SqlParameter("@key", key));
            SqlDataAdapter da = new SqlDataAdapter(cmm);
            DataTable dt = new DataTable();
            try
            {
                cn.Open();
                da.Fill(dt);
                cn.Close();
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
            }
            return dt;
        }
        #endregion      

        #region 分页并且根据日期类型（显示几天）和是否审核得到文章"数量"
        public static int GetArticleRecordCountByAuditByDttype(int isAudit, DateTime dtbegin, DateTime dtend, int dttype)
        {
            int count = 0;
            SqlConnection cn = DBLink.GetConnection();
            string sqlstr = "";
            if (isAudit == 2)
            {
                //如果是已经审核的，去Article表里找
                if (dttype == -1)
                {
                    //显示全部信息
                    sqlstr = "SELECT Count(*) from Article";
                }
                else
                {
                    sqlstr = "SELECT Count(*) from Article where (select PlaceTime from PlaceListSecond where PlaceListSecond.PlaceSecondID=Article.PlaceSecondID) between " + dtbegin.ToShortDateString() + " and " + dtend.ToShortDateString();
                }
            }
            else
            {
                //待审核的或者已经拒绝的，去TempArticle表里根据IsAudit找
                if (dttype == -1)
                {
                    //显示全部信息
                    sqlstr = "SELECT Count(*) from TempArticle where IsAudit=" + isAudit;
                }
                else
                {
                    sqlstr = "SELECT Count(*) from TempArticle where IsAudit=" + isAudit + " and (select PlaceTime from PlaceListSecond where PlaceListSecond.PlaceSecondID=TempArticle.PlaceSecondID) between " + dtbegin.ToShortDateString() + " and " + dtend.ToShortDateString();
                }
            }

            SqlCommand cmm = new SqlCommand(sqlstr, cn);
            cn.Open();
            try
            {
                count = int.Parse(cmm.ExecuteScalar().ToString());
            }
            catch (Exception)
            {

            }
            return count;
        }
        #endregion

        #region 分页并且根据日期类型（显示几天）和是否审核和关键字得到文章"数量"
        public static int GetArticleRecordCountByAuditByDttypeByKey(int isAudit, DateTime dtbegin, DateTime dtend, int dttype, string key,string adminType="0")
        {
            int count = 0;
            SqlConnection cn = DBLink.GetConnection();
            string sqlstr = "";
            int typeID = 2;//adminType为1表示湘雅管理员，对应placeFirstID为2
            if (adminType == "1" || adminType == "2")
            {
                //根据湘雅还是铁道分校区显示              
                if (adminType == "2") typeID = 3;
                if (isAudit == 2)
                {
                    //如果是已经审核的，去Article表里找
                    if (dttype == -1)
                    {
                        //显示全部信息
                        sqlstr = "SELECT Count(*) from Article left join PlaceListSecond on PlaceListSecond.PlaceSecondID=Article.PlaceSecondID where Title like '%'+@key+'%' and PlaceFirstID=@typeID";
                    }
                    else
                    {
                        sqlstr = "SELECT Count(*) from Article left join PlaceListSecond on PlaceListSecond.PlaceSecondID=Article.PlaceSecondID where (select PlaceTime from PlaceListSecond where PlaceListSecond.PlaceSecondID=Article.PlaceSecondID) between " + dtbegin.ToShortDateString() + " and " + dtend.ToShortDateString() + " and Title like '%'+@key+'%' and PlaceFirstID=@typeID";
                    }
                }
                else
                {
                    //待审核的或者已经拒绝的，去TempArticle表里根据IsAudit找
                    if (dttype == -1)
                    {
                        //显示全部信息
                        sqlstr = "SELECT Count(*) from TempArticle where Title like '%'+@key+'%' and PlaceFirstID=@typeID and IsAudit=" + isAudit;
                    }
                    else
                    {
                        sqlstr = "SELECT Count(*) from TempArticle where Title like '%'+@key+'%' and PlaceFirstID=@typeID and IsAudit=" + isAudit + " and (select PlaceTime from PlaceListSecond where PlaceListSecond.PlaceSecondID=TempArticle.PlaceSecondID) between " + dtbegin.ToShortDateString() + " and " + dtend.ToShortDateString();
                    }
                }
            }
            else
            {
                //显示所有的
                if (isAudit == 2)
                {
                    //如果是已经审核的，去Article表里找
                    if (dttype == -1)
                    {
                        //显示全部信息
                        sqlstr = "SELECT Count(*) from Article where Title like '%'+@key+'%'";
                    }
                    else
                    {
                        sqlstr = "SELECT Count(*) from Article where (select PlaceTime from PlaceListSecond where PlaceListSecond.PlaceSecondID=Article.PlaceSecondID) between " + dtbegin.ToShortDateString() + " and " + dtend.ToShortDateString() + " and Title like '%'+@key+'%'";
                    }
                }
                else
                {
                    //待审核的或者已经拒绝的，去TempArticle表里根据IsAudit找
                    if (dttype == -1)
                    {
                        //显示全部信息
                        sqlstr = "SELECT Count(*) from TempArticle where Title like '%'+@key+'%' and IsAudit=" + isAudit;
                    }
                    else
                    {
                        sqlstr = "SELECT Count(*) from TempArticle where Title like '%'+@key+'%' and IsAudit=" + isAudit + " and (select PlaceTime from PlaceListSecond where PlaceListSecond.PlaceSecondID=TempArticle.PlaceSecondID) between " + dtbegin.ToShortDateString() + " and " + dtend.ToShortDateString();
                    }
                }
            }

            SqlCommand cmm = new SqlCommand(sqlstr, cn);
            cmm.Parameters.Add(new SqlParameter("@key", key));
            if (adminType == "1" || adminType == "2")
            {
                cmm.Parameters.Add(new SqlParameter("@typeID", typeID));
            }
            cn.Open();
            try
            {
                count = int.Parse(cmm.ExecuteScalar().ToString());
            }
            catch (Exception)
            {

            }
            return count;
        }
        #endregion

        #region 高级检索文章——得到文章
        public static DataTable GetEmployerArticleByAdvancedKey(int PageSize, int CurrentPageIndex, string companyName
            , string companyNature, string companyBusiness, string companySize, string companyAreaProvince, string companyAreaCity,
            string major)
        {
            //单位名称，单位性质，单位行业，单位规模，单位所在省份，单位所在城市
            SqlConnection cn = DBLink.GetConnection();
            SqlCommand cmm = new SqlCommand();
            cmm.Connection = cn;
            //调用存储过程
            cmm.CommandType = CommandType.StoredProcedure;
            cmm.CommandText = "GetEmployerArticleByAdvancedKey";
            //存储过程参数
            cmm.Parameters.Add(new SqlParameter("@pagesize", PageSize));
            cmm.Parameters.Add(new SqlParameter("@pageindex", CurrentPageIndex));
            cmm.Parameters.Add(new SqlParameter("@companyName", companyName));
            cmm.Parameters.Add(new SqlParameter("@companyNature", companyNature));
            cmm.Parameters.Add(new SqlParameter("@companyBusiness", companyBusiness));
            cmm.Parameters.Add(new SqlParameter("@companySize", companySize));
            cmm.Parameters.Add(new SqlParameter("@companyAreaProvince", companyAreaProvince));
            cmm.Parameters.Add(new SqlParameter("@companyAreaCity", companyAreaCity));
            cmm.Parameters.Add(new SqlParameter("@major", major));
            SqlDataAdapter da = new SqlDataAdapter(cmm);
            DataTable dt = new DataTable();
            try
            {
                cn.Open();
                da.Fill(dt);
                cn.Close();
                //HttpContext.Current.Response.Write(dt.Rows[0]["CompanyName"].ToString());
            }
            catch (Exception ex)
            {
                //HttpContext.Current.Response.Write(ex.Message);
            }
            return dt;
        }
        #endregion

        #region 高级检索文章——得到文章数量
        public static int GetEmployerArticleCountByAdvancedKey(string companyName, string companyNature,
            string companyBusiness, string companySize, string companyAreaProvince, string companyAreaCity,
            string major)
        {
            //单位名称，单位性质，单位行业，单位规模，单位所在省份，单位所在城市
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            string sqlstr = "";
            sqlstr = "select Count(*) from Article,Employer";
            sqlstr += " where Article.EditorAccount=Employer.EmployerAccount";
            sqlstr += " and CompanyName like '%'+@companyName+'%'";
            sqlstr += " and CompanyNature like '%'+@companyNature+'%'";
            sqlstr += " and CompanyBusiness like '%'+@companyBusiness+'%'";
            sqlstr += " and CompanySize like '%'+@companySize+'%'";
            sqlstr += " and CompanyAreaProvince like '%'+@companyAreaProvince+'%'";
            sqlstr += " and CompanyAreaCity like '%'+@companyAreaCity+'%'";
            sqlstr += " and ArticleID in(select ArticleID from DemandInfo where Major like '%'+@major+'%')";
            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            //存储过程参数
            cmd.Parameters.Add(new SqlParameter("@companyName", companyName));
            cmd.Parameters.Add(new SqlParameter("@companyNature", companyNature));
            cmd.Parameters.Add(new SqlParameter("@companyBusiness", companyBusiness));
            cmd.Parameters.Add(new SqlParameter("@companySize", companySize));
            cmd.Parameters.Add(new SqlParameter("@companyAreaProvince", companyAreaProvince));
            cmd.Parameters.Add(new SqlParameter("@companyAreaCity", companyAreaCity));
            cmd.Parameters.Add(new SqlParameter("@major", major));

            int totalpagecount = Convert.ToInt32(cmd.ExecuteScalar());
            //HttpContext.Current.Response.Write(totalpagecount);
            cmd.Dispose();
            conn.Close();
            return totalpagecount;
        }
        #endregion

        public static bool InsertBigArticle(int EmployerBigArticleID, int BigArticleID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "BigArticleInsert";
            cmd.Parameters.Add(new SqlParameter("@EmployerBigArticleID", EmployerBigArticleID));
            cmd.Parameters.Add(new SqlParameter("@BigArticleID", BigArticleID));
            try { cmd.ExecuteNonQuery(); }
            catch
            {
                return false; //如果出错，返回
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
            return true;//如果没有出错，返回
        }

        public static bool DeleteBigArticle(int EmployerBigArticleID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("delete from BigArticle where EmployerBigArticleID = @EmployerBigArticleID", conn);
            cmd.Parameters.Add(new SqlParameter("@EmployerBigArticleID", EmployerBigArticleID));
            try { cmd.ExecuteNonQuery(); }
            catch
            {
                return false; //如果出错，返回
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
            return true;//如果没有出错，返回
        }

        public static bool UpdateBigArticle(int EmployerBigArticleID, int BigArticleID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("update BigArticle set BigArticleID = @BigArticleID where EmployerBigArticleID = @EmployerBigArticleID", conn);
            cmd.Parameters.Add(new SqlParameter("@EmployerBigArticleID", EmployerBigArticleID));
            cmd.Parameters.Add(new SqlParameter("@BigArticleID", BigArticleID));
            try { cmd.ExecuteNonQuery(); }
            catch
            {
                return false; //如果出错，返回
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
            return true;//如果没有出错，返回
        }

        #region 获取大型文章ID
        public static int GetBigArticleByArticleID(int EmployerBigArticleID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select BigArticleID from [BigArticle] where EmployerBigArticleID = @EmployerBigArticleID", conn);
            cmd.Parameters.Add(new SqlParameter("@EmployerBigArticleID", EmployerBigArticleID));
            int articleid = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            conn.Close();
            return articleid;
        }
        #endregion

        #region 获取最近信息
        /// <summary>
        /// 获取最近信息，使用存储过程，根据属性RecordCount设置信息条数，SmallCID确定文章类型
        /// </summary>
        /// <returns></returns>
        public static DataTable GetRecentRecord(int RecordCount, int TypeID)
        {
            SqlConnection conn = DBLink.GetJobsky6Connection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetRecentRecordFromOldAndNewDatabase";
            cmd.Parameters.Add(new SqlParameter("@recordcount", RecordCount));
            cmd.Parameters.Add(new SqlParameter("@TypeID", TypeID));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion


        #region 获取类型为1234的列表(本部、湘雅、铁道、在线招聘 typeid=0则选择所有类型  只针对新数据库的Article)
        public static DataTable GetArticlePagedRecord_123(int pagesize, int currentpageindex, int typeid)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandTimeout = 180;//重新设置超时时间试试，修改时间2012年6月26日13:31:00
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetArticlePagedRecord";
            cmd.Parameters.Add(new SqlParameter("@pagesize", pagesize));
            cmd.Parameters.Add(new SqlParameter("@pageindex", currentpageindex));
            cmd.Parameters.Add(new SqlParameter("@TypeID", typeid));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion


        #region 修改已经审核过的文章
        public static bool UpdateArticle(AllModel articleAndPlace, int articleID, string typeName)
        {
            //得到文章ID
            int ArticleID = articleID;
            //根据文章ID更新Title
            SqlConnection connTitle = DBLink.GetConnection();
            string sqlstrTitle = "update Article set Title='" + articleAndPlace.article.Title + "' where ArticleID=" + ArticleID;
            SqlCommand cmmTitle = new SqlCommand(sqlstrTitle, connTitle);
            try
            {
                connTitle.Open();
                cmmTitle.ExecuteNonQuery();
                cmmTitle.Dispose();
                connTitle.Close();
            }
            catch (Exception)
            {
                return false;
            }

            //接下来是专场招聘（要更新场地）
            if (typeName == "专场招聘")
            {
                DataTable dtArticle = Article.GetArticleByIDForUpdate(ArticleID);    //根据ArticleID得到Article文章信息
                if (dtArticle.Rows[0]["TypeName"].ToString().Trim() == "专场招聘")
                {
                    //更新场地
                    int placeSecondID = Int32.Parse(dtArticle.Rows[0]["PlaceSecondID"].ToString());
                    int placefirstID = articleAndPlace.placeListSecond.PlaceFirstID;
                    string placeName = articleAndPlace.placeListSecond.PlaceName;
                    DateTime placeTime = articleAndPlace.placeListSecond.PlaceTime;
                    if (PlaceListSecond.PlaceListSecondUpdate(placeSecondID, placefirstID, placeName, placeTime))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region 根据文章ID获取已经通过审核的文章详细信息
        public static DataTable GetArticleByIDForUpdate(int articleID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetArticleByIDForUpdate";
            cmd.Parameters.Add(new SqlParameter("@ArticleID", articleID));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion

        #region 审核文章操作
        public static bool AuditArticle(AllModel auditArticle, int tempArticleID, string typeName, int isAgree)
        {
            //isAgree=1表示通过审核，=-1表示审核未通过
            if (isAgree == 1)
            {
                //根据文章ID更新Title
                SqlConnection connTitle = DBLink.GetConnection();
                string sqlstrTitle = "update TempArticle set Title='" + auditArticle.tempArticle.Title + "' where TempArticleID=" + tempArticleID;
                SqlCommand cmmTitle = new SqlCommand(sqlstrTitle, connTitle);
                try
                {
                    connTitle.Open();
                    cmmTitle.ExecuteNonQuery();
                    cmmTitle.Dispose();
                    connTitle.Close();
                }
                catch (Exception)
                {
                    return false;
                }
                DataTable dtTempArticle = TempArticle.GetTempArticleByTempArticleID(tempArticleID);    //根据TempArticleID得到TempArticle文章信息
                if (dtTempArticle.Rows[0]["ArticleID"].ToString() == "")//第一次审核
                {
                    int PlaceSecondID = -1;//根据这个为-1还是其他值判断是否专场招聘
                    if (dtTempArticle.Rows[0]["PlaceSecondID"].ToString() != "")
                    {
                        //如果以前PlaceSecondID字段不为空，则默认为这个值
                        PlaceSecondID = Int32.Parse(dtTempArticle.Rows[0]["PlaceSecondID"].ToString());
                    }
                    if (dtTempArticle.Rows[0]["TypeID"].ToString().Trim() == "1")//是专场招聘
                    {
                        //先判空，专场招聘必须填写招聘时间和地点
                        //插入PlaceListSecond表并且得到刚刚插入的ID
                        PlaceSecondID = PlaceListSecond.PlaceListSecondInsert(auditArticle.placeListSecond.PlaceFirstID, auditArticle.placeListSecond.PlaceName, auditArticle.placeListSecond.PlaceTime);
                    }
                    Article article = new Article();
                    article.TypeID = Int32.Parse(dtTempArticle.Rows[0]["TypeID"].ToString());
                    article.Title = dtTempArticle.Rows[0]["Title"].ToString();
                    article.Introduction = dtTempArticle.Rows[0]["Introduction"].ToString();
                    article.PlaceSecondID = PlaceSecondID;
                    article.EditTime = DateTime.Parse(dtTempArticle.Rows[0]["EditTime"].ToString());
                    article.EditorAccount = dtTempArticle.Rows[0]["EditorAccount"].ToString();
                    article.ContactInfo = dtTempArticle.Rows[0]["ContactInfo"].ToString();
                    article.ClickTimes = 0;
                    article.FileAddr = dtTempArticle.Rows[0]["FileAddr"].ToString(); ;
                    //把TempArticle表内容插入到Article表并且得到刚刚插入的ArticleID
                    int ArticleID = Article.ArticleInsert(article);
                    //判断是不是大型招聘会
                    if (Int32.Parse(dtTempArticle.Rows[0]["TypeID"].ToString()) == 2 || Int32.Parse(dtTempArticle.Rows[0]["TypeID"].ToString())==3)
                    {
                        Article.InsertBigArticle(ArticleID, Convert.ToInt32(dtTempArticle.Rows[0]["BigArticleID"].ToString()));
                    }

                    //把对应TempDemandInfo记录插入到DemandInfo
                    DataTable dtTempDemandInfo = TempDemandInfo.GetTempDemandInfoByTempArticleID(tempArticleID);
                    for (int i = 0; i < dtTempDemandInfo.Rows.Count; i++)
                    {
                        DemandInfo demandInfo = new DemandInfo();
                        demandInfo.ArticleID = ArticleID;
                        demandInfo.PositionName = dtTempDemandInfo.Rows[i]["PositionName"].ToString();
                        demandInfo.EducationalLevel = dtTempDemandInfo.Rows[i]["EducationalLevel"].ToString();
                        demandInfo.Major = dtTempDemandInfo.Rows[i]["Major"].ToString();
                        demandInfo.DemandNum = Int32.Parse(dtTempDemandInfo.Rows[i]["DemandNum"].ToString());
                        demandInfo.PositionDec = dtTempDemandInfo.Rows[i]["PositionDec"].ToString();
                        DemandInfo.Insert(demandInfo);
                    }
                }
                else    //二次审核，TempArticle的ArticleID不为空
                {
                    //先得到TempArticle记录的ArticleID
                    int ArticleID = Int32.Parse(dtTempArticle.Rows[0]["ArticleID"].ToString());
                    if (dtTempArticle.Rows[0]["TypeID"].ToString().Trim() == "1")  //是专场招聘
                    {
                        //先判空，专场招聘必须填写招聘时间和地点
                        //先根据TempArticle表正在审核的ArticelID找到Article表记录再找PlaceSecondID对应PlaceSecondList的记录然后update 
                        PlaceListSecond placeListSecond = new PlaceListSecond();
                        placeListSecond.PlaceFirstID = auditArticle.placeListSecond.PlaceFirstID;
                        placeListSecond.PlaceName = auditArticle.placeListSecond.PlaceName;
                        placeListSecond.PlaceTime = auditArticle.placeListSecond.PlaceTime;
                        if (!PlaceListSecond.PlaceListSecondUpdateByArticleIDFromTempArticle(ArticleID, placeListSecond))
                        {
                            return false;
                        }
                    }
                    //判断是不是大型招聘会
                    if (Int32.Parse(dtTempArticle.Rows[0]["TypeID"].ToString()) == 2 || Int32.Parse(dtTempArticle.Rows[0]["TypeID"].ToString()) == 3)
                    {
                        Article.UpdateBigArticle(ArticleID, Convert.ToInt32(dtTempArticle.Rows[0]["BigArticleID"].ToString()));
                    }

                    //把对应TempArticle表ArticleID的Article表对应记录update
                    Article article = new Article();
                    article.ArticleID = ArticleID;
                    article.Title = dtTempArticle.Rows[0]["Title"].ToString();
                    article.Introduction = dtTempArticle.Rows[0]["Introduction"].ToString();
                    article.ContactInfo = dtTempArticle.Rows[0]["ContactInfo"].ToString();
                    article.FileAddr = dtTempArticle.Rows[0]["FileAddr"].ToString();
                    if (!Article.Update(article))
                    {
                        return false;
                    }
                    //先删除原来的DemandInfo
                    SqlConnection cn = DBLink.GetConnection();
                    string sqlstr = "delete from DemandInfo where ArticleID=" + ArticleID;
                    SqlCommand cmm = new SqlCommand(sqlstr, cn);
                    try
                    {
                        cn.Open();
                        cmm.ExecuteNonQuery();
                        cmm.Dispose();
                        cn.Close();
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                    //把对应TempDemandInfo记录插入到DemandInfo
                    DataTable dtTempDemandInfo = TempDemandInfo.GetTempDemandInfoByTempArticleID(tempArticleID);
                    for (int i = 0; i < dtTempDemandInfo.Rows.Count; i++)
                    {
                        DemandInfo demandInfo = new DemandInfo();
                        demandInfo.ArticleID = ArticleID;
                        demandInfo.PositionName = dtTempDemandInfo.Rows[i]["PositionName"].ToString();
                        demandInfo.EducationalLevel = dtTempDemandInfo.Rows[i]["EducationalLevel"].ToString();
                        demandInfo.Major = dtTempDemandInfo.Rows[i]["Major"].ToString();
                        demandInfo.DemandNum = Int32.Parse(dtTempDemandInfo.Rows[i]["DemandNum"].ToString());
                        demandInfo.PositionDec = dtTempDemandInfo.Rows[i]["PositionDec"].ToString();
                        DemandInfo.Insert(demandInfo);
                    }
                }
                //最后先删掉TempDemandInfo内容，再删掉TempArticle内容
                SqlConnection conn = DBLink.GetConnection();
                string sqlstr1 = "delete from TempDemandInfo where TempArticleID=" + tempArticleID;
                string sqlstr2 = "delete from TempArticle where TempArticleID=" + tempArticleID;
                SqlCommand cmm1 = new SqlCommand(sqlstr1, conn);
                SqlCommand cmm2 = new SqlCommand(sqlstr2, conn);
                try
                {
                    conn.Open();
                    cmm1.ExecuteNonQuery();
                    cmm2.ExecuteNonQuery();
                    cmm1.Dispose();
                    cmm2.Dispose();
                    conn.Close();
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
            if (isAgree == -1)
            {
                SqlConnection cn = DBLink.GetConnection();
                //审核信息
                string auditInfo = auditArticle.tempArticle.AuditInfo;
                //Audit=0表示拒绝了审核
                string sqlstr = "update TempArticle set IsAudit=0,AuditInfo='" + auditInfo + "' where TempArticleID=" + tempArticleID;
                SqlCommand cmm = new SqlCommand(sqlstr, cn);
                try
                {
                    cn.Open();
                    cmm.ExecuteNonQuery();
                    cmm.Dispose();
                    cn.Close();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
        #endregion

    }     
}