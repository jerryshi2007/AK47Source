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
    // TSPWF_GENERIC_OPINIONS
    public partial class GENERIC_OPINIONSBOConfiguration : EntityTypeConfiguration<GENERIC_OPINIONSBO>
    {
        public GENERIC_OPINIONSBOConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".TSPWF_GENERIC_OPINIONS");
            HasKey(x => x.ID);

            Property(x => x.ID).HasColumnName("ID").IsRequired().HasMaxLength(36);
            Property(x => x.RESOURCE_ID).HasColumnName("RESOURCE_ID").IsOptional().HasMaxLength(36);
            Property(x => x.CONTENT).HasColumnName("CONTENT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.ISSUE_PERSON_ID).HasColumnName("ISSUE_PERSON_ID").IsOptional().HasMaxLength(36);
            Property(x => x.ISSUE_PERSON_NAME).HasColumnName("ISSUE_PERSON_NAME").IsOptional().HasMaxLength(64);
            Property(x => x.ISSUE_PERSON_LEVEL).HasColumnName("ISSUE_PERSON_LEVEL").IsOptional().HasMaxLength(36);
            Property(x => x.APPEND_PERSON_ID).HasColumnName("APPEND_PERSON_ID").IsOptional().HasMaxLength(36);
            Property(x => x.APPEND_PERSON_NAME).HasColumnName("APPEND_PERSON_NAME").IsOptional().HasMaxLength(64);
            Property(x => x.ISSUE_DATETIME).HasColumnName("ISSUE_DATETIME").IsOptional();
            Property(x => x.APPEND_DATETIME).HasColumnName("APPEND_DATETIME").IsOptional();
            Property(x => x.PROCESS_ID).HasColumnName("PROCESS_ID").IsOptional().HasMaxLength(36);
            Property(x => x.ACTIVITY_ID).HasColumnName("ACTIVITY_ID").IsOptional().HasMaxLength(36);
            Property(x => x.LEVEL_NAME).HasColumnName("LEVEL_NAME").IsOptional().HasMaxLength(64);
            Property(x => x.LEVEL_DESP).HasColumnName("LEVEL_DESP").IsOptional().HasMaxLength(64);
            Property(x => x.OPINION_TYPE).HasColumnName("OPINION_TYPE").IsOptional().HasMaxLength(64);
            Property(x => x.EVALUE).HasColumnName("EVALUE").IsOptional();
            Property(x => x.RESULT).HasColumnName("RESULT").IsOptional();
            Property(x => x.EXT_DATA).HasColumnName("EXT_DATA").IsOptional().HasMaxLength(1073741823);
			Ignore(x => x.ExtendProperties);
            Ignore(x => x.AdditionalFields);
            //Ignore(x => x.DynamicExtendProperties);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
