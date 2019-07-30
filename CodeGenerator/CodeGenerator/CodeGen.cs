using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using CodeGenerator;

public static class CodeGen
{
    public static void GenerateSharedProject(string outputFolder, string projectName, string sharedGuid, IEnumerable<Type> types)
    {
        string projFolder = Path.Combine(outputFolder, projectName);
        string shprojName = Path.Combine(projFolder, projectName + ".shproj");
        string projitemsName = Path.Combine(projFolder, projectName + ".projitems");
        
        GenerateClassFiles(projFolder, projectName, types);
        File.WriteAllText(shprojName, GenerateShProj(projectName, sharedGuid));
        File.WriteAllText(projitemsName, GenerateProjItems(projectName, types, sharedGuid));
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
            .Where(p => p.CanWrite && p.SetMethod.IsPublic)
            .Where(p => p.GetIndexParameters().Length == 0);    // Skip indexers

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