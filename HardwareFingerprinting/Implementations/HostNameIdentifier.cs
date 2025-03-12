using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using HardwareFingerprinting.Interfaces;

namespace HardwareFingerprinting.Implementations
{
    /// <summary>
    /// Implementation of the host name hardware identifier
    /// </summary>
    public class HostNameIdentifier : IHardwareIdentifier
    {
        /// <summary>
        /// Gets the type of hardware component
        /// </summary>
        public HardwareComponentType ComponentType => HardwareComponentType.Host;

        /// <summary>
        /// Gets the identifier for this hardware component
        /// </summary>
        /// <returns>A byte array containing the hardware identifier</returns>
        public byte[] GetIdentifier()
        {
            try
            {
                var hostName = Dns.GetHostName().ToUpperInvariant();
                return Encoding.Unicode.GetBytes(hostName);
            }
            catch (SocketException)
            {
                // Return empty array if host name cannot be retrieved
                return Array.Empty<byte>();
            }
        }
    }
}