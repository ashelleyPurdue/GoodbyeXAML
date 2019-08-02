using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using CodeGenerator;

public static class CodeGen
{
    /// <summary>
    /// Generates a shared project containing extension methods for all
    /// the given types, and their subtypes
    /// </summary>
    /// <param name="outputFolder">Where the generated files should be placed.</param>
    /// <param name="projectName"></param>
    /// <param name="sharedGuid">A GUID for the generated .shproj file.  Visual Studio INSISTS on .shproj's having GUIDs.</param>
    /// <param name="types">All types that you want extension methods generated for.  Derived types will automatically be roped in, too.</param>
    public static void GenerateSharedProject(string outputFolder, string projectName, string sharedGuid, params Type[] types)
    {
        string projFolder = Path.Combine(outputFolder, projectName);
        string shprojName = Path.Combine(projFolder, projectName + ".shproj");
        string projitemsName = Path.Combine(projFolder, projectName + ".projitems");

        var derivedTypes = types
            .SelectMany(t => GetDerivedTypes(t))
            .Where(t => t.IsPublic)
            .Where(t => !t.IsGenericType)
            .Distinct()
            .OrderBy(t => t.FullName);

        GenerateClassFiles(projFolder, projectName, derivedTypes);
        File.WriteAllText(shprojName, GenerateShProj(projectName, sharedGuid));
        File.WriteAllText(projitemsName, GenerateProjItems(projectName, derivedTypes, sharedGuid));
    }

    public static IEnumerable<Type> GetDerivedTypes(Type type) => type
        .Assembly
        .GetTypes()
        .Where(t => t == type || t.IsSubclassOf(type));

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
            .Where(p => p.CanWrite && p.SetMethod.IsPublic)
            .Where(p => p.GetIndexParameters().Length == 0);    // Skip indexers

        var events = T
            .GetEvents()
            .Where(e => e.DeclaringType == T);

        var generator = new ClassGenerator(namespaceName, T);

        foreach (PropertyInfo p in settableProperties)
            generator.AddProperty(p);

        foreach (EventInfo e in events)
            generator.AddEvent(e);

        return generator.Generate();
    }

    public static string GenerateShProj(string namespaceName, string sharedGuid) =>
        $@"
            <Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
                <PropertyGroup Label=""Globals"">
                    <ProjectGuid>{sharedGuid}</ProjectGuid>
                    <MinimumVisualStudioVersion>14.0</MinimumVisualStudioVersion>
                </PropertyGroup>
                <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
                <Import Project=""$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\CodeSharing\Microsoft.CodeSharing.Common.Default.props"" />
                <Import Project=""$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\CodeSharing\Microsoft.CodeSharing.Common.props"" />
                <PropertyGroup />
                <Import Project=""{namespaceName}.projitems"" Label=""Shared"" />
                <Import Project=""$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\CodeSharing\Microsoft.CodeSharing.CSharp.targets"" />
            </Project>
        ";

    public static string GenerateProjItems(string namespaceName, IEnumerable<Type> types, string sharedGuid)
        {
            return 
            $@"
                <Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
                    <PropertyGroup>
                        <MSBuildAllProjects>$(MSBuildAllProjects);$(MSBuildThisFileFullPath)</MSBuildAllProjects>
                        <HasSharedItems>true</HasSharedItems>
                        <SharedGUID>{sharedGuid}</SharedGUID>
                    </PropertyGroup>
                    <PropertyGroup Label=""Configuration"">
                        <Import_RootNamespace>{namespaceName}</Import_RootNamespace>
                    </PropertyGroup>
                    <ItemGroup>
                        {GenerateCompileIncludes()}
                    </ItemGroup>
                </Project>
            ";

            string GenerateCompileIncludes()
            {
                var builder = new StringBuilder();
                foreach (Type t in types)
                    builder.AppendLine($@"<Compile Include=""$(MSBuildThisFileDirectory){t.Name}Extensions.cs"" />");

                return builder.ToString();
            }
        }

}