using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elfo.Wardein.APIs
{
    public static class HttpContextExtensionMethods
    {
        public static Task ApiTryCatch(this HttpContext context, Func<Task> functionToExecute)
        {
            try
            {
                return functionToExecute();
            }
            catch (Exception ex)
            {
                return context.Response.WriteAsync(ex.Message);
            }
        }

        public static Task ApiTryCatch(this HttpContext context, Func<HttpContext, Task> functionToExecute)
        {
            try
            {
                return functionToExecute(context);
            }
            catch (Exception ex)
            {
                return context.Response.WriteAsync(ex.Message);
            }
        }
    }
}
