using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OBDPI.Server.Filters
{
    public class ModelValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid) return;

            var list = new List<string>();
            foreach (var modelState in context.ModelState.Values)
            foreach (var error in modelState.Errors)
                list.Add(error.ErrorMessage);

            context.Result = new BadRequestObjectResult(list);
        }
    }
}
