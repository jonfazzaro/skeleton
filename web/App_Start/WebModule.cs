namespace Skeleton.Web.App_Start {
    using Cards;
    using Ninject.Modules;
    using State;
    using State.Web;
    using TeamFoundation;

    public class WebModule : NinjectModule {
        public override void Load() {
            Bind<ISessionProvider>().To<HttpSessionProvider>();
            Bind<ICardsClient>().To<TeamFoundationClient>();
            Bind<IProjectsClient>().To<TeamFoundationClient>();
        }
    }
}