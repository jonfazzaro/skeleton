namespace Skeleton.State.Web {
    using System.Web;

    public class HttpSessionProvider : ISessionProvider {
        public SkeletonSession Session {
            get {
                return HttpContext.Current.Session[SkeletonSession.Key] as SkeletonSession;
            }

            set {
                HttpContext.Current.Session[SkeletonSession.Key] = value;
            }
        }
    }
}