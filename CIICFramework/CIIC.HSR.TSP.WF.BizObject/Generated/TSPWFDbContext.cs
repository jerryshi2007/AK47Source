// ReSharper disable RedundantUsingDirective
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable RedundantNameQualifier
using CIIC.HSR.TSP.DataAccess.EF;
using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.Models;
using CIIC.HSR.TSP.IoC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;
//using DatabaseGeneratedOption = System.ComponentModel.DataAnnotations.DatabaseGeneratedOption;

namespace CIIC.HSR.TSP.WF.BizObject
{


            public class TSPWFReposIoCConfigure : IIoCConfigure
	    {
		    const string PERFIX = "TSPWF_";

		    public void Configure(IIoCContainer container)
		    {
			    container.Register<IDbModelConfig>(
				    PERFIX + "IDbModelConfig",
				    SimpleDbModelConfig.Create(builder => TSPWFDbContext.CreateModel(builder, "dbo"))
		    );
		    }
	    }



    public partial class TSPWFDbContext : DbContext, ITSPWFDbContext
    {
        public IDbSet<GENERIC_OPINIONSBO> GENERIC_OPINIONSBO { get; set; } // TSPWF_GENERIC_OPINIONS
        public IDbSet<ProcessBO> ProcessBOes { get; set; } // TSPWF_Process
        public IDbSet<USER_ACCOMPLISHED_TASKBO> USER_ACCOMPLISHED_TASKBO { get; set; } // TSPWF_USER_ACCOMPLISHED_TASK
        public IDbSet<USER_OPERATION_LOGBO> USER_OPERATION_LOGBO { get; set; } // TSPWF_USER_OPERATION_LOG
        public IDbSet<USER_OPERATION_TASKS_LOGBO> USER_OPERATION_TASKS_LOGBO { get; set; } // TSPWF_USER_OPERATION_TASKS_LOG
        public IDbSet<USER_TASKBO> USER_TASKBO { get; set; } // TSPWF_USER_TASK

        static TSPWFDbContext()
        {
            Database.SetInitializer<TSPWFDbContext>(null);
        }

        public TSPWFDbContext()
            : base("Name=Default")
        {
        InitializePartial();
        }

        public TSPWFDbContext(string connectionString) : base(connectionString)
        {
        InitializePartial();
        }

        public TSPWFDbContext(string connectionString, System.Data.Entity.Infrastructure.DbCompiledModel model) : base(connectionString, model)
        {
        InitializePartial();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new GENERIC_OPINIONSBOConfiguration());
            modelBuilder.Configurations.Add(new ProcessBOConfiguration());
            modelBuilder.Configurations.Add(new USER_ACCOMPLISHED_TASKBOConfiguration());
            modelBuilder.Configurations.Add(new USER_OPERATION_LOGBOConfiguration());
            modelBuilder.Configurations.Add(new USER_OPERATION_TASKS_LOGBOConfiguration());
            modelBuilder.Configurations.Add(new USER_TASKBOConfiguration());
        OnModelCreatingPartial(modelBuilder);
        }

        public static DbModelBuilder CreateModel(DbModelBuilder modelBuilder, string schema)
        {
            modelBuilder.Configurations.Add(new GENERIC_OPINIONSBOConfiguration(schema));
            modelBuilder.Configurations.Add(new ProcessBOConfiguration(schema));
            modelBuilder.Configurations.Add(new USER_ACCOMPLISHED_TASKBOConfiguration(schema));
            modelBuilder.Configurations.Add(new USER_OPERATION_LOGBOConfiguration(schema));
            modelBuilder.Configurations.Add(new USER_OPERATION_TASKS_LOGBOConfiguration(schema));
            modelBuilder.Configurations.Add(new USER_TASKBOConfiguration(schema));
            return modelBuilder;
        }

        partial void InitializePartial();
        partial void OnModelCreatingPartial(DbModelBuilder modelBuilder);
    }
}
