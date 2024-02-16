using System;
using BeanJuice.Model.Json;
using BeanJuice.Model.Json.Gitlab;
using BeanJuice.Model.Services;
using BeanJuice.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using HttpSpy;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Beans;

public static class Defaults
{
    public const int GitlabProjectId = 2;

    /// Never fetch more than this many pages at once
    public const int GitlabApiPageMax = 50;

    /// Records requested per page (server max is documented as 100)
    public const int GitlabApiPageSize = 100;

    public static Ioc Locator = Ioc.Default;

    public static IServiceProvider ConfigureDefaultServices(
        string gitlabToken,
        Action<IServiceCollection>? postConfig = null
    )
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton<IGitlabAuthTokenStore>(new GitlabAuthStaticTokenStore(gitlabToken));
        services.AddTransient<GitlabAuthHeaderHandler>();
        services.AddTransient<IGitlabService, GitlabService>();
        services.AddTransient<HttpSpyHandler>();

        services.AddHttpClient<IGitLabApi, GitlabApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://gitlab.eikongroup.io/api/v4"))
            .AddHttpMessageHandler<GitlabAuthHeaderHandler>()
            .AddHttpMessageHandler<HttpSpyHandler>();
        // services.AddRefitClient<IGitLabApi>(
        //         new RefitSettings
        //         {
        //             ContentSerializer =
        //                 new SystemTextJsonContentSerializerForSourceGenerator(GitlabJsonContext.Default)
        //         }
        //     )
        //     .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://gitlab.eikongroup.io/api/v4"))
        //     .AddHttpMessageHandler<GitlabAuthHeaderHandler>();

        postConfig?.Invoke(services);

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        Locator.ConfigureServices(serviceProvider);
        return serviceProvider;
    }
}
