using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Kekiri.IoC
{
    public partial class ServiceProviderBootstrapper
    {
        Type _startupType;
        Action<IServiceCollection> _config;

        public ServiceProviderBootstrapper()
        {
            _config = x => { };
        }

        public ServiceProviderBootstrapper UseStartup<TStartup>() where TStartup : class
            => UseStartup(typeof(TStartup));

        public ServiceProviderBootstrapper UseStartup(Type type)
        {
            _startupType = type;
            return this;
        }

        public ServiceProviderBootstrapper OverrideServicesWithTypesFromAssemblyOf<T>() =>
            ConfigureServicesPostStartup(x =>
                typeof(T).Assembly.GetTypes()
                    .Where(y => !y.IsAbstract && !y.IsInterface)
                    .SelectMany(ServiceDescriptors)
                    .Aggregate(x, (s, y) => { s.Add(y); return s; }));

        static IEnumerable<ServiceDescriptor> ServiceDescriptors(Type type)
        {
            yield return ServiceDescriptor.Scoped(type, type);

            foreach (var serviceType in type.GetInterfaces())
            {
                yield return ServiceDescriptor.Scoped(serviceType, x => x.GetRequiredService(type));
            }

            Type t = type.BaseType;

            while (t != null && t.IsAbstract)
            {
                yield return ServiceDescriptor.Scoped(t, x => x.GetRequiredService(type));

                t = t.BaseType;
            }
        }

        public ServiceProviderBootstrapper ConfigureServicesPostStartup(Action<IServiceCollection> config)
            => ConfigureServices(x =>
            {
                if (_startupType == null)
                {
                    config(x);
                }
                else
                {
                    x.AddSingleton<IStartupConfigureServicesFilter>(y =>
                        new PostStartupConfigureServicesFilter(config));
                }
            });

        public ServiceProviderBootstrapper ConfigureServices(Action<IServiceCollection> config)
        {
            _config += config;
            return this;
        }

        public IServiceProvider BuildServiceProvider()
        {
            IServiceProvider result;

            if (_startupType == null)
            {
                var services = new ServiceCollection();

                    _config(services);

                    result = services.BuildServiceProvider();
            }
            else
            {
                // ReSharper disable once PossibleNullReferenceException
                result = (IServiceProvider) typeof(ServiceProviderBootstrapper)
                    .GetMethod(nameof(BuildServiceProvider),
                        BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetGenericMethodDefinition().MakeGenericMethod(_startupType).Invoke(this, new object[0]);
            }

            return result;
        }

        IServiceProvider BuildServiceProvider<TStartup>() where TStartup : class
            => WebHostBuilderFactory
                .CreateFromTypesAssemblyEntryPoint<TStartup>(new string[0])
                .ConfigureServices(_config)
                .Build()
                .Services;

        public static void Initialize(IServiceProvider services) =>
            ScenarioBase.ContainerFactory = () => new ServiceProviderContainer(services);
    }
}