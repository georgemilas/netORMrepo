using System;
using System.Collections.Generic;
namespace EM.DB
{
    
    public interface IDBParams: IList<IDBParam>
    {
        void Add(string name, object value, Type actualDBParamType);
        void Add(string name, object value);
    }

}

