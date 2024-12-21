using SharpDX.DirectInput;

namespace dcsInputBackup;

public class GameControllers
{
    private readonly DirectInput _directInput = new();
    private readonly Dictionary<string, DeviceInstance> _knownDevices;

    public GameControllers()
    {
        _knownDevices = DetectIds();
    }

    private Dictionary<string, DeviceInstance> DetectIds()
    {
        var result = new Dictionary<string, DeviceInstance>();
        foreach (var device in _directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AllDevices))
        {
            result[device.ProductName] = device;
        }

        return result;
    }

    public Dictionary<string, DeviceInstance> Devices()
    {
        return _knownDevices;
    }
}