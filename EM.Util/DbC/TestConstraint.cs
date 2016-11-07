using System;
using System.Collections.Generic;
using System.Text;

namespace EM.Util
{
    class AClass : ICloneable
    {
        public int a;
        private int _b;

        protected Constraint invariant;

        public AClass()
        {
            invariant = new Constraint();
            invariant.ensure += delegate() { return a > 20; };
            invariant.ensure += delegate() { return b < 20; };

            this.a = 20;
            this.b = 5;
        }

        public int b
        {
            get { return _b; }
            set
            {
                _b = value;
                //invariant.verify();
            }
        }

        public int getC(int c)
        {
            Constraint<AClass> constraint = invariant.newAfterConstraint<AClass>(this);
            constraint.require(c > 30);
            constraint.ensure += delegate() { return a == constraint.old.a; };
            
            using (constraint)
            {
                b = 25;     //fail invariant below
                return c;
            }
        }

        public object Clone()
        {
            AClass c = new AClass();
            c.a = this.a;
            c.b = this.b;
            return c;
        }




    }


    class TestDBC
    {
        public void TestDB()
        {
            AClass c = new AClass();
            c.b = 15;
            c.getC(31);
        }

    }
}
