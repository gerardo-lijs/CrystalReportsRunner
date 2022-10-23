#pragma warning disable IDE1006 // Naming Styles

namespace LijsDev.CrystalReportsRunner;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal static class RuntimePolicyHelper
{
    public static bool LegacyV2Runtime_IsEnabled { get; private set; }

    public static void LegacyV2Runtime_Enable()
    {
        // Check already enabled
        if (LegacyV2Runtime_IsEnabled) return;

        var clrRuntimeInfo =
            (ICLRRuntimeInfo)RuntimeEnvironment.GetRuntimeInterfaceAsObject(
                Guid.Empty,
                typeof(ICLRRuntimeInfo).GUID);
        try
        {
            clrRuntimeInfo.BindAsLegacyV2Runtime();
            LegacyV2Runtime_IsEnabled = true;
        }
        catch (COMException ex)
        {
            // This occurs with an HRESULT meaning 
            // "A different runtime was already bound to the legacy CLR version 2 activation policy."
            throw new Exception("Could not enable LegacyV2Runtime. A different runtime was already bound to the legacy CLR version 2 activation policy.", ex);
        }
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("BD39D1D2-BA2F-486A-89B0-B4B0CB466891")]
    private interface ICLRRuntimeInfo
    {
        void xGetVersionString();
        void xGetRuntimeDirectory();
        void xIsLoaded();
        void xIsLoadable();
        void xLoadErrorString();
        void xLoadLibrary();
        void xGetProcAddress();
        void xGetInterface();
        void xSetDefaultStartupFlags();
        void xGetDefaultStartupFlags();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void BindAsLegacyV2Runtime();
    }
}
