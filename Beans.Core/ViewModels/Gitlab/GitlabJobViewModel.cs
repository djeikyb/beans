using BeanJuice.Model.Json.Gitlab;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Beans.ViewModels.Gitlab;

public class GitlabJobViewModel : ObservableObject
{
    private readonly Job _job;

    public GitlabJobViewModel()
    {
        _job = new Job { Pipeline = new JobPipeline() };
    }

    public GitlabJobViewModel(Job job)
    {
        _job = job;
    }

    public string? Name
    {
        get => _job.Name;
        set => SetProperty(_job.Name, value, _job, (u, n) => u.Name = n);
    }

    public int? PipelineId
    {
        get => _job.Pipeline?.Id;
        set => SetProperty(
            _job.Pipeline?.Id,
            value,
            _job,
            (u, n) =>
            {
                if (u.Pipeline != null) u.Pipeline.Id = n;
            }
        );
    }

    public decimal? Duration
    {
        get => _job.Duration;
        set => SetProperty(_job.Duration, value, _job, (u, n) => u.Duration = n);
    }
}
