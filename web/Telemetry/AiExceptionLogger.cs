using System.Web.Http.ExceptionHandling;
using Microsoft.ApplicationInsights;

namespace Skeleton.Web.Telemetry
{
    public class AiExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            var ai = new TelemetryClient();
            ai.TrackException(context.Exception);
        }
    }
}