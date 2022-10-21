namespace LijsDev.CrystalReportsRunner.Core;

using System;

using Newtonsoft.Json;

// NB: We define our own WindowHandle class to avoid a dependency on either WinForms or WPF and keep the core interface more abstract.

/// <summary>
/// Generic window handle to a WinForms Form or WPF Window in the caller application.
/// </summary>
[Serializable]
public sealed class WindowHandle
{
    /// <inheritdoc/>
    [JsonConstructor]
    public WindowHandle(IntPtr handle)
    {
        HandleInternal = handle.ToInt64();
    }

    private long HandleInternal { get; }

    /// <inheritdoc/>
    public IntPtr Handle => new(HandleInternal);
}
