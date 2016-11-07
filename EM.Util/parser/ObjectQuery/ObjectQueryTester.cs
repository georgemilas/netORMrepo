using System;

namespace EM.parser.ObjectQuery
{
    class TT
    {
        public int Rate { get; set; }
        public string Type { get; set; }
    }

    class ObjectQueryTester
    {
        public ObjectQueryTester() { }
        
        public void test()
        {
            string kw = "(Rate ne 0 and Type eq H) or Type ne H";
            Console.WriteLine(kw);

            ObjectQueryExpressionParser kexp = new ObjectQueryExpressionParser(kw, new ObjectEvaluatorSemantic());
            TT t1 = new TT() { Rate=5, Type = "H" };
            var evRes = kexp.evaluate(t1);

            TT t2 = new TT() { Rate = 0, Type = "H" };
            evRes = kexp.evaluate(t2);

            TT t3 = new TT() { Rate = 0, Type = "$" };
            evRes = kexp.evaluate(t3);
                            
        }
    }
}