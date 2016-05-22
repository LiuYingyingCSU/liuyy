using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class DBLink
    {
        public static SqlConnection GetConnection()
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Jobsky8Connection"].ConnectionString);
            return conn;
        }
        public static SqlConnection GetJobsky6Connection()
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Jobsky6Connection"].ConnectionString);
            return conn;
        }
    }
}