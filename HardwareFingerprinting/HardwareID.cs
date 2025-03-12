using HardwareFingerprinting.Utils;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace HardwareFingerprinting
{
    /// <summary>
    /// Creates a unique hardware identifier by combining multiple hardware characteristics
    /// to generate a consistent system fingerprint.
    /// </summary>
    public class HardwareID
    {
        // Constants for block allocation and type masking
        private const int OldMethodBlocks = 8;                  // Reserved space for legacy identification methods
        private const int MaxBlocks = 16 + OldMethodBlocks;     // Maximum number of hardware identifier blocks
        private const uint TypeMask = 3;                        // Mask for the lower 2 bits to store hardware type

        /// <summary>
        /// Types of hardware components used for identification
        /// </summary>
        private enum BlockType
        {
            Cpu,    // CPU information
            Host,   // Host/machine name
            Mac,    // MAC address of network adapter
            Hdd     // Hard drive identifier
        };

        // Index where the newer identification methods start
        private readonly int _startBlock;

        // Collection of hardware identifier blocks
        private readonly List<uint> _blocks = new List<uint>();

        /// <summary>
        /// Initializes a new instance of the HardwareID class.
        /// Collects hardware information from multiple sources to create a unique system fingerprint.
        /// </summary>
        public HardwareID()
        {
            // Collect CPU information with different methods
            GetCpu(0);    // Standard CPU identification
            GetCpu(1);    // CPU identification with OSXSAVE bit masked
            _startBlock = _blocks.Count;

            // Additional identification methods
            GetCpu(2);    // CPU identification with all feature bits cleared
            GetMachineName();   // System hostname
            GetHdd();           // Storage device identifier
            GetMacAddresses();  // Network adapter MAC addresses
        }

        /// <summary>
        /// Adds a hardware identifier block to the collection after hashing and type-marking.
        /// </summary>
        /// <param name="p">Raw bytes representing hardware information</param>
        /// <param name="type">Type of hardware component</param>
        private void AddBlock(byte[] p, BlockType type)
        {
            if (_blocks.Count == MaxBlocks) return; // No space left for additional blocks
            if (p.Length == 0) return;              // Skip empty data

            using (var hash = new SHA1Managed())
            {
                // Create a 32-bit hash from the first 4 bytes of SHA1
                var h = hash.ComputeHash(p);
                var block = (uint)(h[0] << 24 | h[1] << 16 | h[2] << 8 | h[3]);

                // Clear the type bits and then set them according to the hardware type
                block &= ~TypeMask;             // Clear lowest 2 bits
                block |= (uint)type & TypeMask; // Set type bits

                // Check for duplicates of the same type to avoid redundancy
                for (var i = _blocks.Count; i > _startBlock; i--)
                {
                    var prevBlock = _blocks[i - 1];
                    if (prevBlock == block)
                        return;  // Skip duplicate blocks

                    // Once we find a block of a different type, stop checking
                    if ((prevBlock & TypeMask) != (block & TypeMask))
                        break;
                }

                // Add the new block to the collection
                _blocks.Add(block);
            }
        }

        /// <summary>
        /// Retrieves CPU information using the CPUID instruction and applies
        /// various masking techniques to normalize the identification.
        /// </summary>
        /// <param name="method">
        /// CPU identification method:
        /// 0 = Standard identification
        /// 1 = Mask OSXSAVE feature bit
        /// 2 = Clear all feature bits
        /// </param>
        private void GetCpu(int method)
        {
            // Retrieve CPU information using CPUID function 1
            var info = CpuId.Invoke(1);

            // Fix for specific AMD Athlon processor bug
            if ((info[0] & 0xFF0) == 0xFE0)
                info[0] ^= 0x20;

            // Remove APIC Physical ID which can vary between cores
            info[1] &= 0x00FFFFFF;

            // Apply method-specific feature masking
            if (method == 2)
            {
                info[2] = 0;            // Method 2: Clear all CPU feature flags
            }
            else if (method == 1)
            {
                info[2] &= ~(1 << 27);  // Method 1: Only clear OSXSAVE feature bit
            }
            // Method 0: Use all feature flags as-is

            // Convert to byte array and add to blocks
            var infob = new byte[16];
            Buffer.BlockCopy(info, 0, infob, 0, infob.Length);
            AddBlock(infob, BlockType.Cpu);
        }

        /// <summary>
        /// Retrieves and adds the machine hostname as an identifier.
        /// Converts hostname to uppercase for consistency.
        /// </summary>
        private void GetMachineName()
        {
            try
            {
                // Get system hostname in uppercase and UTF-16 encoding
                var hn = Encoding.Unicode.GetBytes(Dns.GetHostName().ToUpperInvariant());
                AddBlock(hn, BlockType.Host);
            }
            catch (SocketException)
            {
                // Socket exception can occur if hostname resolution fails
                // In this case, we simply don't add this identifier
            }
        }

        /// <summary>
        /// Processes a MAC address and adds it as an identifier if it's not from a known virtual adapter.
        /// </summary>
        /// <param name="p">MAC address bytes</param>
        private void ProcessMac(byte[] p)
        {
            // Extract the OUI (first 3 bytes) of the MAC address
            var dw = (p[0] << 16) + (p[1] << 8) + p[2];

            // Skip known virtual machine and virtual adapter OUIs
            if (dw == 0x000569 || dw == 0x000C29 || dw == 0x001C14 || dw == 0x005056 || // VMware
                dw == 0x0003FF || dw == 0x000D3A || dw == 0x00125A || dw == 0x00155D ||
                dw == 0x0017FA || dw == 0x001DD8 || dw == 0x002248 || dw == 0x0025AE ||
                dw == 0x0050F2 || // Microsoft
                dw == 0x001C42 || // Parallels
                dw == 0x0021F6)   // Virtual Iron
                return;

            // Add the physical MAC address
            AddBlock(p, BlockType.Mac);
        }

        /// <summary>
        /// Collects MAC addresses from physical Ethernet adapters.
        /// Filters out virtual adapters and non-Ethernet interfaces.
        /// </summary>
        private void GetMacAddresses()
        {
            var blockCountNoMac = _blocks.Count;  // Track count before adding MAC addresses
            byte[] paBytes = null;

            // Iterate through all network interfaces
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet interfaces (excludes loopback, tunnels, etc.)
                if (nic.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
                    continue;

                paBytes = nic.GetPhysicalAddress().GetAddressBytes();

                // Only process MAC addresses with at least 3 bytes (for OUI checking)
                if (paBytes.Length >= 3)
                    ProcessMac(paBytes);
            }

            // If no MAC was added and we have a valid MAC, add it anyway
            // This is a fallback for systems with only virtual adapters
            if (blockCountNoMac == _blocks.Count && paBytes != null && paBytes.Length >= 3)
                AddBlock(paBytes, BlockType.Mac);
        }

        /// <summary>
        /// Retrieves hard drive identifier information based on the current platform.
        /// </summary>
        private void GetHdd()
        {
            try
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Unix:
                        // For Unix-based systems, use disk UUIDs
                        var rootUuids = new DirectoryInfo("/dev/disk/by-uuid");
                        var uuids = new StringBuilder();
                        foreach (var f in rootUuids.GetFiles("*"))
                        {
                            uuids.Append(f.Name);
                        }
                        if (uuids.Length > 0)
                            AddBlock(Encoding.UTF8.GetBytes(uuids.ToString()), BlockType.Hdd);
                        break;

                    default:
                        // For Windows, use volume serial number of system drive
                        var driveLetter = Path.GetPathRoot(Environment.SystemDirectory);
                        uint serialNumber = 0;
                        uint maxComponentLength = 0, fileSystemFlags = 0;

                        // Get volume information and use serial number if available
                        if (Win32.GetVolumeInformation(driveLetter, null,
                            0, ref serialNumber, ref maxComponentLength,
                            ref fileSystemFlags, null, 0) && serialNumber != 0)
                        {
                            AddBlock(BitConverter.GetBytes(serialNumber), BlockType.Hdd);
                        }
                        break;
                }
            }
            catch (Exception)
            {
                // Ignore exceptions, but I really shouldn't...
            }
        }

        /// <summary>
        /// Gets the raw bytes of the hardware fingerprint.
        /// Only includes the newer identification methods (excludes legacy blocks).
        /// </summary>
        /// <returns>Byte array containing the hardware fingerprint</returns>
        public byte[] GetBytes()
        {
            var ms = new MemoryStream();

            // Only include blocks from startBlock onwards (newer methods)
            for (var i = _startBlock; i < _blocks.Count; i++)
            {
                ms.Write(BitConverter.GetBytes(_blocks[i]), 0, 4);
            }

            return ms.ToArray();
        }

        /// <summary>
        /// Returns the hardware fingerprint as a Base64 string.
        /// </summary>
        /// <returns>Base64-encoded string representation of the hardware fingerprint</returns>
        public override string ToString()
        {
            return Convert.ToBase64String(GetBytes());
        }

        /// <summary>
        /// Validates if the provided fingerprint matches the current hardware.
        /// Implements a tolerance algorithm that allows for some hardware changes.
        /// </summary>
        /// <param name="p">Fingerprint bytes to validate</param>
        /// <returns>
        /// True if the fingerprint is valid for this hardware, based on these rules:
        /// 1. CPU identifiers must always match
        /// 2. Either all component types must match, or at least 3 out of 4 must match
        /// </returns>
        public bool IsCorrect(byte[] p)
        {
            // Validate input format (must be non-empty and multiple of 4 bytes)
            if (p.Length == 0 || (p.Length & 3) != 0)
                return false;

            var equals = new bool[4];  // Tracks if each component type has a match
            var found = new bool[4];   // Tracks which component types are present

            // For each block in our hardware fingerprint
            foreach (var id1 in _blocks)
            {
                // Mark this component type as found
                found[id1 & 3] = true;

                // Check if this block matches any in the provided fingerprint
                for (var i = 0; i < p.Length; i += 4)
                {
                    var id2 = BitConverter.ToUInt32(p, i);
                    if (id1 == id2)
                    {
                        // Mark this component type as matching
                        equals[id1 & 3] = true;
                        break;
                    }
                }
            }

            // Rule 1: CPU (type 0) must always match
            if (!equals[0])
                return false;

            // Rule 2: Either all found components match or at least 3 components match
            var n = 0;  // Count of matching components
            var c = 0;  // Count of found components

            for (var i = 0; i < 4; i++)
            {
                if (found[i])
                    c++;
                if (equals[i])
                    n++;
            }

            return n == c || n >= 3;  // All match OR at least 3 match
        }
    }
}