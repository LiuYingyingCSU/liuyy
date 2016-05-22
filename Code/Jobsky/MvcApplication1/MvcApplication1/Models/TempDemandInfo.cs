using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace MvcApplication1.Models
{
    public class TempDemandInfo
    {
        public int TempDemandID { get; set; }
        public int TempArticleID { get; set; }
        public string PositionName { get; set; }
        public string EducationalLevel { get; set; }
        public string Major { get; set; }
        public int DemandNum { get; set; }
        public string PositionDec { get; set; }

        public static bool Insert(TempDemandInfo tempDemandInfo)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "TempDemandInfoInsert";
            cmd.Parameters.Add(new SqlParameter("@TempArticleID", tempDemandInfo.TempArticleID));
            cmd.Parameters.Add(new SqlParameter("@PositionName", tempDemandInfo.PositionName));
            cmd.Parameters.Add(new SqlParameter("@EducationalLevel", tempDemandInfo.EducationalLevel));
            cmd.Parameters.Add(new SqlParameter("@Major", tempDemandInfo.Major));
            cmd.Parameters.Add(new SqlParameter("@DemandNum", tempDemandInfo.DemandNum));
            cmd.Parameters.Add(new SqlParameter("@PositionDec", tempDemandInfo.PositionDec));
            //int ID = 0;
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

        public static bool Delete(int TempArticleID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("delete from TempDemandInfo where TempArticleID = @TempArticleID", conn);
            cmd.Parameters.Add(new SqlParameter("@TempArticleID", TempArticleID));
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

        public static DataTable GetTempDemandInfoByTempArticleID(int TempArticleID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select * from [TempDemandInfo] where TempArticleID = @TempArticleID", conn);
            cmd.Parameters.Add(new SqlParameter("@TempArticleID", TempArticleID));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }

    }
}