using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// 组合型校验器的创建
    /// </summary>
    internal class CompositeValidatorBuilder
    {
        private List<Validator> validatorList;
        private CompositionType compositionType;
        private Validator builderValidator;
        private string messageTemplate;

        public CompositeValidatorBuilder(CompositionType compositionType)
            : this(compositionType, string.Empty)
        {
            
        }

        public CompositeValidatorBuilder(CompositionType compositionType, string messageTemplate)
        {
            this.validatorList = new List<Validator>();
            this.compositionType = compositionType;
            this.messageTemplate = messageTemplate;
        }

        public Validator GetValidator()
        {
            if (this.compositionType == CompositionType.And)
                this.builderValidator = new AndCompositeValidator(this.validatorList.ToArray());
            else
                this.builderValidator = new OrCompositeValidator(this.validatorList.ToArray());

            this.builderValidator.MessageTemplate = this.messageTemplate;

            return this.builderValidator;
        }

        internal void AddValueValidator(Validator valueValidator)
        {
            this.validatorList.Add(valueValidator);
        }

        public int GetCompositeValidatorsCount()
        {
            return this.validatorList.Count;
        }
    }
}
