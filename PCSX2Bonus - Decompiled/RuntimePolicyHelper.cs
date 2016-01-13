using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public static class RuntimePolicyHelper
{
    static RuntimePolicyHelper()
    {
        ICLRRuntimeInfo runtimeInterfaceAsObject = (ICLRRuntimeInfo) RuntimeEnvironment.GetRuntimeInterfaceAsObject(Guid.Empty, typeof(ICLRRuntimeInfo).GUID);
        try
        {
            runtimeInterfaceAsObject.BindAsLegacyV2Runtime();
            LegacyV2RuntimeEnabledSuccessfully = true;
        }
        catch (COMException exception)
        {
            Console.WriteLine(exception.Message);
            LegacyV2RuntimeEnabledSuccessfully = false;
        }
    }

    public static bool LegacyV2RuntimeEnabledSuccessfully
    {
        [CompilerGenerated]
        get
        {
            return <LegacyV2RuntimeEnabledSuccessfully>k__BackingField;
        }
        [CompilerGenerated]
        private set
        {
            <LegacyV2RuntimeEnabledSuccessfully>k__BackingField = value;
        }
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("BD39D1D2-BA2F-486A-89B0-B4B0CB466891")]
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
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void BindAsLegacyV2Runtime();
    }
}

