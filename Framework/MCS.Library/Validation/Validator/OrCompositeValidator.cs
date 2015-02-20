using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    internal class OrCompositeValidator : Validator
    {
        private IEnumerable<Validator> validators;

        public OrCompositeValidator(params Validator[] validators)
            : this(string.Empty, validators)
        {

        }

        public OrCompositeValidator(string messageTemplate, params Validator[] validators)
            : base(messageTemplate)
        {
            this.validators = validators;
        }

        protected  internal override void DoValidate(object objectToValidate, 
            object currentObject, 
            string key, 
            ValidationResults validateResults)
        {
            List<ValidationResult> childrenValidationResults = new List<ValidationResult>();
 
            foreach (Validator validator in this.validators)
            {
                ValidationResults childValidationResults = new ValidationResults();
                
                validator.DoValidate(objectToValidate, currentObject, key, childValidationResults);
                
                if (childValidationResults.IsValid())
                    return;

                childrenValidationResults.AddRange(childValidationResults);  

            }

            RecordValidationResult(validateResults, this.MessageTemplate, currentObject, key, childrenValidationResults);
        }
    }
}
