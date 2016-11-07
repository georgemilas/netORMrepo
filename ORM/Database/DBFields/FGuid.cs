using System;
using System.Collections.Generic;
using System.Text;

using ORM.exceptions;
using ORM.render;
using EM.Collections;

namespace ORM.DBFields
{
    [Serializable]
    public class FGuid : FVarchar
    {
        public FGuid() : base() { }
        public FGuid(string name, bool allowNull, int length) : this(name, allowNull, length, false) { }
        public FGuid(string name, bool allowNull, int length, bool hasDefaultConstraint) : this(name, allowNull, null, hasDefaultConstraint, length) { }
        public FGuid(string name, bool allowNull, Object value, bool hasDefaultConstraint, int length) : base(name, allowNull, value, hasDefaultConstraint, length) { }

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

            /*
            if (this.value != null && this.length>0 && ((string)this.value).Length > this.length)
            {
                //this.validationErrors.Add(new ValidationException("this must be maximum " + this.length + " characters long"));
                ValidationException ve = new ValidationException("this must be maximum " + this.length + " characters long");
                ve.fieldName = this.name;
                this.validationErrors.Add(ve);
                res = false;
            }
            */
            return res;

        }
        
        public override Object parseStringToValue(string val)
        {
            Guid g = new Guid(val);
            return g;
        }

    }


}
