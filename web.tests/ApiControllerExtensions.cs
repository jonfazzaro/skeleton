namespace Skeleton.Web.Tests
{
    public static class ApiControllerExtensions
    {
        //public static string RoutePrefix<TApi>(this TApi controller) where TApi : ApiController
        //{
        //    return typeof(TApi)
        //            .GetCustomAttributes(true)
        //            .OfType<RoutePrefixAttribute>()
        //            .FirstOrDefault()
        //            .Prefix;
        //}

        //public static string RouteOf<TApi>(this TApi controller, string methodName) where TApi : ApiController
        //{
        //    return typeof(TApi).GetMethod(methodName)
        //            .GetCustomAttributes(true)
        //            .OfType<RouteAttribute>()
        //            .FirstOrDefault()
        //            .Template;
        //}

        //public static bool IsSecure<TApi>(this TApi controller) where TApi : ApiController
        //{
        //    return typeof(TApi)
        //        .GetCustomAttributes(true)
        //        .OfType<AuthorizeAttribute>()
        //        .Any();
        //}

        //public static bool IsCallSecure<TApi>(this TApi controller, string methodName) where TApi : ApiController
        //{
        //    return typeof(TApi).GetMethod(methodName)
        //            .GetCustomAttributes(true)
        //            .OfType<AuthorizeAttribute>()
        //            .Any();
        //}

        //public static bool IsAnonymous<TApi>(this TApi controller) where TApi : ApiController
        //{
        //    return typeof(TApi)
        //            .GetCustomAttributes(true)
        //            .OfType<AllowAnonymousAttribute>()
        //            .Any();
        //}

        //public static bool IsCallAnonymous<TApi>(this TApi controller, string methodName) where TApi : ApiController
        //{
        //    return typeof(TApi).GetMethod(methodName)
        //            .GetCustomAttributes(true)
        //            .OfType<AllowAnonymousAttribute>()
        //            .Any();
        //}


        //public static HttpStatusCode? StatusCodeFor<TApi>(this TApi controller, string methodName, Type exceptionType) where TApi : ApiController
        //{
        //    return typeof(TApi).GetMethod(methodName)
        //            .GetCustomAttributes(true)
        //            .OfType<HandleExceptionAttribute>()
        //            .FirstOrDefault(a => a.Type.Equals(exceptionType))?
        //            .Status;
        //}

    }
}
