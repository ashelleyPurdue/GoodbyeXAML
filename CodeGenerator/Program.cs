using System;
using System.Linq;
using System.Reflection;

namespace CodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateWPFExtensions("foo");
        }

        private static void GenerateWPFExtensions(string outputFolder)
        {
            Assembly wpfAssembly = Assembly.Load("System.Windows");
            var types = wpfAssembly.GetTypes();

            foreach (Type t in types)
                Console.WriteLine(t.Name);
        }
    }
}
