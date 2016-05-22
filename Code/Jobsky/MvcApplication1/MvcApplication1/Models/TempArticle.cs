using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace MvcApplication1.Models
{
    public class TempArticle
    {
        public int TempArticleID { get; set; }
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
        public string ArticleDescription { get; set; }
        public string AuditInfo { get; set; }
        public int BigArticleID { get; set; }
        public int IsAudit { get; set; }
        public int PlaceFirstID { get; set; }
        public string RecruitPlace { get; set; }
        public DateTime RecruitTime { get; set; }
        /// <summary>
        /// 供给创建文章使用
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        public static int InsertTempArticle(TempArticle article)
        {
            int ID = 0;//存储执行insert存储过程返回的ID

            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TempArticleInsert";
            //cmd.Parameters.Add(new SqlParameter("@ArticleID", article.ArticleID));  //注释的部分都是存储过程中默认值是null的
            cmd.Parameters.Add(new SqlParameter("@TypeID", article.TypeID));
            cmd.Parameters.Add(new SqlParameter("@Title", article.Title));
            cmd.Parameters.Add(new SqlParameter("@Introduction", article.Introduction));
            //cmd.Parameters.Add(new SqlParameter("@PlaceSecondID", article.PlaceSecondID));
            cmd.Parameters.Add(new SqlParameter("@EditTime", article.EditTime));
            cmd.Parameters.Add(new SqlParameter("@EditorAccount", article.EditorAccount));
            cmd.Parameters.Add(new SqlParameter("@ContactInfo", article.ContactInfo));
            if (article.TypeID == 1) {//专场招聘
                cmd.Parameters.Add(new SqlParameter("@PlaceFirstID", article.PlaceFirstID));
                cmd.Parameters.Add(new SqlParameter("@RecruitTime", article.RecruitTime));
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter("@PlaceFirstID", DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@RecruitTime", DBNull.Value));
            }

            if (article.FileAddr == null || article.FileAddr == "")
            {
                cmd.Parameters.Add(new SqlParameter("@FileAddr", DBNull.Value));
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter("@FileAddr", article.FileAddr));
            }
            cmd.Parameters.Add(new SqlParameter("@ArticleDescription", article.ArticleDescription));
            cmd.Parameters.Add(new SqlParameter("@BigArticleID", article.BigArticleID));
            cmd.Parameters.Add(new SqlParameter("@IsAudit", article.IsAudit));
            
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
        /// <summary>
        /// 供给已审核通过的文章插入使用
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        public static int InsertTempArticleFromArticle(TempArticle article)
        {
            int ID = 0;//存储执行insert存储过程返回的ID

            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TempArticleInsert";
            cmd.Parameters.Add(new SqlParameter("@ArticleID", article.ArticleID));  //注释的部分都是存储过程中默认值是null的
            cmd.Parameters.Add(new SqlParameter("@TypeID", article.TypeID));
            cmd.Parameters.Add(new SqlParameter("@Title", article.Title));
            cmd.Parameters.Add(new SqlParameter("@Introduction", article.Introduction));
            //cmd.Parameters.Add(new SqlParameter("@PlaceSecondID", article.PlaceSecondID));
            cmd.Parameters.Add(new SqlParameter("@EditTime", article.EditTime));
            cmd.Parameters.Add(new SqlParameter("@EditorAccount", article.EditorAccount));
            cmd.Parameters.Add(new SqlParameter("@ContactInfo", article.ContactInfo));
            if (article.TypeID == 1)
            {//专场招聘
                cmd.Parameters.Add(new SqlParameter("@PlaceFirstID", article.PlaceFirstID));
                cmd.Parameters.Add(new SqlParameter("@RecruitTime", article.RecruitTime));
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter("@PlaceFirstID", DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@RecruitTime", DBNull.Value));
            }
            if (article.FileAddr == null || article.FileAddr == "")
            {
                cmd.Parameters.Add(new SqlParameter("@FileAddr", DBNull.Value));
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter("@FileAddr", article.FileAddr));
            }
            cmd.Parameters.Add(new SqlParameter("@ArticleDescription", article.ArticleDescription));
            cmd.Parameters.Add(new SqlParameter("@BigArticleID", article.BigArticleID));
            cmd.Parameters.Add(new SqlParameter("@IsAudit", article.IsAudit));

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

        public static DataTable GetArticleListFromTempArticle(string editoraccount, int pageindex, int pagesize, int typeid)
        {
            //安全性检查，如果time格式不对，函数返回false
            //try {  }
            //catch () { return false; }
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetTempArticlePagedRecord";

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

        public static int GetTempArticleRecordCount(string editoraccount, int typeid)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            string sqlstr = "";
            if (typeid == 0)
            {
                sqlstr = "SELECT Count(*) from TempArticle where EditorAccount=@EditorAccount";
            }
            else
            {
                sqlstr = "SELECT Count(*) from TempArticle where EditorAccount=@EditorAccount and TypeID=@TypeID";
            }
            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            cmd.Parameters.Add(new SqlParameter("@EditorAccount", editoraccount));
            cmd.Parameters.Add(new SqlParameter("@TypeID", typeid));

            int totalpagecount = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            conn.Close();
            return totalpagecount;
        }

        public static DataTable GetTempArticleByTempArticleID(int TempArticleID, string EditorAccount)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select * from [TempArticle] where TempArticleID = @TempArticleID and EditorAccount = @EditorAccount", conn);
            cmd.Parameters.Add(new SqlParameter("@TempArticleID", TempArticleID));
            cmd.Parameters.Add(new SqlParameter("@EditorAccount", EditorAccount));

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }

        public static DataTable GetTempArticleByArticleID(int ArticleID, string EditorAccount)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select * from [TempArticle] where ArticleID = @ArticleID and EditorAccount = @EditorAccount", conn);
            cmd.Parameters.Add(new SqlParameter("@ArticleID", ArticleID));
            cmd.Parameters.Add(new SqlParameter("@EditorAccount", EditorAccount));

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }

        public static int GetTypeIDByTempArticleID(int TempArticleID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select TypeID from [TempArticle] where TempArticleID = @TempArticleID", conn);
            cmd.Parameters.Add(new SqlParameter("@TempArticleID", TempArticleID));
            int typeid = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            conn.Close();
            return typeid;
        }

        public static bool Update(TempArticle article)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TempArticleUpdate";
            cmd.Parameters.Add(new SqlParameter("@TempArticleID", article.TempArticleID));
            cmd.Parameters.Add(new SqlParameter("@Title", article.Title));
            cmd.Parameters.Add(new SqlParameter("@Introduction", article.Introduction));
            cmd.Parameters.Add(new SqlParameter("@ContactInfo", article.ContactInfo));
            if (article.TypeID == 1)
            {//专场招聘
                cmd.Parameters.Add(new SqlParameter("@PlaceFirstID", article.PlaceFirstID));
                cmd.Parameters.Add(new SqlParameter("@RecruitTime", article.RecruitTime));
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter("@PlaceFirstID", DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@RecruitTime", DBNull.Value));
            }

            if (article.FileAddr == null || article.FileAddr == "")
            {
                cmd.Parameters.Add(new SqlParameter("@FileAddr", DBNull.Value));
            }
            else
            {
                cmd.Parameters.Add(new SqlParameter("@FileAddr", article.FileAddr));
            }
            cmd.Parameters.Add(new SqlParameter("@BigArticleID", article.BigArticleID));
            cmd.Parameters.Add(new SqlParameter("@ArticleDescription", article.ArticleDescription));
            cmd.Parameters.Add(new SqlParameter("@IsAudit", article.IsAudit));
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
        /// <summary>
        /// 删除TempArticle里指定id的文章
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        public static bool Delete(int TempArticleID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("Delete From [TempArticle] where TempArticleID = @TempArticleID", conn);
            cmd.Parameters.Add(new SqlParameter("@TempArticleID", TempArticleID));
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

        #region 根据待审核文章ID获取待审核文章详细信息（包括雇主联系人电话）
        public static DataTable GetAuditArticleByTempArticleID(int ID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetAuditArticleByID";
            cmd.Parameters.Add(new SqlParameter("@TempArticleID", ID));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion

        #region 根据待审核文章ID获取待审核文章详细信息
        public static DataTable GetTempArticleByTempArticleID(int TempArticleID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetTempArticleByTempArticleID";
            cmd.Parameters.Add(new SqlParameter("@TempArticleID", TempArticleID));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion
    }
}