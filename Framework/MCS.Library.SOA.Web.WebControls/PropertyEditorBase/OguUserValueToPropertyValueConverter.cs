using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;

namespace MCS.Web.WebControls
{
	public class OguUserValueToPropertyValueConverter : OguObjectValueToPropertyValueConverterBase<IUser>
	{
		internal override OUUserInputEditorParams CreateEditorParams(Type objectValueType, object objectValue)
		{
			OUUserInputEditorParams result = base.CreateEditorParams(objectValueType, objectValue);

			result.selectMask = UserControlObjectMask.User | UserControlObjectMask.Sideline;
			result.listMask = UserControlObjectMask.Organization | UserControlObjectMask.User | UserControlObjectMask.Sideline;

			return result;
		}
	}
}
