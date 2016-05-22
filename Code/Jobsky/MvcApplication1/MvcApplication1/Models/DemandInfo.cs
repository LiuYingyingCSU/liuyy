using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace MvcApplication1.Models
{
    public class DemandInfo
    {
        public int DemandID { get; set; }
        public int ArticleID { get; set; }
        public string PositionName { get; set; }
        public string EducationalLevel { get; set; }
        public string Major { get; set; }
        public int DemandNum { get; set; }
        public string PositionDec { get; set; }

        public static bool Insert(DemandInfo tempDemandInfo)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "DemandInfoInsert";
            cmd.Parameters.Add(new SqlParameter("@ArticleID", tempDemandInfo.ArticleID));
            cmd.Parameters.Add(new SqlParameter("@PositionName", tempDemandInfo.PositionName));
            cmd.Parameters.Add(new SqlParameter("@EducationalLevel", tempDemandInfo.EducationalLevel));
            cmd.Parameters.Add(new SqlParameter("@Major", tempDemandInfo.Major));
            cmd.Parameters.Add(new SqlParameter("@DemandNum", tempDemandInfo.DemandNum));
            cmd.Parameters.Add(new SqlParameter("@PositionDec", tempDemandInfo.PositionDec));
            try { cmd.ExecuteNonQuery(); }
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

        public static bool Delete(int ArticleID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("delete from DemandInfo where ArticleID = @ArticleID", conn);
            cmd.Parameters.Add(new SqlParameter("@ArticleID", ArticleID));
            try { cmd.ExecuteNonQuery(); }
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
        public static DataTable GetDemandInfoByArticleID(int ArticleID)
        {
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
    }
}