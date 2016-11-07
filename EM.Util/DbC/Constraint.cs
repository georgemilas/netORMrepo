using System;
using System.Collections.Generic;
using System.Text;

namespace EM.Util
{
    public class ConstraintException : Exception { public ConstraintException() : base() { } }
    public class RequireException : Exception { public RequireException() : base() { } }

    public delegate bool ConstraintFunc();

    /////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Use ensure event to add verification delegates.  Call verify to trigger them or use  "using (constraint) { ... }" to verify 
    /// this after a code block has ran.
    /// </summary>
    public interface IConstraint : IDisposable
    {
        event ConstraintFunc ensure;
        void require(bool b);
        IList<IConstraint> chainBefore { get; }
        IList<IConstraint> chainAfter { get; }
        void verify();
    }

    /////////////////////////////////////////////////////////////////////////////
    public class Constraint : IConstraint
    {
        public event ConstraintFunc ensure;

        public Constraint()
        {
            this._chainBefore = new List<IConstraint>();
            this._chainAfter = new List<IConstraint>();
        }

        private IList<IConstraint> _chainBefore;
        public IList<IConstraint> chainBefore
        {
            get { return _chainBefore; }
        }

        private IList<IConstraint> _chainAfter;
        public IList<IConstraint> chainAfter
        {
            get { return _chainAfter; }
        }

        public virtual void require(bool b)
        {
            if (!b) throw new RequireException();
        }

        public virtual void verify()
        {
            if (this.ensure != null)
            {
                foreach (IConstraint cb in this.chainBefore)
                {
                    cb.verify();
                }
                if (!this.ensure())
                {
                    throw new ConstraintException();
                }
                foreach (IConstraint ca in this.chainAfter)
                {
                    ca.verify();
                }
            }
        }

        public virtual void Dispose()
        {
            this.verify();
        }


        /// <summary>
        /// Return new constraint. Verification of curent contraint will happed before verifications of new constraint.
        /// </summary>
        public Constraint newBeforeConstraint()
        {
            Constraint res = new Constraint();
            res.chainBefore.Add(this);
            return res;
        }
        /// <summary>
        /// Return new constraint. Verification of curent contraint will happed after verifications of new constraint.
        /// </summary>
        public Constraint newAfterConstraint()
        {
            Constraint res = new Constraint();
            res.chainAfter.Add(this);
            return res;
        }
        /// <summary>
        /// Return new constraint that hold a reference to a clone of T. Verification of curent contraint will happed before verifications of new constraint.
        /// </summary>
        public Constraint<T> newBeforeConstraint<T>(T instance) where T : ICloneable
        {
            Constraint<T> res = new Constraint<T>(instance);
            res.chainBefore.Add(this);
            return res;
        }
        /// <summary>
        /// Return new constraint that hold a reference to a clone of T. Verification of curent contraint will happed after verifications of new constraint.
        /// </summary>
        public Constraint<T> newAfterConstraint<T>(T instance) where T : ICloneable
        {
            Constraint<T> res = new Constraint<T>(instance);
            res.chainAfter.Add(this);
            return res;
        }

    }


    /////////////////////////////////////////////////////////////////////////////
    public class Constraint<T> : Constraint where T : ICloneable
    {
        public T old;
        public Constraint(T instance)
        {
            this.old = (T)instance.Clone();
        }
    }



}
