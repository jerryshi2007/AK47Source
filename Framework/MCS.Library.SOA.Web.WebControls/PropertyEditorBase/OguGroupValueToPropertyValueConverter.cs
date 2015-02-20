using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;

namespace MCS.Web.WebControls
{
	public class OguGroupValueToPropertyValueConverter : OguObjectValueToPropertyValueConverterBase<IGroup>
	{
		internal override OUUserInputEditorParams CreateEditorParams(Type objectValueType, object objectValue)
		{
			OUUserInputEditorParams result = base.CreateEditorParams(objectValueType, objectValue);

			result.selectMask = UserControlObjectMask.Group;
			result.listMask = UserControlObjectMask.Organization | UserControlObjectMask.Group;

			return result;
		}
	}
}
