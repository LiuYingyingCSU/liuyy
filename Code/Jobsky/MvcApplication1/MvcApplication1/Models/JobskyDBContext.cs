using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MvcApplication1.Models
{
    public class JobskyDBContext : DbContext
    {
        public JobskyDBContext()
            : base("Jobsky8Connection")
        {

        }
        public DbSet<Employer> Employers { get; set; }

        public DbSet<Student> Students { get; set; }
    }
}