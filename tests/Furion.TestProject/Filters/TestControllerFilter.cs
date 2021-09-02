using Microsoft.AspNetCore.Mvc.Filters;

namespace Furion.TestProject.Filters;

public class TestControllerFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        base.OnActionExecuted(context);
    }
}