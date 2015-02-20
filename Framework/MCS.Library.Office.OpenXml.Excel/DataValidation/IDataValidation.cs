using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	/// <summary>
	/// 用于数据验证的接口
	/// </summary>
	public interface IDataValidation
	{
		/// <summary>
		/// 地址
		/// </summary>
		Range Address { get; }

		/// <summary>
		/// 验证类型
		/// </summary>
		DataValidationType ValidationType { get; }

		/// <summary>
		/// 处理无效数据样式
		/// </summary>
		ExcelDataValidationWarningStyle ErrorStyle { get; set; }

		/// <summary>
		/// True 显示提示输入消息
		/// </summary>
		bool? AllowBlank { get; set; }

		/// <summary>
		/// 选定单元格时显示待输入信息
		/// </summary>
		bool? ShowInputMessage { get; set; }

		/// <summary>
		/// 输入无效数据时显示错误警告
		/// </summary>
		bool? ShowErrorMessage { get; set; }

		/// <summary>
		/// 错误标题
		/// </summary>
		string ErrorTitle { get; set; }

		/// <summary>
		/// 错误内容
		/// </summary>
		string Error { get; set; }

		/// <summary>
		/// 输入消息框的标题
		/// </summary>
		string PromptTitle { get; set; }

		/// <summary>
		/// 提示消息
		/// </summary>
		string Prompt { get; set; }

		/// <summary>
		/// 当前的验证类型允许操作
		/// </summary>
		bool AllowsOperator { get; }

		/// <summary>
		/// 证的状态。
		/// </summary>
		void Validate();

	}
}
