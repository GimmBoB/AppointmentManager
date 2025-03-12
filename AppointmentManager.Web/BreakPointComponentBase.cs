using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Services;

namespace AppointmentManager.Web;

public abstract class BreakpointComponentBase : ComponentBase, IAsyncDisposable
{
#pragma warning disable CS0618
    [Inject] private IBreakpointService BreakpointListener { get; set; }
#pragma warning restore CS0618

    private Guid _subscriptionId;
    
#pragma warning disable CS8618
    protected BreakpointComponentBase() {}
#pragma warning restore CS8618

    protected bool IsSmartphoneView => CurrentBreakpoint.Equals(Breakpoint.Xs);
    protected bool IsMobileView => CurrentBreakpoint.Equals(Breakpoint.Xs) || CurrentBreakpoint.Equals(Breakpoint.Sm);
   
    protected Breakpoint CurrentBreakpoint { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        var breakpoint = await BreakpointListener.GetBreakpoint();
        CurrentBreakpoint = breakpoint;
    
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetBreakPointAsync();
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task SetBreakPointAsync()
    {
        var subscriptionResult = await BreakpointListener.SubscribeAsync(breakpoint =>
        {
            CurrentBreakpoint = breakpoint;
            InvokeAsync(StateHasChanged);
        });
        _subscriptionId = subscriptionResult.SubscriptionId;
    }

    public async ValueTask DisposeAsync()
    {
        await BreakpointListener.UnsubscribeAsync(_subscriptionId);
    }
}