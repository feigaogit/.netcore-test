using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Entities
{
    public class MyContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("xxxx connection string");
        //    base.OnConfiguring(optionsBuilder);
        //}
        public MyContext(DbContextOptions<MyContext> options)
            : base(options)
        {
            //==命令 Update-Database，将更改有效的应用于数据库
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
        }
    }
}
