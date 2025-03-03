using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace eCommerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Declare default  Variables
            string message = "sorry, internal server error occurred. Kindly try again later";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Internal Server Error";
            try
            {
                await next(context);

                //check if Response is Too Many Request // 429 status code.
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    message = "Sorry, too many request. Kindly try again later";
                    statusCode = (int)HttpStatusCode.TooManyRequests;
                    title = "Too Many Requests";
                    await ModifyHeader( context, title, message, statusCode);
                }
                // If Response is UnAuthorize // 401 status code
                if(context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    message = "Sorry, you are not authorized to access this resource";
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    title = "Alrert";
                    await ModifyHeader( context,  title, message, statusCode);
                }

                //If Response is forbidden // 403 status code
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    message = "Sorry, you are forbidden to access this resource";
                    statusCode = (int)HttpStatusCode.Forbidden;
                    title = "Out of Access";
                    await ModifyHeader(context, title, message, statusCode);
                }

            }
            catch (Exception ex)
            {
                // Log Original Exception / File, Console , Debugger
                LogException.LogExceptions(ex);

                //check if Exception is Timeout // 408 request timeout
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    message = "Sorry, request timeout. Kindly try again later";
                    statusCode = StatusCodes.Status408RequestTimeout;
                    title = "Request Timeout";
                }

                //If Exception is caught.
                //if none The Exception   then  do the default
                await ModifyHeader(context, title, message, statusCode);
            }
        }

        private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Title = title,
                Detail = message,
                Status = statusCode
            }), CancellationToken.None);
            return;
        }

    }
}
