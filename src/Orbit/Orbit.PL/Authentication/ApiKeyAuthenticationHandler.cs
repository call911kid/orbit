using System.Security.Claims;
using System.Text.Encodings.Web;
using MediatR;
using Orbit.BLL.Queries.ApiKeys;
using Orbit.Common.Enums;
using Orbit.PL.Configuration;
using Orbit.PL.Constants;
using Orbit.PL.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Orbit.PL.Authentication;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ApiKey";
    public const string HeaderName = "X-Api-Key";
}

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly ApiAuthenticationOptions _authOptions;
    private readonly ISender _sender;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IOptions<ApiAuthenticationOptions> authOptions,
        ISender sender)
        : base(options, logger, encoder)
    {
        _authOptions = authOptions.Value;
        _sender = sender;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.HeaderName, out var extractedApiKey) ||
            string.IsNullOrWhiteSpace(extractedApiKey))
        {
            return AuthenticateResult.NoResult();
        }

        var incomingKey = extractedApiKey.ToString();

        // 1. Check Master Admin Key from configuration
        var masterKey = _authOptions.ApiKey;
        if (!string.IsNullOrEmpty(masterKey) && string.Equals(masterKey, incomingKey))
        {
            var adminClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Roles.Admin),
                new Claim(ClaimTypes.Name, Roles.Admin),
                new Claim(ClaimTypes.Role, Roles.Admin)
            };
            var adminIdentity = new ClaimsIdentity(adminClaims, Scheme.Name);
            var adminPrincipal = new ClaimsPrincipal(adminIdentity);
            return AuthenticateResult.Success(new AuthenticationTicket(adminPrincipal, Scheme.Name));
        }

        // 2. Check Database-backed API Keys
        var (isValid, keyId, name, role) = await _sender.Send(new ValidateApiKeyQuery(incomingKey));
        if (isValid)
        {
            var callerClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, keyId.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role.ToString())
            };
            var callerIdentity = new ClaimsIdentity(callerClaims, Scheme.Name);
            var callerPrincipal = new ClaimsPrincipal(callerIdentity);
            return AuthenticateResult.Success(new AuthenticationTicket(callerPrincipal, Scheme.Name));
        }

        return AuthenticateResult.Fail("Invalid API key.");
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = "application/json";
        await Response.WriteAsJsonAsync(ApiResponse.Failure(
            ErrorCodes.Unauthorized,
            "Invalid or missing API key.",
            new List<string>()));
    }
}
