using EM.Collections;

namespace EM.parser.ObjectQuery
{
    public class OperatorExpression : ExpressionTree
    {
        public OperatorExpression(IOperator op, PropertyToken p, LiteralToken l): base(op, new EList<IEvaluableExpression>() { p, l }) { }
        public override object evaluate(object obj)
        {
            ObjectComparerOperator co = (ObjectComparerOperator)op;
            LiteralToken lt = (LiteralToken) this.args[1];
            return lt.compare(obj, co);
        }
    }
}