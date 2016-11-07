using EM.parser.keywords;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace EM.parser.ObjectQuery
{
    public interface IObjectQuerySemantic : IKeywordsSemantic
    {
        IOperator EQ {get; set;}   //equal              ==
        IOperator NE { get; set;}  //not equal          !=
        IOperator LT { get; set;}  //less then          <
        IOperator GT { get; set; } //greater then       >
        IOperator LE { get; set; } //less then equal    <=
        IOperator GE { get; set; } //greater then equal >=       
    }


    
}



