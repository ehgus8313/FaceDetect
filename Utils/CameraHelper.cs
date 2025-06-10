using DirectShowLib;

using OpenCvSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APR_TEST.Utils
{
    public static class CameraHelper
    {
        public static List<string> GetVideoInputDeviceNames()
        {
            var devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            return devices.Select(d => d.Name).ToList();
        }

        public static int? GetCameraIndexByName(string targetName)
        {
            var devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            for (int i = 0; i < devices.Length; i++)
            {
                if (devices[i].Name.Contains(targetName, StringComparison.OrdinalIgnoreCase))
                {
                    using var cap = new VideoCapture(i);
                    if (cap.IsOpened())
                        return i;
                }
            }
            return null;
        }
    }
}
