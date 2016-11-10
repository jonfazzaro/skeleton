using Skeleton.Web.State;

namespace Skeleton.Web.Tests.Fakes
{
    internal class FakeSessionProvider : ISessionProvider
    {
        public SkeletonSession Session { get; set; }
    }
}