using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace MvcApplication1.Models
{
    [Table("Employer")]
    public class Employer
    {
        public int EmployerID { get; set; }

        //账号信息部分
        [Display(Name = "账号", Description = "4-50个字符。")]
        [Required(ErrorMessage = "*账号不能为空")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "长度必须为4-50个字符")]
        [RegularExpression("^([0-9A-Za-z_]+)$", ErrorMessage = "只能输入字母，数字，下划线")]
        [System.Web.Mvc.Remote("CheckLoginAccount", "Employer", ErrorMessage = "登录账号已经被占用,请改用其他账号")]
        public string EmployerAccount { get; set; }

        [Display(Name = "密码", Description = "6-50个字符。")]
        [Required(ErrorMessage = "*密码不能为空")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "密码至少为6个字符")]
        [RegularExpression("^([0-9A-Za-z_]+)$", ErrorMessage = "只能输入字母，数字，下划线")]
        [DataType(DataType.Password)]
        public string EmployerPwd { get; set; }

        //联系人信息部分
        [Display(Name = "联系人姓名", Description = "单位人力资源部负责人姓名")]
        [Required(ErrorMessage = "*联系人姓名不能为空")]
        [MaxLength(20)]
        public string ContactPersonName { get; set; }

        [Display(Name = "联系人性别", Description = "单位人力资源部负责人性别")]
        [Required(ErrorMessage = "*联系人性别不能为空")]
        [MaxLength(20)]
        public Int16 ContactPersonSex { get; set; }

        [Display(Name = "固定电话", Description = "单位人力资源部负责人电话")]
        [MaxLength(20)]
        [RegularExpression("^([0-9-]+)$", ErrorMessage = "只能输入数字和-")]
        public string FixedTelephone { get; set; }

        [Display(Name = "移动电话", Description = "单位人力资源部负责人电话")]
        [Required(ErrorMessage = "*联系人移动电话不能为空")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "电话号码必须为11位")]
        [RegularExpression("^([0-9-]+)$", ErrorMessage = "只能输入数字和-")]
        public string MobilePhone { get; set; }

        [Display(Name = "邮箱", Description = "请填写常用邮箱，邮箱是联系和找回密码的重要凭证")]
        [Required(ErrorMessage = "*邮箱不能为空")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "邮箱格式不正确")]
        [MaxLength(50)]
        [System.Web.Mvc.Remote("CheckEmailUnique", "Employer", ErrorMessage = "邮箱已经被占用,请改用其他邮箱")]
        public string Email { get; set; }

        //单位基本信息部分
        [Display(Name = "单位名称")]
        [Required(ErrorMessage = "*单位名称不能为空")]
        [MaxLength(30)]
        public string CompanyName { get; set; }

        [Display(Name = "总公司名称")]
        [MaxLength(30)]
        public string ParentCompanyName { get; set; }

        [Display(Name = "单位简介")]
        [DataType(DataType.MultilineText)]
        public string CompanyIntroduction { get; set; }

        [Display(Name = "办公电话")]
        [Required(ErrorMessage = "*办公电话不能为空")]
        [RegularExpression("^([0-9-]+)$", ErrorMessage = "只能输入数字和-")]
        public string CompanyPhone { get; set; }

        [Display(Name = "组织机构代码")]
        [Required(ErrorMessage = "*组织机构代码不能为空")]
        [MaxLength(20)]
        public string OrganizationCode { get; set; }

        [Display(Name = "组织机构代码有效期")]
        [Required(ErrorMessage = "*组织机构代码有效期不能为空")]
        [DataType(DataType.DateTime)]
        public DateTime ValidPeriod { get; set; }

        [Display(Name = "单位性质")]
        [Required(ErrorMessage = "*单位性质不能为空")]
        [MaxLength(50)]
        public string CompanyNature { get; set; }

        [Display(Name = "单位行业")]
        [Required(ErrorMessage = "*单位行业不能为空")]
        [MaxLength(50)]
        public string CompanyBusiness { get; set; }

        [Display(Name = "单位规模")]
        [Required(ErrorMessage = "*单位规模不能为空")]
        [MaxLength(20)]
        public string CompanySize { get; set; }

        [Display(Name = "注册资本")]
        [Required(ErrorMessage = "*注册资本不能为空")]
        [RegularExpression("^([0-9.]+)$", ErrorMessage = "只能输入数字和小数点")]
        public decimal RegisteredCapital { get; set; }

        [Display(Name = "是否是五百强")]
        [Required(ErrorMessage = "*必须选择是否为五百强")]
        public int IsTop500 { get; set; }

        [Display(Name = "所在地区省份")]
        [Required(ErrorMessage = "*单位所在地区省份不能为空")]
        [MaxLength(30)]
        public string CompanyAreaProvince { get; set; }

        [Display(Name = "所在地区市（区）")]
        [Required(ErrorMessage = "*单位所在地区市（区）不能为空")]
        [MaxLength(30)]
        public string CompanyAreaCity { get; set; }

        [Display(Name = "单位地址")]
        [Required(ErrorMessage = "*单位地址不能为空")]
        public string CompanyAddress { get; set; }

        [Display(Name = "城市类型")]
        [Required(ErrorMessage = "*城市类型不能为空")]
        [MaxLength(20)]
        public string CityClass { get; set; }

        [Display(Name = "备注")]
        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }

        //证件照片部分
        [Display(Name = "营业执照或组织结构代码证")]
        [Required(ErrorMessage = "*")]
        public string CredentialsDir { get; set; }

        //其他字段部分
        [Display(Name = "注册时间")]
        [DataType(DataType.DateTime)]
        public DateTime RegisterTime { get; set; }

        [Display(Name = "是否被删除")]
        public int IsDelete { get; set; }

        #region 得到EmployerRegister
        //得到EmployerRegister
      
        public EmployerRegister GetEmployerRegister()
        {
            return new EmployerRegister
            {
                EmployerID = this.EmployerID,
                //账号信息部分
                EmployerAccount = this.EmployerAccount,
                EmployerPwd = this.EmployerPwd,

                //联系人信息部分
                ContactPersonName = this.ContactPersonName,
                ContactPersonSex = this.ContactPersonSex,
                FixedTelephone = this.FixedTelephone,
                MobilePhone = this.MobilePhone,
                Email = this.Email,

                //单位基本信息部分
                CompanyName = this.CompanyName,
                ParentCompanyName = this.ParentCompanyName,
                CompanyIntroduction = this.CompanyIntroduction,
                CompanyPhone = this.CompanyPhone,
                OrganizationCode = this.OrganizationCode,

                ValidPeriod = this.ValidPeriod,
                CompanyNature = this.CompanyNature,
                CompanyBusiness = this.CompanyBusiness,
                CompanySize = this.CompanySize,
                RegisteredCapital = this.RegisteredCapital,

                IsTop500 = this.IsTop500,
                CompanyAreaProvince = this.CompanyAreaProvince,
                CompanyAreaCity = this.CompanyAreaCity,
                CompanyAddress = this.CompanyAddress,
                CityClass = this.CityClass,

                Remark = this.Remark,

                //证件照片部分                
                CredentialsDir = this.CredentialsDir,

                //其他字段                
                RegisterTime = this.RegisterTime,
                IsDelete = this.IsDelete
            };
        }
        #endregion 

        #region 注册信息插入数据库
        public static bool EmployerInsert(Employer employer)
        {
            SqlConnection conn = DBLink.GetConnection();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "EmployerInsert";
            //共25个字段
            //账号信息部分
            cmd.Parameters.Add(new SqlParameter("@EmployerAccount", employer.EmployerAccount));
            cmd.Parameters.Add(new SqlParameter("@EmployerPwd", employer.EmployerPwd));
            //联系人信息
            cmd.Parameters.Add(new SqlParameter("@ContactPersonName", employer.ContactPersonName));
            cmd.Parameters.Add(new SqlParameter("@ContactPersonSex", employer.ContactPersonSex));
            cmd.Parameters.Add(new SqlParameter("@FixedTelephone", employer.FixedTelephone));
            cmd.Parameters.Add(new SqlParameter("@MobilePhone", employer.MobilePhone));
            cmd.Parameters.Add(new SqlParameter("@Email", employer.Email));
            //单位基本信息
            cmd.Parameters.Add(new SqlParameter("@CompanyName", employer.CompanyName));
            cmd.Parameters.Add(new SqlParameter("@ParentCompanyName", employer.ParentCompanyName));
            cmd.Parameters.Add(new SqlParameter("@CompanyIntroduction", employer.CompanyIntroduction));
            cmd.Parameters.Add(new SqlParameter("@CompanyPhone", employer.CompanyPhone));
            cmd.Parameters.Add(new SqlParameter("@OrganizationCode", employer.OrganizationCode));

            cmd.Parameters.Add(new SqlParameter("@ValidPeriod", employer.ValidPeriod));
            cmd.Parameters.Add(new SqlParameter("@CompanyNature", employer.CompanyNature));
            cmd.Parameters.Add(new SqlParameter("@CompanyBusiness", employer.CompanyBusiness));
            cmd.Parameters.Add(new SqlParameter("@CompanySize", employer.CompanySize));
            cmd.Parameters.Add(new SqlParameter("@RegisteredCapital", employer.RegisteredCapital));

            cmd.Parameters.Add(new SqlParameter("@IsTop500", employer.IsTop500));
            cmd.Parameters.Add(new SqlParameter("@CompanyAreaProvince", employer.CompanyAreaProvince));
            cmd.Parameters.Add(new SqlParameter("@CompanyAreaCity", employer.CompanyAreaCity));
            cmd.Parameters.Add(new SqlParameter("@CompanyAddress", employer.CompanyAddress));
            cmd.Parameters.Add(new SqlParameter("@CityClass", employer.CityClass));

            cmd.Parameters.Add(new SqlParameter("@Remark", employer.Remark));
            //证件照片
            cmd.Parameters.Add(new SqlParameter("@CredentialsDir", employer.CredentialsDir));
            //其他字段
            cmd.Parameters.Add(new SqlParameter("@RegisterTime", employer.RegisterTime));
            cmd.Parameters.Add(new SqlParameter("@IsDelete", employer.IsDelete));

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                return true;
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
        }
        #endregion

        #region 验证账号密码是否正确（登录时专用，只有待审核或者审核通过的可以登录）
        //返回0表示账号密码错误（包括已删除），1表示正确，2表示待审核
        public static int LoginAuthentication(string account, string password)
        {
            SqlConnection conn = DBLink.GetConnection();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetEmployerDetailsByAccount";
            cmd.Parameters.Add(new SqlParameter("@EmployerAccount", account));
            try
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows[0]["EmployerPwd"].ToString().Trim() == password)
                {
                    if (dt.Rows[0]["IsDelete"].ToString() == "0")
                    {
                        return 1;//正常
                    }
                    else if (dt.Rows[0]["IsDelete"].ToString() == "2")
                    {
                        return 2;//待审核
                    }
                }
                else
                {
                    return 0;//账号密码不匹配
                }
            }
            catch (Exception)
            {
                return 0;//账号密码不匹配
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
            return 0;
        }
        #endregion

        #region 验证账号密码是否正确
        public static bool Authentication(string account, string password)
        {
            int count = 0;
            SqlConnection conn = DBLink.GetConnection();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "EmployerAuthentication";
            cmd.Parameters.Add(new SqlParameter("@EmployerAccount", account));
            cmd.Parameters.Add(new SqlParameter("@EmployerPwd", password));
            try
            {
                conn.Open();
                count = int.Parse(cmd.ExecuteScalar().ToString());
                if (count > 0) return true;
                else return false;
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
        }
        #endregion

        #region 根据ID修改雇主信息
        public static bool UpdateEmployerInfo(Employer employer)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UpdateEmployerInfo";
            //共26个字段
            cmd.Parameters.Add(new SqlParameter("@EmployerID", employer.EmployerID));

            //账号信息部分
            cmd.Parameters.Add(new SqlParameter("@EmployerPwd", employer.EmployerPwd));
            //联系人信息
            cmd.Parameters.Add(new SqlParameter("@ContactPersonName", employer.ContactPersonName));
            cmd.Parameters.Add(new SqlParameter("@ContactPersonSex", employer.ContactPersonSex));
            cmd.Parameters.Add(new SqlParameter("@FixedTelephone", employer.FixedTelephone));
            cmd.Parameters.Add(new SqlParameter("@MobilePhone", employer.MobilePhone));
            cmd.Parameters.Add(new SqlParameter("@Email", employer.Email));
            //单位基本信息
            cmd.Parameters.Add(new SqlParameter("@CompanyName", employer.CompanyName));
            cmd.Parameters.Add(new SqlParameter("@ParentCompanyName", employer.ParentCompanyName));
            cmd.Parameters.Add(new SqlParameter("@CompanyIntroduction", employer.CompanyIntroduction));
            cmd.Parameters.Add(new SqlParameter("@CompanyPhone", employer.CompanyPhone));
            cmd.Parameters.Add(new SqlParameter("@OrganizationCode", employer.OrganizationCode));

            cmd.Parameters.Add(new SqlParameter("@ValidPeriod", employer.ValidPeriod));
            cmd.Parameters.Add(new SqlParameter("@CompanyNature", employer.CompanyNature));
            cmd.Parameters.Add(new SqlParameter("@CompanyBusiness", employer.CompanyBusiness));
            cmd.Parameters.Add(new SqlParameter("@CompanySize", employer.CompanySize));
            cmd.Parameters.Add(new SqlParameter("@RegisteredCapital", employer.RegisteredCapital));

            cmd.Parameters.Add(new SqlParameter("@IsTop500", employer.IsTop500));
            cmd.Parameters.Add(new SqlParameter("@CompanyAreaProvince", employer.CompanyAreaProvince));
            cmd.Parameters.Add(new SqlParameter("@CompanyAreaCity", employer.CompanyAreaCity));
            cmd.Parameters.Add(new SqlParameter("@CompanyAddress", employer.CompanyAddress));
            cmd.Parameters.Add(new SqlParameter("@CityClass", employer.CityClass));

            cmd.Parameters.Add(new SqlParameter("@Remark", employer.Remark));
            //证件照片
            cmd.Parameters.Add(new SqlParameter("@CredentialsDir", employer.CredentialsDir));
            //其他字段
            cmd.Parameters.Add(new SqlParameter("@RegisterTime", employer.RegisterTime));
            cmd.Parameters.Add(new SqlParameter("@IsDelete", employer.IsDelete));

            try
            {
                cmd.ExecuteNonQuery();
                return true;//如果没有出错，返回true
            }
            catch(Exception ex)
            {
                //HttpContext.Current.Response.Write(ex.Message);
                return false;
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }
        #endregion

        #region 根据id得到雇主详细信息
        public static Employer GetEmployerDetailsByID(int id)
        {
            Employer employer = new Employer();
            SqlConnection conn = DBLink.GetConnection();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetEmployerByID";
            cmd.Parameters.Add(new SqlParameter("@EmployerID", id));
            try
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                //共27个字段，Emploeyr表所有内容
                //ID
                employer.EmployerID = Int32.Parse(dt.Rows[0]["EmployerID"].ToString());
                //账号信息部分
                employer.EmployerAccount = dt.Rows[0]["EmployerAccount"].ToString();
                employer.EmployerPwd = dt.Rows[0]["EmployerPwd"].ToString();
                //联系人信息部分
                employer.ContactPersonName = dt.Rows[0]["ContactPersonName"].ToString();
                employer.ContactPersonSex = Int16.Parse(dt.Rows[0]["ContactPersonSex"].ToString());
                employer.FixedTelephone = dt.Rows[0]["FixedTelephone"].ToString();
                employer.MobilePhone = dt.Rows[0]["MobilePhone"].ToString();
                employer.Email = dt.Rows[0]["Email"].ToString();
                //单位基本信息部分
                employer.CompanyName = dt.Rows[0]["CompanyName"].ToString();
                employer.ParentCompanyName = dt.Rows[0]["ParentCompanyName"].ToString();
                employer.CompanyIntroduction = dt.Rows[0]["CompanyIntroduction"].ToString();
                employer.CompanyPhone = dt.Rows[0]["CompanyPhone"].ToString();
                employer.OrganizationCode = dt.Rows[0]["OrganizationCode"].ToString();

                employer.ValidPeriod = DateTime.Parse(dt.Rows[0]["ValidPeriod"].ToString());
                employer.CompanyNature = dt.Rows[0]["CompanyNature"].ToString();
                employer.CompanyBusiness = dt.Rows[0]["CompanyBusiness"].ToString();
                employer.CompanySize = dt.Rows[0]["CompanySize"].ToString();
                employer.RegisteredCapital = decimal.Parse(dt.Rows[0]["RegisteredCapital"].ToString());

                employer.IsTop500 = Int32.Parse(dt.Rows[0]["IsTop500"].ToString());
                employer.CompanyAreaProvince = dt.Rows[0]["CompanyAreaProvince"].ToString();
                employer.CompanyAreaCity = dt.Rows[0]["CompanyAreaCity"].ToString();
                employer.CompanyAddress = dt.Rows[0]["CompanyAddress"].ToString();
                employer.CityClass = dt.Rows[0]["CityClass"].ToString();

                employer.Remark = dt.Rows[0]["Remark"].ToString();
                //证件照片
                employer.CredentialsDir = dt.Rows[0]["CredentialsDir"].ToString();
                //其他字段
                employer.RegisterTime = DateTime.Parse(dt.Rows[0]["RegisterTime"].ToString());
                employer.IsDelete = Int32.Parse(dt.Rows[0]["IsDelete"].ToString());

                return employer;
            }
            catch (Exception)
            {
                return new Employer();
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }
        #endregion

        #region 根据ID删除Employer的信息（置IsDelete=1）
        public static bool Delete(int EmployerID)
        {
            SqlConnection conn = DBLink.GetConnection();
            string sqlstr = "update Employer set IsDelete=1 where EmployerID=@EmployerID";
            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            cmd.Parameters.Add(new SqlParameter("@EmployerID", EmployerID));
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

        #region 根据类型获取雇主数量
        public static int GetEmployerRecordCountByType(int type)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            string sqlstr = "";
            sqlstr = "SELECT Count(*) from Employer where IsDelete=" + type;
            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            int totalpagecount = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            conn.Close();
            return totalpagecount;
        }
        #endregion

        #region 根据类型及关键字获取雇主数量
        public static int GetEmployerRecordCountByTypeByKey(int type, string key)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            string sqlstr = "";
            sqlstr = "SELECT Count(*) from Employer where CompanyName like '%'+@key+'%' and IsDelete=@type";
            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            cmd.Parameters.Add(new SqlParameter("@key", key));
            cmd.Parameters.Add(new SqlParameter("@type", type));
            int totalpagecount = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            conn.Close();
            return totalpagecount;
        }
        #endregion

        #region 根据类型获取Emloyer列表
        public static DataTable GetEmployerListByType(int pageindex, int pagesize, int type)
        {
            //安全性检查，如果time格式不对，函数返回false
            //try {  }
            //catch () { return false; }
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetEmployerPagedRecordByType";
            cmd.Parameters.Add(new SqlParameter("@pagesize", pagesize));
            cmd.Parameters.Add(new SqlParameter("@pageindex", pageindex));
            cmd.Parameters.Add(new SqlParameter("@type", type));

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion

        #region 根据类型及关键字获取Emloyer列表
        public static DataTable GetEmployerListByTypeByKey(int pageindex, int pagesize, int type, string key)
        {
            //安全性检查，如果time格式不对，函数返回false
            //try {  }
            //catch () { return false; }
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetEmployerPagedRecordByTypeByKey";
            cmd.Parameters.Add(new SqlParameter("@pagesize", pagesize));
            cmd.Parameters.Add(new SqlParameter("@pageindex", pageindex));
            cmd.Parameters.Add(new SqlParameter("@type", type));
            cmd.Parameters.Add(new SqlParameter("@key", key));

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion

        #region 雇主通过审核
        public static int GetEmployerAuditResult(int id)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            string sqlstr = "";
            sqlstr = "update Employer set IsDelete=0 where EmployerID=" + id;
            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            cmd.ExecuteNonQuery();
            return 1;
        }
        #endregion

        #region 根据ID查询单位信息
        public static DataTable Select(string EditorAccount)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select * from [Employer] where [EmployerAccount] = @EmployerAccount", conn);
            cmd.Parameters.Add(new SqlParameter("@EmployerAccount", EditorAccount));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            return dt;
        }
        #endregion

        #region 根据邮箱得到雇主密码（解密后的）
        public static Employer GetEmployerPwdByEmail(string email)
        {
            SqlConnection conn = DBLink.GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("select * from [Employer] where [Email] = @Email and IsDelete!=1", conn);
            cmd.Parameters.Add(new SqlParameter("@Email", email));
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cmd.Dispose();
            conn.Close();
            Employer employer = new Employer();
            if (dt != null && dt.Rows.Count > 0)
            {
                employer.Email = email;
                employer.EmployerPwd = Common.Text.DeCrypt(dt.Rows[0]["EmployerPwd"].ToString());
                employer.CompanyName = dt.Rows[0]["CompanyName"].ToString();
            }
            return employer;
        }
        #endregion

    }
    /// <summary>
    /// 雇主注册模型
    /// </summary>
    [NotMapped]
    public class EmployerRegister : Employer
    {
        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "密码", Description = "6-50个字符。")]
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "密码至少为6个字符")]
        [RegularExpression("^([0-9A-Za-z_]+)$", ErrorMessage = "只能输入字母，数字，下划线")]
        [DataType(DataType.Password)]
        public new string EmployerPwd { get; set; }
        /// <summary>
        /// 确认密码
        /// </summary>
        [Display(Name = "确认密码", Description = "再次输入密码。")]
        [Compare("EmployerPwd", ErrorMessage = "两次输入密码不一致")]
        [DataType(DataType.Password)]
        public string ConfirmEmployerPwd { get; set; }

        public Employer GetEmployer()
        {
            return new Employer
            {
                EmployerID = this.EmployerID,
                //账号信息部分
                EmployerAccount = this.EmployerAccount,
                EmployerPwd = this.EmployerPwd,

                //联系人信息部分
                ContactPersonName = this.ContactPersonName,
                ContactPersonSex = this.ContactPersonSex,
                FixedTelephone = this.FixedTelephone,
                MobilePhone = this.MobilePhone,
                Email = this.Email,

                //单位基本信息部分
                CompanyName = this.CompanyName,
                ParentCompanyName = this.ParentCompanyName,
                CompanyIntroduction = this.CompanyIntroduction,
                CompanyPhone = this.CompanyPhone,
                OrganizationCode = this.OrganizationCode,

                ValidPeriod = this.ValidPeriod,
                CompanyNature = this.CompanyNature,
                CompanyBusiness = this.CompanyBusiness,
                CompanySize = this.CompanySize,
                RegisteredCapital = this.RegisteredCapital,

                IsTop500 = this.IsTop500,
                CompanyAreaProvince = this.CompanyAreaProvince,
                CompanyAreaCity = this.CompanyAreaCity,
                CompanyAddress = this.CompanyAddress,
                CityClass = this.CityClass,

                Remark = this.Remark,

                //证件照片部分                
                CredentialsDir = this.CredentialsDir,

                //其他字段                
                RegisterTime = this.RegisterTime,
                IsDelete = this.IsDelete
            };
        }
    }
    /// <summary>
    /// 用户登陆模型
    /// </summary>
    [NotMapped]
    public class EmployerLogin
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "账号", Description = "4-50个字符。")]
        [Required(ErrorMessage = "×")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "×")]
        public string EmployerAccount { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "密码", Description = "6-50个字符。")]
        [Required(ErrorMessage = "×")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "×")]
        [DataType(DataType.Password)]
        public string EmployerPwd { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        [Display(Name = "验证码", Description = "请输入图片中的验证码。")]
        [Required(ErrorMessage = "×")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "×")]
        public string VerificationCode { get; set; }

    }
}