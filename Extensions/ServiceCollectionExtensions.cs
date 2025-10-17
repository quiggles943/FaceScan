using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFaceScan(this IServiceCollection services, Action<FaceScan.FaceScanFactoryOptionsBuilder> configure)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            var factory = new FaceScan.FaceScanFactory(configure);
            services.AddSingleton(factory);
            services.AddTransient(provider => factory.CreateFaceScanGenerator());
            services.AddTransient(provider => factory.CreateFaceScanComparator());
            services.AddTransient(provider => factory.CreateAggregateModelGenerator());
            return services;
        }
    }
}
