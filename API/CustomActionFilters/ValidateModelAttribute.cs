using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.CustomActionFilters
{
  // FIXME: try detailing a bit this. Not only generic 400 responses.
  // https://medium.com/codenx/exception-handling-in-net-core-web-api-e0c4aad1db06
  // https://stackoverflow.com/questions/10732644/best-practice-to-return-errors-in-asp-net-web-api
  public class ValidateModelAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext context)
    {
      if (!context.ModelState.IsValid)
      {
        if (context.ModelState.IsValid == false)
        {
          context.Result = new BadRequestResult();
        }
      }
    }
  }
}
