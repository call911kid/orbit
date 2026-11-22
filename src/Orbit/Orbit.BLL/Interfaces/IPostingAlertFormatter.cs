using Orbit.Providers.Contracts;

namespace Orbit.BLL.Interfaces;

public interface IPostingAlertFormatter
{
    string Format(ExternalPosting posting);
}
