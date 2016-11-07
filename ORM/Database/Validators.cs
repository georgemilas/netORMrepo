using System;
using System.Collections.Generic;
using System.Text;
using ORM.DBFields;
using System.Text.RegularExpressions;
using ORM.exceptions;

namespace ORM
{
    public class Validators
    {
        public static bool isPhoneNumber(GenericField fld)
        {
            if (fld.value == null) { return true; }

            string val = ((string)fld.value).Trim();
            if (val == "") return true;

            Regex r = new Regex(@"^\d\d\d[ -]*\d\d\d[ -]*\d\d\d\d$", RegexOptions.IgnoreCase);
            Match m = r.Match(val);
            if (!m.Success)
            {
                fld.validationErrors.Add(new ValidationException("Value entered is not a phone number"));
                return false;
            }
            fld.value = val.Replace(" ", "").Replace("-", "");
            return true;
        }

        public static bool isIntegerNumber(GenericField fld)
        {
            if (fld.value == null) { return true; }

            string val = ((string)fld.value).Trim();
            if (val == "") return true;

            Regex r = new Regex(@"^\d+$", RegexOptions.IgnoreCase);
            Match m = r.Match(val);
            if (!m.Success)
            {
                fld.validationErrors.Add(new ValidationException("Value entered must be only numbers"));
                return false;
            }
            
            return true;
        }



        public static bool isFloatNumber(GenericField fld)
        {
            if (fld.value == null) { return true; }

            string val = ((string)fld.value).Trim();
            if (val == "") return true;

            Regex r = new Regex(@"^\d+(\.\d+)?$", RegexOptions.IgnoreCase);
            Match m = r.Match(val);
            if (!m.Success)
            {
                fld.validationErrors.Add(new ValidationException("Value entered must be only numbers"));
                return false;
            }

            return true;
        }

        public static bool isEmail(GenericField fld)
        {
            if (fld.value == null) { return true; }

            string val = ((string)fld.value).Trim();
            if (val == "") return true;

            Regex r = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,})(\]?)$", RegexOptions.IgnoreCase);
            Match m = r.Match(val);
            if (!m.Success)
            {
                fld.validationErrors.Add(new ValidationException("Value entered is not in email format"));
                return false;
            }
            return true;
        }

        public static bool isPOBox(GenericField fld)
        {
            if (fld.value == null) { return true; }

            string val = ((string)fld.value).Trim();
            if (val == "") return true;

            val = val.Trim().Replace(".", "").Replace(" ", "").ToUpper();

            if (val.Length >= 5 && val.Substring(0, 5) == "POBOX")
            {
                //this is PO Box, let's check if the string after X is a proper number
                var field = ((string) fld.value).Trim().ToUpper();
                var xPos = field.IndexOf('X');
                var number = field.Substring(xPos + 1).Trim();

                long pobNumber;
                bool bResult = long.TryParse(number, out pobNumber);
                if (!bResult)
                {
                    fld.validationErrors.Add(new ValidationException("PO Box number must be numeric"));
                    return false;
                }
                if (number.Length > 10)
                {
                    fld.validationErrors.Add(new ValidationException("PO Box number must be ten digits or less"));
                    return false;
                }

            }

            return true;
        }
    }
}
