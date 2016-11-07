using System;
using System.Collections.Generic;
using System.Text;

using ORM.exceptions;
using ORM.render;
using EM.Collections;

namespace ORM.DBFields
{
    [Serializable]
    public class FInteger : FNumber
    {
        public FInteger() : base() { }
        public FInteger(string name, bool allowNull, bool isIdentity) : this(name, allowNull, null, false, isIdentity) { }
        public FInteger(string name, bool allowNull, bool hasDefaultConstraint, bool isIdentity) : this(name, allowNull, null, hasDefaultConstraint, isIdentity) { }
        public FInteger(string name, bool allowNull, Object value, bool hasDefaultConstraint, bool isIdentity) : base(name, allowNull, value, hasDefaultConstraint, isIdentity) { }

        protected override bool mainValidate()
        {
            bool res = base.mainValidate();    //do all validations in once
            if (this.value != null  && !(this.value is int))
            {
                try
                {
                    this.value = this.parseValue();
                }
                catch (Exception e)
                {
                    validationErrors.Add(new ValidationException("this must be a number (no decimals)", e));
                    res = false;
                }
            }
            return res;

        }
        
        public override Object parseStringToValue(string val)
        {
            switch (this.valueTypeName)
            {
                case "Int32?":
                case "Int32":
                    return Int32.Parse(val);
                case "Int16?":
                case "Int16":
                    return Int16.Parse(val);
                case "Int64?":
                case "Int64":
                    return Int64.Parse(val);
                case "Byte?":
                case "Byte":
                    return Byte.Parse(val);
                default:
                    return int.Parse(val);
            }
        }


        protected Object parseValue() {
            return parseStringToValue(this.value.ToString());            
        }


    }


}
