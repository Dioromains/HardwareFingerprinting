using System;

namespace HardwareFingerprinting.Interfaces
{
    /// <summary>
    /// Interface for the hardware fingerprint service
    /// </summary>
    public interface IHardwareFingerprintService
    {
        /// <summary>
        /// Generates a hardware fingerprint
        /// </summary>
        /// <returns>A string representation of the hardware fingerprint</returns>
        string GenerateFingerprint();

        /// <summary>
        /// Gets the raw bytes of the hardware fingerprint
        /// </summary>
        /// <returns>A byte array containing the hardware fingerprint</returns>
        byte[] GetFingerprintBytes();

        /// <summary>
        /// Validates if the provided fingerprint matches the current hardware
        /// </summary>
        /// <param name="fingerprint">The fingerprint to validate</param>
        /// <returns>True if the fingerprint is valid for this hardware, false otherwise</returns>
        bool ValidateFingerprint(byte[] fingerprint);
    }
}