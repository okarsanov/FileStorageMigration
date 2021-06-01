using Microsoft.Extensions.DependencyInjection;
using System;

namespace FileStorageMigration
{
    public class DependencyInjectionContainer
    {
        private ServiceProvider serviceProvider;
        public IServiceCollection Services { get; private set; }

        public DependencyInjectionContainer()
        {
            Services = new ServiceCollection();
        }

        public void Create()
        {
            serviceProvider = Services.BuildServiceProvider();
        }
        public T Get<T>()
        {
            if (serviceProvider == null)
            {
                throw new Exception("DIC not build");
            }

            return serviceProvider.GetService<T>();
        }
    }
}
