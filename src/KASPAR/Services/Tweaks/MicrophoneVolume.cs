using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace KASPAR.Services.Tweaks;

/// <summary>Sets the default capture device's volume and unmutes it (replaces the pycaw-based Python scripts).</summary>
internal static partial class MicrophoneVolume
{
    private const int ERole_Multimedia = 1;
    private const int EDataFlow_Capture = 1;
    private const uint CLSCTX_ALL = 23;
    private const int COINIT_MULTITHREADED = 0;

    private static readonly Guid MMDeviceEnumeratorClsid = new("BCDE0395-E52F-467C-8E3D-C4579291692E");
    private static readonly Guid AudioEndpointVolumeIid = new("5CDF2C82-841E-4546-9722-0CF74078229A");

    public static void Set(float level)
    {
        // Callers run on a thread-pool thread; make sure COM is up on it. S_OK/S_FALSE need a matching uninit,
        // RPC_E_CHANGED_MODE (thread already initialised differently) does not.
        var hrInit = CoInitializeEx(0, COINIT_MULTITHREADED);
        try
        {
            var comWrappers = new StrategyBasedComWrappers();

            Marshal.ThrowExceptionForHR(CoCreateInstance(
                MMDeviceEnumeratorClsid, 0, CLSCTX_ALL, typeof(IMMDeviceEnumerator).GUID, out var enumeratorPtr));
            IMMDeviceEnumerator enumerator;
            try
            {
                enumerator = (IMMDeviceEnumerator)comWrappers.GetOrCreateObjectForComInstance(enumeratorPtr, CreateObjectFlags.None);
            }
            finally
            {
                Marshal.Release(enumeratorPtr);
            }

            enumerator.GetDefaultAudioEndpoint(EDataFlow_Capture, ERole_Multimedia, out var device);

            var iid = AudioEndpointVolumeIid;
            device.Activate(in iid, CLSCTX_ALL, 0, out var volumePtr);
            IAudioEndpointVolume volume;
            try
            {
                volume = (IAudioEndpointVolume)comWrappers.GetOrCreateObjectForComInstance(volumePtr, CreateObjectFlags.None);
            }
            finally
            {
                Marshal.Release(volumePtr);
            }

            volume.SetMasterVolumeLevelScalar(level, 0);
            volume.SetMute(false, 0);
        }
        finally
        {
            if (hrInit >= 0)
            {
                CoUninitialize();
            }
        }
    }

    [LibraryImport("ole32.dll")]
    private static partial int CoInitializeEx(nint pvReserved, int dwCoInit);

    [LibraryImport("ole32.dll")]
    private static partial void CoUninitialize();

    [LibraryImport("ole32.dll")]
    private static partial int CoCreateInstance(in Guid rclsid, nint pUnkOuter, uint dwClsContext, in Guid riid, out nint ppv);

    // Only the vtable prefix we need is declared for the first two interfaces; vtable order must match mmdeviceapi.h.

    [GeneratedComInterface]
    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
    internal partial interface IMMDeviceEnumerator
    {
        void EnumAudioEndpoints(int dataFlow, uint stateMask, out nint devices);
        void GetDefaultAudioEndpoint(int dataFlow, int role, out IMMDevice device);
    }

    [GeneratedComInterface]
    [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
    internal partial interface IMMDevice
    {
        void Activate(in Guid iid, uint clsCtx, nint activationParams, out nint iface);
    }

    // endpointvolume.h - every method up to SetMute must be present, in order. The event-context GUID* args are
    // always null here, so they are typed as nint.
    [GeneratedComInterface]
    [Guid("5CDF2C82-841E-4546-9722-0CF74078229A")]
    internal partial interface IAudioEndpointVolume
    {
        void RegisterControlChangeNotify(nint notify);
        void UnregisterControlChangeNotify(nint notify);
        void GetChannelCount(out uint channelCount);
        void SetMasterVolumeLevel(float levelDb, nint eventContext);
        void SetMasterVolumeLevelScalar(float level, nint eventContext);
        void GetMasterVolumeLevel(out float levelDb);
        void GetMasterVolumeLevelScalar(out float level);
        void SetChannelVolumeLevel(uint channel, float levelDb, nint eventContext);
        void SetChannelVolumeLevelScalar(uint channel, float level, nint eventContext);
        void GetChannelVolumeLevel(uint channel, out float levelDb);
        void GetChannelVolumeLevelScalar(uint channel, out float level);
        void SetMute([MarshalAs(UnmanagedType.Bool)] bool mute, nint eventContext);
    }
}
