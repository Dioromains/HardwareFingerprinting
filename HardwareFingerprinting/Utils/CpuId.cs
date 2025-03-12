using System.Runtime.InteropServices;

namespace HardwareFingerprinting.Utils
{
    /// <summary>
    /// Provides access to CPU identification information through the CPUID instruction.
    /// Dynamically generates and executes assembly code to retrieve processor details.
    /// </summary>
    public static class CpuId
    {
        /// <summary>
        /// Executes the CPUID instruction with the specified function level and returns register values.
        /// </summary>
        /// <param name="level">
        /// The CPUID function level to execute:
        /// - 0: Returns vendor identification string
        /// - 1: Returns processor info and feature bits
        /// - 2+: Returns various processor-specific information
        /// </param>
        /// <returns>
        /// An array of 4 integers containing the values from EAX, EBX, ECX, and EDX registers after CPUID execution:
        /// - [0]: EAX - Usually contains processor version information
        /// - [1]: EBX - Usually contains additional identification information
        /// - [2]: ECX - Usually contains feature flags
        /// - [3]: EDX - Usually contains more feature flags
        /// </returns>
        public static int[] Invoke(int level)
        {
            var codePointer = nint.Zero;
            try
            {
                // Dynamically generate the appropriate assembly code based on architecture
                byte[] codeBytes;
                if (nint.Size == 4)
                {
                    // 32-bit x86 assembly code
                    codeBytes = new byte[30];
                    codeBytes[0] = 0x55;                                                // push ebp                     - save base pointer
                    codeBytes[1] = 0x8B; codeBytes[2] = 0xEC;                           // mov ebp, esp                 - set up stack frame
                    codeBytes[3] = 0x53;                                                // push ebx                     - preserve register, modified by CPUID
                    codeBytes[4] = 0x57;                                                // push edi                     - preserve register
                    codeBytes[5] = 0x8B; codeBytes[6] = 0x45; codeBytes[7] = 0x08;      // mov eax, dword ptr [ebp+8]   - load 'level' parameter into eax
                    codeBytes[8] = 0x0F; codeBytes[9] = 0xA2;                           // cpuid                        - execute CPUID instruction
                    codeBytes[10] = 0x8B; codeBytes[11] = 0x7D; codeBytes[12] = 0x0C;   // mov edi, dword ptr [ebp+12]  - load buffer address into edi
                    codeBytes[13] = 0x89; codeBytes[14] = 0x07;                         // mov dword ptr [edi+0], eax   - store eax result to buffer[0]
                    codeBytes[15] = 0x89; codeBytes[16] = 0x5F; codeBytes[17] = 0x04;   // mov dword ptr [edi+4], ebx   - store ebx result to buffer[1]
                    codeBytes[18] = 0x89; codeBytes[19] = 0x4F; codeBytes[20] = 0x08;   // mov dword ptr [edi+8], ecx   - store ecx result to buffer[2]
                    codeBytes[21] = 0x89; codeBytes[22] = 0x57; codeBytes[23] = 0x0C;   // mov dword ptr [edi+12], edx  - tore edx result to buffer[3]
                    codeBytes[24] = 0x5F;                                               // pop edi                      - restore register
                    codeBytes[25] = 0x5B;                                               // pop ebx                      - restore register
                    codeBytes[26] = 0x8B; codeBytes[27] = 0xE5;                         // mov esp, ebp                 - restore stack pointer
                    codeBytes[28] = 0x5D;                                               // pop ebp                      - restore base pointer
                    codeBytes[29] = 0xc3;                                               // ret                          - return to caller
                }
                else
                {
                    // 64-bit x64 assembly code
                    codeBytes = new byte[26];
                    codeBytes[0] = 0x53;                                                                    // push rbx                     - preserve register, modified by CPUID
                    codeBytes[1] = 0x49; codeBytes[2] = 0x89; codeBytes[3] = 0xd0;                          // mov r8, rdx                  - move buffer address from rdx to r8
                    codeBytes[4] = 0x89; codeBytes[5] = 0xc8;                                               // mov eax, ecx                 - move 'level' parameter from ecx to eax
                    codeBytes[6] = 0x0F; codeBytes[7] = 0xA2;                                               // cpuid                        - execute CPUID instruction
                    codeBytes[8] = 0x41; codeBytes[9] = 0x89; codeBytes[10] = 0x40; codeBytes[11] = 0x00;   // mov dword ptr [r8+0], eax    - store eax result to buffer[0]
                    codeBytes[12] = 0x41; codeBytes[13] = 0x89; codeBytes[14] = 0x58; codeBytes[15] = 0x04; // mov dword ptr [r8+4], ebx    - store ebx result to buffer[1]
                    codeBytes[16] = 0x41; codeBytes[17] = 0x89; codeBytes[18] = 0x48; codeBytes[19] = 0x08; // mov dword ptr [r8+8], ecx    - store ecx result to buffer[2]
                    codeBytes[20] = 0x41; codeBytes[21] = 0x89; codeBytes[22] = 0x50; codeBytes[23] = 0x0c; // mov dword ptr [r8+12], edx   - store edx result to buffer[3]
                    codeBytes[24] = 0x5b;                                                                   // pop rbx                      - restore register
                    codeBytes[25] = 0xc3;                                                                   // ret                          - return to caller
                }

                // Allocate executable memory for the dynamically generated assembly code
                codePointer = Win32.VirtualAlloc(
                    nint.Zero,                                                     // Let the system choose the allocation address
                    new nuint((uint)codeBytes.Length),                                       // Size of the code bytes
                    Win32.AllocationType.Commit | Win32.AllocationType.Reserve, // Allocation type
                    Win32.MemoryProtection.ExecuteReadWrite                           // Memory protection (must be executable)
                );

                // Copy the assembly code bytes to the executable memory
                Marshal.Copy(codeBytes, 0, codePointer, codeBytes.Length);

                // Create a managed delegate that points to the unmanaged assembly code
                var cpuIdDelg =
                    (CpuIdDelegate)Marshal.GetDelegateForFunctionPointer(codePointer, typeof(CpuIdDelegate));

                // Prepare buffer to receive the CPUID results
                var buffer = new int[4]; // 4 integers for EAX, EBX, ECX, EDX

                // Pin the buffer in memory so the unmanaged code can safely write to it
                var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                {
                    // Execute the CPUID instruction via the delegate
                    cpuIdDelg(level, buffer);
                }
                finally
                {
                    // Release the pinned buffer
                    handle.Free();
                }

                // Return the CPUID register values
                return buffer;
            }
            finally
            {
                // Free the executable memory if it was allocated
                if (codePointer != nint.Zero)
                {
                    Win32.VirtualFree(codePointer, nuint.Zero, Win32.FreeType.Release);
                }
            }
        }

        /// <summary>
        /// Delegate for the dynamically generated CPUID function.
        /// </summary>
        /// <remarks>
        /// Cdecl calling convention is used for cross-platform compatibility with dynamically generated code.
        /// </remarks>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void CpuIdDelegate(int level, int[] buffer);
    }
}