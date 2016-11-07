using System;
using System.Collections.Generic;
using System.Text;

namespace ORM.exceptions
{
   public class BusinessLogicError : ORMException 
   {
        public BusinessLogicError(string msg)
            : base(msg)
        { }
       public BusinessLogicError(string msg, Exception e)
            : base(msg, e)
        { }
   }
}
