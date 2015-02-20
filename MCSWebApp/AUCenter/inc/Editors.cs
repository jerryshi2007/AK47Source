using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Web.WebControls;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace MCS.Web.WebControls //巨坑的命名空间
{
	public sealed class CodeNameUniqueEditor : PropertyEditorBase
	{
	}

	public sealed class GetPinYinEditor : PropertyEditorBase
	{
	}

	public sealed class PObjectNameEditor : PropertyEditorBase
	{
	}

	public sealed class AdminScopeEditor : PropertyEditorBase
	{
		public override bool IsCloneControlEditor
		{
			get
			{
				return false ;
			}
		}
	}

	public sealed class SchemaCategoryEditor : PropertyEditorBase
	{

	}
}