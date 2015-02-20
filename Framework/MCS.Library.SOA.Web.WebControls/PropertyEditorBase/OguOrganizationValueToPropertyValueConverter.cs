using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;

namespace MCS.Web.WebControls
{
	public class OguOrganizationValueToPropertyValueConverter : OguObjectValueToPropertyValueConverterBase<IOrganization>
	{
		internal override OUUserInputEditorParams CreateEditorParams(Type objectValueType, object objectValue)
		{
			OUUserInputEditorParams result = base.CreateEditorParams(objectValueType, objectValue);

			result.selectMask = UserControlObjectMask.Organization;
			result.listMask = UserControlObjectMask.Organization;

			return result;
		}
	}
}
