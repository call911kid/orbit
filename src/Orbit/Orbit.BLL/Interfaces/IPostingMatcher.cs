using Orbit.Common.Enums;

namespace Orbit.BLL.Interfaces;

public interface IPostingMatcher
{
    bool IsMatch(string postingTitle, IReadOnlyList<string> targetTitles, ExperienceLevel allowedExperience);
    bool MatchesExperience(string postingTitle, IReadOnlyList<string> targetTitles, ExperienceLevel allowedExperience);
    bool MatchesTitle(string postingTitle, IReadOnlyList<string> targetTitles);
}
