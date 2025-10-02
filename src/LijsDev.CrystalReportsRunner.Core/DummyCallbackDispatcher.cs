namespace LijsDev.CrystalReportsRunner.Core;

using System.Data;
using System.Diagnostics;

/// <summary>
/// Dummy implementation of ICallbackDispatcher. Does Nothing!
/// </summary>
public class DummyCallbackDispatcher : ICallbackDispatcher
{
    /// <inheritdoc/>
    public bool TryInvokeCallbackFromGuid(DataTable sender, Guid guid)
    {
        Debug.WriteLine($"Callback Dispatcher called with GUID: {guid}");
        return true;
    }
}
