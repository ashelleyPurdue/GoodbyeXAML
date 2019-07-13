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
        string csprojName = Path.Combine(outputFolder, projectName, projectName + ".csproj");
        string namespaceName = projectName;

        GenerateClassFiles(projFolder, namespaceName, types);
        File.WriteAllText(csprojName, GenerateCSProj());

        string GenerateCSProj() =>
        $@"
            <Project Sdk=""Microsoft.NET.Sdk.WindowsDesktop"">
                <PropertyGroup>
                    <OutputType>WinExe</OutputType>
                    <TargetFramework>netcoreapp3.0</TargetFramework>
                    <RootNamespace>{namespaceName}</RootNamespace>
                    <UseWPF>true</UseWPF>
                 </PropertyGroup>
            </Project>
        ";
    }

    public static void GenerateWPFDotnetFrameworkProject(string outputFolder, string projectName, IEnumerable<Type> types)
    {
        // TODO: Refactor this duplicated code.
        string projFolder = Path.Combine(outputFolder, projectName);
        string csprojName = Path.Combine(outputFolder, projectName, projectName + ".csproj");
        string namespaceName = projectName;
        string assemblyName = projectName;
        const string frameworkVersion = "v4.8";

        GenerateClassFiles(projFolder, namespaceName, types);
        File.WriteAllText(csprojName, GenerateCSProj());

        string GenerateCSProj() =>
        $@"
            <Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
                <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
                <PropertyGroup>
                    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
                    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
                    <OutputType>library</OutputType>
                    <RootNamespace>{namespaceName}</RootNamespace>
                    <AssemblyName>{assemblyName}</AssemblyName>
                    <TargetFrameworkVersion>{frameworkVersion}</TargetFrameworkVersion>
                    <FileAlignment>512</FileAlignment>
                    <ProjectTypeGuids>{{60dc8134-eba5-43b8-bcc9-bb4bc16c2548}};{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}</ProjectTypeGuids>
                    <WarningLevel>4</WarningLevel>
                    <Deterministic>true</Deterministic>
                </PropertyGroup>
                <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
                    <DebugSymbols>true</DebugSymbols>
                    <DebugType>full</DebugType>
                    <Optimize>false</Optimize>
                    <OutputPath>bin\Debug\</OutputPath>
                    <DefineConstants>DEBUG;TRACE</DefineConstants>
                    <ErrorReport>prompt</ErrorReport>
                    <WarningLevel>4</WarningLevel>
                </PropertyGroup>
                <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
                    <DebugType>pdbonly</DebugType>
                    <Optimize>true</Optimize>
                    <OutputPath>bin\Release\</OutputPath>
                    <DefineConstants>TRACE</DefineConstants>
                    <ErrorReport>prompt</ErrorReport>
                    <WarningLevel>4</WarningLevel>
                </PropertyGroup>
                <ItemGroup>
                    <Reference Include=""System"" />
                    <Reference Include=""System.Data"" />
                    <Reference Include=""System.Xml"" />
                    <Reference Include=""Microsoft.CSharp"" />
                    <Reference Include=""System.Core"" />
                    <Reference Include=""System.Xml.Linq"" />
                    <Reference Include=""System.Data.DataSetExtensions"" />
                    <Reference Include=""System.Net.Http"" />
                    <Reference Include=""System.Xaml"">
                        <RequiredTargetFramework>4.0</RequiredTargetFramework>
                    </Reference>
                    <Reference Include=""WindowsBase"" />
                    <Reference Include=""PresentationCore"" />
                    <Reference Include=""PresentationFramework"" />
                </ItemGroup>
                <ItemGroup>
                    {GenerateCompileIncludes()}
                </ItemGroup>
                <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
            </Project>
        ";

        string GenerateCompileIncludes()
        {
            var builder = new StringBuilder();
            foreach (Type t in types)
                builder.AppendLine($@"<Compile Include=""{t.Name}Extensions.cs"" />");

            return builder.ToString();
        }
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
        var namespaces = new HashSet<string>();
        namespaces.Add(T.Namespace);

        // GeneratePropertyExtensions and GenerateEventExtenions both update "namespaces"
        // as a side-effect.
        string classBody = GeneratePropertyExtensions() + GenerateEventExtensions();
        string usingsSection = GenerateUsingsSection();

        return 
        $@"
            {usingsSection}

            namespace {namespaceName}
            {{
                public static class {T.Name}Extensions
                {{
                    {classBody}
                }}
            }}
        ";

        string GeneratePropertyExtensions()
        {
            var builder = new StringBuilder();
            var settableProperties = T
                .GetProperties()
                .Where(p => p.DeclaringType == T)   // Skip properties added by parent class
                .Where(p => p.CanWrite && p.SetMethod.IsPublic);

            foreach (PropertyInfo p in settableProperties)
            {
                namespaces.AddRange(p.PropertyType.AllReferencedNamespaces());
                builder.AppendLine(GenerateSinglePropertyExtension(p));
            }

            return builder.ToString();
        }

        string GenerateEventExtensions()
        {
            var builder = new StringBuilder();
            var events = T
                .GetEvents()
                .Where(e => e.DeclaringType == T);

            foreach (EventInfo e in events)
            {
                namespaces.AddRange(e.EventHandlerType.AllReferencedNamespaces());
                builder.AppendLine(GenerateSingleEventExtension(e));
            }

            return builder.ToString();
        }

        string GenerateUsingsSection()
        {
            var builder = new StringBuilder();
            var sortedNamespaces = namespaces
                .OrderBy(s => s);

            foreach (string ns in sortedNamespaces)
                builder.AppendLine($"using {ns};");

            return builder.ToString();
        }

        string GenerateSinglePropertyExtension(PropertyInfo p) =>
        $@"
            public static {FunctionSignature($"With{p.Name}", p.PropertyType, "value")}
            {{
                obj.{p.Name} = value;
                return obj;
            }}
        ";

        string GenerateSingleEventExtension(EventInfo e) =>
        $@"
            public static {FunctionSignature($"Handle{e.Name}", e.EventHandlerType, "handler")}
            {{
                obj.{e.Name} += handler;
                return obj;
            }}
        ";

        string FunctionSignature(string funcName, Type paramType, string paramName) => IsValidGenericConstraint()
            ? $"TObject {funcName}<TObject>(this TObject obj, {paramType.GenericName()} {paramName}) where TObject : {T.Name}"
            : $"{T.Name} {funcName}(this {T.Name} obj, {paramType.GenericName()} {paramName})";

        bool IsValidGenericConstraint() =>
            (T.IsInterface) ||
            (T.IsGenericTypeParameter) ||
            (!T.IsSealed);
    }

}