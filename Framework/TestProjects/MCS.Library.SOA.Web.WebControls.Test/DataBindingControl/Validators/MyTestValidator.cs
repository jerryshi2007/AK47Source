using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MCS.Library.Validation;
namespace MCS.Library.SOA.Web.WebControls.Test.DataBindingControl
{
    public class MyTestValidator : Validator
    {
        protected override void DoValidate(object objectToValidate, object currentObject, string key, ValidationResults validateResults)
        {
            if (objectToValidate is string || objectToValidate == null)
            {
                if (string.IsNullOrEmpty((string)objectToValidate))
                    RecordValidationResult(validateResults, this.MessageTemplate, currentObject, key);
            }
        }
    }
}