using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using CodeGenerator;

public static class CodeGen
{
    public static void GenerateWPFDotnetCoreProject(string outputFolder, string projectName, IEnumerable<Type> types)
    {
        string projFolder = Path.Combine(outputFolder, projectName);
        string csprojName = Path.Combine(projFolder, projectName + ".csproj");
        string nuspecName = Path.Combine(projFolder, projectName + ".nuspec");

        GenerateClassFiles(projFolder, projectName, types);
        File.WriteAllText(csprojName, CsProj.GenerateNetCore(projectName));
        File.WriteAllText(nuspecName, Nuspec.Generate(projectName));
    }

    public static void GenerateWPFDotnetFrameworkProject(string outputFolder, string projectName, IEnumerable<Type> types)
    {
        // TODO: Refactor this duplicated code.
        string projFolder = Path.Combine(outputFolder, projectName);
        string csprojName = Path.Combine(projFolder, projectName + ".csproj");
        string nuspecName = Path.Combine(projFolder, projectName + ".nuspec");

        GenerateClassFiles(projFolder, projectName, types);
        File.WriteAllText(csprojName, CsProj.GenerateNetFramework(projectName, types));
        File.WriteAllText(nuspecName, Nuspec.Generate(projectName));
    }

    public static void GenerateClassFiles(string outputFolder, string namespaceName, IEnumerable<Type> types)
    {
        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        foreach (Type t in types)
        {
            Console.WriteLine($"Generating {t.Name}Extensions.cs");

            string outputFilePath = Path.Combine(outputFolder, $"{t.Name}Extensions.cs");
            string text = GenerateExtensionClassFor(namespaceName, t);
            File.WriteAllText(outputFilePath, text);
        }
    }

    public static string GenerateExtensionClassFor(string namespaceName, Type T)
    {
        var settableProperties = T
            .GetProperties()
            .Where(p => p.DeclaringType == T)   // Skip properties added by parent class
            .Where(p => p.CanWrite && p.SetMethod.IsPublic);

        var events = T
            .GetEvents()
            .Where(e => e.DeclaringType == T);

        var generator = new ClassGenerator(namespaceName, T.Name + "Extensions");

        foreach (PropertyInfo p in settableProperties)
            generator.AddProperty(p);

        foreach (EventInfo e in events)
            generator.AddEvent(e);

        return generator.Generate();
    }

}