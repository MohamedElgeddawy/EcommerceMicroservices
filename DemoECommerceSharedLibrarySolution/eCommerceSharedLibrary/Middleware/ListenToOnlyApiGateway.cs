﻿using Microsoft.AspNetCore.Http;

namespace eCommerce.SharedLibrary.Middleware
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Extract specific header from the request
            var signedHeader = context.Request.Headers["Api-Gateway"];

            //Null means, the request is not coming from the Api Gateway // 503 service unavailable
            if (signedHeader.FirstOrDefault() == null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("sorry, service is unavailable");
                return;
            }
            else
            {
                await next(context);
            }
        }
    }
}

