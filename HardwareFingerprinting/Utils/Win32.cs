using System;
using System.Runtime.InteropServices;
using System.Text;

namespace HardwareFingerprinting.Utils
{
    /// <summary>
    /// Utility class for Windows API functions
    /// </summary>
    public static class Win32
    {
        [Flags()]
        public enum AllocationType : uint
        {
            Commit = 0x1000,
            Reserve = 0x2000,
        }

        [Flags()]
        public enum FreeType : uint
        {
            Release = 0x8000,
        }

        [Flags()]
        public enum MemoryProtection : uint
        {
            ExecuteReadWrite = 0x40,
        }

        /// <summary>
        /// Allocates virtual memory in the specified process
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAlloc(
            IntPtr lpAddress,
            UIntPtr dwSize,
            AllocationType flAllocationType,
            MemoryProtection flProtect);

        /// <summary>
        /// Frees virtual memory in the specified process
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualFree(
            IntPtr lpAddress,
            UIntPtr dwSize,
            FreeType dwFreeType);

        /// <summary>
        /// Gets information about a volume
        /// </summary>
        [DllImport("kernel32", SetLastError = true)]
        public static extern bool GetVolumeInformation(
            string pathName,
            StringBuilder volumeNameBuffer,
            uint volumeNameSize,
            ref uint volumeSerialNumber,
            ref uint maximumComponentLength,
            ref uint fileSystemFlags,
            StringBuilder fileSystemNameBuffer,
            uint fileSystemNameSize);
    }
}