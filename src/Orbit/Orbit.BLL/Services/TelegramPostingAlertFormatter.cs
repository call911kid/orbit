using System.Net;
using System.Text;
using Orbit.BLL.Interfaces;
using Orbit.Providers.Contracts;

namespace Orbit.BLL.Services;

public class TelegramPostingAlertFormatter : IPostingAlertFormatter
{
    public string Format(ExternalPosting posting)
    {
        var sb = new StringBuilder();

        sb.AppendLine(WebUtility.HtmlEncode(posting.Title));
        sb.AppendLine($"🏢 {WebUtility.HtmlEncode(posting.Company)}");

        if (!string.IsNullOrWhiteSpace(posting.Location))
        {
            sb.AppendLine($"📍 {WebUtility.HtmlEncode(posting.Location)}");
        }

        sb.AppendLine($"🌐 {posting.Source}");

        if (!string.IsNullOrWhiteSpace(posting.PostedText))
        {
            sb.AppendLine($"🕐 {WebUtility.HtmlEncode(posting.PostedText)}");
        }

        sb.AppendLine();
        sb.AppendLine($"🔗 {posting.Url}");

        return sb.ToString().TrimEnd();
    }
}
