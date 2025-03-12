using System;
using HardwareFingerprinting.Interfaces;

namespace HardwareFingerprinting.Implementations
{
    /// <summary>
    /// Implementation of the CPU hardware identifier
    /// </summary>
    public class CpuIdentifier : IHardwareIdentifier
    {
        private readonly int _method;

        /// <summary>
        /// Gets the type of hardware component
        /// </summary>
        public HardwareComponentType ComponentType => HardwareComponentType.Cpu;

        /// <summary>
        /// Initializes a new instance of the CpuIdentifier class
        /// </summary>
        /// <param name="method">The method to use for CPU identification (0, 1, or 2)</param>
        public CpuIdentifier(int method = 0)
        {
            if (method < 0 || method > 2)
                throw new ArgumentOutOfRangeException(nameof(method), "Method must be 0, 1, or 2");

            _method = method;
        }

        /// <summary>
        /// Gets the identifier for this hardware component
        /// </summary>
        /// <returns>A byte array containing the hardware identifier</returns>
        public byte[] GetIdentifier()
        {
            var info = Utils.CpuId.Invoke(1);

            // Fix Athlon bug
            if ((info[0] & 0xFF0) == 0xFE0)
                info[0] ^= 0x20;

            // Mask out APIC Physical ID
            info[1] &= 0x00FFFFFF;

            // Apply method-specific modifications
            switch (_method)
            {
                case 1:
                    info[2] &= ~(1 << 27);
                    break;
                case 2:
                    info[2] = 0;
                    break;
            }

            var infob = new byte[16];
            Buffer.BlockCopy(info, 0, infob, 0, infob.Length);

            return infob;
        }
    }
}