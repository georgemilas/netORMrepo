using System;
using System.Collections.Generic;
using System.Text;

using ORM.exceptions;
using ORM.render;
using EM.Collections;

namespace ORM.DBFields
{
    [Serializable]
    public class FFloat : FNumber
    {
        public FFloat() : base() { }
        public FFloat(string name, bool allowNull, bool isIdentity) : this(name, allowNull, null, false, isIdentity) { }
        public FFloat(string name, bool allowNull, bool hasDefaultConstraint, bool isIdentity) : this(name, allowNull, null, hasDefaultConstraint, isIdentity) { }
        public FFloat(string name, bool allowNull, Object value, bool hasDefaultConstraint, bool isIdentity) : base(name, allowNull, value, hasDefaultConstraint, isIdentity) { }

        protected override bool mainValidate()
        {
            bool res = base.mainValidate();    //do all validations in once
            if (this.value != null && !(this.value is float))
            {
                try
                {
                    this.value = this.parseValue();
                }
                catch (Exception e)
                {
                    this.validationErrors.Add(new ValidationException("this must be a number (no decimals)", e));
                    res = false;
                }
            }
            return res;
        }
        
        public override Object parseStringToValue(string val)
        {
            switch (this.valueTypeName)
            {
                case "Double?":
                case "Double":
                    return Double.Parse(val);
                case "Decimal?":
                case "Decimal":
                    return Decimal.Parse(val);
                default:
                    return float.Parse(val);
            }
        }


        protected Object parseValue()
        {
            return parseStringToValue(this.value.ToString());
        }


    }

}
