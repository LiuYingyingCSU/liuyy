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
    public class ArticleType
    {
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        //构造函数
        public ArticleType()
        {
            ;
        }
        public ArticleType(int typeid, string typename)
        {
            TypeID = typeid;
            TypeName = typename;
        }

        public static List<ArticleType> Select()
        {
            //安全性检查，如果time格式不对，函数返回false
            //try {  }
            //catch () { return false; }
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select * from [ArticleType]", conn);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            List<ArticleType> list = new List<ArticleType>();
            //for(int i=0;i<dt.Rows.Count;++i){
            //    list.Add(new ArticleType(Convert.ToInt32( dt.Rows[i][0]), dt.Rows[i][1].ToString()));
            //}
            foreach (DataRow item in dt.Rows)
            {
                list.Add(new ArticleType(Convert.ToInt32(item["TypeID"]), item["TypeName"].ToString()));
            } 
            return list;
        }

        public static string GetTypeNameByTypeID(int typeid)
        {
            string typename = "";
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select TypeName from [ArticleType] where TypeID = @typeid", conn);
            cmd.Parameters.Add(new SqlParameter("@typeid", typeid));
            typename = cmd.ExecuteScalar().ToString();
            cmd.Dispose();
            conn.Close();
            return typename;
        }

        public static string GetTypeIDFromView_ArticleTypeToNewsType(int ArticleID)
        {
            string typename = "";
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT [TypeID] FROM [View_ArticleTypeToNewsType] WHERE ArticleID = @ArticleID", conn);
            cmd.Parameters.Add(new SqlParameter("@ArticleID", ArticleID));
            typename = cmd.ExecuteScalar().ToString();
            cmd.Dispose();
            conn.Close();
            return typename;
        }
    }
}