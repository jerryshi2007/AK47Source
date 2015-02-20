using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    internal class AndCompositeValidator : Validator
    {
       private IEnumerable<Validator> validators;
  
       public AndCompositeValidator(params Validator[] validators)
           : base(null, null)
       {
            this.validators = validators;
       }

	   public AndCompositeValidator(IEnumerable<Validator> validators)
		   : base(null, null)
	   {
		   this.validators = validators;
	   }

        protected internal override void DoValidate(object objectToValidate, object currentObject, 
            string key, ValidationResults validateResults)
        {
            foreach(Validator validator in this.validators)
            {
                validator.DoValidate(objectToValidate, currentObject, key, validateResults);
            }
        }
     }
}

    

