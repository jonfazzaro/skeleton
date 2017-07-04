namespace Skeleton.Web.Api
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Filters;

    public class HandleExceptionAttribute : ExceptionFilterAttribute
    {

        public Type Type { get; set; }
        public HttpStatusCode Status { get; set; }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var ex = actionExecutedContext.Exception;
            if (ex.GetType().IsAssignableFrom(Type))
            {
                var response = actionExecutedContext.Request.CreateResponse(Status);
                response.Content = new StringContent(ex.Message);
                throw new HttpResponseException(response);
            }
        }
    }
}
