using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Web.UI;
using System.Reflection;
using MCS.Library.Core;

namespace MCS.Library.Web.Controls
{
	internal class PassportWebControlHelper
	{
		public static T GetViewState<T>(StateBag stateBag, string strKey, T defaultValue)
		{
			object oValue = stateBag[strKey];

			if (oValue == null)
				oValue = defaultValue;

			if (defaultValue != null)
			{
				System.Type t = defaultValue.GetType();
				oValue = DataConverter.ChangeType(oValue, t);
			}

			return (T)oValue;
		}

		public static T GetControlValue<T>(Control container, string controlID, string propertyName, T defaultValue)
		{
			T data = defaultValue;

			if (container != null)
			{
				Control ctrl = FindControlRecursively(container, controlID);

				if (ctrl != null)
				{
					PropertyInfo[] piArray = ctrl.GetType().GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);

					try
					{
						for (int i = 0; i < piArray.Length; i++)
						{
							PropertyInfo pi = piArray[i];

							if (string.Compare(pi.Name, propertyName, true) == 0)
							{
								if (pi.CanRead)
									data = (T)pi.GetValue(ctrl, null);

								break;
							}
						}
					}
					catch (System.Exception ex)
					{
						throw new SystemSupportException(string.Format("读取控件{0}属性{1}值\"{2}\"出错，{3}",
							controlID, propertyName, data.ToString(), ex.Message), ex);
					}
				}
			}

			return data;
		}

		public static void SetControlValue(Control container, string controlID, string propertyName, object data)
		{
			if (container != null)
			{
				Control ctrl = FindControlRecursively(container, controlID);

				if (ctrl != null)
				{
					PropertyInfo[] piArray = ctrl.GetType().GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);

					try
					{
						for (int i = 0; i < piArray.Length; i++)
						{
							PropertyInfo pi = piArray[i];

							if (string.Compare(pi.Name, propertyName, true) == 0)
							{
								if (pi.CanWrite)
									pi.SetValue(ctrl, data, null);

								break;
							}
						}
					}
					catch (System.Exception ex)
					{
						throw new SystemSupportException(string.Format("为控件{0}，设置属性{1}值\"{2}\"出错，{3}",
															controlID, propertyName, data.ToString(), ex.Message), ex);
					}
				}
			}
		}

		public static Control FindControlRecursively(Control parent, string controlID)
		{
			Control innerCtrl = parent.FindControl(controlID);

			if (innerCtrl == null)
			{
				foreach (Control ctrl in parent.Controls)
				{
					innerCtrl = FindControlRecursively(ctrl, controlID);

					if (innerCtrl != null)
						break;
				}
			}

			return innerCtrl;
		}
	}
}
