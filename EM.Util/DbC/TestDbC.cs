using System;
using System.Collections.Generic;
using System.Text;

//using NUnit.Framework;

//using EM.DbC;

namespace EM.DbC
{

    class AClass : ICloneable
    {
        public int a;
        private int _b;

        protected DbC<AClass> dbc;

        public AClass()
        {
            this.dbc = new EM.DbC.DbC<AClass>(this);
            this.dbc.invariant += delegate() { return a > 20; };
            this.dbc.invariant += delegate() { return b < 20; };

            this.a = 20;
            this.b = 5;



        }

        public int b
        {
            get { return _b; }
            set
            {
                _b = value;
                dbc.testInvariants();
            }
        }

        public int getC(int c)
        {
            dbc.require(c > 30);
            DbC<AClass>.Returner<int> ret = new DbC<AClass>.Returner<int>(dbc);
            ret.ensure += delegate() { return b == ret.old.b; };
            ret.ensure += delegate() { return ret.retValue == c; };

            b = 15;

            return ret.doReturn(c);

        }

        public object Clone()
        {
            AClass c = new AClass();
            c.a = this.a;
            c.b = this.b;
            return c;
        }




    }


    //[TestFixture]
    class DbCTest
    {
        //[SetUp]
        public void SetUp()
        { }

        //[Test]
        public void TestDB()
        {
            AClass c = new AClass();
            c.b = 15;
            c.getC(15);
        }

    }
}
