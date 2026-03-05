using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace SwiggyClone.UnitTests.Common;

/// <summary>
/// IHttpResponseFeature that tracks and fires OnStarting callbacks.
/// DefaultHttpResponseFeature in test contexts does not fire them through StartAsync.
/// </summary>
public sealed class TestHttpResponseFeature : IHttpResponseFeature
{
    private readonly List<(Func<object, Task> Callback, object State)> _onStarting = [];

    public int StatusCode { get; set; } = 200;
    public string? ReasonPhrase { get; set; }
    public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();
    public Stream Body { get; set; } = Stream.Null;
    public bool HasStarted { get; private set; }

    public void OnStarting(Func<object, Task> callback, object state)
    {
        _onStarting.Add((callback, state));
    }

    public void OnCompleted(Func<object, Task> callback, object state) { }

    /// <summary>
    /// Manually fires all registered OnStarting callbacks (in LIFO order, matching ASP.NET behavior).
    /// Call this in tests after invoking middleware to trigger response header injection.
    /// </summary>
    public async Task FireOnStartingAsync()
    {
        HasStarted = true;
        for (var i = _onStarting.Count - 1; i >= 0; i--)
        {
            await _onStarting[i].Callback(_onStarting[i].State);
        }
    }
}
