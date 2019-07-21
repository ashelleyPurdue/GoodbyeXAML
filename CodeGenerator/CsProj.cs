using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGenerator
{
    public static class CsProj
    {
        public const string NET_CORE_VERSION = "3.0";
        public const string NET_FRAMEWORK_VERSION = "4.8";
        public const string NET_STANDARD_VERSION = "2.0";

        public static string GenerateNetCore(string namespaceName) =>
        $@"
            <Project Sdk=""Microsoft.NET.Sdk.WindowsDesktop"">
                <PropertyGroup>
                    <OutputType>Library</OutputType>
                    <TargetFramework>netcoreapp{NET_CORE_VERSION}</TargetFramework>
                    <RootNamespace>{namespaceName}</RootNamespace>
                    <UseWPF>true</UseWPF>
                 </PropertyGroup>

                <Import Project=""../../LambdaBinding/LambdaBinding.projitems"" Label=""Shared"" />
            </Project>
        ";

        public static string GenerateNetFramework(string namespaceName, IEnumerable<Type> types)
        {
            return
            $@"
                <Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
                    <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
                    <PropertyGroup>
                        <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
                        <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
                        <OutputType>library</OutputType>
                        <RootNamespace>{namespaceName}</RootNamespace>
                        <AssemblyName>{namespaceName}</AssemblyName>
                        <TargetFrameworkVersion>v{NET_FRAMEWORK_VERSION}</TargetFrameworkVersion>
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
                    <Import Project=""../../LambdaBinding/LambdaBinding.projitems"" Label=""Shared"" />
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
    }
}
