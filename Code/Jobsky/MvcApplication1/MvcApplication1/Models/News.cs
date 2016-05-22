using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcApplication1.Models
{
    //TypeID	TypeName
    //1	本部招聘
    //2	湘雅招聘
    //3	铁道信息
    //4	同城招聘
    //5	在线招聘
    //6	图片新闻
    //7	新闻动态
    //8	通知公告
    //9	基层项目

    [Table("News")]
    public class News
    {
        public int NewsID { get; set; }

        [Required(ErrorMessage = "*请选择一个文章类型")]
        public int TypeID { get; set; }

        [Required(ErrorMessage = "*文章标题不能为空")]
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime NewsTime { get; set; }
        public string Editor { get; set; }
        public int ClickTimes { get; set; }
        public string FileAddr { get; set; }

        public static int Insert(News news)
        {           
            int ID = 0;//存储执行insert存储过程返回的ID

            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "NewsInsert";
            cmd.Parameters.Add(new SqlParameter("@TypeID", news.TypeID));
            cmd.Parameters.Add(new SqlParameter("@Title", news.Title));            
            cmd.Parameters.Add(new SqlParameter("@Content", news.Content));
            cmd.Parameters.Add(new SqlParameter("@NewsTime", news.NewsTime));
            cmd.Parameters.Add(new SqlParameter("@Editor", news.Editor));
            cmd.Parameters.Add(new SqlParameter("@ClickTimes", 0));
            cmd.Parameters.Add(new SqlParameter("@FileAddr", news.FileAddr));
            try { ID = Convert.ToInt32(cmd.ExecuteScalar()); }
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

        public static bool Update(News news)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "NewsUpdate";
            cmd.Parameters.Add(new SqlParameter("@NewsID", news.NewsID));
            cmd.Parameters.Add(new SqlParameter("@TypeID", news.TypeID));
            cmd.Parameters.Add(new SqlParameter("@Title", news.Title));
            cmd.Parameters.Add(new SqlParameter("@Content", news.Content));
            cmd.Parameters.Add(new SqlParameter("@NewsTime", news.NewsTime));
            cmd.Parameters.Add(new SqlParameter("@Editor", news.Editor));
            cmd.Parameters.Add(new SqlParameter("@FileAddr", news.FileAddr));
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {
                return false; //如果出错，返回false
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
            return true;//如果没有出错，返回true
        }

        public static DataTable Select(int NewsID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM News where News.NewsID = @NewsID", conn);
            cmd.Parameters.Add(new SqlParameter("@NewsID", NewsID));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }

        #region 获取文章类型名称
        public static string GetTypeNameByTypeID(int typeid)
        {
            string typename = "";
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select TypeName from [NewsType] where TypeID = @typeid", conn);
            cmd.Parameters.Add(new SqlParameter("@typeid", typeid));
            typename = cmd.ExecuteScalar().ToString();
            cmd.Dispose();
            conn.Close();
            return typename;
        }
        #endregion

        #region 根据ID删除News的信息
        public static bool Delete(int NewsID)
        {
            SqlConnection conn = DBLink.GetConnection();
            SqlCommand cmd = new SqlCommand("Delete FROM News where News.NewsID = @NewsID", conn);
            cmd.Parameters.Add(new SqlParameter("@NewsID", NewsID));
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {

            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
            return false;
        }
        #endregion

        #region 根据文章ID和数据库ID删除指定数据库的文章信息
        public static bool DeleteByDatabase(int ID, int databaseID)
        {
            SqlConnection conn;
            SqlCommand cmd;
            if (databaseID == 0)
            {
                //DatabaseID=0：老数据库Article
                conn = DBLink.GetJobsky6Connection();
                cmd = new SqlCommand("Delete FROM Article where ArticleID = @ArticleID", conn);
                cmd.Parameters.Add(new SqlParameter("@ArticleID", ID));
            }
            else if (databaseID == 1)
            {
                //1：新数据库Article
                conn = DBLink.GetConnection();
                cmd = new SqlCommand("Delete FROM Article where ArticleID = @ArticleID", conn);
                cmd.Parameters.Add(new SqlParameter("@ArticleID", ID));
            }
            else
            {
                //2：新数据库News
                conn = DBLink.GetConnection();
                cmd = new SqlCommand("Delete FROM News where NewsID = @NewsID", conn);
                cmd.Parameters.Add(new SqlParameter("@NewsID", ID));
            }
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {

            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
            return false;
        }
        #endregion

        #region 把DataTable的信息转换成News类型
        public News ConvertDtToNews(DataTable dt)
        {
            News news = new News();
            if (dt != null && dt.Rows.Count > 0)
            {
                news.NewsID = Int32.Parse(dt.Rows[0]["NewsID"].ToString());
                news.TypeID = Int32.Parse(dt.Rows[0]["TypeID"].ToString());
                news.Title = dt.Rows[0]["Title"].ToString();
                news.Content = dt.Rows[0]["Content"].ToString();
                news.NewsTime = DateTime.Parse(dt.Rows[0]["NewsTime"].ToString());
                news.Editor = dt.Rows[0]["Editor"].ToString(); ;
                news.ClickTimes = Int32.Parse(dt.Rows[0]["ClickTimes"].ToString()); ;
                news.FileAddr = dt.Rows[0]["FileAddr"].ToString();
            }
            return news;
        }
        #endregion

        public static int PictureNewsInsert(int NewsID,string ImgAddr,int Rank)
        {
            int ID = 0;//存储执行insert存储过程返回的ID

            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("Insert into PictureNews(NewsID,ImgAddr,Rank) values (@NewsID,@ImgAddr,@Rank); SELECT @@IDENTITY", conn);
            cmd.Parameters.Add(new SqlParameter("@NewsID", NewsID));
            cmd.Parameters.Add(new SqlParameter("@ImgAddr", ImgAddr));
            cmd.Parameters.Add(new SqlParameter("@Rank", Rank));
            try { ID = Convert.ToInt32(cmd.ExecuteScalar()); }
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
        public static bool PictureNewsUpdate(int NewsID, string ImgAddr, int Rank)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("update PictureNews set ImgAddr=@ImgAddr, Rank=@Rank where NewsID=@NewsID", conn);
            cmd.Parameters.Add(new SqlParameter("@NewsID", NewsID));
            cmd.Parameters.Add(new SqlParameter("@ImgAddr", ImgAddr));
            cmd.Parameters.Add(new SqlParameter("@Rank", Rank));
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {
                return false; //如果出错，返回false
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
            return true;//如果没有出错，返回true
        }

        public static DataTable PictureNewsSelect(int NewsID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM PictureNews where NewsID = @NewsID", conn);
            cmd.Parameters.Add(new SqlParameter("@NewsID", NewsID));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }

        public static DataTable GetPictureNewsByRank(int Rank)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM PictureNews where Rank = @Rank", conn);
            cmd.Parameters.Add(new SqlParameter("@Rank", Rank));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }

        /// <summary>
        /// 选出序列化轮播图：选出Rank=（1，2，3，4，5）的News的标题和图片地址并排序（注：数量小于或等于五条）
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static DataTable PictureNewsSelectRank()
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT News.NewsID,News.Title,PictureNews.ImgAddr,PictureNews.Rank FROM News,PictureNews where News.NewsID = PictureNews.NewsID and Rank in (1,2,3,4,5) order by Rank asc", conn);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        /// <summary>
        /// 顶置：先对Rank=@Rank的数据条对应的Rank=0, 再把NewsID=@NewsID的Rank值改为@Rank（1至5内）
        /// </summary>
        /// <param name="NewsID"></param>
        /// <param name="Rank"></param>
        /// <returns></returns>
        public static bool PictureNewsUpRank(int NewsID,int Rank)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("update PictureNews set Rank='0' where Rank=@Rank;  update PictureNews set Rank=@Rank where NewsID=@NewsID", conn);
            cmd.Parameters.Add(new SqlParameter("@NewsID", NewsID));
            cmd.Parameters.Add(new SqlParameter("@Rank", Rank));
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {
                return false; //如果出错，返回false
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
            return true;//如果没有出错，返回true
        }

        #region 获取最近信息
        /// <summary>
        /// 获取最近信息，使用存储过程，根据属性RecordCount设置信息条数，TypeID确定文章类型
        /// </summary>
        /// <returns></returns>
        public static DataTable GetRecentRecord(int RecordCount, int TypeID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetRecentRecordFromView_ALL";
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

        #region 根据类型获取View_ALL数量
        public static int GetNewsRecordCountByType(int typeid)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            string sqlstr = "";
            //-1则返回所有文章数量
            if (typeid == -1)
            {
                sqlstr = "SELECT Count(*) from View_ALL";
            }
            else
            {
                sqlstr = "SELECT Count(*) from View_ALL where TypeID=@TypeID";
            }

            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            cmd.Parameters.Add(new SqlParameter("@TypeID", typeid));

            int totalpagecount = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            conn.Close();
            return totalpagecount;
        }
        #endregion

        #region 根据关键字获取View_ALL数量
        public static int GetNewsRecordCountByKey(string key, string adminType = "0")
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            string sqlstr = "";
            if (adminType == "1")
            {
                //1为湘雅
                sqlstr = "SELECT Count(*) from View_ALL where Title like '%" + key + "%' and TypeID=2";
            }
            else if (adminType == "2")
            {
                //2为铁道
                sqlstr = "SELECT Count(*) from View_ALL where Title like '%" + key + "%' and TypeID=3";
            }
            else
            {
                //0为本部管理员，默认为0用来显示所有
                sqlstr = "SELECT Count(*) from View_ALL where Title like '%" + key + "%'";
            } 
            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            int totalpagecount = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            conn.Close();
            return totalpagecount;
        }
        #endregion

        #region 根据“类型”获取View_ALL列表
        public static DataTable GetListByTypeID(int pageindex, int pagesize, int typeid)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetPagedRecordFromView_ALL";
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
        #endregion

        #region 根据“类型和日期”获取View_ALL数量
        public static int GetNewsRecordCountByTypeIDAndDateTime(int typeid, DateTime begindate, DateTime enddate)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            string sqlstr = "";
            //-1则返回所有文章数量
            if (typeid == -1)
            {
                sqlstr = "SELECT Count(*) from View_ALL where convert(varchar(10),[NewsTime],120) between @begindate and @enddate";
            }
            else
            {
                sqlstr = "SELECT Count(*) from View_ALL where TypeID=@TypeID and convert(varchar(10),[NewsTime],120) between @begindate and @enddate";
            }

            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            cmd.Parameters.Add(new SqlParameter("@TypeID", typeid));
            cmd.Parameters.Add(new SqlParameter("@begindate", begindate));
            cmd.Parameters.Add(new SqlParameter("@enddate", enddate));
            int totalpagecount = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            conn.Close();
            return totalpagecount;
        }
        #endregion
        #region 根据“类型和日期”获取View_ALL列表
        public static DataTable GetListByTypeIDAndDateTime(int pageindex, int pagesize, int typeid, DateTime begindate, DateTime enddate)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetPagedRecordByDateTimeFromView_ALL";
            cmd.Parameters.Add(new SqlParameter("@TypeID", typeid));
            cmd.Parameters.Add(new SqlParameter("@pagesize", pagesize));
            cmd.Parameters.Add(new SqlParameter("@pageindex", pageindex));
            cmd.Parameters.Add(new SqlParameter("@begindate", begindate));
            cmd.Parameters.Add(new SqlParameter("@enddate", enddate));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion

        #region 根据关键字获取View_ALL列表
        public static DataTable GetListByKey(int pageindex, int pagesize, string key,string adminType="0")
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            if (adminType == "1" || adminType == "2")
            {
                //1为湘雅，对应文章ID为2；2为铁道，对应文章TypeID为3
                int typeID = 2;
                if (adminType == "2") typeID = 3;
                cmd.CommandText = "GetPagedRecordFromView_ALLByKeyByAdminType";
                cmd.Parameters.Add(new SqlParameter("@typeID", typeID));
            }
            else
            {
                cmd.CommandText = "GetPagedRecordFromView_ALLByKey";
            }           
            cmd.Parameters.Add(new SqlParameter("@Key", key));
            cmd.Parameters.Add(new SqlParameter("@pagesize", pagesize));
            cmd.Parameters.Add(new SqlParameter("@pageindex", pageindex));

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion

        #region 根据Date日期获取“某一天”的View_ALL列表（为招聘日历服务）
        public static DataTable GetListByDate(DateTime date)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetRecordByDate";
            cmd.Parameters.Add(new SqlParameter("@date", date));

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion

        #region 根据Date日期获取“某个月”View_ALL列表（为招聘日历服务），date只需要是那个月的随便一天就可以
        public static DataTable GetListAllMouth(DateTime date)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetRecordAllMouth";
            cmd.Parameters.Add(new SqlParameter("@date", date));

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion

        #region 新数据库News浏览次数加一
        public static void NewsClickTimesPlus(int NewsID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("update [News] set [ClickTimes]=([ClickTimes]+1) where [NewsID]=@NewsID",conn);
            cmd.Parameters.Add(new SqlParameter("@NewsID", NewsID));
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            conn.Close();
        }
        #endregion

        #region 老数据库的查询
        public static DataTable SelectFromOldArticle(int ArticleID)
        {
            SqlConnection conn = DBLink.GetJobsky6Connection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Article where Article.ArticleID = @ArticleID", conn);
            cmd.Parameters.Add(new SqlParameter("@ArticleID", ArticleID));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion

        #region 老数据库浏览次数加一
        public static void OldArticleClickTimesPlus(int ArticleID)
        {
            SqlConnection conn = DBLink.GetJobsky6Connection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ClickTimesPlus";
            cmd.Parameters.Add(new SqlParameter("@ArticleID", ArticleID));
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            conn.Close();
        }
        #endregion

        #region 获取院系风采列表
        public static DataTable GetCollegeList()
        {
            SqlConnection conn = DBLink.GetJobsky6Connection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select ArticleID,Title,PicSource from [CollegeInfo];", conn);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion
        #region 获取院系风采文章
        public static DataTable GetCollege(int ArticleID)
        {
            SqlConnection conn = DBLink.GetJobsky6Connection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select * from [CollegeInfo] where ArticleID=@id", conn);
            cmd.Parameters.Add(new SqlParameter("@id", ArticleID));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion

        #region 字符截取,字母是一个字节，中文是两个字符
        /// <summary>
        /// 字符串截取函数
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="length">最大长度，以字节为单位</param>
        /// <returns>截取后的字符</returns>
        public static string LengthSub(string str, int length)
        {
            string temp = string.Empty;
            if (System.Text.Encoding.Default.GetByteCount(str) <= length)//如果长度比需要的长度n小,返回原字符串
            {
                return str;
            }
            else
            {
                int t = 0;
                char[] q = str.ToCharArray();
                for (int i = 0; i < q.Length; i++)
                {
                    if ((int)q[i] >= 0x4E00 && (int)q[i] <= 0x9FA5)//是否汉字
                    {
                        temp += q[i];
                        t += 2;
                    }
                    else
                    {
                        temp += q[i];
                        t += 1;
                    }
                    if (t >= length)
                    {
                        break;
                    }
                }
                return (temp + "...");
            }
        }
        #endregion

    }
}