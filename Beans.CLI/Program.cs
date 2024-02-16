using System.Text;
using BeanJuice.Model.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Beans.CLI;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        if (args.Length == 0) return -1;
        var gitlabToken = args[0];

        Defaults.ConfigureDefaultServices(
            gitlabToken,
            services => services.AddLogging(
                lb => lb
                    .AddConsole()
                    .AddSimpleConsole(o => o.IncludeScopes = true)
                    .SetMinimumLevel(LogLevel.Trace)
                    .AddFilter("System.Net.Http.HttpClient.Refit.Implementation", LogLevel.Warning)
            )
        );

        var gitlab = Defaults.Locator.GetRequiredService<IGitlabService>();
        Console.WriteLine("pipe\tjob\tcommit  \tdate                \tname");
        await foreach (var job in gitlab.ListProjectJobsSuccessful(2, 150))
        {
            var sb = new StringBuilder();
            if (job.Name != null && !job.Name.StartsWith("deploy live")) continue;

            // job.FinishedAt.Value.ToLocalTime();

            sb.Append(job.Pipeline?.Id ?? -1)
                .Append('\t')
                .Append(job.Id)
                .Append('\t')
                .Append(job.Commit?.ShortId ?? "n/a")
                .Append('\t')
                .Append(job.FinishedAt?.ToLocalTime())
                .Append('\t')
                .Append(job.Name)
                .AppendLine();

            Console.Write(sb.ToString());
        }

        return 0;
    }
}
