//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using HardwareFingerprinting.Factories;
//using HardwareFingerprinting.Implementations;
//using HardwareFingerprinting.Interfaces;

//namespace HardwareFingerprinting
//{
//    /// <summary>
//    /// Example program demonstrating the usage of the hardware fingerprinting library
//    /// </summary>
//    public class Program
//    {
//        private static readonly Random _random = new Random();

//        public static void Main(string[] args)
//        {
//            Console.OutputEncoding = Encoding.UTF8;
//            Console.Title = "HardwareFingerprinting | v1.0";
//            Console.CursorVisible = false;

//            try
//            {
//                // Animation
//                DisplayIntro();

//                // Create the factory and service
//                var factory = new HardwareIdentifierFactory();
//                var service = new HardwareFingerprintService(factory);

//                DisplayScanningAnimation();

//                var fingerprint = service.GenerateFingerprint();
//                DisplayFingerprintGeneration(fingerprint);

//                // Validation
//                var fingerprintBytes = service.GetFingerprintBytes();
//                var isValid = service.ValidateFingerprint(fingerprintBytes);
//                DisplayValidation(isValid);

//                // Show hardware components
//                DisplayHardwareComponents(factory);
//            }
//            catch (Exception ex)
//            {
//                Console.Clear();
//                Console.ForegroundColor = ConsoleColor.Red;
//                DrawTextBox("ERROR DETECTED", new[] { ex.Message }, ConsoleColor.Red);
//                Console.ResetColor();
//            }

//            // Exit prompt
//            Console.WriteLine();
//            Console.ForegroundColor = ConsoleColor.DarkGray;
//            Console.WriteLine("Press any key to terminate...");
//            Console.ResetColor();
//            Console.ReadKey(true);
//        }

//        /// <summary>
//        /// Displays an intro
//        /// </summary>
//        private static void DisplayIntro()
//        {
//            Console.Clear();

//            Console.WriteLine("HardwareFingerprinting | v1.0");
//            Console.WriteLine();

//            string[] asciiArt = new[] {
//                @" _    _ _______        _______ ",
//                @"| |  | |  __ \ \      / /  __ \",
//                @"| |__| | |  | \ \ /\ / /| |  | |",
//                @"|  __  | |  | |\ V  V / | |  | |",
//                @"| |  | | |__| | \_/\_/  | |__| |",
//                @"|_|  |_|_____/          |_____/ "
//            };

//            // Display ASCII art
//            Console.ForegroundColor = ConsoleColor.Cyan;
//            foreach (var line in asciiArt)
//            {
//                Console.WriteLine(line);
//            }

//            Console.ForegroundColor = ConsoleColor.White;
//            Console.WriteLine("\n" + new string('=', Console.WindowWidth - 1));
//            Console.WriteLine();

//            // Display tagline
//            string tagline = "[ Unique Hardware Identity Sequencing System ]";
//            int centerX = (Console.WindowWidth - tagline.Length) / 2;
//            Console.SetCursorPosition(centerX, Console.CursorTop);

//            // Process in small batches for speed
//            for (int i = 0; i < tagline.Length; i += 2)
//            {
//                int charsToWrite = Math.Min(2, tagline.Length - i);
//                Console.ForegroundColor = GetRandomConsoleColor();
//                Console.Write(tagline.Substring(i, charsToWrite));
//                Thread.Sleep(10);
//            }

//            Console.ResetColor();
//            Console.WriteLine("\n");
//            Thread.Sleep(200);
//        }

//        /// <summary>
//        /// Displays an animated scanning effect
//        /// </summary>
//        private static void DisplayScanningAnimation()
//        {
//            Console.ForegroundColor = ConsoleColor.Yellow;
//            Console.WriteLine("▶ INITIATING HARDWARE SCAN");
//            Console.ResetColor();

//            // Show progress bar
//            Console.Write("  [");
//            Console.CursorLeft = Console.WindowWidth - 3;
//            Console.Write("]");
//            Console.CursorLeft = 3;

//            var progressChars = new[] { '░', '▒', '▓', '█' };
//            int width = Console.WindowWidth - 6;

//            // Process in batches for speed
//            int batchSize = 5;
//            for (int i = 0; i < width; i += batchSize)
//            {
//                int charsToWrite = Math.Min(batchSize, width - i);
//                Console.ForegroundColor = GetProgressColor(i, width);

//                for (int j = 0; j < charsToWrite; j++)
//                {
//                    Console.Write(progressChars[_random.Next(progressChars.Length)]);
//                }

