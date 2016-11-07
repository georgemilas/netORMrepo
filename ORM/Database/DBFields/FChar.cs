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
    public class FChar : GenericField
    {
        public int length { get; set; }
        
        public FChar() : base() { }
        public FChar(string name, bool allowNull, int length) : this(name, allowNull, length, false) { }
        public FChar(string name, bool allowNull, int length, bool hasDefaultConstraint) : this(name, allowNull, null, hasDefaultConstraint, length) { }
        public FChar(string name, bool allowNull, Object value, bool hasDefaultConstraint, int length)
            : base(name, allowNull, value, hasDefaultConstraint)
        {
            this.length = length;
            //this.renderControl = new RenderText(this);            
        }

        protected override bool mainValidate()
        {
            bool res = true;
            if (this.value == null && this.isRequired)
            {
                //this.value = "";    //defeats the purpose of required field but that's it we must be able to remove a NULL
                ValidationException ve = new ValidationException("This is a required field");
                ve.fieldName = this.name;
                this.validationErrors.Add(ve);
                res = false;
            }

            if (this.value != null && (string)this.value != "" && this.length >0 && ((string)this.value).Length != this.length)
            {
                ValidationException ve = new ValidationException("this must be exact " + this.length + " characters long");
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
