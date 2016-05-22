using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using MvcApplication1.Models;

namespace MvcApplication1.Repository
{
    public class EmployerRepository
    {
        /// <summary>
        /// 用户验证【1-成功；0-失败（账号密码不存在或者不匹配）】
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PassWrod"></param>
        /// <returns></returns>
        public int Authentication(string EmployerAccount, string EmployerPwd)
        {
            int count = 0;
            SqlConnection conn = DBLink.GetConnection();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "EmployerAuthentication";
            cmd.Parameters.Add(new SqlParameter("@EmployerAccount", EmployerAccount));
            cmd.Parameters.Add(new SqlParameter("@EmployerPwd", EmployerPwd));
            try
            {
                conn.Open();
                count = int.Parse(cmd.ExecuteScalar().ToString());
                if (count > 0) return 1;
                else return 0;
            }
            catch (Exception)
            {
                return 0;
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }
    }
}