namespace Skeleton.Web.App_Start {
    using Cards;
    using Ninject.Modules;
    using Ninject.Web.Common;
    using State;
    using State.Web;
    using TeamFoundation;

    public class WebModule : NinjectModule {
        public override void Load() {
            Bind<ISessionProvider>().To<HttpSessionProvider>().InRequestScope();
            Bind<ICardsClient>().To<TeamFoundationClient>().InRequestScope();
            Bind<IProjectsClient>().To<TeamFoundationClient>().InRequestScope();
        }
    }
}