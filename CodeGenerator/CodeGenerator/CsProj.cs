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
}
