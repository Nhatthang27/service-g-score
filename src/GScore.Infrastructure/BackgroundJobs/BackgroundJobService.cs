using System.Linq.Expressions;
using Hangfire;
using GScore.Application.Interfaces;

namespace GScore.Infrastructure.BackgroundJobs;

public class BackgroundJobService : IBackgroundJobService
{
    public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }
}
