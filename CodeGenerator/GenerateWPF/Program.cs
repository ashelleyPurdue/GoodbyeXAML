using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

using CodeGenerator;

namespace GenerateWPF
{
    class Program
    {
        static void Main(string[] args)
        {
            string outputFolder = args[0];

            CodeGen.GenerateSharedProject
            (
                outputFolder,
                "GoodbyeXAML.Wpf.Shared",
                "38dc387f-0306-4f71-bf34-eb3060308dba",
                typeof(FrameworkElement)
            );
        }
    }
}
