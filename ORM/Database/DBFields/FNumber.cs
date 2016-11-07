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
    public abstract class FNumber : GenericField
    {
        public FNumber() : base() { }
        public FNumber(string name, bool allowNull, Object value, bool hasDefaultConstraint, bool isIdentity)
            : base(name, allowNull, value, hasDefaultConstraint)
        {
            this.isIdentity = isIdentity;
            if (this.isIdentity) this.allowNull = false;
            //this.renderControl = new RenderNumber(this);
        }
    }

}
