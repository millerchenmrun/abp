﻿using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Volo.Abp.AspNetCore.Components.WebAssembly.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc.Client;
using Volo.Abp.DynamicProxy;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.UI;

namespace Volo.Abp.AspNetCore.Components.WebAssembly
{
    [DependsOn(
        typeof(AbpAspNetCoreMvcClientCommonModule),
        typeof(AbpUiModule)
        )]
    public class AbpAspNetCoreComponentsWebAssemblyModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            DynamicProxyIgnoreTypes.Add<ComponentBase>();

            context.Services.AddConventionalRegistrar(new AbpWebAssemblyConventionalRegistrar());

            PreConfigure<AbpHttpClientBuilderOptions>(options =>
            {
                options.ProxyClientBuildActions.Add((_, builder) =>
                {
                    builder.AddHttpMessageHandler<AbpBlazorClientHttpMessageHandler>();
                });
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Replace(ServiceDescriptor.Transient<IComponentActivator, ServiceProviderComponentActivator>());

            context.Services
                .GetHostBuilder().Logging
                .AddProvider(new AbpExceptionHandlingLoggerProvider(context.Services));
        }
    }
}
