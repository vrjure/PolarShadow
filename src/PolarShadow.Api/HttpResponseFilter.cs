using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PolarShadow.Services;
using System.Net;

namespace PolarShadow.Api
{
    public class HttpResponseFilter : IActionFilter
    {
        private readonly ILogger _logger;
        public HttpResponseFilter(ILogger<HttpResponseFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception == null)
            {
                if (context.Result?.GetType() == typeof(EmptyResult))
                {
                    context.Result = new ObjectResult(Result.Success);
                }
                else
                {
                    context.Result = new ObjectResult(new ResultObject(context.Result));
                }
            }
            else
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(context.Exception, "An error happend");
                }

                if (context.Exception is ResultException resultException)
                {
                    var result = new Result { Code = resultException.Code, Message = resultException.Message };
                    context.Result = new ObjectResult(result) { StatusCode = (int)HttpStatusCode.OK };
                }
                else
                {
                    var result = new Result { Code = ResultCode.UnhandledException, Message = context.Exception.ToString() };
                    context.Result = new ObjectResult(result) { StatusCode = (int)HttpStatusCode.OK };
                }

                context.ExceptionHandled = true;
            }
        }
    }
}
