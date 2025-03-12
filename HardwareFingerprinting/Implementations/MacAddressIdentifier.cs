using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using HardwareFingerprinting.Interfaces;

namespace HardwareFingerprinting.Implementations
{
    /// <summary>
    /// Implementation of the MAC address hardware identifier
    /// </summary>
    public class MacAddressIdentifier : IHardwareIdentifier
    {
        // List of known virtual machine MAC address prefixes
        private static readonly HashSet<int> VirtualMachinePrefixes = new HashSet<int>
        {
            0x000569, 0x000C29, 0x001C14, 0x005056, // VMware
            0x0003FF, 0x000D3A, 0x00125A, 0x00155D, 0x0017FA, 0x001DD8, 0x002248, 0x0025AE, 0x0050F2, // Microsoft
            0x001C42, // Parallels
            0x0021F6  // Virtual Iron
        };

        /// <summary>
        /// Gets the type of hardware component
        /// </summary>
        public HardwareComponentType ComponentType => HardwareComponentType.Mac;

        /// <summary>
        /// Gets the identifier for this hardware component
        /// </summary>
        /// <returns>A byte array containing the hardware identifier</returns>
        public byte[] GetIdentifier()
        {
            try
            {
                var physicalMacs = GetPhysicalMacAddresses();

                // If no physical MAC addresses found, return empty array
                if (!physicalMacs.Any())
                    return Array.Empty<byte>();

                // Return the first physical MAC address
                return physicalMacs.First();
            }
            catch (Exception)
            {
                return Array.Empty<byte>();
            }
        }

        /// <summary>
        /// Gets all physical MAC addresses, filtering out virtual machine MAC addresses
        /// </summary>
        /// <returns>A collection of MAC addresses as byte arrays</returns>
        private IEnumerable<byte[]> GetPhysicalMacAddresses()
        {
            var result = new List<byte[]>();

            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Skip non-Ethernet interfaces
                if (nic.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
                    continue;

                var macBytes = nic.GetPhysicalAddress().GetAddressBytes();

                // Skip invalid MAC addresses
                if (macBytes.Length < 3)
                    continue;

                // Skip virtual machine MAC addresses
                var prefix = (macBytes[0] << 16) + (macBytes[1] << 8) + macBytes[2];
                if (VirtualMachinePrefixes.Contains(prefix))
                    continue;

                result.Add(macBytes);
            }

            return result;
        }
    }
}