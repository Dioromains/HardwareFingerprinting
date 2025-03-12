# Hardware Fingerprinting Library Documentation

## Basic Usage

### Generating a Hardware Fingerprint

```csharp
using HardwareFingerprinting.Factories;
using HardwareFingerprinting.Implementations;

// Create factory and service
var factory = new HardwareIdentifierFactory();
var service = new HardwareFingerprintService(factory);

// Generate fingerprint (returns Base64 string)
string fingerprint = service.GenerateFingerprint();
```

### Validating a Hardware Fingerprint

```csharp
// Validate fingerprint from string
byte[] fingerprintBytes = Convert.FromBase64String(fingerprintString);
bool isValid = service.ValidateFingerprint(fingerprintBytes);

// Or get current fingerprint bytes and validate
byte[] currentFingerprint = service.GetFingerprintBytes();
bool isCurrentValid = service.ValidateFingerprint(currentFingerprint);
```

## Working with Individual Hardware Components

### Accessing Specific Hardware Identifiers

```csharp
using HardwareFingerprinting.Interfaces;

// Create factory
var factory = new HardwareIdentifierFactory();

// Get specific hardware component identifier
var cpuIdentifier = factory.CreateIdentifier(HardwareComponentType.Cpu);
var hostIdentifier = factory.CreateIdentifier(HardwareComponentType.Host);
var macIdentifier = factory.CreateIdentifier(HardwareComponentType.Mac);
var hddIdentifier = factory.CreateIdentifier(HardwareComponentType.Hdd);

// Get raw identifier data
byte[] cpuData = cpuIdentifier.GetIdentifier();
```

### Creating CPU Identifiers with Different Methods

```csharp
// Create CPU identifier with different methods (0, 1, or 2)
// Method 0: Standard identification (default)
// Method 1: Masks OSXSAVE feature bit
// Method 2: Clears all CPU feature flags
var cpuStandard = new CpuIdentifier(0);  
var cpuMaskedOSXSAVE = new CpuIdentifier(1);
var cpuNoFeatures = new CpuIdentifier(2);
```

## Integration with Licensing Systems

### Simple License Check Example

```csharp
public class LicenseChecker
{
    private readonly IHardwareFingerprintService _fingerprintService;
    private readonly string _storedFingerprint;
    
    public LicenseChecker(IHardwareFingerprintService fingerprintService, string storedFingerprint)
    {
        _fingerprintService = fingerprintService;
        _storedFingerprint = storedFingerprint;
    }
    
    public bool IsLicenseValid()
    {
        try
        {
            var storedFingerprintBytes = Convert.FromBase64String(_storedFingerprint);
            return _fingerprintService.ValidateFingerprint(storedFingerprintBytes);
        }
        catch
        {
            return false;
        }
    }
}
```

## Console Application Example

```csharp
class Program
{
    static void Main(string[] args)
    {
        // Create the factory and service
        var factory = new HardwareIdentifierFactory();
        var service = new HardwareFingerprintService(factory);
        
        // Generate and display hardware fingerprint
        Console.WriteLine("Hardware Fingerprint:");
        Console.WriteLine(service.GenerateFingerprint());
        
        // Store this fingerprint for later validation
        string savedFingerprint = service.GenerateFingerprint();
        
        // Later, validate the fingerprint
        byte[] fingerprintBytes = Convert.FromBase64String(savedFingerprint);
        bool isValid = service.ValidateFingerprint(fingerprintBytes);
        
        Console.WriteLine($"Fingerprint validation: {(isValid ? "Valid" : "Invalid")}");
    }
}
```

## Validation Logic

The hardware fingerprint validation applies these rules:

1. CPU identifier must always match
2. At least 2 out of 3 other identifiers (host, MAC, HDD) must match
3. Alternatively, all present identifiers must match

This provides tolerance for minor hardware changes while ensuring the system is largely the same.
