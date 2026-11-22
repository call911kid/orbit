using Orbit.Common.Enums;

namespace Orbit.Providers.Contracts;

public interface IPostingProvider
{
    PostingSource Source { get; }
    Task<IReadOnlyList<ExternalPosting>> FetchPostingsAsync(PostingSearchCriteria criteria, CancellationToken cancellationToken = default);
}
