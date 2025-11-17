using System.Net;

namespace API.Middlewares
{
  public class ExceptionHandlerMiddleware
  {
    private readonly ILogger logger;
    private readonly RequestDelegate next;

    public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate next)
    {
      this.logger = logger;
      this.next = next;
    }


    public async Task InvokeAsync(HttpContext httpContext)
    {
      try
      {
        await next(httpContext);
      }
      catch (Exception ex)
      {
        // FIXME: why are we setting the generic 500 error on everything?
        // I tried to register a user with wrong fields, which was clearly a 400 BadRequest but I got 500. This is the CURL:

        // curl -X 'POST' \
        //   'http://localhost:5016/api/Auth/Register' \
        //   -H 'accept: */
        //         *' \
        //   -H 'Content - Type: application / json' \
        //   -d '{
        //           "username": "user@example.com",
        //   "password": "string",
        //   "roles": [
        //     "string"
        //   ],
        //   "firstname": "string",
        //   "lastname": "string"
        // }'
        var errorId = Guid.NewGuid();
        logger.LogError(ex, "ErrorId: {ErrorId} - {Message}", errorId, ex.Message);

        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = "application/json";

        var error = new
        {
          Id = errorId,
          ErrorMessage = "Something went wrong"
        };
        await httpContext.Response.WriteAsJsonAsync(error);
      }

    }
  }
}
