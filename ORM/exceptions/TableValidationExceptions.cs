using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;

namespace ORM.exceptions
{
    public class TableValidationExceptions : ValidationException
    {
        private ESet<ValidationException> _errorsList;
        public ESet<ValidationException> errorsList
        {
          get { return _errorsList; }
          set { _errorsList = value; }
        }
        public string TableName 
        {
            get;
            set;
        }

        public TableValidationExceptions(string msg)
            : base(msg)
        { 
            this._errorsList = new ESet<ValidationException>();
            this.errorLocation = ERROR_LOCATION.TABLE;
        }
        public TableValidationExceptions(string msg, string tableName)
            : this(msg)
        {
            this.TableName = tableName;
        }
        public TableValidationExceptions(string msg, ESet<ValidationException> errors)
            : this(msg)
        { 
            this.errorsList = errors;            
        }
        public TableValidationExceptions(string msg, string tableName, ESet<ValidationException> errors)
            : this(msg, errors)
        {
            this.TableName = tableName;
        }
        public string getAllErrorsDetails()
        {
            StringBuilder sb = new StringBuilder(this.Message + StringUtil.CRLF);
            foreach (ValidationException er in errorsList)
            {
                sb.AppendFormat("{0} - {1}" + StringUtil.CRLF, er.fieldName, er.Message);
            }
            return sb.ToString();
        }
    }
}
