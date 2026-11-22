using Orbit.Common.Enums;

namespace Orbit.Providers.Contracts;

public record ExternalPosting(
    string ExternalId,
    string Title,
    string Company,
    string? CompanyIdentifier,
    string? Location,
    string Url,
    string? PostedText,
    DateTime? PostedAt,
    PostingSource Source,
    string? DescriptionSnippet = null
);
