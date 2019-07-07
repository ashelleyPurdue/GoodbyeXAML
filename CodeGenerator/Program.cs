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
            Console.ReadKey();
            FreeConsole();
        }

        private static void GenerateWPFExtensions(string outputFolder)
        {
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            string outFilePath = Path.Combine(outputFolder, "WPFExtensions.cs");
            using (var outFile = new StreamWriter(outFilePath))
            {
                outFile.WriteLine("namespace GoodbyeXAML.Wpf {");

                // Write the extension method class for every FrameworkElement type.
                Type frameworkElement = typeof(FrameworkElement);

                var controlTypes = frameworkElement
                    .Assembly
                    .GetTypes()
                    .Where(t => t == frameworkElement || t.IsSubclassOf(frameworkElement))
                    .OrderBy(t => t.FullName);

                foreach (Type t in controlTypes)
                {
                    outFile.WriteLine(ExtensionMethodGenerator.GenerateExtensionClassFor(t));
                }

                outFile.WriteLine("}");
            }
        }
    }
}
