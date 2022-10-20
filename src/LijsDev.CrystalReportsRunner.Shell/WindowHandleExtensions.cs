namespace LijsDev.CrystalReportsRunner.Shell;

using LijsDev.CrystalReportsRunner.Core;

using System;
using System.Windows.Forms;

public static class WindowHandleExtensions
{
    public class Win32Window : IWin32Window
    {
        public IntPtr Handle { get; }

        public Win32Window(IntPtr handle)
        {
            Handle = handle;
        }
    }

    public static IWin32Window GetWindow(this WindowHandle handle)
    {
        return new Win32Window(handle.Handle);
    }
}