//                Thread.Sleep(10);
//            }

//            Console.ResetColor();
//            Console.WriteLine("\n");
//        }

//        /// <summary>
//        /// Displays fingerprint generation
//        /// </summary>
//        private static void DisplayFingerprintGeneration(string fingerprint)
//        {
//            Console.ForegroundColor = ConsoleColor.Green;
//            Console.WriteLine("▶ FINGERPRINT ACQUIRED");
//            Console.ResetColor();

//            // Matrix-style effect for fingerprint
//            Console.WriteLine();
//            Console.Write("  ");

//            // Scrambling effect (2 iterations)
//            for (int iterations = 0; iterations < 2; iterations++)
//            {
//                Console.CursorLeft = 2;
//                for (int i = 0; i < fingerprint.Length; i++)
//                {
//                    char randomChar = (char)_random.Next(33, 126);
//                    Console.ForegroundColor = GetRandomConsoleColor();
//                    Console.Write(randomChar);
//                }
//                Thread.Sleep(500);
//            }

//            // Display actual fingerprint (in chunks for speed)
//            Console.CursorLeft = 2;
//            StringBuilder output = new StringBuilder();

//            for (int i = 0; i < fingerprint.Length; i++)
//            {
//                output.Append(fingerprint[i]);

//                if ((i + 1) % 6 == 0 && i < fingerprint.Length - 1)
//                {
//                    output.Append(':');
//                }
//            }

//            // Output in small batches
//            string finalOutput = output.ToString();
//            int batchSize = 4;

//            for (int i = 0; i < finalOutput.Length; i += batchSize)
//            {
//                int charsToWrite = Math.Min(batchSize, finalOutput.Length - i);
//                Console.ForegroundColor = ConsoleColor.Cyan;
//                Console.Write(finalOutput.Substring(i, charsToWrite));
//                Thread.Sleep(20);
//            }

//            Console.ResetColor();
//            Console.WriteLine("\n");
//        }

//        /// <summary>
//        /// Displays validation results
//        /// </summary>
//        private static void DisplayValidation(bool isValid)
//        {
//            Console.ForegroundColor = ConsoleColor.Magenta;
//            Console.WriteLine("▶ PERFORMING INTEGRITY VERIFICATION");
//            Console.ResetColor();

//            // Loading spinner (reduced iterations)
//            Console.Write("  Validating sequence integrity ");

//            var spinChars = new[] { '⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏' };
//            for (int i = 0; i < 6; i++)
//            {
//                Console.Write(spinChars[i % spinChars.Length]);
//                Thread.Sleep(200);
//                Console.Write("\b");
//            }

//            // Show validation result
//            if (isValid)
//            {
//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine("✓ SEQUENCE AUTHENTIC");

//                // Draw a verification box
//                string[] messages = new[] {
//                    "System fingerprint verification complete",
//                    "All sequence markers authenticated",
//                    "Hardware signature integrity: 100%"
//                };

//                DrawTextBox("VALIDATION SUCCESSFUL", messages, ConsoleColor.Green);
//            }
//            else
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine("✗ SEQUENCE MISMATCH DETECTED");

//                string[] messages = new[] {
//                    "WARNING: Hardware profile has been altered",
//                    "Sequence coherence compromised",
//                    "Recommend full system reverification"
//                };

//                DrawTextBox("VALIDATION FAILED", messages, ConsoleColor.Red);
//            }

//            Console.ResetColor();
//            Console.WriteLine();
//        }

//        /// <summary>
//        /// Displays hardware components
//        /// </summary>
//        private static void DisplayHardwareComponents(IHardwareIdentifierFactory factory)
//        {
//            Console.ForegroundColor = ConsoleColor.Blue;
//            Console.WriteLine("▶ HARDWARE COMPONENT ANALYSIS");
//            Console.ResetColor();
//            Console.WriteLine();

//            // Display components with different visualizations
//            var components = new List<(string Name, HardwareComponentType Type, ConsoleColor Color, string Icon)>
//            {
//                ("CPU              ", HardwareComponentType.Cpu, ConsoleColor.DarkRed, "🔲"),
//                ("System Hostname  ", HardwareComponentType.Host, ConsoleColor.DarkGreen, "🏠"),
//                ("Network Interface", HardwareComponentType.Mac, ConsoleColor.DarkCyan, "🌐"),
//                ("Storage Subsystem", HardwareComponentType.Hdd, ConsoleColor.DarkYellow, "💾")
//            };

