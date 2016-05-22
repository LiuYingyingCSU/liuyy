using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    [Table("PlaceListSecond")]
    public class PlaceListSecond
    {
        [Key]
        public int PlaceSecondID { get; set; }

        [Required]
        public int PlaceFirstID { get; set; }

        [StringLength(20, ErrorMessage = "长度必须少于20个字符")]
        public string PlaceName { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime PlaceTime { get; set; }

        #region 更新招聘地点和时间
        public static bool PlaceListSecondUpdate(int PlaceSecondID, int PlaceFirstID, string PlaceName, DateTime PlaceTime)
        {
            //安全性检查
            SqlConnection conn = DBLink.GetConnection();//拿到新数据库的链接 
            conn.Open();
            SqlCommand cmd = new SqlCommand();

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "update PlaceListSecond set PlaceFirstID = @PlaceFirstID, PlaceName = @PlaceName, PlaceTime = @PlaceTime where PlaceSecondID = @PlaceSecondID";
                cmd.Parameters.Add(new SqlParameter("@PlaceSecondID", PlaceSecondID));
                cmd.Parameters.Add(new SqlParameter("@PlaceFirstID", PlaceFirstID));
                cmd.Parameters.Add(new SqlParameter("@PlaceName", PlaceName));
                cmd.Parameters.Add(new SqlParameter("@PlaceTime", PlaceTime));
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
            return true;//如果没有出错，返回true
        }
        #endregion

        #region 插入新的记录，并返回ID
        public static int PlaceListSecondInsert(int PlaceFirstID, string PlaceName, DateTime PlaceTime)
        {
            //安全性检查

            SqlConnection conn = DBLink.GetConnection();//拿到新数据库的链接 
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PlaceListSecondInsert";
            cmd.Parameters.Add(new SqlParameter("@PlaceFirstID", PlaceFirstID));
            cmd.Parameters.Add(new SqlParameter("@PlaceName", PlaceName));
            cmd.Parameters.Add(new SqlParameter("@PlaceTime", PlaceTime));
            int insertid = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            cmd.Dispose();
            conn.Close();
            return insertid;//如果没有出错，返回true
        }
        #endregion

        #region 根据TempArticle表-ArticleID-Article表-PlaceSecondID-PlaceSecondList表-更新
        public static bool PlaceListSecondUpdateByArticleIDFromTempArticle(int ArticleID, PlaceListSecond placeListSecond)
        {
            //安全性检查
            SqlConnection conn = DBLink.GetConnection();//拿到新数据库的链接 
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            string sqlstr = "update PlaceListSecond set PlaceFirstID = @PlaceFirstID, PlaceName = @PlaceName, PlaceTime = @PlaceTime where PlaceSecondID = ";
            sqlstr += "(select Article.PlaceSecondID from Article,TempArticle where TempArticle.ArticleID=Article.ArticleID and TempArticle.ArticleID=@ArticleID)";
            cmd.CommandText = sqlstr;
            cmd.Parameters.Add(new SqlParameter("@ArticleID", ArticleID));
            cmd.Parameters.Add(new SqlParameter("@PlaceFirstID", placeListSecond.PlaceFirstID));
            cmd.Parameters.Add(new SqlParameter("@PlaceName", placeListSecond.PlaceName));
            cmd.Parameters.Add(new SqlParameter("@PlaceTime", placeListSecond.PlaceTime));
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            conn.Close();
            return true;//如果没有出错，返回true
        }
        #endregion

    }
}