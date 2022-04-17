using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KurnaiNetbookHelper
{
    class Validate
    {
        private Constants _Constants = new Constants();
        public bool IsAlpha(string text)
        {
            bool result = false;

            if (Regex.IsMatch(text, @"^[a-zA-Z]+$"))
                result = true;

            return result;
        }

        public bool IsNumber(string text)
        {
            bool result = false;

            if (Regex.IsMatch(text, @"^[0-9]+$"))
                result = true;

            return result;
        }

        public bool IsAlphaNum(string text)
        {
            bool result = false;

            if (Regex.IsMatch(text, @"^[a-zA-Z0-9]+$"))
                result = true;

            return result;
        }

        public bool IsPhrase(string text)
        {
            bool result = false;

            if (Regex.IsMatch(text, @"^[a-zA-Z0-9 '\-.]+$"))
                result = true;

            return result;
        }

        public bool IsEmail(string text)
        {
            bool result = false;

            if (Regex.IsMatch(text, @"^[_a-z0-9-]+(\.[_a-z0-9-]+)*@edumail\.vic\.gov\.au$"))
                result = true;

            return result;
        }

        public bool IsStudentID(string text)
        {
            bool result = false;

            if (Regex.IsMatch(text, _Constants.COMMON_INFO["StudentIDFormat"]))
                result = true;

            return result;
        }

        public bool IsPONumber(string text, string campus)
        {
            bool result = false;

            for (int i = 0; i < _Constants.CAMPUS_INFO.GetLength(0); i++)
                if (campus == _Constants.CAMPUS_INFO[i]["Name"] && Regex.IsMatch(text, _Constants.CAMPUS_INFO[i]["POFormat"]))
                    result = true;

            return result;
        }

        public bool IsSerial(string serial, string model)
        {
            bool result = false;

            for (int i = 0; i < _Constants.MODEL_INFO.GetLength(0); i++)
                if (model == _Constants.MODEL_INFO[i]["Name"] && Regex.IsMatch(serial, _Constants.MODEL_INFO[i]["SerialFormat"]))
                    result = true;

            return result;
        }
    }
}
