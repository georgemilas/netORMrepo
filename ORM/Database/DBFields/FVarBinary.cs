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
    public class FVarBinary : FByteArray
    {
        public FVarBinary() : base() { }
        public FVarBinary(string name, bool allowNull) : base(name, allowNull) { }
        public FVarBinary(string name, bool allowNull, bool hasDefaultConstraint) : base(name, allowNull, hasDefaultConstraint) { }
        public FVarBinary(string name, bool allowNull, Object value, bool hasDefaultConstraint) : base(name, allowNull, value, hasDefaultConstraint) { }

    }

}
