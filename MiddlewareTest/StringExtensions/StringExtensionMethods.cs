using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace MiddlewareTest.StringExtensions
{
    /// <summary>
    /// String Extensions Methods
    /// </summary>
    public static class StringExtensionMethods
    {
       public static char[] _dangerousChars = new char[] { '<', '>', '*', '%', '&', ':', ';', '\\', '?' };

        /// <summary>
        /// Detects if there are any dangerous characters in Json
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static bool DetectDangerousChars(this string jsonString)
        {
            var jsonReader = new JsonTextReader(new StringReader(jsonString));
            var isPasswordField = false;

            while (jsonReader.Read())
            {
                if (jsonReader.Value != null)
                {
                    var token = jsonReader.TokenType;
                    var tokenValue = jsonReader.Value;

                    switch (token)
                    {
                        case JsonToken.String:

                            if (isPasswordField)
                            {
                                isPasswordField = false;
                                continue;
                            }

                            var danger = ((string)tokenValue).ContainsDangerousChars();
                            if (danger)
                                return true;

                            break;
                        case JsonToken.PropertyName:

                            var propName = (string)tokenValue;

                            if (propName.ToLower().Equals("password"))
                                isPasswordField = true;

                            break;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks is a string contains dangerous character
        /// </summary>
        /// <param name="tokenValue">String to check</param>
        /// <returns>true if string contains dangerous character, and false otherwise</returns>
        public static bool ContainsDangerousChars(this string tokenValue)
        {
            foreach (var c in _dangerousChars)
            {
                if (tokenValue.Contains(c))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Validates Json String
        /// </summary>
        /// <param name="strInput">Json string to validate</param>
        /// <returns>True if valid Json, false otherwise</returns>
        public static bool IsValidJson(this string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
