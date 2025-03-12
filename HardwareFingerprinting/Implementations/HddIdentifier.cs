using System;
using System.IO;
using System.Text;
using HardwareFingerprinting.Interfaces;
using HardwareFingerprinting.Utils;

namespace HardwareFingerprinting.Implementations
{
    /// <summary>
    /// Implementation of the HDD hardware identifier
    /// </summary>
    public class HddIdentifier : IHardwareIdentifier
    {
        /// <summary>
        /// Gets the type of hardware component
        /// </summary>
        public HardwareComponentType ComponentType => HardwareComponentType.Hdd;

        /// <summary>
        /// Gets the identifier for this hardware component
        /// </summary>
        /// <returns>A byte array containing the hardware identifier</returns>
        public byte[] GetIdentifier()
        {
            try
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Unix:
                        return GetUnixHddIdentifier();
                    default:
                        return GetWindowsHddIdentifier();
                }
            }
            catch (Exception)
            {
                return Array.Empty<byte>();
            }
        }

        /// <summary>
        /// Gets the HDD identifier for Unix-based systems
        /// </summary>
        /// <returns>A byte array containing the HDD identifier</returns>
        private byte[] GetUnixHddIdentifier()
        {
            var rootUuids = new DirectoryInfo("/dev/disk/by-uuid");
            if (!rootUuids.Exists)
                return Array.Empty<byte>();

            var uuids = new StringBuilder();
            foreach (var f in rootUuids.GetFiles("*"))
            {
                uuids.Append(f.Name);
            }

            if (uuids.Length > 0)
                return Encoding.UTF8.GetBytes(uuids.ToString());

            return Array.Empty<byte>();
        }

        /// <summary>
        /// Gets the HDD identifier for Windows-based systems
        /// </summary>
        /// <returns>A byte array containing the HDD identifier</returns>
        private byte[] GetWindowsHddIdentifier()
        {
            var driveLetter = Path.GetPathRoot(Environment.SystemDirectory);
            uint serialNumber = 0;
            uint maxComponentLength = 0;
            uint fileSystemFlags = 0;

            if (Win32.GetVolumeInformation(driveLetter, null, 0, ref serialNumber,
                ref maxComponentLength, ref fileSystemFlags, null, 0) && serialNumber != 0)
            {
                return BitConverter.GetBytes(serialNumber);
            }

            return Array.Empty<byte>();
        }
    }
}