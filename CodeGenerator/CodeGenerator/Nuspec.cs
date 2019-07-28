using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGenerator
{
    public static class Nuspec
    {
        public const string VERSION = "0.0.1";
        public const string AUTHOR = "Alex Shelley";

        public static string Generate(string projectName) =>
        $@"
            <package>
                <metadata>
                    <id>{projectName}</id>
                    <version>{VERSION}</version>
                    <title>{projectName}</title>
                    <authors>{AUTHOR}</authors>
                    <owners>{AUTHOR}</owners>
                    <licenseUrl>https://opensource.org/licenses/MIT</licenseUrl>
                    <projectUrl>https://github.com/ashelleyPurdue/GoodbyeXAML</projectUrl>
                    <requireLicenseAcceptance>false</requireLicenseAcceptance>
                    <description>foo description</description>
                    <copyright>Copyright {DateTime.Now.Year}</copyright>
                </metadata>
            </package>
        ";
    }
}
