using System;
using System.Collections.Generic;
using HardwareFingerprinting.Implementations;
using HardwareFingerprinting.Interfaces;

namespace HardwareFingerprinting.Factories
{
    /// <summary>
    /// Factory for creating hardware identifiers
    /// </summary>
    public class HardwareIdentifierFactory : IHardwareIdentifierFactory
    {
        /// <summary>
        /// Creates all available hardware identifiers
        /// </summary>
        /// <returns>A collection of hardware identifiers</returns>
        public IEnumerable<IHardwareIdentifier> CreateAllIdentifiers()
        {
            // CPU identifiers with different methods
            yield return new CpuIdentifier(0);
            yield return new CpuIdentifier(1);
            yield return new CpuIdentifier(2);

            // Host name identifier
            yield return new HostNameIdentifier();

            // MAC address identifier
            yield return new MacAddressIdentifier();

            // HDD identifier
            yield return new HddIdentifier();
        }

        /// <summary>
        /// Creates a specific hardware identifier
        /// </summary>
        /// <param name="type">The type of hardware identifier to create</param>
        /// <returns>The hardware identifier</returns>
        public IHardwareIdentifier CreateIdentifier(HardwareComponentType type)
        {
            switch (type)
            {
                case HardwareComponentType.Cpu:
                    return new CpuIdentifier();
                case HardwareComponentType.Host:
                    return new HostNameIdentifier();
                case HardwareComponentType.Mac:
                    return new MacAddressIdentifier();
                case HardwareComponentType.Hdd:
                    return new HddIdentifier();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown hardware component type");
            }
        }
    }
}