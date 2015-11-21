namespace Microsoft.Iot.Extended.Hid
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Windows.Devices.Enumeration;
    using Windows.Devices.HumanInterfaceDevice;

    public class HidService
    {
        public static async Task<IList<HidDevice>> Discover(ushort page, ushort id)
        {
            var deviceSelector = HidDevice.GetDeviceSelector(page, id);

            var deviceInformationCollection = await DeviceInformation.FindAllAsync(deviceSelector);

            if (deviceInformationCollection.Count == 0)
            {
                return null;
            }

            var result = new List<HidDevice>();

            foreach (var d in deviceInformationCollection)
            {
                var hidDevice = await HidDevice.FromIdAsync(d.Id, Windows.Storage.FileAccessMode.Read);

                if (hidDevice == null)
                {
                    try
                    {
                        var deviceAccessStatus = DeviceAccessInformation.CreateFromId(d.Id).CurrentStatus;
                        if (!deviceAccessStatus.Equals(DeviceAccessStatus.Allowed))
                        {
                            Debug.WriteLine("DeviceAccess: " + deviceAccessStatus);
                           // FoundLocalControlsWorking = true;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Xbox init - " + e.Message);
                    }
                }

                result.Add(hidDevice);
            }

            return null;
        }
    }
}
