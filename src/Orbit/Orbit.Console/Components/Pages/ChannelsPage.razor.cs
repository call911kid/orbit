using Microsoft.AspNetCore.Components;
using Orbit.Console.Models;
using Orbit.Console.Services;

namespace Orbit.Console.Components.Pages;

public partial class ChannelsPage : ComponentBase, IDisposable
{
    [Inject] private ConsoleState State { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    [Parameter] public Guid? ChannelId { get; set; }

    private Guid? lastMissingChannelId;

    private SearchProfileViewModel? ActiveProfile => State.SelectedProfile;

    protected override void OnInitialized()
    {
        State.OnProfileRestored += HandleProfileRestored;
    }

    protected override void OnParametersSet()
    {
        var path = ConsoleUrls.GetNormalizedRelativePath(Navigation);

        if (path.Equals(ConsoleUrls.NewChannel.TrimStart('/'), StringComparison.OrdinalIgnoreCase))
        {
            lastMissingChannelId = null;
            State.EnsureNewProfileDraft(notify: false);
            return;
        }

        if (ChannelId.HasValue)
        {
            SelectChannelById(ChannelId.Value);
            return;
        }

        lastMissingChannelId = null;
        RedirectToFallbackChannel();
    }

    private void SelectChannelById(Guid id)
    {
        var match = State.Profiles.FirstOrDefault(p => p.Id == id);
        State.SetSelectedProfile(match);

        if (match is not null)
        {
            lastMissingChannelId = null;
            return;
        }

        if (lastMissingChannelId != id)
        {
            lastMissingChannelId = id;
            State.ShowToast("Channel not found.");
        }
    }

    private void RedirectToFallbackChannel()
    {
        var fallback = State.SelectedProfile is not null && State.Profiles.Contains(State.SelectedProfile)
            ? State.SelectedProfile
            : State.Profiles.FirstOrDefault();

        State.SetSelectedProfile(fallback);

        if (fallback is not null)
        {
            Navigation.NavigateTo(ConsoleUrls.Channel(fallback.Id), replace: true);
        }
    }

    private void HandleProfileSaved(Guid id) =>
        Navigation.NavigateTo(ConsoleUrls.Channel(id), replace: true);

    private void HandleProfileDeleted()
    {
        var nextUrl = State.SelectedProfile is not null
            ? ConsoleUrls.Channel(State.SelectedProfile.Id)
            : ConsoleUrls.Channels;
        Navigation.NavigateTo(nextUrl, replace: true);
    }

    private void HandleProfileRestored(SearchProfileViewModel profile) =>
        InvokeAsync(() => Navigation.NavigateTo(ConsoleUrls.Channel(profile.Id), replace: true));

    public void Dispose()
    {
        State.OnProfileRestored -= HandleProfileRestored;
    }
}
