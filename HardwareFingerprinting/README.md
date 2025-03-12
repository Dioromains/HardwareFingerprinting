# Hardware Fingerprinting Library

A C# library for generating unique hardware identifiers based on system components.

## About

This library creates reliable hardware fingerprints by combining information from multiple hardware sources:
- CPU characteristics (with adjustable masking levels)
- Machine/host name
- Network adapters (with virtual machine detection)
- Storage device identifiers

Designed for applications that require hardware-based licensing, virtual machine detection, or system identification.

## Requirements

- .NET Core 8.0 or higher
- Windows or Linux operating system

## Features

- **Comprehensive Hardware Collection**: Gathers information from multiple hardware sources
- **Virtualization Detection**: Filters known virtual machine MAC addresses and hardware identifiers
- **Cross-Platform**: Works on both Windows and Linux environments
- **Configurable Fingerprinting**: Multiple identification methods with different masking levels
- **Validation System**: Validates fingerprints with tolerance for hardware changes

## Usage

```csharp
// Create the factory and service
var factory = new HardwareIdentifierFactory();
var service = new HardwareFingerprintService(factory);

// Generate a hardware fingerprint
string fingerprint = service.GenerateFingerprint();

// Validate a hardware fingerprint
byte[] fingerprintBytes = Convert.FromBase64String(fingerprint);
bool isValid = service.ValidateFingerprint(fingerprintBytes);
```

## Documentation

Full API documentation is available in the project wiki.

Implementation notes:
- Uses direct CPUID instruction access
- Hardware identifiers are hashed with SHA-1
- Validation allows for partial hardware changes

## Suggestions & Pull Requests

Contributions to the project are welcome. Areas for improvement include:

- Additional hardware sources for fingerprinting
- Enhanced virtual environment detection
- Integration with software licensing systems
- Additional platform support (macOS)

## License

This project is available for educational and commercial use. See LICENSE file for details.
