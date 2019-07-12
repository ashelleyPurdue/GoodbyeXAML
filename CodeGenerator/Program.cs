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
            GenerateWPFExtensions("../../../../GeneratedExtensionMethods");
            FreeConsole();
        }

        private static void GenerateWPFExtensions(string outputFolder)
        {
            // Write the extension method class for every FrameworkElement type.
            Type frameworkElement = typeof(FrameworkElement);

            var types = frameworkElement
                .Assembly
                .GetTypes()
                .Where(t => t == frameworkElement || t.IsSubclassOf(frameworkElement))
                .Where(t => t.IsPublic)
                .Where(t => !t.IsGenericType)
                .OrderBy(t => t.FullName);

            CodeGen.GenerateWPFDotnetCoreProject(outputFolder, "GoodbyeXAML.Wpf.Core", types);
            CodeGen.GenerateDotnetFrameworkProject(outputFolder, "GoodbyeXAML.Wpf.Framework", types);
        }
    }
}
