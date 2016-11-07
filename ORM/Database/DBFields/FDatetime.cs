using System;
using System.Collections.Generic;
using System.Text;

using ORM.exceptions;
using ORM.render;
using EM.Collections;
using ORM.render.RenderControls;

namespace ORM.DBFields
{
    [Serializable]
    public class FDatetime : GenericField
    {
        public FDatetime() : base() { }
        public FDatetime(string name, bool allowNull) : this(name, allowNull, null, false) { }
        public FDatetime(string name, bool allowNull, bool hasDefaultConstraint) : this(name, allowNull, null, hasDefaultConstraint) { }
        public FDatetime(string name, bool allowNull, Object value, bool hasDefaultConstraint) 
            : base(name, allowNull, value, hasDefaultConstraint)
        {
            //this.renderControl = new RenderDateTime(this);
        }

        protected override bool mainValidate()
        {
            bool res = base.mainValidate();    //do all validations in once
            if (this.value != null && !(this.value is DateTime))
            {
                try
                {
                    if (this.value is string)
                    {
                        this.value = DateTime.Parse((string)this.value);
                    }
                    else
                    {
                        //this.validationErrors.Add(new ValidationException("this must be date/time value"));
                        ValidationException ve = new ValidationException("this must be a date/time value");
                        ve.fieldName = this.name;
                        this.validationErrors.Add(ve);
                        return false;
                    }
                }
                catch (Exception e)   //InvalidCastException 
                {
                    this.validationErrors.Add(new ValidationException("this must be a date/time value", e));
                    return false;
                }
            }
            return res;
        }
        
        public override Object parseStringToValue(string val)
        {
            return DateTime.Parse(val);
        }
    }


}
