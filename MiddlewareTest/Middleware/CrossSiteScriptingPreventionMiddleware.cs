using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace MiddlewareTest.Middleware
{
    public class CrossSiteScriptingPreventionMiddleware
    {
        private RequestDelegate _next;
        private char[] _dangerousChars = new char[] { '<', '>', '*', '%', '&', ':', ';', '\\', '?' };

        public CrossSiteScriptingPreventionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var method = context.Request.Method;

            if (method.ToLower().Equals("post") ||
                        method.ToLower().Equals("put"))
            {
                var request = context.Request;

                using (var reader = new StreamReader(request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    var danger = DetectDangerousChars(body);

                    if (!danger)
                    {
                        await _next(context);
                    }
                    else
                    {
                        var response = context.Response;
                        response.StatusCode = 501;
                        await response.WriteAsync($"Dangerous Characters are not allowed!");
                    }
                }
            }
            else
            {
                await _next(context);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonString"></param>
        private bool DetectDangerousChars(string jsonString)
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

                            var danger = ContainsDangerousChars((string)tokenValue);
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
        /// 
        /// </summary>
        /// <param name="tokenValue"></param>
        /// <returns></returns>
        private bool ContainsDangerousChars(string tokenValue)
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

    }

}
