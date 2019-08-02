﻿using System;
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

            CodeGen.GenerateSharedProject
            (
                OUTPUT_FOLDER,
                "GoodbyeXAML.Wpf.Shared",
                "38dc387f-0306-4f71-bf34-eb3060308dba",
                typeof(FrameworkElement)
            );
            FreeConsole();
        }
    }
}