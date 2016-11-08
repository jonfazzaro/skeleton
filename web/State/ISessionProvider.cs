namespace Skeleton.Web.State
{
    public interface ISessionProvider
    {
        SkeletonSession Session { get; set; }
    }
}