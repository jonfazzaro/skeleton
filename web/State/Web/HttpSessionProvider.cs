namespace Skeleton.State.Web {
    using Newtonsoft.Json;
    using System.Linq;
    using System.Web;

    public class HttpSessionProvider : ISessionProvider {
        readonly HttpContext _context;

        public HttpSessionProvider() {
            _context = HttpContext.Current;
        }

        public SkeletonSession Session {
            get {
                if (_context.Session[SkeletonSession.Key] == null)
                    _context.Session[SkeletonSession.Key] = Retrieve();
                return _context.Session[SkeletonSession.Key] as SkeletonSession;
            }

            set {
                _context.Session[SkeletonSession.Key] = value;
                if (value?.RememberMe ?? true)
                    Stash(value);
            }
        }

        private SkeletonSession Retrieve() {
            if (!_context.Request.Cookies.AllKeys.Contains(SkeletonSession.Key))
                return null;

            return JsonConvert.DeserializeObject<SkeletonSession>(
                _context.Request.Cookies.Get(SkeletonSession.Key).Value);
        }

        private void Stash(SkeletonSession session) {
            var cookie = _context.Request.Cookies.Get(SkeletonSession.Key);
            if (cookie == null && session != null)
                cookie = AddCookie();

            if (session == null)
                _context.Request.Cookies.Remove(SkeletonSession.Key);
            else
                cookie.Value = JsonConvert.SerializeObject(session);
        }

        private HttpCookie AddCookie() {
            var cookie = new HttpCookie(SkeletonSession.Key);
            cookie.HttpOnly = true;
            _context.Request.Cookies.Add(cookie);
            return cookie;
        }
    }
}