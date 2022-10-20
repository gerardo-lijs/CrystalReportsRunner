namespace LijsDev.CrystalReportsRunner.Core;

using System;

using Newtonsoft.Json;

public class WindowHandle
{
    [JsonConstructor]
    public WindowHandle(IntPtr handle)
    {
        HandleInternal = handle.ToInt64();
    }

    private long HandleInternal { get; }
    public IntPtr Handle => new(HandleInternal);
}
