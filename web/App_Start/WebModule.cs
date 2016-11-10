using Ninject.Modules;
using Ninject.Web.Common;
using Skeleton.Web.Cards;
using Skeleton.Web.State;
using Skeleton.Web.State.Web;
using Skeleton.Web.TeamFoundation;

namespace Skeleton.Web.App_Start
{
    public class WebModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISessionProvider>().To<HttpSessionProvider>().InRequestScope();
            Bind<ICardsClient>().To<TeamFoundationClient>().InRequestScope();
            Bind<IProjectsClient>().To<TeamFoundationClient>().InRequestScope();
        }
    }
}