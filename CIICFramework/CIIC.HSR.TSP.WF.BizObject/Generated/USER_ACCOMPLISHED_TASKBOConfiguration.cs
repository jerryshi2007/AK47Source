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
    // TSPWF_USER_ACCOMPLISHED_TASK
    public partial class USER_ACCOMPLISHED_TASKBOConfiguration : EntityTypeConfiguration<USER_ACCOMPLISHED_TASKBO>
    {
        public USER_ACCOMPLISHED_TASKBOConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".TSPWF_USER_ACCOMPLISHED_TASK");
            HasKey(x => x.TASK_GUID);

            Property(x => x.TASK_GUID).HasColumnName("TASK_GUID").IsRequired().HasMaxLength(36).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.APPLICATION_NAME).HasColumnName("APPLICATION_NAME").IsOptional().HasMaxLength(64);
            Property(x => x.PROGRAM_NAME).HasColumnName("PROGRAM_NAME").IsOptional().HasMaxLength(64);
            Property(x => x.TASK_LEVEL).HasColumnName("TASK_LEVEL").IsOptional();
            Property(x => x.TASK_TITLE).HasColumnName("TASK_TITLE").IsOptional().HasMaxLength(1024);
            Property(x => x.RESOURCE_ID).HasColumnName("RESOURCE_ID").IsOptional().HasMaxLength(36);
            Property(x => x.PROCESS_ID).HasColumnName("PROCESS_ID").IsOptional().HasMaxLength(36);
            Property(x => x.ACTIVITY_ID).HasColumnName("ACTIVITY_ID").IsOptional().HasMaxLength(36);
            Property(x => x.URL).HasColumnName("URL").IsOptional().HasMaxLength(2048);
            Property(x => x.DATA).HasColumnName("DATA").IsOptional().HasMaxLength(1073741823);
            Property(x => x.EMERGENCY).HasColumnName("EMERGENCY").IsOptional();
            Property(x => x.PURPOSE).HasColumnName("PURPOSE").IsOptional().HasMaxLength(64);
            Property(x => x.STATUS).HasColumnName("STATUS").IsOptional().HasMaxLength(50);
            Property(x => x.TASK_START_TIME).HasColumnName("TASK_START_TIME").IsOptional();
            Property(x => x.EXPIRE_TIME).HasColumnName("EXPIRE_TIME").IsOptional();
            Property(x => x.SOURCE_ID).HasColumnName("SOURCE_ID").IsOptional().HasMaxLength(36);
            Property(x => x.SOURCE_NAME).HasColumnName("SOURCE_NAME").IsOptional().HasMaxLength(64);
            Property(x => x.SEND_TO_USER).HasColumnName("SEND_TO_USER").IsOptional().HasMaxLength(36);
            Property(x => x.SEND_TO_USER_NAME).HasColumnName("SEND_TO_USER_NAME").IsOptional().HasMaxLength(64);
            Property(x => x.READ_TIME).HasColumnName("READ_TIME").IsOptional();
            Property(x => x.CATEGORY_GUID).HasColumnName("CATEGORY_GUID").IsOptional().HasMaxLength(36);
            Property(x => x.TOP_FLAG).HasColumnName("TOP_FLAG").IsOptional();
            Property(x => x.DRAFT_DEPARTMENT_NAME).HasColumnName("DRAFT_DEPARTMENT_NAME").IsOptional().HasMaxLength(512);
            Property(x => x.DELIVER_TIME).HasColumnName("DELIVER_TIME").IsOptional();
            Property(x => x.DRAFT_USER_ID).HasColumnName("DRAFT_USER_ID").IsOptional().HasMaxLength(36);
            Property(x => x.DRAFT_USER_NAME).HasColumnName("DRAFT_USER_NAME").IsOptional().HasMaxLength(64);
            Property(x => x.TenantCode).HasColumnName("TenantCode").IsOptional().HasMaxLength(36);
            Property(x => x.TaskType).HasColumnName("TaskType").IsOptional().HasMaxLength(50);
            Property(x => x.DepartmentCode).HasColumnName("DepartmentCode").IsOptional().HasMaxLength(100);
            Property(x => x.DepartmentName).HasColumnName("DepartmentName").IsOptional().HasMaxLength(100);
			Ignore(x => x.ExtendProperties);
            Ignore(x => x.AdditionalFields);
            //Ignore(x => x.DynamicExtendProperties);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
