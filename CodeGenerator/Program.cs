using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace CodeGenerator
{
    class Program
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        static void Main(string[] args)
        {
            AllocConsole();
            GenerateWPFExtensions("foo");
            FreeConsole();
        }

        private static void GenerateWPFExtensions(string outputFolder)
        {
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            // Write the extension method class for every FrameworkElement type.
            Type frameworkElement = typeof(FrameworkElement);

            var controlTypes = frameworkElement
                .Assembly
                .GetTypes()
                .Where(t => t == frameworkElement || t.IsSubclassOf(frameworkElement))
                .OrderBy(t => t.FullName);

            foreach (Type t in controlTypes)
            {
                Console.WriteLine($"Generating {t.Name}Extensions.cs");

                string outputFilePath = Path.Combine(outputFolder, $"{t.Name}Extensions.cs");
                File.WriteAllText(outputFilePath,
                $@"
                    using System;
                    using {t.Namespace};
                    namespace GoodbyeXAML.Wpf
                    {{
                        {ExtensionMethodGenerator.GenerateExtensionClassFor(t)}
                    }}
                ");
                
            }
        }
    }
}
