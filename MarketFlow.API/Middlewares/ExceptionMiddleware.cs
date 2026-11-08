using API.Errors;
using System.Net;
using System.Text.Json;

namespace API.Middlewares
{
    public class ExceptionMiddleware(IHostEnvironment env,
                                    RequestDelegate next,
                                    ILogger<ExceptionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
                
            }catch(Exception ex)
            {
                logger.LogError(ex, "Unhandled exception for {Method} {Path}",
                                context.Request.Method, context.Request.Path);

                context.Response.ContentType="applicaiton/json";
                context.Response.StatusCode=(int)HttpStatusCode.InternalServerError;

                var response = env.IsDevelopment()
                ? new ApiResponse(500, ex.Message, ex.StackTrace)
                : new ApiResponse(500, "Internal server error");

                var options = new JsonSerializerOptions
                    { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
                
            }
        }


    }
}
