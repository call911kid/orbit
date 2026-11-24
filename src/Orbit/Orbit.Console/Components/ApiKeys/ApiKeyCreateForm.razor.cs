using Microsoft.AspNetCore.Components;
using Orbit.Common.Enums;

namespace Orbit.Console.Components.ApiKeys;

public sealed record CreateApiKeyFormSubmit(string Name, Role Role, int ValidForMinutes);

public partial class ApiKeyCreateForm : ComponentBase
{
    private static readonly IReadOnlyList<Role> AvailableRoles = Enum.GetValues<Role>()
        .Where(r => r != Role.Admin)
        .OrderByDescending(r => r)
        .ToArray();

    [Parameter] public bool IsBusy { get; set; }
    [Parameter] public EventCallback<CreateApiKeyFormSubmit> OnCreate { get; set; }

    private string keyName = string.Empty;
    private Role selectedRole = Role.Agent;
    private string selectedExpiryOption = "1440";
    private bool isCustomHours;
    private int? customHours;
    private string? validationError;

    private void HandleExpiryOptionChanged(ChangeEventArgs e)
    {
        var value = e.Value?.ToString() ?? "1440";
        validationError = null;

        if (value == "custom")
        {
            isCustomHours = true;
            customHours = null;
        }
        else
        {
            isCustomHours = false;
            selectedExpiryOption = value;
        }
    }

    private void ResetToPresets()
    {
        isCustomHours = false;
        selectedExpiryOption = "1440";
        customHours = null;
        validationError = null;
    }

    private async Task HandleSubmitAsync()
    {
        if (IsBusy) return;

        validationError = null;
        var trimmedName = keyName.Trim();
        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            validationError = "Key name is required.";
            return;
        }

        int validForMinutes;
        if (isCustomHours)
        {
            if (!customHours.HasValue || customHours.Value <= 0)
            {
                validationError = "Enter a valid number of hours greater than 0.";
                return;
            }

            validForMinutes = customHours.Value * 60;
        }
        else
        {
            validForMinutes = int.TryParse(selectedExpiryOption, out var parsed) && parsed > 0
                ? parsed
                : 1440;
        }

        await OnCreate.InvokeAsync(new CreateApiKeyFormSubmit(trimmedName, selectedRole, validForMinutes));
        keyName = string.Empty;
    }
}
