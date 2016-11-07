using System;
using System.Collections.Generic;
using System.Text;

using ORM.exceptions;
using ORM.render;
using ORM.render.RenderControls;
using EM.Collections;

namespace ORM.DBFields
{
    [Serializable]
    public class FBoolean : GenericField
    {
        public FBoolean() : base() { }    
        public FBoolean(string name, bool allowNull) : this(name, allowNull, null, false) { }
        public FBoolean(string name, bool allowNull, bool hasDefaultConstraint) : this(name, allowNull, null, hasDefaultConstraint) { }
        public FBoolean(string name, bool allowNull, Object value, bool hasDefaultConstraint) : base(name, allowNull, value, hasDefaultConstraint) 
        {
            //this.renderControl = new RenderBool(this);
        }

        protected override bool mainValidate()
        {
            if ( this.value == null && this.isRequired ) 
            { 
                this.value = false;     //in web non selecting a check box means sending nothing (so null)
            }

            //bool res = base.validate();    //do all validations in once

            if (this.value != null)
            {
                try
                {
                    bool v = (bool)this.value;
                }
                catch (InvalidCastException e)
                {
                    if (this.value is string && (string)this.value == "1") 
                    {
                        this.value = true;
                        return true;
                    }
                    ValidationException ve = new ValidationException("this must be True or False", e);
                    ve.fieldName = this.name;
                    validationErrors.Add(ve);
                    return false;
                }
            }
            return true;
        }
        
        public override Object parseStringToValue(string val)
        {
            if (val == "1") return true;
            if (val == null || val=="" || val == "0" ) return false;
            throw new InvalidCastException("this is not in correct format, either 0/1 or nothing");
        }
    }


}
