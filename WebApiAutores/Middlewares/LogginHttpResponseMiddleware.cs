using Microsoft.AspNetCore.Builder;

namespace WebApiAutores.middlewares
{

    public static class LogginHttpResponseMiddlewareExtensions
    {
        public static IApplicationBuilder UseLogginHttpResponse(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LogginHttpResponseMiddleware>();
        }
    }

    public class LogginHttpResponseMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<LogginHttpResponseMiddleware> logger;

        public LogginHttpResponseMiddleware(RequestDelegate next, ILogger<LogginHttpResponseMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (var ms = new MemoryStream())
            {
                var originalBody = context.Response.Body;
                context.Response.Body = ms;

                await next(context);

                ms.Seek(0, SeekOrigin.Begin);
                string response = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(originalBody);

                context.Response.Body = originalBody;

                logger.LogInformation(response);
            }
        }
    }
}
