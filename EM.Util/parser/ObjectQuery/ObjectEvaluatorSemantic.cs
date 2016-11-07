using EM.parser.keywords.TextSearch;

namespace EM.parser.ObjectQuery
{
    public class ObjectEvaluatorSemantic : ObjectQuerySemantic
    {
        public ObjectEvaluatorSemantic()
        {
            //this are short-circuit operators evaluators so maybe not everything will be evaluated
            this.AND = new SearchAND();
            this.OR = new SearchOR();
            this.NOT = new SearchNOT();
            this.EQ = new ObjectComparerOperator("eq"); //, (l,r) => l == r);
            this.NE = new ObjectComparerOperator("ne"); //, (l, r) => l != r);
            this.LT = new ObjectComparerOperator("lt"); //, (l, r) => l < r);
            this.GT = new ObjectComparerOperator("gt"); //, (l, r) => l > r);
            this.LE = new ObjectComparerOperator("le"); //, (l, r) => l <= r);
            this.GE = new ObjectComparerOperator("ge"); //, (l, r) => l >= r);            
        }       
    }
}