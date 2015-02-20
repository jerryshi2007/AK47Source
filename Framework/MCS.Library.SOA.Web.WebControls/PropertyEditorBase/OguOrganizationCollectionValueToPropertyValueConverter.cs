using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;

namespace MCS.Web.WebControls
{
	public class OguOrganizationCollectionValueToPropertyValueConverter : OguObjectValueToPropertyValueConverterBase<OguDataCollection<IOrganization>>
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
