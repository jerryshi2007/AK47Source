using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Web.WebControls;

namespace MCS.Web.WebControls
{
    public class ReceiversObjectListPropertyEditor : PropertyEditorBase
    {

    }

    public sealed class ObjectListPropertyEditor : PropertyEditorBase
    {
    }

	/// <summary>
	/// 流转环节可编辑活动点
	/// </summary>
	public sealed class CanActivityKeysEditor : PropertyEditorBase
	{ 
	
	}

    public sealed class ConditionExpressionPropertyEditor : PropertyEditorBase
    {

    }

    /// <summary>
    /// 子流程创建方式
    /// </summary>
    public sealed class GenerateTypePropertyEditor : PropertyEditorBase
    {

    }

    /// <summary>
    /// 子流程Key编辑
    /// </summary>
    public sealed class BranchProcessKeyPropertyEditor : PropertyEditorBase
    {

    }

    /// <summary>
    /// 分支流程资源编辑。
    /// </summary>
    public sealed class ResourcePropertyEditor : PropertyEditorBase
    {

    }

    /// <summary>
    /// 分支流程— 调用服务编辑类型
    /// </summary>
    public sealed class ServiceOperationPropertyEditor : PropertyEditorBase
    {

    }

    /// <summary>
    /// 分支流程 条件编辑
    /// </summary>
    public sealed class BranchConditionPropertyEditor : PropertyEditorBase
    { 
    
    }
	/// <summary>
    /// 返回值为string且不需再处理的编辑类型，例如一个key
    /// </summary>
	public sealed class KeyPropertyEditor : PropertyEditorBase
    { 
    
    }

	/// <summary>
	/// 是否为模版
	/// </summary>
	public sealed class DynamicPropertyEditor : PropertyEditorBase
	{ 
	
	}
	
}