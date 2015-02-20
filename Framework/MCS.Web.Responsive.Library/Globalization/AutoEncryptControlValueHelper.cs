using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace MCS.Web.Responsive.Library
{
    internal static class AutoEncryptControlValueHelper
    {
        public static void EncryptControlsValue(string[] controlIDs, Control container)
        {
            foreach (string ctrlID in controlIDs)
            {
                Control ctrl = container.FindControlByID(ctrlID, true);

                if (ctrl != null)
                {
                    string ctrlValue = GetControlValue(ctrl);

                    if (string.IsNullOrEmpty(ctrlValue) == false)
                    {
                        SetControlValue(ctrl, EncryptValue(ctrlValue));
                    }
                }
            }
        }

        public static void DecryptControlsValue(string[] controlIDs, Control container)
        {
            foreach (string ctrlID in controlIDs)
            {
                Control ctrl = container.FindControlByID(ctrlID, true);

                if (ctrl != null)
                {
                    string ctrlValue = GetControlValue(ctrl);

                    if (string.IsNullOrEmpty(ctrlValue) == false)
                    {
                        SetControlValue(ctrl, DecryptValue(ctrlValue));
                    }
                }
            }
        }

        private static string DecryptValue(string originalValue)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(originalValue) == false)
            {
                byte[] data = Convert.FromBase64String(originalValue);

                result = Encoding.UTF8.GetString(data);
            }

            return result;
        }

        private static string EncryptValue(string originalValue)
        {
            string result = string.Empty;

            byte[] data = Encoding.UTF8.GetBytes(originalValue);

            result = Convert.ToBase64String(data);

            return result;
        }

        private static void SetControlValue(Control ctrl, string ctrlValue)
        {
            if (ctrl is HtmlInputControl)
            {
                ((HtmlInputControl)ctrl).Value = ctrlValue;
            }
            else
                if (ctrl is ITextControl)
                {
                    ((ITextControl)ctrl).Text = ctrlValue;
                }
                else
                    if (ctrl is HtmlContainerControl)
                    {
                        ((HtmlContainerControl)ctrl).InnerText = ctrlValue;
                    }
        }

        private static string GetControlValue(Control ctrl)
        {
            string result = null;

            if (ctrl is HtmlInputControl)
            {
                result = ((HtmlInputControl)ctrl).Value;
            }
            else
                if (ctrl is ITextControl)
                {
                    result = ((ITextControl)ctrl).Text;
                }
                else
                    if (ctrl is HtmlContainerControl)
                    {
                        result = ((HtmlContainerControl)ctrl).InnerText;
                    }

            return result;
        }
    }
}
