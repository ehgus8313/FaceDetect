using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace APR_TEST.Utils
{
    public static class CameraHelper
    {
        public static List<string> GetConnectedCameras()
        {
            var result = new List<string>();

            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%(usb%'");
            foreach (var device in searcher.Get())
            {
                if (device["Caption"] != null)
                    result.Add(device["Caption"].ToString());
            }

            return result;
        }
    }
}
