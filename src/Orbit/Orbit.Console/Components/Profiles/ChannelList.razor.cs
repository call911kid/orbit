using Microsoft.AspNetCore.Components;
using Orbit.Console.Services;

namespace Orbit.Console.Components.Profiles;

public partial class ChannelList : ComponentBase, IDisposable
{
    [Inject] private ConsoleState State { get; set; } = default!;

    [Parameter] public Guid? SelectedProfileId { get; set; }

    protected override void OnInitialized()
    {
        State.OnChange += HandleStateChanged;
    }

    private void OnFilterInput(ChangeEventArgs e)
    {
        State.SetSearchFilter(e.Value?.ToString() ?? string.Empty);
    }

    private void HandleStateChanged() => InvokeAsync(StateHasChanged);

    public void Dispose()
    {
        State.OnChange -= HandleStateChanged;
    }
}
