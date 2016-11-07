using System;
using System.Collections.Generic;
using System.Text;

namespace ORM.DBFields
{
    [Serializable]
    public class FXML : FVarchar
    {
        public FXML() : base() { }
        public FXML(string name, bool allowNull, int length) : this(name, allowNull, length, false) { }
        public FXML(string name, bool allowNull, int length, bool hasDefaultConstraint) : this(name, allowNull, null, hasDefaultConstraint, length) { }
        public FXML(string name, bool allowNull, Object value, bool hasDefaultConstraint, int length) : base(name, allowNull, value, hasDefaultConstraint, length) { }
        
    }

}
