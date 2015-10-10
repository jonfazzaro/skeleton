namespace Skeleton.Web.Tests.Fakes {
    using Skeleton.State;

    internal class FakeSessionProvider : ISessionProvider {
        public SkeletonSession Session { get; set; }
    }
}