//            foreach (var component in components)
//            {
//                DisplayComponentWithVisual(factory, component.Name, component.Type, component.Color, component.Icon);
//                Thread.Sleep(600);
//            }
//        }

//        /// <summary>
//        /// Displays a component
//        /// </summary>
//        private static void DisplayComponentWithVisual(
//            IHardwareIdentifierFactory factory,
//            string name,
//            HardwareComponentType type,
//            ConsoleColor color,
//            string icon)
//        {
//            Console.ForegroundColor = color;
//            Console.Write($"  {icon} {name}");
//            Console.ResetColor();

//            Console.Write(" [");

//            // Fill with empty squares
//            Console.ForegroundColor = ConsoleColor.DarkGray;
//            Console.Write(new string('▫', 10));
//            Thread.Sleep(10);

//            // Replace with filled squares
//            Console.CursorLeft -= 10;
//            Console.ForegroundColor = color;
//            Console.Write(new string('▪', 10));
//            Console.Write("] ");

//            // Get component data
//            var identifier = factory.CreateIdentifier(type);
//            var data = identifier.GetIdentifier();

//            // Format data based on component type
//            string displayValue;
//            if (type == HardwareComponentType.Host)
//            {
//                displayValue = Encoding.Unicode.GetString(data);
//            }
//            else if (type == HardwareComponentType.Mac)
//            {
//                displayValue = string.Join(":", data.Select(b => b.ToString("X2")));
//            }
//            else
//            {
//                // You can choose to show only first 8 bytes for better display or show all
//                // displayValue =BitConverter.ToString(data.Take(Math.Min(data.Length, 8)).ToArray());
//                displayValue = BitConverter.ToString(data);
//                //if (data.Length > 8) displayValue += "...";
//            }

//            // Output the result
//            Console.ForegroundColor = ConsoleColor.White;
//            Console.WriteLine(displayValue);
//        }

//        /// <summary>
//        /// Creates a text box with a border
//        /// </summary>
//        private static void DrawTextBox(string title, string[] messages, ConsoleColor color)
//        {
//            int maxLength = Math.Max(title.Length, messages.Max(m => m.Length)) + 4;

//            Console.WriteLine();

//            // Top border
//            Console.ForegroundColor = color;
//            Console.Write("  ╔");
//            Console.Write(new string('═', maxLength));
//            Console.WriteLine("╗");

//            // Title
//            Console.Write("  ║");
//            Console.ForegroundColor = ConsoleColor.White;
//            Console.Write(title.PadRight(maxLength));
//            Console.ForegroundColor = color;
//            Console.WriteLine("║");

//            // Separator
//            Console.Write("  ╟");
//            Console.Write(new string('─', maxLength));
//            Console.WriteLine("╢");

//            // Messages
//            foreach (var message in messages)
//            {
//                Console.Write("  ║");
//                Console.ForegroundColor = ConsoleColor.Gray;
//                Console.Write(message.PadRight(maxLength));
//                Console.ForegroundColor = color;
//                Console.WriteLine("║");
//            }

//            // Bottom border
//            Console.Write("  ╚");
//            Console.Write(new string('═', maxLength));
//            Console.WriteLine("╝");

//            Console.ResetColor();
//        }

//        /// <summary>
//        /// Gets a random console color
//        /// </summary>
//        private static ConsoleColor GetRandomConsoleColor()
//        {
//            var colors = new[] {
//                ConsoleColor.Blue, ConsoleColor.Cyan, ConsoleColor.Green,
//                ConsoleColor.Magenta, ConsoleColor.Red, ConsoleColor.Yellow,
//                ConsoleColor.DarkBlue, ConsoleColor.DarkCyan, ConsoleColor.DarkGreen,
//                ConsoleColor.DarkMagenta, ConsoleColor.DarkRed, ConsoleColor.DarkYellow
//            };

//            return colors[_random.Next(colors.Length)];
//        }

//        /// <summary>
//        /// Gets a color based on progress percentage
//        /// </summary>
//        private static ConsoleColor GetProgressColor(int position, int total)
//        {
//            var percent = (double)position / total;

//            if (percent < 0.3) return ConsoleColor.Red;
//            if (percent < 0.6) return ConsoleColor.Yellow;
//            if (percent < 0.9) return ConsoleColor.Cyan;
//            return ConsoleColor.Green;
//        }
//    }
//}