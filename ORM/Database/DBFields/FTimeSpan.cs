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
    public class FTimeSpan : GenericField
    {
        public FTimeSpan() : base() { }
        public FTimeSpan(string name, bool allowNull) : this(name, allowNull, null, false) { }
        public FTimeSpan(string name, bool allowNull, bool hasDefaultConstraint) : this(name, allowNull, null, hasDefaultConstraint) { }
        public FTimeSpan(string name, bool allowNull, Object value, bool hasDefaultConstraint) 
            : base(name, allowNull, value, hasDefaultConstraint)
        {
            //this.renderControl = new RenderDateTime(this);
        }

        protected override bool mainValidate()
        {
            bool res = base.mainValidate();    //do all validations in once
            if (this.value != null && !(this.value is TimeSpan))
            {
                try
                {
                    if (this.value is string)
                    {
                        this.value = TimeSpan.Parse((string)this.value);
                    }
                    else
                    {
                        //this.validationErrors.Add(new ValidationException("this must be date/time value"));
                        ValidationException ve = new ValidationException("this must be a timespan value");
                        ve.fieldName = this.name;
                        this.validationErrors.Add(ve);
                        return false;
                    }
                }
                catch (Exception e)   //InvalidCastException 
                {
                    this.validationErrors.Add(new ValidationException("this must be a timespan value", e));
                    return false;
                }
            }
            return res;
        }
        
        public override Object parseStringToValue(string val)
        {
            return TimeSpan.Parse(val);
        }
    }


}
