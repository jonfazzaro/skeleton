namespace Skeleton.Web.Telemetry {
    using System.Web.Http.ExceptionHandling;

    public class AiExceptionLogger : ExceptionLogger {

        public override void Log(ExceptionLoggerContext context) {
            var ai = new Microsoft.ApplicationInsights.TelemetryClient();
            ai.TrackException(context.Exception);
        }
    }
}