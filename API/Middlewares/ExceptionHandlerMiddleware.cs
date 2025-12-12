using System.Net;

namespace API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly ILogger<ExceptionHandlerMiddleware> logger;
        private readonly RequestDelegate next;
        private readonly IHostEnvironment env; 

        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate next, IHostEnvironment env)
        {
            this.logger = logger;
            this.next = next;
            this.env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();

      
                logger.LogError(ex, "ErrorId: {ErrorId} - {Message}", errorId, ex.Message);

                httpContext.Response.ContentType = "application/json";

                var statusCode = (int)HttpStatusCode.InternalServerError;
                var message = "Something went wrong."; 

                switch (ex)
                {
                    case KeyNotFoundException:
                        statusCode = (int)HttpStatusCode.NotFound;
                        message = "The resource was not found.";
                        break;

                    case ArgumentException:
                    case InvalidOperationException: 
                    case BadHttpRequestException:
                        statusCode = (int)HttpStatusCode.BadRequest;
                        message = ex.Message;
                        break;

                    case UnauthorizedAccessException:
                        statusCode = (int)HttpStatusCode.Unauthorized;
                        message = "You are not authorized.";
                        break;

                    default:
                        statusCode = (int)HttpStatusCode.InternalServerError;
                      
                        if (env.IsDevelopment())
                        {
                            message = $"{ex.Message} (ErrorId: {errorId})";
                        }
                        else
                        {
                            message = "Something went wrong, please contact support.";
                        }
                        break;
                }

                httpContext.Response.StatusCode = statusCode;

                var error = new
                {
                    Id = errorId,
                    ErrorMessage = message
                };

                await httpContext.Response.WriteAsJsonAsync(error);
            }
        }
    }
}