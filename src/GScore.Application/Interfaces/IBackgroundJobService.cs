using System.Linq.Expressions;

namespace GScore.Application.Interfaces;

public interface IBackgroundJobService
{
    string Enqueue<T>(Expression<Func<T, Task>> methodCall);
}
