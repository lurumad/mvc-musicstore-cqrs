using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ninject;

namespace MvcMusicStore.App_Start
{
    public class NinjectMvcDependencyResolver : IDependencyResolver, IDisposable
    {
        private readonly IKernel _kernel;

        public NinjectMvcDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Dispose()
        {
            _kernel.Dispose();
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }
    }
}