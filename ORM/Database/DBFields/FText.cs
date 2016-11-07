using System;
using System.Collections.Generic;
using System.Text;

using ORM.exceptions;
using ORM.render;
using EM.Collections;

namespace ORM.DBFields
{
    [Serializable]
    public class FText : GenericField
    {
        public FText() : base() { }
        public FText(string name, bool allowNull) : this(name, allowNull, false) { }
        public FText(string name, bool allowNull, bool hasDefaultConstraint) : this(name, allowNull, null, hasDefaultConstraint) { }
        public FText(string name, bool allowNull, Object value, bool hasDefaultConstraint) : base(name, allowNull, value, hasDefaultConstraint) { }

        protected override bool mainValidate()
        {
            //base.validate() intending to skip FChar.validate and go directly GenericField.validate is not posible  

            bool res = true;
            if (this.value == null && this.isRequired)
            {
                //this.value = "";    //defets the purpose of required field but that's it we must be able to remove a NULL
                //this.validationErrors.Add(new ValidationException("This is a required field"));
                ValidationException ve = new ValidationException("This is a required field");
                ve.fieldName = this.name;
                this.validationErrors.Add(ve);
                res = false;
            }

            return res;

        }
        
        public override Object parseStringToValue(string val)
        {
            return val;
        }
    }


}
