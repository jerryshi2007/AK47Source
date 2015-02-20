using CIIC.HSR.TSP.WF.Ctrl.Test.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CIIC.HSR.TSP.WF.Ctrl.Test.DataAccess
{
    public class DataContext:DbContext
    {
        public DbSet<Expense> Expenses { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Expense>().HasKey(p => p.ID);
            base.OnModelCreating(modelBuilder);
        }
    }
}