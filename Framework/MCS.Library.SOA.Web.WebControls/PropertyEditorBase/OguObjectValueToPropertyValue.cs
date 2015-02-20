using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Web.WebControls
{
	public abstract class OguObjectValueToPropertyValueConverter : OguObjectValueToPropertyValueConverterBase<IOguObject>
	{
		internal override OUUserInputEditorParams CreateEditorParams(Type objectValueType, object objectValue)
		{
			OUUserInputEditorParams result = base.CreateEditorParams(objectValueType, objectValue);

			result.selectMask = UserControlObjectMask.All;
			result.listMask = UserControlObjectMask.All;

			return result;
		}
	}
}
