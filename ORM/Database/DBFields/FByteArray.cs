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
    public class FByteArray : GenericField
    {
        public FByteArray() : base() { }
        public FByteArray(string name, bool allowNull) : this(name, allowNull, null, false) { }
        public FByteArray(string name, bool allowNull, bool hasDefaultConstraint) : this(name, allowNull, null, hasDefaultConstraint) { }
        public FByteArray(string name, bool allowNull, Object value, bool hasDefaultConstraint) : base(name, allowNull, value, hasDefaultConstraint) 
        {
            //this.renderControl = new RenderGeneric(this);
        }

        protected override bool mainValidate()
        {
            bool res = base.mainValidate();    //do all validations in once
            if (this.value != null  && !(this.value is byte[]))
            {
                try
                {
                    this.value = this.parseValue();
                }
                catch (Exception e)
                {
                    validationErrors.Add(new ValidationException("this must be an array of bytes", e));
                    res = false;
                }
            }
            return res;
        }
        
        public override Object parseStringToValue(string val)
        {
            throw new ValidationException("can not parse a string into an array of bytes");
        }


        protected Object parseValue() {
            return parseStringToValue(this.value.ToString());            
        }


    }


}
