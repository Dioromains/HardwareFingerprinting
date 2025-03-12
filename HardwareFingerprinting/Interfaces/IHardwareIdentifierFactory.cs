using System.Collections.Generic;

namespace HardwareFingerprinting.Interfaces
{
    /// <summary>
    /// Interface for a factory that creates hardware identifiers
    /// </summary>
    public interface IHardwareIdentifierFactory
    {
        /// <summary>
        /// Creates all available hardware identifiers
        /// </summary>
        /// <returns>A collection of hardware identifiers</returns>
        IEnumerable<IHardwareIdentifier> CreateAllIdentifiers();

        /// <summary>
        /// Creates a specific hardware identifier
        /// </summary>
        /// <param name="type">The type of hardware identifier to create</param>
        /// <returns>The hardware identifier</returns>
        IHardwareIdentifier CreateIdentifier(HardwareComponentType type);
    }
}