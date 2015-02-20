using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow.Exporters
{
	/// <summary>
	/// 导出流程定义的参数
	/// </summary>
	[Serializable]
	public class WfExportProcessDescriptorParams
	{
		public WfExportProcessDescriptorParams()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="matrixRoleAsPerson">是否将权限矩阵中的角色展开，变成人员</param>
		public WfExportProcessDescriptorParams(bool matrixRoleAsPerson)
		{
			this.MatrixRoleAsPerson = matrixRoleAsPerson;
		}

		/// <summary>
		/// 是否将权限矩阵中的角色展开，变成人员
		/// </summary>
		public bool MatrixRoleAsPerson
		{
			get;
			set;
		}
	}
}
