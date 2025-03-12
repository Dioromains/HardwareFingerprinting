using HardwareFingerprinting.Interfaces;
using HardwareFingerprinting.Utils;

namespace HardwareFingerprinting.Implementations
{
    /// <summary>
    /// Implementation of the hardware fingerprint service
    /// </summary>
    public class HardwareFingerprintService : IHardwareFingerprintService
    {
        private readonly IHardwareIdentifierFactory _factory;
        private readonly List<uint> _blocks = new List<uint>();
        private readonly int _startBlock;

        /// <summary>
        /// Initializes a new instance of the HardwareFingerprintService class
        /// </summary>
        /// <param name="factory">The hardware identifier factory</param>
        public HardwareFingerprintService(IHardwareIdentifierFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

            // Initialize hardware identifiers
            InitializeHardwareIdentifiers();

            // Set the start block for the fingerprint
            _startBlock = 2; // Skip the first two CPU identifiers (old methods)
        }

        /// <summary>
        /// Initializes hardware identifiers and computes their hash blocks
        /// </summary>
        private void InitializeHardwareIdentifiers()
        {
            foreach (var identifier in _factory.CreateAllIdentifiers())
            {
                var data = identifier.GetIdentifier();
                if (data.Length == 0)
                    continue;

                var block = HashingUtility.ComputeHash(data, identifier.ComponentType);
                if (block == 0)
                    continue;

                // Check for duplicates of the same type
                var isDuplicate = false;
                for (var i = _blocks.Count - 1; i >= 0; i--)
                {
                    var existingBlock = _blocks[i];
                    if (existingBlock == block)
                    {
                        isDuplicate = true;
                        break;
                    }

                    // If we've moved to a different type, stop checking
                    if ((existingBlock & 3) != (block & 3))
                        break;
                }

                if (!isDuplicate)
                    _blocks.Add(block);
            }
        }

        /// <summary>
        /// Generates a hardware fingerprint
        /// </summary>
        /// <returns>A string representation of the hardware fingerprint</returns>
        public string GenerateFingerprint()
        {
            return Convert.ToBase64String(GetFingerprintBytes());
        }

        /// <summary>
        /// Gets the raw bytes of the hardware fingerprint
        /// </summary>
        /// <returns>A byte array containing the hardware fingerprint</returns>
        public byte[] GetFingerprintBytes()
        {
            var ms = new MemoryStream();
            for (var i = _startBlock; i < _blocks.Count; i++)
            {
                ms.Write(BitConverter.GetBytes(_blocks[i]), 0, 4);
            }
            return ms.ToArray();
        }

        /// <summary>
        /// Validates if the provided fingerprint matches the current hardware
        /// </summary>
        /// <param name="fingerprint">The fingerprint to validate</param>
        /// <returns>True if the fingerprint is valid for this hardware, false otherwise</returns>
        public bool ValidateFingerprint(byte[] fingerprint)
        {
            if (fingerprint == null || fingerprint.Length == 0 || (fingerprint.Length & 3) != 0)
                return false;

            var equals = new bool[4];
            var found = new bool[4];

            foreach (var id1 in _blocks)
            {
                found[id1 & 3] = true;
                for (var i = 0; i < fingerprint.Length; i += 4)
                {
                    var id2 = BitConverter.ToUInt32(fingerprint, i);
                    if (id1 == id2)
                    {
                        equals[id1 & 3] = true;
                        break;
                    }
                }
            }

            // CPU must match
            if (!equals[0])
                return false;

            // At least 2 of 3 other components must match
            var matchedComponents = equals.Count(e => e);
            var totalComponents = found.Count(f => f);

            return matchedComponents == totalComponents || matchedComponents >= 3;
        }
    }
}