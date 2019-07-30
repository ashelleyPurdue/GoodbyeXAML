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

        private const string OUTPUT_FOLDER = "../../../../../GeneratedExtensionMethods";

        static void Main(string[] args)
        {
            AllocConsole();
            GenerateWPFExtensions(OUTPUT_FOLDER);
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

            const string SHARED_GUID = "38dc387f-0306-4f71-bf34-eb3060308dba";
            CodeGen.GenerateSharedProject(outputFolder, "GoodbyeXAML.Wpf.Shared", SHARED_GUID, types);
        }
    }
}
