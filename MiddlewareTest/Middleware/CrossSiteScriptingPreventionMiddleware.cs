using Microsoft.AspNetCore.Http;
using MiddlewareTest.StringExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;

namespace MiddlewareTest.Middleware
{
    /// <summary>
    /// Middleware that reads request's body, and checks if it has 
    /// dangerous characters
    /// </summary>
    public class CrossSiteScriptingPreventionMiddleware
    {
        private RequestDelegate _next;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="next">Function pointer to the next 
        /// middleware in the pipeline
        /// </param>
        public CrossSiteScriptingPreventionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// When request comes in -> this Method is invoked by the runtime,
        /// in order it is added to the middleware pipeline
        /// </summary>
        /// <param name="context">HttpContext, such as HttpMethod, Response, Request etc...</param>
        /// <returns></returns>
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

                    if (body.IsValidJson())
                    {
                        var danger = body.DetectDangerousChars();

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
                    else
                    {
                        await _next(context);
                    }
                }
            }
            else
            {
                await _next(context);
            }
        }
     }
}
