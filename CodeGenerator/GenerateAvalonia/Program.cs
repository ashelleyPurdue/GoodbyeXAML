using System;
using CodeGenerator;

namespace GenerateAvalonia
{
    class Program
    {
        static void Main(string[] args)
        {
            string outputFolder = args[0];

            CodeGen.GenerateSharedProject
            (
                outputFolder,
                "GoodbyeXAML.Avalonia.Shared",
                "87ce4651-9057-4725-9b56-65b177d6dfa9",
                typeof(Avalonia.AvaloniaObject),
                typeof(Avalonia.Controls.Control),
                typeof(Avalonia.Layout.Layoutable),
                typeof(Avalonia.StyledElement),
                typeof(Avalonia.Styling.Style)
            );

        }
    }
}
