using System;
using System.Collections.Generic;
using System.Text;

//dummy and kind of ugly Design by Contract but sort of works but probably not over inheritance 
//      unless the child class also uses DbC
//TODO: use a compiler addon DbC instead
namespace EM.DbC
{
    public class EnsureException    : Exception { public EnsureException() : base() { } }
    public class RequireException   : Exception { public RequireException() : base() { } }
    public class InvariantException : Exception { public InvariantException() : base() { } }

    public class DbC<T> where T : ICloneable
    {
        public delegate bool Invariant();
        public event Invariant invariant;

        protected T cls;
        
        public DbC(T t) 
        {
            this.cls = t;
        }

        public T current
        {
            get
            {
                return (T)this.cls.Clone();
            }
        }

        public void require(bool b)
        {
            if (!b) throw new RequireException();            
        }
        public void testInvariants()
        {

            if (this.invariant != null)
            {
                if (!this.invariant())
                {
                    throw new InvariantException();
                }
            }
        }


        /////////////////////////////////////////////////////////////////////////////
        public class Returner<R>
        {
            public delegate bool Ensure();
            public event Ensure ensure;
            
            private DbC<T> _dbc;
            private T _old;
            private R _retValue;

            public Returner()
            {}
            public Returner(DbC<T> dbc)
            {
                this.dbc = dbc;
            }
            
            public DbC<T> dbc
            {
                get { return this._dbc; }
                set 
                {
                    this._dbc = value;
                    _old = this._dbc.current;
                }

            }
            public T old 
            {
                get { return this._old; }
            }

            public R retValue
            {
                get { return this._retValue; }
            }

            public R doReturn(R retValue)
            {
                this._retValue = retValue;

                if (this.ensure != null)
                {
                    if (!this.ensure())
                    {
                        throw new EnsureException();
                    }
                }
                if (dbc != null) 
                { 
                    dbc.testInvariants(); 
                }

                return retValue;

            }

        }


    }

    
}
