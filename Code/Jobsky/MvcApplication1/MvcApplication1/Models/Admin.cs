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
    [Table("Admin")]
    public class Admin
    {
        public int AdminID { get; set; }

        [Required(ErrorMessage = "*账号不能为空")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "长度必须为4-20个字符")]
        [RegularExpression("^([0-9A-Za-z_]+)$", ErrorMessage = "只能输入字母，数字，下划线")]
        public string AdminAccount { get; set; }

        [Required(ErrorMessage = "*用户名不能为空")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "长度必须为1-20个字符")]
        public string AdminName { get; set; }

        [Required(ErrorMessage = "*密码不能为空")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "长度必须为6-50个字符")]
        [RegularExpression("^([0-9A-Za-z_]+)$", ErrorMessage = "只能输入字母，数字，下划线")]
        public string AdminPwd { get; set; }

        [Required(ErrorMessage = "*管理员类型不能为空")]
        public int AdminType { get; set; }

        #region 验证账号密码是否正确，返回一个DataRow
        public static DataRow Authentication(string account, string password)
        {
            SqlConnection conn = DBLink.GetConnection();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "AdminAuthentication";
            cmd.Parameters.Add(new SqlParameter("@AdminAccount", account));
            cmd.Parameters.Add(new SqlParameter("@AdminPwd", password));
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
                cmd.Dispose();
                conn.Close();
                return dr;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }
        #endregion  

        #region 添加用户
        public static int Insert(Admin admin)
        {
            //安全性检查，如果time格式不对，函数返回false
            //try {  }
            //catch () { return false; }

            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "AdminInsert";
            cmd.Parameters.Add(new SqlParameter("@AdminAccount", admin.AdminAccount));
            cmd.Parameters.Add(new SqlParameter("@AdminName", admin.AdminName));
            cmd.Parameters.Add(new SqlParameter("@AdminPwd", admin.AdminPwd));
            cmd.Parameters.Add(new SqlParameter("@AdminType", admin.AdminType));
            try { cmd.ExecuteNonQuery(); }
            catch
            {
                return -1; //如果出错，返回-1
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
            return 1;//如果没有出错，返回1
        }
        #endregion

        #region 根据ID得到Admin的信息
        public static DataTable Select(int AdminID)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Admin where AdminID = @AdminID", conn);
            cmd.Parameters.Add(new SqlParameter("@AdminID", AdminID));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion

        #region 根据ID更新Admin信息
        public static bool Update(Admin admin)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "AdminUpdate";
            cmd.Parameters.Add(new SqlParameter("@AdminID", admin.AdminID));
            cmd.Parameters.Add(new SqlParameter("@AdminName", admin.AdminName));
            cmd.Parameters.Add(new SqlParameter("@AdminPwd", admin.AdminPwd));
            cmd.Parameters.Add(new SqlParameter("@AdminType", admin.AdminType));
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
        #endregion

        #region 获取管理员数量
        public static int GetAdminRecordCount()
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            string sqlstr = "";
            sqlstr = "SELECT Count(*) from Admin";
            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            int totalpagecount = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            conn.Close();
            return totalpagecount;
        }
        #endregion

        #region 获取Admin列表
        public static DataTable GetAdminList(int pageindex, int pagesize)
        {
            //安全性检查，如果time格式不对，函数返回false
            //try {  }
            //catch () { return false; }
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetAdminPagedRecord";
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

        #region 根据AdminID删除Admin的信息
        public static bool Delete(int AdminID)
        {
            SqlConnection conn = DBLink.GetConnection();
            SqlCommand cmd = new SqlCommand("Delete FROM Admin where AdminID = @AdminID", conn);
            cmd.Parameters.Add(new SqlParameter("@AdminID", AdminID));
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

    }

    /// <summary>
    /// 管理员注册模型
    /// </summary>
    [NotMapped]
    public class AdminRegister : Admin
    {
        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "密码", Description = "6-50个字符。")]
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "密码至少为6个字符")]
        [RegularExpression("^([0-9A-Za-z_]+)$", ErrorMessage = "只能输入字母，数字，下划线")]
        [DataType(DataType.Password)]
        public new string AdminPwd { get; set; }
        /// <summary>
        /// 确认密码
        /// </summary>
        [Display(Name = "确认密码", Description = "再次输入密码。")]
        [Compare("AdminPwd", ErrorMessage = "两次输入密码不一致")]
        [DataType(DataType.Password)]
        public string ConfirmAdminPwd { get; set; }
        public Admin GetAdmin()
        {
            return new Admin
            {
                AdminID = this.AdminID,

                AdminAccount = this.AdminAccount,
                AdminName = this.AdminName,
                AdminPwd = this.AdminPwd,
                AdminType = this.AdminType
            };
        }

        #region 把DataTable转换成AdminRegister
        public AdminRegister ConvertDtToAdminRegister(DataTable dt)
        {
            AdminRegister adminRegister = new AdminRegister();
            if (dt != null && dt.Rows.Count > 0)
            {
                adminRegister.AdminID = Int32.Parse(dt.Rows[0]["AdminID"].ToString());
                adminRegister.AdminAccount = dt.Rows[0]["AdminAccount"].ToString();
                adminRegister.AdminName = dt.Rows[0]["AdminName"].ToString();
                adminRegister.AdminPwd = dt.Rows[0]["AdminPwd"].ToString();//加密的
                adminRegister.ConfirmAdminPwd = dt.Rows[0]["AdminPwd"].ToString();//加密的
                adminRegister.AdminType = Int32.Parse(dt.Rows[0]["AdminType"].ToString());
            }
            return adminRegister;
        }
        #endregion
    }

    /// <summary>
    /// 雇主登陆模型
    /// </summary>
    [NotMapped]
    public class AdminLogin
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "账号", Description = "4-50个字符。")]
        [Required(ErrorMessage = "×")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "×")]
        public string AdminAccount { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "密码", Description = "6-50个字符。")]
        [Required(ErrorMessage = "×")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "×")]
        [DataType(DataType.Password)]
        public string AdminPwd { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        [Display(Name = "验证码", Description = "请输入图片中的验证码。")]
        [Required(ErrorMessage = "×")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "×")]
        public string VerificationCode { get; set; }
    }
     ///<summary>
     ///雇主搜索模型
     ///</summary>
    //[NotMapped]
    //public class EmployerSearch
    //{
    //    public string CompanyName { get; set; }
    //    public string CompanyNature{get;set;}
    //    public string CompanyBusiness { get; set; }
    //    public bool isSelected { get; set; }
    //}
}