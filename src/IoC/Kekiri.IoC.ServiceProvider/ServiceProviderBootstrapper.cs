using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Kekiri.IoC
{
    public partial class ServiceProviderBootstrapper
    {
        Type _startupType;
        Action<IServiceCollection> _config;
        Action<IServiceCollection> _postStartupServicesConfig;
        Action<object> _postStartupContainerBuilderConfig;
        Type _containerType;

        public ServiceProviderBootstrapper()
        {
            _config = x => { };
            _postStartupServicesConfig = x => { };
            _postStartupContainerBuilderConfig = x => { };
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
                ServiceDescriptorsForConcreteTypes(typeof(T).Assembly)
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
                    _postStartupServicesConfig += config;
                }
                else
                {
                    x.AddSingleton<IStartupConfigureServicesFilter>(y =>
                        new PostStartupConfigureServicesFilter(config));
                }
            });

        public ServiceProviderBootstrapper OverrideServicesWithTypesFromAssemblyUsingContainerBuilder<TContainerBuilder>(
            Assembly assembly,
            Action<TContainerBuilder, ServiceDescriptor> serviceRegistrationDelegate) 
            => ConfigureServicesPostStartup<TContainerBuilder>(x =>
                ServiceDescriptorsForConcreteTypes(assembly)
                    .Aggregate(x, (s, y) => { serviceRegistrationDelegate.Invoke(s, y); return s; }));

        public ServiceProviderBootstrapper OverrideServicesWithTypesFromAssemblyUsingContainerBuilder<TContainerBuilder>(
            Assembly assembly,
            Action<TContainerBuilder, IEnumerable<ServiceDescriptor>> serviceRegistrationDelegate)
            => ConfigureServicesPostStartup<TContainerBuilder>(x =>
                serviceRegistrationDelegate(x, ServiceDescriptorsForConcreteTypes(assembly)));

        static IEnumerable<ServiceDescriptor> ServiceDescriptorsForConcreteTypes(Assembly assembly)
            => assembly.GetTypes()
                .Where(y => !y.IsAbstract && !y.IsInterface)
                .SelectMany(ServiceDescriptors);

        public ServiceProviderBootstrapper ConfigureServicesPostStartup<TContainerBuilder>(
            Action<TContainerBuilder> config)
        {
            _containerType = typeof(TContainerBuilder);

            return ConfigureServices(x =>
            {
                if (_startupType == null)
                {
                    _postStartupContainerBuilderConfig += y => config((TContainerBuilder) y);
                }
                else
                {
                    x.AddSingleton<IStartupConfigureContainerFilter<TContainerBuilder>>(y =>
                        new PostStartupConfigureContainerFilter<TContainerBuilder>(config));
                }
            });
        }

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

                _postStartupServicesConfig(services);

                result = services.BuildServiceProvider();

                if (_containerType != null)
                {
                    result = BuildServiceProvider(result, services);
                }
            }
            else
            {
                // ReSharper disable once PossibleNullReferenceException
                result = (IServiceProvider) typeof(ServiceProviderBootstrapper)
                    .GetMethod(
                        nameof(BuildServiceProvider),
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        binder: null,
                        types: new Type[0],
                        modifiers: null)
                    .GetGenericMethodDefinition().MakeGenericMethod(_startupType).Invoke(this, new object[0]);
            }

            return result;
        }

        /// <summary>
        /// Builds an instance of <see cref="IServiceProvider"/> from an instance of <see cref="IServiceProviderFactory{TContainerBuilder}"/>.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        IServiceProvider BuildServiceProvider(IServiceProvider result, ServiceCollection services)
        {
            var spfType = typeof(IServiceProviderFactory<>).MakeGenericType(_containerType);

            var spf = result.GetRequiredService(spfType);

            var containerBuilderFactory = spf.GetType().GetMethod(nameof(IServiceProviderFactory<object>.CreateBuilder));

            // ReSharper disable once PossibleNullReferenceException
            var containerBuilder = containerBuilderFactory.Invoke(spf, new object[] { services });

            _postStartupContainerBuilderConfig.Invoke(containerBuilder);

            var serviceProviderFactory = spf.GetType()
                .GetMethod(nameof(IServiceProviderFactory<object>.CreateServiceProvider));

            // ReSharper disable once PossibleNullReferenceException
            return (IServiceProvider) serviceProviderFactory.Invoke(spf, new[] { containerBuilder });
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
