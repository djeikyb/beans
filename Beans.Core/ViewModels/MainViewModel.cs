using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using BeanJuice.Model.Services;
using Beans.ViewModels.Gitlab;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nito.AsyncEx;

namespace Beans.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private ObservableCollection<GitlabJobViewModel> _jobs;
    private readonly AsyncLock _fetchLock = new();

    [ObservableProperty]
    private bool _topmost;

    [ObservableProperty]
    private string? _theme;

    [ObservableProperty]
    private string? _gitlabToken;

    [ObservableProperty]
    private bool? _revealGitlabToken;

    public MainViewModel()
    {
        Topmost = true;
        _jobs = new ObservableCollection<GitlabJobViewModel>();
        _revealGitlabToken = false;
    }

    partial void OnGitlabTokenChanged(string? value)
    {
        if (value is not null)
            Defaults.Locator
                .GetRequiredService<IGitlabAuthTokenStore>()
                .SetToken(value);
    }

    public ObservableCollection<GitlabJobViewModel> Jobs
    {
        get => _jobs;
        set => SetProperty(ref _jobs, value);
    }

    [RelayCommand]
    private void ClickyClick() => Console.WriteLine("clickedy cleck");

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task FetchJobsAsync(CancellationToken ct)
    {
        using (await _fetchLock.LockAsync())
        {
            try
            {
                // _cts = new CancellationTokenSource();
                // var ct = _cts.Token;
                var gitlab = Defaults.Locator.GetService<IGitlabService>(); // can this be constructor injected?
                if (gitlab is null) return; // TODO why guard? why not required?
                //                             Maybe for hoisting a.. view? in some kind
                //                             of design-time something or other?
                var foundLiveDeployJobs = gitlab.ListLiveDeployJobs(
                    Defaults.GitlabProjectId,
                    Defaults.GitlabApiPageMax,
                    ct
                );

                Jobs.Clear();
                await foreach (var job in foundLiveDeployJobs)
                {
                    if (ct.IsCancellationRequested) break;
                    Jobs.Add(new GitlabJobViewModel(job));
                }
            }
            catch (OperationCanceledException) { } // do nothing
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
