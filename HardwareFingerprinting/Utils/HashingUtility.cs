using System;
using System.Security.Cryptography;

namespace HardwareFingerprinting.Utils
{
    /// <summary>
    /// Utility class for hashing operations
    /// </summary>
    public static class HashingUtility
    {
        /// <summary>
        /// Computes a SHA1 hash of the input data and returns a 32-bit integer
        /// </summary>
        /// <param name="data">The data to hash</param>
        /// <param name="type">The type of hardware component</param>
        /// <returns>A 32-bit integer hash with the component type encoded in the lower 2 bits</returns>
        public static uint ComputeHash(byte[] data, Interfaces.HardwareComponentType type)
        {
            if (data == null || data.Length == 0)
                return 0;

            using (var hash = SHA1.Create())
            {
                var h = hash.ComputeHash(data);
                var block = (uint)(h[0] << 24 | h[1] << 16 | h[2] << 8 | h[3]);

                // Clear the lower 2 bits
                block &= ~3U;

                // Set the type bits
                block |= (uint)type & 3U;

                return block;
            }
        }
    }
}