namespace Skeleton.Web.App_Start {
    using Ninject.Activation;
    using Ninject.Parameters;
    using Ninject.Syntax;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Web.Http.Dependencies;


    public class NinjectDependencyScope : IDependencyScope {
        private IResolutionRoot resolver;

        internal NinjectDependencyScope(IResolutionRoot resolver) {
            Contract.Assert(resolver != null);

            this.resolver = resolver;
        }

        public void Dispose() {
            IDisposable disposable = resolver as IDisposable;
            if (disposable != null)
                disposable.Dispose();

            resolver = null;
        }

        public object GetService(Type serviceType) {
            IRequest request = resolver.CreateRequest(serviceType, null, new Parameter[0], true, true);
            return resolver.Resolve(request).SingleOrDefault();
        }
        public IEnumerable<object> GetServices(Type serviceType) {
            IRequest request = resolver.CreateRequest(serviceType, null, new Parameter[0], true, true);
            return resolver.Resolve(request).ToList();
        }
    }
}