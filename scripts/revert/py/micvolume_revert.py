from ctypes import cast, POINTER
from comtypes import CLSCTX_ALL
from pycaw.pycaw import AudioUtilities, IAudioEndpointVolume
from pycaw.pycaw import EDataFlow, ERole

mic = AudioUtilities.GetMicrophone()
interface = mic.Activate(IAudioEndpointVolume._iid_, CLSCTX_ALL, None)
volume = cast(interface, POINTER(IAudioEndpointVolume))

volume.SetMasterVolumeLevelScalar(0.0, None)
volume.SetMute(0, None)