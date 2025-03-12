namespace ApiGateway.Presentation.Middleware
{
    public class AttachSignatureToRequest(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Extract specific header from the request
           context.Request.Headers["Api-Gateway"]= "Signed";
           await next(context);
            
        }
    }
}
