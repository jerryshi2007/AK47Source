namespace CIIC.HSR.TSP.WF.Ctrl.Test.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DataModel : DbContext
    {
        public DataModel()
            : base("name=DataModel")
        {
        }

        public virtual DbSet<USER_ACCOMPLISHED_TASK> USER_ACCOMPLISHED_TASK { get; set; }
        public virtual DbSet<USER_TASK> USER_TASK { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<USER_ACCOMPLISHED_TASK>()
                .Property(e => e.STATUS)
                .IsFixedLength();
        }
    }
}
