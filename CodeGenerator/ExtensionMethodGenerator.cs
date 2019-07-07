using System;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CodeGenerator
{
    public static class ExtensionMethodGenerator
    {
        public static string GenerateExtensionClassFor(Type T) => 
        $@"
            public static class {T.Name}Extensions
            {{
                {GeneratePropertyExtensions(T)}
                {GenerateEventExtensions(T)}
            }}
        ";

        private static string GeneratePropertyExtensions(Type T)
        {
            var builder = new StringBuilder();
            var settableProperties = T
                .GetProperties()
                .Where(p => p.CanWrite && p.SetMethod.IsPublic);

            foreach (PropertyInfo p in settableProperties)
                builder.AppendLine(GenerateSinglePropertyExtension(T, p));

            return builder.ToString();
        }

        private static string GenerateEventExtensions(Type T) => "";

        private static string GenerateSinglePropertyExtension(Type T, PropertyInfo p) =>
        $@"
            public static TObject With{p.Name}<TObject>(this TObject obj, {p.PropertyType.Name} value)
                where TObject : {T.Name}
            {{
                obj.{p.Name} = value;
                return obj;
            }}
        ";
    }
}
