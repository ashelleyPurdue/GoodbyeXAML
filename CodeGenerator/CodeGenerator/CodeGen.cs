using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using CodeGenerator;

public static class CodeGen
{
    public static void GenerateWPFShProj(string outputFolder, string projectName, IEnumerable<Type> types)
    {
        const string SHARED_GUID = "38dc387f-0306-4f71-bf34-eb3060308dba";

        string projFolder = Path.Combine(outputFolder, projectName);
        string shprojName = Path.Combine(projFolder, projectName + ".shproj");
        string projitemsName = Path.Combine(projFolder, projectName + ".projitems");

        GenerateClassFiles(projFolder, projectName, types);
        File.WriteAllText(shprojName, CsProj.GenerateShProj(projectName, SHARED_GUID));
        File.WriteAllText(projitemsName, CsProj.GenerateProjItems(projectName, types, SHARED_GUID));
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