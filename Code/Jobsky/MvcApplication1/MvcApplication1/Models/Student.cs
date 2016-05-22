using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Models
{
    public class Student
    {
        //用户名
        [Display(Name="用户名",Description="请输入10位学号。")]
        [Required(ErrorMessage="*用户名不能为空")]
        [StringLength(20,MinimumLength=8,ErrorMessage="用户名为学号")]
        public string StudentID { get; set; }

        //密码
        [Display(Name="密码",Description="初始密码为学号。")]
        [Required(ErrorMessage="*密码不能为空")]
        [StringLength(50,MinimumLength=6,ErrorMessage="密码长度为6-50个字符")]
        [RegularExpression("^([0-9A-Za-z_]+)$",ErrorMessage="密码只能包含数字、字母、下划线")]
        public string StudentPwd { get; set; }
        
        //姓名
        public string StudentName { get; set; }
        
        public static DataRow Authenciation(string ID,string pwd)
        {
            SqlConnection conn = DBLink.GetConnection();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "StudentDetails";
            cmd.Parameters.Add(new SqlParameter("@StudentID", ID));
            cmd.Parameters.Add(new SqlParameter("@StudentPwd", pwd));
            try
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                DataRow dr = null;
                if (dt != null)
                {
                    dr = dt.Rows[0];
                }
                return dr;
            }
            catch (Exception)
            {
                return null;
            }
            finally {
                cmd.Dispose();
                conn.Close();
            }
        }
    }

    [NotMapped]
    public class StudentLogin{

        [Display(Name="用户名",Description="用户名为学号。")]
        [Required(ErrorMessage = "×")]
        public string StudentID { get; set; }

        [Display(Name="密码",Description="初始密码为学号")]
        [Required(ErrorMessage = "×")]
        [StringLength(50,MinimumLength=6,ErrorMessage="密码长度在6-50位之间")]
        [DataType(DataType.Password)]
        public string StudentPwd { get; set; }

        [Display(Name="验证码",Description="请输入图片显示的字符")]
        [Required(ErrorMessage = "×")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "×")]
        public string VerificationCode { get; set; }
    }
}