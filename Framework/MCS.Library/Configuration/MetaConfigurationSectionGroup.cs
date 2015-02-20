#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	MetaConfigurationSectionGroup.cs
// Remark	��	DeluxeWorks root meta configuration section group entity.
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;

using System.Configuration;
using System.Diagnostics;

namespace MCS.Library.Configuration
{
    /// <summary>
    /// DeluxeWorks root meta configuration section group entity.
    /// 
    /// <example>
      ///  <mcs.library.metaConfig>
      ///  <instance name="risk.qd">
      ///    <configMappings>
      ///      <add name="db" value="db.xml"/>
      ///      <add name="authorization" value="authorization.xml"/>
      ///    </configMappings>
      ///  </instance>
      ///  <instance name="risk.sh">
      ///    <configMappings>
      ///      <add name="db" value="db2.xml"/>
      ///      <add name="authorization" value="authorization2.xml"/>
      ///      <add name="olap" value="olap.xml"/>
      ///    </configMappings>
      ///  </instance>
      ///</mcs.library.metaConfig>
    /// </example>
    /// </summary>
    sealed class MetaConfigurationSectionGroup : ConfigurationSectionGroup 
	{

		///// <summary>
		///// Private const
		///// </summary>
		//private const string _InstanceItem = "instance";
		
        /// <summary>
        /// ���캯��
        /// </summary>
        public MetaConfigurationSectionGroup()
            : base()
        {
#if DELUXEWORKSTEST
            Trace.WriteLine("'mcs.library.metaConfig' configuration section group  constructor ...");
#endif
        }
		
        /// <summary>
        /// Դ����ӳ���
        /// </summary>
        [ConfigurationProperty(MetaConfigurationSourceInstanceSection.Name)]
        public MetaConfigurationSourceInstanceSection SourceConfigurationMapping
        {
            get
            {
                return base.Sections[MetaConfigurationSourceInstanceSection.Name]
                    as MetaConfigurationSourceInstanceSection;
            }
        }

    } // class end
}
