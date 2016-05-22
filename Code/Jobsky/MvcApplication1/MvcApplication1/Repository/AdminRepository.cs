using MvcApplication1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MvcApplication1.Repository
{
    public class AdminRepository
    {
        /// <summary>
        /// 用户验证【1-成功；0-失败（账号密码不存在或者不匹配）】
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PassWrod"></param>
        /// <returns></returns>
        public int Authentication(string Account, string Pwd)
        {
            SqlConnection conn = DBLink.GetConnection();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "AdminAuthentication";
            cmd.Parameters.Add(new SqlParameter("@AdminAccount", Account));
            cmd.Parameters.Add(new SqlParameter("@AdminPwd", Pwd));
            try
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt != null)
                {
                    return 1;
                }
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