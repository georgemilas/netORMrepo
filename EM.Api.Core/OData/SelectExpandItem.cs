using System;
using EM.parser;
using EM.parser.ObjectQuery;
using EM.Api.Core.Models.Exceptions;

namespace EM.Api.Core.OData
{
    public enum SelectExpandItemType { Simple, Complex }

    public class SelectExpandItem
    {
        public string Name { get; set; }
        public SelectExpandItemType ItemType { get; set; }
        public string DataType { get; set; }
        public SelectExpandResult SelectExpand{ get; set; }
        public string RawQuery { get; set; }

        private ObjectQueryExpressionParser _ParsedQuery;
        public ObjectQueryExpressionParser ParsedQuery
        {
            get
            {
                if (_ParsedQuery == null)
                {
                    try
                    {
                        _ParsedQuery = new ObjectQueryExpressionParser(RawQuery, new ObjectEvaluatorSemantic());
                    }
                    catch (Exception err)
                    {
                        throw new ApiParameterParsingException("Invalid $query expression, an error occured parsing the expression: " + err.Message, err);
                    }
                }
                return _ParsedQuery;
            }
        }
        public bool EvaluateQuery(object obj)
        {
            try
            {
                return (bool)ParsedQuery.evaluate(obj);
            }
            catch (EvaluationException ee)
            {
                throw new ApiParameterParsingException("Invalid $query expression: " + ee.Message);  //handeled, don't log
            }
            catch (Exception err)
            {
                throw new ApiParameterParsingException("Invalid $query expression, an error occured parsing the expression: " + err.Message, err); //unhandeled, log
            }            
        }



        public override int GetHashCode()
        {
            var hash = Name.GetHashCode() | ItemType.GetHashCode() | DataType.GetHashCode() | RawQuery.GetHashCode();
            return hash;
        }
        public override bool Equals(object obj)
        {
            if (obj is SelectExpandItem)
            {
                var other = obj as SelectExpandItem;
                var eq = Name == other.Name && ItemType == other.ItemType && DataType == other.DataType && RawQuery == other.RawQuery;
                return eq;
            }
            return false;
        }
    }
}