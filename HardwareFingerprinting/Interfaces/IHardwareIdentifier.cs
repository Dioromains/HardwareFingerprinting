using System;

namespace HardwareFingerprinting.Interfaces
{
    /// <summary>
    /// Interface for hardware component identifiers
    /// </summary>
    public interface IHardwareIdentifier
    {
        /// <summary>
        /// Gets the identifier for this hardware component
        /// </summary>
        /// <returns>A byte array containing the hardware identifier</returns>
        byte[] GetIdentifier();

        /// <summary>
        /// Gets the type of hardware component
        /// </summary>
        HardwareComponentType ComponentType { get; }
    }

    /// <summary>
    /// Enum representing different hardware component types
    /// </summary>
    public enum HardwareComponentType
    {
        Cpu,
        Host,
        Mac,
        Hdd
    }
}